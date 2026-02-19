using Bluetooth.Maui.Platforms.Apple.Scanning.Factories;
using Bluetooth.Maui.Platforms.Apple.Scanning.NativeObjects;

namespace Bluetooth.Maui.Platforms.Apple.Scanning;

/// <inheritdoc cref="BaseBluetoothRemoteService" />
public class AppleBluetoothRemoteService : Core.Scanning.BaseBluetoothRemoteService, CbPeripheralWrapper.ICbServiceDelegate
{

    /// <summary>
    /// Gets the native iOS Core Bluetooth service.
    /// </summary>
    public CBService CbService { get; }

    /// <summary>
    /// Gets the Bluetooth device to which this service belongs, cast to the Apple-specific implementation.
    /// </summary>
    public AppleBluetoothRemoteDevice AppleBluetoothRemoteDevice => (AppleBluetoothRemoteDevice) Device;

    /// <inheritdoc />
    public AppleBluetoothRemoteService(IBluetoothRemoteDevice device, IBluetoothServiceFactory.BluetoothServiceFactoryRequest request, IBluetoothCharacteristicFactory characteristicFactory) : base(device, request, characteristicFactory)
    {
        ArgumentNullException.ThrowIfNull(request);
        if (request is not AppleBluetoothServiceFactoryRequest appleRequest)
        {
            throw new ArgumentException($"Expected request of type {typeof(AppleBluetoothServiceFactoryRequest)}, but got {request.GetType()}");
        }
        CbService = appleRequest.CbService;
    }

    /// <inheritdoc />
    protected override ValueTask NativeCharacteristicsExplorationAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(CbService.Peripheral);
        CbService.Peripheral.DiscoverCharacteristics(CbService);
        return ValueTask.CompletedTask;
    }

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

            OnCharacteristicsExplorationSucceeded(CbService.Characteristics, FromInputTypeToOutputTypeConversion, AreRepresentingTheSameObject);
        }
        catch (Exception e)
        {
            OnCharacteristicsExplorationFailed(e);
        }

        return;

        Abstractions.Scanning.IBluetoothRemoteCharacteristic FromInputTypeToOutputTypeConversion(CBCharacteristic native)
        {
            var request = new AppleBluetoothCharacteristicFactoryRequest(native);
            return CharacteristicFactory.CreateCharacteristic(this, request);
        }
    }

    /// <inheritdoc />
    public CbPeripheralWrapper.ICbCharacteristicDelegate GetCharacteristic(CBCharacteristic? characteristic)
    {
        if (characteristic == null)
        {
            throw new Abstractions.Scanning.Exceptions.CharacteristicNotFoundException(this);
        }

        try
        {
            var match = GetCharacteristic(sharedCharacteristic => AreRepresentingTheSameObject(characteristic, sharedCharacteristic));
            return match as CbPeripheralWrapper.ICbCharacteristicDelegate ?? throw new Abstractions.Scanning.Exceptions.CharacteristicNotFoundException(this, characteristic.UUID.ToGuid());
        }
        catch (InvalidOperationException e)
        {
            var matches = GetCharacteristics(sharedCharacteristic => AreRepresentingTheSameObject(characteristic, sharedCharacteristic)).ToArray();
            throw new Abstractions.Scanning.Exceptions.MultipleCharacteristicsFoundException(this, matches, e);
        }
    }

    private static bool AreRepresentingTheSameObject(CBCharacteristic native, Abstractions.Scanning.IBluetoothRemoteCharacteristic shared)
    {
        return shared is AppleBluetoothRemoteCharacteristic s && native.UUID.Equals(s.CbCharacteristic.UUID) && native.Handle.Handle.Equals(s.CbCharacteristic.Handle.Handle);
    }
}
