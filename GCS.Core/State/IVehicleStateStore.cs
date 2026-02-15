using GCS.Core.Domain;

namespace GCS.Core.State;

public interface IVehicleStateStore
{
    VehicleState Current { get; }

    event Action<VehicleState> StateChanged;
}
