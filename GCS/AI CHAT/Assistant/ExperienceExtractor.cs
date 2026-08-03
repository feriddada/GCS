using GCS.AI_CHAT.Models;


namespace GCS.AI_CHAT.Assistant;


public class ExperienceExtractor
{


    public Experience Extract(string text)
    {

        text = text.ToLower();


        Experience exp = new Experience();


        exp.Aircraft =
        "Custom 15kg Fixed Wing";



        if (text.Contains("vibration"))
        {
            exp.Problem =
            "vibration";
        }


        if (text.Contains("gps"))
        {
            exp.Problem =
            "gps problem";
        }


        if (text.Contains("battery"))
        {
            exp.Problem =
            "battery problem";
        }




        // CAUSE DETECTION 🧠


        if (text.Contains("motor"))
        {
            exp.Cause =
            "Motor problem";
        }


        else if (text.Contains("propeller"))
        {
            exp.Cause =
            "Propeller imbalance";
        }


        else if (text.Contains("frame"))
        {
            exp.Cause =
            "Frame resonance";
        }


        else
        {
            exp.Cause =
            "Unknown";
        }





        exp.Solution =
        text.Replace("learn", "")
            .Trim();



        exp.Solved =
        true;



        return exp;

    }



}