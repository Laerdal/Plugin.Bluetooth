namespace Bluetooth.Unity;

/// <summary>
///     Unified Bluetooth remote characteristic facade providing extension points for client customization.
/// </summary>
public class BluetoothRemoteCharacteristic : IBluetoothRemoteCharacteristic
{
    private readonly IBluetoothRemoteCharacteristic _platformCharacteristic;

    private readonly Dictionary<IBluetoothRemoteDescriptor, IBluetoothRemoteDescriptor> _descriptorWrappers = new();

    /// <summary>
    ///     Initializes a new instance of the <see cref="BluetoothRemoteCharacteristic" /> class.
    /// </summary>
    /// <param name="platformCharacteristic">The platform characteristic to wrap.</param>
    /// <param name="service">The wrapped parent service.</param>
    public BluetoothRemoteCharacteristic(IBluetoothRemoteCharacteristic platformCharacteristic, IBluetoothRemoteService service)
    {
        ArgumentNullException.ThrowIfNull(platformCharacteristic);
        ArgumentNullException.ThrowIfNull(service);

        _platformCharacteristic = platformCharacteristic;
        Service = service;

        _platformCharacteristic.ValueUpdated += (sender, args) => ValueUpdated?.Invoke(this, args);
        _platformCharacteristic.DescriptorListChanged += (sender, args) => DescriptorListChanged?.Invoke(this, args);
        _platformCharacteristic.DescriptorsAdded += (sender, args) => DescriptorsAdded?.Invoke(this, args);
        _platformCharacteristic.DescriptorsRemoved += (sender, args) => DescriptorsRemoved?.Invoke(this, args);
        _platformCharacteristic.PropertyChanged += (sender, args) => PropertyChanged?.Invoke(this, args);
    }

    /// <summary>
    ///     Gets the wrapped platform characteristic.
    /// </summary>
    public IBluetoothRemoteCharacteristic PlatformCharacteristic => _platformCharacteristic;

    /// <inheritdoc />
    public IBluetoothRemoteService Service { get; }

    /// <inheritdoc />
    public Guid Id => _platformCharacteristic.Id;

    /// <inheritdoc />
    public string Name => _platformCharacteristic.Name;

    /// <inheritdoc />
    public bool CanRead => _platformCharacteristic.CanRead;

    /// <inheritdoc />
    public bool IsReading => _platformCharacteristic.IsReading;

    /// <inheritdoc />
    public bool CanWrite => _platformCharacteristic.CanWrite;

    /// <inheritdoc />
    public bool IsWriting => _platformCharacteristic.IsWriting;

    /// <inheritdoc />
    public bool CanListen => _platformCharacteristic.CanListen;

    /// <inheritdoc />
    public bool IsListening => _platformCharacteristic.IsListening;

    /// <inheritdoc />
    public ReadOnlySpan<byte> ValueSpan => _platformCharacteristic.ValueSpan;

    /// <inheritdoc />
    public ReadOnlyMemory<byte> Value => _platformCharacteristic.Value;

    /// <inheritdoc />
    public bool IsExploringDescriptors => _platformCharacteristic.IsExploringDescriptors;

    /// <inheritdoc />
    public event EventHandler<ValueUpdatedEventArgs> ValueUpdated = delegate { };

    /// <inheritdoc />
    public event EventHandler<DescriptorListChangedEventArgs>? DescriptorListChanged;

    /// <inheritdoc />
    public event EventHandler<DescriptorsAddedEventArgs>? DescriptorsAdded;

    /// <inheritdoc />
    public event EventHandler<DescriptorsRemovedEventArgs>? DescriptorsRemoved;

    /// <inheritdoc />
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <inheritdoc />
    public ValueTask<ReadOnlyMemory<byte>> ReadValueAsync(bool skipIfPreviouslyRead = false,
        TimeSpan? timeout = null,
        CancellationToken cancellationToken = default)
    {
        return _platformCharacteristic.ReadValueAsync(skipIfPreviouslyRead, timeout, cancellationToken);
    }

    /// <inheritdoc />
    public ValueTask<ReadOnlyMemory<byte>> WaitForValueChangeAsync(Func<ReadOnlyMemory<byte>, bool>? valueFilter = null,
        TimeSpan? timeout = null,
        CancellationToken cancellationToken = default)
    {
        return _platformCharacteristic.WaitForValueChangeAsync(valueFilter, timeout, cancellationToken);
    }

