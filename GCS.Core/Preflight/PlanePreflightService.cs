using GCS.Core.Domain;

namespace GCS.Core.Preflight;

public sealed class PlanePreflightService : IPreflightService
{
    public PreflightState Evaluate(VehicleState state)
    {
        var checks = new List<PreflightCheckResult>();
        var now = DateTime.UtcNow;

        // 1. Connection
        checks.Add(state.Connection?.IsConnected == true
            ? new PreflightCheckResult("Connection", PreflightCheckStatus.Passed, null)
            : new PreflightCheckResult("Connection", PreflightCheckStatus.Failed, "Нет связи с автопилотом"));

        // 2. Attitude
        checks.Add(state.Attitude != null
            ? new PreflightCheckResult("Attitude", PreflightCheckStatus.Passed, null)
            : new PreflightCheckResult("Attitude", PreflightCheckStatus.Failed, "Нет данных ориентации"));

        // 3. GPS
        checks.Add(state.Position != null
            ? new PreflightCheckResult("GPS", PreflightCheckStatus.Passed, null)
            : new PreflightCheckResult("GPS", PreflightCheckStatus.Failed, "Нет позиции (GPS)"));

        // 4. Altitude sanity
        if (state.Position != null)
        {
            checks.Add(state.Position.AltitudeMslMeters >= -100
                ? new PreflightCheckResult("Altitude", PreflightCheckStatus.Passed, null)
                : new PreflightCheckResult("Altitude", PreflightCheckStatus.Failed, "Подозрительная высота"));
        }

        // 5. Speed sanity (from VfrHud)
        if (state.VfrHud != null)
        {
            checks.Add(state.VfrHud.GroundspeedMps <= 100
                ? new PreflightCheckResult("Speed", PreflightCheckStatus.Passed, null)
                : new PreflightCheckResult("Speed", PreflightCheckStatus.Failed, "Слишком большая скорость"));
        }

        return new PreflightState(checks, now);
    }
}