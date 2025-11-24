using Plugin.Bluetooth.Maui.PlatformSpecific;
using Plugin.Bluetooth.Maui.PlatformSpecific.Exceptions;

namespace Plugin.Bluetooth.Maui;

public partial class BluetoothCharacteristic
{
    /// <inheritdoc/>
    protected override bool NativeCanWrite()
    {
        return NativeCharacteristic.Properties.HasFlag(CBCharacteristicProperties.WriteWithoutResponse) || NativeCharacteristic.Properties.HasFlag(CBCharacteristicProperties.Write);
    }

    /// <inheritdoc/>
    protected override ValueTask NativeWriteValueAsync(ReadOnlyMemory<byte> value)
    {
        // Ensure CbCharacteristic.Service.Peripheral ref exists and is available
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

            // Should not happen ... but just in case
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

}
