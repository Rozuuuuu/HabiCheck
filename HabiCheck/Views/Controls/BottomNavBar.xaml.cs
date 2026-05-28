using System;
using System.Windows;
using System.Windows.Controls;

namespace HabiCheck.Views.Controls;

/// <summary>
/// Interaction logic for BottomNavBar.xaml.
/// Handles self-contained navigation between the primary application screens.
/// </summary>
public partial class BottomNavBar : UserControl
{
    public BottomNavBar()
    {
        InitializeComponent();
    }

    private void Dashboard_Click(object sender, RoutedEventArgs e)
    {
        NavigateTo(new DashboardWindow());
    }

    private void Scanner_Click(object sender, RoutedEventArgs e)
    {
        NavigateTo(new ScannerWindow());
    }

    private void History_Click(object sender, RoutedEventArgs e)
    {
        NavigateTo(new HistoryWindow());
    }

    /// <summary>
    /// Helper method to show the target window and close the parent window containing this nav bar.
    /// </summary>
    private void NavigateTo(Window targetWindow)
    {
        // 💡 DEVELOPER NOTE: Window.GetWindow(this) retrieves the parent Window containing this control.
        var parentWindow = Window.GetWindow(this);
        
        // Prevent open/close flicker by checking if the target window is already the current window
        if (parentWindow != null && parentWindow.GetType() == targetWindow.GetType())
        {
            return;
        }

        targetWindow.Show();
        parentWindow?.Close();
    }
}
