namespace Bluetooth.Core.Scanning;

/// <inheritdoc cref="IBluetoothRemoteService" />
public abstract partial class BaseBluetoothRemoteService : BaseBindableObject, IBluetoothRemoteService
{
    /// <summary>
    ///     The logger instance used for logging service operations.
    /// </summary>
    private readonly ILogger<IBluetoothRemoteService> _logger;

    /// <summary>
    ///     Initializes a new instance of the <see cref="BaseBluetoothRemoteService" /> class.
    /// </summary>
    /// <param name="device">The Bluetooth device associated with this service.</param>
    /// <param name="request">The factory request containing service information.</param>
    /// <param name="characteristicFactory">The factory for creating Bluetooth characteristics.</param>
    /// <param name="logger">The logger instance to use for logging (optional).</param>
    protected BaseBluetoothRemoteService(IBluetoothRemoteDevice device,
        IBluetoothServiceFactory.BluetoothServiceFactoryRequest request,
        IBluetoothCharacteristicFactory characteristicFactory,
        ILogger<IBluetoothRemoteService>? logger = null)
    {
        ArgumentNullException.ThrowIfNull(device);
        ArgumentNullException.ThrowIfNull(characteristicFactory);
        ArgumentNullException.ThrowIfNull(request);

        _logger = logger ?? NullLogger<IBluetoothRemoteService>.Instance;
        Device = device;
        CharacteristicFactory = characteristicFactory;
        Id = request.ServiceId;
    }

    /// <summary>
    ///     The factory responsible for creating characteristics associated with this service.
    /// </summary>
    protected IBluetoothCharacteristicFactory CharacteristicFactory { get; }

    /// <inheritdoc />
    public IBluetoothRemoteDevice Device { get; }

    /// <inheritdoc />
    public Guid Id { get; }

    /// <inheritdoc />
    public string Name { get; } = "Unknown Service";

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

    /// <inheritdoc />
    public override string ToString()
    {
        return $"[{Id}] {Name}";
    }
}