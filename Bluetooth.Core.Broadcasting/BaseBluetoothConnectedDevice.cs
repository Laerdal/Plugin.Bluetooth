namespace Bluetooth.Core.Broadcasting;

/// <summary>
/// Base class for Bluetooth broadcaster devices.
/// </summary>
public abstract partial class BaseBluetoothConnectedDevice : BaseBindableObject, IBluetoothConnectedDevice
{
    /// <summary>
    /// The logger instance used for logging connected device operations.
    /// </summary>
    private readonly ILogger<IBluetoothConnectedDevice> _logger;

    /// <inheritdoc />
    public IBluetoothBroadcaster Broadcaster { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseBluetoothConnectedDevice"/> class.
    /// </summary>
    /// <param name="broadcaster">The broadcaster that owns this client device.</param>
    /// <param name="request">The request for creating the client device.</param>
    /// <param name="logger">The logger for logging operations.</param>
    protected BaseBluetoothConnectedDevice(IBluetoothBroadcaster broadcaster,
        IBluetoothConnectedDeviceFactory.BluetoothConnectedDeviceSpec request,
        ILogger<IBluetoothConnectedDevice>? logger = null) : base(logger)
    {
        ArgumentNullException.ThrowIfNull(broadcaster);
        ArgumentNullException.ThrowIfNull(request);

        _logger = logger ?? NullLogger<IBluetoothConnectedDevice>.Instance;
        Broadcaster = broadcaster;
        Id = request.ClientId;
    }

    /// <inheritdoc />
    public string Id { get; }

    /// <inheritdoc />
    public string Name { get; } = "Unknown Client Device";

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"[{Id}] {Name}";
    }

    /// <summary>
    /// Disposes the resources asynchronously.
    /// </summary>
    protected virtual ValueTask DisposeAsyncCore()
    {
        RemoveAllCharacteristicSubscriptions();
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        await DisposeAsyncCore().ConfigureAwait(false);
        GC.SuppressFinalize(this);
    }
}
