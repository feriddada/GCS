using GCS.Core.Domain;
using GCS.Core.Mavlink;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;

namespace GCS.ViewModels;

public class PreflightViewModel : ViewModelBase
{
    private IMavlinkBackend? _backend;

    private bool _allChecksPassed;
    private int _passedCount;
    private int _totalCount;
    private string _summary = "No data";
    private bool _isConnected;

    public ObservableCollection<PreflightCheckItemViewModel> Checks { get; } = new();

    public bool AllChecksPassed
    {
        get => _allChecksPassed;
        private set => SetProperty(ref _allChecksPassed, value);
    }

    public int PassedCount
    {
        get => _passedCount;
        private set => SetProperty(ref _passedCount, value);
    }

    public int TotalCount
    {
        get => _totalCount;
        private set => SetProperty(ref _totalCount, value);
    }

    public string Summary
    {
        get => _summary;
        private set => SetProperty(ref _summary, value);
    }

    public bool IsConnected
    {
        get => _isConnected;
        private set
        {
            if (SetProperty(ref _isConnected, value))
            {
                CommandManager.InvalidateRequerySuggested();
            }
        }
    }

    public string StatusColor => AllChecksPassed ? "#4CAF50" : "#F44336";

    // Commands
    public ICommand ForceArmCommand { get; }

    public PreflightViewModel()
    {
        ForceArmCommand = new RelayCommand(async () => await ForceArmAsync(), () => IsConnected);
    }

    /// <summary>
    /// Set the backend for sending commands. Called after connection.
    /// </summary>
    public void SetBackend(IMavlinkBackend backend)
    {
        _backend = backend;
    }

    /// <summary>
    /// Force arm - bypasses all preflight checks.
    /// Sends MAV_CMD_COMPONENT_ARM_DISARM (400) with param2 = 21196
    /// </summary>
    private async Task ForceArmAsync()
    {
        if (_backend == null) return;

        try
        {
            Debug.WriteLine("[PreflightViewModel] Sending FORCE ARM command...");

            // MAV_CMD_COMPONENT_ARM_DISARM = 400
            // param1 = 1 (arm)
            // param2 = 21196 (magic number to bypass checks)
            await _backend.SendCommandLongAsync(
                command: 400,
                param1: 1.0f,      // 1 = arm
                param2: 21196.0f   // bypass safety checks
            );

            Debug.WriteLine("[PreflightViewModel] FORCE ARM sent");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"[PreflightViewModel] FORCE ARM failed: {ex.Message}");
        }
    }

    public void UpdatePreflight(PreflightState state)
    {
        Checks.Clear();

        foreach (var check in state.Checks)
        {
            Checks.Add(new PreflightCheckItemViewModel(check));
        }

        PassedCount = state.Checks.Count(c => c.Status == PreflightCheckStatus.Passed);
        TotalCount = state.Checks.Count;
        AllChecksPassed = PassedCount == TotalCount && TotalCount > 0;

        Summary = AllChecksPassed
            ? "All checks passed"
            : $"{PassedCount}/{TotalCount} checks passed";

        OnPropertyChanged(nameof(StatusColor));
    }

    public void UpdateConnectionState(bool isConnected)
    {
        IsConnected = isConnected;
    }
}

public class PreflightCheckItemViewModel : ViewModelBase
{
    public string Name { get; }
    public PreflightCheckStatus Status { get; }
    public string? Reason { get; }
    public string StatusText { get; }
    public string StatusColor { get; }
    public string StatusIcon { get; }

    public PreflightCheckItemViewModel(PreflightCheckResult check)
    {
        Name = check.Name;
        Status = check.Status;
        Reason = check.Reason;

        StatusText = check.Status switch
        {
            PreflightCheckStatus.Passed => "PASS",
            PreflightCheckStatus.Failed => "FAIL",
            _ => "???"
        };

        StatusColor = check.Status switch
        {
            PreflightCheckStatus.Passed => "#4CAF50",
            PreflightCheckStatus.Failed => "#F44336",
            _ => "#9E9E9E"
        };

        StatusIcon = check.Status switch
        {
            PreflightCheckStatus.Passed => "✓",
            PreflightCheckStatus.Failed => "✗",
            _ => "?"
        };
    }
}