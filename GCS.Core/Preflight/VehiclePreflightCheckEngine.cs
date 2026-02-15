using GCS.Core.Domain;
using GCS.Core.Health;
using GCS.Core.State;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace GCS.Core.Preflight;

public sealed class VehiclePreflightCheckEngine :
    IPreflightCheckEngine, IDisposable
{
    private readonly IVehicleStateStore _stateStore;
    private readonly IVehicleHealthMonitor _health;
    private readonly PreflightPolicy _policy;
    private readonly SynchronizationContext _context;

    private PreflightState _current =
        new(Array.Empty<PreflightCheckResult>(), DateTime.UtcNow);

    private readonly CancellationTokenSource _cts = new();

    public PreflightState Current => _current;

    public event Action<PreflightState>? PreflightChanged;

    public VehiclePreflightCheckEngine(
        IVehicleStateStore stateStore,
        IVehicleHealthMonitor health,
        PreflightPolicy policy,
        SynchronizationContext context)
    {
        _stateStore = stateStore;
        _health = health;
        _policy = policy;
        _context = context;

        _stateStore.StateChanged += _ => Evaluate();
        _health.HealthChanged += _ => Evaluate();

        StartLoop();
    }

    private void StartLoop()
    {
        Task.Run(async () =>
        {
            try
            {
                while (!_cts.Token.IsCancellationRequested)
                {
                    Evaluate();
                    await Task.Delay(500, _cts.Token);
                }
            }
            catch (OperationCanceledException) { }
        });
    }

    private void Evaluate()
    {
        var vs = _stateStore.Current;
        var hs = _health.Current;
        var now = DateTime.UtcNow;

        var checks = new List<PreflightCheckResult>();

        // --- LINK ---
        checks.Add(
            hs.LinkAlive
                ? Pass("Link")
                : Fail("Link", "No connection")
        );

        // --- ATTITUDE ---
        checks.Add(
            hs.AttitudeFresh
                ? Pass("Attitude")
                : Fail("Attitude", "No fresh attitude data")
        );

        // --- POSITION ---
        checks.Add(
            hs.PositionFresh
                ? Pass("Position")
                : Fail("Position", "No fresh position data")
        );

        // --- BATTERY ---
        if (vs.Battery == null)
        {
            checks.Add(
                new PreflightCheckResult(
                    "Battery",
                    PreflightCheckStatus.Unknown,
                    "No battery data"
                )
            );
        }
        else if (vs.Battery.VoltageVolts < _policy.MinBatteryVoltage)
        {
            checks.Add(
                Fail("Battery", "Voltage too low")
            );
        }
        else if (
            vs.Battery.RemainingPercent >= 0 &&
            vs.Battery.RemainingPercent < _policy.MinBatteryPercent)
        {
            checks.Add(
                Fail("Battery", "Battery level too low")
            );
        }
        else
        {
            checks.Add(Pass("Battery"));
        }

        var next = new PreflightState(checks, now);

        if (!Equals(next, _current))
        {
            _context.Post(_ =>
            {
                _current = next;
                PreflightChanged?.Invoke(_current);
            }, null);
        }
    }

    private static PreflightCheckResult Pass(string name) =>
        new(name, PreflightCheckStatus.Passed, null);

    private static PreflightCheckResult Fail(string name, string reason) =>
        new(name, PreflightCheckStatus.Failed, reason);

    public void Dispose()
    {
        _cts.Cancel();
        _stateStore.StateChanged -= _ => Evaluate();
        _health.HealthChanged -= _ => Evaluate();
    }
}
