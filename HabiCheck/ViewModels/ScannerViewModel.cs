using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HabiCheck.Services;
using Microsoft.Win32;

namespace HabiCheck.ViewModels;

/// <summary>
/// ViewModel for the Fabric Scanner screen. Handles file selection and scanning simulation.
/// </summary>
public partial class ScannerViewModel : ObservableObject
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanScan))]
    private string _imagePath = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanScan))]
    private bool _isScanning;

    [ObservableProperty]
    private string _statusMessage = "Select a fabric image to begin the scan.";

    /// <summary>
    /// Checks if a scan can be started. True if an image is selected and we are not currently scanning.
    /// </summary>
    public bool CanScan => !string.IsNullOrEmpty(ImagePath) && !IsScanning;

    /// <summary>
    /// Opens a standard Windows file dialog to let the user pick a fabric image.
    /// </summary>
    [RelayCommand]
    private void SelectImage()
    {
        // 💡 DEVELOPER NOTE: Microsoft.Win32.OpenFileDialog is the standard WPF file picker.
        var openFileDialog = new OpenFileDialog
        {
            Title = "Select Fabric Image",
            Filter = "Image Files (*.jpg;*.jpeg;*.png)|*.jpg;*.jpeg;*.png|All Files (*.*)|*.*"
        };

        if (openFileDialog.ShowDialog() == true)
        {
            ImagePath = openFileDialog.FileName;
            StatusMessage = "Image selected. Click 'Scan Fabric' to analyze.";
        }
    }

    /// <summary>
    /// Simulates a fabric scan with delay, saves the record locally, and navigates to the result window.
    /// </summary>
    /// <param name="currentWindow">The current scanner window to close.</param>
    [RelayCommand]
    private async Task StartScanAsync(Window? currentWindow)
    {
        if (!CanScan) return;

        IsScanning = true;
        StatusMessage = "Analyzing weave pattern and fiber composition...";

        try
        {
            // Simulate scanning delays
            await Task.Delay(1000);
            StatusMessage = "Calculating breathability and sustainability indices...";
            await Task.Delay(1000);
            StatusMessage = "Cross-referencing with Cebu climate metrics...";
            await Task.Delay(800);

            // 💡 DEVELOPER NOTE: Randomly simulate either a good natural fabric (true) or a synthetic blend (false).
            bool isSuccess = Random.Shared.NextDouble() > 0.5;

            var profileId = SessionService.CurrentProfileId;
            if (string.IsNullOrEmpty(profileId))
            {
                MessageBox.Show("No active user session found. Please log in again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Build simulated fabric data based on result
            var fabric = App.Habi.BuildFabricResult(isSuccess);

            // Copy image to a local folder in AppData for persistence (to avoid path break if original file is moved/deleted)
            string relativeImagePath = string.Empty;
            try
            {
                string imagesFolder = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "HabiCheck", "Images");
                Directory.CreateDirectory(imagesFolder);

                string fileExtension = Path.GetExtension(ImagePath);
                string newFileName = $"{Guid.NewGuid()}{fileExtension}";
                string destPath = Path.Combine(imagesFolder, newFileName);
                
                File.Copy(ImagePath, destPath, overwrite: true);
                relativeImagePath = newFileName; // We store just the file name in the DB
            }
            catch (Exception ex)
            {
                // Fallback to absolute path if copying fails
                relativeImagePath = ImagePath;
                System.Diagnostics.Debug.WriteLine($"Failed to copy image: {ex.Message}");
            }

            // Save the scan record into the local SQLite database
            App.Database.SaveScan(profileId, fabric.Name, fabric.Grade, fabric.FiberType, relativeImagePath);

            // Fetch the newly created scan record (which is the most recent one) to get its database ID
            var latestScan = App.Habi.GetRecentScans(1).FirstOrDefault();
            if (latestScan == null)
            {
                throw new InvalidOperationException("Failed to retrieve the saved scan from the database.");
            }

            // Navigate to ResultWindow with the new scan's ID
            var resultWin = new Views.ResultWindow(latestScan.Id);
            resultWin.Show();
            currentWindow?.Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Scan failed: {ex.Message}", "Scan Error", MessageBoxButton.OK, MessageBoxImage.Error);
            StatusMessage = "Scan failed. Please try again.";
        }
        finally
        {
            IsScanning = false;
        }
    }

    /// <summary>
    /// Cancels and navigates back to the dashboard.
    /// </summary>
    [RelayCommand]
    private void Cancel(Window? currentWindow)
    {
        var dashboard = new Views.DashboardWindow();
        dashboard.Show();
        currentWindow?.Close();
    }
}
