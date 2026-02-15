using MavLinkSharp;

namespace GCS.Core.Mavlink.Dispatch;

public interface IMavlinkMessageHandler
{
    /// <summary>
    /// MAVLink message id this handler is responsible for.
    /// </summary>
    uint MessageId { get; }

    /// <summary>
    /// Handle parsed MAVLink frame.
    /// </summary>
    void Handle(Frame frame);
}
