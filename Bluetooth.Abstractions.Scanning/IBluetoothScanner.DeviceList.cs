namespace Bluetooth.Abstractions.Scanning;

public partial interface IBluetoothScanner
{
    // ## LIST OF DEVICES ##

    #region Devices - Events

    /// <summary>
    /// Event triggered when the list of available devices changes.
    /// </summary>
    event EventHandler<DeviceListChangedEventArgs>? DeviceListChanged;

    /// <summary>
    /// Event triggered when devices are added.
    /// </summary>
    event EventHandler<DevicesAddedEventArgs>? DevicesAdded;

    /// <summary>
    /// Event triggered when devices are removed.
    /// </summary>
    event EventHandler<DevicesRemovedEventArgs>? DevicesRemoved;

    #endregion

    #region Devices - Clear

    /// <summary>
    /// Clears resources associated with Bluetooth devices.
    /// If no devices are specified, it clears all devices.
    /// </summary>
    /// <param name="devices">The devices to clear, or null to clear all devices.</param>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    ValueTask ClearDevicesAsync(IEnumerable<IBluetoothRemoteDevice>? devices = null);

    /// <summary>
    /// Clears resources associated with a specific Bluetooth device.
    /// </summary>
    /// <param name="device">The device to clear.</param>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    ValueTask ClearDeviceAsync(IBluetoothRemoteDevice? device);

    /// <summary>
    /// Clears resources associated with a Bluetooth device by its ID.
    /// </summary>
    /// <param name="deviceId">The ID of the device to clear.</param>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    ValueTask ClearDeviceAsync(string deviceId);

    #endregion

    #region Devices - Get

    /// <summary>
    /// Gets the device that matches the specified filter.
    /// </summary>
    /// <param name="filter">The filter to apply to the devices.</param>
    /// <returns>The device that matches the filter.</returns>
    /// <exception cref="DeviceNotFoundException">Thrown if no device matches the specified filter.</exception>
    /// <exception cref="MultipleDevicesFoundException">Thrown if multiple devices match the specified filter.</exception>
    IBluetoothRemoteDevice GetDevice(Func<IBluetoothRemoteDevice, bool> filter);

    /// <summary>
    /// Gets the device with the specified ID.
    /// </summary>
    /// <param name="id">The ID of the device to get.</param>
    /// <returns>The device with the specified ID.</returns>
    /// <exception cref="DeviceNotFoundException">Thrown if no device matches the specified ID.</exception>
    /// <exception cref="MultipleDevicesFoundException">Thrown if multiple devices match the specified ID.</exception>
    IBluetoothRemoteDevice GetDevice(string id);

    /// <summary>
    /// Gets the device that matches the specified filter.
    /// </summary>
    /// <param name="filter">The filter to apply to the devices.</param>
    /// <returns>The device that matches the filter, or null if no such device exists.</returns>
    /// <exception cref="MultipleDevicesFoundException">Thrown if multiple devices match the specified filter.</exception>
    IBluetoothRemoteDevice? GetDeviceOrDefault(Func<IBluetoothRemoteDevice, bool> filter);

    /// <summary>
    /// Gets the device with the specified ID.
    /// </summary>
    /// <param name="id">The ID of the device to get.</param>
    /// <returns>The device with the specified ID, or null if no such device exists.</returns>
    /// <exception cref="MultipleDevicesFoundException">Thrown if multiple devices match the specified ID.</exception>
    IBluetoothRemoteDevice? GetDeviceOrDefault(string id);

    /// <summary>
    /// Returns all Bluetooth devices that match the specified filter.
    /// </summary>
    /// <param name="filter">An optional function to filter devices. Defaults to null for all devices.</param>
    /// <returns>
    /// A read-only snapshot of devices at the time of the call. This collection is immutable
    /// and will not be modified if devices are added or removed after the call returns.
    /// To get updated results, call this method again or subscribe to <see cref="DeviceListChanged"/> event.
    /// </returns>
    IReadOnlyList<IBluetoothRemoteDevice> GetDevices(Func<IBluetoothRemoteDevice, bool>? filter = null);

    /// <summary>
    /// Waits for a Bluetooth device with the specified ID to appear or returns it if already available.
    /// </summary>
    /// <param name="id">The ID of the device to wait for.</param>
    /// <param name="timeout">The timeout for this operation</param>
    /// <param name="cancellationToken">A cancellation token to cancel this operation.</param>
    /// <returns>The <see cref="IBluetoothRemoteDevice" /> when it appears.</returns>
    ValueTask<IBluetoothRemoteDevice> WaitForDeviceToAppearAsync(string id, TimeSpan? timeout = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Waits for the first Bluetooth device that matches the specified filter to appear.
    /// </summary>
    /// <param name="filter">A function to filter devices. Should return true for matching devices.</param>
    /// <param name="timeout">The timeout for this operation</param>
    /// <param name="cancellationToken">A cancellation token to cancel this operation.</param>
    /// <returns>The <see cref="IBluetoothRemoteDevice" /> that matches the filter when it appears.</returns>
    ValueTask<IBluetoothRemoteDevice> WaitForDeviceToAppearAsync(Func<IBluetoothRemoteDevice, bool>? filter = null, TimeSpan? timeout = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns the closest Bluetooth device currently available.
    /// </summary>
    /// <param name="filter">An optional function to filter devices. Defaults to null for all devices.</param>
    /// <returns>The closest <see cref="IBluetoothRemoteDevice" />, or null if none are found.</returns>
    IBluetoothRemoteDevice? GetClosestDeviceOrDefault(Func<IBluetoothRemoteDevice, bool>? filter = null);

    #endregion

    #region Devices - Has

    /// <summary>
    /// Checks if a device that matches the specified filter exists.
    /// </summary>
    /// <param name="filter">The filter to apply to the devices.</param>
    /// <returns>True if a device that matches the filter exists, false otherwise.</returns>
    bool HasDevice(Func<IBluetoothRemoteDevice, bool> filter);

    /// <summary>
    /// Checks if a device with the specified ID exists.
    /// </summary>
    /// <param name="id">The ID of the device to check for.</param>
    /// <returns>True if a device with the specified ID exists, false otherwise.</returns>
    bool HasDevice(string id);

    #endregion

}
