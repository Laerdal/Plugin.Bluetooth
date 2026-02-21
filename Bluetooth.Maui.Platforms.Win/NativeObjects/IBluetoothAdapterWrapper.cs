namespace Bluetooth.Maui.Platforms.Win.NativeObjects;

/// <summary>
///     Interface for the BluetoothManager wrapper to allow for abstraction and easier testing.
/// </summary>
public interface IBluetoothAdapterWrapper
{
    ValueTask<Windows.Devices.Bluetooth.BluetoothAdapter> GetBluetoothAdapterAsync(CancellationToken cancellationToken = default);

    bool BluetoothAdapterIsAdvertisementOffloadSupported { get; }

    bool BluetoothAdapterIsLowEnergySupported { get; }

    bool BluetoothAdapterIsClassicSupported { get; }

    bool BluetoothAdapterAreLowEnergySecureConnectionsSupported { get; }

    bool BluetoothAdapterAreClassicSecureConnectionsSupported { get; }

    bool BluetoothAdapterIsPeripheralRoleSupported { get; }
}