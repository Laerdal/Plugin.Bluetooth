using Bluetooth.Maui.PlatformSpecific;

namespace Bluetooth.Maui;

public partial class BluetoothService
{
    protected override ValueTask NativeCharacteristicsExplorationAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        if (NativeService.Characteristics == null)
        {
            throw new InvalidOperationException("NativeService.Characteristics == null");

            // MIGHT NEED Retries and/or delays
        }

        // on android, characteristics are scanned at the same time the Service is discovered
        OnCharacteristicsExplorationSucceeded(NativeService.Characteristics, FromInputTypeToOutputTypeConversion, AreRepresentingTheSameObject);

        return ValueTask.CompletedTask;

    }

    private BluetoothCharacteristic FromInputTypeToOutputTypeConversion(BluetoothGattCharacteristic native)
    {
        var id = native.Uuid?.ToGuid() ?? throw new InvalidOperationException("nativeService.Uuid == null");
        return new BluetoothCharacteristic(this, id, native);
    }

    /// <summary>
    /// Compares a native Characteristic to a shared Characteristic.
    /// While they are different types, we can usually determine if the
    /// shared Characteristic is a wrapper around the same Native characteristic or not.
    /// </summary>
    /// <param name="native">Native characteristic</param>
    /// <param name="shared">Shared characteristic</param>
    /// <returns>true if the Native characteristic wrapped in the Shared characteristic, and the Native characteristic are the same</returns>
    private static bool AreRepresentingTheSameObject(BluetoothGattCharacteristic native, IBluetoothCharacteristic shared)
    {
        return shared is BluetoothCharacteristic s && (native.Uuid?.Equals(s.NativeCharacteristic.Uuid) ?? false) && native.InstanceId.Equals(s.NativeCharacteristic.InstanceId);
    }

    public BluetoothGattProxy.ICharacteristic GetCharacteristic(BluetoothGattCharacteristic? nativeCharacteristic)
    {
        ArgumentNullException.ThrowIfNull(nativeCharacteristic);

        try
        {
            var match = Characteristics.OfType<BluetoothCharacteristic>().SingleOrDefault(characteristic => AreRepresentingTheSameObject(nativeCharacteristic, characteristic));

            if (match == null)
            {
                throw new CharacteristicNotFoundException(this, nativeCharacteristic.Uuid?.ToGuid());
            }

            return match;
        }
        catch (InvalidOperationException e)
        {
            var matches = Characteristics.OfType<BluetoothCharacteristic>().Where(characteristic => AreRepresentingTheSameObject(nativeCharacteristic, characteristic)).ToArray();
            throw new MultipleCharacteristicsFoundException(this, matches, e);
        }
    }
}
