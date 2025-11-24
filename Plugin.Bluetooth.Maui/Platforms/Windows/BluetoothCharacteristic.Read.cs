using Plugin.Bluetooth.Maui.PlatformSpecific;
using Plugin.Bluetooth.Maui.PlatformSpecific.Exceptions;

namespace Plugin.Bluetooth.Maui;

public partial class BluetoothCharacteristic
{
    /// <inheritdoc/>
    protected override bool NativeCanRead()
    {
        return GattCharacteristicProxy.GattCharacteristic.CharacteristicProperties.HasFlag(GattCharacteristicProperties.Read);
    }

    /// <inheritdoc/>
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
