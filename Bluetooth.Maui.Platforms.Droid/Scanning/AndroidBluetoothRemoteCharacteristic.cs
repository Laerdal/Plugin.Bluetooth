using Bluetooth.Abstractions.Options;
using Bluetooth.Core.Infrastructure.Retries;
using Bluetooth.Maui.Platforms.Droid.Enums;
using Bluetooth.Maui.Platforms.Droid.Exceptions;
using Bluetooth.Maui.Platforms.Droid.Scanning.Factories;
using Bluetooth.Maui.Platforms.Droid.Scanning.NativeObjects;
using Bluetooth.Maui.Platforms.Droid.Tools;

namespace Bluetooth.Maui.Platforms.Droid.Scanning;

/// <summary>
///     Android implementation of a Bluetooth Low Energy remote characteristic.
///     This class wraps Android's BluetoothGattCharacteristic, providing platform-specific
///     implementations for read, write, notify, and descriptor operations.
/// </summary>
public class AndroidBluetoothRemoteCharacteristic : BaseBluetoothRemoteCharacteristic, BluetoothGattProxy.IBluetoothGattCharacteristicDelegate
{
    /// <summary>
    ///     The CCCD (Client Characteristic Configuration Descriptor) UUID used for enabling/disabling notifications.
    /// </summary>
    private readonly static Guid _cccdUuid = Guid.Parse("00002902-0000-1000-8000-00805f9b34fb");

    /// <summary>
    ///     Initializes a new instance of the <see cref="AndroidBluetoothRemoteCharacteristic" /> class.
    /// </summary>
    /// <param name="remoteService">The Bluetooth service to which this characteristic belongs.</param>
    /// <param name="request">The factory request containing characteristic information.</param>
    /// <param name="descriptorFactory">The factory for creating descriptors.</param>
    public AndroidBluetoothRemoteCharacteristic(IBluetoothRemoteService remoteService, IBluetoothCharacteristicFactory.BluetoothCharacteristicFactoryRequest request, IBluetoothDescriptorFactory descriptorFactory) :
        base(remoteService, request, descriptorFactory)
    {
        ArgumentNullException.ThrowIfNull(request);
        if (request is not AndroidBluetoothCharacteristicFactoryRequest androidRequest)
        {
            throw new ArgumentException($"Expected request of type {typeof(AndroidBluetoothCharacteristicFactoryRequest)}, but got {request.GetType()}");
        }

        NativeCharacteristic = androidRequest.NativeCharacteristic;
    }

    /// <summary>
    ///     Gets the native Android GATT characteristic.
    /// </summary>
    public BluetoothGattCharacteristic NativeCharacteristic { get; }

    /// <summary>
    ///     Gets the Bluetooth service to which this characteristic belongs, cast to the Android-specific implementation.
    /// </summary>
    public AndroidBluetoothRemoteService AndroidBluetoothRemoteService =>
        (AndroidBluetoothRemoteService) RemoteService;

    /// <summary>
    ///     Gets the GATT proxy from the device.
    /// </summary>
    private BluetoothGattProxy BluetoothGattProxy =>
        AndroidBluetoothRemoteService.AndroidBluetoothRemoteDevice.BluetoothGattProxy ?? throw new InvalidOperationException("Device not connected - GATT proxy is null");

    #region Read

    /// <inheritdoc />
    /// <seealso href="https://developer.android.com/reference/android/bluetooth/BluetoothGatt#readCharacteristic(android.bluetooth.BluetoothGattCharacteristic)">Android BluetoothGatt.readCharacteristic()</seealso>
    protected override ValueTask NativeReadValueAsync()
    {
        var success = BluetoothGattProxy.BluetoothGatt.ReadCharacteristic(NativeCharacteristic);
        if (!success)
        {
            throw new InvalidOperationException("Failed to initiate characteristic read");
        }

        return ValueTask.CompletedTask;
    }

    /// <inheritdoc />
    protected override bool NativeCanRead()
    {
        return NativeCharacteristic.Properties.HasFlag(GattProperty.Read);
    }

    #endregion

    #region Write

