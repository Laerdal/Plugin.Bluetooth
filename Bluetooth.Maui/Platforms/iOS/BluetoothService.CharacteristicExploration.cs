using Bluetooth.Maui.PlatformSpecific;
using Bluetooth.Maui.PlatformSpecific.Exceptions;

namespace Bluetooth.Maui;

public partial class BluetoothService
{
    /// <inheritdoc/>
    /// <remarks>
    /// On iOS, this initiates characteristic discovery by calling <see cref="CBPeripheral.DiscoverCharacteristics(CBService)"/>.
    /// Results are delivered asynchronously via the <see cref="DiscoveredCharacteristics"/> callback.
    /// </remarks>
    /// <exception cref="ArgumentNullException">Thrown when <see cref="NativeService"/> or <see cref="CBService.Peripheral"/> is <c>null</c>.</exception>
    protected override ValueTask NativeCharacteristicsExplorationAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(NativeService);
        ArgumentNullException.ThrowIfNull(NativeService.Peripheral, nameof(NativeService.Peripheral));

        NativeService.Peripheral.DiscoverCharacteristics(NativeService);
        return ValueTask.CompletedTask;
    }

    /// <summary>
    /// Called when characteristics are discovered for this service.
    /// </summary>
    /// <param name="error">Error that occurred during discovery, or <c>null</c> if successful.</param>
    /// <param name="service">The service for which characteristics were discovered.</param>
    /// <exception cref="ArgumentNullException">Thrown when <see cref="CBService.Characteristics"/> is <c>null</c>.</exception>
    /// <exception cref="AppleNativeBluetoothException">Thrown when an error occurred during characteristic discovery.</exception>
    public void DiscoveredCharacteristics(NSError? error, CBService service)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(NativeService.Characteristics, nameof(NativeService.Characteristics));

            AppleNativeBluetoothException.ThrowIfError(error);

            OnCharacteristicsExplorationSucceeded(NativeService.Characteristics, FromInputTypeToOutputTypeConversion, AreRepresentingTheSameObject);
        }
        catch (Exception e)
        {
            OnCharacteristicsExplorationFailed(e);
        }

        return;

        BluetoothCharacteristic FromInputTypeToOutputTypeConversion(CBCharacteristic native)
        {
            return new BluetoothCharacteristic(this, native.UUID.ToGuid(), native);
        }
    }

    /// <summary>
    /// Compares a native characteristic to a shared characteristic to determine if they represent the same object.
    /// </summary>
    /// <param name="native">Native iOS Core Bluetooth characteristic.</param>
    /// <param name="shared">Shared characteristic interface.</param>
    /// <returns><c>true</c> if the shared characteristic wraps the same native characteristic; otherwise, <c>false</c>.</returns>
    /// <remarks>
    /// Comparison is based on UUID and handle equality.
    /// </remarks>
    private static bool AreRepresentingTheSameObject(CBCharacteristic native, IBluetoothCharacteristic shared)
    {
        return shared is BluetoothCharacteristic s && native.UUID.Equals(s.NativeCharacteristic.UUID) && native.Handle.Handle.Equals(s.NativeCharacteristic.Handle.Handle);
    }

    /// <summary>
    /// Gets the characteristic proxy for a given native iOS Core Bluetooth characteristic.
    /// </summary>
    /// <param name="characteristic">The native characteristic to find.</param>
    /// <returns>The characteristic proxy for the native characteristic.</returns>
    /// <exception cref="CharacteristicNotFoundException">Thrown when the characteristic is <c>null</c> or not found in the characteristics list.</exception>
    /// <exception cref="MultipleCharacteristicsFoundException">Thrown when multiple characteristics match the native characteristic.</exception>
    public CbPeripheralProxy.ICbCharacteristicDelegate GetCharacteristic(CBCharacteristic? characteristic)
    {
        if (characteristic == null)
        {
            throw new CharacteristicNotFoundException(this, null);
        }

        try
        {
            var match = Characteristics.OfType<BluetoothCharacteristic>().SingleOrDefault(sharedCharacteristic => AreRepresentingTheSameObject(characteristic, sharedCharacteristic));

            if (match == null)
            {
                throw new CharacteristicNotFoundException(this, characteristic.UUID.ToGuid());
            }

            return match;
        }
        catch (InvalidOperationException e)
        {
            var matches = Characteristics.OfType<BluetoothCharacteristic>().Where(sharedCharacteristic => AreRepresentingTheSameObject(characteristic, sharedCharacteristic)).ToArray();
            throw new MultipleCharacteristicsFoundException(this, matches, e);
        }
    }
}
