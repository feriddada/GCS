using GCS.Core.Domain;
using GCS.Core.Mavlink;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;
using FlightModeEnum = GCS.Core.Domain.FlightMode;

namespace GCS.ViewModels;

public class ActionsViewModel : ViewModelBase
{
    private readonly IMavlinkBackend _backend;

    private string _flightMode = "UNKNOWN";
    private int _selectedModeIndex = -1;
    private bool _isConnected;

    public string FlightMode
    {
        get => _flightMode;
        set => SetProperty(ref _flightMode, value);
    }

    public int SelectedModeIndex
    {
        get => _selectedModeIndex;
        set => SetProperty(ref _selectedModeIndex, value);
    }

    public bool IsConnected
    {
        get => _isConnected;
        set
        {
            if (SetProperty(ref _isConnected, value))
            {
                CommandManager.InvalidateRequerySuggested();
            }
        }
    }

    // Commands
    public ICommand ArmCommand { get; }
    public ICommand DisarmCommand { get; }
    public ICommand RtlCommand { get; }
    public ICommand LoiterCommand { get; }
    public ICommand LandCommand { get; }
    public ICommand SetModeCommand { get; }

    public ActionsViewModel(IMavlinkBackend backend)
    {
        _backend = backend;

        ArmCommand = new RelayCommand(async () => await ArmAsync(), () => IsConnected);
        DisarmCommand = new RelayCommand(async () => await DisarmAsync(), () => IsConnected);
        RtlCommand = new RelayCommand(async () => await SetModeAsync(FlightModeEnum.Rtl), () => IsConnected);
        LoiterCommand = new RelayCommand(async () => await SetModeAsync(FlightModeEnum.Loiter), () => IsConnected);
        LandCommand = new RelayCommand(async () => await SetModeAsync(FlightModeEnum.QLand), () => IsConnected);
        SetModeCommand = new RelayCommand(async () => await SetSelectedModeAsync(), () => IsConnected && SelectedModeIndex >= 0);
    }

    private async Task ArmAsync()
    {
        try
        {
            Debug.WriteLine("[ActionsViewModel] Sending ARM command...");
            await _backend.SendArmDisarmAsync(arm: true);
            Debug.WriteLine("[ActionsViewModel] ARM sent");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[ActionsViewModel] ARM failed: {ex.Message}");
        }
    }

    private async Task DisarmAsync()
    {
        try
        {
            Debug.WriteLine("[ActionsViewModel] Sending DISARM command...");
            await _backend.SendArmDisarmAsync(arm: false);
            Debug.WriteLine("[ActionsViewModel] DISARM sent");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[ActionsViewModel] DISARM failed: {ex.Message}");
        }
    }

    private async Task SetModeAsync(FlightModeEnum mode)
    {
        try
        {
            Debug.WriteLine($"[ActionsViewModel] Setting mode to {mode}...");

            uint customMode = ArdupilotPlaneFlightModeMapper.ToCustomMode(mode);
            byte baseMode = 81; // MAV_MODE_FLAG_CUSTOM_MODE_ENABLED

            await _backend.SendSetModeAsync(baseMode, customMode);
            Debug.WriteLine($"[ActionsViewModel] Mode {mode} sent (custom_mode={customMode})");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[ActionsViewModel] SetMode failed: {ex.Message}");
        }
    }

    private async Task SetSelectedModeAsync()
    {
        FlightModeEnum? mode = SelectedModeIndex switch
        {
            0 => FlightModeEnum.Manual,
            1 => FlightModeEnum.Stabilize,
            2 => FlightModeEnum.Fbwa,
            3 => FlightModeEnum.Fbwb,
            4 => FlightModeEnum.Auto,
            5 => FlightModeEnum.Rtl,
            6 => FlightModeEnum.Loiter,
            7 => FlightModeEnum.Circle,
            8 => FlightModeEnum.Guided,
            9 => FlightModeEnum.Cruise,
            10 => FlightModeEnum.Autotune,
            11 => FlightModeEnum.QStabilize,
            12 => FlightModeEnum.QHover,
            13 => FlightModeEnum.QLoiter,
            14 => FlightModeEnum.QLand,
            15 => FlightModeEnum.QRtl,
            16 => FlightModeEnum.Acro,

            _ => null
        };

        if (mode.HasValue)
        {
            await SetModeAsync(mode.Value);
        }
    }

    public void UpdateFromVehicleState(VehicleState state)
    {
        if (state.FlightMode.HasValue)
        {
            FlightMode = state.FlightMode.Value.ToString().ToUpper();
        }

        // Use multiple indicators to determine connection status
        // If we're receiving flight mode OR position OR attitude, we're connected
        bool hasData = state.FlightMode.HasValue ||
                       state.Position != null ||
                       state.Attitude != null;

        // Also check Connection state if available
        bool connectionOk = state.Connection?.IsConnected == true;

        IsConnected = hasData || connectionOk;
    }
}