    /// <inheritdoc />
    /// <seealso href="https://developer.android.com/reference/android/bluetooth/BluetoothGatt#writeCharacteristic(android.bluetooth.BluetoothGattCharacteristic)">Android BluetoothGatt.writeCharacteristic()</seealso>
    protected async override ValueTask NativeWriteValueAsync(ReadOnlyMemory<byte> value)
    {
        // Get retry options from device connection options, or use default
        var retryOptions = AndroidBluetoothRemoteService.AndroidBluetoothRemoteDevice.ConnectionOptions?.Android?.GattWriteRetry
                           ?? RetryOptions.Default;

        // Call with configurable retry
        await RetryTools.RunWithRetriesAsync(
            () => BluetoothGattCharacteristicWrite(value),
            retryOptions,
            CancellationToken.None
        ).ConfigureAwait(false);
    }

    private void BluetoothGattCharacteristicWrite(ReadOnlyMemory<byte> value)
    {
        // Ensure BluetoothGatt exists and is available
        ArgumentNullException.ThrowIfNull(BluetoothGattProxy);

        // Ensure WRITE is supported
        CharacteristicCantWriteException.ThrowIfCantWrite(this);

        // Get WriteType
        NativeCharacteristic.WriteType = GetBluetoothGattCharacteristicWriteType();

        if (OperatingSystem.IsAndroidVersionAtLeast(33))
        {
            // Write the value
            var writeResult = (Android.Bluetooth.CurrentBluetoothStatusCodes) BluetoothGattProxy.BluetoothGatt.WriteCharacteristic(NativeCharacteristic, value.ToArray(), (int) GetBluetoothGattCharacteristicWriteType());

            AndroidNativeCurrentBluetoothStatusCodesException.ThrowIfNotSuccess(writeResult);
        }
        else
        {
            // Write the value
            if (!NativeCharacteristic.SetValue(value.ToArray()))
            {
                throw new CharacteristicWriteException(this, value, $"BluetoothGattCharacteristic.SetValue() Failed");
            }

            // Write the characteristic
            if (!BluetoothGattProxy.BluetoothGatt.WriteCharacteristic(NativeCharacteristic))
            {
                throw new CharacteristicWriteException(this, value, "BluetoothGatt.WriteCharacteristic() Failed");
            }
        }
    }

    private GattWriteType GetBluetoothGattCharacteristicWriteType()
    {
        if (NativeCharacteristic.Properties.HasFlag(GattProperty.WriteNoResponse))
        {
            return GattWriteType.NoResponse;
        }

        if (NativeCharacteristic.Properties.HasFlag(GattProperty.SignedWrite))
        {
            return GattWriteType.Signed;
        }

        if (NativeCharacteristic.Properties.HasFlag(GattProperty.Write))
        {
            return GattWriteType.Default;
        }

        throw new UnreachableException("This case should be caught by CharacteristicCantWriteException.ThrowIfCantWrite");
    }

    /// <inheritdoc />
    protected override bool NativeCanWrite()
    {
        return NativeCharacteristic.Properties.HasFlag(GattProperty.Write) || NativeCharacteristic.Properties.HasFlag(GattProperty.WriteNoResponse) || NativeCharacteristic.Properties.HasFlag(GattProperty.SignedWrite);
    }

    /// <summary>
    ///     Gets the Android-specific write capability string representation for the characteristic.
    /// </summary>
    /// <returns>
    ///     Returns "WNR" for write without response, "WS" for signed writes, "W" for standard write,
    ///     or an empty string if no write operations are supported.
    /// </returns>
    protected override string ToWriteString()
    {
        if (NativeCharacteristic.Properties.HasFlag(GattProperty.WriteNoResponse))
        {
            return "WNR";
        }

        if (NativeCharacteristic.Properties.HasFlag(GattProperty.SignedWrite))
        {
            return "WS";
        }

        if (NativeCharacteristic.Properties.HasFlag(GattProperty.Write))
        {
            return "W";
        }

        return string.Empty;
    }

    #endregion

    #region Listen (Notifications/Indications)

    /// <inheritdoc />
    protected override bool NativeCanListen()
    {
        return NativeCharacteristic.Properties.HasFlag(GattProperty.Notify) || NativeCharacteristic.Properties.HasFlag(GattProperty.Indicate);
    }

