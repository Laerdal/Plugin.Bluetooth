using Android.Bluetooth;

using Bluetooth.Maui.Platforms.Droid.Broadcasting.NativeObjects;

namespace Bluetooth.Maui.Platforms.Droid.Broadcasting;

/// <inheritdoc/>
public partial class BluetoothBroadcastCharacteristic : BaseBluetoothBroadcastCharacteristic,
    BluetoothGattServerCallbackProxy.ICharacteristic
{
    /// <summary>
    /// Gets the native Android Bluetooth GATT characteristic.
    /// </summary>
    public BluetoothGattCharacteristic NativeCharacteristic { get; }

    /// <inheritdoc/>
    public BluetoothBroadcastCharacteristic(IBluetoothBroadcastService service, IBluetoothBroadcastCharacteristicFactory.BluetoothBroadcastCharacteristicFactoryRequest request) : base(service, request)
    {
        // Convert properties to Android GATT properties
        var properties = GattProperty.Read;
        if (request.Properties.HasFlag(CharacteristicProperties.Write) || request.Properties.HasFlag(CharacteristicProperties.WriteWithoutResponse))
        {
            properties |= GattProperty.Write;
        }
        if (request.Properties.HasFlag(CharacteristicProperties.Indicate))
        {
            properties |= GattProperty.Indicate;
        }
        if (request.Properties.HasFlag(CharacteristicProperties.Notify))
        {
            properties |= GattProperty.Notify;
        }

        // Convert permissions to Android GATT permissions
        var permissions = GattPermission.Read;
        if (request.Permissions.HasFlag(CharacteristicPermissions.Write) ||
            request.Permissions.HasFlag(CharacteristicPermissions.WriteEncrypted) ||
            request.Permissions.HasFlag(CharacteristicPermissions.WriteEncryptedMitm) ||
            request.Permissions.HasFlag(CharacteristicPermissions.WriteSigned) ||
            request.Permissions.HasFlag(CharacteristicPermissions.WriteSignedMitm))
        {
            permissions |= GattPermission.Write;
        }

        // Create native GATT characteristic
        NativeCharacteristic = new BluetoothGattCharacteristic(
            Java.Util.UUID.FromString(request.Id.ToString()),
            properties,
            permissions
        );

        // Add CCCD descriptor if characteristic supports notifications
        if (request.Properties.HasFlag(CharacteristicProperties.Notify) ||
            request.Properties.HasFlag(CharacteristicProperties.Indicate))
        {
            var cccdUuid = Java.Util.UUID.FromString("00002902-0000-1000-8000-00805f9b34fb");
#pragma warning disable CA2000
            var cccdDescriptor = new BluetoothGattDescriptor(
                                                             cccdUuid,
                                                             GattDescriptorPermission.Read | GattDescriptorPermission.Write
                                                            );
#pragma warning restore CA2000
            NativeCharacteristic.AddDescriptor(cccdDescriptor);
        }

        // Set initial value if provided
        if (request.InitialValue != null)
        {
            if (OperatingSystem.IsAndroidVersionAtLeast(33))
            {
                NativeCharacteristic.
            }
            else
            {
                NativeCharacteristic.SetValue(request.InitialValue.Value.ToArray());
            }
        }
    }

    /// <inheritdoc/>
    public override ValueTask DisposeAsync()
    {
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc/>
    protected async override Task NativeUpdateValueAsync(ReadOnlyMemory<byte> value, bool notifyClients = true, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        // Update the characteristic value
        NativeCharacteristic.SetValue(value.ToArray());

        // Notify connected clients if requested
        if (notifyClients && Service.Broadcaster is BluetoothBroadcaster broadcaster && broadcaster.GattServerProxy != null)
        {
            foreach (var device in Service.Broadcaster.ConnectedDevices.Values)
            {
                if (device is BluetoothBroadcastClientDevice droidDevice && droidDevice.NativeDevice != null)
                {
                    broadcaster.GattServerProxy.BluetoothGattServer.NotifyCharacteristicChanged(
                        droidDevice.NativeDevice,
                        NativeCharacteristic,
                        false
                    );
                }
            }
        }

        await Task.CompletedTask.ConfigureAwait(false);
    }

    // BluetoothGattServerCallbackProxy.ICharacteristic implementation
    void BluetoothGattServerCallbackProxy.ICharacteristic.OnCharacteristicReadRequest(
        BluetoothGattServerCallbackProxy.IDevice sharedDevice,
        int requestId,
        int offset)
    {
        // Send response with the current characteristic value
        if (Service.Broadcaster is BluetoothBroadcaster broadcaster &&
            broadcaster.GattServerProxy != null &&
            sharedDevice is BluetoothBroadcastClientDevice device &&
            device.NativeDevice != null)
        {
            var value = NativeCharacteristic.GetValue() ?? [];
            broadcaster.GattServerProxy.BluetoothGattServer.SendResponse(
                device.NativeDevice,
                requestId,
                GattStatus.Success,
                offset,
                value
            );
        }
    }

    void BluetoothGattServerCallbackProxy.ICharacteristic.OnCharacteristicWriteRequest(
        BluetoothGattServerCallbackProxy.IDevice sharedDevice,
        int requestId,
        bool preparedWrite,
        bool responseNeeded,
        int offset,
        byte[] value)
    {
        // Update the characteristic value
        NativeCharacteristic.SetValue(value);

        // Update the Value property
        Value = value;

        // Send response if needed
        if (responseNeeded &&
            Service.Broadcaster is BluetoothBroadcaster broadcaster &&
            broadcaster.GattServerProxy != null &&
            sharedDevice is BluetoothBroadcastClientDevice device &&
            device.NativeDevice != null)
        {
            broadcaster.GattServerProxy.BluetoothGattServer.SendResponse(
                device.NativeDevice,
                requestId,
                GattStatus.Success,
                offset,
                value
            );
        }

        // Raise ValueWritten event
        OnValueWritten(new()
        {
            Characteristic = this,
            Device = sharedDevice,
            Value = value
        });
    }

    void BluetoothGattServerCallbackProxy.ICharacteristic.OnDescriptorReadRequest(
        BluetoothGattServerCallbackProxy.IDevice sharedDevice,
        int requestId,
        int offset,
        BluetoothGattDescriptor? descriptor)
    {
        // Send response with descriptor value
        if (Service.Broadcaster is BluetoothBroadcaster broadcaster &&
            broadcaster.GattServerProxy != null &&
            sharedDevice is BluetoothBroadcastClientDevice device &&
            device.NativeDevice != null &&
            descriptor != null)
        {
            var value = descriptor.GetValue() ?? [];
            broadcaster.GattServerProxy.BluetoothGattServer.SendResponse(
                device.NativeDevice,
                requestId,
                GattStatus.Success,
                offset,
                value
            );
        }
    }

    void BluetoothGattServerCallbackProxy.ICharacteristic.OnDescriptorWriteRequest(
        BluetoothGattServerCallbackProxy.IDevice sharedDevice,
        int requestId,
        BluetoothGattDescriptor? descriptor,
        bool preparedWrite,
        bool responseNeeded,
        int offset,
        byte[] value)
    {
        // Update descriptor value (typically CCCD for notifications)
        descriptor?.SetValue(value);

        // Send response if needed
        if (responseNeeded &&
            Service.Broadcaster is BluetoothBroadcaster broadcaster &&
            broadcaster.GattServerProxy != null &&
            sharedDevice is BluetoothBroadcastClientDevice device &&
            device.NativeDevice != null)
        {
            broadcaster.GattServerProxy.BluetoothGattServer.SendResponse(
                device.NativeDevice,
                requestId,
                GattStatus.Success,
                offset,
                value
            );
        }
    }
}
