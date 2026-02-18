namespace Bluetooth.Abstractions.Scanning;

public partial interface IBluetoothRemoteDevice
{
    // ## LIST OF SERVICES ##

    #region Services - Events

    /// <summary>
    /// Occurs when the service list changes.
    /// </summary>
    event EventHandler<ServiceListChangedEventArgs>? ServiceListChanged;

    /// <summary>
    /// Event triggered when services are added.
    /// </summary>
    event EventHandler<ServicesAddedEventArgs>? ServicesAdded;

    /// <summary>
    /// Event triggered when services are removed.
    /// </summary>
    event EventHandler<ServicesRemovedEventArgs>? ServicesRemoved;

    #endregion

    #region Services - Exploration

    /// <summary>
    /// Gets a value indicating whether the device is exploring services.
    /// </summary>
    bool IsExploringServices { get; }

    /// <summary>
    /// Explores the services of the device asynchronously.
    /// </summary>
    /// <param name="options">
    /// Optional exploration configuration. If null, uses default options (services only, with caching enabled).
    /// Use <see cref="Options.ServiceExplorationOptions.ServicesOnly"/> for services-only exploration,
    /// <see cref="Options.ServiceExplorationOptions.WithCharacteristics"/> to include characteristics,
    /// or <see cref="Options.ServiceExplorationOptions.Full"/> for full discovery (services + characteristics + descriptors).
    /// Set <c>UseCache = false</c> to force re-exploration even if services were previously discovered.
    /// </param>
    /// <param name="timeout">The timeout for this operation</param>
    /// <param name="cancellationToken">A cancellation token to cancel this operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    /// <remarks>
    /// <b>Common Usage Patterns:</b>
    /// <example>
    /// <code>
    /// // Simple exploration (uses defaults: services only, with caching):
    /// await device.ExploreServicesAsync();
    ///
    /// // Explore services and characteristics:
    /// await device.ExploreServicesAsync(ServiceExplorationOptions.WithCharacteristics);
    ///
    /// // Full exploration (services, characteristics, descriptors):
    /// await device.ExploreServicesAsync(ServiceExplorationOptions.Full);
    ///
    /// // Force re-exploration (ignore cache):
    /// await device.ExploreServicesAsync(new() { UseCache = false });
    ///
    /// // Custom options with UUID filtering:
    /// await device.ExploreServicesAsync(new ServiceExplorationOptions
    /// {
    ///     Depth = ExplorationDepth.Characteristics,
    ///     ServiceUuidFilter = uuid => uuid == myServiceUuid,
    ///     UseCache = false
    /// });
    /// </code>
    /// </example>
    ///
    /// <b>Caching Behavior:</b>
    /// By default (<c>options = null</c>), caching is enabled (<c>UseCache = true</c>).
    /// This means if services have already been explored, the method returns immediately
    /// without re-querying the device. To force re-exploration, explicitly set <c>UseCache = false</c>.
    /// </remarks>
    /// <exception cref="DeviceNotConnectedException">Thrown when the device is not connected.</exception>
    /// <exception cref="TimeoutException">Thrown when the operation times out.</exception>
    /// <exception cref="OperationCanceledException">Thrown when the operation is cancelled.</exception>
    Task ExploreServicesAsync(Options.ServiceExplorationOptions? options = null, TimeSpan? timeout = null, CancellationToken cancellationToken = default);

    #endregion

    #region Services - Clear

    /// <summary>
    /// Resets the list of services and characteristics, and stops all subscriptions and notifications.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask ClearServicesAsync();

    #endregion

    #region Services - Get

    /// <summary>
    /// Gets the service that matches the specified filter.
    /// </summary>
    /// <param name="filter">The filter to apply to the services.</param>
    /// <returns>The service that matches the filter.</returns>
    /// <exception cref="ServiceNotFoundException">Thrown if no service matches the specified filter.</exception>
    /// <exception cref="MultipleServicesFoundException">Thrown if multiple services match the specified filter.</exception>
    IBluetoothRemoteService GetService(Func<IBluetoothRemoteService, bool> filter);

    /// <summary>
    /// Gets the service with the specified ID.
    /// </summary>
    /// <param name="id">The ID of the service to get.</param>
    /// <returns>The service with the specified ID.</returns>
    /// <exception cref="ServiceNotFoundException">Thrown if no service with the specified ID is found.</exception>
    /// <exception cref="MultipleServicesFoundException">Thrown if multiple services match the specified filter.</exception>
    IBluetoothRemoteService GetService(Guid id);


    /// <summary>
    /// Gets the service that matches the specified filter.
    /// </summary>
    /// <param name="filter">The filter to apply to the services.</param>
    /// <returns>The service that matches the filter, or null if no such service exists.</returns>
    /// <exception cref="MultipleServicesFoundException">Thrown if multiple services match the specified filter.</exception>
    IBluetoothRemoteService? GetServiceOrDefault(Func<IBluetoothRemoteService, bool> filter);

    /// <summary>
    /// Gets the service with the specified ID.
    /// </summary>
    /// <param name="id">The ID of the service to get.</param>
    /// <returns>The service with the specified ID, or null if no such service exists.</returns>
    /// <exception cref="MultipleServicesFoundException">Thrown if multiple services match the specified filter.</exception>
    IBluetoothRemoteService? GetServiceOrDefault(Guid id);

    /// <summary>
    /// Gets the services that match the specified filter.
    /// </summary>
    /// <param name="filter">The filter to apply to the services.</param>
    /// <returns>The services that match the filter, or all services if the filter is null.</returns>
    IEnumerable<IBluetoothRemoteService> GetServices(Func<IBluetoothRemoteService, bool>? filter = null);

    #endregion

    #region Services - Has

    /// <summary>
    /// Determines whether the device has a service that matches the specified filter.
    /// </summary>
    /// <param name="filter">The filter to apply to the services.</param>
    /// <returns>True if a matching service exists; otherwise, false.</returns>
    bool HasService(Func<IBluetoothRemoteService, bool> filter);

    /// <summary>
    /// Determines whether the device has a service with the specified ID.
    /// </summary>
    /// <param name="id">The ID of the service to check for.</param>
    /// <returns>True if a service with the specified ID exists; otherwise, false.</returns>
    bool HasService(Guid id);

    #endregion

}
