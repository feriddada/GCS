namespace GCS.Core.Domain;

public enum FlightMode
{
    // --- Fixed-wing ---
    Manual,
    Circle,
    Stabilize,
    Training,
    Acro,
    Fbwa,
    Fbwb,
    Cruise,
    Autotune,
    Auto,
    Rtl,
    Loiter,
    Takeoff,
    AvoidAdsb,
    Guided,
    Initialising,

    // --- VTOL (QuadPlane) ---
    QStabilize,
    QHover,
    QLoiter,
    QLand,
    QRtl
}
