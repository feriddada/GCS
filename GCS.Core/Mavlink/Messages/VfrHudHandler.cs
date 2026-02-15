using GCS.Core.Domain;
using GCS.Core.Mavlink.Dispatch;
using MavLinkSharp;
using System;

namespace GCS.Core.Mavlink.Messages;

public sealed class VfrHudHandler : IMavlinkMessageHandler
{
    public uint MessageId => 74; // VFR_HUD

    private readonly Action<VfrHudState> _onHud;

    public VfrHudHandler(Action<VfrHudState> onHud)
    {
        _onHud = onHud;
    }

    public void Handle(Frame frame)
    {
        float airspeed =
            Convert.ToSingle(frame.Fields["airspeed"]);
        float groundspeed =
            Convert.ToSingle(frame.Fields["groundspeed"]);
        short headingRaw =
            Convert.ToInt16(frame.Fields["heading"]);
        float climb =
            Convert.ToSingle(frame.Fields["climb"]);

        _onHud(
            new VfrHudState(
                AirspeedMps: airspeed,
                GroundspeedMps: groundspeed,
                HeadingDeg: headingRaw,
                ClimbMps: climb,
                TimestampUtc: DateTime.UtcNow
            )
        );
    }
}
