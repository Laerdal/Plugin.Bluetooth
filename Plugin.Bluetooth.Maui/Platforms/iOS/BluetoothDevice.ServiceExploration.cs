using Plugin.Bluetooth.Maui.PlatformSpecific;
using Plugin.Bluetooth.Maui.PlatformSpecific.Exceptions;

namespace Plugin.Bluetooth.Maui;

public partial class BluetoothDevice
{
    protected override ValueTask NativeServicesExplorationAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        CbPeripheralDelegateProxy.CbPeripheral.DiscoverServices();
        return ValueTask.CompletedTask;
    }

    public void DiscoveredService(NSError? error)
    {
        try
        {
            AppleNativeBluetoothException.ThrowIfError(error);
            var services = CbPeripheralDelegateProxy.CbPeripheral.Services ?? [];
            OnServicesExplorationSucceeded(services, FromInputTypeToOutputTypeConversion, AreRepresentingTheSameObject);
        }
        catch (Exception e)
        {
            OnServicesExplorationFailed(e);
        }
        return;

        BluetoothService FromInputTypeToOutputTypeConversion(CBService native)
        {
            return new BluetoothService(this, native.UUID.ToGuid(), native);
        }
    }

    private static bool AreRepresentingTheSameObject(CBService native, IBluetoothService shared)
    {
        return shared is BluetoothService s && native.UUID.Equals(s.NativeService.UUID) &&  native.Handle.Handle.Equals(s.NativeService.Handle.Handle) ;
    }

    public void ModifiedServices(CBService[] services)
    {
        // Placeholder for future implementation
    }

    public CbPeripheralProxy.ICbServiceDelegate GetService(CBService? characteristicService)
    {
        if (characteristicService == null)
        {
            throw new ServiceNotFoundException(this, null);
        }

        try
        {
            var match = Services.OfType<BluetoothService>().SingleOrDefault(service => AreRepresentingTheSameObject(characteristicService, service));

            if (match == null)
            {
                throw new ServiceNotFoundException(this, characteristicService.UUID.ToGuid());
            }

            return match;
        }
        catch (InvalidOperationException e)
        {
            var matches = Services.OfType<BluetoothService>().Where(service => AreRepresentingTheSameObject(characteristicService, service)).ToArray();
            throw new MultipleServicesFoundException(this, matches, e);
        }
    }
}
