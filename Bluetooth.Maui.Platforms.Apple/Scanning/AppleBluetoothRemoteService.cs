using Bluetooth.Maui.Platforms.Apple.Logging;
using Bluetooth.Maui.Platforms.Apple.Scanning.Factories;
using Bluetooth.Maui.Platforms.Apple.Scanning.NativeObjects;

using CharacteristicNotFoundException = Bluetooth.Abstractions.Scanning.Exceptions.CharacteristicNotFoundException;
using MultipleCharacteristicsFoundException = Bluetooth.Abstractions.Scanning.Exceptions.MultipleCharacteristicsFoundException;

namespace Bluetooth.Maui.Platforms.Apple.Scanning;

/// <inheritdoc cref="BaseBluetoothRemoteService" />
public class AppleBluetoothRemoteService : BaseBluetoothRemoteService, CbPeripheralWrapper.ICbServiceDelegate
{
    /// <inheritdoc />
    public AppleBluetoothRemoteService(IBluetoothRemoteDevice device, IBluetoothRemoteServiceFactory.BluetoothRemoteServiceFactorySpec spec, IBluetoothRemoteCharacteristicFactory characteristicFactory) : base(device, spec, characteristicFactory)
    {
        ArgumentNullException.ThrowIfNull(spec);
        if (spec is not AppleBluetoothRemoteServiceFactorySpec nativeSpec)
        {
            throw new ArgumentException($"Expected spec of type {typeof(AppleBluetoothRemoteServiceFactorySpec)}, but got {spec.GetType()}");
        }

        CbService = nativeSpec.CbService;
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
            return CharacteristicFactory.Create(this, spec);
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
