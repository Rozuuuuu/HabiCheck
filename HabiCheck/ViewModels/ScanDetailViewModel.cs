using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HabiCheck.Models;
using HabiCheck.Services;
using Newtonsoft.Json;

namespace HabiCheck.ViewModels;

/// <summary>
/// ViewModel for the Scan Detail screen. Displays a past scan and enables AI chat about the fabric.
/// </summary>
public partial class ScanDetailViewModel : AntigravityViewModel
{
    [ObservableProperty]
    private string _scanId = string.Empty;

    [ObservableProperty]
    private ScanRecord? _scan;

    [ObservableProperty]
    private FabricData? _fabric;

    [ObservableProperty]
    private string? _localImagePath;

    [ObservableProperty]
    private bool _isLoading;

    [ObservableProperty]
    private string _newChatMessage = string.Empty;

    [ObservableProperty]
    private bool _isChatOpen;

    public ObservableCollection<FabricChatMessage> ChatHistory { get; } = new();

    public ScanDetailViewModel()
    {
    }

    /// <summary>
    /// Loads scan information from the local database.
    /// </summary>
    [RelayCommand]
    public async Task LoadDataAsync()
    {
        IsLoading = true;
        try
        {
            if (string.IsNullOrEmpty(ScanId)) return;

            Scan = App.Habi.GetScanById(ScanId);
            if (Scan == null)
            {
                MessageBox.Show("Scan record not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            LocalImagePath = App.Habi.GetScanImagePath(Scan.ImagePath);

            bool isSuccess = Scan.Grade.Equals("A+", StringComparison.OrdinalIgnoreCase);
            Fabric = App.Habi.BuildFabricResult(isSuccess);

            // Populate initial helpful system/assistant message in chat
            ChatHistory.Clear();
            ChatHistory.Add(new FabricChatMessage("assistant", 
                $"Kumusta! Ako si Antigravity. Gusto mo ba pag-usapan ang {Fabric.Name} ({Fabric.FiberType}) sa init ng Cebu? " +
                "Tanungin mo ako tungkol sa airflow, amoy-araw, o paano ito alagaan."));
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to load details: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
            IsLoading = false;
        }
    }

    /// <summary>
    /// Toggles the drawer chat control.
    /// </summary>
    [RelayCommand]
    private void ToggleChat()
    {
        IsChatOpen = !IsChatOpen;
    }

    /// <summary>
    /// Sends a chat message to the Antigravity AI service and appends the response.
    /// </summary>
    [RelayCommand]
    public async Task SendChatMessageAsync()
    {
        if (string.IsNullOrWhiteSpace(NewChatMessage) || Scan == null || Fabric == null) return;

        string userMsg = NewChatMessage.Trim();
        NewChatMessage = string.Empty;

        // 1. Add user message to local chat history list
        ChatHistory.Add(new FabricChatMessage("user", userMsg));

        IsAiLoading = true;
        AiError = null;

        try
        {
            // 2. Prepare payload strings
            string scanRecordJson = JsonConvert.SerializeObject(Scan);
            var historyList = ChatHistory.Take(ChatHistory.Count - 1).ToList(); // Send history without the userMsg (it is sent in the next param)

            var hulas = App.Habi.GetHulas();
            var persona = App.Habi.GetHulasPersona(hulas);
            string hulasPersona = $"Sweat Level: {hulas}. Persona Label: {persona.Label}. Advice: {persona.Advice}";

            // 💡 DEVELOPER NOTE: Call Gemini API to get assistant reply
            string reply = await App.Antigravity.SendFabricChatMessageAsync(scanRecordJson, historyList, userMsg, hulasPersona);

            // 3. Add AI response to chat history
            ChatHistory.Add(new FabricChatMessage("assistant", reply));
        }
        catch (Exception ex)
        {
            AiError = $"Failed to reach AI: {ex.Message}";
            ChatHistory.Add(new FabricChatMessage("assistant", 
                $"Pasensya na, hindi ako makakonekta sa system: {ex.Message}"));
            System.Diagnostics.Debug.WriteLine($"Chat failed: {ex}");
        }
        finally
        {
            IsAiLoading = false;
        }
    }

    /// <summary>
    /// Navigates back to the Scan History list.
    /// </summary>
    [RelayCommand]
    private void NavigateBack(Window? currentWindow)
    {
        var history = new Views.HistoryWindow();
        history.Show();
        currentWindow?.Close();
    }
}
