using System.Windows;
using HabiCheck.ViewModels;

namespace HabiCheck.Views;

/// <summary>
/// Interaction logic for OnboardingWindow.xaml.
/// Lets the user configure their Hulas sweat profile during first startup.
/// </summary>
public partial class OnboardingWindow : Window
{
    public OnboardingWindow()
    {
        InitializeComponent();
        
        // 💡 DEVELOPER NOTE: Connecting the View to OnboardingViewModel.
        DataContext = new OnboardingViewModel();
    }
}
