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
}
