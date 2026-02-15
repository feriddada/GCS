using GCS.Core.Domain;
using GCS.Core.Mavlink.Dispatch;
using MavLinkSharp;
using System;
using System.Diagnostics;

namespace GCS.Core.Mavlink.Messages;

/// <summary>
/// Handles MISSION_ITEM (msg 39) - Receives mission item from vehicle (legacy format).
/// Some ArduPilot versions send this instead of MISSION_ITEM_INT (73).
/// </summary>
public sealed class MissionItemRxHandler : IMavlinkMessageHandler
{
    public uint MessageId => 39;

    private readonly Action<MissionItem> _onItem;

    public MissionItemRxHandler(Action<MissionItem> onItem)
    {
        _onItem = onItem;
        Debug.WriteLine("[MissionItemRxHandler] Handler registered for msg 39");
    }

    public void Handle(Frame frame)
    {
        Debug.WriteLine($"[MissionItemRxHandler] === MSG 39 RECEIVED ===");

        try
        {
            ushort seq = Convert.ToUInt16(frame.Fields["seq"]);
            ushort command = Convert.ToUInt16(frame.Fields["command"]);
            byte frameType = Convert.ToByte(frame.Fields["frame"]);

            // MISSION_ITEM uses float lat/lon (not int like MISSION_ITEM_INT)
            float lat = Convert.ToSingle(frame.Fields["x"]);
            float lon = Convert.ToSingle(frame.Fields["y"]);
            float alt = Convert.ToSingle(frame.Fields["z"]);
            float param1 = Convert.ToSingle(frame.Fields["param1"]);
            float param2 = Convert.ToSingle(frame.Fields["param2"]);
            float param3 = Convert.ToSingle(frame.Fields["param3"]);
            float param4 = Convert.ToSingle(frame.Fields["param4"]);
            byte autoContinue = Convert.ToByte(frame.Fields["autocontinue"]);

            var item = new MissionItem(
                Sequence: seq,
                Command: command,
                LatitudeDeg: lat,
                LongitudeDeg: lon,
                AltitudeMeters: alt,
                Param1: param1,
                Param2: param2,
                Param3: param3,
                Param4: param4,
                Frame: frameType,
                AutoContinue: autoContinue != 0
            );

            Debug.WriteLine($"[MissionItemRxHandler] Item {seq}: cmd={command}, lat={lat:F6}, lon={lon:F6}, alt={alt}");
            _onItem(item);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[MissionItemRxHandler] Error: {ex.Message}");
        }
    }
}