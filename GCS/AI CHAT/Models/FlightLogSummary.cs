namespace GCS.AI.Models;

public class FlightLogSummary
{

    public string AircraftName { get; set; } = "";


    public double FlightTime { get; set; }


    public double MaxAltitude { get; set; }


    public double MaxVibration { get; set; }


    public double AverageRollError { get; set; }


    public double AveragePitchError { get; set; }


    public bool HadProblem { get; set; }

}