using Bluetooth.Maui.Platforms.Droid.Broadcasting.NativeObjects;
using Bluetooth.Maui.Platforms.Droid.Tools;

namespace Bluetooth.Maui.Platforms.Droid.Broadcasting;

/// <inheritdoc cref="BaseBluetoothLocalService" />
public class AndroidBluetoothLocalService : BaseBluetoothLocalService, BluetoothGattServerCallbackProxy.IBluetoothGattServiceDelegate
{
    /// <inheritdoc />
    public AndroidBluetoothLocalService(IBluetoothBroadcaster broadcaster, IBluetoothLocalServiceFactory.BluetoothLocalServiceSpec spec, IBluetoothLocalCharacteristicFactory characteristicFactory) :
        base(broadcaster, spec ?? throw new ArgumentNullException(nameof(spec)), characteristicFactory)
    {
        NativeService = new BluetoothGattService(spec.ServiceId.ToUuid(), spec.IsPrimary ? GattServiceType.Primary : GattServiceType.Secondary);
    }

    /// <summary>
    ///     Gets the native Android Bluetooth GATT service.
    /// </summary>
    public BluetoothGattService? NativeService { get; private set; }

    /// <inheritdoc />
    public BluetoothGattServerCallbackProxy.IBluetoothGattCharacteristicDelegate GetCharacteristic(BluetoothGattCharacteristic? native)
    {
        ArgumentNullException.ThrowIfNull(native);

        var uuid = Guid.Parse(native.Uuid?.ToString() ?? throw new InvalidOperationException("Characteristic UUID is null"));

        var characteristic = GetCharacteristicOrDefault(uuid);
        if (characteristic == null)
        {
            throw new InvalidOperationException($"Characteristic {uuid} not found");
        }

        if (characteristic is not AndroidBluetoothLocalCharacteristic droidCharacteristic)
        {
            throw new InvalidOperationException("Characteristic is not Android AndroidBluetoothLocalCharacteristic");
        }

        return droidCharacteristic;
    }

    /// <inheritdoc />
    protected override ValueTask<IBluetoothLocalCharacteristic> NativeCreateCharacteristicAsync(
        Guid id,
        BluetoothCharacteristicProperties properties,
        BluetoothCharacteristicPermissions permissions,
        string? name = null,
        TimeSpan? timeout = null,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (CharacteristicFactory == null)
        {
            throw new InvalidOperationException("CharacteristicFactory is not configured for Android local service creation.");
        }

        var characteristicSpec = new IBluetoothLocalCharacteristicFactory.BluetoothLocalCharacteristicSpec(id,
                                                                                                           properties,
                                                                                                           permissions,
                                                                                                           name);
        var characteristic = CharacteristicFactory.Create(this, characteristicSpec);

        if (characteristic is not AndroidBluetoothLocalCharacteristic androidCharacteristic)
        {
            throw new InvalidOperationException("Characteristic created by factory is not AndroidBluetoothLocalCharacteristic.");
        }

        NativeService?.AddCharacteristic(androidCharacteristic.NativeCharacteristic);
        return new ValueTask<IBluetoothLocalCharacteristic>(characteristic);
    }

    void BluetoothGattServerCallbackProxy.IBluetoothGattServiceDelegate.OnServiceAdded(GattStatus status)
    {
        // Service has been added to GATT server
        // Nothing specific to do here, just log if needed
    }
}
