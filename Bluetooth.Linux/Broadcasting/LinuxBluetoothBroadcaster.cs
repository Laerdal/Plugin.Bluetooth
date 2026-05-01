namespace Bluetooth.Linux.Broadcasting;

/// <summary>
///     Linux stub for the Bluetooth broadcaster. Broadcasting via BlueZ GATT server is not yet supported.
/// </summary>
public class LinuxBluetoothBroadcaster : BaseBluetoothBroadcaster
{
    /// <inheritdoc />
    public LinuxBluetoothBroadcaster(IBluetoothAdapter adapter, ITicker ticker, ILoggerFactory? loggerFactory = null)
        : base(adapter, ticker, loggerFactory)
    {
    }

    /// <inheritdoc />
    protected override void NativeRefreshIsRunning() => IsRunning = false;

    /// <inheritdoc />
    protected override ValueTask NativeStartAsync(BroadcastingOptions options, TimeSpan? timeout = null, CancellationToken cancellationToken = default) =>
        ValueTask.FromException(new NotSupportedException("Broadcasting is not yet supported on Linux via BlueZ."));

    /// <inheritdoc />
    protected override ValueTask NativeStopAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default) =>
        ValueTask.CompletedTask;

    /// <inheritdoc />
    protected override ValueTask<IBluetoothLocalService> NativeCreateServiceAsync(Guid id, string? name = null, bool isPrimary = true, TimeSpan? timeout = null, CancellationToken cancellationToken = default) =>
        ValueTask.FromException<IBluetoothLocalService>(new NotSupportedException("Broadcasting is not yet supported on Linux via BlueZ."));

    /// <inheritdoc />
    protected override ValueTask<bool> NativeHasBroadcasterPermissionsAsync() =>
        ValueTask.FromResult(false);

    /// <inheritdoc />
    protected override ValueTask NativeRequestBroadcasterPermissionsAsync(CancellationToken cancellationToken) =>
        ValueTask.FromException(new NotSupportedException("Broadcasting is not yet supported on Linux via BlueZ."));
}
