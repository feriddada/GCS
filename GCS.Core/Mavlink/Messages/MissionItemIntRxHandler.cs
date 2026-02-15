using GCS.Core.Domain;
using GCS.Core.Mavlink.Dispatch;
using MavLinkSharp;
using System;

namespace GCS.Core.Mavlink.Messages;

/// <summary>
/// Handles MISSION_ITEM_INT (msg 73) - Receives mission item from vehicle.
/// </summary>
public sealed class MissionItemIntRxHandler : IMavlinkMessageHandler
{
    public uint MessageId => 73;

    private readonly Action<MissionItem> _onItem;

    public MissionItemIntRxHandler(Action<MissionItem> onItem)
    {
        _onItem = onItem;
    }

    public void Handle(Frame frame)
    {
        try
        {
            ushort seq = Convert.ToUInt16(frame.Fields["seq"]);
            ushort command = Convert.ToUInt16(frame.Fields["command"]);
            byte frameType = Convert.ToByte(frame.Fields["frame"]);
            int latE7 = Convert.ToInt32(frame.Fields["x"]);
            int lonE7 = Convert.ToInt32(frame.Fields["y"]);
            float alt = Convert.ToSingle(frame.Fields["z"]);
            float param1 = Convert.ToSingle(frame.Fields["param1"]);
            float param2 = Convert.ToSingle(frame.Fields["param2"]);
            float param3 = Convert.ToSingle(frame.Fields["param3"]);
            float param4 = Convert.ToSingle(frame.Fields["param4"]);
            byte autoContinue = Convert.ToByte(frame.Fields["autocontinue"]);

            var item = new MissionItem(
                Sequence: seq,
                Command: command,
                LatitudeDeg: latE7 / 1e7,
                LongitudeDeg: lonE7 / 1e7,
                AltitudeMeters: alt,
                Param1: param1,
                Param2: param2,
                Param3: param3,
                Param4: param4,
                Frame: frameType,
                AutoContinue: autoContinue != 0
            );

            System.Diagnostics.Debug.WriteLine($"[MissionItemIntRxHandler] Item {seq}: cmd={command}, lat={item.LatitudeDeg:F6}, lon={item.LongitudeDeg:F6}, alt={alt}");
            _onItem(item);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[MissionItemIntRxHandler] Error: {ex.Message}");
        }
    }
}
