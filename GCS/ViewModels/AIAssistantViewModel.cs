using CommunityToolkit.Mvvm.Input;
using GCS.AI_CHAT.Assistant;
using GCS.AI_CHAT.Models;
using GCS.AI_CHAT.Models.AI;
using System.Collections.ObjectModel;


namespace GCS.ViewModels;


public partial class AIAssistantViewModel : ViewModelBase
{


    public ObservableCollection<string> Messages { get; }
    = new();
    public ObservableCollection<AIMessage> LiveMessages { get; }
    = new();
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
 
    private readonly AIAssistant ai =
    new();

    private AircraftReport _report = new();

    public AircraftReport Report
    {
        get => _report;
        set => SetProperty(ref _report, value);
    }

    private string _input = "";


    public string Input
    {
        get => _input;

        set => SetProperty(ref _input, value);
    }



    public RelayCommand SendCommand { get; }

    public RelayCommand AnalyzeCommand { get; }

    public RelayCommand ClearCommand { get; }

    public AIAssistantViewModel()
    {
        SendCommand =
            new RelayCommand(Send);

        AnalyzeCommand =
            new RelayCommand(AnalyzeAircraft);

        ClearCommand =
            new RelayCommand(Clear);
    }


    private void Clear()
    {
        LiveMessages.Clear();
    }

    private void Send()
    {

        if (string.IsNullOrWhiteSpace(Input))
            return;



        Messages.Add(
        "Operator: " + Input);



        UAVState state = new UAVState()
        {
            BatteryVoltage = 16,
            GpsSatellites = 12
        };



        AIResponse answer =
        ai.Ask(Input, state);



        Messages.Add(
        "NAA AI: " + answer.Message);



        Input = "";

    }
    public void UpdateHUD(
    FlightData data,
    AIAnalysisResult result,
    List<AIMessage> messages)
    {
        BatteryVoltage = data.BatteryVoltage;
        BatteryRemaining = data.BatteryRemaining;
        GpsSatellites = data.Satellites;
        FlightMode = data.FlightMode;
        Armed = data.Armed;

        OverallHealth = result.OverallHealthScore;
        CrashRisk = result.Prediction.CrashRisk;

        Prediction = result.SafeToFly
            ? "Aircraft Ready For Flight"
            : "Aircraft Inspection Required";

        LiveMessages.Clear();

        foreach (var message in messages)
        {
            LiveMessages.Add(message);
        }
    }
    private string _lastReport = "";


    private void AnalyzeAircraft()
    {
        Report = new AircraftReport
        {
            BatteryStatus = BatteryVoltage < 5
                ? "Battery telemetry unavailable"
                : $"Battery Voltage : {BatteryVoltage:F2} V",

            GPSStatus = $"{GpsSatellites} Satellites",

            FlightMode = FlightMode,

            OverallHealth = $"{OverallHealth}%",

            CrashRisk = $"{CrashRisk}%",

            Recommendation = Prediction

        };
        OnPropertyChanged(nameof(Report));
    }
}