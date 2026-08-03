using System.Collections.ObjectModel;
using GCS.AI_CHAT.Models;
using GCS.AI_CHAT.Models.AI;

namespace GCS.ViewModels;

public class LiveAIViewModel : ViewModelBase
{
    //---------------------------------
    // Aircraft Status
    //---------------------------------

    private bool _flightControllerConnected;
    public bool FlightControllerConnected
    {
        get => _flightControllerConnected;
        set => SetProperty(ref _flightControllerConnected, value);
    }

    private double _batteryVoltage;
    public double BatteryVoltage
    {
        get => _batteryVoltage;
        set => SetProperty(ref _batteryVoltage, value);
    }

    private int _batteryRemaining;
    public int BatteryRemaining
    {
        get => _batteryRemaining;
        set => SetProperty(ref _batteryRemaining, value);
    }

    private int _gpsSatellites;
    public int GpsSatellites
    {
        get => _gpsSatellites;
        set => SetProperty(ref _gpsSatellites, value);
    }

    private string _flightMode = "--";
    public string FlightMode
    {
        get => _flightMode;
        set => SetProperty(ref _flightMode, value);
    }

    private bool _armed;
    public bool Armed
    {
        get => _armed;
        set => SetProperty(ref _armed, value);
    }

    //---------------------------------
    // AI Result
    //---------------------------------

    private int _overallHealth;
    public int OverallHealth
    {
        get => _overallHealth;
        set => SetProperty(ref _overallHealth, value);
    }

    private int _crashRisk;
    public int CrashRisk
    {
        get => _crashRisk;
        set => SetProperty(ref _crashRisk, value);
    }

    private string _prediction = "--";
    public string Prediction
    {
        get => _prediction;
        set => SetProperty(ref _prediction, value);
    }

    //---------------------------------
    // Live Feed
    //---------------------------------

    public ObservableCollection<LiveAIItem> Messages
        = new();

    //---------------------------------
    // Update HUD
    //---------------------------------

    public void Update(
        FlightData data,
        AIAnalysisResult analysis,
        List<AIMessage> aiMessages)
    {
        FlightControllerConnected =
            data.FlightControllerConnected;

        BatteryVoltage =
            data.BatteryVoltage;

        BatteryRemaining =
            data.BatteryRemaining;

        GpsSatellites =
            data.Satellites;

        FlightMode =
            data.FlightMode;

        Armed =
            data.Armed;

        OverallHealth =
            analysis.OverallHealthScore;

        CrashRisk =
            analysis.Prediction.CrashRisk;

        Prediction =
            analysis.SafeToFly
                ? "Aircraft Ready For Flight"
                : "Aircraft Inspection Required";

        Messages.Clear();

        foreach (var item in aiMessages)
        {
            Messages.Add(
                new LiveAIItem
                {
                    Time = DateTime.Now,
                    Component = item.Component,
                    Title = item.Title,
                    Message = item.Message,
                    Confidence = item.Confidence,
                    Warning = item.Warning,
                    Critical = item.Critical
                });
        }
    }

    //---------------------------------
    // Clear Feed
    //---------------------------------

    public void ClearFeed()
    {
        Messages.Clear();
    }
}