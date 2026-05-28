using System.Windows;
using HabiCheck.ViewModels;

namespace HabiCheck.Views;

/// <summary>
/// Interaction logic for ScannerWindow.xaml.
/// Houses the image selection and mock scanner processor.
/// </summary>
public partial class ScannerWindow : Window
{
    public ScannerWindow()
    {
        InitializeComponent();
        
        // 💡 DEVELOPER NOTE: Creating and setting the view model data context.
        DataContext = new ScannerViewModel();
    }
}
