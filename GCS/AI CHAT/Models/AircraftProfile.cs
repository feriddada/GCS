namespace GCS.AI.Models;

public class AircraftProfile
{
    public string Name { get; set; } = "";

    public string Type { get; set; } = "";
    // Fixed Wing / VTOL / Multirotor


    // Physical

    public double MTOW { get; set; }

    public double WingSpan { get; set; }

    public double WingArea { get; set; }


    // Flight

    public double CruiseSpeed { get; set; }

    public double StallSpeed { get; set; }


    // Hardware

    public string FlightController { get; set; } = "";

    public string Firmware { get; set; } = "";
}