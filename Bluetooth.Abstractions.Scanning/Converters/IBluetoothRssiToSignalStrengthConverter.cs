namespace Bluetooth.Abstractions.Scanning.Converters;

/// <summary>
/// Converts RSSI (Received Signal Strength Indicator) values in dBm to normalized signal strength percentages.
/// </summary>
/// <remarks>
/// <para>
/// RSSI values typically range from -100 dBm (very weak signal) to -30 dBm (very strong signal).
/// This converter normalizes these values to a 0.0-1.0 range for easier consumption by UI and business logic.
/// </para>
/// <para>
/// <b>Default Implementation:</b> The default LinearRssiToSignalStrengthConverter uses linear mapping with auto-scaling.
/// It automatically expands its range based on observed min/max RSSI values during the session.
/// </para>
/// <para>
/// <b>Available Implementations:</b>
/// </para>
/// <list type="bullet">
/// <item><b>LinearRssiToSignalStrengthConverter</b> (default): Auto-scaling linear mapping that adjusts range based on observed min/max values</item>
/// <item><b>FixedRangeLinearRssiToSignalStrengthConverter</b>: Linear mapping with fixed min/max RSSI boundaries for consistent behavior</item>
/// <item><b>PiecewiseLinearRssiToSignalStrengthConverter</b>: Non-linear mapping using different slopes for different RSSI ranges, better matching real-world signal behavior</item>
/// </list>
/// <para>
/// <b>Custom Curves:</b> To use a different converter or implement custom curves (exponential, logarithmic, polynomial, etc.),
/// register your implementation via dependency injection:
/// </para>
/// <code>
/// // Example: Use the piecewise linear converter (recommended for realistic signal quality)
/// services.AddSingleton&lt;IBluetoothRssiToSignalStrengthConverter, PiecewiseLinearRssiToSignalStrengthConverter&gt;();
///
/// // Example: Use fixed-range linear converter
/// services.AddSingleton&lt;IBluetoothRssiToSignalStrengthConverter, FixedRangeLinearRssiToSignalStrengthConverter&gt;();
///
/// // Example: Register a custom exponential converter
/// services.AddSingleton&lt;IBluetoothRssiToSignalStrengthConverter, ExponentialRssiConverter&gt;();
/// </code>
/// <para>
/// <b>Note:</b> This converter operates on smoothed RSSI values (after jitter smoothing configured via
/// <see cref="Options.SignalStrengthSmoothingOptions"/>). The smoothing happens before conversion.
/// </para>
/// </remarks>
public interface IBluetoothRssiToSignalStrengthConverter
{
    /// <summary>
    /// Converts the given RSSI value in dBm to a normalized signal strength percentage.
    /// </summary>
    /// <param name="rssi">The RSSI value in dBm (typically -100 to -30). This is usually a smoothed/averaged value rather than a raw reading.</param>
    /// <returns>A signal strength value between 0.0 (weakest) and 1.0 (strongest).</returns>
    double Convert(double rssi);

}
