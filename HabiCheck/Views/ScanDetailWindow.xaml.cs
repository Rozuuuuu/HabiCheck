using System.Windows;
using HabiCheck.ViewModels;

namespace HabiCheck.Views;

/// <summary>
/// Interaction logic for ScanDetailWindow.xaml.
/// Shows past scan results and embeds the Antigravity interactive chat drawer.
/// </summary>
public partial class ScanDetailWindow : Window
{
    /// <summary>
    /// Initializes a new instance of ScanDetailWindow.
    /// </summary>
    /// <param name="scanId">The ID of the scan record to load.</param>
    public ScanDetailWindow(string scanId)
    {
        InitializeComponent();

        // 💡 DEVELOPER NOTE: Creating the view model and assigning the scan ID.
        var vm = new ScanDetailViewModel
        {
            ScanId = scanId
        };
        DataContext = vm;

        // Trigger loading the scan record from the database
        this.Loaded += async (s, e) => await vm.LoadDataAsync();
    }
}
