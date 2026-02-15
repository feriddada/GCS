namespace GCS.Core.Domain;

public record VehicleState(
    ConnectionState? Connection,
    AttitudeState? Attitude,
    PositionState? Position,
    VfrHudState? VfrHud,
    BatteryState? Battery,
    FlightMode? FlightMode,
    bool IsArmed = false
);