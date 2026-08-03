namespace GCS.AI_CHAT.Assistant;

public enum IntentType
{
    Greeting,

    LearnExperience,

    AircraftAnalysis,

    ParameterQuestion,

    LogAnalysis,

    AircraftInfo,

    ExperienceSearch,

    Unknown
}

public class IntentDetector
{
    public IntentType Detect(string input)
    {
        input = input.ToLower();

        //---------------------------------
        // Greeting
        //---------------------------------

        if (input.Contains("salam") ||
            input.Contains("hello") ||
            input.Contains("hi"))
        {
            return IntentType.Greeting;
        }

        //---------------------------------
        // Aircraft Analysis
        //---------------------------------

        if (input.Contains("analyze aircraft") ||
            input.Contains("aircraft analysis") ||
            input.Contains("analyze uav") ||
            input.Contains("health report") ||
            input.Contains("check aircraft") ||
            input.Contains("analiz") ||
            input.Contains("analysis"))
        {
            return IntentType.AircraftAnalysis;
        }

        //---------------------------------
        // Parameters
        //---------------------------------

        if (input.Contains("pid") ||
            input.Contains("parametr"))
        {
            return IntentType.ParameterQuestion;
        }

        //---------------------------------
        // Aircraft Information
        //---------------------------------

        if (input.Contains("pua") ||
            input.Contains("aircraft info"))
        {
            return IntentType.AircraftInfo;
        }

        //---------------------------------
        // Flight Log
        //---------------------------------

        if (input.Contains("log") ||
            input.Contains("dataflash") ||
            input.Contains("flight"))
        {
            return IntentType.LogAnalysis;
        }

        //---------------------------------
        // Learning
        //---------------------------------

        if (input.Contains("learn") ||
            input.Contains("yadda saxla") ||
            input.Contains("oyren"))
        {
            return IntentType.LearnExperience;
        }

        //---------------------------------
        // Reasoning
        //---------------------------------

        if (input.Contains("vibration") ||
            input.Contains("problem") ||
            input.Contains("error"))
        {
            return IntentType.ExperienceSearch;
        }

        return IntentType.Unknown;
    }
}