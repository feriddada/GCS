using GCS.AI_CHAT.Analyzer;

namespace GCS.AI_CHAT.LogSystem;

public class FlightLogAnalyzer
{
    private BatteryAnalyzer batteryAnalyzer =
        new BatteryAnalyzer();

    private GPSAnalyzer gpsAnalyzer =
        new GPSAnalyzer();

    private VibrationAnalyzer vibrationAnalyzer =
        new VibrationAnalyzer();

    private PIDAnalyzer pidAnalyzer =
        new PIDAnalyzer();

    public FlightLogReport Analyze(FlightLog log)
    {
        FlightLogReport report =
            new FlightLogReport();

   
        report.Results.Add(
            gpsAnalyzer.Analyze(log.Data));

        report.Results.Add(
            vibrationAnalyzer.Analyze(log.Data));

        report.Results.Add(
            pidAnalyzer.Analyze(log.Data));

        report.TotalProblems =
            report.Results.Count(x => x.HasProblem);

        report.SafeToFly =
            report.TotalProblems == 0;

        return report;
    }
}