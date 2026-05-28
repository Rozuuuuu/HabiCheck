using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace HabiCheck.Converters;

/// <summary>
/// Converts the message role ("user" or "assistant") to HorizontalAlignment.
/// User messages align Right; Assistant messages align Left.
/// </summary>
public class ChatAlignmentConverter : IValueConverter
{
    // 💡 DEVELOPER NOTE: HorizontalAlignment is a WPF layout enum (Left, Center, Right, Stretch).
    
    /// <summary>
    /// Returns HorizontalAlignment based on the role.
    /// </summary>
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is string role)
        {
            if (role.Equals("user", StringComparison.OrdinalIgnoreCase))
            {
                return HorizontalAlignment.Right;
            }
            return HorizontalAlignment.Left;
        }
        return HorizontalAlignment.Left;
    }

    /// <summary>
    /// Not implemented.
    /// </summary>
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
