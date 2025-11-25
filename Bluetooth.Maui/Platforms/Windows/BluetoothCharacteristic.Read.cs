using Bluetooth.Maui.PlatformSpecific;
using Bluetooth.Maui.PlatformSpecific.Exceptions;

namespace Bluetooth.Maui;

public partial class BluetoothCharacteristic
{
    /// <inheritdoc/>
    /// <remarks>
    /// On Windows, checks if the characteristic has the Read property flag set.
    /// </remarks>
    protected override bool NativeCanRead()
    {
        return GattCharacteristicProxy.GattCharacteristic.CharacteristicProperties.HasFlag(GattCharacteristicProperties.Read);
    }

    /// <inheritdoc/>
    /// <exception cref="WindowsNativeBluetoothException">Thrown when the read operation fails with a status other than Success.</exception>
    /// <remarks>
    /// On Windows, uses BluetoothCacheMode.Uncached to ensure fresh data is read from the device.
    /// </remarks>
    protected async override ValueTask NativeReadValueAsync()
    {
        var result = await GattCharacteristicProxy.GattCharacteristic.ReadValueAsync(BluetoothCacheMode.Uncached).AsTask().ConfigureAwait(false);
        if (result.Status != GattCommunicationStatus.Success)
        {
            throw new WindowsNativeBluetoothException(result.Status, result.ProtocolError);
        }
        OnReadValueSucceeded(result.Value.Capacity <= 0 ? [] : result.Value.ToArray() ?? []);
    }

}
