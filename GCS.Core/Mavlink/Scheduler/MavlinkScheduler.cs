using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace GCS.Core.Mavlink.Scheduler;

public sealed class MavlinkScheduler : IAsyncDisposable, IDisposable
{
    private readonly List<Func<CancellationToken, Task>> _tasks = new();
    private CancellationTokenSource? _cts;
    private Task? _loop;

    public void AddPeriodicTask(
        TimeSpan interval,
        Func<CancellationToken, Task> action)
    {
        if (_cts != null)
            throw new InvalidOperationException("Cannot add tasks after scheduler started");

        _tasks.Add(async token =>
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    await action(token);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception)
                {
                    // TODO: diagnostics/log
                }

                try
                {
                    await Task.Delay(interval, token);
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            }
        });
    }

    public void Start(CancellationToken external)
    {
        if (_cts != null)
            throw new InvalidOperationException("Scheduler already started");

        _cts = CancellationTokenSource.CreateLinkedTokenSource(external);

        _loop = Task.Run(async () =>
        {
            var tasks = new List<Task>();
            foreach (var t in _tasks)
                tasks.Add(t(_cts.Token));

            await Task.WhenAll(tasks);
        });
    }

    public async ValueTask DisposeAsync()
    {
        if (_cts != null)
        {
            _cts.Cancel();

            if (_loop != null)
            {
                try
                {
                    await _loop;
                }
                catch (OperationCanceledException)
                {
                    // Expected
                }
            }

            _cts.Dispose();
            _cts = null;
        }
    }

    public void Dispose()
    {
        _cts?.Cancel();
        _loop?.GetAwaiter().GetResult(); // Blocking wait - prefer DisposeAsync
        _cts?.Dispose();
    }
}