using Bluetooth.Abstractions.Scanning.Factories;

namespace Bluetooth.Maui.Platforms.Droid.Scanning;

/// <inheritdoc/>
public record BluetoothDeviceFactoryRequest : IBluetoothDeviceFactory.BluetoothDeviceFactoryRequest
{
    /// <summary>
    /// Gets or sets the native Android Bluetooth device.
    /// </summary>
    public Android.Bluetooth.BluetoothDevice? NativeDevice { get; init; }
}
