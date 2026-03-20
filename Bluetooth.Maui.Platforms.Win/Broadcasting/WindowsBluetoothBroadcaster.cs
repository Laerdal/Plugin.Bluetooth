namespace Bluetooth.Maui.Platforms.Win.Broadcasting;

/// <inheritdoc />
public class WindowsBluetoothBroadcaster : BaseBluetoothBroadcaster
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="WindowsBluetoothBroadcaster" /> class.
    /// </summary>
    /// <param name="adapter">The Bluetooth adapter associated with this broadcaster.</param>
    /// <param name="ticker">The ticker for scheduling periodic refresh tasks.</param>
    /// <param name="loggerFactory">An optional logger factory for creating loggers used by this broadcaster and its components.</param>
    public WindowsBluetoothBroadcaster(IBluetoothAdapter adapter, ITicker ticker, ILoggerFactory? loggerFactory = null) : base(adapter, ticker, loggerFactory)
    {
    }

    /// <inheritdoc />
    protected override void NativeRefreshIsRunning()
    {
        throw new NotImplementedException("WindowsBluetoothBroadcaster is not yet implemented on Windows.");
    }

    /// <inheritdoc />
    protected override ValueTask NativeStartAsync(BroadcastingOptions options, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("WindowsBluetoothBroadcaster is not yet implemented on Windows.");
    }

    /// <inheritdoc />
    protected override ValueTask NativeStopAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException("WindowsBluetoothBroadcaster is not yet implemented on Windows.");
    }

    /// <inheritdoc />
    protected override ValueTask<IBluetoothLocalService> NativeCreateServiceAsync(Guid id,
        string? name = null,
        bool isPrimary = true,
        TimeSpan? timeout = null,
        CancellationToken cancellationToken = default)
    {
        // TODO: Implement Windows GATT server support
        throw new NotImplementedException("Windows GATT server implementation pending");
    }

    #region Permission Methods

    /// <inheritdoc />
    /// <remarks>
    ///     On Windows, Bluetooth permissions are capability-based and granted at install time
    ///     if the 'bluetooth' capability is declared in Package.appxmanifest.
    ///     This method always returns true.
    /// </remarks>
    protected override ValueTask<bool> NativeHasBroadcasterPermissionsAsync()
    {
        // On Windows, Bluetooth permissions are capability-based and granted at install time
        return ValueTask.FromResult(true);
    }

    /// <inheritdoc />
    /// <remarks>
    ///     On Windows, no runtime permission spec is needed. Bluetooth permissions are
    ///     declared in Package.appxmanifest and granted at install time.
    /// </remarks>
    protected override ValueTask NativeRequestBroadcasterPermissionsAsync(CancellationToken cancellationToken)
    {
        // No runtime spec needed on Windows - permissions are declared at install time
        return ValueTask.CompletedTask;
    }

    #endregion

}
