using Bluetooth.Maui.PlatformSpecific;
using Bluetooth.Maui.PlatformSpecific.Exceptions;

namespace Bluetooth.Maui;

public partial class BluetoothService
{
    protected override ValueTask NativeCharacteristicsExplorationAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(NativeService);
        ArgumentNullException.ThrowIfNull(NativeService.Peripheral, nameof(NativeService.Peripheral));

        NativeService.Peripheral.DiscoverCharacteristics(NativeService);
        return ValueTask.CompletedTask;
    }

    public void DiscoveredCharacteristics(NSError? error, CBService service)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(NativeService.Characteristics, nameof(NativeService.Characteristics));

            AppleNativeBluetoothException.ThrowIfError(error);

            OnCharacteristicsExplorationSucceeded(NativeService.Characteristics, FromInputTypeToOutputTypeConversion, AreRepresentingTheSameObject);
        }
        catch (Exception e)
        {
            OnCharacteristicsExplorationFailed(e);
        }

        return;

        BluetoothCharacteristic FromInputTypeToOutputTypeConversion(CBCharacteristic native)
        {
            return new BluetoothCharacteristic(this, native.UUID.ToGuid(), native);
        }
    }

    private static bool AreRepresentingTheSameObject(CBCharacteristic native, IBluetoothCharacteristic shared)
    {
        return shared is BluetoothCharacteristic s && native.UUID.Equals(s.NativeCharacteristic.UUID) && native.Handle.Handle.Equals(s.NativeCharacteristic.Handle.Handle);
    }

    public CbPeripheralProxy.ICbCharacteristicDelegate GetCharacteristic(CBCharacteristic? characteristic)
    {
        if (characteristic == null)
        {
            throw new CharacteristicNotFoundException(this, null);
        }

        try
        {
            var match = Characteristics.OfType<BluetoothCharacteristic>().SingleOrDefault(sharedCharacteristic => AreRepresentingTheSameObject(characteristic, sharedCharacteristic));

            if (match == null)
            {
                throw new CharacteristicNotFoundException(this, characteristic.UUID.ToGuid());
            }

            return match;
        }
        catch (InvalidOperationException e)
        {
            var matches = Characteristics.OfType<BluetoothCharacteristic>().Where(sharedCharacteristic => AreRepresentingTheSameObject(characteristic, sharedCharacteristic)).ToArray();
            throw new MultipleCharacteristicsFoundException(this, matches, e);
        }
    }
}
