namespace GCS.Core.Domain;

public sealed record PreflightCheckResult(
    string Name,
    PreflightCheckStatus Status,
    string? Reason
);
