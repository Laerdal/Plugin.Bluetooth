namespace Bluetooth.Maui;

public partial class BluetoothCharacteristic
{
    /// <inheritdoc/>
    /// <remarks>
    /// On Windows, checks if the characteristic has the Indicate or Notify property flag set.
    /// </remarks>
    protected override bool NativeCanListen()
    {
        return GattCharacteristicProxy.GattCharacteristic.CharacteristicProperties.HasFlag(GattCharacteristicProperties.Indicate) || GattCharacteristicProxy.GattCharacteristic.CharacteristicProperties.HasFlag(GattCharacteristicProperties.Notify);
    }

    /// <inheritdoc/>
    /// <exception cref="WindowsNativeBluetoothException">Thrown when the read operation fails with a status other than Success.</exception>
    protected async override ValueTask NativeReadIsListeningAsync()
    {
        // Ensure LISTEN is supported
        CharacteristicCantListenException.ThrowIfCantListen(this);

        var result = await GattCharacteristicProxy.GattCharacteristic.ReadClientCharacteristicConfigurationDescriptorAsync().AsTask().ConfigureAwait(false);
        if (result.Status != GattCommunicationStatus.Success)
        {
            throw new WindowsNativeBluetoothException(result.Status, result.ProtocolError);
        }
        OnReadIsListeningSucceeded(result.ClientCharacteristicConfigurationDescriptor != GattClientCharacteristicConfigurationDescriptorValue.None);

    }

    /// <inheritdoc/>
    /// <exception cref="WindowsNativeBluetoothException">Thrown when the write operation fails with a status other than Success.</exception>
    /// <exception cref="UnreachableException">Thrown when the characteristic doesn't support Notify or Indicate (should be caught by CharacteristicCantListenException).</exception>
    protected async override ValueTask NativeWriteIsListeningAsync(bool shouldBeListening)
    {
        // Ensure LISTEN is supported
        CharacteristicCantListenException.ThrowIfCantListen(this);

        // Get which bytes to write
        GattClientCharacteristicConfigurationDescriptorValue descriptorValue;
        if (!shouldBeListening)
        {
            descriptorValue = GattClientCharacteristicConfigurationDescriptorValue.None;
        }
        else if (GattCharacteristicProxy.GattCharacteristic.CharacteristicProperties.HasFlag(GattCharacteristicProperties.Notify))
        {
            descriptorValue = GattClientCharacteristicConfigurationDescriptorValue.Notify;
        }
        else if (GattCharacteristicProxy.GattCharacteristic.CharacteristicProperties.HasFlag(GattCharacteristicProperties.Indicate))
        {
            descriptorValue = GattClientCharacteristicConfigurationDescriptorValue.Indicate;
        }
        else
        {
            throw new UnreachableException("This case should be caught by CharacteristicCantListenException.ThrowIfCantListen");
        }

        var result = await GattCharacteristicProxy.GattCharacteristic.WriteClientCharacteristicConfigurationDescriptorAsync(descriptorValue).AsTask().ConfigureAwait(false);

        if (result != GattCommunicationStatus.Success)
        {
            throw new WindowsNativeBluetoothException(result);
        }

        OnWriteIsListeningSucceeded(); // Didn't see any native callback to confirm write is done, calling it here;
    }
}
