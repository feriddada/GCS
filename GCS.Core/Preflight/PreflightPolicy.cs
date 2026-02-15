namespace GCS.Core.Preflight;

public sealed record PreflightPolicy(
    float MinBatteryVoltage,
    int MinBatteryPercent
);
