using GCS.Core.Domain;

namespace GCS.Core.Alerts;

public interface IAlertEngine
{
    IReadOnlyList<AlertState> Current { get; }
    event Action<IReadOnlyList<AlertState>> AlertsChanged;
}
