namespace Bluetooth.Abstractions.Broadcasting;

public partial interface IBluetoothBroadcaster
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

    #region Services - Create

    /// <summary>
    /// Adds a GATT service to be hosted by the broadcaster.
    /// </summary>
    /// <param name="request">The request containing the service details.</param>
    /// <param name="timeout">The timeout for this operation</param>
    /// <param name="cancellationToken">A cancellation token to cancel this operation.</param>
    /// <returns>The added service.</returns>
    ValueTask<IBluetoothLocalService> CreateServiceAsync(IBluetoothLocalServiceFactory.BluetoothLocalServiceSpec request, TimeSpan? timeout = null, CancellationToken cancellationToken = default);

    #endregion

    #region Services - Get

    /// <summary>
    /// Gets a hosted GATT service that matches the specified filter.
    /// </summary>
    /// <param name="filter">A function to filter services. Should return true for the desired service.</param>
    /// <returns>The matching service.</returns>
    /// <exception cref="ServiceNotFoundException">Thrown if no service matches the specified filter.</exception>
    /// <exception cref="MultipleServicesFoundException">Thrown if multiple services match the specified filter.</exception>
    IBluetoothLocalService GetService(Func<IBluetoothLocalService, bool> filter);

    /// <summary>
    /// Gets a hosted GATT service by its UUID.
    /// </summary>
    /// <param name="id">The UUID of the service to retrieve.</param>
    /// <returns>The matching service.</returns>
    /// <exception cref="ServiceNotFoundException">Thrown if no service matches the specified filter.</exception>
    /// <exception cref="MultipleServicesFoundException">Thrown if multiple services match the specified filter.</exception>
    IBluetoothLocalService GetService(Guid id);

    /// <summary>
    /// Gets a hosted GATT service that matches the specified filter.
    /// </summary>
    /// <param name="filter">A function to filter services. Should return true for the desired service.</param>
    /// <returns>The matching service, or null if not found.</returns>
    /// <exception cref="MultipleServicesFoundException">Thrown if multiple services match the specified filter.</exception>
    IBluetoothLocalService? GetServiceOrDefault(Func<IBluetoothLocalService, bool> filter);

    /// <summary>
    /// Gets a hosted GATT service by its UUID.
    /// </summary>
    /// <param name="id">The UUID of the service to retrieve.</param>
    /// <returns>The matching service, or null if not found.</returns>
    /// <exception cref="MultipleServicesFoundException">Thrown if multiple services match the specified filter.</exception>
    IBluetoothLocalService? GetServiceOrDefault(Guid id);

    /// <summary>
    /// Gets all hosted GATT services.
    /// </summary>
    /// <param name="filter">An optional filter to apply to the services.</param>
    /// <returns>
    /// A read-only snapshot of services at the time of the call. This collection is immutable
    /// and will not be modified if services are added or removed after the call returns.
    /// To get updated results, call this method again or subscribe to <see cref="ServiceListChanged"/> event.
    /// </returns>
    IReadOnlyList<IBluetoothLocalService> GetServices(Func<IBluetoothLocalService, bool>? filter = null);

    #endregion

    #region Services - Remove

    /// <summary>
    /// Removes a hosted GATT service from the broadcaster.
    /// </summary>
    /// <param name="id">The UUID of the service to remove.</param>
    /// <param name="timeout">The timeout for this operation</param>
    /// <param name="cancellationToken">A cancellation token to cancel this operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask RemoveServiceAsync(Guid id, TimeSpan? timeout = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes a hosted GATT service from the broadcaster.
    /// </summary>
    /// <param name="localService">The service to remove.</param>
    /// <param name="timeout">The timeout for this operation</param>
    /// <param name="cancellationToken">A cancellation token to cancel this operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask RemoveServiceAsync(IBluetoothLocalService localService, TimeSpan? timeout = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes all hosted services from the broadcaster.
    /// </summary>
    /// <param name="timeout">The timeout for this operation</param>
    /// <param name="cancellationToken">A cancellation token to cancel this operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask RemoveAllServicesAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default);

    #endregion

    #region Services - Has

    /// <summary>
    /// Checks if a hosted GATT service that matches the specified filter exists.
    /// </summary>
    /// <param name="filter">A function to filter services. Should return true for the desired service.</param>
    /// <returns>True if a matching service exists, false otherwise.</returns>
    bool HasService(Func<IBluetoothLocalService, bool> filter);

    /// <summary>
    /// Checks if a hosted GATT service with the specified UUID exists.
    /// </summary>
    /// <param name="id">The UUID of the service to check for.</param>
    /// <returns>True if a matching service exists, false otherwise.</returns>
    bool HasService(Guid id);

    #endregion

}
