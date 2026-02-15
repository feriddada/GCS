namespace GCS.Core.Transport;

public sealed record UdpTransportConfig(
    int LocalPort,
    string RemoteHost,
    int RemotePort
) : TransportConfig;
