using Bluetooth.Maui.PlatformSpecific;

namespace Bluetooth.Maui;

/// <summary>
/// Windows implementation of the Bluetooth scanner using Windows.Devices.Bluetooth APIs.
/// </summary>
/// <remarks>
/// This implementation uses <see cref="BluetoothLEAdvertisementWatcher"/> to monitor BLE advertisements.
/// </remarks>
public partial class BluetoothScanner : BaseBluetoothScanner, BluetoothLeAdvertisementWatcherWrapper.IBluetoothLeAdvertisementWatcherProxyDelegate
{
    /// <summary>
    /// Gets the Bluetooth LE advertisement watcher proxy.
    /// </summary>
    public BluetoothLeAdvertisementWatcherWrapper BluetoothLeAdvertisementWatcherProxy { get; protected set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="BluetoothScanner"/> class.
    /// </summary>
    /// <param name="adapter">The Bluetooth adapter.</param>
    /// <param name="knownServicesAndCharacteristicsRepository">The repository for known services and characteristics.</param>
    public BluetoothScanner(IBluetoothAdapter adapter, IBluetoothCharacteristicAccessServicesRepository knownServicesAndCharacteristicsRepository) :
        base(adapter, knownServicesAndCharacteristicsRepository)
    {
        BluetoothLeAdvertisementWatcherProxy = ((BluetoothAdapter)Adapter).InitializeBluetoothLeAdvertisementWatcher(this);
    }

    /// <inheritdoc/>
    public async override Task EnsurePermissionsAsync(CancellationToken cancellationToken = default)
    {
        var adapter = await ((BluetoothAdapter) Adapter).GetBluetoothAdapterAsync().ConfigureAwait(false);
        var radio = await ((BluetoothAdapter) Adapter).GetBluetoothRadioAsync(adapter).ConfigureAwait(false);

        // Is Peripheral role supported ?
        if (!adapter.IsPeripheralRoleSupported)
        {
            throw new PermissionException($"BluetoothAdapter.IsPeripheralRoleSupported = false");
        }

        if (radio.State != RadioState.On) // trying to turn on BT Radio
        {
            // ReSharper disable once RedundantNameQualifier
            var access = await Windows.Devices.Radios.Radio.RequestAccessAsync();
            if (access != RadioAccessStatus.Allowed)
            {
                throw new PermissionException($"Radio.RequestAccessAsync = {access}, Did you forget to add '<DeviceCapability Name=\"radios\" />' in your Manifest's Capabilities ?");
            }

            var success = await radio.SetStateAsync(RadioState.On);
            if (success != RadioAccessStatus.Allowed)
            {
                throw new PermissionException($"Radio.SetStateAsync(RadioState.On) = {success}, Did you forget to add '<DeviceCapability Name=\"radios\" />' in your Manifest's Capabilities ?");
            }
        }
    }
}