    /// <inheritdoc />
    protected async override ValueTask NativeReadIsListeningAsync()
    {
        // On Android, we need to check the CCCD descriptor to determine if notifications are enabled
        var descriptor = GetDescriptor(_cccdUuid);
        if (descriptor == null)
        {
            // No CCCD descriptor found, assume not listening
            OnReadIsListeningSucceeded(false);
            return;
        }

        var cccdValue = await descriptor.ReadValueAsync().ConfigureAwait(false);
        var cccdValueArray = cccdValue.ToArray();
        var isListening = cccdValueArray.SequenceEqual(BluetoothGattDescriptor.EnableNotificationValue?.ToArray() ?? []) || cccdValueArray.SequenceEqual(BluetoothGattDescriptor.EnableIndicationValue?.ToArray() ?? []);
        OnReadIsListeningSucceeded(isListening);
    }

    /// <inheritdoc />
    /// <seealso href="https://developer.android.com/reference/android/bluetooth/BluetoothGatt#setCharacteristicNotification(android.bluetooth.BluetoothGattCharacteristic,%20boolean)">Android BluetoothGatt.setCharacteristicNotification()</seealso>
    protected async override ValueTask NativeWriteIsListeningAsync(bool shouldBeListening)
    {
        // First, enable/disable notifications locally on the GATT object
        var success = BluetoothGattProxy.BluetoothGatt.SetCharacteristicNotification(NativeCharacteristic, shouldBeListening);

        if (!success)
        {
            throw new InvalidOperationException($"Failed to {(shouldBeListening ? "enable" : "disable")} characteristic notification");
        }

        var descriptor = GetDescriptor(_cccdUuid);
        if (descriptor == null)
        {
            throw new InvalidOperationException("CCCD descriptor not found for this characteristic");
        }

        byte[] cccdValue;
        if (!shouldBeListening)
        {
            // Disable notifications
            cccdValue = BluetoothGattDescriptor.DisableNotificationValue?.ToArray() ?? [];
        }
        else if (NativeCharacteristic.Properties.HasFlag(GattProperty.Indicate))
        {
            // Enable indications
            cccdValue = BluetoothGattDescriptor.EnableIndicationValue?.ToArray() ?? [];
        }
        else if (NativeCharacteristic.Properties.HasFlag(GattProperty.Notify))
        {
            // Enable notifications
            cccdValue = BluetoothGattDescriptor.EnableNotificationValue?.ToArray() ?? [];
        }
        else
        {
            throw new InvalidOperationException("Characteristic does not support Notify or Indicate, cannot enable notifications");
        }

        await descriptor.WriteValueAsync(cccdValue).ConfigureAwait(false);
    }

    #endregion

    #region Reliable Write

    /// <inheritdoc />
    protected override ValueTask NativeBeginReliableWriteAsync()
    {
        var success = BluetoothGattProxy.BluetoothGatt.BeginReliableWrite();
        if (!success)
        {
            throw new InvalidOperationException("Failed to begin reliable write");
        }

        return ValueTask.CompletedTask;
    }

    /// <inheritdoc />
    protected override ValueTask NativeExecuteReliableWriteAsync()
    {
        var success = BluetoothGattProxy.BluetoothGatt.ExecuteReliableWrite();
        if (!success)
        {
            throw new InvalidOperationException("Failed to execute reliable write");
        }

        return ValueTask.CompletedTask;
    }

    /// <inheritdoc />
    protected override ValueTask NativeAbortReliableWriteAsync()
    {
        BluetoothGattProxy.BluetoothGatt.AbortReliableWrite();
        return ValueTask.CompletedTask;
    }

    #endregion

    #region Descriptors

    /// <inheritdoc />
    protected override ValueTask NativeDescriptorsExplorationAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        try
        {
            // On Android, descriptors are discovered along with characteristics
            // The NativeCharacteristic.Descriptors property should already contain all descriptors
            var descriptors = NativeCharacteristic.Descriptors;
            if (descriptors == null || descriptors.Count == 0)
            {
                // No descriptors for this characteristic
                OnDescriptorsExplorationSucceeded(System.Array.Empty<BluetoothGattDescriptor>(), ConvertNativeDescriptorToDescriptor, AreRepresentingTheSameObject);
            }
            else
            {
                OnDescriptorsExplorationSucceeded(descriptors, ConvertNativeDescriptorToDescriptor, AreRepresentingTheSameObject);
            }
        }
        catch (Exception ex)
        {
            OnDescriptorsExplorationFailed(ex);
        }

