namespace GCS.AI_CHAT.Models.AI;

public class AIMessage
{
    //---------------------------------
    // Time
    //---------------------------------

    public DateTime Time { get; set; }
        = DateTime.Now;

    //---------------------------------
    // Component
    //---------------------------------

    public string Component { get; set; } = "";

    //---------------------------------
    // Title
    //---------------------------------

    public string Title { get; set; } = "";

    //---------------------------------
    // Message
    //---------------------------------

    public string Message { get; set; } = "";

    //---------------------------------
    // Confidence
    //---------------------------------

    public int Confidence { get; set; }

    //---------------------------------
    // Priority
    //---------------------------------

    public int Priority { get; set; }

    //---------------------------------
    // Attention
    //---------------------------------

    public bool RequiresAttention { get; set; }

    //---------------------------------
    // Status
    //---------------------------------

    public bool Warning { get; set; }

    public bool Critical { get; set; }
}