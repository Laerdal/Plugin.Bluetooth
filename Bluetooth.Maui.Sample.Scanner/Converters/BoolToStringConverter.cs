namespace Bluetooth.Maui.Sample.Scanner.Converters;

/// <summary>
///     Converts a boolean value to a string based on a parameter containing two options separated by '|'.
///     The parameter format is "TrueValue|FalseValue".
/// </summary>
public class BoolToStringConverter : IValueConverter
{
    /// <summary>
    ///     Converts a boolean to a string using the parameter format "TrueValue|FalseValue".
    /// </summary>
    /// <param name="value">The boolean value to convert.</param>
    /// <param name="targetType">The target type (not used).</param>
    /// <param name="parameter">The parameter string in format "TrueValue|FalseValue".</param>
    /// <param name="culture">The culture (not used).</param>
    /// <returns>The true or false string value based on the boolean input.</returns>
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not bool boolValue || parameter is not string paramString)
        {
            return string.Empty;
        }

        var parts = paramString.Split('|');
        if (parts.Length != 2)
        {
            return string.Empty;
        }

        return boolValue ? parts[0] : parts[1];
    }

    /// <summary>
    ///     Not implemented for one-way conversion.
    /// </summary>
    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
