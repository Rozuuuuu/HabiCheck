using System.Windows;
using HabiCheck.ViewModels;

namespace HabiCheck.Views;

/// <summary>
/// Interaction logic for HistoryWindow.xaml.
/// Lists past scan records and integrates the AI Closet Audit Card.
/// </summary>
public partial class HistoryWindow : Window
{
    public HistoryWindow()
    {
        InitializeComponent();
        
        // 💡 DEVELOPER NOTE: Instantiating and setting the data context view model.
        DataContext = new HistoryViewModel();
    }
}
