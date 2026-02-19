

namespace Bluetooth.Core.Scanning.Converters;

/// <summary>
/// A fixed-range linear implementation of <see cref="IBluetoothRssiToSignalStrengthConverter"/> that maps RSSI values to signal strength percentages using a predefined range.
/// </summary>
/// <remarks>
/// Unlike <see cref="LinearRssiToSignalStrengthConverter"/> which auto-scales based on observed values,
/// this converter uses fixed minimum and maximum RSSI boundaries for consistent, predictable behavior.
/// <para>
/// <b>Use cases:</b>
/// </para>
/// <list type="bullet">
/// <item>When you want consistent signal strength mapping across app restarts</item>
/// <item>When you know the expected RSSI range for your devices</item>
/// <item>When auto-scaling behavior is undesirable (e.g., for calibrated applications)</item>
/// </list>
/// </remarks>
public class FixedRangeLinearRssiToSignalStrengthConverter : IBluetoothRssiToSignalStrengthConverter
{
    private readonly double _minRssi;
    private readonly double _maxRssi;

    /// <summary>
    /// Initializes a new instance of the <see cref="FixedRangeLinearRssiToSignalStrengthConverter"/> class with the specified RSSI range.
    /// </summary>
    /// <param name="minRssi">The minimum RSSI value in dBm (representing 0% signal strength). Default is -100.</param>
    /// <param name="maxRssi">The maximum RSSI value in dBm (representing 100% signal strength). Default is -50.</param>
    /// <remarks>
    /// Typical Bluetooth LE RSSI ranges:
    /// <list type="bullet">
    /// <item><b>-100 dBm</b>: Very weak signal, at the edge of reliable communication range</item>
    /// <item><b>-80 dBm</b>: Weak signal, typical at 10-20 meters</item>
    /// <item><b>-60 dBm</b>: Good signal, typical at 5-10 meters</item>
    /// <item><b>-50 dBm</b>: Strong signal, typical at 1-5 meters</item>
    /// <item><b>-30 dBm</b>: Very strong signal, typically very close proximity</item>
    /// </list>
    /// Choose min/max values based on your application's expected operating distance and environment.
    /// </remarks>
    public FixedRangeLinearRssiToSignalStrengthConverter(double minRssi = -100, double maxRssi = -50)
    {
        if (minRssi >= maxRssi)
        {
            throw new ArgumentException($"minRssi ({minRssi}) must be less than maxRssi ({maxRssi})", nameof(minRssi));
        }

        _minRssi = minRssi;
        _maxRssi = maxRssi;
    }

    /// <inheritdoc/>
    public double Convert(double rssi)
    {
        // Clamp to 0.0 if below minimum
        if (rssi <= _minRssi)
        {
            return 0;
        }

        // Clamp to 1.0 if above maximum
        if (rssi >= _maxRssi)
        {
            return 1;
        }

        // Linear interpolation between min and max
        return (rssi - _minRssi) / (_maxRssi - _minRssi);
    }
}
