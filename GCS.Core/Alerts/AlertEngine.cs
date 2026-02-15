using GCS.Core.Domain;
using GCS.Core.Health;
using System;
using System.Collections.Generic;
using System.Threading;

namespace GCS.Core.Alerts;

public sealed class AlertEngine : IAlertEngine, IDisposable
{
    private readonly IVehicleHealthMonitor _health;
    private readonly AlertPolicy _policy;
    private readonly SynchronizationContext _context;

    private readonly Dictionary<AlertType, AlertState> _alerts = new();

    public IReadOnlyList<AlertState> Current =>
        new List<AlertState>(_alerts.Values);

    public event Action<IReadOnlyList<AlertState>>? AlertsChanged;

    public AlertEngine(
        IVehicleHealthMonitor health,
        AlertPolicy policy,
        SynchronizationContext context)
    {
        _health = health;
        _policy = policy;
        _context = context;

        _health.HealthChanged += OnHealthChanged;
    }

    private void OnHealthChanged(HealthState health)
    {
        Evaluate(AlertType.LinkLost,
            active: !health.LinkAlive,
            _policy.LinkLostSeverity);

        Evaluate(AlertType.AttitudeStale,
            active: !health.AttitudeFresh,
            _policy.AttitudeStaleSeverity);

        Evaluate(AlertType.PositionStale,
            active: !health.PositionFresh,
            _policy.PositionStaleSeverity);

        Publish();
    }

    private void Evaluate(
        AlertType type,
        bool active,
        AlertSeverity severity)
    {
        if (_alerts.TryGetValue(type, out var existing))
        {
            if (existing.Active == active)
                return;

            _alerts[type] = existing with
            {
                Active = active,
                TimestampUtc = DateTime.UtcNow
            };
        }
        else
        {
            _alerts[type] = new AlertState(
                type,
                severity,
                active,
                DateTime.UtcNow
            );
        }
    }

    private void Publish()
    {
        _context.Post(_ =>
        {
            AlertsChanged?.Invoke(Current);
        }, null);
    }

    public void Dispose()
    {
        _health.HealthChanged -= OnHealthChanged;
    }
}
