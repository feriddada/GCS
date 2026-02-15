namespace GCS.Core.Domain;

public sealed record ConnectionState(
    bool IsConnected,
    byte SystemId,
    byte ComponentId,
    DateTime LastHeartbeatUtc
) : TimestampedState(LastHeartbeatUtc);
