using Bluetooth.Abstractions.Options;
using Bluetooth.Core.Infrastructure.Retries;
using Bluetooth.Maui.Platforms.Droid.Enums;
using Bluetooth.Maui.Platforms.Droid.Exceptions;
using Bluetooth.Maui.Platforms.Droid.Scanning.Factories;

namespace Bluetooth.Maui.Platforms.Droid.Scanning;

/// <summary>
///     Android implementation of a Bluetooth Low Energy descriptor.
///     This class wraps Android's BluetoothGattDescriptor, providing platform-specific
///     implementations for reading and writing descriptor values.
/// </summary>
public class AndroidBluetoothRemoteDescriptor : BaseBluetoothRemoteDescriptor
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="AndroidBluetoothRemoteDescriptor" /> class.
    /// </summary>
    /// <param name="characteristic">The Bluetooth characteristic that contains this descriptor.</param>
    /// <param name="spec">The descriptor factory spec containing descriptor information.</param>
    public AndroidBluetoothRemoteDescriptor(
        IBluetoothRemoteCharacteristic characteristic,
        IBluetoothRemoteDescriptorFactory.BluetoothRemoteDescriptorFactorySpec spec)
        : base(characteristic, spec)
    {
        ArgumentNullException.ThrowIfNull(spec);
        if (spec is not AndroidBluetoothRemoteDescriptorFactorySpec nativeSpec)
        {
            throw new ArgumentException(
                $"Expected spec of type {typeof(AndroidBluetoothRemoteDescriptorFactorySpec)}, but got {spec.GetType()}");
        }

        NativeDescriptor = nativeSpec.NativeDescriptor;
    }

    /// <summary>
    ///     Gets the native Android GATT descriptor.
    /// </summary>
    public BluetoothGattDescriptor NativeDescriptor { get; }

    /// <summary>
    ///     Gets the Bluetooth characteristic to which this descriptor belongs, cast to the Android-specific implementation.
    /// </summary>
    public AndroidBluetoothRemoteCharacteristic AndroidBluetoothRemoteCharacteristic =>
        (AndroidBluetoothRemoteCharacteristic) RemoteCharacteristic;

    /// <summary>
    ///     Gets the GATT proxy from the device.
    /// </summary>
    private NativeObjects.BluetoothGattProxy BluetoothGattProxy =>
        AndroidBluetoothRemoteCharacteristic.AndroidBluetoothRemoteService.AndroidBluetoothRemoteDevice.BluetoothGattProxy
        ?? throw new InvalidOperationException("Device not connected - GATT proxy is null");

    #region Read

    /// <inheritdoc />
    protected async override ValueTask NativeReadValueAsync()
    {
        // Get retry options from device connection options, or use default
        var retryOptions = AndroidBluetoothRemoteCharacteristic.AndroidBluetoothRemoteService.AndroidBluetoothRemoteDevice.ConnectionOptions?.Android?.GattReadRetry
                           ?? new RetryOptions { MaxRetries = 2, DelayBetweenRetries = TimeSpan.FromMilliseconds(100) };

        // Call with configurable retry
        await RetryTools.RunWithRetriesAsync(ReadDescriptorInternal, retryOptions, CancellationToken.None).ConfigureAwait(false);
    }

    private void ReadDescriptorInternal()
    {
        var success = BluetoothGattProxy.BluetoothGatt.ReadDescriptor(NativeDescriptor);
        if (!success)
        {
            throw new InvalidOperationException("Failed to initiate descriptor read");
        }
    }

    /// <inheritdoc />
    protected override bool NativeCanRead()
    {
        return NativeDescriptor.Permissions.HasFlag(GattDescriptorPermission.Read) ||
               NativeDescriptor.Permissions.HasFlag(GattDescriptorPermission.ReadEncrypted) ||
               NativeDescriptor.Permissions.HasFlag(GattDescriptorPermission.ReadEncryptedMitm);
    }

    #endregion

    #region Write

    /// <inheritdoc />
    protected async override ValueTask NativeWriteValueAsync(ReadOnlyMemory<byte> value)
    {
        // Get retry options from device connection options, or use default
        var retryOptions = AndroidBluetoothRemoteCharacteristic.AndroidBluetoothRemoteService.AndroidBluetoothRemoteDevice.ConnectionOptions?.Android?.GattWriteRetry ?? RetryOptions.Default;

        // Call with configurable retry
        await RetryTools.RunWithRetriesAsync(() => BluetoothGattCharacteristicWrite(value), retryOptions, CancellationToken.None).ConfigureAwait(false);
    }


    private void BluetoothGattCharacteristicWrite(ReadOnlyMemory<byte> value)
    {
        // Ensure BluetoothGatt exists and is available
        ArgumentNullException.ThrowIfNull(BluetoothGattProxy);

        // Ensure WRITE is supported
        DescriptorCantWriteException.ThrowIfCantWrite(this);

        if (OperatingSystem.IsAndroidVersionAtLeast(33))
        {
            // Write the value
            var writeResult = (Android.Bluetooth.CurrentBluetoothStatusCodes) BluetoothGattProxy.BluetoothGatt.WriteDescriptor(NativeDescriptor, value.ToArray());

            AndroidNativeCurrentBluetoothStatusCodesException.ThrowIfNotSuccess(writeResult);
        }
        else
        {
            // Write the value
            if (!NativeDescriptor.SetValue(value.ToArray()))
            {
                throw new DescriptorWriteException(this, value, $"BluetoothGattDescriptor.SetValue() Failed");
            }

            // Write the characteristic
            if (!BluetoothGattProxy.BluetoothGatt.WriteDescriptor(NativeDescriptor))
            {
                throw new DescriptorWriteException(this, value, "BluetoothGatt.WriteCharacteristic() Failed");
            }
        }
    }

    /// <inheritdoc />
    protected override bool NativeCanWrite()
    {
        return NativeDescriptor.Permissions.HasFlag(GattDescriptorPermission.Write) ||
               NativeDescriptor.Permissions.HasFlag(GattDescriptorPermission.WriteEncrypted) ||
               NativeDescriptor.Permissions.HasFlag(GattDescriptorPermission.WriteEncryptedMitm) ||
               NativeDescriptor.Permissions.HasFlag(GattDescriptorPermission.WriteSigned) ||
               NativeDescriptor.Permissions.HasFlag(GattDescriptorPermission.WriteSignedMitm);
    }

    #endregion

    #region Internal Notification Methods

    /// <summary>
    ///     Called internally by the characteristic when a descriptor read completes.
    ///     This method is called from the OnDescriptorRead callback in the characteristic.
    /// </summary>
    /// <param name="status">The GATT status of the read operation.</param>
    /// <param name="value">The value that was read from the descriptor.</param>
    internal void NotifyDescriptorRead(GattStatus status, byte[]? value)
    {
        if (status != GattStatus.Success)
        {
            OnReadValueFailed(new AndroidNativeGattCallbackStatusException((GattCallbackStatus) status));
            return;
        }

        OnReadValueSucceeded(value ?? []);
    }

    /// <summary>
    ///     Called internally by the characteristic when a descriptor write completes.
    ///     This method is called from the OnDescriptorWrite callback in the characteristic.
    /// </summary>
    /// <param name="status">The GATT status of the write operation.</param>
    internal void NotifyDescriptorWrite(GattStatus status)
    {
        if (status != GattStatus.Success)
        {
            OnWriteValueFailed(new AndroidNativeGattCallbackStatusException((GattCallbackStatus) status));
            return;
        }

        OnWriteValueSucceeded();
    }

    #endregion
}
