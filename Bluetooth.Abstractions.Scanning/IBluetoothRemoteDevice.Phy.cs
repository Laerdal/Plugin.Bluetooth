namespace Bluetooth.Abstractions.Scanning;

/// <summary>
///     Interface representing a Bluetooth device, providing properties and methods for interacting with it.
/// </summary>
public partial interface IBluetoothRemoteDevice
{
    #region PHY (Physical Layer)

    /// <summary>
    ///     Gets the current transmit PHY mode.
    /// </summary>
    PhyMode CurrentTxPhy { get; }

    /// <summary>
    ///     Gets the current receive PHY mode.
    /// </summary>
    PhyMode CurrentRxPhy { get; }

    /// <summary>
    ///     Requests a preferred PHY mode for the connection (Bluetooth 5.0+ feature).
    /// </summary>
    /// <param name="txPhy">The desired transmit PHY mode.</param>
    /// <param name="rxPhy">The desired receive PHY mode.</param>
    /// <param name="timeout">The timeout for this operation.</param>
    /// <param name="cancellationToken">A cancellation token to cancel this operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the device is not connected or PHY mode is not supported.</exception>
    /// <exception cref="TimeoutException">Thrown when the operation times out.</exception>
    /// <exception cref="OperationCanceledException">Thrown when the operation is cancelled.</exception>
    /// <remarks>
    ///     <b>Platform Support:</b>
    ///     <list type="bullet">
    ///         <item><b>Android</b>: Full support on Android 8.0 (API 26+) with Bluetooth 5.0 hardware via BluetoothGatt.setPreferredPhy()</item>
    ///         <item><b>iOS/macOS</b>: PHY selection is system-managed and automatic. This method is a no-op.</item>
    ///         <item><b>Windows</b>: Limited support - PHY is typically auto-negotiated by the system.</item>
    ///     </list>
    ///     <para>
    ///         PHY modes affect range, speed, and power consumption:
    ///         - <b>1M PHY</b>: Standard mode with balanced range and throughput (1 Mbps)
    ///         - <b>2M PHY</b>: High-speed mode with reduced range (2 Mbps)
    ///         - <b>Coded PHY</b>: Long-range mode with reduced throughput (~125-500 Kbps) but 4x range
    ///     </para>
    ///     <para>The actual PHY used depends on negotiation with the remote device and hardware capabilities.</para>
    /// </remarks>
    ValueTask SetPreferredPhyAsync(PhyMode txPhy, PhyMode rxPhy, TimeSpan? timeout = null, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Event raised when the PHY changes.
    /// </summary>
    event EventHandler<PhyChangedEventArgs>? PhyChanged;

    #endregion
}
