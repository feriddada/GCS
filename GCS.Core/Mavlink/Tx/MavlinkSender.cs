namespace GCS.Core.Mavlink.Tx;

/// <summary>
/// Sends raw MAVLink packets via the backend.
/// </summary>
public sealed class MavlinkSender : IMavlinkSender
{
    private readonly IMavlinkBackend _backend;

    public MavlinkSender(IMavlinkBackend backend)
    {
        _backend = backend;
    }

    public async Task SendAsync(byte[] packet, CancellationToken ct = default)
    {
        await _backend.SendRawAsync(packet, ct);
    }
}