using GCS.Core.Domain;

namespace GCS.Core.Health;

public interface IVehicleHealthMonitor
{
    HealthState Current { get; }
    event Action<HealthState> HealthChanged;
}
