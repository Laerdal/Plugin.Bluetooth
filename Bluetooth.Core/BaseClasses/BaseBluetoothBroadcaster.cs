
namespace Bluetooth.Core.BaseClasses;

/// <inheritdoc cref="IBluetoothBroadcaster" />
/// <summary>
/// Base class for Bluetooth Low Energy broadcaster implementations that advertise the device's presence.
/// </summary>
/// <remarks>
/// Broadcasters allow a device to act as a BLE peripheral, advertising its presence and services to nearby devices.
/// This is the opposite role of a scanner, which listens for advertisements.
/// </remarks>
public abstract partial class BaseBluetoothBroadcaster : BaseBluetoothActivity, IBluetoothBroadcaster
{
    /// <inheritdoc />
    protected async override ValueTask InitializeAsync()
    {
        await KnownServicesAndCharacteristicsRepository.AddAllServiceDefinitionsInCurrentAssemblyAsync().ConfigureAwait(false);
        await NativeInitializeAsync().ConfigureAwait(false);
    }

    /// <inheritdoc />
    public IBluetoothCharacteristicAccessServicesRepository KnownServicesAndCharacteristicsRepository { get; } = new CharacteristicAccessServicesRepository();

    /// <summary>
    /// Creates a native platform-specific service instance.
    /// </summary>
    /// <param name="serviceId">The UUID of the service to create.</param>
    /// <param name="isPrimary">Indicates whether this is a primary service.</param>
    /// <returns>A platform-specific <see cref="IBluetoothService"/> instance.</returns>
    protected abstract IBluetoothService NativeCreateService(Guid serviceId, bool isPrimary);

    /// <summary>
    /// Creates a native service and adds it to the service list.
    /// </summary>
    /// <param name="serviceId">The UUID of the service to create and add.</param>
    /// <param name="isPrimary">Indicates whether this is a primary service.</param>
    /// <returns>The newly created and added <see cref="IBluetoothService"/> instance.</returns>
    protected virtual IBluetoothService AddService(Guid serviceId, bool isPrimary)
    {
        var service = NativeCreateService(serviceId, isPrimary);
        lock (HostedServicesInternal)
        {
            HostedServicesInternal.Add(service);
        }
        return service;
    }

    /// <inheritdoc />
    public async Task<IBluetoothService> AddServiceAsync(Guid serviceId, bool isPrimary = true, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        var service = AddService(serviceId, isPrimary);
        await NativeAddServiceAsync(service, timeout, cancellationToken).ConfigureAwait(false);
        return service;
    }

    /// <summary>
    /// Platform-specific implementation for adding a service to the GATT server.
    /// </summary>
    /// <param name="service">The service to add.</param>
    /// <param name="timeout">The timeout for this operation.</param>
    /// <param name="cancellationToken">A cancellation token to cancel this operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    protected abstract Task NativeAddServiceAsync(IBluetoothService service, TimeSpan? timeout, CancellationToken cancellationToken);

    /// <inheritdoc />
    public async Task RemoveServiceAsync(Guid serviceId, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        var service = GetServiceOrDefault(serviceId);
        if (service == null)
        {
            return;
        }

        await NativeRemoveServiceAsync(service, timeout, cancellationToken).ConfigureAwait(false);

        lock (HostedServicesInternal)
        {
            HostedServicesInternal.Remove(service);
        }
    }

    /// <summary>
    /// Platform-specific implementation for removing a service from the GATT server.
    /// </summary>
    /// <param name="service">The service to remove.</param>
    /// <param name="timeout">The timeout for this operation.</param>
    /// <param name="cancellationToken">A cancellation token to cancel this operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    protected abstract Task NativeRemoveServiceAsync(IBluetoothService service, TimeSpan? timeout, CancellationToken cancellationToken);

