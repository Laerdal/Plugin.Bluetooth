namespace Bluetooth.Abstractions.Scanning;

/// <summary>
///     Interface representing a Bluetooth device, providing properties and methods for interacting with it.
/// </summary>
public partial interface IBluetoothRemoteDevice
{
    #region MTU (Maximum Transmission Unit)

    /// <summary>
    ///     Gets the current Maximum Transmission Unit (MTU) for this connection.
    ///     The MTU determines the maximum size of a single packet that can be sent.
    /// </summary>
    int Mtu { get; }

    /// <summary>
    ///     Requests a new MTU size for the connection.
    /// </summary>
    /// <param name="requestedMtu">The desired MTU size (typically between 23 and 517 bytes).</param>
    /// <param name="timeout">The timeout for this operation.</param>
    /// <param name="cancellationToken">A cancellation token to cancel this operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the negotiated MTU size.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="requestedMtu" /> is out of valid range.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the device is not connected.</exception>
    /// <exception cref="TimeoutException">Thrown when the operation times out.</exception>
    /// <exception cref="OperationCanceledException">Thrown when the operation is cancelled.</exception>
    /// <remarks>
    ///     <b>Platform Support:</b>
    ///     <list type="bullet">
    ///         <item><b>Android</b>: Full support for explicit MTU negotiation via BluetoothGatt.requestMtu() (range: 23-517 bytes, default: 23)</item>
    ///         <item><b>iOS/macOS</b>: MTU is automatically negotiated by the system. This method is a no-op and returns the current system-negotiated MTU.</item>
    ///         <item><b>Windows</b>: MTU is automatically negotiated via GATT session. This method returns the current negotiated value.</item>
    ///     </list>
    ///     <para>
    ///         MTU (Maximum Transmission Unit) determines the maximum payload size per packet:
    ///         - Larger MTU = fewer packets for large data transfers = better throughput
    ///         - Default MTU is 23 bytes (20 bytes payload + 3 bytes ATT header)
    ///         - Maximum MTU is typically 517 bytes but depends on hardware
    ///     </para>
    ///     <para>The final MTU is negotiated between both devices and may be lower than requested.</para>
    /// </remarks>
    ValueTask<int> RequestMtuAsync(int requestedMtu, TimeSpan? timeout = null, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Event raised when the MTU changes.
    /// </summary>
    event EventHandler<MtuChangedEventArgs>? MtuChanged;

    #endregion
}
