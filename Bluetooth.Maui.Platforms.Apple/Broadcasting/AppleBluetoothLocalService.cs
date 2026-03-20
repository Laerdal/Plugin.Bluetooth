using Bluetooth.Maui.Platforms.Apple.Broadcasting.NativeObjects;

using CharacteristicNotFoundException = Bluetooth.Abstractions.Broadcasting.Exceptions.CharacteristicNotFoundException;
using MultipleCharacteristicsFoundException = Bluetooth.Abstractions.Broadcasting.Exceptions.MultipleCharacteristicsFoundException;

namespace Bluetooth.Maui.Platforms.Apple.Broadcasting;

/// <inheritdoc cref="BaseBluetoothLocalService" />
public class AppleBluetoothLocalService : BaseBluetoothLocalService, CbPeripheralManagerWrapper.ICbServiceDelegate
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="AppleBluetoothLocalService" /> class with the specified Core Bluetooth service, broadcaster, ID, name, primary status, and logger.
    /// </summary>
    /// <param name="cbService">The native iOS Core Bluetooth service represented by this local service.</param>
    /// <param name="broadcaster">The Bluetooth broadcaster to which this service belongs.</param>
    /// <param name="id">The unique identifier for this service.</param>
    /// <param name="name">An optional name for the service, used for debugging purposes.</param>
    /// <param name="isPrimary">Indicates whether this service is a primary service (default is true).</param>
    /// <param name="logger">An optional logger for logging service-related events and errors.</param>
    public AppleBluetoothLocalService(CBMutableService cbService,
        IBluetoothBroadcaster broadcaster,
        Guid id,
        string? name = null,
        bool isPrimary = true,
        ILogger<IBluetoothLocalService>? logger = null) : base(broadcaster,
                                                               id,
                                                               name,
                                                               isPrimary,
                                                               logger)
    {
        CbService = cbService ?? throw new ArgumentNullException(nameof(cbService));
    }

    /// <summary>
    ///     Gets the native iOS Core Bluetooth service.
    /// </summary>
    public CBMutableService CbService { get; }

    /// <summary>
    ///     Gets the Bluetooth broadcaster to which this service belongs, cast to the Apple-specific implementation.
    /// </summary>
    public AppleBluetoothBroadcaster AppleBluetoothBroadcaster => (AppleBluetoothBroadcaster) Broadcaster;

    /// <inheritdoc />
    public CbPeripheralManagerWrapper.ICbCharacteristicDelegate GetCharacteristic(CBCharacteristic? characteristic)
    {
        if (characteristic == null)
        {
            throw new CharacteristicNotFoundException(this);
        }

        try
        {
            var match = GetCharacteristic(sharedCharacteristic => AreRepresentingTheSameObject(characteristic, sharedCharacteristic));
            return match as CbPeripheralManagerWrapper.ICbCharacteristicDelegate ?? throw new CharacteristicNotFoundException(this, characteristic.UUID.ToGuid());
        }
        catch (InvalidOperationException e)
        {
            var matches = GetCharacteristics(sharedCharacteristic => AreRepresentingTheSameObject(characteristic, sharedCharacteristic)).ToArray();
            throw new MultipleCharacteristicsFoundException(this, matches, e);
        }
    }

    private static bool AreRepresentingTheSameObject(CBCharacteristic native, IBluetoothLocalCharacteristic shared)
    {
        return shared is AppleBluetoothLocalCharacteristic s && native.UUID.Equals(s.CbCharacteristic.UUID);
    }

    /// <inheritdoc />
    protected override ValueTask<IBluetoothLocalCharacteristic> NativeCreateCharacteristicAsync(Guid id,
        BluetoothCharacteristicProperties properties,
        BluetoothCharacteristicPermissions permissions,
        string? name = null,
        TimeSpan? timeout = null,
        CancellationToken cancellationToken = default)
    {
        // Create a new CBMutableCharacteristic with the specified properties and permissions
        var cbCharacteristic = new CBMutableCharacteristic(id.ToCBUuid(), properties.ToNative(), null, permissions.ToNative());

        // Create the corresponding AppleBluetoothLocalCharacteristic instance
        var logger = Broadcaster.LoggerFactory?.CreateLogger<AppleBluetoothLocalCharacteristic>();
#pragma warning disable CA2000 // Characteristic is returned via ValueTask; caller takes ownership
        var localCharacteristic = new AppleBluetoothLocalCharacteristic(cbCharacteristic, this, id, properties, permissions, null, name, logger);
#pragma warning restore CA2000

        // Add the new characteristic to the service's list of characteristics
        var characteristics = CbService.Characteristics?.ToList() ?? new List<CBCharacteristic>();
        characteristics.Add(cbCharacteristic);
        CbService.Characteristics = characteristics.ToArray();

        return new ValueTask<IBluetoothLocalCharacteristic>(localCharacteristic);
    }
}
