using GCS.Core.Mavlink.Dispatch;
using MavLinkSharp;
using System;
using System.Diagnostics;

namespace GCS.Core.Mavlink.Messages;

/// <summary>
/// Handles MISSION_REQUEST (msg 40) - Vehicle requests a mission item during upload.
/// Some older ArduPilot versions use this instead of MISSION_REQUEST_INT (51).
/// </summary>
public sealed class MissionRequestHandler : IMavlinkMessageHandler
{
    public uint MessageId => 40;

    private readonly Action<ushort> _onRequest;

    public MissionRequestHandler(Action<ushort> onRequest)
    {
        _onRequest = onRequest;
        Debug.WriteLine("[MissionRequestHandler] Handler registered for msg 40");
    }

    public void Handle(Frame frame)
    {
        Debug.WriteLine($"[MissionRequestHandler] === MSG 40 RECEIVED ===");

        try
        {
            ushort seq = Convert.ToUInt16(frame.Fields["seq"]);
            Debug.WriteLine($"[MissionRequestHandler] Vehicle requesting item {seq}");
            _onRequest(seq);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[MissionRequestHandler] Error: {ex.Message}");
        }
    }
}