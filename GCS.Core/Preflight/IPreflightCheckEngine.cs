using GCS.Core.Domain;

namespace GCS.Core.Preflight;

public interface IPreflightCheckEngine
{
    PreflightState Current { get; }
    event Action<PreflightState> PreflightChanged;
}
