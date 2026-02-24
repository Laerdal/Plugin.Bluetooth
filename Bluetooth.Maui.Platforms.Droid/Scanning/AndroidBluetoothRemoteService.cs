using Bluetooth.Maui.Platforms.Droid.Exceptions;
using Bluetooth.Maui.Platforms.Droid.Scanning.Factories;
using Bluetooth.Maui.Platforms.Droid.Scanning.NativeObjects;
using Bluetooth.Maui.Platforms.Droid.Tools;

using CharacteristicNotFoundException = Bluetooth.Abstractions.Scanning.Exceptions.CharacteristicNotFoundException;
using MultipleCharacteristicsFoundException = Bluetooth.Abstractions.Scanning.Exceptions.MultipleCharacteristicsFoundException;

namespace Bluetooth.Maui.Platforms.Droid.Scanning;

/// <summary>
///     Android implementation of a Bluetooth Low Energy remote service.
///     This class wraps Android's BluetoothGattService, providing platform-specific
///     implementations for characteristic discovery and management.
/// </summary>
public class AndroidBluetoothRemoteService : BaseBluetoothRemoteService, BluetoothGattProxy.IBluetoothGattServiceDelegate
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="AndroidBluetoothRemoteService" /> class.
    /// </summary>
    /// <param name="device">The Bluetooth device that owns this service.</param>
    /// <param name="spec">The factory spec containing service information.</param>
    /// <param name="characteristicFactory">The factory for creating characteristics.</param>
    public AndroidBluetoothRemoteService(IBluetoothRemoteDevice device, IBluetoothRemoteServiceFactory.BluetoothRemoteServiceFactorySpec spec, IBluetoothRemoteCharacteristicFactory characteristicFactory) :
        base(device, spec, characteristicFactory)
    {
        ArgumentNullException.ThrowIfNull(spec);
        if (spec is not AndroidBluetoothRemoteServiceFactorySpec nativeSpec)
        {
            throw new ArgumentException($"Expected spec of type {typeof(AndroidBluetoothRemoteServiceFactorySpec)}, but got {spec.GetType()}");
        }

        NativeService = nativeSpec.NativeService;
    }

    /// <summary>
    ///     Gets the native Android GATT service.
    /// </summary>
    public BluetoothGattService NativeService { get; }

    /// <summary>
    ///     Gets the Bluetooth device to which this service belongs, cast to the Android-specific implementation.
    /// </summary>
    public AndroidBluetoothRemoteDevice AndroidBluetoothRemoteDevice =>
        (AndroidBluetoothRemoteDevice) Device;

    #region BluetoothGattProxy.IBluetoothGattServiceDelegate Implementation

    /// <inheritdoc />
    public BluetoothGattProxy.IBluetoothGattCharacteristicDelegate GetCharacteristic(BluetoothGattCharacteristic? nativeCharacteristic)
    {
        if (nativeCharacteristic == null)
        {
            throw new CharacteristicNotFoundException(this);
        }

        try
        {
            var match = GetCharacteristic(characteristic => AreRepresentingTheSameObject(nativeCharacteristic, characteristic));
            return match as BluetoothGattProxy.IBluetoothGattCharacteristicDelegate ?? throw new CharacteristicNotFoundException(this, nativeCharacteristic.Uuid.ToGuid());
        }
        catch (InvalidOperationException e)
        {
            var matches = GetCharacteristics(characteristic => AreRepresentingTheSameObject(nativeCharacteristic, characteristic)).ToArray();
            throw new MultipleCharacteristicsFoundException(this, matches, e);
        }
    }

    private static bool AreRepresentingTheSameObject(BluetoothGattCharacteristic native, IBluetoothRemoteCharacteristic shared)
    {
        return shared is AndroidBluetoothRemoteCharacteristic androidCharacteristic && native.Uuid?.Equals(androidCharacteristic.NativeCharacteristic.Uuid) == true;
    }

    #endregion

    #region Characteristic Discovery

    /// <inheritdoc />
    protected override ValueTask NativeCharacteristicsExplorationAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        try
        {
            // On Android, characteristics are discovered along with services
            // The NativeService.Characteristics property should already contain all characteristics
            var characteristics = NativeService.Characteristics ?? new List<BluetoothGattCharacteristic>();
            OnCharacteristicsExplorationSucceeded(characteristics, AreRepresentingTheSameObject, FromInputTypeToOutputTypeConversion);
        }
        catch (Exception ex)
        {
            OnCharacteristicsExplorationFailed(ex);
        }

        return ValueTask.CompletedTask;

        IBluetoothRemoteCharacteristic FromInputTypeToOutputTypeConversion(BluetoothGattCharacteristic nativeCharacteristic)
        {
            var spec = new AndroidBluetoothRemoteCharacteristicFactorySpec(nativeCharacteristic);
            return CharacteristicFactory.Create(this, spec);
        }
    }

    #endregion

}