    /// <inheritdoc />
    public async Task RemoveAllServicesAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        var services = HostedServices.ToList();
        foreach (var service in services)
        {
            await RemoveServiceAsync(service.Id, timeout, cancellationToken).ConfigureAwait(false);
        }
    }

    /// <inheritdoc />
    public async Task<IBluetoothCharacteristic> AddCharacteristicAsync(
        Guid serviceId,
        Guid characteristicId,
        CharacteristicProperties properties,
        CharacteristicPermissions permissions,
        byte[]? initialValue = null,
        TimeSpan? timeout = null,
        CancellationToken cancellationToken = default)
    {
        var service = GetServiceOrDefault(serviceId) ?? throw new InvalidOperationException($"Service with ID {serviceId} not found.");

        var characteristic = await NativeAddCharacteristicAsync(service, characteristicId, properties, permissions, initialValue, timeout, cancellationToken).ConfigureAwait(false);

        return characteristic;
    }

    /// <summary>
    /// Platform-specific implementation for adding a characteristic to a service.
    /// </summary>
    /// <param name="service">The service to add the characteristic to.</param>
    /// <param name="characteristicId">The UUID of the characteristic.</param>
    /// <param name="properties">The properties of the characteristic.</param>
    /// <param name="permissions">The permissions of the characteristic.</param>
    /// <param name="initialValue">The initial value of the characteristic.</param>
    /// <param name="timeout">The timeout for this operation.</param>
    /// <param name="cancellationToken">A cancellation token to cancel this operation.</param>
    /// <returns>The added characteristic.</returns>
    protected abstract Task<IBluetoothCharacteristic> NativeAddCharacteristicAsync(
        IBluetoothService service,
        Guid characteristicId,
        CharacteristicProperties properties,
        CharacteristicPermissions permissions,
        byte[]? initialValue,
        TimeSpan? timeout,
        CancellationToken cancellationToken);

    /// <inheritdoc />
    public async Task UpdateCharacteristicValueAsync(
        Guid serviceId,
        Guid characteristicId,
        byte[] value,
        bool notifyClients = true,
        TimeSpan? timeout = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(value);

        var service = GetServiceOrDefault(serviceId) ?? throw new InvalidOperationException($"Service with ID {serviceId} not found.");

        await NativeUpdateCharacteristicValueAsync(service, characteristicId, value, notifyClients, timeout, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Platform-specific implementation for updating a characteristic value.
    /// </summary>
    /// <param name="service">The service containing the characteristic.</param>
    /// <param name="characteristicId">The UUID of the characteristic.</param>
    /// <param name="value">The new value.</param>
    /// <param name="notifyClients">Whether to notify subscribed clients.</param>
    /// <param name="timeout">The timeout for this operation.</param>
    /// <param name="cancellationToken">A cancellation token to cancel this operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    protected abstract Task NativeUpdateCharacteristicValueAsync(
        IBluetoothService service,
        Guid characteristicId,
        byte[] value,
        bool notifyClients,
        TimeSpan? timeout,
        CancellationToken cancellationToken);

    /// <inheritdoc />
    public async Task NotifyClientAsync(
        string clientId,
        Guid serviceId,
        Guid characteristicId,
        byte[] value,
        TimeSpan? timeout = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(clientId);
        ArgumentNullException.ThrowIfNull(value);

        var service = GetServiceOrDefault(serviceId) ?? throw new InvalidOperationException($"Service with ID {serviceId} not found.");

        await NativeNotifyClientAsync(clientId, service, characteristicId, value, timeout, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Platform-specific implementation for notifying a specific client.
    /// </summary>
    /// <param name="clientId">The client identifier to notify.</param>
    /// <param name="service">The service containing the characteristic.</param>
    /// <param name="characteristicId">The UUID of the characteristic.</param>
    /// <param name="value">The value to send.</param>
    /// <param name="timeout">The timeout for this operation.</param>
    /// <param name="cancellationToken">A cancellation token to cancel this operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    protected abstract Task NativeNotifyClientAsync(
        string clientId,
        IBluetoothService service,
        Guid characteristicId,
        byte[] value,
        TimeSpan? timeout,
        CancellationToken cancellationToken);

    /// <inheritdoc />
    public async Task DisconnectClientAsync(string clientId, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(clientId);

        await NativeDisconnectClientAsync(clientId, timeout, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Platform-specific implementation for disconnecting a client.
    /// </summary>
    /// <param name="clientId">The identifier of the client to disconnect.</param>
    /// <param name="timeout">The timeout for this operation.</param>
    /// <param name="cancellationToken">A cancellation token to cancel this operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    protected abstract Task NativeDisconnectClientAsync(string clientId, TimeSpan? timeout, CancellationToken cancellationToken);

    /// <inheritdoc />
    public async Task DisconnectAllClientsAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        var clients = ConnectedClients.ToList();
        foreach (var clientId in clients)
        {
            await DisconnectClientAsync(clientId, timeout, cancellationToken).ConfigureAwait(false);
        }
    }

    /// <inheritdoc/>
    public event EventHandler<CharacteristicReadRequestEventArgs>? CharacteristicReadRequested;

    /// <inheritdoc/>
    public event EventHandler<CharacteristicWriteRequestEventArgs>? CharacteristicWriteRequested;

    /// <inheritdoc/>
    public event EventHandler<CharacteristicSubscriptionChangedEventArgs>? CharacteristicSubscriptionChanged;

    /// <summary>
    /// Raises the <see cref="CharacteristicReadRequested"/> event.
    /// </summary>
    /// <param name="e">The event arguments.</param>
    protected virtual void OnCharacteristicReadRequested(CharacteristicReadRequestEventArgs e)
    {
        CharacteristicReadRequested?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the <see cref="CharacteristicWriteRequested"/> event.
    /// </summary>
    /// <param name="e">The event arguments.</param>
    protected virtual void OnCharacteristicWriteRequested(CharacteristicWriteRequestEventArgs e)
    {
        CharacteristicWriteRequested?.Invoke(this, e);
    }

    /// <summary>
    /// Raises the <see cref="CharacteristicSubscriptionChanged"/> event.
    /// </summary>
    /// <param name="e">The event arguments.</param>
    protected virtual void OnCharacteristicSubscriptionChanged(CharacteristicSubscriptionChangedEventArgs e)
    {
        CharacteristicSubscriptionChanged?.Invoke(this, e);
    }
}
