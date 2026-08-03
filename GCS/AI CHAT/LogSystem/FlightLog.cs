using GCS.AI_CHAT.Models;

namespace GCS.AI_CHAT.LogSystem;

public class FlightLog
{
    public FlightData Data { get; set; } = new();

    public DateTime FlightTime { get; set; }

    public string Aircraft { get; set; } = "";

    public string Firmware { get; set; } = "";

    public string FlightMode { get; set; } = "";

    public double FlightDuration { get; set; }
}