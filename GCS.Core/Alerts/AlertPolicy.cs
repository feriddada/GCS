using GCS.Core.Domain;
namespace GCS.Core.Alerts;

public sealed record AlertPolicy(
    AlertSeverity LinkLostSeverity,
    AlertSeverity AttitudeStaleSeverity,
    AlertSeverity PositionStaleSeverity
);
