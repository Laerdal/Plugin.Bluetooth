using System.Diagnostics.CodeAnalysis;

namespace Bluetooth.Maui.Platforms.Apple.Scanning.Factories;

/// <inheritdoc />
public record AppleBluetoothDeviceFactoryRequest : IBluetoothDeviceFactory.BluetoothDeviceFactoryRequest
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="AppleBluetoothDeviceFactoryRequest" /> record.
    /// </summary>
    /// <param name="peripheral">The native iOS Core Bluetooth peripheral.</param>
    /// <param name="manufacturer">The manufacturer of the Bluetooth device, if known.</param>
    public AppleBluetoothDeviceFactoryRequest(CBPeripheral peripheral, Manufacturer manufacturer = Manufacturer.None) : base(peripheral?.Identifier.ToString() ?? throw new ArgumentNullException(nameof(peripheral)), manufacturer)
    {
        CbPeripheral = peripheral;
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="AppleBluetoothDeviceFactoryRequest" /> record from a Bluetooth advertisement.
    /// </summary>
    /// <param name="advertisement">The Bluetooth advertisement containing the peripheral information.</param>
    public AppleBluetoothDeviceFactoryRequest(AppleBluetoothAdvertisement advertisement) : base(advertisement)
    {
        CbPeripheral = advertisement.CbPeripheral;
    }

    /// <summary>
    ///     Gets the native iOS Core Bluetooth peripheral.
    /// </summary>
    public CBPeripheral CbPeripheral { get; init; }
}