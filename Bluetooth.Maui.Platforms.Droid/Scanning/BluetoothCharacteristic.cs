using Bluetooth.Maui.Platforms.Droid.Exceptions;
using Bluetooth.Maui.Platforms.Droid.Scanning.NativeObjects;

using Microsoft.Extensions.Logging;

namespace Bluetooth.Maui.Platforms.Droid.Scanning;

/// <inheritdoc/>
public partial class BluetoothCharacteristic : BaseBluetoothCharacteristic, BluetoothGattProxy.ICharacteristic
{
    /// <summary>
    /// Gets the native Android Bluetooth GATT characteristic.
    /// </summary>
    public BluetoothGattCharacteristic NativeCharacteristic { get; }

    /// <summary>
    /// Gets the Bluetooth GATT instance for this characteristic.
    /// </summary>
    private BluetoothGatt BluetoothGatt
    {
        get
        {
            if (Service.Device is not BluetoothDevice androidDevice)
            {
                throw new InvalidOperationException("Device must be an Android BluetoothDevice");
            }
            return androidDevice.BluetoothGatt ?? throw new InvalidOperationException("BluetoothGatt is not available");
        }
    }

    /// <inheritdoc/>
    public BluetoothCharacteristic(IBluetoothService service, IBluetoothCharacteristicFactory.BluetoothCharacteristicFactoryRequest request) : base(service, request)
    {
        if (request is not BluetoothCharacteristicFactoryRequest androidRequest)
        {
            throw new ArgumentException($"Request must be of type {nameof(BluetoothCharacteristicFactoryRequest)}", nameof(request));
        }

        NativeCharacteristic = androidRequest.NativeCharacteristic ?? throw new ArgumentNullException(nameof(androidRequest.NativeCharacteristic));
    }

    /// <inheritdoc/>
    protected override bool NativeCanListen()
    {
        var properties = NativeCharacteristic.Properties;
        return properties.HasFlag(GattProperty.Notify) || properties.HasFlag(GattProperty.Indicate);
    }

    /// <inheritdoc/>
    protected override ValueTask NativeReadIsListeningAsync()
    {
        // On Android, we track this state ourselves
        // There's no direct way to query if notifications are enabled
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc/>
    protected override ValueTask NativeWriteIsListeningAsync(bool shouldBeListening)
    {
        try
        {
            var result = BluetoothGatt.SetCharacteristicNotification(NativeCharacteristic, shouldBeListening);
            if (!result)
            {
                throw new InvalidOperationException("Failed to set characteristic notification");
            }

            // Write to the Client Characteristic Configuration Descriptor (CCCD)
            var descriptor = NativeCharacteristic.GetDescriptor(Java.Util.UUID.FromString("00002902-0000-1000-8000-00805f9b34fb"));
            if (descriptor != null)
            {
                var value = shouldBeListening
                    ? (NativeCharacteristic.Properties.HasFlag(GattProperty.Indicate)
                        ? BluetoothGattDescriptor.EnableIndicationValue
                        : BluetoothGattDescriptor.EnableNotificationValue)
                    : BluetoothGattDescriptor.DisableNotificationValue;

                descriptor.SetValue(value?.ToArray());
                var writeResult = BluetoothGatt.WriteDescriptor(descriptor);
                if (!writeResult)
                {
                    throw new InvalidOperationException("Failed to write CCCD descriptor");
                }
            }
        }
        catch (Exception e)
        {
            Logger?.LogError(e, "Error setting listen state for characteristic {CharacteristicId}", Id);
            throw;
        }

        return ValueTask.CompletedTask;
    }

    /// <inheritdoc/>
    protected override ValueTask NativeWriteValueAsync(ReadOnlyMemory<byte> value)
    {
        try
        {
            NativeCharacteristic.SetValue(value.ToArray());
            var result = BluetoothGatt.WriteCharacteristic(NativeCharacteristic);
            if (!result)
            {
                throw new InvalidOperationException("Failed to write characteristic value");
            }
        }
        catch (Exception e)
        {
            Logger?.LogError(e, "Error writing value to characteristic {CharacteristicId}", Id);
            throw;
        }

        return ValueTask.CompletedTask;
    }

    /// <inheritdoc/>
    protected override bool NativeCanWrite()
    {
        var properties = NativeCharacteristic.Properties;
        return properties.HasFlag(GattProperty.Write) ||
               properties.HasFlag(GattProperty.WriteNoResponse) ||
               properties.HasFlag(GattProperty.SignedWrite);
    }

    /// <inheritdoc/>
    protected override ValueTask NativeReadValueAsync()
    {
        try
        {
            var result = BluetoothGatt.ReadCharacteristic(NativeCharacteristic);
            if (!result)
            {
                throw new InvalidOperationException("Failed to read characteristic value");
            }
        }
        catch (Exception e)
        {
            Logger?.LogError(e, "Error reading value from characteristic {CharacteristicId}", Id);
            throw;
        }

        return ValueTask.CompletedTask;
    }

    /// <inheritdoc/>
    protected override bool NativeCanRead()
    {
        return NativeCharacteristic.Properties.HasFlag(GattProperty.Read);
    }

    #region BluetoothGattProxy.ICharacteristic Implementation

    /// <inheritdoc/>
    public void OnCharacteristicRead(GattStatus status, BluetoothGattCharacteristic? nativeCharacteristic, byte[]? value)
    {
        try
        {
            AndroidNativeGattCallbackStatusRequestResponseException.ThrowIfError(status);
            if (value != null)
            {
                OnReadCompleted(value);
            }
        }
        catch (Exception e)
        {
            OnReadFailed(e);
        }
    }

    /// <inheritdoc/>
    public void OnCharacteristicWrite(GattStatus status, BluetoothGattCharacteristic? nativeCharacteristic)
    {
        try
        {
            AndroidNativeGattCallbackStatusRequestResponseException.ThrowIfError(status);
            OnWriteCompleted();
        }
        catch (Exception e)
        {
            OnWriteFailed(e);
        }
    }

    /// <inheritdoc/>
    public void OnCharacteristicChanged(BluetoothGattCharacteristic? nativeCharacteristic, byte[]? value)
    {
        try
        {
            if (value != null)
            {
                OnValueChanged(value);
            }
        }
        catch (Exception e)
        {
            Logger?.LogError(e, "Error handling characteristic value change for {CharacteristicId}", Id);
        }
    }

    /// <inheritdoc/>
    public void OnDescriptorRead(GattStatus status, BluetoothGattDescriptor? nativeDescriptor, byte[]? value)
    {
        try
        {
            AndroidNativeGattCallbackStatusRequestResponseException.ThrowIfError(status);
            // Descriptor read completed - this is used for reading CCCD and other descriptors
            Logger?.LogDebug("Descriptor read completed for characteristic {CharacteristicId}", Id);
        }
        catch (Exception e)
        {
            Logger?.LogError(e, "Error reading descriptor for characteristic {CharacteristicId}", Id);
        }
    }

    /// <inheritdoc/>
    public void OnDescriptorWrite(GattStatus status, BluetoothGattDescriptor? nativeDescriptor)
    {
        try
        {
            AndroidNativeGattCallbackStatusRequestResponseException.ThrowIfError(status);
            // Descriptor write completed - this is used for enabling/disabling notifications via CCCD
            OnListeningWriteCompleted();
        }
        catch (Exception e)
        {
            OnListeningWriteFailed(e);
        }
    }

    #endregion
}
