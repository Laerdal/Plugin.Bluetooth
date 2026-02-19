using Bluetooth.Maui.Platforms.Droid.Broadcasting.NativeObjects;

namespace Bluetooth.Maui.Platforms.Droid.Broadcasting;

/// <inheritdoc cref="BaseBluetoothLocalService" />
public partial class BluetoothBroadcastService : BaseBluetoothLocalService, BluetoothGattServerCallbackProxy.IBluetoothGattServiceDelegate
{
    /// <summary>
    /// Gets the native Android Bluetooth GATT service.
    /// </summary>
    public BluetoothGattService? NativeService { get; private set; }

    /// <inheritdoc/>
    public BluetoothBroadcastService(IBluetoothBroadcaster broadcaster, IBluetoothLocalServiceFactory.BluetoothLocalServiceSpec request, IBluetoothLocalCharacteristicFactory characteristicFactory) :
        base(broadcaster, request, characteristicFactory)
    {
        throw new NotImplementedException("BluetoothBroadcastService is not yet implemented on Android.");
    }

    /// <inheritdoc/>
    public BluetoothGattServerCallbackProxy.IBluetoothGattCharacteristicDelegate GetCharacteristic(BluetoothGattCharacteristic? native)
    {
        ArgumentNullException.ThrowIfNull(native);

        var uuid = Guid.Parse(native.Uuid?.ToString() ?? throw new InvalidOperationException("Characteristic UUID is null"));

        var characteristic = GetCharacteristicOrDefault(uuid);
        if (characteristic == null)
        {
            throw new InvalidOperationException($"Characteristic {uuid} not found");
        }

        if (characteristic is not BluetoothBroadcastCharacteristic droidCharacteristic)
        {
            throw new InvalidOperationException("Characteristic is not Android BluetoothBroadcastCharacteristic");
        }

        return droidCharacteristic;
    }

    void BluetoothGattServerCallbackProxy.IBluetoothGattServiceDelegate.OnServiceAdded(GattStatus status)
    {
        // Service has been added to GATT server
        // Nothing specific to do here, just log if needed
    }
}
