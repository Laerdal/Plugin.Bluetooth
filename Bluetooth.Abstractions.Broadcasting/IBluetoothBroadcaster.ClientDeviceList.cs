namespace Bluetooth.Abstractions.Broadcasting;

public partial interface IBluetoothBroadcaster
{
    // ## LIST OF CONNECTED CLIENT DEVICES ##

    #region ClientDevices - Events

    /// <summary>
    /// Event triggered when the list of connected client devices changes.
    /// </summary>
    event EventHandler<ClientDeviceListChangedEventArgs>? ClientDeviceListChanged;

    /// <summary>
    /// Event triggered when client devices connect.
    /// </summary>
    event EventHandler<ClientDevicesAddedEventArgs>? ClientDevicesAdded;

    /// <summary>
    /// Event triggered when client devices disconnect.
    /// </summary>
    event EventHandler<ClientDevicesRemovedEventArgs>? ClientDevicesRemoved;

    #endregion

    #region ClientDevices - Get

    /// <summary>
    /// Returns the first Bluetooth device that matches the specified filter.
    /// </summary>
    /// <param name="filter">A function to filter devices. Should return true for matching devices.</param>
    /// <returns>The matching <see cref="IBluetoothConnectedDevice" />.</returns>
    /// <exception cref="ClientDeviceNotFoundException">Thrown when no device matches the specified filter.</exception>
    /// <exception cref="MultipleClientDevicesFoundException">Thrown when multiple devices match the specified filter.</exception>
    IBluetoothConnectedDevice GetClientDevice(Func<IBluetoothConnectedDevice, bool> filter);

    /// <summary>
    /// Returns a Bluetooth device with the specified ID.
    /// </summary>
    /// <param name="id">The ID of the device to retrieve.</param>
    /// <returns>The matching <see cref="IBluetoothConnectedDevice" />.</returns>
    /// <exception cref="ClientDeviceNotFoundException">Thrown when no device with the specified ID is found.</exception>
    /// <exception cref="MultipleClientDevicesFoundException">Thrown when multiple devices with the specified ID are found.</exception>
    IBluetoothConnectedDevice GetClientDevice(string id);

    /// <summary>
    /// Returns the first Bluetooth device that matches the specified filter.
    /// </summary>
    /// <param name="filter">A function to filter devices. Should return true for matching devices.</param>
    /// <returns>The matching <see cref="IBluetoothConnectedDevice" />, or null if none are found.</returns>
    /// <exception cref="MultipleClientDevicesFoundException">Thrown when multiple devices match the specified filter.</exception>
    IBluetoothConnectedDevice? GetClientDeviceOrDefault(Func<IBluetoothConnectedDevice, bool> filter);

    /// <summary>
    /// Returns a Bluetooth device with the specified ID.
    /// </summary>
    /// <param name="id">The ID of the device to retrieve.</param>
    /// <returns>The matching <see cref="IBluetoothConnectedDevice" />, or null if none are found.</returns>
    /// <exception cref="MultipleClientDevicesFoundException">Thrown when multiple devices with the specified ID are found.</exception>
    IBluetoothConnectedDevice? GetClientDeviceOrDefault(string id);

    /// <summary>
    /// Returns all Bluetooth devices that match the specified filter.
    /// </summary>
    /// <param name="filter">An optional function to filter devices. Defaults to null for all devices.</param>
    /// <returns>A collection of <see cref="IBluetoothConnectedDevice" /> that match the filter.</returns>
    IEnumerable<IBluetoothConnectedDevice> GetClientDevices(Func<IBluetoothConnectedDevice, bool>? filter = null);

    #endregion

    #region ClientDevices - Has

    /// <summary>
    /// Determines if there is at least one connected client device that matches the specified filter.
    /// </summary>
    /// <param name="filter">A function to filter devices. Should return true for matching devices.</param>
    /// <returns>True if at least one matching device is found; otherwise, false.</returns>
    bool HasClientDevice(Func<IBluetoothConnectedDevice, bool> filter);

    /// <summary>
    /// Determines if there is a connected client device with the specified ID.
    /// </summary>
    /// <param name="id">The ID of the device to check for.</param>
    /// <returns>True if a device with the specified ID is found; otherwise, false.</returns>
    bool HasClientDevice(string id);

    #endregion

}
