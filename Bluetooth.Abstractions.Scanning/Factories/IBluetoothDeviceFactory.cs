namespace Bluetooth.Abstractions.Scanning.Factories;

/// <summary>
///     Interface representing a factory for creating Bluetooth devices.
/// </summary>
public interface IBluetoothDeviceFactory
{
    /// <summary>
    ///     Creates a Bluetooth device based on the provided factory request.
    /// </summary>
    /// <param name="scanner">The Bluetooth scanner that found the device.</param>
    /// <param name="request">The factory request containing necessary information to create the device.</param>
    /// <returns>The created Bluetooth device.</returns>
    IBluetoothRemoteDevice CreateDevice(IBluetoothScanner scanner, BluetoothDeviceFactoryRequest request);

    /// <summary>
    ///     Record representing a request to create a Bluetooth device with ID and Manufacturer.
    /// </summary>
    record BluetoothDeviceFactoryRequest
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="BluetoothDeviceFactoryRequest" /> class using the provided Bluetooth advertisement.
        ///     This constructor extracts the device ID and manufacturer information from the advertisement, allowing for streamlined device creation based on the advertisement data received during scanning.
        /// </summary>
        /// <param name="advertisement">The Bluetooth advertisement containing information about the device.</param>
        protected BluetoothDeviceFactoryRequest(IBluetoothAdvertisement advertisement)
        {
            ArgumentNullException.ThrowIfNull(advertisement);
            Advertisement = advertisement;
            DeviceId = advertisement.BluetoothAddress;
            Manufacturer = advertisement.Manufacturer;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="BluetoothDeviceFactoryRequest" /> class with the specified device ID and manufacturer information.
        ///     This constructor allows for manual creation of a factory request when the device ID and manufacturer information are known, without relying on a Bluetooth advertisement.
        ///     This can be useful in scenarios where the device information is obtained from a different source or when creating devices programmatically without scanning.
        /// </summary>
        /// <param name="deviceId">The unique identifier of the Bluetooth device.</param>
        /// <param name="manufacturer">The manufacturer information of the Bluetooth device, which can be used for filtering and categorization during scanning.</param>
        protected BluetoothDeviceFactoryRequest(string deviceId, Manufacturer manufacturer)
        {
            DeviceId = deviceId;
            Manufacturer = manufacturer;
        }

        /// <summary>
        ///     Gets the unique identifier of the Bluetooth device.
        /// </summary>
        public string DeviceId { get; }

        /// <summary>
        ///     Gets the manufacturer information of the Bluetooth device, which can be used for filtering and categorization during scanning.
        /// </summary>
        public Manufacturer Manufacturer { get; }

        /// <summary>
        ///     Gets the Bluetooth advertisement associated with the device, which contains information about the device's presence and capabilities.
        ///     This can be used for initializing the device with relevant data from the advertisement, such as signal strength, service UUIDs, and manufacturer-specific data.
        /// </summary>
        public IBluetoothAdvertisement? Advertisement { get; }
    }
}
