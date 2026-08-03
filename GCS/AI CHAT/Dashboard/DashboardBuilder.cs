using GCS.AI_CHAT.Models;
using GCS.AI_CHAT.Models.AI;

namespace GCS.AI_CHAT.Dashboard;

public class DashboardBuilder
{
    public AIDashboard Build(
        AIAnalysisResult result)
    {
        AIDashboard dashboard =
            new();

        dashboard.OverallHealth =
            result.OverallHealthScore;

        dashboard.SafeToFly =
            result.SafeToFly;

        dashboard.Recommendations =
            result.Recommendations;

        //---------------------------------
        // Battery
        //---------------------------------

        dashboard.Components.Add(
            new AIStatus
            {
                Component = "Battery",

                Status =
                    result.Health.Battery.Status,

                HealthScore =
                    result.Health.Battery.HealthScore,

                AIComment =
                    result.Health.Battery.Warning
                    ? "Battery requires attention."
                    : "Battery operating normally.",

                Warning =
                    result.Health.Battery.Warning,

                Critical =
                    result.Health.Battery.Critical
            });

        //---------------------------------
        // Motor
        //---------------------------------

        dashboard.Components.Add(
            new AIStatus
            {
                Component = "Motor",

                Status =
                    result.Health.Motor.Status,

                HealthScore =
                    result.Health.Motor.HealthScore,

                AIComment =
                    result.Health.Motor.Warning
                    ? "Motor anomaly detected."
                    : "Motors operating normally.",

                Warning =
                    result.Health.Motor.Warning,

                Critical =
                    result.Health.Motor.Critical
            });

        //---------------------------------
        // ESC
        //---------------------------------

        if (result.Health.ESC != null)
        {
            dashboard.Components.Add(
                new AIStatus
                {
                    Component = "ESC",

                    Status =
                        result.Health.ESC.Status,

                    HealthScore =
                        result.Health.ESC.HealthScore,

                    AIComment =
                        result.Health.ESC.Warning
                        ? "ESC temperature increasing."
                        : "ESC operating normally.",

                    Warning =
                        result.Health.ESC.Warning,

                    Critical =
                        result.Health.ESC.Critical
                });
        }

        return dashboard;
    }
}