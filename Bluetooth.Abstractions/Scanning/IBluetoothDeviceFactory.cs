using System.Diagnostics.CodeAnalysis;

using Bluetooth.Abstractions.Enums;

namespace Bluetooth.Abstractions.Scanning;

/// <summary>
/// Interface representing a factory for creating Bluetooth devices.
/// </summary>
[SuppressMessage("Design", "CA1034:Nested types should not be visible")]
public interface IBluetoothDeviceFactory
{
    /// <summary>
    /// Creates a Bluetooth device based on the provided factory request.
    /// </summary>
    /// <param name="scanner">The Bluetooth scanner that found the device.</param>
    /// <param name="request">The factory request containing necessary information to create the device.</param>
    /// <returns>The created Bluetooth device.</returns>
    IBluetoothDevice CreateDevice(IBluetoothScanner scanner, BluetoothDeviceFactoryRequest request);

    /// <summary>
    /// Record representing a request to create a Bluetooth device.
    /// </summary>
    public record BluetoothDeviceFactoryRequestWithAdvertisement : BluetoothDeviceFactoryRequest
    {
        /// <summary>
        /// Gets the advertisement information used to create the device.
        /// </summary>
        public IBluetoothAdvertisement Advertisement { get; init; } = null!;

        /// <summary>
        /// Gets the unique identifier of the Bluetooth device.
        /// </summary>
        public override string Id => Advertisement.BluetoothAddress;

        /// <summary>
        /// Gets the manufacturer of the Bluetooth device.
        /// </summary>
        public override Manufacturer Manufacturer => Advertisement.Manufacturer;
    }

    /// <summary>
    /// Record representing a request to create a Bluetooth device with ID and Manufacturer.
    /// </summary>
    public record BluetoothDeviceFactoryRequest
    {
        /// <summary>
        /// Gets the unique identifier of the Bluetooth device.
        /// </summary>
        public virtual string Id { get; init; } = string.Empty;

        /// <summary>
        /// Gets the manufacturer of the Bluetooth device.
        /// </summary>
        public virtual Manufacturer Manufacturer { get; init; } = Manufacturer.None;
    }
}
