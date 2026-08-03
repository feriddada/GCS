namespace GCS.AI.Models;


public class ParameterChange
{

    public string ParameterName { get; set; } = "";


    public double OldValue { get; set; }


    public double NewValue { get; set; }


    public bool Successful { get; set; }

}