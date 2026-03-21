using Bluetooth.Maui.Platforms.Droid.Broadcasting.NativeObjects;
using Bluetooth.Maui.Platforms.Droid.Tools;

using DescriptorNotFoundException = Bluetooth.Abstractions.Broadcasting.Exceptions.DescriptorNotFoundException;

namespace Bluetooth.Maui.Platforms.Droid.Broadcasting;

/// <inheritdoc cref="BaseBluetoothLocalCharacteristic" />
public class AndroidBluetoothLocalCharacteristic : BaseBluetoothLocalCharacteristic,
    BluetoothGattServerCallbackProxy.IBluetoothGattCharacteristicDelegate
{
    /// <inheritdoc />
    public AndroidBluetoothLocalCharacteristic(IBluetoothLocalService service, IBluetoothLocalCharacteristicFactory.BluetoothLocalCharacteristicSpec spec, IBluetoothLocalDescriptorFactory descriptorFactory) : base(service, spec ?? throw new ArgumentNullException(nameof(spec)),
        descriptorFactory)
    {
        NativeCharacteristic = new BluetoothGattCharacteristic(spec.CharacteristicId.ToUuid(), spec.Properties.ToNative(), spec.Permissions.ToNative());
    }

    /// <summary>
    ///     Gets the native Android GATT characteristic.
    /// </summary>
    public BluetoothGattCharacteristic NativeCharacteristic { get; }

    /// <inheritdoc />
    protected override ValueTask<IBluetoothLocalDescriptor> NativeCreateDescriptorAsync(
        Guid id,
        string? name = null,
        TimeSpan? timeout = null,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (DescriptorFactory == null)
        {
            throw new InvalidOperationException("DescriptorFactory is not configured for Android local characteristic creation.");
        }

        var descriptorSpec = new IBluetoothLocalDescriptorFactory.BluetoothLocalDescriptorSpec(id, name);
        var descriptor = DescriptorFactory.Create(this, descriptorSpec);
        if (descriptor is not AndroidBluetoothLocalDescriptor androidDescriptor)
        {
            throw new InvalidOperationException("Descriptor created by factory is not AndroidBluetoothLocalDescriptor.");
        }

        NativeCharacteristic.AddDescriptor(androidDescriptor.NativeDescriptor);
        return new ValueTask<IBluetoothLocalDescriptor>(descriptor);
    }

    /// <inheritdoc />
    public void OnCharacteristicReadRequest(
        BluetoothGattServerCallbackProxy.IBluetoothDeviceDelegate sharedBluetoothDeviceDelegate,
        int requestId,
        int offset)
    {
        ArgumentNullException.ThrowIfNull(sharedBluetoothDeviceDelegate);

        if (sharedBluetoothDeviceDelegate is not AndroidBluetoothConnectedDevice androidDevice || androidDevice.NativeDevice == null)
        {
            return;
        }

        var broadcaster = (AndroidBluetoothBroadcaster) Service.Broadcaster;

        try
        {
            var responseValue = OnReadRequestReceived(androidDevice).ToArray();
            broadcaster.TrySendResponse(androidDevice.NativeDevice,
                                        requestId,
                                        GattStatus.Success,
                                        offset,
                                        responseValue);
        }
        catch (Exception)
        {
            broadcaster.TrySendResponse(androidDevice.NativeDevice,
                                        requestId,
                                        GattStatus.RequestNotSupported,
                                        offset,
                                        []);
        }
    }

    /// <inheritdoc />
    public async void OnCharacteristicWriteRequest(
        BluetoothGattServerCallbackProxy.IBluetoothDeviceDelegate sharedBluetoothDeviceDelegate,
        int requestId,
        bool preparedWrite,
        bool responseNeeded,
        int offset,
        byte[] value)
    {
        ArgumentNullException.ThrowIfNull(sharedBluetoothDeviceDelegate);
        ArgumentNullException.ThrowIfNull(value);

        if (sharedBluetoothDeviceDelegate is not AndroidBluetoothConnectedDevice androidDevice || androidDevice.NativeDevice == null)
        {
            return;
        }

        var broadcaster = (AndroidBluetoothBroadcaster) Service.Broadcaster;

        try
        {
            await OnWriteRequestReceivedAsync(androidDevice, value).ConfigureAwait(false);

            if (responseNeeded)
            {
                broadcaster.TrySendResponse(androidDevice.NativeDevice,
                                            requestId,
                                            GattStatus.Success,
                                            offset,
                                            []);
            }
        }
        catch (Exception)
        {
            if (responseNeeded)
            {
                broadcaster.TrySendResponse(androidDevice.NativeDevice,
                                            requestId,
                                            GattStatus.RequestNotSupported,
                                            offset,
                                            []);
            }
        }
    }

    /// <inheritdoc />
    public BluetoothGattServerCallbackProxy.IBluetoothGattDescriptorDelegate GetDescriptor(BluetoothGattDescriptor? native)
    {
        ArgumentNullException.ThrowIfNull(native);
        ArgumentNullException.ThrowIfNull(native.Uuid);

        var descriptorId = native.Uuid.ToGuid();
        var descriptor = GetDescriptorOrDefault(descriptorId);
        if (descriptor == null)
        {
            throw new DescriptorNotFoundException(this, descriptorId);
        }

        if (descriptor is not BluetoothGattServerCallbackProxy.IBluetoothGattDescriptorDelegate descriptorDelegate)
        {
            throw new InvalidOperationException("Descriptor is not Android GATT descriptor delegate.");
        }

        return descriptorDelegate;
    }

    /// <inheritdoc />
    protected override ValueTask NativeUpdateValueAsync(ReadOnlyMemory<byte> value, bool notifyClients, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (!notifyClients)
        {
            return ValueTask.CompletedTask;
        }

        var broadcaster = (AndroidBluetoothBroadcaster) Service.Broadcaster;
        var requiresConfirmation = Properties.HasFlag(BluetoothCharacteristicProperties.Indicate);
        var payload = value.ToArray();
        foreach (var subscribedDevice in SubscribedDevices.OfType<AndroidBluetoothConnectedDevice>())
        {
            if (subscribedDevice.NativeDevice == null)
            {
                continue;
            }

            broadcaster.TryNotifyCharacteristicChanged(subscribedDevice.NativeDevice, NativeCharacteristic, requiresConfirmation, payload);
        }

        return ValueTask.CompletedTask;
    }
}
