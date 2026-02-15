namespace GCS.Core.Domain;

public sealed record BatteryState(
    float VoltageVolts,
    float CurrentAmps,
    int RemainingPercent,
    DateTime TimestampUtc
) : TimestampedState(TimestampUtc);
