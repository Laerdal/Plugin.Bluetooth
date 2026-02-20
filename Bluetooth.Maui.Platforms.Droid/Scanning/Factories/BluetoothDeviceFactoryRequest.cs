namespace Bluetooth.Maui.Platforms.Droid.Scanning.Factories;

/// <inheritdoc />
public record BluetoothDeviceFactoryRequest : IBluetoothDeviceFactory.BluetoothDeviceFactoryRequest
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="BluetoothDeviceFactoryRequest" /> record from a Bluetooth advertisement.
    /// </summary>
    /// <param name="advertisement">The Bluetooth advertisement containing the device information.</param>
    public BluetoothDeviceFactoryRequest(IBluetoothAdvertisement advertisement) : base(advertisement)
    {
        // NativeDevice is not available from advertisement on Android
        NativeDevice = null;
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="BluetoothDeviceFactoryRequest" /> record with device ID and manufacturer.
    /// </summary>
    /// <param name="deviceId">The unique identifier of the Bluetooth device.</param>
    /// <param name="manufacturer">The manufacturer information of the Bluetooth device.</param>
    /// <param name="nativeDevice">The native Android Bluetooth device.</param>
    public BluetoothDeviceFactoryRequest(string deviceId, Manufacturer manufacturer, BluetoothDevice? nativeDevice = null) : base(deviceId, manufacturer)
    {
        NativeDevice = nativeDevice;
    }

    /// <summary>
    ///     Gets the native Android Bluetooth device.
    /// </summary>
    public BluetoothDevice? NativeDevice { get; init; }
}