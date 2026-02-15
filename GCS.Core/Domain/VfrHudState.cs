namespace GCS.Core.Domain;

public sealed record VfrHudState(
    float AirspeedMps,
    float GroundspeedMps,
    float HeadingDeg,
    float ClimbMps,
    DateTime TimestampUtc
) : TimestampedState(TimestampUtc);
