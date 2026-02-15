namespace GCS.Core.Mavlink.Tx;

/// <summary>
/// Interface for sending raw MAVLink packets.
/// </summary>
public interface IMavlinkSender
{
    Task SendAsync(byte[] packet, CancellationToken ct = default);
}