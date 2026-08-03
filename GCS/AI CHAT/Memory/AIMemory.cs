using System.Text.Json;
using GCS.AI_CHAT.Models;
using System.IO;
using System.Diagnostics;


namespace GCS.AI_CHAT.Memory;


public class AIMemory
{


    private List<Experience> data = new();



    string path =
    Path.Combine(
        AppDomain.CurrentDomain.BaseDirectory,
        "AI CHAT",
        "Database",
        "Experience.JSON"
    );




    public AIMemory()
    {
        Load();
    }






    public void Load()
    {


        try
        {


            Directory.CreateDirectory(
                Path.GetDirectoryName(path)!
            );



            if (!File.Exists(path))
            {

                File.WriteAllText(
                    path,
                    "[]"
                );


                return;

            }




            string json =
            File.ReadAllText(path);




            if (string.IsNullOrWhiteSpace(json))
            {

                data = new();

                return;

            }





            var result =
            JsonSerializer.Deserialize<List<Experience>>(json);




            if (result != null)
            {
                data = result;
            }



            Debug.WriteLine(
            $"MEMORY LOADED: {data.Count} records");


        }



        catch (Exception ex)
        {


            Debug.WriteLine(
            "MEMORY LOAD ERROR: "
            + ex.Message);



            data = new();


        }


    }








    public void SaveExperience(Experience exp)
    {



        bool exists =
        data.Any(x =>

        x.Problem == exp.Problem
        &&
        x.Solution == exp.Solution

        );




        if (exists)
            return;





        data.Add(exp);





        string json =
        JsonSerializer.Serialize(
            data,
            new JsonSerializerOptions
            {
                WriteIndented = true
            });






        Directory.CreateDirectory(
            Path.GetDirectoryName(path)!
        );




        File.WriteAllText(
            path,
            json
        );





        Debug.WriteLine(
        "========= MEMORY SAVED =========");


        Debug.WriteLine(path);


    }








    public List<Experience> Search(string problem)
    {



        problem =
        problem.ToLower();





        var result =
        data
        .Where(x =>

        problem.Contains(
        x.Problem.ToLower())

        ||

        x.Problem.ToLower()
        .Contains(problem)

        )
        .ToList();





        Debug.WriteLine(
        $"SEARCH FOUND: {result.Count}");




        return result;


    }



}