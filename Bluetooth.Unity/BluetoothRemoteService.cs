namespace Bluetooth.Unity;

/// <summary>
///     Unified Bluetooth remote service facade providing extension points for client customization.
/// </summary>
public class BluetoothRemoteService : IBluetoothRemoteService
{
    private readonly IBluetoothRemoteService _platformService;

    private readonly Dictionary<IBluetoothRemoteCharacteristic, IBluetoothRemoteCharacteristic> _characteristicWrappers = new();

    /// <summary>
    ///     Initializes a new instance of the <see cref="BluetoothRemoteService" /> class.
    /// </summary>
    /// <param name="platformService">The platform service to wrap.</param>
    /// <param name="device">The wrapped parent device.</param>
    public BluetoothRemoteService(IBluetoothRemoteService platformService, IBluetoothRemoteDevice device)
    {
        ArgumentNullException.ThrowIfNull(platformService);
        ArgumentNullException.ThrowIfNull(device);

        _platformService = platformService;
        Device = device;

        _platformService.CharacteristicListChanged += (sender, args) => CharacteristicListChanged?.Invoke(this, args);
        _platformService.CharacteristicsAdded += (sender, args) => CharacteristicsAdded?.Invoke(this, args);
        _platformService.CharacteristicsRemoved += (sender, args) => CharacteristicsRemoved?.Invoke(this, args);
        _platformService.PropertyChanged += (sender, args) => PropertyChanged?.Invoke(this, args);
    }

    /// <summary>
    ///     Gets the wrapped platform service.
    /// </summary>
    public IBluetoothRemoteService PlatformService => _platformService;

    /// <inheritdoc />
    public IBluetoothRemoteDevice Device { get; }

    /// <inheritdoc />
    public Guid Id => _platformService.Id;

    /// <inheritdoc />
    public string Name => _platformService.Name;

    /// <inheritdoc />
    public bool IsExploringCharacteristics => _platformService.IsExploringCharacteristics;

    /// <inheritdoc />
    public event EventHandler<CharacteristicListChangedEventArgs>? CharacteristicListChanged;

    /// <inheritdoc />
    public event EventHandler<CharacteristicsAddedEventArgs>? CharacteristicsAdded;

    /// <inheritdoc />
    public event EventHandler<CharacteristicsRemovedEventArgs>? CharacteristicsRemoved;

    /// <inheritdoc />
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <inheritdoc />
    public ValueTask ClearCharacteristicsAsync()
    {
        _characteristicWrappers.Clear();
        return _platformService.ClearCharacteristicsAsync();
    }

    /// <inheritdoc />
    public ValueTask ExploreCharacteristicsAsync(CharacteristicExplorationOptions? options = null,
        TimeSpan? timeout = null,
        CancellationToken cancellationToken = default)
    {
        return _platformService.ExploreCharacteristicsAsync(options, timeout, cancellationToken);
    }

    /// <inheritdoc />
    public IBluetoothRemoteCharacteristic GetCharacteristic(Func<IBluetoothRemoteCharacteristic, bool> filter)
    {
        ArgumentNullException.ThrowIfNull(filter);
        return GetCharacteristics(filter).Single();
    }

    /// <inheritdoc />
    public IBluetoothRemoteCharacteristic GetCharacteristic(Guid id)
    {
        return WrapCharacteristic(_platformService.GetCharacteristic(id));
    }

    /// <inheritdoc />
    public IBluetoothRemoteCharacteristic? GetCharacteristicOrDefault(Func<IBluetoothRemoteCharacteristic, bool> filter)
    {
        ArgumentNullException.ThrowIfNull(filter);
        return GetCharacteristics(filter).SingleOrDefault();
    }

    /// <inheritdoc />
    public IBluetoothRemoteCharacteristic? GetCharacteristicOrDefault(Guid id)
    {
        var characteristic = _platformService.GetCharacteristicOrDefault(id);
        return characteristic == null ? null : WrapCharacteristic(characteristic);
    }

    /// <inheritdoc />
    public IReadOnlyList<IBluetoothRemoteCharacteristic> GetCharacteristics(Func<IBluetoothRemoteCharacteristic, bool>? filter = null)
    {
        var wrappedCharacteristics = _platformService.GetCharacteristics()
                                                    .Select(WrapCharacteristic);

        if (filter != null)
        {
            wrappedCharacteristics = wrappedCharacteristics.Where(filter);
        }

        return wrappedCharacteristics.ToList();
    }

    /// <inheritdoc />
    public bool HasCharacteristic(Func<IBluetoothRemoteCharacteristic, bool> filter)
    {
        ArgumentNullException.ThrowIfNull(filter);
        return GetCharacteristics(filter).Any();
    }

    /// <inheritdoc />
    public bool HasCharacteristic(Guid id)
    {
        return _platformService.HasCharacteristic(id);
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        _characteristicWrappers.Clear();
        await _platformService.DisposeAsync().ConfigureAwait(false);
    }

    /// <summary>
    ///     Creates a wrapped characteristic facade for a platform characteristic.
    /// </summary>
    /// <param name="platformCharacteristic">The platform characteristic to wrap.</param>
    /// <returns>The wrapped characteristic facade.</returns>
    protected virtual IBluetoothRemoteCharacteristic CreateCharacteristicFacade(IBluetoothRemoteCharacteristic platformCharacteristic)
    {
        return new BluetoothRemoteCharacteristic(platformCharacteristic, this);
    }

    private IBluetoothRemoteCharacteristic WrapCharacteristic(IBluetoothRemoteCharacteristic platformCharacteristic)
    {
        ArgumentNullException.ThrowIfNull(platformCharacteristic);

        if (_characteristicWrappers.TryGetValue(platformCharacteristic, out var wrappedCharacteristic))
        {
            return wrappedCharacteristic;
        }

        var facade = CreateCharacteristicFacade(platformCharacteristic);
        _characteristicWrappers[platformCharacteristic] = facade;
        return facade;
    }
}
