namespace Bluetooth.Abstractions.Scanning;

/// <summary>
///     Interface representing a Bluetooth device, providing properties and methods for interacting with it.
/// </summary>
public partial interface IBluetoothRemoteDevice
{
    #region Identity

    /// <summary>
    ///     Gets the unique identifier of the device.
    /// </summary>
    string Id { get; }

    /// <summary>
    ///     Gets the manufacturer of the device.
    /// </summary>
    Manufacturer Manufacturer { get; }

    /// <summary>
    ///     Gets the advertised name of the device.
    /// </summary>
    string AdvertisedName { get; }

    /// <summary>
    ///     Gets the name of the device, using the advertised name first, if empty it will use the cached name.
    /// </summary>
    string Name { get; }

    /// <summary>
    ///     Gets the cached name of the device as seen by the native platform.
    /// </summary>
    string CachedName { get; }

    /// <summary>
    ///     Gets the debug name of the device.
    /// </summary>
    string DebugName { get; }

    /// <summary>
    ///     Gets the last time the device was seen, either by scanning or by connection.
    /// </summary>
    DateTimeOffset LastSeen { get; }

    /// <summary>
    ///     Waits for the name of the device to change asynchronously.
    /// </summary>
    /// <param name="timeout">The timeout for this operation</param>
    /// <param name="cancellationToken">A cancellation token to cancel this operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <exception cref="TimeoutException">Thrown when the operation times out.</exception>
    /// <exception cref="OperationCanceledException">Thrown when the operation is cancelled.</exception>
    ValueTask WaitForNameToChangeAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default);

    #endregion
}
