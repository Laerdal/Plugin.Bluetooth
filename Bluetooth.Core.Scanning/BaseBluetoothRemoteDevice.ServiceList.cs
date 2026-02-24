namespace Bluetooth.Core.Scanning;

public abstract partial class BaseBluetoothRemoteDevice
{
    /// <summary>
    ///     The services' collection.
    /// </summary>
    private ObservableCollection<IBluetoothRemoteService> Services
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

    #region Services - Clear

    /// <summary>
    ///     Clears all services and their characteristics, disposing of them properly.
    /// </summary>
    /// <returns>A task that completes when all services have been cleared and disposed.</returns>
    public async ValueTask ClearServicesAsync()
    {
        var serviceCount = Services.Count;

        foreach (var service in Services)
        {
            await service.DisposeAsync().ConfigureAwait(false);
        }

        lock (Services)
        {
            Services.Clear();
        }

        LogServicesCleared(Id, serviceCount);
    }

    #endregion

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

    /// <inheritdoc />
    public event EventHandler<ServicesAddedEventArgs>? ServicesAdded;

    /// <inheritdoc />
    public event EventHandler<ServicesRemovedEventArgs>? ServicesRemoved;

    /// <inheritdoc />
    public event EventHandler<ServiceListChangedEventArgs>? ServiceListChanged;

    #endregion

    #region Services - Exploration

    /// <summary>
    ///     Gets a value indicating whether service exploration is currently in progress.
    /// </summary>
    public bool IsExploringServices
    {
        get => GetValue(false);
        private set => SetValue(value);
    }

    private TaskCompletionSource? ServicesExplorationTcs
    {
        get => GetValue<TaskCompletionSource?>(null);
        set => SetValue(value);
    }

    /// <summary>
    ///     Explores characteristics for all services matching the filter.
    /// </summary>
    private async Task ExploreCharacteristicsForServicesAsync(ServiceExplorationOptions options, TimeSpan? timeout, CancellationToken cancellationToken)
    {
        var characteristicOptions = new CharacteristicExplorationOptions
        {
            ExploreDescriptors = options.ExploreDescriptors,
            UseCache = options.UseCache
        };

        var filteredServiceCount = Services.Count(s => options.ServiceUuidFilter == null || options.ServiceUuidFilter(s.Id));
        if (filteredServiceCount > 0)
        {
            LogCascadingToCharacteristics(Id, filteredServiceCount);
        }

        foreach (var service in Services.ToList())
        {
            if (ShouldSkipService(service, options))
            {
                continue;
            }

            await service.ExploreCharacteristicsAsync(characteristicOptions, timeout, cancellationToken).ConfigureAwait(false);
        }
    }

    /// <summary>
    ///     Determines if a service should be skipped based on the filter.
    /// </summary>
    private static bool ShouldSkipService(IBluetoothRemoteService service, ServiceExplorationOptions options)
    {
        return options.ServiceUuidFilter != null && !options.ServiceUuidFilter(service.Id);
    }

    /// <summary>
    ///     Handles service exploration when cached services are available.
    /// </summary>
    /// <returns>True if cached services were used, false if fresh exploration is needed.</returns>
    private async Task<bool> TryUseCachedServicesAsync(ServiceExplorationOptions options, TimeSpan? timeout, CancellationToken cancellationToken)
    {
        if (!options.UseCache || !Services.Any())
        {
            return false;
        }

        LogUsingCachedServices(Id, Services.Count);

        if (options.ExploreCharacteristics)
        {
            await ExploreCharacteristicsForServicesAsync(options, timeout, cancellationToken).ConfigureAwait(false);
        }

        return true;
    }

    /// <summary>
    ///     Waits for an ongoing service exploration to complete.
    /// </summary>
    /// <returns>True if waiting for an ongoing exploration, false if no exploration is in progress.</returns>
    private async Task<bool> TryAwaitOngoingExplorationAsync()
    {
        if (ServicesExplorationTcs is { Task.IsCompleted: false })
        {
            LogMergingServiceExploration(Id);
            await ServicesExplorationTcs.Task.ConfigureAwait(false);
            return true;
        }

        return false;
    }

