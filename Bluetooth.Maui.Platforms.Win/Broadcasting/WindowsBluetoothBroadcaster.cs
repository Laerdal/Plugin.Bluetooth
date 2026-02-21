namespace Bluetooth.Maui.Platforms.Win.Broadcasting;

/// <inheritdoc />
public class WindowsBluetoothBroadcaster : BaseBluetoothBroadcaster
{
    /// <inheritdoc />
    public WindowsBluetoothBroadcaster(
        IBluetoothAdapter adapter,
        IBluetoothLocalServiceFactory localServiceFactory,
        IBluetoothConnectedDeviceFactory connectedDeviceFactory,
        ITicker ticker,
        ILogger<IBluetoothBroadcaster>? logger = null)
        : base(adapter, localServiceFactory, connectedDeviceFactory, ticker, logger)
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
    ///     On Windows, no runtime permission request is needed. Bluetooth permissions are
    ///     declared in Package.appxmanifest and granted at install time.
    /// </remarks>
    protected override ValueTask NativeRequestBroadcasterPermissionsAsync(CancellationToken cancellationToken)
    {
        // No runtime request needed on Windows - permissions are declared at install time
        return ValueTask.CompletedTask;
    }

    #endregion
}
