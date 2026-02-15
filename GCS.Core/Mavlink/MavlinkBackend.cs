using GCS.Core.Domain;
using GCS.Core.Mavlink.CommandAck;
using GCS.Core.Mavlink.Connection;
using GCS.Core.Mavlink.Dispatch;
using GCS.Core.Mavlink.Messages;
using GCS.Core.Transport;
using MavLinkSharp;
using MavLinkSharp.Enums;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GCS.Core.Mavlink;

public sealed class MavlinkBackend : IMavlinkBackend
{
    // ═══════════════════════════════════════════════════════════════
    // Dependencies
    // ═══════════════════════════════════════════════════════════════

    private readonly ITransport _transport;
    private readonly MavlinkDispatcher _dispatcher;
    private readonly MavlinkConnectionTracker _connection;
    private readonly CommandAckTracker _commandAckTracker;
    private readonly MavlinkFrameBuffer _frameBuffer = new();
    // ═══════════════════════════════════════════════════════════════
    // State
    // ═══════════════════════════════════════════════════════════════

    private CancellationTokenSource? _cts;
    private Task? _tickTask;
    private TransportState _transportState = TransportState.Disconnected;
    private bool _disposed;

    // ═══════════════════════════════════════════════════════════════
    // Constants
    // ═══════════════════════════════════════════════════════════════

    // GCS identity written into outgoing packet headers
    private const byte GcsSysId = 255;
    private const byte GcsCompId = 190; // MAV_COMP_ID_MISSIONPLANNER

    // MAV_CMD constants
    private const ushort MAV_CMD_COMPONENT_ARM_DISARM = 400;

    // ═══════════════════════════════════════════════════════════════
    // Events - Telemetry
    // ═══════════════════════════════════════════════════════════════

    public event Action<HeartbeatState>? HeartbeatReceived;
    public event Action<AttitudeState>? AttitudeReceived;
    public event Action<PositionState>? PositionReceived;
    public event Action<VfrHudState>? VfrHudReceived;
    public event Action<BatteryState>? BatteryReceived;
    public event Action<RcChannelsData>? RcChannelsReceived;

    // ═══════════════════════════════════════════════════════════════
    // Events - Messages & Acks
    // ═══════════════════════════════════════════════════════════════

    public event Action<AutopilotMessage>? AutopilotMessageReceived;
    public event Action<ushort, byte>? CommandAckReceived;

    // ═══════════════════════════════════════════════════════════════
    // Events - Mission Protocol
    // ═══════════════════════════════════════════════════════════════

    public event Action<ushort>? MissionCountReceived;
    public event Action<MissionItem>? MissionItemReceived;
    public event Action<ushort>? MissionRequestReceived;
    public event Action<byte>? MissionAckReceived;

    // ═══════════════════════════════════════════════════════════════
    // Events - Connection State
    // ═══════════════════════════════════════════════════════════════

    public event Action<ConnectionState>? ConnectionStateChanged;
    public event Action<TransportState>? TransportStateChanged;

    // ═══════════════════════════════════════════════════════════════
    // Properties
    // ═══════════════════════════════════════════════════════════════

    public bool IsConnected => _connection.IsConnected;
    public byte SystemId => _connection.SystemId;
    public byte ComponentId => _connection.ComponentId;

    // ═══════════════════════════════════════════════════════════════
    // Constructor
    // ═══════════════════════════════════════════════════════════════

    public MavlinkBackend(ITransport transport)
    {
        _transport = transport;

        // Wire up transport events
        _transport.DataReceived += OnDataReceived;
        _transport.TransportError += OnTransportError;

        // Create connection tracker
        _connection = new MavlinkConnectionTracker(TimeSpan.FromSeconds(3));
        _connection.ConnectionChanged += OnConnectionChanged;

        // Create command ack tracker
        _commandAckTracker = new CommandAckTracker();

        // Create dispatcher with all handlers
        _dispatcher = new MavlinkDispatcher(CreateHandlers());
    }

    private IMavlinkMessageHandler[] CreateHandlers()
    {
        return new IMavlinkMessageHandler[]
        {
            // Telemetry handlers
            new HeartbeatHandler(_connection, s => HeartbeatReceived?.Invoke(s)),
            new AttitudeHandler(s => AttitudeReceived?.Invoke(s)),
            new GlobalPositionHandler(s => PositionReceived?.Invoke(s)),
            new VfrHudHandler(s => VfrHudReceived?.Invoke(s)),
            new SysStatusHandler(s => BatteryReceived?.Invoke(s)),
            new RcChannelsHandler(s => RcChannelsReceived?.Invoke(s)),
            new MissionRequestHandler(seq => MissionRequestReceived?.Invoke(seq)),
            // Message handlers
            new StatustextHandler(s => AutopilotMessageReceived?.Invoke(s)),
            
            // Command ack handler
            new CommandAckHandler(_commandAckTracker),
            
            // Mission protocol handlers
            new MissionCountHandler(count => MissionCountReceived?.Invoke(count)),
            new MissionItemIntRxHandler(item => MissionItemReceived?.Invoke(item)),
            new MissionRequestIntHandler(seq => MissionRequestReceived?.Invoke(seq)),
            new MissionAckHandler(result => MissionAckReceived?.Invoke(result)),
        };
    }

