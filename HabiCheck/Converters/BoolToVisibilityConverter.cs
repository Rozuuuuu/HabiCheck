using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace HabiCheck.Converters;

/// <summary>
/// Converts a value to a WPF Visibility value.
/// Supports: bool, string (non-empty = Visible), int (> 0 = Visible), and null (= Collapsed).
/// Pass ConverterParameter="invert" to reverse the result.
/// </summary>
public class BoolToVisibilityConverter : IValueConverter
{
    // 💡 DEVELOPER NOTE: Value converters let WPF translate data types to UI states.
    // This one is upgraded to handle multiple input types and an optional "invert" flag.

    /// <summary>
    /// Converts a value to Visibility. Pass ConverterParameter="invert" to flip the result.
    /// </summary>
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        // Determine if the value is truthy based on its type
        bool isTrue = value switch
        {
            bool b   => b,
            string s => !string.IsNullOrEmpty(s),
            int i    => i > 0,
            null     => false,
            _        => value != null
        };

        // 💡 DEVELOPER NOTE: ConverterParameter lets XAML pass extra info to the converter.
        // Passing "invert" flips the logic — so an empty string becomes Visible, etc.
        bool invert = parameter is string p &&
                      p.Equals("invert", StringComparison.OrdinalIgnoreCase);

        if (invert) isTrue = !isTrue;

        return isTrue ? Visibility.Visible : Visibility.Collapsed;
    }

    /// <summary>
    /// Converts Visibility back to bool.
    /// </summary>
    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        bool invert = parameter is string p &&
                      p.Equals("invert", StringComparison.OrdinalIgnoreCase);

        bool isVisible = value is Visibility v && v == Visibility.Visible;
        return invert ? !isVisible : isVisible;
    }
}
