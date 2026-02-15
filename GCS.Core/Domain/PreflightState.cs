namespace GCS.Core.Domain;

public sealed record PreflightState(
    IReadOnlyList<PreflightCheckResult> Checks,
    DateTime TimestampUtc
);
