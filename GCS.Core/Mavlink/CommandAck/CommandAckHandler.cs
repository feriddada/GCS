using GCS.Core.Mavlink.Dispatch;
using MavLinkSharp;

namespace GCS.Core.Mavlink.CommandAck;

public sealed class CommandAckHandler : IMavlinkMessageHandler
{
    public uint MessageId => 77; // COMMAND_ACK

    private readonly CommandAckTracker _tracker;

    public CommandAckHandler(CommandAckTracker tracker)
    {
        _tracker = tracker;
    }

    public void Handle(Frame frame)
    {
        ushort command =
            Convert.ToUInt16(frame.Fields["command"]);
        byte result =
            Convert.ToByte(frame.Fields["result"]);

        _tracker.OnAck(
            command,
            frame.SystemId,
            frame.ComponentId,
            result
        );

    }
}
