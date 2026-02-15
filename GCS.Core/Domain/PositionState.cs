using System;

namespace GCS.Core.Domain;

/// <summary>
/// GPS position from GLOBAL_POSITION_INT (msg 33)
/// Add this to your Domain folder if missing
/// </summary>
public record PositionState(
    double LatitudeDeg,
    double LongitudeDeg,
    float AltitudeMslMeters,
    float AltitudeRelMeters,
    float HeadingDeg,
    float VelocityNorthMps,
    float VelocityEastMps,
    float VelocityDownMps,
    DateTime TimestampUtc
);