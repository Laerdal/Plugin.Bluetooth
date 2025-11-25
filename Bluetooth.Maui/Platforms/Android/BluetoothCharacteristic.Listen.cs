using System.Diagnostics;

using Bluetooth.Maui.Helpers;
using Bluetooth.Maui.PlatformSpecific.Exceptions;
using Plugin.ByteArrays;

namespace Bluetooth.Maui;

public partial class BluetoothCharacteristic
{
    /// <inheritdoc/>
    protected override bool NativeCanListen()
    {
        return NativeCharacteristic.Properties.HasFlag(GattProperty.Indicate) || NativeCharacteristic.Properties.HasFlag(GattProperty.Notify);
    }

    /// <inheritdoc/>
    protected override ValueTask NativeReadIsListeningAsync()
    {
        // Ensure BluetoothGatt exists and is available
        ArgumentNullException.ThrowIfNull(BluetoothGattProxy);

        // Get the descriptor
        if (NativeCharacteristic.GetDescriptor(UUID.FromString(NotificationDescriptorId)) is not { } descriptor)
        {
            throw new CharacteristicNotifyException(this, "BluetoothGattCharacteristic.GetDescriptor() Failed");
        }

        // descriptor.GetValue() is Deprecated in Android 33
        // New way : BluetoothGatt.ReadDescriptor(descriptor)
        if (!BluetoothGattProxy.BluetoothGatt.ReadDescriptor(descriptor))
        {
            throw new CharacteristicNotifyException(this, "BluetoothGatt.ReadDescriptor() Failed");
        }

        return ValueTask.CompletedTask;
    }

    /// <inheritdoc/>
    protected async override ValueTask NativeWriteIsListeningAsync(bool shouldBeListening)
    {
        // Ensure LISTEN is supported
        CharacteristicCantListenException.ThrowIfCantListen(this);

        // Ensure BluetoothGatt exists and is available
        if (!BluetoothGattProxy.BluetoothGatt.SetCharacteristicNotification(NativeCharacteristic, enable: true))
        {
            throw new CharacteristicNotifyException(this, "BluetoothGatt.SetCharacteristicNotification() Failed");
        }

        // Get the descriptor
        if (NativeCharacteristic.GetDescriptor(UUID.FromString(NotificationDescriptorId)) is not { } descriptor)
        {
            throw new CharacteristicNotifyException(this, "BluetoothGattCharacteristic.GetDescriptor() Failed");
        }

        // Get which bytes to write
        byte[] enableNotificationBytes;
        if (!shouldBeListening)
        {
            ArgumentNullException.ThrowIfNull(BluetoothGattDescriptor.DisableNotificationValue, nameof(BluetoothGattDescriptor.DisableNotificationValue));
            enableNotificationBytes = BluetoothGattDescriptor.DisableNotificationValue.ToArray();
        }
        else if (NativeCharacteristic.Properties.HasFlag(GattProperty.Notify))
        {
            ArgumentNullException.ThrowIfNull(BluetoothGattDescriptor.EnableNotificationValue, nameof(BluetoothGattDescriptor.EnableNotificationValue));
            enableNotificationBytes = BluetoothGattDescriptor.EnableNotificationValue.ToArray();
        }
        else if (NativeCharacteristic.Properties.HasFlag(GattProperty.Indicate))
        {
            ArgumentNullException.ThrowIfNull(BluetoothGattDescriptor.EnableIndicationValue, nameof(BluetoothGattDescriptor.EnableIndicationValue));
            enableNotificationBytes = BluetoothGattDescriptor.EnableIndicationValue.ToArray();
        }
        else
        {
            throw new UnreachableException("This case should be caught by CharacteristicCantListenException.ThrowIfCantListen");
        }

        await RetryTools.RunWithRetriesAsync(() => BluetoothGattCharacteristicWriteDescriptor(descriptor, enableNotificationBytes), maxRetries: 3, delayBetweenRetries: TimeSpan.FromMilliseconds(200)).ConfigureAwait(false);
    }


