namespace Bluetooth.Core.Broadcasting;

public abstract partial class BaseBluetoothLocalCharacteristic
{
    /// <summary>
    ///     Gets the collection of descriptors for this characteristic.
    /// </summary>
    private ObservableCollection<IBluetoothLocalDescriptor> Descriptors
    {
        get
        {
            field ??= [];
            return field;
        }
    }

    #region Descriptors - Add

    /// <inheritdoc />
    public ValueTask<IBluetoothLocalDescriptor> AddDescriptorAsync(IBluetoothLocalDescriptorFactory.BluetoothLocalDescriptorSpec spec, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(spec);
        var existingDescriptor = GetDescriptorOrDefault(spec.Id);
        if (existingDescriptor != null)
        {
            throw new DescriptorAlreadyExistsException(this, spec.Id, existingDescriptor);
        }

        var newDescriptor = LocalDescriptorFactory.Create(this, spec);
        Descriptors.Add(newDescriptor);
        return ValueTask.FromResult(newDescriptor);
    }

    #endregion

    #region Descriptors - Get

    private readonly static Func<IBluetoothLocalDescriptor, bool> _defaultAcceptAllFilter = _ => true;

    /// <inheritdoc />
    public IBluetoothLocalDescriptor GetDescriptor(Guid id)
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

    /// <inheritdoc />
    public IBluetoothLocalDescriptor GetDescriptor(Func<IBluetoothLocalDescriptor, bool> filter)
    {
        ArgumentNullException.ThrowIfNull(filter);
        try
        {
            return Descriptors.SingleOrDefault(filter) ?? throw new DescriptorNotFoundException(this);
        }
        catch (InvalidOperationException e)
        {
            throw new MultipleDescriptorsFoundException(this, Descriptors.Where(filter).ToList(), e);
        }
    }

    /// <inheritdoc />
    public IBluetoothLocalDescriptor? GetDescriptorOrDefault(Guid id)
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

    /// <inheritdoc />
    public IBluetoothLocalDescriptor? GetDescriptorOrDefault(Func<IBluetoothLocalDescriptor, bool> filter)
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

    /// <inheritdoc />
    public IEnumerable<IBluetoothLocalDescriptor> GetDescriptors(Func<IBluetoothLocalDescriptor, bool>? filter = null)
    {
        filter ??= _defaultAcceptAllFilter;
        return Descriptors.Where(filter).ToList();
    }

    #endregion

    #region Descriptors - Remove

    /// <inheritdoc />
    public ValueTask RemoveDescriptorAsync(Guid id, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        var descriptor = GetDescriptor(id);
        return RemoveDescriptorAsync(descriptor, timeout, cancellationToken);
    }

    /// <inheritdoc />
    public async ValueTask RemoveDescriptorAsync(IBluetoothLocalDescriptor localDescriptor, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(localDescriptor);
        Descriptors.Remove(localDescriptor);
        await localDescriptor.DisposeAsync().ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask RemoveAllDescriptorsAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        foreach (var descriptor in Descriptors.ToList())
        {
            await RemoveDescriptorAsync(descriptor, timeout, cancellationToken).ConfigureAwait(false);
        }
    }

    #endregion

    #region Descriptors - Has

    /// <inheritdoc />
    public bool HasDescriptor(Func<IBluetoothLocalDescriptor, bool> filter)
    {
        ArgumentNullException.ThrowIfNull(filter);
        return Descriptors.Any(filter);
    }

    /// <inheritdoc />
    public bool HasDescriptor(Guid id)
    {
        return HasDescriptor(d => d.Id == id);
    }

    #endregion
}
