namespace Bluetooth.Abstractions.Scanning;

/// <summary>
///     Interface representing a Bluetooth device, providing properties and methods for interacting with it.
/// </summary>
public partial interface IBluetoothRemoteDevice
{
    #region L2CAP Channels

    /// <summary>
    ///     Opens an L2CAP channel to the specified PSM (Protocol Service Multiplexer).
    ///     L2CAP channels provide connection-oriented data transfer with higher throughput than GATT.
    /// </summary>
    /// <param name="psm">The PSM value to connect to.</param>
    /// <param name="timeout">The timeout for this operation.</param>
    /// <param name="cancellationToken">A cancellation token to cancel this operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the opened L2CAP channel.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="psm" /> is invalid.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the device is not connected or L2CAP is not supported.</exception>
    /// <exception cref="TimeoutException">Thrown when the operation times out.</exception>
    /// <exception cref="OperationCanceledException">Thrown when the operation is cancelled.</exception>
    /// <remarks>
    ///     <b>Platform Support:</b>
    ///     <list type="bullet">
    ///         <item><b>iOS/macOS</b>: Supported on iOS 11+ / macOS 10.13+ via CBPeripheral.openL2CAPChannel()</item>
    ///         <item><b>Android</b>: Supported on Android 10+ (API 29+) via BluetoothDevice.createL2capChannel()</item>
    ///         <item><b>Windows</b>: Not supported - throws PlatformNotSupportedException</item>
    ///     </list>
    ///     <para>
    ///         L2CAP channels bypass GATT overhead and provide direct socket-like communication:
    ///         - Higher throughput for bulk data transfer
    ///         - Lower latency for time-sensitive data
    ///         - Custom protocol implementation support
    ///     </para>
    ///     <para>Valid PSM range is typically 0x0001-0x00FF (dynamic) or vendor-specific values.</para>
    /// </remarks>
    ValueTask<IBluetoothL2CapChannel> OpenL2CapChannelAsync(int psm, TimeSpan? timeout = null, CancellationToken cancellationToken = default);

    #endregion
}
