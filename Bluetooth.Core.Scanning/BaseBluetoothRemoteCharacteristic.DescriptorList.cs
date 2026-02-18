using System.Collections.Specialized;

using Plugin.BaseTypeExtensions;

namespace Bluetooth.Core.Scanning;

public abstract partial class BaseBluetoothRemoteCharacteristic
{
    /// <summary>
    /// Gets the collection of descriptors associated with this characteristic.
    /// </summary>
    private ObservableCollection<IBluetoothRemoteDescriptor> Descriptors
    {
        get
        {
            if (field == null)
            {
                field = [];
                field.CollectionChanged += DescriptorsOnCollectionChanged;
            }

            return field;
        }
    }

    #region Descriptors - Events

    /// <summary>
    /// Handles collection change notifications for the <see cref="Descriptors"/> collection.
    /// </summary>
    private void DescriptorsOnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs ea)
    {
        var listChangedEventArgs = new DescriptorListChangedEventArgs(ea);
        if (listChangedEventArgs.AddedItems != null)
        {
            DescriptorsAdded?.Invoke(this, new DescriptorsAddedEventArgs(listChangedEventArgs.AddedItems));
        }
        if (listChangedEventArgs.RemovedItems != null)
        {
            DescriptorsRemoved?.Invoke(this, new DescriptorsRemovedEventArgs(listChangedEventArgs.RemovedItems));
        }
        DescriptorListChanged?.Invoke(this, listChangedEventArgs);
    }

    /// <inheritdoc/>
    public event EventHandler<DescriptorListChangedEventArgs>? DescriptorListChanged;

    /// <inheritdoc/>
    public event EventHandler<DescriptorsAddedEventArgs>? DescriptorsAdded;

    /// <inheritdoc/>
    public event EventHandler<DescriptorsRemovedEventArgs>? DescriptorsRemoved;

    #endregion

    #region Descriptors - Exploration

    /// <summary>
    /// Gets a value indicating whether a descriptor exploration operation is currently in progress.
    /// </summary>
    public bool IsExploringDescriptors
    {
        get => GetValue(false);
        private set => SetValue(value);
    }

    private TaskCompletionSource? DescriptorsExplorationTcs
    {
        get => GetValue<TaskCompletionSource?>(null);
        set => SetValue(value);
    }

    /// <inheritdoc/>
    public async Task ExploreDescriptorsAsync(Bluetooth.Abstractions.Scanning.Options.DescriptorExplorationOptions? options = null, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        // Use default options if none provided
        options ??= new Bluetooth.Abstractions.Scanning.Options.DescriptorExplorationOptions();

        // If caching enabled and descriptors already exist, skip exploration
        if (options.UseCache && Descriptors.Any())
        {
            return;
        }

        // Prevent multiple concurrent exploration calls
        if (DescriptorsExplorationTcs is { Task.IsCompleted: false })
        {
            await DescriptorsExplorationTcs.Task.ConfigureAwait(false);
            return;
        }

        DescriptorsExplorationTcs = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        IsExploringDescriptors = true;

        try
        {
            await NativeDescriptorsExplorationAsync(timeout, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception e)
        {
            OnDescriptorsExplorationFailed(e);
        }

        try
        {
            await DescriptorsExplorationTcs.Task.WaitBetterAsync(timeout, cancellationToken).ConfigureAwait(false);

            // If a descriptor UUID filter is specified, remove non-matching descriptors
            if (options.DescriptorUuidFilter != null)
            {
                // Create a list of descriptors to remove (those that don't match the filter)
                var descriptorsToRemove = Descriptors.Where(d => !options.DescriptorUuidFilter(d.Id)).ToList();

                foreach (var descriptor in descriptorsToRemove)
                {
                    lock (Descriptors)
                    {
                        Descriptors.Remove(descriptor);
                    }
                }
            }
        }
        finally
        {
            IsExploringDescriptors = false;
            DescriptorsExplorationTcs = null;
        }
    }

    /// <summary>
    /// Platform-specific implementation to explore (discover) descriptors.
    /// </summary>
    protected abstract ValueTask NativeDescriptorsExplorationAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Called when descriptor exploration succeeds.
    /// </summary>
    protected void OnDescriptorsExplorationSucceeded<TNativeDescriptorType>(IList<TNativeDescriptorType> descriptors,
        Func<TNativeDescriptorType, IBluetoothRemoteDescriptor> fromInputTypeToOutputTypeConversion,
        Func<TNativeDescriptorType, IBluetoothRemoteDescriptor, bool> areRepresentingTheSameObject)
    {
        Descriptors.UpdateFrom(descriptors, areRepresentingTheSameObject, fromInputTypeToOutputTypeConversion);

        // Attempt to dispatch success to the TaskCompletionSource
        var success = DescriptorsExplorationTcs?.TrySetResult() ?? false;
        if (success)
        {
            return;
        }

        // Else throw an exception
        throw new UnexpectedDescriptorExplorationException(this);
    }

    /// <summary>
    /// Called when descriptor exploration fails.
    /// </summary>
    protected void OnDescriptorsExplorationFailed(Exception e)
    {
        var success = DescriptorsExplorationTcs?.TrySetException(e) ?? false;
        if (success)
        {
            return;
        }

        BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, e);
    }

    #endregion

    #region Descriptors - Get

    private readonly static Func<IBluetoothRemoteDescriptor, bool> _defaultAcceptAllFilter = _ => true;

    /// <inheritdoc/>
    public IBluetoothRemoteDescriptor GetDescriptor(Func<IBluetoothRemoteDescriptor, bool> filter)
    {
        try
        {
            return Descriptors.SingleOrDefault(filter) ?? throw new DescriptorNotFoundException(this);
        }
        catch (InvalidOperationException e)
        {
            throw new MultipleDescriptorsFoundException(this, Descriptors.Where(filter).ToList(), e);
        }
    }

    /// <inheritdoc/>
    public IBluetoothRemoteDescriptor GetDescriptor(Guid id)
    {
        try
        {
            return Descriptors.SingleOrDefault(d => d.Id == id) ?? throw new DescriptorNotFoundException(this, id);
        }
        catch (InvalidOperationException e)
        {
            throw new MultipleDescriptorsFoundException(this, id, Descriptors.Where(d => d.Id == id).ToList(), e);
        }
    }

    /// <inheritdoc/>
    public IBluetoothRemoteDescriptor? GetDescriptorOrDefault(Guid id)
    {
        try
        {
            return Descriptors.SingleOrDefault(s => s.Id == id);
        }
        catch (InvalidOperationException e)
        {
            throw new MultipleDescriptorsFoundException(this, id, Descriptors.Where(s => s.Id == id).ToList(), e);
        }
    }

    /// <inheritdoc/>
    public IBluetoothRemoteDescriptor? GetDescriptorOrDefault(Func<IBluetoothRemoteDescriptor, bool> filter)
    {
        try
        {
            return Descriptors.SingleOrDefault(filter);
        }
        catch (InvalidOperationException e)
        {
            throw new MultipleDescriptorsFoundException(this, Descriptors.Where(filter).ToList(), e);
        }
    }

    /// <inheritdoc/>
    public IEnumerable<IBluetoothRemoteDescriptor> GetDescriptors(Func<IBluetoothRemoteDescriptor, bool>? filter = null)
    {
        filter ??= _defaultAcceptAllFilter;
        return Descriptors.Where(filter).ToList();
    }

    #endregion

    /// <inheritdoc/>
    public async ValueTask ClearDescriptorsAsync()
    {
        foreach (var descriptor in Descriptors)
        {
            await descriptor.DisposeAsync().ConfigureAwait(false);
        }

        lock (Descriptors)
        {
            Descriptors.Clear();
        }
    }

    /// <inheritdoc/>
    public bool HasDescriptor(Func<IBluetoothRemoteDescriptor, bool> filter)
    {
        return Descriptors.Any(filter);
    }

    /// <inheritdoc/>
    public bool HasDescriptor(Guid id)
    {
        return Descriptors.Any(d => d.Id == id);
    }

}
