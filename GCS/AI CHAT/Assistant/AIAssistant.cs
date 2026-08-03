using GCS.AI_CHAT.Models;
using GCS.AI_CHAT.KnowledgeBase;
using GCS.AI_CHAT.Memory;
using GCS.AI_CHAT.Reasoning;
using GCS.AI_CHAT.Engine;
using GCS.AI_CHAT.Response;
using GCS.AI_CHAT.Mapper;
namespace GCS.AI_CHAT.Assistant;


public class AIAssistant
{


    private readonly IntentDetector intent =
    new IntentDetector();


    private readonly AircraftDatabase aircraftDB =
    new AircraftDatabase();


    private readonly AIMemory memory =
    new AIMemory();


    private readonly ExperienceExtractor extractor =
    new ExperienceExtractor();


    private readonly ProblemReasoner reasoner =
    new ProblemReasoner();
    private readonly AIAnalysisEngine analysisEngine =
        new();

    private readonly AIResponseBuilder responseBuilder =
        new();
    private readonly FlightDataMapper mapper =
    new();

    public AIAssistant()
    {

        aircraftDB.Load();

        memory.Load();

    }


    public AIResponse Ask(string question, UAVState state)
    {


        AIResponse response =
        new AIResponse();



        var type =
        intent.Detect(question);



        // =====================
        // GREETING
        // =====================

        if (type == IntentType.Greeting)
        {

            return new AIResponse
            {

                Message =
                """
                Salam. Mən Milli Aviasiya Akademiyasının
                PUA analitik köməkçi sistemiyəm.


                Kömək edə bilərəm:

                ✓ Aircraft Analysis

                ✓ Flight Health Monitoring

                ✓ DataFlash Crash Analysis

                ✓ Live AI Monitoring

                ✓ Parameter Recommendation

                ✓ Fault Detection

                ✓ Experience Learning
                """

            };

        }



        // =====================
        // AIRCRAFT ANALYSIS
        // =====================

        // =====================
        // AIRCRAFT ANALYSIS
        // =====================

        if (type == IntentType.AircraftAnalysis)
        {
            //---------------------------------
            // Convert UAVState -> FlightData
            //---------------------------------

            FlightData data =
                mapper.Map(state);

            //---------------------------------
            // AI Analysis
            //---------------------------------

            var analysis =
                analysisEngine.Analyze(data);

            //---------------------------------
            // Build Response
            //---------------------------------

            return new AIResponse
            {
                Message =
                    responseBuilder.Build(analysis)
            };
        }



        // =====================
        // LEARNING MEMORY
        // =====================


        if (type == IntentType.LearnExperience)
        {

            var exp =
            extractor.Extract(question);



            memory.SaveExperience(exp);



            return new AIResponse
            {

                Message =
                $"""
                ✓ Yeni təcrübə yadda saxlanıldı


                Problem:
                {exp.Problem}
                
                 Səbəb:
                {exp.Cause}


                Həll:
                {exp.Solution}
                """

            };

        }
        // =====================
        // AIRCRAFT DATABASE
        // =====================


        if (type == IntentType.AircraftInfo)
        {


            var list =
            aircraftDB.GetAircrafts();



            string text =
            "Bazadakı PUA-lar:\n\n";

            foreach (var uav in list)
            {


                text +=
                $"""
                ====================

                PUA:
                {uav.Name}


                Tip:
                {uav.Type}


                MTOW:
                {uav.MTOW} kg


                Qanad:
                {uav.WingSpan} m


                Sahə:
                {uav.WingArea} m²


                Cruise:
                {uav.CruiseSpeed} m/s


                Stall:
                {uav.StallSpeed} m/s


                Flight Controller:
                {uav.FlightController}


                ====================


                """;


            }



            return new AIResponse
            {
                Message = text
            };


        }
        // =====================
        // REASONING ENGINE 🧠
        // =====================



        if (type == IntentType.ExperienceSearch)
        {


            var decision =
            reasoner.Analyze(question);
            return new AIResponse
            {

                Message =
                $"""
                🧠 AI ANALYSIS RESULT


                Problem:
                {decision.Problem}


                Possible Cause:
                {decision.MostLikelyCause}


                Confidence:
                {decision.Confidence}%


                Recommendation:
                {decision.Recommendation}
                """

            };


        }






        // =====================
        // REAL TIME CHECKS
        // =====================
        // TODO:
        // Replace with AIAnalysisEngine
        // after Live AI module is completed.

        if (state.BatteryVoltage < 14)
        {


            response.Message +=
            """
            ⚠ BATTERY WARNING


            Possible:
            - Low battery
            - Voltage sag


            Recommendation:
            Prepare RTL

            """;


        }






        // =====================
        // UNKNOWN
        // =====================


        if (string.IsNullOrWhiteSpace(response.Message))
        {

            response.Message =
            "Məlumat qəbul edildi. Analiz üçün PUA datası gözlənilir.";

        }



        return response;



    }


}