// GCS.Core.Health
public sealed record HealthPolicy(
    TimeSpan HeartbeatTimeout,
    TimeSpan AttitudeStaleAfter,
    TimeSpan PositionStaleAfter
);
