namespace Bluetooth.Core.BaseClasses;

/// <inheritdoc cref="IBluetoothScanner" />
public abstract partial class BaseBluetoothScanner : BaseBluetoothActivity, IBluetoothScanner
{
    /// <inheritdoc />
    protected async override ValueTask InitializeAsync()
    {
        await KnownServicesAndCharacteristicsRepository.AddAllServiceDefinitionsInCurrentAssemblyAsync().ConfigureAwait(false);
        await NativeInitializeAsync().ConfigureAwait(false);
    }

    /// <inheritdoc />
    public IBluetoothCharacteristicAccessServicesRepository KnownServicesAndCharacteristicsRepository { get; } = new CharacteristicAccessServicesRepository();

    /// <summary>
    /// Creates a native platform-specific device from the advertisement.
    /// </summary>
    /// <param name="advertisement">The advertisement from which to create the device.</param>
    /// <returns>A platform-specific <see cref="IBluetoothDevice"/> instance.</returns>
    protected abstract IBluetoothDevice NativeCreateDevice(IBluetoothAdvertisement advertisement);

    /// <summary>
    /// Creates a native device from the advertisement and adds it to the device list.
    /// </summary>
    /// <param name="advertisement">The advertisement from which to create and add the device.</param>
    /// <returns>The newly created and added <see cref="IBluetoothDevice"/> instance.</returns>
    protected virtual IBluetoothDevice AddDeviceFromAdvertisement(IBluetoothAdvertisement advertisement)
    {
        var device = NativeCreateDevice(advertisement);
        lock (Devices)
        {
            Devices.Add(device);
        }
        return device;
    }
}
