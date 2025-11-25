using Windows.Storage.Streams;

using Bluetooth.Maui.PlatformSpecific;
using Bluetooth.Maui.PlatformSpecific.Exceptions;

namespace Bluetooth.Maui;

public partial class BluetoothCharacteristic
{
    /// <inheritdoc/>
    /// <remarks>
    /// On Windows, checks if the characteristic has the WriteWithoutResponse or Write property flag set.
    /// </remarks>
    protected override bool NativeCanWrite()
    {
        return GattCharacteristicProxy.GattCharacteristic.CharacteristicProperties.HasFlag(GattCharacteristicProperties.WriteWithoutResponse) || GattCharacteristicProxy.GattCharacteristic.CharacteristicProperties.HasFlag(GattCharacteristicProperties.Write);
    }

    /// <inheritdoc/>
    /// <exception cref="WindowsNativeBluetoothException">Thrown when the write operation fails with a status other than Success.</exception>
    /// <remarks>
    /// On Windows, uses WriteWithoutResponse option if the characteristic supports it, otherwise uses WriteWithResponse.
    /// Write operations without response do not wait for confirmation from the device.
    /// </remarks>
    protected async override ValueTask NativeWriteValueAsync(ReadOnlyMemory<byte> value)
    {
        try
        {
            var writeWithoutResponse = GattCharacteristicProxy.GattCharacteristic.CharacteristicProperties.HasFlag(GattCharacteristicProperties.WriteWithoutResponse);

            using (var dataWriter = new DataWriter())
            {
                dataWriter.WriteBytes(value.ToArray());
                var valueBuffer = dataWriter.DetachBuffer();

                var result = await GattCharacteristicProxy.GattCharacteristic.WriteValueWithResultAsync(valueBuffer, writeWithoutResponse ? GattWriteOption.WriteWithoutResponse : GattWriteOption.WriteWithResponse);

                if (result.Status != GattCommunicationStatus.Success)
                {
                    throw new WindowsNativeBluetoothException(result.Status, result.ProtocolError);
                }
            }
            OnWriteValueSucceeded();
        }
        catch (Exception e)
        {
            OnWriteValueFailed(e);
        }
    }

}
