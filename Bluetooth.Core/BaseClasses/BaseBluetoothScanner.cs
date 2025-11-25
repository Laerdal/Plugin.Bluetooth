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
    /// Creates a native device from the advertisement
    /// </summary>
    /// <param name="advertisement"></param>
    /// <returns></returns>
    protected abstract IBluetoothDevice NativeCreateDevice(IBluetoothAdvertisement advertisement);

    /// <summary>
    /// Creates and adds a device from the advertisement
    /// </summary>
    /// <param name="advertisement"></param>
    /// <returns></returns>
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
