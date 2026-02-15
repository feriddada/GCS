using GCS.Core.Domain;

namespace GCS.Core.Preflight;

public interface IPreflightService
{
    PreflightState Evaluate(VehicleState state);
}
