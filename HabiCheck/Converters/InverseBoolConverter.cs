using System;
using System.Globalization;
using System.Windows.Data;

namespace HabiCheck.Converters;

/// <summary>
/// Converts a boolean value to its logical inverse.
/// True returns false, False returns true.
/// </summary>
public class InverseBoolConverter : IValueConverter
{
    // 💡 DEVELOPER NOTE: This converter is useful for disabling a UI element when a property is true (like a loading indicator).
    
    /// <summary>
    /// Inverts a boolean value.
    /// </summary>
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool boolValue)
        {
            return !boolValue;
        }
        return true;
    }

    /// <summary>
    /// Inverts a boolean value back.
    /// </summary>
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool boolValue)
        {
            return !boolValue;
        }
        return true;
    }
}
