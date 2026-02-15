namespace GCS.Core.Transport;

public interface ITransport : IDisposable
{
    /// <summary>
    /// Fired when raw bytes are received.
    /// Called from transport thread.
    /// </summary>
    event Action<ReadOnlyMemory<byte>> DataReceived;

    /// <summary>
    /// Fired on transport-level error (port closed, socket error, etc).
    /// </summary>
    event Action<Exception> TransportError;

    Task StartAsync(CancellationToken cancellationToken);
    Task StopAsync();

    /// <summary>
    /// Optional: send raw bytes (for commands, heartbeat, etc).
    /// </summary>
    Task SendAsync(ReadOnlyMemory<byte> data, CancellationToken cancellationToken);
}
