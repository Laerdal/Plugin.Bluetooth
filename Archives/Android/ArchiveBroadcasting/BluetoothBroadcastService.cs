using Android.Bluetooth;

using Bluetooth.Abstractions.Broadcasting.Factories;
using Bluetooth.Maui.Platforms.Droid.Broadcasting.NativeObjects;

namespace Bluetooth.Maui.Platforms.Droid.Broadcasting;

/// <inheritdoc/>
public partial class BluetoothBroadcastService : BaseBluetoothBroadcastService, BluetoothGattServerCallbackProxy.IService
{
    /// <summary>
    /// Gets the native Android Bluetooth GATT service.
    /// </summary>
    public BluetoothGattService NativeService { get; private set; }

    /// <inheritdoc/>
    public BluetoothBroadcastService(IBluetoothBroadcaster broadcaster, IBluetoothBroadcastServiceFactory.BluetoothBroadcastServiceFactoryRequest request, IBluetoothBroadcastCharacteristicFactory characteristicFactory) :
        base(broadcaster, request, characteristicFactory)
    {
        throw new NotImplementedException("BluetoothBroadcastService is not yet implemented on Android.");
    }

    /// <inheritdoc/>
    public override ValueTask DisposeAsync()
    {
        return base.DisposeAsync();
    }

    // BluetoothGattServerCallbackProxy.IService implementation
    public BluetoothGattServerCallbackProxy.ICharacteristic GetCharacteristic(BluetoothGattCharacteristic? native)
    {
        ArgumentNullException.ThrowIfNull(native);

        var uuid = Guid.Parse(native.Uuid?.ToString() ?? throw new InvalidOperationException("Characteristic UUID is null"));

        if (!Characteristics.TryGetValue(uuid, out var characteristic))
        {
            throw new InvalidOperationException($"Characteristic {uuid} not found");
        }

        if (characteristic is not BluetoothBroadcastCharacteristic droidCharacteristic)
        {
            throw new InvalidOperationException("Characteristic is not Android BluetoothBroadcastCharacteristic");
        }

        return droidCharacteristic;
    }

    void BluetoothGattServerCallbackProxy.IService.OnServiceAdded(GattStatus status)
    {
        // Service has been added to GATT server
        // Nothing specific to do here, just log if needed
    }
}
