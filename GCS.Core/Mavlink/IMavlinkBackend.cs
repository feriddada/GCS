using GCS.Core.Domain;
using GCS.Core.Mavlink.Messages;

namespace GCS.Core.Mavlink;

public interface IMavlinkBackend : IDisposable
{
    // ═══════════════════════════════════════════════════════════════
    // RX Events - Telemetry
    // ═══════════════════════════════════════════════════════════════

    event Action<HeartbeatState>? HeartbeatReceived;
    event Action<AttitudeState>? AttitudeReceived;
    event Action<PositionState>? PositionReceived;
    event Action<VfrHudState>? VfrHudReceived;
    event Action<BatteryState>? BatteryReceived;
    event Action<RcChannelsData>? RcChannelsReceived;

    // ═══════════════════════════════════════════════════════════════
    // RX Events - Messages & Acks
    // ═══════════════════════════════════════════════════════════════

    event Action<AutopilotMessage>? AutopilotMessageReceived;
    event Action<ushort, byte>? CommandAckReceived;  // command, result

    // ═══════════════════════════════════════════════════════════════
    // RX Events - Mission Protocol
    // ═══════════════════════════════════════════════════════════════

    event Action<ushort>? MissionCountReceived;      // count
    event Action<MissionItem>? MissionItemReceived;
    event Action<ushort>? MissionRequestReceived;    // sequence
    event Action<byte>? MissionAckReceived;          // result

    // ═══════════════════════════════════════════════════════════════
    // Connection State Events
    // ═══════════════════════════════════════════════════════════════

    event Action<ConnectionState>? ConnectionStateChanged;
    event Action<TransportState>? TransportStateChanged;

    // ═══════════════════════════════════════════════════════════════
    // Connection Info (read-only)
    // ═══════════════════════════════════════════════════════════════

    bool IsConnected { get; }
    byte SystemId { get; }
    byte ComponentId { get; }

    // ═══════════════════════════════════════════════════════════════
    // Lifecycle
    // ═══════════════════════════════════════════════════════════════

    Task StartAsync(CancellationToken cancellationToken);
    Task StopAsync();

    // ═══════════════════════════════════════════════════════════════
    // TX Methods - Commands
    // ═══════════════════════════════════════════════════════════════

    /// <summary>
    /// Send COMMAND_LONG (msg 76) to the connected vehicle.
    /// </summary>
    Task SendCommandLongAsync(
        ushort command,
        float param1 = 0, float param2 = 0, float param3 = 0, float param4 = 0,
        float param5 = 0, float param6 = 0, float param7 = 0,
        byte confirmation = 0,
        CancellationToken ct = default);

    /// <summary>
    /// Send SET_MODE (msg 11) to the connected vehicle.
    /// </summary>
    Task SendSetModeAsync(
        byte baseMode,
        uint customMode,
        CancellationToken ct = default);

    /// <summary>
    /// Send ARM/DISARM command to the vehicle.
    /// </summary>
    Task SendArmDisarmAsync(
        bool arm,
        CancellationToken ct = default);

    // ═══════════════════════════════════════════════════════════════
    // TX Methods - Raw Packet
    // ═══════════════════════════════════════════════════════════════

    /// <summary>
    /// Send a raw MAVLink packet (for mission protocol, etc.)
    /// </summary>
    Task SendRawAsync(
        ReadOnlyMemory<byte> packet,
        CancellationToken ct = default);
}