using GCS.AI.Models;
using GCS.AI_CHAT.Models;
using System.IO;
using System.Text.Json;


namespace GCS.AI_CHAT.KnowledgeBase;


public class AircraftDatabase
{

    private List<AircraftProfile> aircrafts = new();



    public void Load()
    {

        string path =
        Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "AI CHAT",
            "Database",
            "Aircraft.JSON"
        );


        if (!File.Exists(path))
        {
            Console.WriteLine("JSON tapilmadi: " + path);
            return;
        }


        string json =
        File.ReadAllText(path);


        var data =
        JsonSerializer.Deserialize<List<AircraftProfile>>(json);


        if (data != null)
        {
            aircrafts = data;
        }


        Console.WriteLine("PUA yuklendi: " + aircrafts.Count);

    }



    public List<AircraftProfile> GetAircrafts()
    {
        return aircrafts;
    }


}