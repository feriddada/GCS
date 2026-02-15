namespace GCS.Core.Mavlink.Connection;

public sealed record MavlinkConnectionState(
    bool IsConnected,
    byte SystemId,
    byte ComponentId,
    DateTime LastHeartbeatUtc
);
