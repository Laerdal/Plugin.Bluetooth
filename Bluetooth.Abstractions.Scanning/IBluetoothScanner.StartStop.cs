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

}
