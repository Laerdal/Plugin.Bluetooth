using Bluetooth.Maui.Platforms.Apple.PlatformSpecific;
using Bluetooth.Maui.Platforms.Apple.Scanning.NativeObjects;

namespace Bluetooth.Maui.Platforms.Apple.Scanning;

/// <inheritdoc/>
public record BluetoothDeviceFactoryRequest : IBluetoothDeviceFactory.BluetoothDeviceFactoryRequest
{
    /// <summary>
    /// Gets the iOS Core Bluetooth peripheral delegate wrapper.
    /// </summary>
    public CbPeripheralWrapper? CbPeripheralWrapper { get; init; }
}
