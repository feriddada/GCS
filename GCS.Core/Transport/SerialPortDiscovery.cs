using System.IO.Ports;

namespace GCS.Core.Transport;

public static class SerialPortDiscovery
{
    public static IReadOnlyList<string> GetAvailablePorts()
        => SerialPort.GetPortNames();
}
