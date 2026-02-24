using Bluetooth.Maui.Platforms.Apple.Broadcasting.Factories;
using Bluetooth.Maui.Platforms.Apple.Broadcasting.NativeObjects;

using CharacteristicNotFoundException = Bluetooth.Abstractions.Broadcasting.Exceptions.CharacteristicNotFoundException;
using MultipleCharacteristicsFoundException = Bluetooth.Abstractions.Broadcasting.Exceptions.MultipleCharacteristicsFoundException;

namespace Bluetooth.Maui.Platforms.Apple.Broadcasting;

/// <inheritdoc cref="BaseBluetoothLocalService" />
public class AppleBluetoothLocalService : BaseBluetoothLocalService, CbPeripheralManagerWrapper.ICbServiceDelegate
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="AppleBluetoothLocalService" /> class with the specified broadcaster, factory spec, and characteristic factory.
    /// </summary>
    /// <param name="broadcaster">The Bluetooth broadcaster to which this service belongs.</param>
    /// <param name="spec">The factory spec containing the information needed to create this service.</param>
    /// <param name="localCharacteristicFactory">The factory used to create characteristics for this service.</param>
    public AppleBluetoothLocalService(IBluetoothBroadcaster broadcaster, IBluetoothLocalServiceFactory.BluetoothLocalServiceSpec spec, IBluetoothLocalCharacteristicFactory localCharacteristicFactory) : base(broadcaster, spec,
        localCharacteristicFactory)
    {
        ArgumentNullException.ThrowIfNull(spec);
        if (spec is not AppleBluetoothLocalServiceSpec nativeSpec)
        {
            throw new ArgumentException($"Expected spec of type {typeof(AppleBluetoothLocalServiceSpec)}, but got {spec.GetType()}");
        }

        CbService = nativeSpec.NativeService;
    }

    /// <summary>
    ///     Gets the native iOS Core Bluetooth service.
    /// </summary>
    public CBService CbService { get; }

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
}