    private void BluetoothGattCharacteristicWriteDescriptor(BluetoothGattDescriptor descriptor, byte[] value)
    {
        // Ensure BluetoothGatt exists and is available
        ArgumentNullException.ThrowIfNull(BluetoothGattProxy);

        // Ensure NOTIFY ot INDICATE is supported
        CharacteristicCantListenException.ThrowIfCantListen(this);

        if (OperatingSystem.IsAndroidVersionAtLeast(33))
        {
            // Write the value
            var writeResult = (Android.Bluetooth.CurrentBluetoothStatusCodes) BluetoothGattProxy.BluetoothGatt.WriteDescriptor(descriptor, value);

            AndroidNativeCurrentBluetoothStatusCodesException.ThrowIfNotSuccess(writeResult);
        }
        else
        {
            // Write the value
            if (!descriptor.SetValue(value))
            {
                throw new CharacteristicNotifyException(this, $"descriptor.SetValue() failed writing {value.ToDebugString()} bytes");
            }

            // Write the characteristic
            if (!BluetoothGattProxy.BluetoothGatt.WriteDescriptor(descriptor))
            {
                throw new CharacteristicNotifyException(this, "BluetoothGatt.WriteDescriptor() Failed");
            }
        }
    }


    /// <summary>
    /// Called when a GATT descriptor read operation completes on the Android platform.
    /// This method handles the response from reading the client characteristic configuration descriptor.
    /// </summary>
    /// <param name="status">The status of the GATT operation.</param>
    /// <param name="nativeDescriptor">The descriptor that was read.</param>
    /// <param name="value">The value read from the descriptor.</param>
    /// <exception cref="AndroidNativeGattCallbackStatusException">Thrown when the GATT status indicates an error.</exception>
    public void OnDescriptorRead(GattStatus status, BluetoothGattDescriptor? nativeDescriptor, byte[]? value)
    {
        try
        {
            AndroidNativeGattCallbackStatusException.ThrowIfNotSuccess(status);
            ArgumentNullException.ThrowIfNull(nativeDescriptor);
            ArgumentNullException.ThrowIfNull(value);

            // Should not happen ... but just in case, check we're dealing with the correct descriptor
            if (nativeDescriptor.Uuid?.ToString() != NotificationDescriptorId)
            {
                return;
            }

            OnReadIsListeningSucceeded(value is [1, 0]);

        }
        catch (Exception e)
        {
            OnReadIsListeningFailed(e);
        }
    }

    /// <summary>
    /// Called when a GATT descriptor write operation completes on the Android platform.
    /// This method handles the response from writing to the client characteristic configuration descriptor.
    /// </summary>
    /// <param name="status">The status of the GATT operation.</param>
    /// <param name="nativeDescriptor">The descriptor that was written to.</param>
    /// <exception cref="AndroidNativeGattCallbackStatusException">Thrown when the GATT status indicates an error.</exception>
    public virtual void OnDescriptorWrite(GattStatus status, BluetoothGattDescriptor? nativeDescriptor)
    {
        try
        {
            AndroidNativeGattCallbackStatusException.ThrowIfNotSuccess(status);
            ArgumentNullException.ThrowIfNull(nativeDescriptor);

            // Should not happen ... but just in case, check we're dealing with the correct descriptor
            if (nativeDescriptor.Uuid?.ToString() != NotificationDescriptorId)
            {
                return;
            }

            OnWriteIsListeningSucceeded();
        }
        catch (Exception e)
        {
            OnWriteIsListeningFailed(e);
        }
    }

    /// <summary>
    /// Gets the Android-specific notification capability string representation for the characteristic.
    /// </summary>
    /// <returns>
    /// Returns :<br/>
    /// - "N*" if notifications are enabled and listening,<br/>
    /// - "N" if notifications are supported but not listening,<br/>
    /// - "I*" if indications are enabled and listening,<br/>
    /// - "I" if indications are supported but not listening.<br/>
    /// Otherwise, an empty string if neither notifications nor indications are supported.
    /// </returns>
    protected override string ToListenString()
    {
        if (NativeCharacteristic.Properties.HasFlag(GattProperty.Notify))
        {
            return IsListening ? "N*" : "N";
        }
        if (NativeCharacteristic.Properties.HasFlag(GattProperty.Indicate))
        {
            return IsListening ? "I*" : "I";
        }
        return string.Empty;
    }
}
