using GCS.Core.Mavlink.Dispatch;
using MavLinkSharp;
using System;
using System.Text;

namespace GCS.Core.Mavlink.Messages;

/// <summary>
/// Handles PARAM_VALUE message (ID 22)
/// </summary>
public sealed class ParamValueHandler : IMavlinkMessageHandler
{
    public uint MessageId => 22;

    private readonly Action<string, float> _onParamValue;

    public ParamValueHandler(Action<string, float> onParamValue)
    {
        _onParamValue = onParamValue;
    }

    public void Handle(Frame frame)
    {
        var payload = frame.Payload;

        if (payload == null || payload.Length < 22)
            return;

        // PARAM_VALUE layout:
        // param_value: float (4 bytes) at offset 0
        // param_count: uint16 (2 bytes) at offset 4
        // param_index: uint16 (2 bytes) at offset 6  
        // param_id: char[16] (16 bytes) at offset 8
        // param_type: uint8 (1 byte) at offset 24

        float paramValue = BitConverter.ToSingle(payload, 0);

        // Extract param_id (16 bytes, null-terminated)
        int length = 0;
        for (int i = 8; i < 24 && payload[i] != 0; i++)
            length++;

        string paramId = Encoding.ASCII.GetString(payload, 8, length);

        System.Diagnostics.Debug.WriteLine($"[ParamValueHandler] {paramId} = {paramValue}");

        _onParamValue(paramId, paramValue);
    }
}