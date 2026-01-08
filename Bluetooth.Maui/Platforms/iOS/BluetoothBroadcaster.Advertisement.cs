using Bluetooth.Maui.PlatformSpecific;

namespace Bluetooth.Maui;

public partial class BluetoothBroadcaster
{
    /// <inheritdoc/>
    protected override void NativeSetManufacturerData(ushort manufacturerId, byte[] data)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    protected override void NativeSetAdvertisedServiceUuids(IEnumerable<Guid> serviceUuids)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    protected override void NativeClearAdvertisementData()
    {
        throw new NotImplementedException();
    }
}
