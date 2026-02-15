namespace GCS.Core.Domain;

public sealed record AttitudeState(
    float RollRad,
    float PitchRad,
    float YawRad,
    DateTime TimestampUtc
) : TimestampedState(TimestampUtc);
