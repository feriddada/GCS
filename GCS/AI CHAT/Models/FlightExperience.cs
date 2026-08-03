namespace GCS.AI.Models;


public class FlightExperience
{

    public AircraftProfile Aircraft { get; set; }


    public FlightLogSummary Log { get; set; }


    public List<FlightProblem> Problems { get; set; }
    = new();


    public List<ParameterChange> Changes { get; set; }
    = new();

}