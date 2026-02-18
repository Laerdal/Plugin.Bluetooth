using System.Collections.Specialized;

using Plugin.BaseTypeExtensions;

namespace Bluetooth.Core.Scanning;

public abstract partial class BaseBluetoothRemoteDevice
{
    /// <summary>
    /// The services' collection.
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

    #region Services - Exploration

    /// <summary>
    /// Gets a value indicating whether service exploration is currently in progress.
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
    /// Called when service exploration succeeds. Updates the Services collection and completes the exploration task.
    /// </summary>
    /// <typeparam name="TNativeServiceType">The platform-specific service type.</typeparam>
    /// <param name="services">The list of native services discovered.</param>
    /// <param name="areRepresentingTheSameObject">Function to determine if a native service and IBluetoothService represent the same object.</param>
    /// <param name="fromInputTypeToOutputTypeConversion">Function to convert from native service type to IBluetoothService.</param>
    protected void OnServicesExplorationSucceeded<TNativeServiceType>(IList<TNativeServiceType> services, Func<TNativeServiceType, IBluetoothRemoteService, bool> areRepresentingTheSameObject, Func<TNativeServiceType, IBluetoothRemoteService> fromInputTypeToOutputTypeConversion)
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
    /// Called when service exploration fails. Completes the exploration task with an exception or dispatches to the unhandled exception listener.
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
    /// Platform-specific implementation to explore services.
    /// </summary>
    protected abstract ValueTask NativeServicesExplorationAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default);

    /// <inheritdoc/>
    public async Task ExploreServicesAsync(bool exploreCharacteristicsToo = false, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        // Prevents multiple calls to ReadValueAsync, if already exploring, we merge the calls
        if (ServicesExplorationTcs is { Task.IsCompleted: false })
        {
            LogMergingServiceExploration(Id);
            await ServicesExplorationTcs.Task.ConfigureAwait(false);
            return;
        }

        ServicesExplorationTcs = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously); // Reset the TCS
        IsExploringServices = true; // Set the flag to true

        try // try-catch to dispatch exceptions rising from start
        {
            // Ensure Device is Connected
            if(!IsConnected)
            {
                LogServiceExplorationNotConnected(Id);
                throw new DeviceNotConnectedException(this, "Device must be connected to explore services.");
            }

            LogExploringServices(Id);
            await NativeServicesExplorationAsync(timeout, cancellationToken).ConfigureAwait(false); // actual service exploration native call
        }
        catch (Exception e)
        {
            OnServicesExplorationFailed(e); // if exception is thrown during start, we trigger the failure
        }

        // try-finally to ensure disposal and release of resources
        try
        {
            // Wait for OnServicesExplorationSucceeded to be called
            await ServicesExplorationTcs.Task.WaitBetterAsync(timeout, cancellationToken).ConfigureAwait(false);

            if (exploreCharacteristicsToo) // EXPLORE CHARACTERISTICS IF NEEDED
            {
                LogCascadingToCharacteristics(Id, Services.Count);
                foreach (var service in Services.ToList())
                {
                    await service.ExploreCharacteristicsAsync(timeout, cancellationToken).ConfigureAwait(false);
                }
            }
        }
        finally
        {
            IsExploringServices = false; // Reset the flag
            ServicesExplorationTcs = null;
        }
    }

    /// <inheritdoc/>
    public async Task ExploreServicesIfNeededAsync(bool exploreCharacteristicsToo = false, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        // Check if services have already been explored
        if (Services.Any())
        {
            LogUsingCachedServices(Id, Services.Count);
            // Services already exist, but we may still need to explore characteristics
            if (exploreCharacteristicsToo)
            {
                LogCascadingToCharacteristics(Id, Services.Count);
                foreach (var service in Services.ToList())
                {
                    await service.ExploreCharacteristicsIfNeededAsync(timeout, cancellationToken).ConfigureAwait(false);
                }
            }
            return;
        }

        // No services exist yet, perform full exploration
        await ExploreServicesAsync(exploreCharacteristicsToo, timeout, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task ExploreServicesAsync(Bluetooth.Abstractions.Scanning.Options.ServiceExplorationOptions? options, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        // Use default options if none provided
        options ??= new Bluetooth.Abstractions.Scanning.Options.ServiceExplorationOptions();

        // If caching enabled and services already exist, handle cascading exploration only
        if (options.UseCache && Services.Any())
        {
            LogUsingCachedServices(Id, Services.Count);
            // Services already explored, but may need to explore children
            if (options.ExploreCharacteristics)
            {
                var characteristicOptions = new Bluetooth.Abstractions.Scanning.Options.CharacteristicExplorationOptions
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
                    // Apply service filter if present
                    if (options.ServiceUuidFilter != null && !options.ServiceUuidFilter(service.Id))
                    {
                        continue;
                    }

                    await service.ExploreCharacteristicsAsync(characteristicOptions, timeout, cancellationToken).ConfigureAwait(false);
                }
            }
            return;
        }

        // Prevent multiple concurrent exploration calls
        if (ServicesExplorationTcs is { Task.IsCompleted: false })
        {
            LogMergingServiceExploration(Id);
            await ServicesExplorationTcs.Task.ConfigureAwait(false);
            return;
        }

        ServicesExplorationTcs = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        IsExploringServices = true;

        try
        {
            // Ensure device is connected
            if (!IsConnected)
            {
                LogServiceExplorationNotConnected(Id);
                throw new DeviceNotConnectedException(this, "Device must be connected to explore services.");
            }

            LogExploringServices(Id);
            await NativeServicesExplorationAsync(timeout, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception e)
        {
            OnServicesExplorationFailed(e);
        }

        try
        {
            // Wait for exploration to complete
            await ServicesExplorationTcs.Task.WaitBetterAsync(timeout, cancellationToken).ConfigureAwait(false);

            // Cascade to characteristics if requested
            if (options.ExploreCharacteristics)
            {
                var characteristicOptions = new Bluetooth.Abstractions.Scanning.Options.CharacteristicExplorationOptions
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
                    // Apply service filter if present
                    if (options.ServiceUuidFilter != null && !options.ServiceUuidFilter(service.Id))
                    {
                        continue;
                    }

                    await service.ExploreCharacteristicsAsync(characteristicOptions, timeout, cancellationToken).ConfigureAwait(false);
                }
            }
        }
        finally
        {
            IsExploringServices = false;
            ServicesExplorationTcs = null;
        }
    }

    #endregion

    #region Services - Clear

    /// <summary>
    /// Clears all services and their characteristics, disposing of them properly.
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

    #region Services - Get

    private readonly static Func<IBluetoothRemoteService, bool> _defaultAcceptAllFilter = _ => true;

    /// <inheritdoc/>
    public IBluetoothRemoteService GetService(Func<IBluetoothRemoteService, bool> filter)
    {
        return GetServiceOrDefault(filter) ?? throw new ServiceNotFoundException(this);
    }

    /// <inheritdoc/>
    public IBluetoothRemoteService GetService(Guid id)
    {
        return GetServiceOrDefault(id) ?? throw new ServiceNotFoundException(this, id);
    }

    /// <inheritdoc/>
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

    /// <inheritdoc/>
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

    /// <inheritdoc/>
    public IEnumerable<IBluetoothRemoteService> GetServices(Func<IBluetoothRemoteService, bool>? filter = null)
    {
        filter ??= _defaultAcceptAllFilter;

        lock (Services)
        {
            foreach (var service in Services)
            {
                if (filter.Invoke(service))
                {
                    yield return service;
                }
            }
        }
    }

    #endregion

    #region Services - Has

    /// <inheritdoc/>
    public bool HasService(Func<IBluetoothRemoteService, bool>? filter = null)
    {
        filter ??= _defaultAcceptAllFilter;
        return Services.Any(filter);
    }

    /// <inheritdoc/>
    public bool HasService(Guid id)
    {
        return HasService(d => d.Id == id);
    }

    #endregion
}
