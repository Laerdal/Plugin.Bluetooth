namespace Bluetooth.Core.Broadcasting;

/// <summary>
///     Base class for Bluetooth broadcaster devices.
/// </summary>
public abstract partial class BaseBluetoothConnectedDevice : BaseBindableObject, IBluetoothConnectedDevice
{
    /// <inheritdoc />
    public IBluetoothBroadcaster Broadcaster { get; }

    /// <summary>
    ///     Initializes a new instance of the <see cref="BaseBluetoothConnectedDevice" /> class.
    /// </summary>
    /// <param name="broadcaster">The broadcaster that owns this client device.</param>
    /// <param name="id">The unique identifier for the connected device, typically a UUID or MAC address.</param>
    /// <param name="logger">The logger for logging operations.</param>
    protected BaseBluetoothConnectedDevice(IBluetoothBroadcaster broadcaster,
        string id,
        ILogger<IBluetoothConnectedDevice>? logger = null) : base(logger)
    {
        ArgumentNullException.ThrowIfNull(broadcaster);

        Broadcaster = broadcaster;
        Id = id;
    }

    /// <summary>
    ///     Initializes a new instance using a factory spec.
    /// </summary>
    /// <param name="broadcaster">The broadcaster that owns this client device.</param>
    /// <param name="spec">The factory spec containing connected device information.</param>
    /// <param name="logger">The logger for logging operations.</param>
    protected BaseBluetoothConnectedDevice(
        IBluetoothBroadcaster broadcaster,
        IBluetoothConnectedDeviceFactory.BluetoothConnectedDeviceSpec spec,
        ILogger<IBluetoothConnectedDevice>? logger = null)
        : this(broadcaster, (spec ?? throw new ArgumentNullException(nameof(spec))).DeviceId, logger)
    {
    }

    /// <inheritdoc />
    public string Id { get; }

    /// <inheritdoc />
    public string Name { get; } = "Unknown Client Device";


    #region Dispose

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        await DisposeAsyncCore().ConfigureAwait(false);
        GC.SuppressFinalize(this);
    }
    /// <summary>
    ///     Disposes the resources asynchronously.
    /// </summary>
    protected virtual ValueTask DisposeAsyncCore()
    {
        RemoveAllCharacteristicSubscriptions();
        return ValueTask.CompletedTask;
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