    /// <inheritdoc />
    public ValueTask StartListeningAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        return _platformCharacteristic.StartListeningAsync(timeout, cancellationToken);
    }

    /// <inheritdoc />
    public ValueTask StopListeningAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        return _platformCharacteristic.StopListeningAsync(timeout, cancellationToken);
    }

    /// <inheritdoc />
    public virtual async ValueTask WriteValueAsync(ReadOnlyMemory<byte> value,
        bool skipIfOldValueMatchesNewValue = false,
        TimeSpan? timeout = null,
        CancellationToken cancellationToken = default)
    {
        await OnBeforeWriteValueAsync(value, cancellationToken).ConfigureAwait(false);
        await _platformCharacteristic.WriteValueAsync(value,
                                                      skipIfOldValueMatchesNewValue,
                                                      timeout,
                                                      cancellationToken)
                                     .ConfigureAwait(false);
        await OnAfterWriteValueAsync(value, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    ///     Called before writing characteristic value.
    /// </summary>
    protected virtual ValueTask OnBeforeWriteValueAsync(ReadOnlyMemory<byte> value, CancellationToken cancellationToken)
    {
        return ValueTask.CompletedTask;
    }

    /// <summary>
    ///     Called after writing characteristic value.
    /// </summary>
    protected virtual ValueTask OnAfterWriteValueAsync(ReadOnlyMemory<byte> value, CancellationToken cancellationToken)
    {
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc />
    public ValueTask BeginReliableWriteAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        return _platformCharacteristic.BeginReliableWriteAsync(timeout, cancellationToken);
    }

    /// <inheritdoc />
    public ValueTask ExecuteReliableWriteAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        return _platformCharacteristic.ExecuteReliableWriteAsync(timeout, cancellationToken);
    }

    /// <inheritdoc />
    public ValueTask AbortReliableWriteAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        return _platformCharacteristic.AbortReliableWriteAsync(timeout, cancellationToken);
    }

    /// <inheritdoc />
    public ValueTask ClearDescriptorsAsync()
    {
        _descriptorWrappers.Clear();
        return _platformCharacteristic.ClearDescriptorsAsync();
    }

    /// <inheritdoc />
    public Task ExploreDescriptorsAsync(DescriptorExplorationOptions? options = null, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        return _platformCharacteristic.ExploreDescriptorsAsync(options, timeout, cancellationToken);
    }

    /// <inheritdoc />
    public IBluetoothRemoteDescriptor GetDescriptor(Func<IBluetoothRemoteDescriptor, bool> filter)
    {
        ArgumentNullException.ThrowIfNull(filter);
        return GetDescriptors(filter).Single();
    }

    /// <inheritdoc />
    public IBluetoothRemoteDescriptor GetDescriptor(Guid id)
    {
        return WrapDescriptor(_platformCharacteristic.GetDescriptor(id));
    }

    /// <inheritdoc />
    public IBluetoothRemoteDescriptor? GetDescriptorOrDefault(Func<IBluetoothRemoteDescriptor, bool> filter)
    {
        ArgumentNullException.ThrowIfNull(filter);
        return GetDescriptors(filter).SingleOrDefault();
    }

    /// <inheritdoc />
    public IBluetoothRemoteDescriptor? GetDescriptorOrDefault(Guid id)
    {
        var descriptor = _platformCharacteristic.GetDescriptorOrDefault(id);
        return descriptor == null ? null : WrapDescriptor(descriptor);
    }

    /// <inheritdoc />
    public IEnumerable<IBluetoothRemoteDescriptor> GetDescriptors(Func<IBluetoothRemoteDescriptor, bool>? filter = null)
    {
        var descriptors = _platformCharacteristic.GetDescriptors();
        var wrapped = descriptors.Select(WrapDescriptor);
        return filter == null ? wrapped : wrapped.Where(filter);
    }

    /// <inheritdoc />
    public bool HasDescriptor(Func<IBluetoothRemoteDescriptor, bool> filter)
    {
        ArgumentNullException.ThrowIfNull(filter);
        return GetDescriptors(filter).Any();
    }

    /// <inheritdoc />
    public bool HasDescriptor(Guid id)
    {
        return _platformCharacteristic.HasDescriptor(id);
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        _descriptorWrappers.Clear();
        await _platformCharacteristic.DisposeAsync().ConfigureAwait(false);
    }

    /// <summary>
    ///     Creates a wrapped descriptor facade for a platform descriptor.
    /// </summary>
    /// <param name="platformDescriptor">The platform descriptor to wrap.</param>
    /// <returns>The wrapped descriptor facade.</returns>
    protected virtual IBluetoothRemoteDescriptor CreateDescriptorFacade(IBluetoothRemoteDescriptor platformDescriptor)
    {
        return new BluetoothRemoteDescriptor(platformDescriptor, this);
    }

    private IBluetoothRemoteDescriptor WrapDescriptor(IBluetoothRemoteDescriptor platformDescriptor)
    {
        ArgumentNullException.ThrowIfNull(platformDescriptor);

        if (_descriptorWrappers.TryGetValue(platformDescriptor, out var wrappedDescriptor))
        {
            return wrappedDescriptor;
        }

        var facade = CreateDescriptorFacade(platformDescriptor);
        _descriptorWrappers[platformDescriptor] = facade;
        return facade;
    }
}
