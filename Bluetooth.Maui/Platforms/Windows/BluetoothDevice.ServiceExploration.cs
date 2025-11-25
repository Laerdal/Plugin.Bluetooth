namespace Bluetooth.Maui;

public partial class BluetoothDevice
{
    protected async override ValueTask NativeServicesExplorationAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(BluetoothLeDeviceProxy);
            var result = await BluetoothLeDeviceProxy.ReadGattServicesAsync(BluetoothCacheMode.Uncached, cancellationToken: cancellationToken).ConfigureAwait(false);

            // Async task on Windows with result;
            OnServicesExplorationSucceeded(result, FromInputTypeToOutputTypeConversion, AreRepresentingTheSameObject);
            return;

            BluetoothService FromInputTypeToOutputTypeConversion(GattDeviceService native)
            {
                return new BluetoothService(this, native.Uuid, native);
            }
        }
        catch (Exception ex)
        {
            OnServicesExplorationFailed(ex);
        }
    }

    static bool AreRepresentingTheSameObject(GattDeviceService native, IBluetoothService shared)
    {
        return shared is BluetoothService s && native.Uuid.Equals(s.NativeServiceProxy.GattDeviceService.Uuid) && native.AttributeHandle.Equals(s.NativeServiceProxy.GattDeviceService.AttributeHandle);
    }
}
