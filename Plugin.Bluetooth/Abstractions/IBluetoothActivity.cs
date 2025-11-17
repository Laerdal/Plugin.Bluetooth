
namespace Plugin.Bluetooth.Abstractions;

/// <summary>
/// Interface for managing a Bluetooth activity's lifecycle and state.
/// </summary>
public interface IBluetoothActivity : INotifyPropertyChanged
{
    #region IsBluetoothOn

    /// <summary>
    /// Gets a value indicating whether Bluetooth is currently enabled on the device.
    /// </summary>
    bool IsBluetoothOn { get; }

    /// <summary>
    /// Gets the event that is raised when the Bluetooth state changes.
    /// </summary>
    event EventHandler<BluetoothStateChangedEventArgs> BluetoothStateChanged;

    /// <summary>
    /// Waits asynchronously for Bluetooth to be turned on within the specified timeout.
    /// </summary>
    /// <param name="timeout">The timeout for this operation</param>
    /// <param name="cancellationToken">A cancellation token to cancel this operation.</param>
    /// <returns>A task that represents the asynchronous wait operation.</returns>
    Task WaitForBluetoothToBeOnAsync(TimeSpan? timeout, CancellationToken cancellationToken = default);

    #endregion

    /// <summary>
    /// Gets a value indicating whether the Bluetooth activity is actively running.
    /// </summary>
    bool IsRunning { get; }

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
    /// <param name="timeout">The timeout for this operation</param>
    /// <param name="cancellationToken">A cancellation token to cancel this operation.</param>
    /// <returns>A task that represents the asynchronous start operation.</returns>
    /// <remarks>Ensures that the Bluetooth activity is initialized and ready for use.</remarks>
    Task StartAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously starts the Bluetooth activity if it is not already running, with an optional timeout.
    /// </summary>
    /// <param name="timeout">The timeout for this operation</param>
    /// <param name="cancellationToken">A cancellation token to cancel this operation.</param>
    /// <returns>A task that represents the asynchronous start operation.</returns>
    /// <remarks>Checks if the Bluetooth activity is already running before attempting to start it.</remarks>
    ValueTask StartIfNeededAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default);

    #endregion

    #region Stop

    /// <summary>
    /// Gets a value indicating whether the Bluetooth manager is stopping.
    /// </summary>
    bool IsStopping { get; }

    /// <summary>
    /// Occurs when the Bluetooth manager is stopping.
    /// </summary>
    event EventHandler Stopping;

    /// <summary>
    /// Occurs when the Bluetooth manager has stopped.
    /// </summary>
    event EventHandler Stopped;

    /// <summary>
    /// Asynchronously stops the Bluetooth manager with an optional timeout.
    /// </summary>
    /// <param name="timeout">The timeout for this operation</param>
    /// <param name="cancellationToken">A cancellation token to cancel this operation.</param>
    /// <returns>A task that represents the asynchronous stop operation.</returns>
    /// <remarks>Ensures that the Bluetooth manager and its resources are safely released.</remarks>
    Task StopAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously stops the Bluetooth manager if it is running, with an optional timeout.
    /// </summary>
    /// <param name="timeout">The timeout for this operation</param>
    /// <param name="cancellationToken">A cancellation token to cancel this operation.</param>
    /// <returns>A task that represents the asynchronous stop operation.</returns>
    /// <remarks>Checks if the Bluetooth manager is running before attempting to stop it.</remarks>
    ValueTask StopIfNeededAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default);

    #endregion
}
