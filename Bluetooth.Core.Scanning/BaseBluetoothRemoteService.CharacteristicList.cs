namespace Bluetooth.Core.Scanning;

public abstract partial class BaseBluetoothRemoteService
{
    /// <summary>
    ///     Gets the collection of characteristics associated with this Bluetooth service.
    /// </summary>
    /// <remarks>
    ///     This collection is lazily initialized and automatically hooks up collection change notifications
    ///     to raise the appropriate events (<see cref="CharacteristicsAdded" />, <see cref="CharacteristicsRemoved" />, <see cref="CharacteristicListChanged" />).
    /// </remarks>
    private ObservableCollection<IBluetoothRemoteCharacteristic> Characteristics
    {
        get
        {
            if (field == null)
            {
                field = [];
                field.CollectionChanged += CharacteristicsOnCollectionChanged;
            }

            return field;
        }
    }

    #region Characteristics - Clear

    /// <inheritdoc />
    public async ValueTask ClearCharacteristicsAsync()
    {
        var characteristicCount = Characteristics.Count;

        foreach (var characteristic in Characteristics)
        {
            await characteristic.DisposeAsync().ConfigureAwait(false);
        }

        lock (Characteristics)
        {
            Characteristics.Clear();
        }

        LogCharacteristicsCleared(Id, Device.Id, characteristicCount);
    }

    #endregion

    #region Characteristics - Events

