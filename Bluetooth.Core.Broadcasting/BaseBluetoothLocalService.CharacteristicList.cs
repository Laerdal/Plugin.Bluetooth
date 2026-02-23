namespace Bluetooth.Core.Broadcasting;

/// <summary>
///     Base class for Bluetooth broadcaster services.
/// </summary>
public abstract partial class BaseBluetoothLocalService
{
    private ObservableCollection<IBluetoothLocalCharacteristic> Characteristics
    {
        get
        {
            field ??= [];
            return field;
        }
    }

    #region Characteristics - Add

    /// <inheritdoc />
    public ValueTask<IBluetoothLocalCharacteristic> AddCharacteristicAsync(IBluetoothLocalCharacteristicFactory.BluetoothLocalCharacteristicSpec request,
        TimeSpan? timeout = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var existingCharacteristic = GetCharacteristicOrDefault(request.Id);
        if (existingCharacteristic != null)
        {
            LogCharacteristicAlreadyExists(Id, request.Id);
            throw new CharacteristicAlreadyExistsException(this, request.Id, existingCharacteristic);
        }

        LogAddingCharacteristic(Id, request.Id);
        var newCharacteristic = LocalCharacteristicFactory.CreateCharacteristic(this, request);
        Characteristics.Add(newCharacteristic);
        LogCharacteristicAdded(Id, request.Id);

        return ValueTask.FromResult(newCharacteristic);
    }

    #endregion

    #region Characteristics - Get

    /// <inheritdoc />
    public IBluetoothLocalCharacteristic GetCharacteristic(Func<IBluetoothLocalCharacteristic, bool> filter)
    {
        return GetCharacteristicOrDefault(filter) ?? throw new CharacteristicNotFoundException(this);
    }

    /// <inheritdoc />
    public IBluetoothLocalCharacteristic GetCharacteristic(Guid id)
    {
        var characteristic = GetCharacteristicOrDefault(id);
        if (characteristic == null)
        {
            LogCharacteristicNotFound(Id, id);
            throw new CharacteristicNotFoundException(this, id);
        }

        return characteristic;
    }

    /// <inheritdoc />
    public IBluetoothLocalCharacteristic? GetCharacteristicOrDefault(Func<IBluetoothLocalCharacteristic, bool> filter)
    {
        try
        {
            return Characteristics.SingleOrDefault(filter);
        }
        catch (InvalidOperationException e)
        {
            throw new MultipleCharacteristicsFoundException(this, Characteristics.Where(filter).ToArray(), e);
        }
    }

    /// <inheritdoc />
    public IBluetoothLocalCharacteristic? GetCharacteristicOrDefault(Guid id)
    {
        try
        {
            return Characteristics.SingleOrDefault(characteristic => characteristic.Id == id);
        }
        catch (InvalidOperationException e)
        {
            throw new MultipleCharacteristicsFoundException(this, id, Characteristics.Where(characteristic => characteristic.Id == id).ToArray(), e);
        }
    }

    /// <inheritdoc />
    public IEnumerable<IBluetoothLocalCharacteristic> GetCharacteristics(Func<IBluetoothLocalCharacteristic, bool>? filter = null)
    {
        filter ??= _ => true;
        return Characteristics.Where(filter).ToArray();
    }

    #endregion

    #region Characteristics - Remove

    /// <inheritdoc />
    public ValueTask RemoveCharacteristicAsync(Guid id, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        var characteristic = GetCharacteristic(id);
        return RemoveCharacteristicAsync(characteristic, timeout, cancellationToken);
    }

    /// <inheritdoc />
    public async ValueTask RemoveCharacteristicAsync(IBluetoothLocalCharacteristic localCharacteristic, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(localCharacteristic);

        LogRemovingCharacteristic(Id, localCharacteristic.Id);
        Characteristics.Remove(localCharacteristic);
        await localCharacteristic.DisposeAsync().ConfigureAwait(false);
        LogCharacteristicRemoved(Id, localCharacteristic.Id);
    }

    /// <inheritdoc />
    public async ValueTask RemoveAllCharacteristicsAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        var characteristicList = Characteristics.ToList();
        var characteristicCount = characteristicList.Count;

        LogClearingCharacteristics(Id);

        foreach (var characteristic in characteristicList)
        {
            await RemoveCharacteristicAsync(characteristic, timeout, cancellationToken).ConfigureAwait(false);
        }

        LogCharacteristicsCleared(Id, characteristicCount);
    }

    #endregion

    #region Characteristics - Has

    /// <inheritdoc />
    public bool HasCharacteristic(Func<IBluetoothLocalCharacteristic, bool> filter)
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
