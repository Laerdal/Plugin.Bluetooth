using Bluetooth.Abstractions.Broadcasting.Options;

namespace Bluetooth.Abstractions.Broadcasting;

/// <summary>
/// Interface for managing Bluetooth broadcasting operations.
/// Enables the device to act as a Bluetooth peripheral/server, advertising services and handling client connections.
/// </summary>
public partial interface IBluetoothBroadcaster
{
    /// <summary>
    /// Gets the current information being advertised
    /// </summary>
    BroadcastingOptions CurrentBroadcastingOptions { get; }

    #region IsRunning

    /// <summary>
    /// Occurs when the running state of the Bluetooth activity changes.
    /// </summary>
    event EventHandler? RunningStateChanged;

    /// <summary>
    /// Gets a value indicating whether the Bluetooth activity is actively running.
    /// </summary>
    bool IsRunning { get; }

    #endregion

    #region Start

    /// <summary>
    /// Gets a value indicating whether the Bluetooth activity is starting.
    /// </summary>
    bool IsStarting { get; }

    /// <summary>
    /// Occurs when the Bluetooth activity is starting.
    /// </summary>
    event EventHandler Starting;

    /// <summary>
    /// Occurs when the Bluetooth activity has started.
    /// </summary>
    event EventHandler Started;

    /// <summary>
    /// Asynchronously starts the Bluetooth activity with an optional timeout.
    /// </summary>
    /// <param name="options">The broadcasting options to use when starting the broadcaster.</param>
    /// <param name="timeout">The timeout for this operation</param>
    /// <param name="cancellationToken">A cancellation token to cancel this operation.</param>
    /// <returns>A task that represents the asynchronous start operation.</returns>
    /// <remarks>Ensures that the Bluetooth activity is initialized and ready for use.</remarks>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="options"/> is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown when broadcasting is already active.</exception>
    /// <exception cref="PlatformNotSupportedException">Thrown when the platform doesn't support peripheral mode.</exception>
    /// <exception cref="TimeoutException">Thrown when the operation times out.</exception>
    /// <exception cref="OperationCanceledException">Thrown when the operation is cancelled.</exception>
    ValueTask StartBroadcastingAsync(BroadcastingOptions options, TimeSpan? timeout = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously starts the Bluetooth activity if it is not already running, with an optional timeout.
    /// </summary>
    /// <param name="options">The broadcasting options to use when starting the broadcaster if needed.</param>
    /// <param name="timeout">The timeout for this operation</param>
    /// <param name="cancellationToken">A cancellation token to cancel this operation.</param>
    /// <returns>A task that represents the asynchronous start operation.</returns>
    /// <remarks>Checks if the Bluetooth activity is already running before attempting to start it.</remarks>
    ValueTask StartBroadcastingIfNeededAsync(BroadcastingOptions options, TimeSpan? timeout = null, CancellationToken cancellationToken = default);

    #endregion

    #region Stop

    /// <summary>
    /// Gets a value indicating whether the Broadcaster is stopping.
    /// </summary>
    bool IsStopping { get; }

    /// <summary>
    /// Occurs when the Broadcaster is stopping.
    /// </summary>
    event EventHandler Stopping;

    /// <summary>
    /// Occurs when the Broadcaster has stopped.
    /// </summary>
    event EventHandler Stopped;

    /// <summary>
    /// Asynchronously stops the Broadcaster with an optional timeout.
    /// </summary>
    /// <param name="timeout">The timeout for this operation</param>
    /// <param name="cancellationToken">A cancellation token to cancel this operation.</param>
    /// <returns>A task that represents the asynchronous stop operation.</returns>
    /// <remarks>Ensures that the Broadcaster and its resources are safely released.</remarks>
    /// <exception cref="InvalidOperationException">Thrown when broadcasting is not active.</exception>
    /// <exception cref="TimeoutException">Thrown when the operation times out.</exception>
    /// <exception cref="OperationCanceledException">Thrown when the operation is cancelled.</exception>
    ValueTask StopBroadcastingAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously stops the Broadcaster if it is running, with an optional timeout.
    /// </summary>
    /// <param name="timeout">The timeout for this operation</param>
    /// <param name="cancellationToken">A cancellation token to cancel this operation.</param>
    /// <returns>A task that represents the asynchronous stop operation.</returns>
    /// <remarks>Checks if the Broadcaster is running before attempting to stop it.</remarks>
    ValueTask StopBroadcastingIfNeededAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default);

    #endregion

    /// <summary>
    /// Submits changes to the advertised information
    /// </summary>
    /// <param name="options">The new set of information to broadcast</param>
    /// <param name="timeout">The timeout for this operation</param>
    /// <param name="cancellationToken">A cancellation token to cancel this operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="options"/> is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown when broadcasting is not active.</exception>
    /// <exception cref="TimeoutException">Thrown when the operation times out.</exception>
    /// <exception cref="OperationCanceledException">Thrown when the operation is canceled.</exception>
    ValueTask UpdateBroadcastingOptionsAsync(BroadcastingOptions options, TimeSpan? timeout = null, CancellationToken cancellationToken = default);

}
