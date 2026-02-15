namespace GCS.Core.Domain;

public sealed record AlertState(
    AlertType Type,
    AlertSeverity Severity,
    bool Active,
    DateTime TimestampUtc
);
