namespace Bluetooth.Core.Broadcasting;

/// <summary>
///     Base class for Bluetooth broadcast characteristics.
/// </summary>
public abstract partial class BaseBluetoothLocalCharacteristic : BaseBindableObject, IBluetoothLocalCharacteristic
{
    /// <summary>
    ///     The logger instance used for logging characteristic operations.
    /// </summary>
    private readonly ILogger<IBluetoothLocalCharacteristic> _logger;

    /// <summary>
    ///     Initializes a new instance of the <see cref="BaseBluetoothLocalCharacteristic" /> class.
    /// </summary>
    protected BaseBluetoothLocalCharacteristic(IBluetoothLocalService localService,
        IBluetoothLocalCharacteristicFactory.BluetoothLocalCharacteristicSpec request,
        IBluetoothLocalDescriptorFactory localDescriptorRepository,
        ILogger<IBluetoothLocalCharacteristic>? logger = null)
    {
        ArgumentNullException.ThrowIfNull(localService);
        ArgumentNullException.ThrowIfNull(localDescriptorRepository);
        ArgumentNullException.ThrowIfNull(request);

        _logger = logger ?? NullLogger<IBluetoothLocalCharacteristic>.Instance;
        LocalService = localService;
        LocalDescriptorFactory = localDescriptorRepository;
        Id = request.Id;
        Value = request.InitialValue ?? new ReadOnlyMemory<byte>([]);
        Properties = request.Properties;
        Permissions = request.Permissions;
    }

    /// <summary>
    ///     Backing field for the <see cref="Descriptors" /> property.
    /// </summary>
    protected IBluetoothLocalDescriptorFactory LocalDescriptorFactory { get; }

    /// <inheritdoc />
    public IBluetoothLocalService LocalService { get; }

    /// <inheritdoc />
    public Guid Id { get; }

    /// <inheritdoc />
    public string Name { get; } = "Unknown Characteristic";

    /// <inheritdoc />
    public BluetoothCharacteristicProperties Properties { get; init; }

    /// <inheritdoc />
    public BluetoothCharacteristicPermissions Permissions { get; init; }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        await DisposeAsyncCore().ConfigureAwait(false);
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"[{Id}] {Name}";
    }

    /// <summary>
    ///     Disposes the resources asynchronously.
    /// </summary>
    protected async virtual ValueTask DisposeAsyncCore()
    {
        await RemoveAllDescriptorsAsync().ConfigureAwait(false);
    }
}