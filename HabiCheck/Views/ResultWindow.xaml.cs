using System.Windows;
using HabiCheck.ViewModels;

namespace HabiCheck.Views;

/// <summary>
/// Interaction logic for ResultWindow.xaml.
/// Displays detailed analysis of a just-scanned fabric and retrieves AI insights.
/// </summary>
public partial class ResultWindow : Window
{
    /// <summary>
    /// Initializes a new instance of ResultWindow.
    /// </summary>
    /// <param name="scanId">The ID of the scan record to display.</param>
    public ResultWindow(string scanId)
    {
        InitializeComponent();

        // 💡 DEVELOPER NOTE: Creating the view model and passing the scanId parameter.
        var vm = new ResultViewModel
        {
            ScanId = scanId
        };
        DataContext = vm;

        // Trigger loading database record and weather details
        this.Loaded += async (s, e) => await vm.LoadDataAsync();
    }
}
