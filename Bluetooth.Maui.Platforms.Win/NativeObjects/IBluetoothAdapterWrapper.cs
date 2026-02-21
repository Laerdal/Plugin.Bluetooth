namespace Bluetooth.Maui.Platforms.Win.NativeObjects;

/// <summary>
///     Interface for the BluetoothAdapter wrapper to provide abstraction and easier testing of Bluetooth adapter operations.
/// </summary>
public interface IBluetoothAdapterWrapper
{
    /// <summary>
    ///     Gets the Bluetooth adapter, ensuring that only one instance is created and shared across the application.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the Bluetooth adapter.</returns>
    ValueTask<Windows.Devices.Bluetooth.BluetoothAdapter> GetBluetoothAdapterAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    ///     Gets the Bluetooth adapter's unique identifier as a string.
    ///     The Bluetooth adapter ID is a unique identifier for the Bluetooth adapter and is typically represented as a string in the format "BluetoothAdapter#BluetoothRadioId" (e.g., "BluetoothAdapter#Radio_1").
    ///     This ID can be used to identify and differentiate between multiple Bluetooth adapters on a device, if present.
    /// </summary>
    string BluetoothAdapterDeviceId { get; }
    
    /// <summary>
    ///     Gets the Bluetooth adapter's Bluetooth address as a 64-bit unsigned integer.
    ///     The Bluetooth address is a unique identifier for the Bluetooth adapter and is typically represented as a 48-bit address in hexadecimal format (e.g., "00:1A:7D:DA:71:13").
    ///     However, in this case, it is returned as a 64-bit unsigned integer for easier processing and storage.
    /// </summary>
    ulong BluetoothAdapterBluetoothAddress { get; }
    
    
    /// <summary>
    ///     Gets a value indicating whether Bluetooth advertisement offload is supported.
    /// </summary>
    bool BluetoothAdapterIsAdvertisementOffloadSupported { get; }

    /// <summary>
    ///     Gets a value indicating whether Bluetooth Low Energy is supported.
    /// </summary>
    bool BluetoothAdapterIsLowEnergySupported { get; }

    /// <summary>
    ///     Gets a value indicating whether classic Bluetooth is supported.
    /// </summary>
    bool BluetoothAdapterIsClassicSupported { get; }

    /// <summary>
    ///     Gets a value indicating whether Bluetooth Low Energy secure connections are supported.
    /// </summary>
    bool BluetoothAdapterAreLowEnergySecureConnectionsSupported { get; }

    /// <summary>
    ///     Gets a value indicating whether classic Bluetooth secure connections are supported.
    /// </summary>
    bool BluetoothAdapterAreClassicSecureConnectionsSupported { get; }

    /// <summary>
    ///     Gets a value indicating whether the peripheral role is supported.
    /// </summary>
    bool BluetoothAdapterIsPeripheralRoleSupported { get; }
    
    /// <summary>
    ///     Gets a value indicating whether the central role is supported.
    /// </summary>
    bool BluetoothAdapterIsCentralRoleSupported { get; }
    
    /// <summary>
    ///     Gets the maximum length of Bluetooth advertisement data that can be sent.
    ///     Only available on Windows 10 version 19041 and later.
    /// </summary>
    uint BluetoothAdapterMaxAdvertisementDataLength { get; }
    
    /// <summary>
    ///     Gets a value indicating whether extended advertising is supported.
    ///     Only available on Windows 10 version 19041 and later.
    /// </summary>
    bool BluetoothAdapterIsExtendedAdvertisingSupported { get; }
}
