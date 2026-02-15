using GCS.Core.Mavlink.Dispatch;
using MavLinkSharp;
using System;

namespace GCS.Core.Mavlink.Messages;

/// <summary>
/// Handles MISSION_COUNT (msg 44) - Vehicle tells us how many mission items it has.
/// </summary>
public sealed class MissionCountHandler : IMavlinkMessageHandler
{
    public uint MessageId => 44;

    private readonly Action<ushort> _onCount;

    public MissionCountHandler(Action<ushort> onCount)
    {
        _onCount = onCount;
    }

    public void Handle(Frame frame)
    {
        try
        {
            ushort count = Convert.ToUInt16(frame.Fields["count"]);
            System.Diagnostics.Debug.WriteLine($"[MissionCountHandler] Received count: {count}");
            _onCount(count);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[MissionCountHandler] Error: {ex.Message}");
        }
    }
}
