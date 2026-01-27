using System.Collections.ObjectModel;

namespace Bluetooth.Abstractions.Broadcasting;

/// <summary>
/// Interface for managing Bluetooth broadcasting operations.
/// Enables the device to act as a Bluetooth peripheral/server, advertising services and handling client connections.
/// </summary>
public interface IBluetoothBroadcaster
{
    /// <summary>
    /// Gets the Bluetooth adapter associated with this scanner.
    /// </summary>
    IBluetoothAdapter Adapter { get; }

    /// <summary>
    /// Gets the factory used to create Bluetooth broadcast services.
    /// </summary>
    IBluetoothBroadcastServiceFactory ServiceFactory { get; }

    /// <summary>
    /// Gets the factory used to create Bluetooth broadcast client devices.
    /// </summary>
    IBluetoothBroadcastClientDeviceFactory DeviceFactory { get; }

    /// <summary>
    /// Gets the permission manager for handling Bluetooth permissions.
    /// </summary>
    IBluetoothPermissionManager PermissionManager { get; }

    #region Start/Stop

    /// <summary>
    /// Occurs when the running state of the Bluetooth activity changes.
    /// </summary>
    event EventHandler? RunningStateChanged;

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
    /// <param name="options"></param>
    /// <param name="timeout">The timeout for this operation</param>
    /// <param name="cancellationToken">A cancellation token to cancel this operation.</param>
    /// <returns>A task that represents the asynchronous start operation.</returns>
    /// <remarks>Ensures that the Bluetooth activity is initialized and ready for use.</remarks>
    Task StartBroadcastingAsync(IBluetoothBroadcasterStartBroadcastingOptions options, TimeSpan? timeout = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously starts the Bluetooth activity if it is not already running, with an optional timeout.
    /// </summary>
    /// <param name="options"></param>
    /// <param name="timeout">The timeout for this operation</param>
    /// <param name="cancellationToken">A cancellation token to cancel this operation.</param>
    /// <returns>A task that represents the asynchronous start operation.</returns>
    /// <remarks>Checks if the Bluetooth activity is already running before attempting to start it.</remarks>
    ValueTask StartBroadcastingIfNeededAsync(IBluetoothBroadcasterStartBroadcastingOptions options, TimeSpan? timeout = null, CancellationToken cancellationToken = default);

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
    Task StopBroadcastingAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously stops the Broadcaster if it is running, with an optional timeout.
    /// </summary>
    /// <param name="timeout">The timeout for this operation</param>
    /// <param name="cancellationToken">A cancellation token to cancel this operation.</param>
    /// <returns>A task that represents the asynchronous stop operation.</returns>
    /// <remarks>Checks if the Broadcaster is running before attempting to stop it.</remarks>
    ValueTask StopBroadcastingIfNeededAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default);

    #endregion

    #endregion

    #region Service Managment

    /// <summary>
    /// Gets the collection of Broadcasted Services
    /// </summary>
    ReadOnlyDictionary<Guid, IBluetoothBroadcastService> Services { get; }

    /// <summary>
    /// Adds a GATT service to be hosted by the broadcaster.
    /// </summary>
    /// <param name="id">The UUID of the service.</param>
    /// <param name="name">The user-friendly name of the service.</param>
    /// <param name="isPrimary">Indicates whether this is a primary service.</param>
    /// <param name="timeout">The timeout for this operation</param>
    /// <param name="cancellationToken">A cancellation token to cancel this operation.</param>
    /// <returns>The added service.</returns>
    Task<IBluetoothBroadcastService> AddServiceAsync(Guid id,
        string name,
        bool isPrimary = true,
        TimeSpan? timeout = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes a hosted GATT service from the broadcaster.
    /// </summary>
    /// <param name="id">The UUID of the service to remove.</param>
    /// <param name="timeout">The timeout for this operation</param>
    /// <param name="cancellationToken">A cancellation token to cancel this operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task RemoveServiceAsync(Guid id, TimeSpan? timeout = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes a hosted GATT service from the broadcaster.
    /// </summary>
    /// <param name="service">The service to remove.</param>
    /// <param name="timeout">The timeout for this operation</param>
    /// <param name="cancellationToken">A cancellation token to cancel this operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task RemoveServiceAsync(IBluetoothBroadcastService service, TimeSpan? timeout = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes all hosted services from the broadcaster.
    /// </summary>
    /// <param name="timeout">The timeout for this operation</param>
    /// <param name="cancellationToken">A cancellation token to cancel this operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task RemoveAllServicesAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default);

    #endregion

    #region Configuration

    /// <summary>
    /// Submits changes to the advertised information
    /// </summary>
    /// <param name="options">The new set of information to broadcast</param>
    /// <param name="timeout">The timeout for this operation</param>
    /// <param name="cancellationToken">A cancellation token to cancel this operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task UpdateBroadcasterOptionsAsync(IBluetoothBroadcasterStartBroadcastingOptions options, TimeSpan? timeout = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the current information being advertised
    /// </summary>
    IBluetoothBroadcasterStartBroadcastingOptions StartBroadcastingOptions { get; }

    #endregion
}
