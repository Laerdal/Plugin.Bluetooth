using Bluetooth.Maui.Platforms.Apple.PlatformSpecific;
using Bluetooth.Maui.Platforms.Apple.PlatformSpecific.Exceptions;
using Bluetooth.Maui.Platforms.Apple.Scanning.NativeObjects;

namespace Bluetooth.Maui.Platforms.Apple.Scanning;

/// <summary>
/// Represents an iOS-specific Bluetooth Low Energy GATT characteristic.
/// This class wraps iOS Core Bluetooth's CBCharacteristic and provides platform-specific
/// implementations for reading, writing, and listening to characteristic values.
/// </summary>
public partial class BluetoothCharacteristic : BaseBluetoothCharacteristic, CbPeripheralWrapper.ICbCharacteristicDelegate
{
    /// <summary>
    /// Gets the native iOS Core Bluetooth characteristic used for iOS Bluetooth operations.
    /// </summary>
    public CBCharacteristic NativeCharacteristic { get; }

    /// <inheritdoc/>
    public BluetoothCharacteristic(IBluetoothService service, IBluetoothCharacteristicFactory.BluetoothCharacteristicFactoryRequest request) : base(service, request)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (request is not BluetoothCharacteristicFactoryRequest nativeRequest)
        {
            throw new ArgumentException($"Request must be of type {nameof(BluetoothCharacteristicFactoryRequest)}", nameof(request));
        }

