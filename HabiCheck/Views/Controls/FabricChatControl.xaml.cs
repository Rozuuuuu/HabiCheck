using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using HabiCheck.ViewModels;

namespace HabiCheck.Views.Controls;

/// <summary>
/// Interaction logic for FabricChatControl.xaml.
/// Enables user discussion with the Antigravity Gemini AI.
/// </summary>
public partial class FabricChatControl : UserControl
{
    public FabricChatControl()
    {
        InitializeComponent();
        this.DataContextChanged += FabricChatControl_DataContextChanged;
    }

    private void FabricChatControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        // Unsubscribe from old ViewModel events
        if (e.OldValue is ScanDetailViewModel oldVm)
        {
            oldVm.ChatHistory.CollectionChanged -= ChatHistory_CollectionChanged;
        }

        // Subscribe to new ViewModel events to trigger auto-scroll on new messages
        if (e.NewValue is ScanDetailViewModel newVm)
        {
            newVm.ChatHistory.CollectionChanged += ChatHistory_CollectionChanged;
            ScrollToBottom();
        }
    }

    private void ChatHistory_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        ScrollToBottom();
    }

    /// <summary>
    /// Forces the ScrollViewer to scroll down to display the latest chat bubble.
    /// </summary>
    private void ScrollToBottom()
    {
        // 💡 DEVELOPER NOTE: Dispatcher.BeginInvoke delays the execution slightly,
        // allowing the WPF layout engine to finish rendering the new bubble item before we scroll down.
        Dispatcher.BeginInvoke(new Action(() => ChatScrollViewer.ScrollToEnd()), System.Windows.Threading.DispatcherPriority.Background);
    }

    private void TextBox_KeyDown(object sender, KeyEventArgs e)
    {
        // 💡 DEVELOPER NOTE: Capturing the Enter key makes the chat input feel intuitive and premium,
        // matching web-app chat experiences.
        if (e.Key == Key.Enter)
        {
            if (DataContext is ScanDetailViewModel vm)
            {
                if (vm.SendChatMessageCommand.CanExecute(null))
                {
                    vm.SendChatMessageCommand.Execute(null);
                    e.Handled = true;
                }
            }
        }
    }

    private void Close_Click(object sender, RoutedEventArgs e)
    {
        if (DataContext is ScanDetailViewModel vm)
        {
            vm.IsChatOpen = false;
        }
    }
}
