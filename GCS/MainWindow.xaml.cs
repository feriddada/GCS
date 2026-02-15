using GCS.Core.Mavlink;
using GCS.ViewModels;
using System.Windows;
using System.Windows.Input;

namespace GCS;

public partial class MainWindow : Window
{
    private readonly MainViewModel _viewModel;

    public MainWindow()
    {
        InitializeComponent();

        // Initialize MAVLink
        MavlinkBootstrap.Init();

        // Create and set ViewModel
        _viewModel = new MainViewModel();
        DataContext = _viewModel;

        MouseLeftButtonDown += (s, e) =>
        {
            if (e.ButtonState == MouseButtonState.Pressed)
                DragMove();
        };
    }

    private void LinkButton_Click(object sender, RoutedEventArgs e)
    {
        // Toggle connection popup
        ConnectionPopup.Visibility = 
            ConnectionPopup.Visibility == Visibility.Visible 
                ? Visibility.Collapsed 
                : Visibility.Visible;
    }

    private void MinimizeButton_Click(object sender, RoutedEventArgs e)
    {
        WindowState = WindowState.Minimized;
    }

    private void MaximizeButton_Click(object sender, RoutedEventArgs e)
    {
        WindowState = WindowState == WindowState.Maximized 
            ? WindowState.Normal 
            : WindowState.Maximized;
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        Close();
    }

    protected override async void OnClosing(System.ComponentModel.CancelEventArgs e)
    {
        // Clean shutdown
        await _viewModel.ShutdownAsync();
        base.OnClosing(e);
    }
    private void TopBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (e.ClickCount == 2)
        {
            MaximizeButton_Click(sender, e);
        }
        else
        {
            DragMove();
        }
    }
}
