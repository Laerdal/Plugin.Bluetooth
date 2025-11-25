using Bluetooth.Maui.PlatformSpecific;
using Bluetooth.Maui.PlatformSpecific.Exceptions;

namespace Bluetooth.Maui;

public partial class BluetoothDevice
{
    protected override ValueTask NativeServicesExplorationAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(BluetoothGattProxy);
        BluetoothGattProxy.BluetoothGatt.DiscoverServices();
        return ValueTask.CompletedTask;
    }

    public BluetoothGattProxy.IService GetService(BluetoothGattService? nativeService)
    {
        ArgumentNullException.ThrowIfNull(nativeService);

        try
        {
            var match = Services.Cast<BluetoothService>().SingleOrDefault(service => AreRepresentingTheSameObject(nativeService, service));

            if (match == null)
            {
                throw new ServiceNotFoundException(this, nativeService.Uuid?.ToGuid());
            }

            return match;
        }
        catch (InvalidOperationException e)
        {
            var matches = Services.OfType<BluetoothService>().Where(service => AreRepresentingTheSameObject(nativeService, service)).ToArray();
            throw new MultipleServicesFoundException(this, matches, e);
        }
    }

    private static bool AreRepresentingTheSameObject(BluetoothGattService native, IBluetoothService shared)
    {
        if(shared is not BluetoothService s)
        {
            return false;
        }
        return (native.Uuid?.Equals(s.NativeService.Uuid) ?? false) && native.InstanceId.Equals(s.NativeService.InstanceId);
    }

    public void OnServicesDiscovered(GattStatus status)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(BluetoothGattProxy);
            AndroidNativeGattCallbackStatusException.ThrowIfNotSuccess(status);
            var input = BluetoothGattProxy.BluetoothGatt.Services;
            if(input == null)
            {
                throw new UnexpectedServiceExplorationException(this, "Discovered services list is null.");
            }
            OnServicesExplorationSucceeded(input, FromInputTypeToOutputTypeConversion, AreRepresentingTheSameObject);
        }
        catch (Exception e)
        {
            OnServicesExplorationFailed(e);
        }
    }

    private BluetoothService FromInputTypeToOutputTypeConversion(BluetoothGattService native)
    {
        return new BluetoothService(this, native.Uuid.ToGuid(), native);
    }

    public void OnServiceChanged()
    {
        BluetoothGattProxy?.BluetoothGatt.DiscoverServices();
    }
}
