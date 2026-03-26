namespace Bluetooth.Core.Scanning;

/// <inheritdoc cref="IBluetoothRemoteService" />
public abstract partial class BaseBluetoothRemoteService : BaseBindableObject, IBluetoothRemoteService
{
    /// <inheritdoc />
    public IBluetoothRemoteDevice Device { get; }

    /// <summary>
    ///     Gets the factory for creating Bluetooth remote characteristics.
    /// </summary>
    protected IBluetoothRemoteCharacteristicFactory? CharacteristicFactory { get; }

    /// <summary>
    ///     Initializes a new instance of the <see cref="BaseBluetoothRemoteService" /> class.
    /// </summary>
    /// <param name="parentDevice">The Bluetooth device associated with this service.</param>
    /// <param name="id">The unique identifier (UUID) of the service.</param>
    /// <param name="nameProvider">An optional provider for service names, used to resolve the name based on the ID.</param>
    /// <param name="logger">The logger instance to use for logging (optional).</param>
    protected BaseBluetoothRemoteService(IBluetoothRemoteDevice parentDevice,
        Guid id,
        IBluetoothNameProvider? nameProvider = null,
        ILogger<IBluetoothRemoteService>? logger = null) : base(logger)
    {
        // Validate constructor arguments
        ArgumentNullException.ThrowIfNull(parentDevice);

        Device = parentDevice;
        Id = id;

        // Name
        if (nameProvider != null)
        {
            Name = nameProvider.GetKnownServiceName(Id) ?? Name;
        }
    }

    /// <summary>
    ///     Initializes a new instance using a factory spec.
    /// </summary>
    /// <param name="parentDevice">The Bluetooth device associated with this service.</param>
    /// <param name="spec">The factory spec containing service information.</param>
    /// <param name="characteristicFactory">The factory for creating Bluetooth remote characteristics.</param>
    /// <param name="nameProvider">An optional provider for service names, used to resolve the name based on the ID.</param>
    /// <param name="logger">The logger instance to use for logging (optional).</param>
    protected BaseBluetoothRemoteService(
        IBluetoothRemoteDevice parentDevice,
        IBluetoothRemoteServiceFactory.BluetoothRemoteServiceFactorySpec spec,
        IBluetoothRemoteCharacteristicFactory characteristicFactory,
        IBluetoothNameProvider? nameProvider = null,
        ILogger<IBluetoothRemoteService>? logger = null)
        : this(parentDevice, (spec ?? throw new ArgumentNullException(nameof(spec))).ServiceId, nameProvider, logger)
    {
        ArgumentNullException.ThrowIfNull(characteristicFactory);
        CharacteristicFactory = characteristicFactory;
    }

    /// <inheritdoc />
    public Guid Id { get; }

    /// <inheritdoc />
    public string Name { get; } = "Unknown Service";

    #region Dispose
    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        await DisposeAsyncCore().ConfigureAwait(false);
    }

    /// <summary>
    ///     Performs the core disposal logic for the service, including canceling pending operations and cleaning up resources.
    /// </summary>
    /// <returns>A task that represents the asynchronous disposal operation.</returns>
    protected async virtual ValueTask DisposeAsyncCore()
    {
        // Cancel any pending explorations
        CharacteristicsExplorationTcs?.TrySetCanceled();

        // Unsubscribe from event
        Characteristics.CollectionChanged -= CharacteristicsOnCollectionChanged;

        // Unsubscribe from other events
        CharacteristicListChanged = null;
        CharacteristicsAdded = null;
        CharacteristicsRemoved = null;

        await ClearCharacteristicsAsync().ConfigureAwait(false);
    }

    #endregion

    #region ToString

    /// <inheritdoc />
    public override string ToString()
    {
        return $"[{Id}] {Name}";
    }

    #endregion
}
