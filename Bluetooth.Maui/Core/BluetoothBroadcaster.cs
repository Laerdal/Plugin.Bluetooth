namespace Bluetooth.Maui;

/// <inheritdoc />
public partial class BluetoothBroadcaster : BaseBluetoothBroadcaster
{

    /// <inheritdoc/>
    /// <exception cref="PlatformNotSupportedException">This platform-agnostic implementation is not supported on non-native platforms.</exception>
    protected override ValueTask NativeInitializeAsync()
    {
        throw new PlatformNotSupportedException("This functionality is only supported on Native platforms. You called the shared version.");
    }

    /// <inheritdoc/>
    /// <exception cref="PlatformNotSupportedException">This platform-agnostic implementation is not supported on non-native platforms.</exception>
    protected override void NativeRefreshIsBluetoothOn()
    {
        throw new PlatformNotSupportedException("This functionality is only supported on Native platforms. You called the shared version.");
    }

    /// <inheritdoc/>
    /// <exception cref="PlatformNotSupportedException">This platform-agnostic implementation is not supported on non-native platforms.</exception>
    protected override void NativeRefreshIsRunning()
    {
        throw new PlatformNotSupportedException("This functionality is only supported on Native platforms. You called the shared version.");
    }

    /// <inheritdoc/>
    /// <exception cref="PlatformNotSupportedException">This platform-agnostic implementation is not supported on non-native platforms.</exception>
    protected override ValueTask NativeStartAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        throw new PlatformNotSupportedException("This functionality is only supported on Native platforms. You called the shared version.");
    }

    /// <inheritdoc/>
    /// <exception cref="PlatformNotSupportedException">This platform-agnostic implementation is not supported on non-native platforms.</exception>
    protected override ValueTask NativeStopAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        throw new PlatformNotSupportedException("This functionality is only supported on Native platforms. You called the shared version.");
    }

    /// <inheritdoc/>
    /// <exception cref="PlatformNotSupportedException">This platform-agnostic implementation is not supported on non-native platforms.</exception>
    protected override void NativeAdvertisementDataChanged()
    {
        throw new PlatformNotSupportedException("This functionality is only supported on Native platforms. You called the shared version.");
    }

    /// <inheritdoc/>
    /// <exception cref="PlatformNotSupportedException">This platform-agnostic implementation is not supported on non-native platforms.</exception>
    protected override IBluetoothService NativeCreateService(Guid serviceId, bool isPrimary)
    {
        throw new PlatformNotSupportedException("This functionality is only supported on Native platforms. You called the shared version.");
    }

    /// <inheritdoc/>
    /// <exception cref="PlatformNotSupportedException">This platform-agnostic implementation is not supported on non-native platforms.</exception>
    protected override Task NativeAddServiceAsync(IBluetoothService service, TimeSpan? timeout, CancellationToken cancellationToken)
    {
        throw new PlatformNotSupportedException("This functionality is only supported on Native platforms. You called the shared version.");
    }

    /// <inheritdoc/>
    /// <exception cref="PlatformNotSupportedException">This platform-agnostic implementation is not supported on non-native platforms.</exception>
    protected override Task NativeRemoveServiceAsync(IBluetoothService service, TimeSpan? timeout, CancellationToken cancellationToken)
    {
        throw new PlatformNotSupportedException("This functionality is only supported on Native platforms. You called the shared version.");
    }

    /// <inheritdoc/>
    /// <exception cref="PlatformNotSupportedException">This platform-agnostic implementation is not supported on non-native platforms.</exception>
    protected override Task<IBluetoothCharacteristic> NativeAddCharacteristicAsync(IBluetoothService service,
        Guid characteristicId,
        CharacteristicProperties properties,
        CharacteristicPermissions permissions,
        byte[]? initialValue,
        TimeSpan? timeout,
        CancellationToken cancellationToken)
    {
        throw new PlatformNotSupportedException("This functionality is only supported on Native platforms. You called the shared version.");
    }

    /// <inheritdoc/>
    /// <exception cref="PlatformNotSupportedException">This platform-agnostic implementation is not supported on non-native platforms.</exception>
    protected override Task NativeUpdateCharacteristicValueAsync(IBluetoothService service,
        Guid characteristicId,
        byte[] value,
        bool notifyClients,
        TimeSpan? timeout,
        CancellationToken cancellationToken)
    {
        throw new PlatformNotSupportedException("This functionality is only supported on Native platforms. You called the shared version.");
    }

    /// <inheritdoc/>
    /// <exception cref="PlatformNotSupportedException">This platform-agnostic implementation is not supported on non-native platforms.</exception>
    protected override Task NativeNotifyClientAsync(string clientId,
        IBluetoothService service,
        Guid characteristicId,
        byte[] value,
        TimeSpan? timeout,
        CancellationToken cancellationToken)
    {
        throw new PlatformNotSupportedException("This functionality is only supported on Native platforms. You called the shared version.");
    }

    /// <inheritdoc/>
    /// <exception cref="PlatformNotSupportedException">This platform-agnostic implementation is not supported on non-native platforms.</exception>
    protected override Task NativeDisconnectClientAsync(string clientId, TimeSpan? timeout, CancellationToken cancellationToken)
    {
        throw new PlatformNotSupportedException("This functionality is only supported on Native platforms. You called the shared version.");
    }
}
