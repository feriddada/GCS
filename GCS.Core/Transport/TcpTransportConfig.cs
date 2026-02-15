namespace GCS.Core.Transport;

public sealed record TcpTransportConfig(
    string Host,
    int Port
) : TransportConfig;
