namespace GCS.Core.Mavlink.CommandAck;

public enum CommandAckResult
{
    Accepted,
    Rejected,
    Denied,
    Failed,
    InProgress,
    Timeout,
    Unsupported,
    Temporary
}
