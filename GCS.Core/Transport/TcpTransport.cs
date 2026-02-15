using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace GCS.Core.Transport;

public sealed class TcpTransport : TransportBase
{
    private readonly string _host;
    private readonly int _port;

    private TcpClient? _client;
    private NetworkStream? _stream;

    public TcpTransport(string host, int port)
    {
        _host = host;
        _port = port;
    }

    public override async Task StartAsync(CancellationToken externalToken)
    {
        Cts = CancellationTokenSource.CreateLinkedTokenSource(externalToken);

        _client = new TcpClient();
        await _client.ConnectAsync(_host, _port, Cts.Token);

        _stream = _client.GetStream();
        IoTask = Task.Run(() => ReadLoop(Cts.Token), Cts.Token);
    }

    public override async Task StopAsync()
    {
        if (Cts != null)
        {
            Cts.Cancel();
            if (IoTask != null)
                await IoTask;
            Cts.Dispose();
            Cts = null;
        }

        _stream?.Dispose();
        _client?.Dispose();
    }

    private async Task ReadLoop(CancellationToken token)
    {
        byte[] buffer = new byte[4096];

        try
        {
            while (!token.IsCancellationRequested)
            {
                int bytes = await _stream!.ReadAsync(buffer, token);

                if (bytes == 0)
                {
                    // Server closed connection
                    RaiseError(new IOException("Connection closed by remote host"));
                    break;
                }

                RaiseData(buffer.AsMemory(0, bytes));
            }
        }
        catch (OperationCanceledException)
        {
            // Normal shutdown
        }
        catch (Exception ex)
        {
            RaiseError(ex);
        }
    }

    public override async Task SendAsync(ReadOnlyMemory<byte> data, CancellationToken token)
    {
        if (_stream == null)
            throw new InvalidOperationException("Transport not started");

        try
        {
            await _stream.WriteAsync(data, token);
        }
        catch (Exception ex)
        {
            RaiseError(ex);
            throw;
        }
    }

    public override void Dispose()
    {
        base.Dispose();
        _stream?.Dispose();
        _client?.Dispose();
    }
}