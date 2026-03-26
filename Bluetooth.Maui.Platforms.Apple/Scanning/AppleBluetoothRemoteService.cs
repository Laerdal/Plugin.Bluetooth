using Bluetooth.Maui.Platforms.Apple.Logging;
using Bluetooth.Maui.Platforms.Apple.Scanning.Factories;
using Bluetooth.Maui.Platforms.Apple.Scanning.NativeObjects;

using CharacteristicNotFoundException = Bluetooth.Abstractions.Scanning.Exceptions.CharacteristicNotFoundException;
using MultipleCharacteristicsFoundException = Bluetooth.Abstractions.Scanning.Exceptions.MultipleCharacteristicsFoundException;

namespace Bluetooth.Maui.Platforms.Apple.Scanning;

/// <inheritdoc cref="BaseBluetoothRemoteService" />
public class AppleBluetoothRemoteService : BaseBluetoothRemoteService, CbPeripheralWrapper.ICbServiceDelegate
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="AppleBluetoothRemoteService" /> class with the specified Core Bluetooth service, parent device, ID, name provider, and logger.
    /// </summary>
    /// <param name="cbService">The native iOS Core Bluetooth service represented by this remote service.</param>
    /// <param name="parentDevice">The Bluetooth device associated with this service.</param>
    /// <param name="id">The unique identifier for this service.</param>
    /// <param name="nameProvider">An optional provider for service names, used to resolve the name based on the ID.</param>
    /// <param name="logger">An optional logger for logging service-related events and errors.</param>
    public AppleBluetoothRemoteService(CBService cbService,
        IBluetoothRemoteDevice parentDevice,
        Guid id,
        IBluetoothNameProvider? nameProvider = null,
        ILogger<IBluetoothRemoteService>? logger = null) : base(parentDevice, id, nameProvider, logger)
    {
        CbService = cbService;
    }

    /// <summary>
    ///     Initializes a new instance using a factory spec.
    /// </summary>
    /// <param name="parentDevice">The Bluetooth device associated with this service.</param>
    /// <param name="spec">The Apple-specific factory spec containing the native service.</param>
    /// <param name="characteristicFactory">The factory for creating remote characteristics.</param>
    /// <param name="nameProvider">An optional provider for service names.</param>
    /// <param name="logger">An optional logger for logging service-related events and errors.</param>
    public AppleBluetoothRemoteService(
        IBluetoothRemoteDevice parentDevice,
        AppleBluetoothRemoteServiceFactorySpec spec,
        IBluetoothRemoteCharacteristicFactory characteristicFactory,
        IBluetoothNameProvider? nameProvider = null,
        ILogger<IBluetoothRemoteService>? logger = null) : base(parentDevice, spec, characteristicFactory, nameProvider, logger)
    {
        ArgumentNullException.ThrowIfNull(spec);
        CbService = spec.CbService;
    }

    /// <summary>
    ///     Gets the native iOS Core Bluetooth service.
    /// </summary>
    public CBService CbService { get; }

    /// <summary>
    ///     Gets the Bluetooth device to which this service belongs, cast to the Apple-specific implementation.
    /// </summary>
    public AppleBluetoothRemoteDevice AppleBluetoothRemoteDevice => (AppleBluetoothRemoteDevice) Device;

    /// <inheritdoc />
    public void DiscoveredIncludedService(NSError? error, CBService service)
    {
        // Placeholder for future implementation
    }

    /// <inheritdoc />
    public void DiscoveredCharacteristics(NSError? error, CBService service)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(CbService.Characteristics);

            AppleNativeBluetoothException.ThrowIfError(error);

            Logger?.LogCharacteristicDiscoveryCompleted(Id, Device.Id, CbService.Characteristics.Length);
            OnCharacteristicsExplorationSucceeded(CbService.Characteristics, AreRepresentingTheSameObject, FromInputTypeToOutputTypeConversion);
        }
        catch (Exception e)
        {
            Logger?.LogCharacteristicDiscoveryError(Id, Device.Id, e.Message, e);
            OnCharacteristicsExplorationFailed(e);
        }

        return;

        IBluetoothRemoteCharacteristic FromInputTypeToOutputTypeConversion(CBCharacteristic native)
        {
            var spec = new AppleBluetoothRemoteCharacteristicFactorySpec(native);
            return (CharacteristicFactory ?? throw new InvalidOperationException("CharacteristicFactory must be initialized via the spec-based constructor.")).Create(this, spec);
        }
    }

    /// <inheritdoc />
    public CbPeripheralWrapper.ICbCharacteristicDelegate GetCharacteristic(CBCharacteristic? characteristic)
    {
        if (characteristic == null)
        {
            throw new CharacteristicNotFoundException(this);
        }

        try
        {
            var match = GetCharacteristic(sharedCharacteristic => AreRepresentingTheSameObject(characteristic, sharedCharacteristic));
            return match as CbPeripheralWrapper.ICbCharacteristicDelegate ?? throw new CharacteristicNotFoundException(this, characteristic.UUID.ToGuid());
        }
        catch (InvalidOperationException e)
        {
            var matches = GetCharacteristics(sharedCharacteristic => AreRepresentingTheSameObject(characteristic, sharedCharacteristic)).ToArray();
            throw new MultipleCharacteristicsFoundException(this, matches, e);
        }
    }

    /// <inheritdoc />
    protected override ValueTask NativeCharacteristicsExplorationAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(CbService.Peripheral);
        Logger?.LogCharacteristicDiscoveryStarting(Id, Device.Id);
        CbService.Peripheral.DiscoverCharacteristics(CbService);
        return ValueTask.CompletedTask;
    }

    private static bool AreRepresentingTheSameObject(CBCharacteristic native, IBluetoothRemoteCharacteristic shared)
    {
        return shared is AppleBluetoothRemoteCharacteristic s && native.UUID.Equals(s.CbCharacteristic.UUID) && native.Handle.Handle.Equals(s.CbCharacteristic.Handle.Handle);
    }
}
