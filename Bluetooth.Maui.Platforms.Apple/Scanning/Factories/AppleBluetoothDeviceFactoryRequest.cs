namespace Bluetooth.Maui.Platforms.Apple.Scanning.Factories;

/// <inheritdoc />
public record AppleBluetoothRemoteDeviceFactorySpec : IBluetoothRemoteDeviceFactory.BluetoothRemoteDeviceFactorySpec
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="AppleBluetoothRemoteDeviceFactorySpec" /> record.
    /// </summary>
    /// <param name="peripheral">The native iOS Core Bluetooth peripheral.</param>
    /// <param name="manufacturer">The manufacturer of the Bluetooth device, if known.</param>
    public AppleBluetoothRemoteDeviceFactorySpec(CBPeripheral peripheral, Manufacturer manufacturer = Manufacturer.None) : base(peripheral?.Identifier.ToString() ?? throw new ArgumentNullException(nameof(peripheral)), manufacturer)
    {
        CbPeripheral = peripheral;
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="AppleBluetoothRemoteDeviceFactorySpec" /> record from a Bluetooth advertisement.
    /// </summary>
    /// <param name="advertisement">The Bluetooth advertisement containing the peripheral information.</param>
    public AppleBluetoothRemoteDeviceFactorySpec(AppleBluetoothAdvertisement advertisement) : base(advertisement)
    {
        CbPeripheral = advertisement.CbPeripheral;
    }

    /// <summary>
    ///     Gets the native iOS Core Bluetooth peripheral.
    /// </summary>
    public CBPeripheral CbPeripheral { get; init; }
}
