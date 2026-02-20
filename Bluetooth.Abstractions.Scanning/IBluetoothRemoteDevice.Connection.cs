namespace Bluetooth.Abstractions.Scanning;

/// <summary>
///     Interface representing a Bluetooth device, providing properties and methods for interacting with it.
/// </summary>
public partial interface IBluetoothRemoteDevice
{
    #region Connection Priority

    /// <summary>
    ///     Requests a change in connection priority/performance mode for an active connection.
    /// </summary>
    /// <param name="priority">The desired connection priority mode.</param>
    /// <param name="timeout">The timeout for this operation.</param>
    /// <param name="cancellationToken">A cancellation token to cancel this operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the device is not connected.</exception>
    /// <exception cref="TimeoutException">Thrown when the operation times out.</exception>
    /// <exception cref="OperationCanceledException">Thrown when the operation is cancelled.</exception>
    /// <remarks>
    ///     <b>Platform Support:</b>
    ///     <list type="bullet">
    ///         <item><b>Android</b>: Full support via BluetoothGatt.requestConnectionPriority()</item>
    ///         <item><b>iOS/macOS</b>: Connection parameters are system-managed. This method is a no-op.</item>
    ///         <item><b>Windows</b>: Connection parameters are system-managed. This method is a no-op.</item>
    ///     </list>
    ///     <para>
    ///         On Android, this allows fine-tuning the connection parameters for different use cases:
    ///         - <b>High priority</b>: Fast data transfer, low latency (11.25-15ms interval)
    ///         - <b>Balanced</b>: Reasonable performance with moderate power (30-50ms interval)
    ///         - <b>Low power</b>: Battery optimization, higher latency (100-125ms interval)
    ///     </para>
    ///     <para>Changes typically take effect within a few connection intervals after the request.</para>
    /// </remarks>
    ValueTask RequestConnectionPriorityAsync(BluetoothConnectionPriority priority, TimeSpan? timeout = null, CancellationToken cancellationToken = default);

    #endregion

    #region ConnectionState

    /// <summary>
    ///     Occurs when the device is connected.
    /// </summary>
    event EventHandler Connected;

    /// <summary>
    ///     Occurs when the device is disconnected.
    /// </summary>
    event EventHandler Disconnected;

    /// <summary>
    ///     Occurs when the connection state of the device changes.
    /// </summary>
    event EventHandler<DeviceConnectionStateChangedEventArgs> ConnectionStateChanged;

    /// <summary>
    ///     Gets a value indicating whether the device is connected.
    /// </summary>
    bool IsConnected { get; }

    /// <summary>
    ///     Waits for the device to be connected or disconnected asynchronously.
    /// </summary>
    /// <param name="isConnected">True to wait for the device to be connected, false to wait for it to be disconnected.</param>
    /// <param name="timeout">The timeout for this operation</param>
    /// <param name="cancellationToken">A cancellation token to cancel this operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask WaitForIsConnectedAsync(bool isConnected, TimeSpan? timeout = null, CancellationToken cancellationToken = default);

    #endregion

    #region Connecting

    /// <summary>
    ///     Gets a value indicating whether the device is connecting.
    /// </summary>
    bool IsConnecting { get; }

    /// <summary>
    ///     Occurs when the device is connecting.
    /// </summary>
    event EventHandler Connecting;

    /// <summary>
    ///     Connects to the device if it is not already connected asynchronously.
    /// </summary>
    /// <param name="connectionOptions">The connection options to use.</param>
    /// <param name="timeout">The timeout for this operation</param>
    /// <param name="cancellationToken">A cancellation token to cancel this operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <exception cref="TimeoutException">Thrown if the connection attempt times out.</exception>
    ValueTask ConnectIfNeededAsync(ConnectionOptions connectionOptions, TimeSpan? timeout = null, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Connects to the device asynchronously.
    /// </summary>
    /// <param name="connectionOptions">The connection options to use.</param>
    /// <param name="timeout">The timeout for this operation</param>
    /// <param name="cancellationToken">A cancellation token to cancel this operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="connectionOptions" /> is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the device is already connected.</exception>
    /// <exception cref="TimeoutException">Thrown if the connection attempt times out.</exception>
    /// <exception cref="OperationCanceledException">Thrown when the operation is cancelled.</exception>
    ValueTask ConnectAsync(ConnectionOptions connectionOptions, TimeSpan? timeout = null, CancellationToken cancellationToken = default);

    #endregion

    #region Disconnecting

    /// <summary>
    ///     Gets a value indicating whether the device is disconnecting.
    /// </summary>
    bool IsDisconnecting { get; }

    /// <summary>
    ///     Occurs when the device is disconnecting.
    /// </summary>
    event EventHandler Disconnecting;

    /// <summary>
    ///     Disconnects from the device if it is not already disconnected asynchronously.
    /// </summary>
    /// <param name="timeout">The timeout for this operation</param>
    /// <param name="cancellationToken">A cancellation token to cancel this operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <exception cref="TimeoutException">Thrown if the disconnection attempt times out.</exception>
    ValueTask DisconnectIfNeededAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Disconnects from the device asynchronously.
    /// </summary>
    /// <param name="timeout">The timeout for this operation</param>
    /// <param name="cancellationToken">A cancellation token to cancel this operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the device is not connected.</exception>
    /// <exception cref="TimeoutException">Thrown if the disconnection attempt times out.</exception>
    /// <exception cref="OperationCanceledException">Thrown when the operation is cancelled.</exception>
    ValueTask DisconnectAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default);

    #region UnexpectedDisconnection

    /// <summary>
    ///     Occurs when an unexpected disconnection happens.
    /// </summary>
    event EventHandler<DeviceUnexpectedDisconnectionEventArgs> UnexpectedDisconnection;

    /// <summary>
    ///     Gets or sets a value indicating whether to ignore the next unexpected disconnection.
    /// </summary>
    bool IgnoreNextUnexpectedDisconnection { get; set; }

    #endregion

    #endregion
}