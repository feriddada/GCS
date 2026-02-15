namespace GCS.Core.Domain;

/// <summary>
/// Represents a mission waypoint or command.
/// </summary>
public record MissionItem(
    int Sequence,
    ushort Command,
    double LatitudeDeg,
    double LongitudeDeg,
    float AltitudeMeters,
    float Param1 = 0,        // Hold time (WP), Pitch (TKOF), etc.
    float Param2 = 0,        // Acceptance radius in meters
    float Param3 = 0,        // Pass through (0) or stop (1)
    float Param4 = 0,        // Yaw angle
    byte Frame = 3,          // MAV_FRAME (3 = GLOBAL_RELATIVE_ALT)
    bool AutoContinue = true
)
{
    /// <summary>
    /// Acceptance radius in meters (from Param2, default 10m)
    /// </summary>
    public float AcceptanceRadius => Param2 > 0 ? Param2 : 10f;
}

/// <summary>
/// Common MAV_CMD values for mission planning.
/// </summary>
public static class MavCmd
{
    public const ushort Home = 0;
    public const ushort Waypoint = 16;
    public const ushort Loiter = 17;
    public const ushort LoiterTurns = 18;
    public const ushort LoiterTime = 19;
    public const ushort ReturnToLaunch = 20;
    public const ushort Land = 21;
    public const ushort Takeoff = 22;
    public const ushort LoiterAlt = 31;
}