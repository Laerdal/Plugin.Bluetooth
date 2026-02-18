namespace Bluetooth.Abstractions.Scanning;

/// <summary>
/// Interface for managing Bluetooth pairing and bonding operations.
/// Pairing establishes a secure connection between devices, while bonding stores the keys for future connections.
/// </summary>
public interface IBluetoothPairingManager
{
    /// <summary>
    /// Gets the Bluetooth adapter associated with this pairing manager.
    /// </summary>
    IBluetoothAdapter Adapter { get; }

    /// <summary>
    /// Pairs with the specified device asynchronously.
    /// </summary>
    /// <param name="device">The device to pair with.</param>
    /// <param name="timeout">The timeout for this operation.</param>
    /// <param name="cancellationToken">A cancellation token to cancel this operation.</param>
    /// <returns>A task that represents the asynchronous pairing operation. The task result indicates whether pairing was successful.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="device"/> is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the device is already paired or pairing is not supported.</exception>
    /// <exception cref="TimeoutException">Thrown when the operation times out.</exception>
    /// <exception cref="OperationCanceledException">Thrown when the operation is cancelled.</exception>
    ValueTask<bool> PairAsync(IBluetoothRemoteDevice device, TimeSpan? timeout = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Unpairs (removes bonding information) from the specified device asynchronously.
    /// </summary>
    /// <param name="device">The device to unpair.</param>
    /// <param name="timeout">The timeout for this operation.</param>
    /// <param name="cancellationToken">A cancellation token to cancel this operation.</param>
    /// <returns>A task that represents the asynchronous unpairing operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="device"/> is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the device is not paired or unpairing is not supported.</exception>
    /// <exception cref="TimeoutException">Thrown when the operation times out.</exception>
    /// <exception cref="OperationCanceledException">Thrown when the operation is cancelled.</exception>
    ValueTask UnpairAsync(IBluetoothRemoteDevice device, TimeSpan? timeout = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if the specified device is currently paired (bonded).
    /// </summary>
    /// <param name="device">The device to check.</param>
    /// <returns>True if the device is paired, false otherwise.</returns>
    bool IsPaired(IBluetoothRemoteDevice device);

    /// <summary>
    /// Checks if the specified device is currently paired (bonded) by its identifier.
    /// </summary>
    /// <param name="deviceId">The identifier of the device to check.</param>
    /// <returns>True if the device is paired, false otherwise.</returns>
    bool IsPaired(string deviceId);

    /// <summary>
    /// Event raised when the pairing state changes for a device.
    /// </summary>
    event EventHandler<PairingStateChangedEventArgs>? PairingStateChanged;

    #region Paired Devices - Exploration

    /// <summary>
    /// Event triggered when the list of paired devices changes.
    /// </summary>
    event EventHandler<DeviceListChangedEventArgs>? PairedDeviceListChanged;

    /// <summary>
    /// Event triggered when devices are added to the paired devices list.
    /// </summary>
    event EventHandler<DevicesAddedEventArgs>? PairedDevicesAdded;

    /// <summary>
    /// Event triggered when devices are removed from the paired devices list.
    /// </summary>
    event EventHandler<DevicesRemovedEventArgs>? PairedDevicesRemoved;

    #endregion

    #region Paired Devices - Get

    /// <summary>
    /// Returns the closest paired Bluetooth device currently available.
    /// </summary>
    /// <param name="filter">An optional function to filter devices. Defaults to null for all devices.</param>
    /// <returns>The closest paired <see cref="IBluetoothRemoteDevice" />, or null if none are found.</returns>
    IBluetoothRemoteDevice? GetClosestPairedDeviceOrDefault(Func<IBluetoothRemoteDevice, bool>? filter = null);

    /// <summary>
    /// Returns the first paired Bluetooth device that matches the specified filter.
    /// </summary>
    /// <param name="filter">A function to filter devices. Should return true for matching devices.</param>
    /// <returns>The matching paired <see cref="IBluetoothRemoteDevice" />, or null if none are found.</returns>
    IBluetoothRemoteDevice? GetPairedDeviceOrDefault(Func<IBluetoothRemoteDevice, bool> filter);

    /// <summary>
    /// Returns a paired Bluetooth device with the specified ID.
    /// </summary>
    /// <param name="id">The ID of the device to retrieve.</param>
    /// <returns>The matching paired <see cref="IBluetoothRemoteDevice" />, or null if none are found.</returns>
    IBluetoothRemoteDevice? GetPairedDeviceOrDefault(string id);

    /// <summary>
    /// Returns all paired Bluetooth devices that match the specified filter.
    /// </summary>
    /// <param name="filter">An optional function to filter devices. Defaults to null for all devices.</param>
    /// <returns>A collection of paired <see cref="IBluetoothRemoteDevice" /> that match the filter.</returns>
    IEnumerable<IBluetoothRemoteDevice> GetPairedDevices(Func<IBluetoothRemoteDevice, bool>? filter = null);

    /// <summary>
    /// Returns a paired device immediately if it exists, otherwise waits for it to be paired.
    /// </summary>
    /// <param name="filter">An optional function to filter devices. Defaults to null for all devices.</param>
    /// <param name="timeout">The timeout for this operation.</param>
    /// <param name="cancellationToken">A cancellation token to cancel this operation.</param>
    /// <returns>The matching paired <see cref="IBluetoothRemoteDevice" />.</returns>
    ValueTask<IBluetoothRemoteDevice> GetPairedDeviceOrWaitForDeviceToBePairedAsync(Func<IBluetoothRemoteDevice, bool>? filter = null, TimeSpan? timeout = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns a paired device by ID immediately if it exists, otherwise waits for it to be paired.
    /// </summary>
    /// <param name="id">The ID of the device to retrieve.</param>
    /// <param name="timeout">The timeout for this operation.</param>
    /// <param name="cancellationToken">A cancellation token to cancel this operation.</param>
    /// <returns>The matching paired <see cref="IBluetoothRemoteDevice" />.</returns>
    ValueTask<IBluetoothRemoteDevice> GetPairedDeviceOrWaitForDeviceToBePairedAsync(string id, TimeSpan? timeout = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Waits for a device with the specified ID to be paired.
    /// </summary>
    /// <param name="id">The ID of the device to wait for.</param>
    /// <param name="timeout">The timeout for this operation.</param>
    /// <param name="cancellationToken">A cancellation token to cancel this operation.</param>
    /// <returns>The paired <see cref="IBluetoothRemoteDevice" />.</returns>
    ValueTask<IBluetoothRemoteDevice> WaitForDeviceToBePairedAsync(string id, TimeSpan? timeout = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Waits for a device matching the filter to be paired.
    /// </summary>
    /// <param name="filter">An optional function to filter devices. Defaults to null for all devices.</param>
    /// <param name="timeout">The timeout for this operation.</param>
    /// <param name="cancellationToken">A cancellation token to cancel this operation.</param>
    /// <returns>The paired <see cref="IBluetoothRemoteDevice" />.</returns>
    ValueTask<IBluetoothRemoteDevice> WaitForDeviceToBePairedAsync(Func<IBluetoothRemoteDevice, bool>? filter = null, TimeSpan? timeout = null, CancellationToken cancellationToken = default);

    #endregion
}
