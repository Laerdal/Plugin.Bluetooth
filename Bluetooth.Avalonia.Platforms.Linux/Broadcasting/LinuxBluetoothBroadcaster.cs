namespace Bluetooth.Avalonia.Platforms.Linux.Broadcasting;

/// <inheritdoc />
public class LinuxBluetoothBroadcaster : BaseBluetoothBroadcaster
{
    /// <inheritdoc />
    /// <exception cref="PlatformNotSupportedException"></exception>
    public LinuxBluetoothBroadcaster(IBluetoothAdapter adapter, ITicker ticker, ILoggerFactory? loggerFactory = null) : base(adapter, ticker, loggerFactory)
    {
        throw new PlatformNotSupportedException("This functionality is only supported on Native platforms. Linux / Desktop BLE is not yet implemented. BlueZ integration is planned.");
    }

    /// <inheritdoc />
    /// <exception cref="PlatformNotSupportedException"></exception>
    protected override void NativeRefreshIsRunning()
    {
        throw new PlatformNotSupportedException("This functionality is only supported on Native platforms. Linux / Desktop BLE is not yet implemented. BlueZ integration is planned.");
    }

    /// <inheritdoc />
    /// <exception cref="PlatformNotSupportedException"></exception>
    protected override ValueTask NativeStartAsync(BroadcastingOptions options, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        try
        {
            throw new PlatformNotSupportedException("This functionality is only supported on Native platforms. Linux / Desktop BLE is not yet implemented. BlueZ integration is planned.");
        }
        catch (Exception exception)
        {
            return ValueTask.FromException(exception);
        }
    }

    /// <inheritdoc />
    /// <exception cref="PlatformNotSupportedException"></exception>
    protected override ValueTask NativeStopAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        try
        {
            throw new PlatformNotSupportedException("This functionality is only supported on Native platforms. Linux / Desktop BLE is not yet implemented. BlueZ integration is planned.");
        }
        catch (Exception exception)
        {
            return ValueTask.FromException(exception);
        }
    }

    /// <inheritdoc />
    /// <exception cref="PlatformNotSupportedException"></exception>
    protected override ValueTask<IBluetoothLocalService> NativeCreateServiceAsync(Guid id,
        string? name = null,
        bool isPrimary = true,
        TimeSpan? timeout = null,
        CancellationToken cancellationToken = default)
    {
        throw new PlatformNotSupportedException("This functionality is only supported on Native platforms. Linux / Desktop BLE is not yet implemented. BlueZ integration is planned.");
    }

    /// <inheritdoc />
    /// <exception cref="PlatformNotSupportedException"></exception>
    protected override ValueTask<bool> NativeHasBroadcasterPermissionsAsync()
    {
        throw new PlatformNotSupportedException("This functionality is only supported on Native platforms. Linux / Desktop BLE is not yet implemented. BlueZ integration is planned.");
    }

    /// <inheritdoc />
    /// <exception cref="PlatformNotSupportedException"></exception>
    protected override ValueTask NativeRequestBroadcasterPermissionsAsync(CancellationToken cancellationToken)
    {
        throw new PlatformNotSupportedException("This functionality is only supported on Native platforms. Linux / Desktop BLE is not yet implemented. BlueZ integration is planned.");
    }
}
