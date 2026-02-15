using System;

namespace GCS.Core.Mavlink.Connection;

public sealed class MavlinkConnectionTracker
{
    private readonly TimeSpan _timeout;

    private byte? _systemId;
    private byte? _componentId;
    private DateTime _lastHeartbeat;

    public bool IsConnected { get; private set; }

    public event Action<MavlinkConnectionState>? ConnectionChanged;

    public byte SystemId => _systemId ?? 0;
    public byte ComponentId => _componentId ?? 0;

    public MavlinkConnectionTracker(TimeSpan timeout)
    {
        _timeout = timeout;
    }

    public void OnHeartbeat(byte systemId, byte componentId, DateTime timestampUtc)
    {
        // Check for changes BEFORE updating state
        bool isFirst = _systemId == null;
        bool vehicleChanged = !isFirst && (_systemId != systemId || _componentId != componentId);

        // Now update state
        _systemId = systemId;
        _componentId = componentId;
        _lastHeartbeat = timestampUtc;

        if (!IsConnected)
        {
            // First connection
            IsConnected = true;
            Raise();
        }
        else if (vehicleChanged)
        {
            // Different vehicle connected mid-session
            Raise();
        }
        // else: same vehicle, already connected - no event needed
    }

    public void Tick(DateTime nowUtc)
    {
        if (!IsConnected)
            return;

        if (nowUtc - _lastHeartbeat > _timeout)
        {
            IsConnected = false;
            Raise();
        }
    }

    /// <summary>
    /// Reset tracker to disconnected state.
    /// </summary>
    public void Reset()
    {
        if (IsConnected)
        {
            IsConnected = false;
            Raise();
        }

        _systemId = null;
        _componentId = null;
    }

    private void Raise()
    {
        ConnectionChanged?.Invoke(
            new MavlinkConnectionState(
                IsConnected,
                _systemId ?? 0,
                _componentId ?? 0,
                _lastHeartbeat
            )
        );
    }
}