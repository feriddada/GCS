using GCS.AI_CHAT.Analyzer;
using GCS.AI_CHAT.Models;
using GCS.AI_CHAT.Models.Analysis;
using GCS.AI_CHAT.Models.Inspection;

namespace GCS.AI_CHAT.Engine;

public class AircraftHealthEngine
{
    //---------------------------------
    // Analyzers
    //---------------------------------

    private readonly BatteryAnalyzer batteryAnalyzer =
        new();

    private readonly MotorAnalyzer motorAnalyzer =
        new();

    // Gələcək
    // private readonly ESCAnalyzer escAnalyzer = new();
    // private readonly GPSAnalyzer gpsAnalyzer = new();
    // private readonly IMUAnalyzer imuAnalyzer = new();
    // private readonly EKFAnalyzer ekfAnalyzer = new();
    // private readonly PowerAnalyzer powerAnalyzer = new();

    //---------------------------------
    // Main Analysis
    //---------------------------------

    public AircraftHealth Analyze(
        FlightData data,
        InspectionReport inspection)
    {
        AircraftHealth health =
            new();

        //---------------------------------
        // Battery
        //---------------------------------

        var battery =
            inspection.Get("Battery");

        if (battery != null &&
            battery.CanAnalyze)
        {
            health.Battery =
                batteryAnalyzer.Analyze(data);

            health.AllEvidence.AddRange(
                health.Battery.Evidence);
        }

        //---------------------------------
        // Motor
        //---------------------------------

        var motor =
            inspection.Get("Motor");

        if (motor != null &&
            motor.CanAnalyze)
        {
            health.Motor =
                motorAnalyzer.Analyze(data);

            health.AllEvidence.AddRange(
                health.Motor.Evidence);
        }

        //---------------------------------
        // ESC
        //---------------------------------

        // var esc =
        //     inspection.Get("ESC");
        //
        // if (esc != null &&
        //     esc.CanAnalyze)
        // {
        //     health.ESC =
        //         escAnalyzer.Analyze(data);
        //
        //     health.AllEvidence.AddRange(
        //         health.ESC.Evidence);
        // }

        return health;
    }
}