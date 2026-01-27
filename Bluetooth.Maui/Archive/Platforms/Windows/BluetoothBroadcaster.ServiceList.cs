namespace Bluetooth.Maui;

public partial class BluetoothBroadcaster
{
    /// <inheritdoc/>
    protected override IBluetoothService NativeCreateService(Guid serviceId, bool isPrimary)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    protected override Task NativeAddServiceAsync(IBluetoothService service, TimeSpan? timeout, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    protected override Task NativeRemoveServiceAsync(IBluetoothService service, TimeSpan? timeout, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    protected override Task<IBluetoothCharacteristic> NativeAddCharacteristicAsync(IBluetoothService service,
        Guid characteristicId,
        CharacteristicProperties properties,
        CharacteristicPermissions permissions,
        byte[]? initialValue,
        TimeSpan? timeout,
        CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    protected override Task NativeUpdateCharacteristicValueAsync(IBluetoothService service,
        Guid characteristicId,
        byte[] value,
        bool notifyClients,
        TimeSpan? timeout,
        CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    protected override Task NativeNotifyClientAsync(string clientId,
        IBluetoothService service,
        Guid characteristicId,
        byte[] value,
        TimeSpan? timeout,
        CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
