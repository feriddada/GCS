using GCS.AI_CHAT.Models.Analysis;

namespace GCS.AI_CHAT.Fault;

public class FaultDetector
{
    public List<AircraftFault> Detect(AircraftHealth health)
    {
        List<AircraftFault> faults = new();

        //-----------------------------------
        // Battery
        //-----------------------------------

        if (health.Battery.Critical)
        {
            faults.Add(new AircraftFault
            {
                Type = AircraftFaultType.Battery,
                Name = "Low Battery",
                Description = "Battery health is critical.",
                Confidence = 95,
                Severity = AircraftFaultSeverity.Critical
            });
        }

        //-----------------------------------
        // Motor
        //-----------------------------------

        if (health.Motor.ImbalanceDetected)
        {
            faults.Add(new AircraftFault
            {
                Type = AircraftFaultType.Motor,
                Name = "Motor Current Imbalance",
                Description = "One or more motors consume abnormal current.",
                Confidence = 85,
                Severity = AircraftFaultSeverity.High
            });
        }

        return faults;
    }
}