    // ═══════════════════════════════════════════════════════════════
    // Lifecycle
    // ═══════════════════════════════════════════════════════════════

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(MavlinkBackend));

        _cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

        SetTransportState(TransportState.Connecting);

        try
        {
            await _transport.StartAsync(_cts.Token);
            SetTransportState(TransportState.Connected);
        }
        catch (Exception)
        {
            SetTransportState(TransportState.Error);
            throw;
        }

        // Start tick loop for connection timeout and command ack timeout
        _tickTask = Task.Run(() => TickLoop(_cts.Token), _cts.Token);
    }

    public async Task StopAsync()
    {
        if (_cts == null)
            return;

        _cts.Cancel();

        if (_tickTask != null)
        {
            try
            {
                await _tickTask;
            }
            catch (OperationCanceledException)
            {
                // Expected
            }
        }

        await _transport.StopAsync();

        _connection.Reset();

        _cts.Dispose();
        _cts = null;

        SetTransportState(TransportState.Disconnected);
    }

    // ═══════════════════════════════════════════════════════════════
    // RX - Data Processing
    // ═══════════════════════════════════════════════════════════════

    private void OnDataReceived(ReadOnlyMemory<byte> data)
    {
        foreach (var frameData in _frameBuffer.AddData(data.Span))
        {
            if (!Message.TryParse(frameData.Span, out var frame))
                continue;

            if (frame.ErrorReason != ErrorReason.None)
                continue;

            _dispatcher.Dispatch(frame);
        }
    }

    private void OnTransportError(Exception ex)
    {
        SetTransportState(TransportState.Error);
        System.Diagnostics.Debug.WriteLine($"[MavlinkBackend] Transport error: {ex.Message}");
    }

    private void OnConnectionChanged(MavlinkConnectionState state)
    {
        ConnectionStateChanged?.Invoke(
            new ConnectionState(
                state.IsConnected,
                state.SystemId,
                state.ComponentId,
                state.LastHeartbeatUtc
            )
        );
    }

    // ═══════════════════════════════════════════════════════════════
    // Tick Loop
    // ═══════════════════════════════════════════════════════════════

    private async Task TickLoop(CancellationToken token)
    {
        try
        {
            while (!token.IsCancellationRequested)
            {
                var now = DateTime.UtcNow;

                // Check connection timeout
                _connection.Tick(now);

                // Check command ack timeouts
                _commandAckTracker.Tick();

                await Task.Delay(200, token);
            }
        }
        catch (OperationCanceledException)
        {
            // Normal shutdown
        }
    }

    // ═══════════════════════════════════════════════════════════════
    // TX - Commands
    // ═══════════════════════════════════════════════════════════════

    public async Task SendCommandLongAsync(
        ushort command,
        float param1 = 0, float param2 = 0, float param3 = 0, float param4 = 0,
        float param5 = 0, float param6 = 0, float param7 = 0,
        byte confirmation = 0,
        CancellationToken ct = default)
    {
        EnsureConnected();

        var packet = Mavlink2Serializer.CommandLong(
            targetSys: _connection.SystemId,
            targetComp: _connection.ComponentId,
            senderSys: GcsSysId,
            senderComp: GcsCompId,
            command: command,
            confirmation: confirmation,
            p1: param1, p2: param2, p3: param3, p4: param4,
            p5: param5, p6: param6, p7: param7);

        await _transport.SendAsync(packet, ct);
    }

    public async Task SendSetModeAsync(
        byte baseMode,
        uint customMode,
        CancellationToken ct = default)
    {
        EnsureConnected();

        var packet = Mavlink2Serializer.SetMode(
            targetSys: _connection.SystemId,
            senderSys: GcsSysId,
            senderComp: GcsCompId,
            baseMode: baseMode,
            customMode: customMode);

        await _transport.SendAsync(packet, ct);
    }

    public async Task SendArmDisarmAsync(bool arm, CancellationToken ct = default)
    {
        await SendCommandLongAsync(
            command: MAV_CMD_COMPONENT_ARM_DISARM,
            param1: arm ? 1f : 0f,
            ct: ct);
    }

    public async Task SendRawAsync(ReadOnlyMemory<byte> packet, CancellationToken ct = default)
    {
        await _transport.SendAsync(packet, ct);
    }

    // ═══════════════════════════════════════════════════════════════
    // TX - With Acknowledgement
    // ═══════════════════════════════════════════════════════════════

    /// <summary>
    /// Send a command and wait for COMMAND_ACK response.
    /// </summary>
    public async Task<CommandAckResult> SendCommandWithAckAsync(
        ushort command,
        float param1 = 0, float param2 = 0, float param3 = 0, float param4 = 0,
        float param5 = 0, float param6 = 0, float param7 = 0,
        CancellationToken ct = default)
    {
        EnsureConnected();

        // Register for acknowledgement
        var ackTask = _commandAckTracker.Register(
            command,
            _connection.SystemId,
            _connection.ComponentId);

        // Send the command
        await SendCommandLongAsync(
            command, param1, param2, param3, param4, param5, param6, param7,
            confirmation: 0, ct: ct);

        // Wait for acknowledgement or timeout
        return await ackTask;
    }

    // ═══════════════════════════════════════════════════════════════
    // Helpers
    // ═══════════════════════════════════════════════════════════════

    private void EnsureConnected()
    {
        if (!_connection.IsConnected)
            throw new InvalidOperationException("Not connected to vehicle");
    }

    private void SetTransportState(TransportState state)
    {
        if (_transportState == state)
            return;

        _transportState = state;
        TransportStateChanged?.Invoke(state);
    }

    // ═══════════════════════════════════════════════════════════════
    // Disposal
    // ═══════════════════════════════════════════════════════════════

    public void Dispose()
    {
        if (_disposed)
            return;

        _disposed = true;

        _transport.DataReceived -= OnDataReceived;
        _transport.TransportError -= OnTransportError;
        _connection.ConnectionChanged -= OnConnectionChanged;

        _cts?.Cancel();
        _cts?.Dispose();

        _transport.Dispose();
    }
}