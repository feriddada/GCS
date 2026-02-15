namespace GCS.Core.Domain;

public sealed record HealthState(
    bool LinkAlive,
    bool AttitudeFresh,
    bool PositionFresh,
    DateTime EvaluatedAtUtc
);