    /// <summary>
    ///     Handles collection change notifications for the <see cref="Characteristics" /> collection.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="ea">Event arguments containing the collection change details.</param>
    private void CharacteristicsOnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs ea)
    {
        var listChangedEventArgs = new CharacteristicListChangedEventArgs(ea);
        if (listChangedEventArgs.AddedItems != null)
        {
            CharacteristicsAdded?.Invoke(this, new CharacteristicsAddedEventArgs(listChangedEventArgs.AddedItems));
        }

        if (listChangedEventArgs.RemovedItems != null)
        {
            CharacteristicsRemoved?.Invoke(this, new CharacteristicsRemovedEventArgs(listChangedEventArgs.RemovedItems));
        }

        CharacteristicListChanged?.Invoke(this, listChangedEventArgs);
    }

    /// <inheritdoc />
    public event EventHandler<CharacteristicsAddedEventArgs>? CharacteristicsAdded;

    /// <inheritdoc />
    public event EventHandler<CharacteristicsRemovedEventArgs>? CharacteristicsRemoved;

    /// <inheritdoc />
    public event EventHandler<CharacteristicListChangedEventArgs>? CharacteristicListChanged;

    #endregion

    #region Characteristics - Exploration

    /// <summary>
    ///     Gets a value indicating whether characteristic exploration is currently in progress.
    /// </summary>
    public bool IsExploringCharacteristics
    {
        get => GetValue(false);
        private set => SetValue(value);
    }

    /// <summary>
    ///     Gets or sets the task completion source for characteristic exploration operations.
    /// </summary>
    /// <remarks>
    ///     This is used to coordinate asynchronous characteristic exploration and ensure only one exploration occurs at a time.
    /// </remarks>
    private TaskCompletionSource? CharacteristicsExplorationTcs
    {
        get => GetValue<TaskCompletionSource?>(null);
        set => SetValue(value);
    }

    /// <summary>
    ///     Explores descriptors for all characteristics matching the filter.
    /// </summary>
    private async Task ExploreDescriptorsForCharacteristicsAsync(CharacteristicExplorationOptions options, TimeSpan? timeout, CancellationToken cancellationToken)
    {
        var descriptorOptions = new DescriptorExplorationOptions
        {
            UseCache = options.UseCache
        };

        var filteredCharacteristicCount = Characteristics.Count(c => options.CharacteristicUuidFilter == null || options.CharacteristicUuidFilter(c.Id));
        if (filteredCharacteristicCount > 0)
        {
            LogCascadingToDescriptors(Id, Device.Id, filteredCharacteristicCount);
        }

        foreach (var characteristic in Characteristics.ToList())
        {
            if (ShouldSkipCharacteristic(characteristic, options))
            {
                continue;
            }

            await characteristic.ExploreDescriptorsAsync(descriptorOptions, timeout, cancellationToken).ConfigureAwait(false);
        }
    }

    /// <summary>
    ///     Determines if a characteristic should be skipped based on the filter.
    /// </summary>
    private static bool ShouldSkipCharacteristic(IBluetoothRemoteCharacteristic characteristic, CharacteristicExplorationOptions options)
    {
        return options.CharacteristicUuidFilter != null && !options.CharacteristicUuidFilter(characteristic.Id);
    }

    /// <summary>
    ///     Handles characteristic exploration when cached characteristics are available.
    /// </summary>
    /// <returns>True if cached characteristics were used, false if fresh exploration is needed.</returns>
    private async Task<bool> TryUseCachedCharacteristicsAsync(CharacteristicExplorationOptions options, TimeSpan? timeout, CancellationToken cancellationToken)
    {
        if (!options.UseCache || !Characteristics.Any())
        {
            return false;
        }

        LogUsingCachedCharacteristics(Id, Device.Id, Characteristics.Count);

        if (options.ExploreDescriptors)
        {
            await ExploreDescriptorsForCharacteristicsAsync(options, timeout, cancellationToken).ConfigureAwait(false);
        }

        return true;
    }

    /// <summary>
    ///     Waits for an ongoing characteristic exploration to complete.
    /// </summary>
    /// <returns>True if waiting for an ongoing exploration, false if no exploration is in progress.</returns>
    private async Task<bool> TryAwaitOngoingCharacteristicExplorationAsync()
    {
        if (CharacteristicsExplorationTcs is { Task.IsCompleted: false })
        {
            LogMergingCharacteristicExploration(Id, Device.Id);
            await CharacteristicsExplorationTcs.Task.ConfigureAwait(false);
            return true;
        }

        return false;
    }

    /// <summary>
    ///     Performs the native characteristic exploration and waits for completion.
    /// </summary>
    private async Task PerformCharacteristicExplorationAsync(TimeSpan? timeout, CancellationToken cancellationToken)
    {
        DeviceNotConnectedException.ThrowIfNotConnected(Device);

        LogExploringCharacteristics(Id, Device.Id);
        await NativeCharacteristicsExplorationAsync(timeout, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    ///     Called when characteristic exploration succeeds. Updates the Characteristics collection and completes the exploration task.
    /// </summary>
    /// <typeparam name="TNativeCharacteristicType">The platform-specific characteristic type.</typeparam>
    /// <param name="characteristics">The list of native characteristics discovered.</param>
    /// <param name="areRepresentingTheSameObject">Function to determine if a native characteristic and IBluetoothCharacteristic represent the same object.</param>
    /// <param name="fromInputTypeToOutputTypeConversion">Function to convert from native characteristic type to IBluetoothCharacteristic.</param>
    /// <exception cref="UnexpectedCharacteristicExplorationException">Thrown when the task completion source is not in the expected state.</exception>
    protected void OnCharacteristicsExplorationSucceeded<TNativeCharacteristicType>(IList<TNativeCharacteristicType> characteristics,
        Func<TNativeCharacteristicType, IBluetoothRemoteCharacteristic, bool> areRepresentingTheSameObject,
        Func<TNativeCharacteristicType, IBluetoothRemoteCharacteristic> fromInputTypeToOutputTypeConversion)
    {
        Characteristics.UpdateFrom(characteristics, areRepresentingTheSameObject, fromInputTypeToOutputTypeConversion);

        LogCharacteristicExplorationSucceeded(Id, Device.Id, Characteristics.Count);

        // Attempt to dispatch success to the TaskCompletionSource
        var success = CharacteristicsExplorationTcs?.TrySetResult() ?? false;
        if (success)
        {
            return;
        }

        // Else throw an exception
        LogUnexpectedCharacteristicExploration(Id, Device.Id);
        throw new UnexpectedCharacteristicExplorationException(this);
    }

    /// <summary>
    ///     Called when characteristic exploration fails. Completes the exploration task with an exception or dispatches to the unhandled exception listener.
    /// </summary>
    /// <param name="e">The exception that occurred during characteristic exploration.</param>
    /// <remarks>
    ///     If the task completion source accepts the exception, it is propagated to waiting tasks.
    ///     Otherwise, the exception is dispatched to the <see cref="BluetoothUnhandledExceptionListener" />.
    /// </remarks>
    protected void OnCharacteristicsExplorationFailed(Exception e)
    {
        LogCharacteristicExplorationFailed(Id, Device.Id, e);

        // Attempt to dispatch exception to the TaskCompletionSource
        var success = CharacteristicsExplorationTcs?.TrySetException(e) ?? false;
        if (success)
        {
            return;
        }

        // If the TaskCompletionSource was already completed, dispatch the exception to the listener
        BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, e);
    }

    /// <summary>
    ///     Platform-specific implementation to explore characteristics.
    /// </summary>
    /// <param name="timeout">Optional timeout for the operation.</param>
    /// <param name="cancellationToken">Optional cancellation token for the operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    protected abstract ValueTask NativeCharacteristicsExplorationAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default);

    /// <inheritdoc />
    /// <remarks>
    ///     This method ensures the device is connected, prevents concurrent explorations,
    ///     and optionally clears existing characteristics before exploring.
    /// </remarks>
    /// <exception cref="DeviceNotConnectedException">Thrown when the device is not connected.</exception>
    public async ValueTask ExploreCharacteristicsAsync(CharacteristicExplorationOptions? options = null, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        options ??= new CharacteristicExplorationOptions();

        // Check if we can use cached characteristics
        if (await TryUseCachedCharacteristicsAsync(options, timeout, cancellationToken).ConfigureAwait(false))
        {
            return;
        }

        // Check if exploration is already in progress
        if (await TryAwaitOngoingCharacteristicExplorationAsync().ConfigureAwait(false))
        {
            return;
        }

        // Start new exploration
        CharacteristicsExplorationTcs = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        IsExploringCharacteristics = true;

        try
        {
            await PerformCharacteristicExplorationAsync(timeout, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception e)
        {
            OnCharacteristicsExplorationFailed(e);
        }

        try
        {
            await CharacteristicsExplorationTcs.Task.WaitBetterAsync(timeout, cancellationToken).ConfigureAwait(false);

            if (options.ExploreDescriptors)
            {
                await ExploreDescriptorsForCharacteristicsAsync(options, timeout, cancellationToken).ConfigureAwait(false);
            }
        }
        finally
        {
            IsExploringCharacteristics = false;
            CharacteristicsExplorationTcs = null;
        }
    }

    #endregion

    #region Characteristics - Get

    private readonly static Func<IBluetoothRemoteCharacteristic, bool> _defaultAcceptAllFilter = _ => true;

    /// <inheritdoc />
    public IBluetoothRemoteCharacteristic GetCharacteristic(Func<IBluetoothRemoteCharacteristic, bool> filter)
    {
        return GetCharacteristicOrDefault(filter) ?? throw new CharacteristicNotFoundException(this);
    }

    /// <inheritdoc />
    public IBluetoothRemoteCharacteristic GetCharacteristic(Guid id)
    {
        return GetCharacteristicOrDefault(id) ?? throw new CharacteristicNotFoundException(this, id);
    }

    /// <inheritdoc />
    public IBluetoothRemoteCharacteristic? GetCharacteristicOrDefault(Func<IBluetoothRemoteCharacteristic, bool> filter)
    {
        try
        {
            return Characteristics.SingleOrDefault(filter);
        }
        catch (InvalidOperationException e)
        {
            throw new MultipleCharacteristicsFoundException(this, Characteristics.Where(filter).ToList(), e);
        }
    }

    /// <inheritdoc />
    public IBluetoothRemoteCharacteristic? GetCharacteristicOrDefault(Guid id)
    {
        try
        {
            return Characteristics.SingleOrDefault(s => s.Id == id);
        }
        catch (InvalidOperationException e)
        {
            throw new MultipleCharacteristicsFoundException(this, id, Characteristics.Where(s => s.Id == id).ToList(), e);
        }
    }

    /// <inheritdoc />
    public IEnumerable<IBluetoothRemoteCharacteristic> GetCharacteristics(Func<IBluetoothRemoteCharacteristic, bool>? filter = null)
    {
        filter ??= _defaultAcceptAllFilter;

        lock (Characteristics)
        {
            foreach (var characteristic in Characteristics)
            {
                if (filter.Invoke(characteristic))
                {
                    yield return characteristic;
                }
            }
        }
    }

    #endregion

    #region Characteristics - Has

    /// <inheritdoc />
    public bool HasCharacteristic(Func<IBluetoothRemoteCharacteristic, bool> filter)
    {
        return Characteristics.Any(filter);
    }

    /// <inheritdoc />
    public bool HasCharacteristic(Guid id)
    {
        return HasCharacteristic(characteristic => characteristic.Id == id);
    }

    #endregion
}
