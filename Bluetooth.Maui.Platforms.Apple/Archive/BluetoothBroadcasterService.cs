using Bluetooth.Maui.PlatformSpecific;

namespace Bluetooth.Maui;

/// <summary>
/// iOS implementation of a mutable Bluetooth service for the broadcaster/peripheral role.
/// </summary>
public class BluetoothBroadcasterService : BaseBluetoothBroadcastService, CbPeripheralManagerWrapper.ICbServiceDelegate
{
    /// <inheritdoc />
    public BluetoothBroadcasterService(IBluetoothBroadcaster broadcaster, Guid id, string name) : base(broadcaster, id, name)
    {
    }

    /// <inheritdoc />
    protected async override Task NativeRemoveCharacteristicAsync(Guid id, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    protected async override Task<IBluetoothBroadcastCharacteristic> NativeAddCharacteristicAsync(Guid id,
        string name,
        CharacteristicProperties properties,
        CharacteristicPermissions permissions,
        ReadOnlyMemory<byte>? initialValue = null,
        TimeSpan? timeout = null,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public CbPeripheralManagerWrapper.ICbCharacteristicDelegate GetCharacteristic(CBCharacteristic? characteristic)
    {
        throw new NotImplementedException();
    }
}
