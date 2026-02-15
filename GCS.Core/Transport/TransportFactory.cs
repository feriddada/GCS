namespace GCS.Core.Transport;

public static class TransportFactory
{
    public static ITransport Create(TransportConfig config)
    {
        return config switch
        {
            SerialTransportConfig s =>
                new SerialTransport(s.PortName, s.BaudRate),

            TcpTransportConfig t =>
                new TcpTransport(t.Host, t.Port),

            UdpTransportConfig u =>
                new UdpTransport(u.LocalPort, u.RemoteHost, u.RemotePort),

            _ => throw new ArgumentOutOfRangeException(
                nameof(config),
                "Unsupported transport config")
        };
    }
}
