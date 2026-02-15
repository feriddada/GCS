namespace GCS.Core.Domain;

public enum AutopilotMessageSeverity
{
    Info,
    Warning,
    Error,
    Critical
}

public sealed record AutopilotMessage(
    AutopilotMessageSeverity Severity,
    string Text,
    DateTime TimestampUtc
);
