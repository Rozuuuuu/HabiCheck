using System;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HabiCheck.Services;

namespace HabiCheck.ViewModels;

/// <summary>
/// ViewModel for the Login screen. Handles user lookup and profile creation.
/// </summary>
public partial class LoginViewModel : ObservableObject
{
    // 💡 DEVELOPER NOTE: [ObservableProperty] is used here to expose fields to the WPF XAML bindings.
    // When these properties change, they notify the UI to refresh automatically.

    [ObservableProperty]
    private string _username = string.Empty;

    [ObservableProperty]
    private string? _errorMessage;

    /// <summary>
    /// Attempts to log in using the entered username.
    /// </summary>
    /// <param name="currentWindow">The current login window, to be closed upon successful login.</param>
    [RelayCommand]
    private void Login(Window? currentWindow)
    {
        ErrorMessage = null;

        if (string.IsNullOrWhiteSpace(Username))
        {
            ErrorMessage = "Username cannot be empty.";
            return;
        }

        // 💡 DEVELOPER NOTE: App.Database is accessed statically. This replaces a Dependency Injection container.
        var profile = App.Database.GetProfileByUsername(Username.Trim());

        if (profile.HasValue)
        {
            // Set the session state in our static memory store
            SessionService.CurrentProfileId = profile.Value.Id;
            SessionService.CurrentUsername = profile.Value.Username;
            SessionService.CurrentHulasLevel = profile.Value.HulasLevel;

            // 💡 DEVELOPER NOTE: WPF navigation is done here by creating the next Window,
            // calling .Show(), and then closing the current login window.
            var dashboard = new Views.DashboardWindow();
            dashboard.Show();
            currentWindow?.Close();
        }
        else
        {
            ErrorMessage = "Username not found. Click 'Register' to create a new profile.";
        }
    }

    /// <summary>
    /// Creates a new profile with the entered username and forwards the user to onboarding.
    /// </summary>
    /// <param name="currentWindow">The current login window, to be closed upon successful registration.</param>
    [RelayCommand]
    private void Register(Window? currentWindow)
    {
        ErrorMessage = null;

        if (string.IsNullOrWhiteSpace(Username))
        {
            ErrorMessage = "Username cannot be empty.";
            return;
        }

        var existing = App.Database.GetProfileByUsername(Username.Trim());
        if (existing.HasValue)
        {
            ErrorMessage = "Username is already taken.";
            return;
        }

        try
        {
            // Create a new user with default 'Pawisin' hulas level
            string newId = App.Database.CreateProfile(Username.Trim(), "Pawisin");

            SessionService.CurrentProfileId = newId;
            SessionService.CurrentUsername = Username.Trim();
            SessionService.CurrentHulasLevel = "Pawisin";

            // Navigate to onboarding window so they can set their real hulas level
            var onboarding = new Views.OnboardingWindow();
            onboarding.Show();
            currentWindow?.Close();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to register: {ex.Message}";
        }
    }
}
