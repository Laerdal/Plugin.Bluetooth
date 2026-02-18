using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

namespace Bluetooth.Core.Broadcasting;

/// <summary>
/// Base class for Bluetooth broadcaster services.
/// </summary>
public abstract partial class BaseBluetoothLocalService : BaseBindableObject, IBluetoothLocalService
{
    /// <summary>
    /// The logger instance used for logging service operations.
    /// </summary>
    private readonly ILogger<IBluetoothLocalService> _logger;

    /// <inheritdoc/>
    public IBluetoothBroadcaster Broadcaster { get; }

    /// <summary>
    /// Factory for creating characteristics within this service.
    /// </summary>
    protected IBluetoothLocalCharacteristicFactory LocalCharacteristicFactory { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseBluetoothLocalService"/> class.
    /// </summary>
    /// <param name="broadcaster">The broadcaster that owns this service.</param>
    /// <param name="request">The request for creating the service.</param>
    /// <param name="localCharacteristicFactory">The factory for creating characteristics.</param>
    /// <param name="logger">The logger instance to use for logging (optional).</param>
    protected BaseBluetoothLocalService(IBluetoothBroadcaster broadcaster,
        IBluetoothLocalServiceFactory.BluetoothLocalServiceSpec request,
        IBluetoothLocalCharacteristicFactory localCharacteristicFactory,
        ILogger<IBluetoothLocalService>? logger = null)
    {
        ArgumentNullException.ThrowIfNull(broadcaster);
        ArgumentNullException.ThrowIfNull(localCharacteristicFactory);
        ArgumentNullException.ThrowIfNull(request);

        _logger = logger ?? NullLogger<IBluetoothLocalService>.Instance;
        Broadcaster = broadcaster;
        LocalCharacteristicFactory = localCharacteristicFactory;
        Id = request.Id;
        IsPrimary = request.IsPrimary;
    }

    /// <inheritdoc/>
    public Guid Id { get; }

    /// <inheritdoc />
    public string Name { get; } = "Unknown Service";

    /// <inheritdoc/>
    public bool IsPrimary { get; }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"{Name} ({Id})";
    }

    /// <summary>
    /// Disposes the resources asynchronously.
    /// </summary>
    protected async virtual ValueTask DisposeAsyncCore()
    {
        await RemoveAllCharacteristicsAsync().ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        await DisposeAsyncCore().ConfigureAwait(false);
        GC.SuppressFinalize(this);
    }
}
