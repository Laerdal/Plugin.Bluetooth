using Bluetooth.Maui.Platforms.Win.Tools;
using Bluetooth.Maui.Platforms.Win.Exceptions;

namespace Bluetooth.Maui.Platforms.Win.Broadcasting;

/// <inheritdoc cref="BaseBluetoothLocalService" />
public partial class WindowsBluetoothLocalService : BaseBluetoothLocalService
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="WindowsBluetoothLocalService" /> class.
    /// </summary>
    /// <param name="serviceProvider">The native GATT service provider.</param>
    /// <param name="localService">The native local service.</param>
    /// <param name="broadcaster">The broadcaster that owns this service.</param>
    /// <param name="id">Service identifier.</param>
    /// <param name="name">Optional service name.</param>
    /// <param name="isPrimary">Whether this service is primary.</param>
    public WindowsBluetoothLocalService(GattServiceProvider serviceProvider,
        GattLocalService localService,
        IBluetoothBroadcaster broadcaster,
        Guid id,
        string? name = null,
        bool isPrimary = true)
        : base(broadcaster, id, name, isPrimary)
    {
        ServiceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        NativeService = localService ?? throw new ArgumentNullException(nameof(localService));
    }

    /// <summary>
    ///     Gets the native GATT service provider.
    /// </summary>
    public GattServiceProvider ServiceProvider { get; }

    /// <summary>
    ///     Gets the native local GATT service.
    /// </summary>
    public GattLocalService NativeService { get; }

    /// <inheritdoc />
    protected override async ValueTask<IBluetoothLocalCharacteristic> NativeCreateCharacteristicAsync(Guid id,
        BluetoothCharacteristicProperties properties,
        BluetoothCharacteristicPermissions permissions,
        string? name = null,
        TimeSpan? timeout = null,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var parameters = new GattLocalCharacteristicParameters
        {
            CharacteristicProperties = properties.ToNative(),
            UserDescription = name
        };
        permissions.ApplyTo(parameters);

        var result = await NativeService.CreateCharacteristicAsync(id, parameters).AsTask(cancellationToken).ConfigureAwait(false);
        WindowsNativeBluetoothErrorException.ThrowIfNotSuccess(result.Error);

        LogNativeCharacteristicCreated(id, Id);

        return new WindowsBluetoothLocalCharacteristic(result.Characteristic, this, id, properties, permissions, name);
    }

    /// <inheritdoc />
    protected override async ValueTask DisposeAsyncCore()
    {
        ServiceProvider.StopAdvertising();

        LogServiceDisposeCleanup(Id);

        await base.DisposeAsyncCore().ConfigureAwait(false);
    }
}
