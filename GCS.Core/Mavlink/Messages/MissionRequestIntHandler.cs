using GCS.Core.Mavlink.Dispatch;
using MavLinkSharp;
using System;
using System.Diagnostics;

namespace GCS.Core.Mavlink.Messages;

/// <summary>
/// Handles MISSION_REQUEST_INT (msg 51) - Vehicle requests a mission item during upload.
/// </summary>
public sealed class MissionRequestIntHandler : IMavlinkMessageHandler
{
    public uint MessageId => 51;

    private readonly Action<ushort> _onRequest;

    public MissionRequestIntHandler(Action<ushort> onRequest)
    {
        _onRequest = onRequest;
        Debug.WriteLine("[MissionRequestIntHandler] Handler registered for msg 51");
    }

    public void Handle(Frame frame)
    {
        Debug.WriteLine($"[MissionRequestIntHandler] === MSG 51 RECEIVED ===");

        try
        {
            Debug.WriteLine($"[MissionRequestIntHandler] Fields: {string.Join(", ", frame.Fields.Keys)}");

            ushort seq = Convert.ToUInt16(frame.Fields["seq"]);
            Debug.WriteLine($"[MissionRequestIntHandler] Vehicle requesting item {seq}");
            _onRequest(seq);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[MissionRequestIntHandler] Error: {ex.Message}");
        }
    }
}