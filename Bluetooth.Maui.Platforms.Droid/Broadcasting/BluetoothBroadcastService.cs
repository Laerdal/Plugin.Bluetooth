using Android.Bluetooth;

using Bluetooth.Maui.Platforms.Droid.Broadcasting.NativeObjects;

namespace Bluetooth.Maui.Platforms.Droid.Broadcasting;

/// <inheritdoc/>
public partial class BluetoothBroadcastService : BaseBluetoothBroadcastService,
    BluetoothGattServerCallbackProxy.IService
{
    /// <summary>
    /// Gets the native Android Bluetooth GATT service.
    /// </summary>
    public BluetoothGattService NativeService { get; }

    /// <inheritdoc/>
    public BluetoothBroadcastService(IBluetoothBroadcaster broadcaster,
        IBluetoothBroadcastServiceFactory.BluetoothBroadcastServiceFactoryRequest request,
        IBluetoothBroadcastCharacteristicFactory characteristicFactory) : base(broadcaster, request, characteristicFactory)
    {
        // Create native GATT service
        var serviceType = request.IsPrimary ? GattServiceType.Primary : GattServiceType.Secondary;
        NativeService = new BluetoothGattService(
            Java.Util.UUID.FromString(request.Id.ToString()),
            serviceType
        ) ?? throw new InvalidOperationException("Failed to create GATT service");

        // Add characteristics to the native service
        foreach (var characteristic in Characteristics.Values)
        {
            if (characteristic is BluetoothBroadcastCharacteristic droidCharacteristic)
            {
                NativeService.AddCharacteristic(droidCharacteristic.NativeCharacteristic);
            }
        }
    }

    /// <inheritdoc/>
    public override ValueTask DisposeAsync()
    {
        return base.DisposeAsync();
    }

    // BluetoothGattServerCallbackProxy.IService implementation
    BluetoothGattServerCallbackProxy.ICharacteristic BluetoothGattServerCallbackProxy.IService.GetCharacteristic(BluetoothGattCharacteristic? native)
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
