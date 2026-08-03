using GCS.AI_CHAT.Models;
using GCS.AI_CHAT.Models.Inspection;

namespace GCS.AI_CHAT.Inspector;

public class SystemInspector
{
    public InspectionReport Inspect(
        FlightData data)
    {
        InspectionReport report =
            new();

        //---------------------------------
        // Battery
        //---------------------------------

        report.Components.Add(
            new ComponentInspection
            {
                Name = "Battery",

                State =
                    data.BatteryVoltage > 0
                    ? DataState.Available
                    : DataState.Missing,

                Reason =
                    data.BatteryVoltage > 0
                    ? "Battery telemetry detected."
                    : "Battery telemetry unavailable."
            });

        //---------------------------------
        // GPS
        //---------------------------------

        report.Components.Add(
            new ComponentInspection
            {
                Name = "GPS",

                State =
                    data.Satellites > 0
                    ? DataState.Available
                    : DataState.Missing,

                Reason =
                    data.Satellites > 0
                    ? "GPS connected."
                    : "GPS not detected."
            });

        //---------------------------------
        // ESC
        //---------------------------------

        report.Components.Add(
            new ComponentInspection
            {
                Name = "ESC",

                State =
                    data.ESCTemperatures.Any()
                    ? DataState.Available
                    : DataState.NotConfigured,

                Reason =
                    data.ESCTemperatures.Any()
                    ? "ESC telemetry available."
                    : "ESC telemetry not configured."
            });

        return report;
    }
}