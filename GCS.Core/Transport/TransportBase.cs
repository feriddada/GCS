using System;
using System.Threading;
using System.Threading.Tasks;

namespace GCS.Core.Transport;

public abstract class TransportBase : ITransport
{
    public event Action<ReadOnlyMemory<byte>>? DataReceived;
    public event Action<Exception>? TransportError;

    protected CancellationTokenSource? Cts;
    protected Task? IoTask;

    public abstract Task StartAsync(CancellationToken cancellationToken);
    public abstract Task StopAsync();
    public abstract Task SendAsync(ReadOnlyMemory<byte> data, CancellationToken cancellationToken);

    protected void RaiseData(ReadOnlyMemory<byte> data)
        => DataReceived?.Invoke(data);

    protected void RaiseError(Exception ex)
        => TransportError?.Invoke(ex);

    public virtual void Dispose()
    {
        Cts?.Cancel();
        Cts?.Dispose();
    }
}
