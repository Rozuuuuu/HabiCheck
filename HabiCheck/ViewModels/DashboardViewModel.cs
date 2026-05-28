using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HabiCheck.Models;
using HabiCheck.Services;

namespace HabiCheck.ViewModels;

/// <summary>
/// ViewModel for the Dashboard screen. Displays recent scans, weather info, and sweat persona.
/// </summary>
public partial class DashboardViewModel : ObservableObject
{
    [ObservableProperty]
    private string _username = string.Empty;

    [ObservableProperty]
    private string _hulasLabel = string.Empty;

    [ObservableProperty]
    private string _hulasAdvice = string.Empty;

    [ObservableProperty]
    private WeatherInfo? _weather;

    [ObservableProperty]
    private string _humidityLabel = string.Empty;

    [ObservableProperty]
    private string _fabricAdvice = string.Empty;

    [ObservableProperty]
    private bool _isLoading;

    // 💡 DEVELOPER NOTE: ObservableCollection<T> is a special collection type in C#.
    // When items are added or removed, it automatically notifies WPF list elements (like ItemsControl)
    // to add or remove their UI list items without needing a full screen refresh.
    public ObservableCollection<ScanRecord> RecentScans { get; } = new();

    public DashboardViewModel()
    {
        Username = SessionService.CurrentUsername ?? "User";
        
        // 💡 DEVELOPER NOTE: We trigger the load operation on creation.
        // Because constructors cannot be async, we run a Task or call an async method.
        _ = LoadDataAsync();
    }

    /// <summary>
    /// Loads recent scans, local weather, and updates sweat advice.
    /// </summary>
    [RelayCommand]
    public async Task LoadDataAsync()
    {
        IsLoading = true;
        try
        {
            // 1. Get the current hulas persona details
            var hulas = App.Habi.GetHulas();
            var persona = App.Habi.GetHulasPersona(hulas);
            HulasLabel = persona.Label;
            HulasAdvice = persona.Advice;

            // 2. Fetch local weather
            // 💡 DEVELOPER NOTE: await releases the UI thread so the window stays responsive
            // while the background operation runs.
            Weather = await App.Habi.GetWeatherAsync();
            if (Weather != null)
            {
                HumidityLabel = App.Habi.HumidityLabel(Weather.Humidity);
                FabricAdvice = App.Habi.FabricAdvice(Weather.Humidity);
            }

            // 3. Get recent scans (limit to 3 for the dashboard summary)
            var scans = App.Habi.GetRecentScans(3);
            RecentScans.Clear();
            foreach (var scan in scans)
            {
                RecentScans.Add(scan);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading dashboard: {ex.Message}", "Load Error", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Navigates to the scanner screen.
    /// </summary>
    [RelayCommand]
    private void NavigateToScanner(Window? currentWindow)
    {
        var scanner = new Views.ScannerWindow();
        scanner.Show();
        currentWindow?.Close();
    }

    /// <summary>
    /// Navigates to the history screen.
    /// </summary>
    [RelayCommand]
    private void NavigateToHistory(Window? currentWindow)
    {
        var history = new Views.HistoryWindow();
        history.Show();
        currentWindow?.Close();
    }

    /// <summary>
    /// Navigates to the scan details screen.
    /// </summary>
    [RelayCommand]
    private void ViewScanDetail(ScanRecord? scan)
    {
        if (scan == null) return;

        // Find the active window to close it
        Window? activeWindow = null;
        foreach (Window window in Application.Current.Windows)
        {
            if (window.IsActive || window.DataContext == this)
            {
                activeWindow = window;
                break;
            }
        }

        var detail = new Views.ScanDetailWindow(scan.Id);
        detail.Show();
        activeWindow?.Close();
    }
}
