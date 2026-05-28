using System;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HabiCheck.Models;
using HabiCheck.Services;

namespace HabiCheck.ViewModels;

/// <summary>
/// ViewModel for the Onboarding screen. Handles Hulas (sweat) level selection.
/// </summary>
public partial class OnboardingViewModel : ObservableObject
{
    [ObservableProperty]
    private HulasLevel _selectedHulas = HulasLevel.Pawisin;

    [ObservableProperty]
    private string _hulasLabel = string.Empty;

    [ObservableProperty]
    private string _hulasAdvice = string.Empty;

    public OnboardingViewModel()
    {
        // Set initial label and advice
        UpdatePersona();
    }

    // 💡 DEVELOPER NOTE: CommunityToolkit.Mvvm automatically generates partial methods 
    // named "On[PropertyName]Changed" when a property is decorated with [ObservableProperty].
    // This allows us to execute custom logic whenever the property is updated by the UI bindings.
    partial void OnSelectedHulasChanged(HulasLevel value)
    {
        UpdatePersona();
    }

    private void UpdatePersona()
    {
        var (label, advice) = App.Habi.GetHulasPersona(SelectedHulas);
        HulasLabel = label;
        HulasAdvice = advice;
    }

    /// <summary>
    /// Command to set the hulas level. Can be bound to buttons in the UI.
    /// </summary>
    [RelayCommand]
    private void SelectHulas(string levelName)
    {
        // 💡 DEVELOPER NOTE: Enum.TryParse converts the string parameter from the button command to our HulasLevel enum.
        if (Enum.TryParse<HulasLevel>(levelName, out var level))
        {
            SelectedHulas = level;
        }
    }

    /// <summary>
    /// Saves the selected hulas level to the database and session, then navigates to the dashboard.
    /// </summary>
    /// <param name="currentWindow">The onboarding window to close.</param>
    [RelayCommand]
    private void SaveAndProceed(Window? currentWindow)
    {
        try
        {
            App.Habi.SetHulas(SelectedHulas);

            var dashboard = new Views.DashboardWindow();
            dashboard.Show();
            currentWindow?.Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to save profile: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
