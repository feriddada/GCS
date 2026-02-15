using GCS.Core.Mavlink.Messages;
using System.Collections.ObjectModel;

namespace GCS.ViewModels;

public class RcChannelsViewModel : ViewModelBase
{
    private byte _channelCount;
    private byte _rssi;
    private string _lastUpdate = "N/A";

    public ObservableCollection<RcChannelItemViewModel> Channels { get; } = new();

    public byte ChannelCount
    {
        get => _channelCount;
        private set => SetProperty(ref _channelCount, value);
    }

    public byte Rssi
    {
        get => _rssi;
        private set => SetProperty(ref _rssi, value);
    }

    public string RssiPercent => Rssi == 255 ? "N/A" : $"{Rssi}%";

    public string LastUpdate
    {
        get => _lastUpdate;
        private set => SetProperty(ref _lastUpdate, value);
    }

    public RcChannelsViewModel()
    {
        // Initialize 18 channels
        for (int i = 1; i <= 18; i++)
        {
            Channels.Add(new RcChannelItemViewModel(i));
        }
    }

    public void UpdateChannels(RcChannelsData data)
    {
        ChannelCount = data.Chancount;
        Rssi = data.Rssi;
        LastUpdate = DateTime.Now.ToString("HH:mm:ss.fff");

        var values = data.ToArray();
        for (int i = 0; i < Math.Min(values.Length, Channels.Count); i++)
        {
            Channels[i].UpdateValue(values[i]);
        }

        OnPropertyChanged(nameof(RssiPercent));
    }
}

public class RcChannelItemViewModel : ViewModelBase
{
    private ushort _rawValue;
    private double _normalizedValue;

    public int ChannelNumber { get; }
    public string ChannelName => $"CH{ChannelNumber}";

    public ushort RawValue
    {
        get => _rawValue;
        private set => SetProperty(ref _rawValue, value);
    }

    public double NormalizedValue
    {
        get => _normalizedValue;
        private set => SetProperty(ref _normalizedValue, value);
    }

    public string BarColor => ChannelNumber switch
    {
        1 => "#2196F3",  // Roll - Blue
        2 => "#4CAF50",  // Pitch - Green
        3 => "#FF9800",  // Throttle - Orange
        4 => "#9C27B0",  // Yaw - Purple
        5 => "#F44336",  // Mode - Red
        _ => "#607D8B"   // Others - Gray
    };

    public RcChannelItemViewModel(int channelNumber)
    {
        ChannelNumber = channelNumber;
    }

    public void UpdateValue(ushort rawValue)
    {
        RawValue = rawValue;

        // Normalize PWM (1000-2000) to 0-100%
        if (rawValue == 0 || rawValue == 65535)
        {
            NormalizedValue = 0;
        }
        else
        {
            NormalizedValue = Math.Clamp((rawValue - 1000.0) / 10.0, 0, 100);
        }
    }
}