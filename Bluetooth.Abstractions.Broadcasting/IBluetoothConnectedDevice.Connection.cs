namespace Bluetooth.Abstractions.Broadcasting;

/// <summary>
///     Represents a Bluetooth device that has connected to a broadcast (GATT server).
/// </summary>
public partial interface IBluetoothConnectedDevice
{
    #region Connection

    /// <summary>
    ///     Event raised when the client device successfully connects to the broadcaster.
    /// </summary>
    event EventHandler Connected;

    /// <summary>
    ///     Event raised when the client device disconnects from the broadcaster.
    /// </summary>
    event EventHandler Disconnected;

    /// <summary>
    ///     Indicates whether the client device is currently connected.
    /// </summary>
    bool IsConnected { get; }

    /// <summary>
    ///     Disconnects the client device from the broadcaster.
    /// </summary>
    /// <param name="timeout">An optional timeout for the disconnection operation.</param>
    /// <param name="cancellationToken">An optional cancellation token to cancel the disconnection operation.</param>
    ValueTask DisconnectAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default);

    #endregion
}