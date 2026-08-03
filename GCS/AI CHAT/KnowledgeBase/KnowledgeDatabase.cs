namespace GCS.AI_CHAT.KnowledgeBase;

public class KnowledgeDatabase
{
    public List<KnowledgeRule> Rules { get; }
        = new();

    public KnowledgeDatabase()
    {
        //---------------------------------
        // Propeller Imbalance
        //---------------------------------

        Rules.Add(
            new KnowledgeRule
            {
                Problem =
                    "Propeller Imbalance",

                RequiredEvidence =
                {
                    "Motor current imbalance",
                    "High vibration"
                },

                Recommendation =
                    "Inspect propeller and motor.",

                Parameters =
                {
                    "INS_GYRO_FILTER",
                    "ATC_RAT_RLL_P"
                }
            });

        //---------------------------------
        // ESC Overheat
        //---------------------------------

        Rules.Add(
            new KnowledgeRule
            {
                Problem =
                    "ESC Overheating",

                RequiredEvidence =
                {
                    "ESC overheating detected"
                },

                Recommendation =
                    "Check ESC cooling."
            });

        //---------------------------------
        // Battery
        //---------------------------------

        Rules.Add(
            new KnowledgeRule
            {
                Problem =
                    "Battery Health Problem",

                RequiredEvidence =
                {
                    "Battery capacity is low",
                    "Voltage sag detected"
                },

                Recommendation =
                    "Replace or recharge battery."
            });
    }
}