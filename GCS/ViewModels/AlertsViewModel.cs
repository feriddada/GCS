using GCS.Core.Domain;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace GCS.ViewModels;

public class AlertsViewModel : ViewModelBase
{
    private bool _hasActiveAlerts;
    private AlertSeverity _highestSeverity = AlertSeverity.Info;

    public ObservableCollection<AlertItemViewModel> ActiveAlerts { get; } = new();

    public bool HasActiveAlerts
    {
        get => _hasActiveAlerts;
        private set => SetProperty(ref _hasActiveAlerts, value);
    }

    public AlertSeverity HighestSeverity
    {
        get => _highestSeverity;
        private set => SetProperty(ref _highestSeverity, value);
    }

    public string SeverityColor => HighestSeverity switch
    {
        AlertSeverity.Critical => "#F44336",  // Red
        AlertSeverity.Warning => "#FF9800",   // Orange
        _ => "#4CAF50"                         // Green
    };

    public void UpdateAlerts(IReadOnlyList<AlertState> alerts)
    {
        ActiveAlerts.Clear();

        var active = alerts.Where(a => a.Active).ToList();

        foreach (var alert in active)
        {
            ActiveAlerts.Add(new AlertItemViewModel(alert));
        }

        HasActiveAlerts = active.Any();

        HighestSeverity = active.Any()
            ? active.Max(a => a.Severity)
            : AlertSeverity.Info;

        OnPropertyChanged(nameof(SeverityColor));
    }
}

public class AlertItemViewModel : ViewModelBase
{
    public AlertType Type { get; }
    public AlertSeverity Severity { get; }
    public string Message { get; }
    public string Timestamp { get; }
    public string SeverityColor { get; }

    public AlertItemViewModel(AlertState alert)
    {
        Type = alert.Type;
        Severity = alert.Severity;
        Timestamp = alert.TimestampUtc.ToString("HH:mm:ss");

        Message = alert.Type switch
        {
            AlertType.LinkLost => "Link Lost - No heartbeat received",
            AlertType.AttitudeStale => "Attitude data is stale",
            AlertType.PositionStale => "Position data is stale",
            _ => alert.Type.ToString()
        };

        SeverityColor = alert.Severity switch
        {
            AlertSeverity.Critical => "#F44336",
            AlertSeverity.Warning => "#FF9800",
            _ => "#2196F3"
        };
    }
}