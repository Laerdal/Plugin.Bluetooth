namespace Bluetooth.Core.Abstractions;

/// <summary>
/// Interface for managing Bluetooth broadcasting operations, extending <see cref="IBluetoothActivity" />.
/// Enables the device to act as a Bluetooth peripheral/server, advertising services and handling client connections.
/// </summary>
public interface IBluetoothBroadcaster : IBluetoothActivity
{
    /// <summary>
    /// Gets the repository for known Bluetooth services and characteristics.
    /// </summary>
    IBluetoothCharacteristicAccessServicesRepository KnownServicesAndCharacteristicsRepository { get; }

    #region Advertisement

    /// <summary>
    /// Gets a value indicating whether the broadcaster is currently advertising.
    /// This reflects the state controlled by <see cref="IBluetoothActivity.StartAsync"/> and <see cref="IBluetoothActivity.StopAsync"/>.
    /// </summary>
    bool IsAdvertising { get; }

    /// <summary>
    /// Gets or sets the local device name advertised to potential clients.
    /// </summary>
    string? LocalDeviceName { get; set; }

    /// <summary>
    /// Gets or sets whether the device is connectable when advertising.
    /// When false, the device will only broadcast advertisement data without accepting connections.
    /// </summary>
    bool IsConnectable { get; set; }

    /// <summary>
    /// Sets custom manufacturer-specific data to include in the advertisement.
    /// </summary>
    /// <param name="manufacturerId">The manufacturer identifier (company ID).</param>
    /// <param name="data">The manufacturer-specific data.</param>
    void SetManufacturerData(ushort manufacturerId, byte[] data);

    /// <summary>
    /// Sets service UUIDs to advertise, making the device discoverable for these specific services.
    /// </summary>
    /// <param name="serviceUuids">The collection of service UUIDs to advertise.</param>
    void SetAdvertisedServiceUuids(IEnumerable<Guid> serviceUuids);

    /// <summary>
    /// Clears all custom advertisement data.
    /// </summary>
    void ClearAdvertisementData();

    #endregion

    #region Service Management

    /// <summary>
    /// Gets the collection of hosted GATT services.
    /// </summary>
    IEnumerable<IBluetoothService> HostedServices { get; }

    /// <summary>
    /// Event triggered when the hosted service list changes.
    /// </summary>
    event EventHandler<ServiceListChangedEventArgs>? HostedServiceListChanged;

    /// <summary>
    /// Event triggered when services are added to the broadcaster.
    /// </summary>
    event EventHandler<ServicesAddedEventArgs>? ServicesAdded;

    /// <summary>
    /// Event triggered when services are removed from the broadcaster.
    /// </summary>
    event EventHandler<ServicesRemovedEventArgs>? ServicesRemoved;

