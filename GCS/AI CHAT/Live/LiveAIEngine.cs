using GCS.AI_CHAT.Engine;
using GCS.AI_CHAT.History;
using GCS.AI_CHAT.Models;
using GCS.AI_CHAT.Models.AI;
using GCS.AI_CHAT.Prediction;

namespace GCS.AI_CHAT.Live;

public class LiveAIEngine
{
    //---------------------------------
    // Engines
    //---------------------------------

    private readonly AIAnalysisEngine analysis =
        new();

    private readonly TelemetryHistory history =
        new();

    private readonly TrendBuilder trendBuilder =
        new();

    private readonly TrendAnalyzer trendAnalyzer =
        new();

    //---------------------------------
    // Live Update
    //---------------------------------

    public List<AIMessage> Update(
        FlightData data)
    {
        //---------------------------------
        // Message List
        //---------------------------------

        List<AIMessage> messages =
            new();

        //---------------------------------
        // Flight Controller
        //---------------------------------

        if (!data.FlightControllerConnected)
        {
            messages.Add(
                new AIMessage
                {
                    Component = "System",

                    Title = "Flight Controller",

                    Message =
                        "Flight Controller not connected.",

                    Confidence = 100
                });

            return messages;
        }

        //---------------------------------
        // Save telemetry history
        //---------------------------------

        history.Add(data);

        //---------------------------------
        // Run AI Analysis
        //---------------------------------

        AIAnalysisResult result =
            analysis.Analyze(data);

        System.Diagnostics.Debug.WriteLine("========== AI DEBUG ==========");

        System.Diagnostics.Debug.WriteLine(
            $"FC Connected : {data.FlightControllerConnected}");

        System.Diagnostics.Debug.WriteLine(
            $"Battery Voltage : {data.BatteryVoltage}");

        System.Diagnostics.Debug.WriteLine(
            $"Battery Remaining : {data.BatteryRemaining}");

        System.Diagnostics.Debug.WriteLine(
            $"Satellites : {data.Satellites}");

        System.Diagnostics.Debug.WriteLine(
            $"Battery Warning : {result.Health.Battery?.Warning}");

        System.Diagnostics.Debug.WriteLine(
            $"Battery Score : {result.Health.Battery?.HealthScore}");

        System.Diagnostics.Debug.WriteLine(
            $"Motor Score : {result.Health.Motor?.HealthScore}");

        System.Diagnostics.Debug.WriteLine(
            $"Overall Health : {result.OverallHealthScore}");

        System.Diagnostics.Debug.WriteLine(
            $"Crash Risk : {result.Prediction.CrashRisk}");

        System.Diagnostics.Debug.WriteLine("==============================");

        if (data.BatteryVoltage < 5 ||
        data.BatteryRemaining < 0)
        {
            messages.Add(
                new AIMessage
                {
                    Component = "Battery",

                    Title = "Battery",

                    Message =
                        "Battery telemetry unavailable.",

                    Confidence = 100
                });
        }
        else
        {
            if (result.Health.Battery != null &&
                result.Health.Battery.Warning)
            {
                messages.Add(
                    new AIMessage
                    {
                        Component = "Battery",

                        Title = "Battery Warning",

                        Message =
                            "Battery health is decreasing.",

                        Confidence = 90,

                        Warning = true
                    });
            }
        }

        //---------------------------------
        // Battery Trend
        //---------------------------------
        if (data.BatteryVoltage >= 5 &&
    data.BatteryRemaining >= 0 &&
    history.HasEnoughData)
        {
            var batteryTrend =
                trendAnalyzer.Analyze(
                    trendBuilder.BatteryVoltage(
                        history.Samples));

            if (batteryTrend.Falling)
            {
                messages.Add(
                    new AIMessage
                    {
                        Component = "Battery",

                        Title = "Battery Trend",

                        Message =
                            batteryTrend.Comment,

                        Confidence = 85,

                        Warning = true
                    });
            }
        }

        //---------------------------------
        // Motor
        //---------------------------------

        if (result.Health.Motor.Warning)
        {
            messages.Add(
                new AIMessage
                {
                    Component = "Motor",

                    Title = "Motor Warning",

                    Message =
                        "Motor anomaly detected.",

                    Confidence = 85,

                    Warning = true
                });
        }

        //---------------------------------
        // ESC
        //---------------------------------

        if (result.Health.ESC != null &&
            result.Health.ESC.Warning)
        {
            messages.Add(
                new AIMessage
                {
                    Component = "ESC",

                    Title = "ESC Warning",

                    Message =
                        "ESC temperature increasing.",

                    Confidence = 88,

                    Warning = true
                });
        }

        //---------------------------------
        // Flight Status
        //---------------------------------

        messages.Add(
            new AIMessage
            {
                Component = "Aircraft",

                Title = "Overall Health",

                Message =
                    $"Aircraft health : {result.OverallHealthScore}%",

                Confidence = 100
            });

        //---------------------------------
        // Crash Prediction
        //---------------------------------

        messages.Add(
            new AIMessage
            {
                Component = "Prediction",

                Title = "Crash Risk",

                Message =
                    $"Crash probability : {result.Prediction.CrashRisk}%",

                Confidence = result.Prediction.CrashRisk
            });

        return messages;
    }
}