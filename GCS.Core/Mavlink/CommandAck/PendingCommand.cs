namespace GCS.Core.Mavlink.CommandAck;

internal sealed class PendingCommand
{
    public ushort CommandId { get; }
    public DateTime SentAtUtc { get; }
    public TaskCompletionSource<CommandAckResult> Tcs { get; }

    public PendingCommand(ushort commandId)
    {
        CommandId = commandId;
        SentAtUtc = DateTime.UtcNow;
        Tcs = new TaskCompletionSource<CommandAckResult>(
            TaskCreationOptions.RunContinuationsAsynchronously
        );
    }
}
