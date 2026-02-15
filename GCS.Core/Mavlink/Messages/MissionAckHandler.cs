using GCS.Core.Mavlink.Dispatch;
using MavLinkSharp;
using System;

namespace GCS.Core.Mavlink.Messages;

/// <summary>
/// Handles MISSION_ACK (msg 47) - Vehicle acknowledges mission transfer.
/// </summary>
public sealed class MissionAckHandler : IMavlinkMessageHandler
{
    public uint MessageId => 47;

    private readonly Action<byte> _onAck;

    public MissionAckHandler(Action<byte> onAck)
    {
        _onAck = onAck;
    }

    public void Handle(Frame frame)
    {
        try
        {
            byte result = Convert.ToByte(frame.Fields["type"]);
            System.Diagnostics.Debug.WriteLine($"[MissionAckHandler] ACK result: {result} ({(result == 0 ? "OK" : "ERROR")})");
            _onAck(result);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[MissionAckHandler] Error: {ex.Message}");
        }
    }
}
