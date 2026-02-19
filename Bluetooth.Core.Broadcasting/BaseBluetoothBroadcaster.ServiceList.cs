namespace Bluetooth.Core.Broadcasting;

public abstract partial class BaseBluetoothBroadcaster
{
    private ObservableCollection<IBluetoothLocalService> Services
    {
        get
        {
            if (field == null)
            {
                field = [];
                field.CollectionChanged += ServicesOnCollectionChanged;
            }
            return field;
        }
    }

    #region Services - Events

    private void ServicesOnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs ea)
    {
        var listChangedEventArgs = new ServiceListChangedEventArgs(ea);
        if (listChangedEventArgs.AddedItems != null)
        {
            ServicesAdded?.Invoke(this, new ServicesAddedEventArgs(listChangedEventArgs.AddedItems));
        }
        if (listChangedEventArgs.RemovedItems != null)
        {
            ServicesRemoved?.Invoke(this, new ServicesRemovedEventArgs(listChangedEventArgs.RemovedItems));
        }
        ServiceListChanged?.Invoke(this, listChangedEventArgs);
    }

    /// <inheritdoc/>
    public event EventHandler<ServicesAddedEventArgs>? ServicesAdded;

    /// <inheritdoc/>
    public event EventHandler<ServicesRemovedEventArgs>? ServicesRemoved;

    /// <inheritdoc/>
    public event EventHandler<ServiceListChangedEventArgs>? ServiceListChanged;

    #endregion

    #region Services - Add

    /// <inheritdoc/>
    public ValueTask<IBluetoothLocalService> CreateServiceAsync(IBluetoothLocalServiceFactory.BluetoothLocalServiceSpec request, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var existingService = GetServiceOrDefault(request.Id);
        if (existingService != null)
        {
            LogServiceAlreadyExists(request.Id);
            throw new ServiceAlreadyExistsException(this, request.Id, existingService);
        }

        LogAddingService(request.Id);
        var newService = LocalServiceFactory.CreateService(this, request);
        Services.Add(newService);
        LogServiceAdded(request.Id);

        return new ValueTask<IBluetoothLocalService>(newService);
    }

    #endregion

    #region Services - Get

    /// <inheritdoc/>
    public IBluetoothLocalService GetService(Func<IBluetoothLocalService, bool> filter)
    {
        return GetServiceOrDefault(filter) ?? throw new ServiceNotFoundException(this);
    }

    /// <inheritdoc/>
    public IBluetoothLocalService GetService(Guid id)
    {
        var service = GetServiceOrDefault(id);
        if (service == null)
        {
            LogServiceNotFound(id);
            throw new ServiceNotFoundException(this, id);
        }
        return service;
    }

    /// <inheritdoc/>
    public IBluetoothLocalService? GetServiceOrDefault(Func<IBluetoothLocalService, bool> filter)
    {
        lock (Services)
        {
            try
            {
                return Services.SingleOrDefault(filter);
            }
            catch (InvalidOperationException e) when (e.Message.Contains("more than one"))
            {
                throw new MultipleServicesFoundException(this, Services.Where(filter).ToArray(), e);
            }
        }
    }

    /// <inheritdoc/>
    public IBluetoothLocalService? GetServiceOrDefault(Guid id)
    {
        lock (Services)
        {
            try
            {
                return Services.SingleOrDefault(service => service.Id == id);
            }
            catch (InvalidOperationException e) when (e.Message.Contains("more than one"))
            {
                throw new MultipleServicesFoundException(this, id, Services.Where(service => service.Id == id).ToArray(), e);
            }
        }
    }

    /// <inheritdoc/>
    public IReadOnlyList<IBluetoothLocalService> GetServices(Func<IBluetoothLocalService, bool>? filter = null)
    {
        filter ??= _ => true;

        lock (Services)
        {
            return Services.Where(filter).ToList();
        }
    }

    #endregion

    #region Services - Remove

    /// <inheritdoc/>
    public ValueTask RemoveServiceAsync(Guid id, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        var service = GetService(id);
        return RemoveServiceAsync(service, timeout, cancellationToken);
    }

    /// <inheritdoc/>
    public async ValueTask RemoveServiceAsync(IBluetoothLocalService localService, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(localService);

        LogRemovingService(localService.Id);
        Services.Remove(localService);
        await localService.DisposeAsync().ConfigureAwait(false);
        LogServiceRemoved(localService.Id);
    }

    /// <inheritdoc/>
    public async ValueTask RemoveAllServicesAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        var serviceList = Services.ToList();
        var serviceCount = serviceList.Count;

        LogClearingServices();

        foreach (var service in serviceList)
        {
            await RemoveServiceAsync(service, timeout, cancellationToken).ConfigureAwait(false);
        }

        LogServicesCleared(serviceCount);
    }

    #endregion

    #region Services - Has

    /// <inheritdoc/>
    public bool HasService(Func<IBluetoothLocalService, bool> filter)
    {
        lock (Services)
        {
            return Services.Any(filter);
        }
    }

    /// <inheritdoc/>
    public bool HasService(Guid id)
    {
        return HasService(service => service.Id == id);
    }

    #endregion

}
