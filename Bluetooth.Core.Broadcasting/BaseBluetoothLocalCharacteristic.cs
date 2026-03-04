namespace Bluetooth.Core.Broadcasting;

/// <summary>
///     Base class for Bluetooth broadcast characteristics.
/// </summary>
public abstract partial class BaseBluetoothLocalCharacteristic : BaseBindableObject, IBluetoothLocalCharacteristic
{
    /// <inheritdoc />
    public IBluetoothLocalService Service { get; }

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
