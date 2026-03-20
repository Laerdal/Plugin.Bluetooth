namespace Bluetooth.Core.Broadcasting;

/// <summary>
///     Base class for Bluetooth broadcast characteristics.
/// </summary>
public abstract partial class BaseBluetoothLocalCharacteristic : BaseBindableObject, IBluetoothLocalCharacteristic
{
    /// <inheritdoc />
    public IBluetoothLocalService Service { get; }

    /// <summary>
    ///     Gets the factory for creating local Bluetooth descriptors.
    /// </summary>
    protected IBluetoothLocalDescriptorFactory? DescriptorFactory { get; }

    /// <summary>
    ///     Initializes a new instance of the <see cref="BaseBluetoothLocalCharacteristic" /> class.
    /// </summary>
    protected BaseBluetoothLocalCharacteristic(IBluetoothLocalService service,
        Guid id,
        BluetoothCharacteristicProperties properties,
        BluetoothCharacteristicPermissions permissions,
        ReadOnlyMemory<byte>? initialValue = null,
        string? name = null,
        ILogger<IBluetoothLocalCharacteristic>? logger = null) : base(logger)
    {
        // Validate constructor arguments
        ArgumentNullException.ThrowIfNull(service);

        Service = service;
        Id = id;
        Value = initialValue ?? new ReadOnlyMemory<byte>([]);
        Properties = properties;
        Permissions = permissions;
        if (!string.IsNullOrWhiteSpace(name))
        {
            Name = name;
        }
    }

    /// <summary>
    ///     Initializes a new instance using a factory spec.
    /// </summary>
    /// <param name="service">The local service this characteristic belongs to.</param>
    /// <param name="spec">The factory spec containing characteristic information.</param>
    /// <param name="descriptorFactory">The factory for creating local descriptors.</param>
    /// <param name="logger">The logger instance to use for logging (optional).</param>
    protected BaseBluetoothLocalCharacteristic(
        IBluetoothLocalService service,
        IBluetoothLocalCharacteristicFactory.BluetoothLocalCharacteristicSpec spec,
        IBluetoothLocalDescriptorFactory descriptorFactory,
        ILogger<IBluetoothLocalCharacteristic>? logger = null)
        : this(service,
            (spec ?? throw new ArgumentNullException(nameof(spec))).CharacteristicId,
            spec.Properties,
            spec.Permissions,
            null,
            spec.Name,
            logger)
    {
        ArgumentNullException.ThrowIfNull(descriptorFactory);
        DescriptorFactory = descriptorFactory;
    }

    /// <inheritdoc />
    public Guid Id { get; }

    /// <inheritdoc />
    public string Name { get; } = "Unknown Characteristic";

    /// <inheritdoc />
    public BluetoothCharacteristicProperties Properties { get; init; }

    /// <inheritdoc />
    public BluetoothCharacteristicPermissions Permissions { get; init; }

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
    protected async virtual ValueTask DisposeAsyncCore()
    {
        await RemoveAllDescriptorsAsync().ConfigureAwait(false);
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
