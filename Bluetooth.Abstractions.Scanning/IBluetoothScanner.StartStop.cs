namespace Bluetooth.Abstractions.Scanning;

public partial interface IBluetoothScanner
{
    /// <summary>
    ///     Gets a value indicating whether the Bluetooth activity is actively running.
    /// </summary>
    bool IsRunning { get; }

    /// <summary>
    ///     Occurs when the running state of the Bluetooth activity changes.
    /// </summary>
    event EventHandler? RunningStateChanged;

    #region Start

    /// <summary>
    ///     Gets a value indicating whether the Bluetooth activity is starting.
    /// </summary>
    bool IsStarting { get; }

    /// <summary>
    ///     Occurs when the Bluetooth activity is starting.
    /// </summary>
    event EventHandler Starting;

    /// <summary>
    ///     Occurs when the Bluetooth activity has started.
    /// </summary>
    event EventHandler Started;

    /// <summary>
    ///     Asynchronously starts the Bluetooth activity with an optional timeout.
    /// </summary>
    /// <param name="scanningOptions">The options for starting the Bluetooth activity. If null, default options will be used.</param>
    /// <param name="permissionOptions">The options for requesting permissions. If null, default options will be used.</param>
    /// <param name="timeout">The timeout for this operation</param>
    /// <param name="cancellationToken">A cancellation token to cancel this operation.</param>
    /// <returns>A task that represents the asynchronous start operation.</returns>
    /// <remarks>Ensures that the Bluetooth activity is initialized and ready for use.</remarks>
    /// <exception cref="ScannerIsAlreadyStartedException">Thrown when the scanner is already running.</exception>
    /// <exception cref="ScannerFailedToStartException">Thrown when the scanner fails to start.</exception>
    /// <exception cref="ScannerUnexpectedStartException">Thrown when an unexpected error occurs during start.</exception>
    /// <exception cref="TimeoutException">Thrown when the operation times out.</exception>
    /// <exception cref="OperationCanceledException">Thrown when the operation is cancelled.</exception>
    Task StartScanningAsync(ScanningOptions? scanningOptions = null,
        PermissionOptions? permissionOptions = null,
        TimeSpan? timeout = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Asynchronously starts the Bluetooth activity if it is not already running, with an optional timeout.
    /// </summary>
    /// <param name="scanningOptions">The options for starting the Bluetooth activity. If null, default options will be used.</param>
    /// <param name="permissionOptions">The options for requesting permissions. If null, default options will be used.</param>
    /// <param name="timeout">The timeout for this operation</param>
    /// <param name="cancellationToken">A cancellation token to cancel this operation.</param>
    /// <returns>A task that represents the asynchronous start operation.</returns>
    /// <remarks>Checks if the Bluetooth activity is already running before attempting to start it.</remarks>
    ValueTask StartScanningIfNeededAsync(ScanningOptions? scanningOptions = null,
        PermissionOptions? permissionOptions = null,
        TimeSpan? timeout = null,
        CancellationToken cancellationToken = default);

    #endregion

    #region Stop

    /// <summary>
    ///     Gets a value indicating whether the Scanner is stopping.
    /// </summary>
    bool IsStopping { get; }

    /// <summary>
    ///     Occurs when the Scanner is stopping.
    /// </summary>
    event EventHandler Stopping;

    /// <summary>
    ///     Occurs when the Scanner has stopped.
    /// </summary>
    event EventHandler Stopped;

    /// <summary>
    ///     Asynchronously stops the Scanner with an optional timeout.
    /// </summary>
    /// <param name="timeout">The timeout for this operation</param>
    /// <param name="cancellationToken">A cancellation token to cancel this operation.</param>
    /// <returns>A task that represents the asynchronous stop operation.</returns>
    /// <remarks>Ensures that the Scanner and its resources are safely released.</remarks>
    /// <exception cref="ScannerIsAlreadyStoppedException">Thrown when the scanner is already stopped.</exception>
    /// <exception cref="ScannerFailedToStopException">Thrown when the scanner fails to stop.</exception>
    /// <exception cref="ScannerUnexpectedStopException">Thrown when an unexpected error occurs during stop.</exception>
    /// <exception cref="TimeoutException">Thrown when the operation times out.</exception>
    /// <exception cref="OperationCanceledException">Thrown when the operation is cancelled.</exception>
    Task StopScanningAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Asynchronously stops the Scanner if it is running, with an optional timeout.
    /// </summary>
    /// <param name="timeout">The timeout for this operation</param>
    /// <param name="cancellationToken">A cancellation token to cancel this operation.</param>
    /// <returns>A task that represents the asynchronous stop operation.</returns>
    /// <remarks>Checks if the Scanner is running before attempting to stop it.</remarks>
    ValueTask StopScanningIfNeededAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default);

    #endregion

    #region Clean Restart

    /// <summary>
    ///     Stops the scanner, discards every device in the scanner's registry, then starts scanning again.
    /// </summary>
    /// <param name="newAdvertisementFilter">
    ///     An optional replacement for <see cref="AdvertisementFilter" />, applied while the scanner is stopped so it is
    ///     already in effect when scanning resumes. When null the current filter is left untouched.
    /// </param>
    /// <param name="scanningOptions">
    ///     The options to restart with. When null the options of the scan session being restarted are reused, falling back to
    ///     defaults if the scanner was not running.
    /// </param>
    /// <param name="permissionOptions">The options for requesting permissions. If null, default options will be used.</param>
    /// <param name="timeout">The timeout applied to the stop and the start leg individually. Does not bound the registry-clear step in between.</param>
    /// <param name="cancellationToken">A cancellation token to cancel this operation.</param>
    /// <returns>A task that represents the asynchronous clean restart operation.</returns>
    /// <remarks>
    ///     <para>
    ///         Unlike <see cref="StopScanningAsync" /> followed by <see cref="StartScanningAsync" />, this also drops the
    ///         device registry, so devices discovered before the restart are gone: every device is disconnected, removed and
    ///         disposed. Hold on to device identifiers rather than <see cref="IBluetoothRemoteDevice" /> instances across a
    ///         clean restart, and re-acquire the instance afterwards with <see cref="WaitForDeviceToAppearAsync(string, TimeSpan?, CancellationToken)" />.
    ///     </para>
    ///     <para>
    ///         The intended use case is a device that changes identity while the scanner is running - most notably a device
    ///         rebooting into or out of firmware-update mode, where a stale registry entry and an in-flight scan session
    ///         otherwise prevent it from being rediscovered under its new advertisement.
    ///     </para>
    ///     <para>
    ///         Restarting a scanner that is already stopped is valid: the stop leg is skipped and the registry is still dropped.
    ///     </para>
    /// </remarks>
    /// <exception cref="ScannerFailedToStopException">Thrown when the scanner fails to stop.</exception>
    /// <exception cref="ScannerFailedToStartException">Thrown when the scanner fails to start again.</exception>
    /// <exception cref="TimeoutException">Thrown when either leg of the operation times out.</exception>
    /// <exception cref="OperationCanceledException">Thrown when the operation is cancelled.</exception>
    Task CleanRestartScanningAsync(Func<IBluetoothAdvertisement, bool>? newAdvertisementFilter = null,
        ScanningOptions? scanningOptions = null,
        PermissionOptions? permissionOptions = null,
        TimeSpan? timeout = null,
        CancellationToken cancellationToken = default);

    #endregion

}
