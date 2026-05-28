using System.Windows;
using HabiCheck.ViewModels;

namespace HabiCheck.Views;

/// <summary>
/// Interaction logic for LoginWindow.xaml.
/// Serves as the application startup authentication portal.
/// </summary>
public partial class LoginWindow : Window
{
    public LoginWindow()
    {
        InitializeComponent();
        
        // 💡 DEVELOPER NOTE: We set the DataContext of the Window to connect our View with the ViewModel.
        // Once set, all {Binding} declarations in the XAML will automatically wire up to properties in LoginViewModel.
        DataContext = new LoginViewModel();
    }
}
