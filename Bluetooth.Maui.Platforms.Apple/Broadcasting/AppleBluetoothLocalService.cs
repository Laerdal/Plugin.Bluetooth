using Bluetooth.Abstractions.Broadcasting.Exceptions;
using Bluetooth.Abstractions.Broadcasting.Factories;
using Bluetooth.Maui.Platforms.Apple.Broadcasting.Factories;
using Bluetooth.Maui.Platforms.Apple.Broadcasting.NativeObjects;
using Bluetooth.Maui.Platforms.Apple.Tools;

namespace Bluetooth.Maui.Platforms.Apple.Broadcasting;

/// <inheritdoc cref="BaseBluetoothLocalService" />
public class AppleBluetoothLocalService : Core.Broadcasting.BaseBluetoothLocalService, CbPeripheralManagerWrapper.ICbServiceDelegate
{
    /// <summary>
    /// Gets the native iOS Core Bluetooth service.
    /// </summary>
    public CBService CbService { get; }

    /// <summary>
    /// Gets the Bluetooth broadcaster to which this service belongs, cast to the Apple-specific implementation.
    /// </summary>
    public AppleBluetoothBroadcaster AppleBluetoothBroadcaster => (AppleBluetoothBroadcaster) Broadcaster;

    /// <summary>
    /// Initializes a new instance of the <see cref="AppleBluetoothLocalService"/> class with the specified broadcaster, factory request, and characteristic factory.
    /// </summary>
    /// <param name="broadcaster">The Bluetooth broadcaster to which this service belongs.</param>
    /// <param name="request">The factory request containing the information needed to create this service.</param>
    /// <param name="localCharacteristicFactory">The factory used to create characteristics for this service.</param>
    public AppleBluetoothLocalService(IBluetoothBroadcaster broadcaster, IBluetoothLocalServiceFactory.BluetoothLocalServiceSpec request, IBluetoothLocalCharacteristicFactory localCharacteristicFactory) : base(broadcaster, request, localCharacteristicFactory)
    {
        ArgumentNullException.ThrowIfNull(request);
        if (request is not AppleBluetoothServiceFactoryRequest appleRequest)
        {
            throw new ArgumentException($"Expected request of type {typeof(AppleBluetoothServiceFactoryRequest)}, but got {request.GetType()}");
        }
        CbService = appleRequest.NativeService;
    }

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

    private static bool AreRepresentingTheSameObject(CBCharacteristic native, Abstractions.Broadcasting.IBluetoothLocalCharacteristic shared)
    {
        return shared is AppleBluetoothLocalCharacteristic s && native.UUID.Equals(s.CbCharacteristic.UUID);
    }
}
