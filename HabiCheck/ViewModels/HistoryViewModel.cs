using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HabiCheck.Models;
using HabiCheck.Services;
using Newtonsoft.Json;

namespace HabiCheck.ViewModels;

/// <summary>
/// ViewModel for the Scan History screen. Displays all past scans and generates an AI weekly digest.
/// </summary>
public partial class HistoryViewModel : AntigravityViewModel
{
    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasDigest))]
    private DigestResult? _digest;

    public bool HasDigest => Digest != null;

    public ObservableCollection<ScanRecord> Scans { get; } = new();

    public HistoryViewModel()
    {
        // Auto-load history when ViewModel is created
        _ = LoadHistoryAsync();
    }

    /// <summary>
    /// Loads all past scans for the current profile from the local SQLite database.
    /// </summary>
    [RelayCommand]
    public async Task LoadHistoryAsync()
    {
        IsLoading = true;
        try
        {
            // 💡 DEVELOPER NOTE: App.Habi.GetRecentScans(null) passes null for the limit
            // which returns ALL scans for this user.
            var list = App.Habi.GetRecentScans(null);
            
            Scans.Clear();
            foreach (var scan in list)
            {
                Scans.Add(scan);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to load history: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Deletes a scan from the database and refreshes the history list.
    /// </summary>
    [RelayCommand]
    private async Task DeleteScanAsync(string scanId)
    {
        if (string.IsNullOrEmpty(scanId)) return;

        var result = MessageBox.Show("Are you sure you want to delete this scan?", "Confirm Delete", 
            MessageBoxButton.YesNo, MessageBoxImage.Question);

        if (result == MessageBoxResult.Yes)
        {
            try
            {
                App.Database.DeleteScan(scanId);
                await LoadHistoryAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to delete scan: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    /// <summary>
    /// Gathers all scans, serializes them, and requests an AI-generated ecological digest from Gemini.
    /// </summary>
    [RelayCommand]
    public async Task GenerateDigestAsync()
    {
        if (Scans.Count == 0)
        {
            MessageBox.Show("Please scan at least one fabric first before generating a digest.", "No History", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        IsAiLoading = true;
        AiError = null;

        try
        {
            // 1. Serialize all scans for analysis
            string scansJson = JsonConvert.SerializeObject(Scans);

            // 2. Prepare hulas profile info
            var hulas = App.Habi.GetHulas();
            var persona = App.Habi.GetHulasPersona(hulas);
            string hulasPersona = $"Sweat Level: {hulas}. Persona Label: {persona.Label}. Advice: {persona.Advice}";

            // 3. Request analysis from AntigravityService
            Digest = await App.Antigravity.GenerateHistoryDigestAsync(scansJson, hulasPersona);
        }
        catch (Exception ex)
        {
            AiError = $"Unable to generate weekly digest: {ex.Message}";
            System.Diagnostics.Debug.WriteLine($"Digest failed: {ex}");
        }
        finally
        {
            IsAiLoading = false;
        }
    }

    /// <summary>
    /// Navigates to the details page of a specific scan.
    /// </summary>
    [RelayCommand]
    private void ViewScanDetail(ScanRecord? scan)
    {
        if (scan == null) return;

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

    /// <summary>
    /// Navigates to the Dashboard.
    /// </summary>
    [RelayCommand]
    private void NavigateToDashboard(Window? currentWindow)
    {
        var dashboard = new Views.DashboardWindow();
        dashboard.Show();
        currentWindow?.Close();
    }

    /// <summary>
    /// Navigates to the Scanner screen.
    /// </summary>
    [RelayCommand]
    private void NavigateToScanner(Window? currentWindow)
    {
        var scanner = new Views.ScannerWindow();
        scanner.Show();
        currentWindow?.Close();
    }
}
