namespace GCS.Core.Transport;

public sealed record SerialTransportConfig(
    string PortName,
    int BaudRate
) : TransportConfig;
