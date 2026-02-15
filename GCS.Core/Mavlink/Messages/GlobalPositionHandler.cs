using GCS.Core.Domain;
using GCS.Core.Mavlink.Dispatch;
using MavLinkSharp;
using System;
using System.Diagnostics;

namespace GCS.Core.Mavlink.Messages;

/// <summary>
/// Handles GLOBAL_POSITION_INT (msg 33) - GPS position data.
/// </summary>
public sealed class GlobalPositionHandler : IMavlinkMessageHandler
{
    public uint MessageId => 33;

    private readonly Action<PositionState> _onPosition;

    public GlobalPositionHandler(Action<PositionState> onPosition)
    {
        _onPosition = onPosition;
     
    }

    public void Handle(Frame frame)
    {
     

        try
        {


  

            int latE7 = Convert.ToInt32(frame.Fields["lat"]);
            int lonE7 = Convert.ToInt32(frame.Fields["lon"]);
            int altMm = Convert.ToInt32(frame.Fields["alt"]);
            int relAltMm = Convert.ToInt32(frame.Fields["relative_alt"]);
            short vx = Convert.ToInt16(frame.Fields["vx"]);
            short vy = Convert.ToInt16(frame.Fields["vy"]);
            short vz = Convert.ToInt16(frame.Fields["vz"]);
            ushort hdgCdeg = Convert.ToUInt16(frame.Fields["hdg"]);

            double lat = latE7 / 1e7;
            double lon = lonE7 / 1e7;

 

            var state = new PositionState(
                LatitudeDeg: lat,
                LongitudeDeg: lon,
                AltitudeMslMeters: (float)(altMm / 1000.0),
                AltitudeRelMeters: (float)(relAltMm / 1000.0),
                HeadingDeg: hdgCdeg / 100.0f,
                VelocityNorthMps: vx / 100.0f,
                VelocityEastMps: vy / 100.0f,
                VelocityDownMps: vz / 100.0f,
                TimestampUtc: DateTime.UtcNow
            );


            _onPosition(state);
 
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[GlobalPositionHandler] ERROR: {ex.Message}");
            Debug.WriteLine($"[GlobalPositionHandler] Stack: {ex.StackTrace}");
        }
    }
}