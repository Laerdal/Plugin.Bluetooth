namespace Bluetooth.Abstractions.Scanning;

/// <summary>
///     Interface representing a Bluetooth device, providing properties and methods for interacting with it.
/// </summary>
public partial interface IBluetoothRemoteDevice
{
    /// <summary>
    ///     Gets the signal strength in dBm.
    /// </summary>
    int SignalStrengthDbm { get; }

    /// <summary>
    ///     Gets the signal strength as a percentage (between 0.00 and 1.00).
    /// </summary>
    double SignalStrengthPercent { get; }

    /// <summary>
    ///     Gets or sets a value indicating whether this device may be probed for its signal strength. Defaults to <c>true</c>.
    ///     When set to <c>false</c>, <see cref="ReadSignalStrengthAsync" /> stops issuing native RSSI reads and returns the
    ///     last known <see cref="SignalStrengthDbm" /> instead.
    /// </summary>
    /// <remarks>
    ///     Set this to <c>false</c> before starting a firmware update. Nordic's DFU bootloader tends to drop the connection
    ///     when it is spammed with RSSI reads, which is particularly dangerous while an update is in flight. A read that is
    ///     already in flight when this is set to <c>false</c> still completes normally.
    ///     <para>
    ///         Signal strengths carried by advertisements are unaffected — those are passive and cost the device nothing.
    ///     </para>
    /// </remarks>
    bool IsSignalStrengthProbingEnabled { get => true; set { } }

    /// <summary>
    ///     Reads the signal strength asynchronously.
    ///     This is an operation running on a ticker when the device is connected.
    ///     We can't get that value from advertisement anymore.
    /// </summary>
    /// <param name="timeout">The timeout for this operation</param>
    /// <param name="cancellationToken">A cancellation token to cancel this operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the device is not connected.</exception>
    /// <exception cref="TimeoutException">Thrown when the operation times out.</exception>
    /// <exception cref="OperationCanceledException">Thrown when the operation is cancelled.</exception>
    /// <remarks>
    ///     <b>Platform Support:</b>
    ///     <list type="bullet">
    ///         <item><b>Android</b>: Real-time RSSI reading via BluetoothGatt.readRemoteRssi() with callback</item>
    ///         <item><b>iOS/macOS</b>: Real-time RSSI reading via CBPeripheral.readRSSI() with delegate callback</item>
    ///         <item><b>Windows</b>: Limited support - RSSI may be cached or require periodic polling</item>
    ///     </list>
    ///     <para>
    ///         Signal strength is measured in dBm (decibel-milliwatts):
    ///         - Typical range: -100 dBm (very weak, ~100m) to -30 dBm (very strong, close proximity)
    ///         - Values update based on radio conditions and may fluctuate
    ///         - Accuracy varies by platform and hardware capabilities
    ///     </para>
    ///     <para>Call frequency should be limited to avoid impacting performance - recommended interval: 1-5 seconds.</para>
    ///     <para>
    ///         Returns the last known <see cref="SignalStrengthDbm" /> without touching the radio when
    ///         <see cref="IsSignalStrengthProbingEnabled" /> is <c>false</c>.
    ///     </para>
    /// </remarks>
    ValueTask<int> ReadSignalStrengthAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default);
}
