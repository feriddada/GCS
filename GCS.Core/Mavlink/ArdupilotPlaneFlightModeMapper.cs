using GCS.Core.Domain;

namespace GCS.Core.Mavlink;

/// <summary>
/// Maps ArduPilot Plane custom_mode values to FlightMode enum.
/// Based on ArduPilot source: ArduPlane/mode.h
/// </summary>
public static class ArdupilotPlaneFlightModeMapper
{
    public static uint ToCustomMode(FlightMode mode)
    {
        return mode switch
        {
            FlightMode.Manual => 0,
            FlightMode.Circle => 1,
            FlightMode.Stabilize => 2,
            FlightMode.Training => 3,
            FlightMode.Acro => 4,
            FlightMode.Fbwa => 5,
            FlightMode.Fbwb => 6,
            FlightMode.Cruise => 7,
            FlightMode.Autotune => 8,
            // 9 is not used
            FlightMode.Auto => 10,
            FlightMode.Rtl => 11,        // RTL is 11!
            FlightMode.Loiter => 12,     // Loiter is 12!
            FlightMode.Takeoff => 13,
            FlightMode.AvoidAdsb => 14,
            FlightMode.Guided => 15,
            FlightMode.Initialising => 16,

            // VTOL modes
            FlightMode.QStabilize => 17,
            FlightMode.QHover => 18,
            FlightMode.QLoiter => 19,
            FlightMode.QLand => 20,
            FlightMode.QRtl => 21,

            _ => 0
        };
    }

    public static FlightMode? FromCustomMode(uint customMode)
    {
        return customMode switch
        {
            0 => FlightMode.Manual,
            1 => FlightMode.Circle,
            2 => FlightMode.Stabilize,
            3 => FlightMode.Training,
            4 => FlightMode.Acro,
            5 => FlightMode.Fbwa,
            6 => FlightMode.Fbwb,
            7 => FlightMode.Cruise,
            8 => FlightMode.Autotune,
            // 9 is not used
            10 => FlightMode.Auto,
            11 => FlightMode.Rtl,        // RTL is 11!
            12 => FlightMode.Loiter,     // Loiter is 12!
            13 => FlightMode.Takeoff,
            14 => FlightMode.AvoidAdsb,
            15 => FlightMode.Guided,
            16 => FlightMode.Initialising,

            // VTOL modes
            17 => FlightMode.QStabilize,
            18 => FlightMode.QHover,
            19 => FlightMode.QLoiter,
            20 => FlightMode.QLand,
            21 => FlightMode.QRtl,

            _ => null
        };
    }
}