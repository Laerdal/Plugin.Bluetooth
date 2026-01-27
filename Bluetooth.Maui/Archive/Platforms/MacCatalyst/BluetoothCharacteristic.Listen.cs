namespace Bluetooth.Maui;

public partial class BluetoothCharacteristic
{
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
    /// On iOS, calls DiscoverDescriptors on the peripheral to read the current notification state.
    /// </remarks>
    protected override ValueTask NativeReadIsListeningAsync()
    {
        // Ensure CbCharacteristic ref exists and is available
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
        // Ensure CbCharacteristic.Service.Peripheral ref exists and is available
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

            // Should not happen ... but just in case
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
}
