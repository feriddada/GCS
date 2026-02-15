using GCS.Core.Mavlink;
using GCS.Core.Mavlink.Dispatch;
using MavLinkSharp;
using System;

namespace GCS.Core.Mavlink.Messages;

/// <summary>
/// Handles RC_CHANNELS (message ID 65) from ArduPilot
/// </summary>
public class RcChannelsHandler : IMavlinkMessageHandler
{
    private readonly Action<RcChannelsData> _onRcChannels;

    public RcChannelsHandler(Action<RcChannelsData> onRcChannels)
    {
        _onRcChannels = onRcChannels ?? throw new ArgumentNullException(nameof(onRcChannels));
    }

    public uint MessageId => 65; // RC_CHANNELS

    public void Handle(Frame frame)
    {
        try
        {
            var data = new RcChannelsData
            {
                TimeBootMs = GetUInt32(frame, "time_boot_ms"),
                Chancount = GetByte(frame, "chancount"),
                Chan1Raw = GetUInt16(frame, "chan1_raw"),
                Chan2Raw = GetUInt16(frame, "chan2_raw"),
                Chan3Raw = GetUInt16(frame, "chan3_raw"),
                Chan4Raw = GetUInt16(frame, "chan4_raw"),
                Chan5Raw = GetUInt16(frame, "chan5_raw"),
                Chan6Raw = GetUInt16(frame, "chan6_raw"),
                Chan7Raw = GetUInt16(frame, "chan7_raw"),
                Chan8Raw = GetUInt16(frame, "chan8_raw"),
                Chan9Raw = GetUInt16(frame, "chan9_raw"),
                Chan10Raw = GetUInt16(frame, "chan10_raw"),
                Chan11Raw = GetUInt16(frame, "chan11_raw"),
                Chan12Raw = GetUInt16(frame, "chan12_raw"),
                Chan13Raw = GetUInt16(frame, "chan13_raw"),
                Chan14Raw = GetUInt16(frame, "chan14_raw"),
                Chan15Raw = GetUInt16(frame, "chan15_raw"),
                Chan16Raw = GetUInt16(frame, "chan16_raw"),
                Chan17Raw = GetUInt16(frame, "chan17_raw"),
                Chan18Raw = GetUInt16(frame, "chan18_raw"),
                Rssi = GetByte(frame, "rssi")
            };

            _onRcChannels(data);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"RC_CHANNELS parse error: {ex.Message}");
        }
    }

    private uint GetUInt32(Frame frame, string field)
    {
        return frame.Fields.TryGetValue(field, out var value) && value is uint u ? u : 0;
    }

    private ushort GetUInt16(Frame frame, string field)
    {
        if (frame.Fields.TryGetValue(field, out var value))
            return ToUInt16(value);

        return 0;
    }

    private static ushort ToUInt16(object value) => value switch
    {
        byte b => b,
        ushort u => u,
        short s => (ushort)s,
        int i => (ushort)i,
        uint ui => (ushort)ui,
        _ => 0
    };

    private byte GetByte(Frame frame, string field)
    {
        if (!frame.Fields.TryGetValue(field, out var value))
            return 0;

        return value switch
        {
            byte b => b,
            int i => (byte)i,
            uint ui => (byte)ui,
            _ => 0
        };
    }

}

/// <summary>
/// RC Channels data from RC_CHANNELS message
/// </summary>
public record RcChannelsData
{
    public uint TimeBootMs { get; init; }
    public byte Chancount { get; init; }
    public ushort Chan1Raw { get; init; }
    public ushort Chan2Raw { get; init; }
    public ushort Chan3Raw { get; init; }
    public ushort Chan4Raw { get; init; }
    public ushort Chan5Raw { get; init; }
    public ushort Chan6Raw { get; init; }
    public ushort Chan7Raw { get; init; }
    public ushort Chan8Raw { get; init; }
    public ushort Chan9Raw { get; init; }
    public ushort Chan10Raw { get; init; }
    public ushort Chan11Raw { get; init; }
    public ushort Chan12Raw { get; init; }
    public ushort Chan13Raw { get; init; }
    public ushort Chan14Raw { get; init; }
    public ushort Chan15Raw { get; init; }
    public ushort Chan16Raw { get; init; }
    public ushort Chan17Raw { get; init; }
    public ushort Chan18Raw { get; init; }
    public byte Rssi { get; init; }

    public ushort[] ToArray() => new[]
    {
        Chan1Raw, Chan2Raw, Chan3Raw, Chan4Raw,
        Chan5Raw, Chan6Raw, Chan7Raw, Chan8Raw,
        Chan9Raw, Chan10Raw, Chan11Raw, Chan12Raw,
        Chan13Raw, Chan14Raw, Chan15Raw, Chan16Raw,
        Chan17Raw, Chan18Raw
    };
}
