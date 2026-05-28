using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace HabiCheck.Converters;

/// <summary>
/// Converts the message role ("user" or "assistant") to the appropriate bubble background brush.
/// User messages get CreamBrush; Assistant messages get DeepSageBrush.
/// </summary>
public class ChatBubbleColorConverter : IValueConverter
{
    // 💡 DEVELOPER NOTE: Brushes are WPF drawing objects. We create static fallbacks in case 
    // resource lookups fail during design time or early initialization.
    private static readonly SolidColorBrush CreamBrush = new(Color.FromRgb(0xF5, 0xF0, 0xE8));
    private static readonly SolidColorBrush DeepSageBrush = new(Color.FromRgb(0x2D, 0x4A, 0x3E));

    /// <summary>
    /// Returns the background Brush based on message sender's role.
    /// </summary>
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string role)
        {
            if (role.Equals("user", StringComparison.OrdinalIgnoreCase))
            {
                return Application.Current?.TryFindResource("CreamBrush") as Brush ?? CreamBrush;
            }
            else
            {
                return Application.Current?.TryFindResource("DeepSageBrush") as Brush ?? DeepSageBrush;
            }
        }
        return DeepSageBrush;
    }

    /// <summary>
    /// Not implemented as converting brush back to role is not supported.
    /// </summary>
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
