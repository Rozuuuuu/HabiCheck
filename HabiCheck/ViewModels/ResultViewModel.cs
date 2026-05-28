using System;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HabiCheck.Models;
using HabiCheck.Services;
using Newtonsoft.Json;

namespace HabiCheck.ViewModels;

/// <summary>
/// ViewModel for the Scan Result screen. Displays calculated fabric details and loads AI insights.
/// </summary>
public partial class ResultViewModel : AntigravityViewModel
{
    [ObservableProperty]
    private string _scanId = string.Empty;

    [ObservableProperty]
    private ScanRecord? _scan;

    [ObservableProperty]
    private FabricData? _fabric;

    [ObservableProperty]
    private WeatherInfo? _weather;

    [ObservableProperty]
    private ScanInsight? _insight;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string? _localImagePath;

    public ResultViewModel()
    {
    }

    /// <summary>
    /// Loads scan details and weather data, then triggers the AI insights generation.
    /// </summary>
    [RelayCommand]
    public async Task LoadDataAsync()
    {
        IsLoading = true;
        try
        {
            if (string.IsNullOrEmpty(ScanId)) return;

            // 1. Fetch scan from local DB
            Scan = App.Habi.GetScanById(ScanId);
            if (Scan == null)
            {
                MessageBox.Show("Scan record not found in database.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // 2. Resolve image path
            LocalImagePath = App.Habi.GetScanImagePath(Scan.ImagePath);

            // 3. Build simulated fabric data
            bool isSuccess = Scan.Grade.Equals("A+", StringComparison.OrdinalIgnoreCase);
            Fabric = App.Habi.BuildFabricResult(isSuccess);

            // 4. Fetch weather info
            Weather = await App.Habi.GetWeatherAsync();

            IsLoading = false;

            // 5. Trigger AI scan insight loading in the background
            _ = LoadInsightAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to load result: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            IsLoading = false;
        }
    }

    /// <summary>
    /// Calls the Antigravity Service to generate AI insights based on fabric data, weather, and sweat profile.
    /// </summary>
    [RelayCommand]
    public async Task LoadInsightAsync()
    {
        if (Fabric == null || Weather == null) return;

        IsAiLoading = true;
        AiError = null;

        try
        {
            // Serialize models for prompt insertion
            string scanResultJson = JsonConvert.SerializeObject(Fabric);
            string weatherJson = JsonConvert.SerializeObject(Weather);

            var hulas = App.Habi.GetHulas();
            var persona = App.Habi.GetHulasPersona(hulas);
            string hulasPersona = $"Sweat Level: {hulas}. Persona Label: {persona.Label}. Advice: {persona.Advice}";

            // 💡 DEVELOPER NOTE: Calls the Anthropic Claude API via HttpClient
            Insight = await App.Antigravity.GenerateScanInsightAsync(scanResultJson, weatherJson, hulasPersona);
        }
        catch (Exception ex)
        {
            AiError = $"Unable to connect to Gemini AI: {ex.Message}";
            System.Diagnostics.Debug.WriteLine($"AI Insights failed: {ex}");
        }
        finally
        {
            IsAiLoading = false;
        }
    }

    /// <summary>
    /// Navigates back to the Dashboard window.
    /// </summary>
    [RelayCommand]
    private void NavigateToDashboard(Window? currentWindow)
    {
        var dashboard = new Views.DashboardWindow();
        dashboard.Show();
        currentWindow?.Close();
    }

    /// <summary>
    /// Navigates to the History window.
    /// </summary>
    [RelayCommand]
    private void NavigateToHistory(Window? currentWindow)
    {
        var history = new Views.HistoryWindow();
        history.Show();
        currentWindow?.Close();
    }
}
