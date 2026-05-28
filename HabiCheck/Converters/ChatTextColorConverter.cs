using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace HabiCheck.Converters;

/// <summary>
/// Converts the message role ("user" or "assistant") to the appropriate text color brush.
/// User messages (cream bubble) get DeepSageBrush; Assistant messages (deep sage bubble) get CreamBrush.
/// </summary>
public class ChatTextColorConverter : IValueConverter
{
    // 💡 DEVELOPER NOTE: We define static fallbacks to guarantee text readability in case 
    // global resource dictionaries are not resolved in design time.
    private static readonly SolidColorBrush DeepSageBrush = new(Color.FromRgb(0x2D, 0x4A, 0x3E));
    private static readonly SolidColorBrush CreamBrush = new(Color.FromRgb(0xF5, 0xF0, 0xE8));

    /// <summary>
    /// Returns the text color Brush based on the role.
    /// </summary>
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string role)
        {
            if (role.Equals("user", StringComparison.OrdinalIgnoreCase))
            {
                return Application.Current?.TryFindResource("DeepSageBrush") as Brush ?? DeepSageBrush;
            }
            else
            {
                return Application.Current?.TryFindResource("CreamBrush") as Brush ?? CreamBrush;
            }
        }
        return CreamBrush;
    }

    /// <summary>
    /// Not implemented.
    /// </summary>
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