    /// <summary>
    ///     Performs the native service exploration and waits for completion.
    /// </summary>
    private async Task PerformServiceExplorationAsync(TimeSpan? timeout, CancellationToken cancellationToken)
    {
        if (!IsConnected)
        {
            LogServiceExplorationNotConnected(Id);
            throw new DeviceNotConnectedException(this, "Device must be connected to explore services.");
        }

        LogExploringServices(Id);
        await NativeServicesExplorationAsync(timeout, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    ///     Called when service exploration succeeds. Updates the Services collection and completes the exploration task.
    /// </summary>
    /// <typeparam name="TNativeServiceType">The platform-specific service type.</typeparam>
    /// <param name="services">The list of native services discovered.</param>
    /// <param name="areRepresentingTheSameObject">Function to determine if a native service and IBluetoothService represent the same object.</param>
    /// <param name="fromInputTypeToOutputTypeConversion">Function to convert from native service type to IBluetoothService.</param>
    protected void OnServicesExplorationSucceeded<TNativeServiceType>(IList<TNativeServiceType> services, Func<TNativeServiceType, IBluetoothRemoteService, bool> areRepresentingTheSameObject,
        Func<TNativeServiceType, IBluetoothRemoteService> fromInputTypeToOutputTypeConversion)
    {
        Services.UpdateFrom(services, areRepresentingTheSameObject, fromInputTypeToOutputTypeConversion);

        LogServiceExplorationSucceeded(Id, Services.Count);

        // Attempt to dispatch success to the TaskCompletionSource
        var success = ServicesExplorationTcs?.TrySetResult() ?? false;
        if (success)
        {
            return;
        }

        // Else throw an exception
        LogUnexpectedServiceExploration(Id);
        throw new UnexpectedServiceExplorationException(this);
    }

    /// <summary>
    ///     Called when service exploration fails. Completes the exploration task with an exception or dispatches to the unhandled exception listener.
    /// </summary>
    /// <param name="e">The exception that occurred during service exploration.</param>
    protected void OnServicesExplorationFailed(Exception e)
    {
        LogServiceExplorationFailed(Id, e);

        // Attempt to dispatch exception to the TaskCompletionSource
        var success = ServicesExplorationTcs?.TrySetException(e) ?? false;
        if (success)
        {
            return;
        }

        // If the TaskCompletionSource was already completed, dispatch the exception to the listener
        BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, e);
    }

    /// <summary>
    ///     Platform-specific implementation to explore services.
    /// </summary>
    protected abstract ValueTask NativeServicesExplorationAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default);

    /// <inheritdoc />
    public async Task ExploreServicesAsync(ServiceExplorationOptions? options = null, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        options ??= new ServiceExplorationOptions();

        // Check if we can use cached services
        if (await TryUseCachedServicesAsync(options, timeout, cancellationToken).ConfigureAwait(false))
        {
            return;
        }

        // Check if exploration is already in progress
        if (await TryAwaitOngoingExplorationAsync().ConfigureAwait(false))
        {
            return;
        }

        // Start new exploration
        ServicesExplorationTcs = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        IsExploringServices = true;

        try
        {
            await PerformServiceExplorationAsync(timeout, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception e)
        {
            OnServicesExplorationFailed(e);
        }

        try
        {
            await ServicesExplorationTcs.Task.WaitBetterAsync(timeout, cancellationToken).ConfigureAwait(false);

            if (options.ExploreCharacteristics)
            {
                await ExploreCharacteristicsForServicesAsync(options, timeout, cancellationToken).ConfigureAwait(false);
            }
        }
        finally
        {
            IsExploringServices = false;
            ServicesExplorationTcs = null;
        }
    }

    #endregion

    #region Services - Get

    private readonly static Func<IBluetoothRemoteService, bool> _defaultAcceptAllFilter = _ => true;

    /// <inheritdoc />
    public IBluetoothRemoteService GetService(Func<IBluetoothRemoteService, bool> filter)
    {
        return GetServiceOrDefault(filter) ?? throw new ServiceNotFoundException(this);
    }

    /// <inheritdoc />
    public IBluetoothRemoteService GetService(Guid id)
    {
        return GetServiceOrDefault(id) ?? throw new ServiceNotFoundException(this, id);
    }

    /// <inheritdoc />
    public IBluetoothRemoteService? GetServiceOrDefault(Func<IBluetoothRemoteService, bool> filter)
    {
        try
        {
            return Services.SingleOrDefault(filter);
        }
        catch (InvalidOperationException e)
        {
            throw new MultipleServicesFoundException(this, Services.Where(filter).ToList(), e);
        }
    }

    /// <inheritdoc />
    public IBluetoothRemoteService? GetServiceOrDefault(Guid id)
    {
        try
        {
            return Services.SingleOrDefault(s => s.Id == id);
        }
        catch (InvalidOperationException e)
        {
            throw new MultipleServicesFoundException(this, id, Services.Where(s => s.Id == id).ToList(), e);
        }
    }

    /// <inheritdoc />
    public IReadOnlyList<IBluetoothRemoteService> GetServices(Func<IBluetoothRemoteService, bool>? filter = null)
    {
        filter ??= _defaultAcceptAllFilter;

        lock (Services)
        {
            // Materialize immediately while holding lock
            return Services.Where(filter).ToList();
        }
    }

    #endregion

    #region Services - Has

    /// <inheritdoc />
    public bool HasService(Func<IBluetoothRemoteService, bool>? filter = null)
    {
        filter ??= _defaultAcceptAllFilter;
        return Services.Any(filter);
    }

    /// <inheritdoc />
    public bool HasService(Guid id)
    {
        return HasService(d => d.Id == id);
    }

    #endregion
}
