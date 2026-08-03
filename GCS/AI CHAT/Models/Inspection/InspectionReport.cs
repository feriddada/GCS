using System;
using System.Collections.Generic;
using System.Linq;

namespace GCS.AI_CHAT.Models.Inspection;

public class InspectionReport
{
    //---------------------------------
    // Components
    //---------------------------------

    public List<ComponentInspection> Components { get; set; }
        = new();

    //---------------------------------
    // Ready For Analysis
    //---------------------------------

    public bool ReadyForAnalysis =>
        Components.Any(x => x.CanAnalyze);

    //---------------------------------
    // Get Component
    //---------------------------------

    public ComponentInspection? Get(string name)
    {
        return Components.FirstOrDefault(x =>
            x.Name.Equals(
                name,
                StringComparison.OrdinalIgnoreCase));
    }
}