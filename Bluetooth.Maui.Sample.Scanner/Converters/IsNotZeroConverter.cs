using Microsoft.Maui.Controls;

namespace Bluetooth.Maui.Sample.Scanner.Converters;

/// <summary>
/// Converts an integer value to a boolean indicating whether it's not zero.
/// </summary>
public class IsNotZeroConverter : IValueConverter
{
    /// <summary>
    /// Converts an integer to a boolean (true if not zero).
    /// </summary>
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is int intValue)
        {
            return intValue != 0;
        }

        if (value is long longValue)
        {
            return longValue != 0;
        }

        if (value is double doubleValue)
        {
            return Math.Abs(doubleValue) > 0.0001;
        }

        return false;
    }

    /// <summary>
    /// Not implemented for one-way conversion.
    /// </summary>
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
