namespace GCS.Core.Domain;

public record HeartbeatState(
    byte SystemId,
    byte ComponentId,
    FlightMode? Mode,
    bool IsArmed,
    DateTime TimestampUtc
);