    /// <summary>
    /// Adds a GATT service to be hosted by the broadcaster.
    /// </summary>
    /// <param name="serviceId">The UUID of the service.</param>
    /// <param name="isPrimary">Indicates whether this is a primary service.</param>
    /// <param name="timeout">The timeout for this operation</param>
    /// <param name="cancellationToken">A cancellation token to cancel this operation.</param>
    /// <returns>The added service.</returns>
    Task<IBluetoothService> AddServiceAsync(Guid serviceId, bool isPrimary = true, TimeSpan? timeout = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes a hosted GATT service from the broadcaster.
    /// </summary>
    /// <param name="serviceId">The UUID of the service to remove.</param>
    /// <param name="timeout">The timeout for this operation</param>
    /// <param name="cancellationToken">A cancellation token to cancel this operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task RemoveServiceAsync(Guid serviceId, TimeSpan? timeout = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Removes all hosted services from the broadcaster.
    /// </summary>
    /// <param name="timeout">The timeout for this operation</param>
    /// <param name="cancellationToken">A cancellation token to cancel this operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task RemoveAllServicesAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a hosted service by its UUID.
    /// </summary>
    /// <param name="serviceId">The UUID of the service.</param>
    /// <returns>The service if found, otherwise null.</returns>
    IBluetoothService? GetServiceOrDefault(Guid serviceId);

    #endregion

    #region Characteristic Management

    /// <summary>
    /// Adds a characteristic to a hosted service.
    /// </summary>
    /// <param name="serviceId">The UUID of the service.</param>
    /// <param name="characteristicId">The UUID of the characteristic.</param>
    /// <param name="properties">The properties of the characteristic (read, write, notify, etc.).</param>
    /// <param name="permissions">The permissions required to access the characteristic.</param>
    /// <param name="initialValue">The initial value of the characteristic.</param>
    /// <param name="timeout">The timeout for this operation</param>
    /// <param name="cancellationToken">A cancellation token to cancel this operation.</param>
    /// <returns>The added characteristic.</returns>
    Task<IBluetoothCharacteristic> AddCharacteristicAsync(
        Guid serviceId,
        Guid characteristicId,
        CharacteristicProperties properties,
        CharacteristicPermissions permissions,
        byte[]? initialValue = null,
        TimeSpan? timeout = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates the value of a hosted characteristic and optionally notifies/indicates subscribed clients.
    /// </summary>
    /// <param name="serviceId">The UUID of the service containing the characteristic.</param>
    /// <param name="characteristicId">The UUID of the characteristic.</param>
    /// <param name="value">The new value.</param>
    /// <param name="notifyClients">Whether to notify subscribed clients of the value change.</param>
    /// <param name="timeout">The timeout for this operation</param>
    /// <param name="cancellationToken">A cancellation token to cancel this operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task UpdateCharacteristicValueAsync(
        Guid serviceId,
        Guid characteristicId,
        byte[] value,
        bool notifyClients = true,
        TimeSpan? timeout = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends a notification to a specific client for a characteristic value change.
    /// </summary>
    /// <param name="clientId">The client identifier to notify.</param>
    /// <param name="serviceId">The UUID of the service containing the characteristic.</param>
    /// <param name="characteristicId">The UUID of the characteristic.</param>
    /// <param name="value">The value to send.</param>
    /// <param name="timeout">The timeout for this operation</param>
    /// <param name="cancellationToken">A cancellation token to cancel this operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task NotifyClientAsync(
        string clientId,
        Guid serviceId,
        Guid characteristicId,
        byte[] value,
        TimeSpan? timeout = null,
        CancellationToken cancellationToken = default);

    #endregion

    #region Client Connection Management

    /// <summary>
    /// Gets the collection of currently connected client identifiers.
    /// </summary>
    IEnumerable<string> ConnectedClients { get; }

    /// <summary>
    /// Gets the number of currently connected clients.
    /// </summary>
    int ConnectedClientCount { get; }

    /// <summary>
    /// Gets or sets the maximum number of simultaneous client connections allowed.
    /// </summary>
    int MaximumConnectedClients { get; set; }

    /// <summary>
    /// Event triggered when a client connects to the broadcaster.
    /// </summary>
    event EventHandler<ClientConnectedEventArgs>? ClientConnected;

    /// <summary>
    /// Event triggered when a client disconnects from the broadcaster.
    /// </summary>
    event EventHandler<ClientDisconnectedEventArgs>? ClientDisconnected;

    /// <summary>
    /// Disconnects a specific client.
    /// </summary>
    /// <param name="clientId">The identifier of the client to disconnect.</param>
    /// <param name="timeout">The timeout for this operation</param>
    /// <param name="cancellationToken">A cancellation token to cancel this operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task DisconnectClientAsync(string clientId, TimeSpan? timeout = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Disconnects all connected clients.
    /// </summary>
    /// <param name="timeout">The timeout for this operation</param>
    /// <param name="cancellationToken">A cancellation token to cancel this operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task DisconnectAllClientsAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets information about a connected client.
    /// </summary>
    /// <param name="clientId">The identifier of the client.</param>
    /// <returns>The client information if connected, otherwise null.</returns>
    IBluetoothDevice? GetConnectedClientOrDefault(string clientId);

    #endregion

    #region Client Request Handlers

    /// <summary>
    /// Event triggered when a client requests to read a characteristic value.
    /// Handle this event to provide the requested value dynamically.
    /// </summary>
    event EventHandler<CharacteristicReadRequestEventArgs>? CharacteristicReadRequested;

    /// <summary>
    /// Event triggered when a client requests to write a value to a characteristic.
    /// Handle this event to process the written value and optionally reject the write.
    /// </summary>
    event EventHandler<CharacteristicWriteRequestEventArgs>? CharacteristicWriteRequested;

    /// <summary>
    /// Event triggered when a client subscribes or unsubscribes from characteristic notifications/indications.
    /// </summary>
    event EventHandler<CharacteristicSubscriptionChangedEventArgs>? CharacteristicSubscriptionChanged;

    #endregion

    #region Resource Management

    /// <summary>
    /// Cleans resources associated with a specific client.
    /// </summary>
    /// <param name="clientId">The identifier of the client to clean.</param>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    ValueTask CleanAsync(string clientId);

    /// <summary>
    /// Cleans all resources associated with clients and hosted services.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    ValueTask CleanAsync();

    #endregion
}
