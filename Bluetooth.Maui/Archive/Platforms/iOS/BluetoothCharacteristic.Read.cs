namespace Bluetooth.Maui;

public partial class BluetoothCharacteristic
{
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
        // Ensure CbCharacteristic.Service.Peripheral ref exists and is available
        ArgumentNullException.ThrowIfNull(NativeCharacteristic);
        ArgumentNullException.ThrowIfNull(NativeCharacteristic.Service, nameof(NativeCharacteristic.Service));
        ArgumentNullException.ThrowIfNull(NativeCharacteristic.Service.Peripheral, nameof(NativeCharacteristic.Service.Peripheral));

        // Call ReadValue
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

            // Should not happen ... but just in case
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

}
