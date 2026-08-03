using GCS.AI_CHAT.Models;

namespace GCS.AI_CHAT.KnowledgeBase;

public class KnowledgeRule
{
    //---------------------------------
    // Problem
    //---------------------------------

    public string Problem { get; set; } = "";

    //---------------------------------
    // Evidence
    //---------------------------------

    public List<string> RequiredEvidence { get; set; }
        = new();

    //---------------------------------
    // Confidence
    //---------------------------------

    public int Confidence { get; set; }

    //---------------------------------
    // Severity
    //---------------------------------

    public string Severity { get; set; } = "";

    //---------------------------------
    // Recommendation
    //---------------------------------

    public string Recommendation { get; set; } = "";

    //---------------------------------
    // Suggested Parameters
    //---------------------------------

    public List<string> Parameters { get; set; }
        = new();

    //---------------------------------
    // Aircraft Types
    //---------------------------------

    public List<string> AircraftTypes { get; set; }
        = new();

    //---------------------------------
    // Flight Modes
    //---------------------------------

    public List<string> FlightModes { get; set; }
        = new();
}