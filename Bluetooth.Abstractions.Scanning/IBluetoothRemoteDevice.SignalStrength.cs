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
    /// </remarks>
    ValueTask<int> ReadSignalStrengthAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default);
}
