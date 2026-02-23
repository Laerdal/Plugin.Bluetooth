namespace Bluetooth.Core.Scanning.Converters;

/// <summary>
///     A piecewise linear implementation of <see cref="IBluetoothRssiToSignalStrengthConverter" /> that uses different conversion rates for different RSSI ranges.
/// </summary>
/// <remarks>
///     <para>
///         This converter implements a non-linear mapping that more accurately reflects real-world Bluetooth signal behavior.
///         Signal quality degrades more rapidly at lower RSSI values, which this converter models through different slopes in different ranges.
///     </para>
///     <para>
///         <b>Conversion Ranges:</b>
///     </para>
///     <list type="table">
///         <listheader>
///             <term>RSSI Range (dBm)</term>
///             <description>Signal Strength</description>
///             <description>Drop Rate</description>
///         </listheader>
///         <item>
///             <term>≥ -50</term>
///             <description>100% (Excellent)</description>
///             <description>Constant 1.00</description>
///         </item>
///         <item>
///             <term>(-50, -70]</term>
///             <description>85% - 100%</description>
///             <description>0.75% per -1 dBm</description>
///         </item>
///         <item>
///             <term>(-70, -85]</term>
///             <description>65% - 85%</description>
///             <description>1.33% per -1 dBm</description>
///         </item>
///         <item>
///             <term>(-85, -100]</term>
///             <description>25% - 65%</description>
///             <description>2.67% per -1 dBm</description>
///         </item>
///         <item>
///             <term>(-100, -110]</term>
///             <description>0% - 25%</description>
///             <description>2.50% per -1 dBm</description>
///         </item>
///         <item>
///             <term>&lt; -110</term>
///             <description>0% (No signal)</description>
///             <description>Constant 0.00</description>
///         </item>
///     </list>
///     <para>
///         <b>Why Piecewise Linear?</b> This approach models the observation that signal quality doesn't degrade uniformly across all RSSI ranges.
///         Strong signals (-50 to -70 dBm) remain highly reliable with gradual degradation, while weak signals (-85 to -110 dBm)
///         deteriorate much more rapidly in perceived quality and connection stability.
///     </para>
///     <para>
///         <b>Visual Reference:</b> See the <see href="https://github.com/user-attachments/assets/8461e5cd-1db7-4316-8927-5ce2a786d972">plot visualization</see>
///         for a graphical representation of this conversion curve.
///     </para>
///     <para>
///         <b>Use Cases:</b>
///     </para>
///     <list type="bullet">
///         <item>Applications requiring realistic signal strength representation in UI</item>
///         <item>Distance estimation or proximity detection scenarios</item>
///         <item>When empirical testing shows non-linear RSSI-to-quality relationships</item>
///     </list>
/// </remarks>
public class PiecewiseLinearRssiToSignalStrengthConverter : IBluetoothRssiToSignalStrengthConverter
{
    /// <inheritdoc />
    public double Convert(double rssi)
    {
        return rssi switch
        {
            // 1. Excellent Signal: Constant 1.00
            >= -50 => 1.00,

            // 2. (-50, -70]: Drop from 100% to 85% (0.15 drop over 20 dBm = 0.75% per -1dBm)
            // Formula: 1.00 + (rssi + 50) * 0.0075
            > -70 => Math.Round(1.00 + (rssi + 50) * 0.0075, 3),

            // 3. (-70, -85]: Drop from 85% to 65% (0.20 drop over 15 dBm ≈ 1.33% per -1dBm)
            // Formula: 0.85 + (rssi + 70) * (0.20 / 15.0)
            >= -85 => Math.Round(0.85 + (rssi + 70) * 0.01333, 3),

            // 4. (-85, -100]: Drop from 65% to 25% (0.40 drop over 15 dBm ≈ 2.67% per -1dBm)
            // Formula: 0.65 + (rssi + 85) * (0.40 / 15.0)
            >= -100 => Math.Round(0.65 + (rssi + 85) * 0.02667, 3),

            // 5. (-100, -110]: Drop from 25% to 0% (0.25 drop over 10 dBm = 2.5% per -1dBm)
            // Formula: 0.25 + (rssi + 100) * 0.025
            > -110 => Math.Round(0.25 + (rssi + 100) * 0.025, 3),

            // 6. Garbage Signal: Constant 0.00
            _ => 0.00
        };
    }
}
