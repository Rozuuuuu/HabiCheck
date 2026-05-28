using CommunityToolkit.Mvvm.ComponentModel;

namespace HabiCheck.ViewModels;

/// <summary>
/// Base ViewModel for view models that interact with the Antigravity AI features.
/// Provides shared observable properties for loading states and error tracking.
/// </summary>
public partial class AntigravityViewModel : ObservableObject
{
    // 💡 DEVELOPER NOTE: [ObservableProperty] is a CommunityToolkit.Mvvm generator attribute.
    // It creates a public PascalCase property (like IsAiLoading) from a private field (like _isAiLoading)
    // and handles all the INotifyPropertyChanged event raising automatically.

    [ObservableProperty]
    private bool _isAiLoading;

    [ObservableProperty]
    private string? _aiError;
}
