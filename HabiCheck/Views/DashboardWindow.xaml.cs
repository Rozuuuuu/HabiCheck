using System.Windows;
using HabiCheck.Services;
using HabiCheck.ViewModels;

namespace HabiCheck.Views;

/// <summary>
/// Interaction logic for DashboardWindow.xaml.
/// Displays summary cards for weather, sweat persona, and recent scans.
/// </summary>
public partial class DashboardWindow : Window
{
    public DashboardWindow()
    {
        InitializeComponent();
        
        // 💡 DEVELOPER NOTE: Creating and setting the view model data context.
        DataContext = new DashboardViewModel();
    }

    private void Logout_Click(object sender, RoutedEventArgs e)
    {
        // 1. Clear session cache
        SessionService.Clear();

        // 2. Return to login screen
        var login = new LoginWindow();
        login.Show();
        this.Close();
    }
}