        ArgumentNullException.ThrowIfNull(nativeRequest.NativeCharacteristic);
        NativeCharacteristic = nativeRequest.NativeCharacteristic;
    }

    #region Listen

    /// <inheritdoc/>
    /// <remarks>
    /// On iOS, checks if the characteristic has the Notify or Indicate property flag set.
    /// </remarks>
    protected override bool NativeCanListen()
    {
        return NativeCharacteristic.Properties.HasFlag(CBCharacteristicProperties.Indicate) || NativeCharacteristic.Properties.HasFlag(CBCharacteristicProperties.Notify);
    }

    /// <inheritdoc/>
    /// <remarks>
    /// On iOS, reads the current notification state from the characteristic.
    /// </remarks>
    protected override ValueTask NativeReadIsListeningAsync()
    {
        ArgumentNullException.ThrowIfNull(NativeCharacteristic);
        OnReadIsListeningSucceeded(NativeCharacteristic.IsNotifying);
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc/>
    /// <remarks>
    /// On iOS, calls SetNotifyValue on the peripheral to enable or disable notifications/indications.
    /// </remarks>
    protected override ValueTask NativeWriteIsListeningAsync(bool shouldBeListening)
    {
        ArgumentNullException.ThrowIfNull(NativeCharacteristic);
        ArgumentNullException.ThrowIfNull(NativeCharacteristic.Service, nameof(NativeCharacteristic.Service));
        ArgumentNullException.ThrowIfNull(NativeCharacteristic.Service.Peripheral, nameof(NativeCharacteristic.Service.Peripheral));

        NativeCharacteristic.Service.Peripheral.SetNotifyValue(shouldBeListening, NativeCharacteristic);
        return ValueTask.CompletedTask;
    }

    /// <summary>
    /// Called when the notification state of the characteristic is updated on the iOS platform.
    /// </summary>
    /// <param name="error">Any error that occurred during the operation.</param>
    /// <param name="characteristic">The characteristic whose notification state was updated.</param>
    /// <exception cref="AppleNativeBluetoothException">Thrown when the error parameter indicates a Bluetooth error.</exception>
    /// <exception cref="CharacteristicNotifyException">Thrown when the characteristic UUID doesn't match the expected UUID.</exception>
    public void UpdatedNotificationState(NSError? error, CBCharacteristic characteristic)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(characteristic);
            AppleNativeBluetoothException.ThrowIfError(error);

            if (characteristic.UUID != NativeCharacteristic.UUID)
            {
                throw new CharacteristicNotifyException(this, $"UpdatedNotificationState : {characteristic.UUID} != {NativeCharacteristic.UUID}");
            }

            OnWriteIsListeningSucceeded();
        }
        catch (Exception e)
        {
            OnWriteIsListeningFailed(e);
        }
    }

    /// <summary>
    /// Gets the iOS-specific notification capability string representation for the characteristic.
    /// </summary>
    /// <returns>
    /// Returns "N*" if notifications are enabled and listening, "N" if notifications are supported but not listening,
    /// "I*" if indications are enabled and listening, "I" if indications are supported but not listening,
    /// otherwise an empty string if neither notifications nor indications are supported.
    /// </returns>
    protected override string ToListenString()
    {
        if (NativeCharacteristic.Properties.HasFlag(CBCharacteristicProperties.Notify))
        {
            return IsListening ? "N*" : "N";
        }
        if (NativeCharacteristic.Properties.HasFlag(CBCharacteristicProperties.Indicate))
        {
            return IsListening ? "I*" : "I";
        }
        return string.Empty;
    }

    #endregion

    #region Write

    /// <inheritdoc/>
    /// <remarks>
    /// On iOS, checks if the characteristic has the WriteWithoutResponse or Write property flag set.
    /// </remarks>
    protected override bool NativeCanWrite()
    {
        return NativeCharacteristic.Properties.HasFlag(CBCharacteristicProperties.WriteWithoutResponse) || NativeCharacteristic.Properties.HasFlag(CBCharacteristicProperties.Write);
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException">Thrown when the native characteristic, its service, or peripheral is <c>null</c>.</exception>
    /// <remarks>
    /// On iOS, uses WriteWithoutResponse type if the characteristic supports it, otherwise uses WithResponse.
    /// Write operations without response complete immediately without confirmation.
    /// </remarks>
    protected override ValueTask NativeWriteValueAsync(ReadOnlyMemory<byte> value)
    {
        ArgumentNullException.ThrowIfNull(NativeCharacteristic);
        ArgumentNullException.ThrowIfNull(NativeCharacteristic.Service, nameof(NativeCharacteristic.Service));
        ArgumentNullException.ThrowIfNull(NativeCharacteristic.Service.Peripheral, nameof(NativeCharacteristic.Service.Peripheral));

        var writeWithoutResponse = NativeCharacteristic.Properties.HasFlag(CBCharacteristicProperties.WriteWithoutResponse);
        var nsData = NSData.FromArray(value.ToArray());

        NativeCharacteristic.Service.Peripheral.WriteValue(nsData, NativeCharacteristic, writeWithoutResponse ? CBCharacteristicWriteType.WithoutResponse : CBCharacteristicWriteType.WithResponse);

        if (writeWithoutResponse)
        {
            OnWriteValueSucceeded();
        }
        return ValueTask.CompletedTask;
    }

    /// <summary>
    /// Called when a characteristic value write operation completes on the iOS platform.
    /// </summary>
    /// <param name="error">Any error that occurred during the write operation.</param>
    /// <param name="characteristic">The characteristic that was written to.</param>
    /// <exception cref="AppleNativeBluetoothException">Thrown when the error parameter indicates a Bluetooth error.</exception>
    /// <exception cref="CharacteristicWriteException">Thrown when the characteristic UUID doesn't match the expected UUID.</exception>
    public void WroteCharacteristicValue(NSError? error, CBCharacteristic characteristic)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(characteristic);
            AppleNativeBluetoothException.ThrowIfError(error);

            if (characteristic.UUID != NativeCharacteristic.UUID)
            {
                throw new CharacteristicWriteException(this, Value, $"WroteCharacteristicValue : {characteristic.UUID} != {NativeCharacteristic.UUID}");
            }

            OnWriteValueSucceeded();
        }
        catch (Exception e)
        {
            OnWriteValueFailed(e);
        }
    }

    /// <summary>
    /// Gets the iOS-specific write capability string representation for the characteristic.
    /// </summary>
    /// <returns>
    /// Returns "WNR" for write without response, "WS" for authenticated signed writes, "W" for standard write,
    /// or an empty string if no write operations are supported.
    /// </returns>
    protected override string ToWriteString()
    {
        if (NativeCharacteristic.Properties.HasFlag(CBCharacteristicProperties.WriteWithoutResponse))
        {
            return "WNR";
        }

        if (NativeCharacteristic.Properties.HasFlag(CBCharacteristicProperties.AuthenticatedSignedWrites))
        {
            return "WS";
        }

        if (NativeCharacteristic.Properties.HasFlag(CBCharacteristicProperties.Write))
        {
            return "W";
        }

        return string.Empty;
    }

    #endregion

    #region Read

    /// <inheritdoc/>
    /// <remarks>
    /// On iOS, checks if the characteristic has the Read property flag set.
    /// </remarks>
    protected override bool NativeCanRead()
    {
        return NativeCharacteristic.Properties.HasFlag(CBCharacteristicProperties.Read);
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentNullException">Thrown when the native characteristic, its service, or peripheral is <c>null</c>.</exception>
    protected override ValueTask NativeReadValueAsync()
    {
        ArgumentNullException.ThrowIfNull(NativeCharacteristic);
        ArgumentNullException.ThrowIfNull(NativeCharacteristic.Service, nameof(NativeCharacteristic.Service));
        ArgumentNullException.ThrowIfNull(NativeCharacteristic.Service.Peripheral, nameof(NativeCharacteristic.Service.Peripheral));

        NativeCharacteristic.Service.Peripheral.ReadValue(NativeCharacteristic);
        return ValueTask.CompletedTask;
    }

    /// <summary>
    /// Called when a characteristic value is updated (read or notified) on the iOS platform.
    /// </summary>
    /// <param name="error">Any error that occurred during the read operation.</param>
    /// <param name="characteristic">The characteristic whose value was updated.</param>
    /// <exception cref="AppleNativeBluetoothException">Thrown when the error parameter indicates a Bluetooth error.</exception>
    /// <exception cref="CharacteristicReadException">Thrown when the characteristic UUID doesn't match the expected UUID.</exception>
    public void UpdatedCharacteristicValue(NSError? error, CBCharacteristic characteristic)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(characteristic);
            AppleNativeBluetoothException.ThrowIfError(error);

            if (characteristic.UUID != NativeCharacteristic.UUID)
            {
                throw new CharacteristicReadException(this, $"UpdatedCharacteristicValue : {characteristic.UUID} != {NativeCharacteristic.UUID}");
            }

            OnReadValueSucceeded(characteristic.Value?.ToArray() ?? []);
        }
        catch (Exception e)
        {
            OnReadValueFailed(e);
        }
    }

    #endregion

    #region Descriptors

    /// <summary>
    /// Called when a descriptor value is written on the iOS platform.
    /// </summary>
    /// <param name="error">Any error that occurred during the operation.</param>
    /// <param name="descriptor">The descriptor whose value was written.</param>
    /// <exception cref="AppleNativeBluetoothException">Thrown when the error parameter indicates a Bluetooth error.</exception>
    public void WroteDescriptorValue(NSError? error, CBDescriptor descriptor)
    {
        AppleNativeBluetoothException.ThrowIfError(error);
    }

    /// <summary>
    /// Called when a descriptor value is updated on the iOS platform.
    /// </summary>
    /// <param name="error">Any error that occurred during the operation.</param>
    /// <param name="descriptor">The descriptor whose value was updated.</param>
    /// <exception cref="AppleNativeBluetoothException">Thrown when the error parameter indicates a Bluetooth error.</exception>
    public void UpdatedValue(NSError? error, CBDescriptor descriptor)
    {
        AppleNativeBluetoothException.ThrowIfError(error);
    }

    /// <summary>
    /// Called when a descriptor is discovered for the characteristic on the iOS platform.
    /// </summary>
    /// <param name="error">Any error that occurred during the discovery operation.</param>
    /// <param name="characteristic">The characteristic for which the descriptor was discovered.</param>
    /// <exception cref="AppleNativeBluetoothException">Thrown when the error parameter indicates a Bluetooth error.</exception>
    public void DiscoveredDescriptor(NSError? error, CBCharacteristic characteristic)
    {
        AppleNativeBluetoothException.ThrowIfError(error);
    }

    #endregion
}
