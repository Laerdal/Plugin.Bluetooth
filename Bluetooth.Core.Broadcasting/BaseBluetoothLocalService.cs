namespace Bluetooth.Core.Broadcasting;

/// <summary>
///     Base class for Bluetooth broadcaster services.
/// </summary>
public abstract partial class BaseBluetoothLocalService : BaseBindableObject, IBluetoothLocalService
{
    /// <inheritdoc />
    public IBluetoothBroadcaster Broadcaster { get; }

    /// <summary>
    ///     Initializes a new instance of the <see cref="BaseBluetoothLocalService" /> class.
    /// </summary>
    /// <param name="broadcaster">The broadcaster that owns this service.</param>
    /// <param name="id">The unique identifier of the service.</param>
    /// <param name="name">The name of the service (optional).</param>
    /// <param name="isPrimary">Indicates whether this service is a primary service (default: true).</param>
    /// <param name="logger">The logger instance to use for logging (optional).</param>
    protected BaseBluetoothLocalService(IBluetoothBroadcaster broadcaster,
        Guid id,
        string? name = null,
        bool isPrimary = true,
        ILogger<IBluetoothLocalService>? logger = null) : base(logger)
    {
        // Validate constructor arguments
        ArgumentNullException.ThrowIfNull(broadcaster);

        Broadcaster = broadcaster;
        Id = id;
        IsPrimary = isPrimary;
        if (!string.IsNullOrWhiteSpace(name))
        {
            Name = name;
        }
    }

    /// <inheritdoc />
    public Guid Id { get; }

    /// <inheritdoc />
    public string Name { get; } = "Unknown Service";

    /// <inheritdoc />
    public bool IsPrimary { get; }

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
        await RemoveAllCharacteristicsAsync().ConfigureAwait(false);
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