        return ValueTask.CompletedTask;
    }

    private IBluetoothRemoteDescriptor ConvertNativeDescriptorToDescriptor(BluetoothGattDescriptor nativeDescriptor)
    {
        var descriptorRequest = new AndroidBluetoothDescriptorFactoryRequest(nativeDescriptor);
        return DescriptorFactory.CreateDescriptor(this, descriptorRequest);
    }

    private static bool AreRepresentingTheSameObject(BluetoothGattDescriptor native, IBluetoothRemoteDescriptor shared)
    {
        return shared is AndroidBluetoothRemoteDescriptor androidDescriptor && native.Uuid?.Equals(androidDescriptor.NativeDescriptor.Uuid) == true;
    }

    #endregion

    #region BluetoothGattProxy.IBluetoothGattCharacteristicDelegate Implementation

    /// <inheritdoc />
    public void OnCharacteristicChanged(BluetoothGattCharacteristic? nativeCharacteristic, byte[]? value)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(nativeCharacteristic);

            // Verify this is the correct characteristic
            if (!NativeCharacteristic.Uuid?.Equals(nativeCharacteristic.Uuid) ?? true)
            {
                return; // Not for this characteristic
            }

            OnReadValueSucceeded(value ?? []);
        }
        catch (Exception e)
        {
            BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, e);
        }
    }

    /// <inheritdoc />
    public void OnCharacteristicWrite(GattStatus status, BluetoothGattCharacteristic? nativeCharacteristic)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(nativeCharacteristic);

            // Verify this is the correct characteristic
            if (!NativeCharacteristic.Uuid?.Equals(nativeCharacteristic.Uuid) ?? true)
            {
                return; // Not for this characteristic
            }

            if (status != GattStatus.Success)
            {
                OnWriteValueFailed(new AndroidNativeGattCallbackStatusException((GattCallbackStatus) status));
                return;
            }

            OnWriteValueSucceeded();
        }
        catch (Exception e)
        {
            OnWriteValueFailed(e);
        }
    }

    /// <inheritdoc />
    public void OnCharacteristicRead(GattStatus status, BluetoothGattCharacteristic? nativeCharacteristic, byte[]? value)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(nativeCharacteristic);

            // Verify this is the correct characteristic
            if (!NativeCharacteristic.Uuid?.Equals(nativeCharacteristic.Uuid) ?? true)
            {
                return; // Not for this characteristic
            }

            if (status != GattStatus.Success)
            {
                OnReadValueFailed(new AndroidNativeGattCallbackStatusException((GattCallbackStatus) status));
                return;
            }

            OnReadValueSucceeded(value ?? []);
        }
        catch (Exception e)
        {
            OnReadValueFailed(e);
        }
    }

    /// <inheritdoc />
    public void OnDescriptorRead(GattStatus status, BluetoothGattDescriptor? nativeDescriptor, byte[]? value)
    {
        // Forward to the appropriate descriptor
        if (nativeDescriptor == null)
        {
            return;
        }

        try
        {
            var descriptor = GetDescriptorOrDefault(d => d is AndroidBluetoothRemoteDescriptor androidDesc && androidDesc.NativeDescriptor.Uuid?.Equals(nativeDescriptor.Uuid) == true);

            if (descriptor is AndroidBluetoothRemoteDescriptor androidDescriptor)
            {
                androidDescriptor.NotifyDescriptorRead(status, value);
            }
        }
        catch (Exception e)
        {
            BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, e);
        }
    }

    /// <inheritdoc />
    public void OnDescriptorWrite(GattStatus status, BluetoothGattDescriptor? nativeDescriptor)
    {
        // Check if this is the CCCD descriptor for notifications
        if (nativeDescriptor?.Uuid?.ToGuid().Equals(_cccdUuid) == true)
        {
            // This is a CCCD write for enabling/disabling notifications
            if (status != GattStatus.Success)
            {
                OnWriteIsListeningFailed(new AndroidNativeGattCallbackStatusException((GattCallbackStatus) status));
                return;
            }

            OnWriteIsListeningSucceeded();
            return;
        }

        // Forward to the appropriate descriptor
        if (nativeDescriptor == null)
        {
            return;
        }

        try
        {
            var descriptor = GetDescriptorOrDefault(d => d is AndroidBluetoothRemoteDescriptor androidDesc && androidDesc.NativeDescriptor.Uuid?.Equals(nativeDescriptor.Uuid) == true);

            if (descriptor is AndroidBluetoothRemoteDescriptor androidDescriptor)
            {
                androidDescriptor.NotifyDescriptorWrite(status);
            }
        }
        catch (Exception e)
        {
            BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, e);
        }
    }

    #endregion

}
