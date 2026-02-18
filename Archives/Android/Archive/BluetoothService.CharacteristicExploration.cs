using Bluetooth.Maui.PlatformSpecific;

namespace Bluetooth.Maui;

public partial class BluetoothService
{
    /// <inheritdoc/>
    /// <remarks>
    /// On Android, characteristics are discovered at the same time as services during service discovery.
    /// This method validates that characteristics are available and processes them.
    /// </remarks>
    /// <exception cref="CharacteristicExplorationException">Thrown when <see cref="BluetoothGattService.Characteristics"/> is <c>null</c>.</exception>
    protected override ValueTask NativeCharacteristicsExplorationAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        if (NativeService.Characteristics == null)
        {
            throw new CharacteristicExplorationException("NativeService.Characteristics == null");

            // MIGHT NEED Retries and/or delays
        }

        // on android, characteristics are scanned at the same time the Service is discovered
        OnCharacteristicsExplorationSucceeded(NativeService.Characteristics, FromInputTypeToOutputTypeConversion, AreRepresentingTheSameObject);

        return ValueTask.CompletedTask;

    }

    /// <summary>
    /// Converts a native Android GATT characteristic to a shared <see cref="BluetoothCharacteristic"/> instance.
    /// </summary>
    /// <param name="native">The native Android GATT characteristic.</param>
    /// <returns>A new <see cref="BluetoothCharacteristic"/> wrapping the native characteristic.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the native characteristic's UUID is <c>null</c>.</exception>
    private BluetoothCharacteristic FromInputTypeToOutputTypeConversion(BluetoothGattCharacteristic native)
    {
        var id = native.Uuid?.ToGuid() ?? throw new InvalidOperationException("nativeService.Uuid == null");
        return new BluetoothCharacteristic(this, id, native);
    }

    /// <summary>
    /// Compares a native characteristic to a shared characteristic to determine if they represent the same object.
    /// </summary>
    /// <param name="native">Native Android GATT characteristic.</param>
    /// <param name="shared">Shared characteristic interface.</param>
    /// <returns><c>true</c> if the shared characteristic wraps the same native characteristic; otherwise, <c>false</c>.</returns>
    /// <remarks>
    /// Comparison is based on UUID and InstanceId equality.
    /// </remarks>
    private static bool AreRepresentingTheSameObject(BluetoothGattCharacteristic native, IBluetoothCharacteristic shared)
    {
        return shared is BluetoothCharacteristic s && (native.Uuid?.Equals(s.NativeCharacteristic.Uuid) ?? false) && native.InstanceId.Equals(s.NativeCharacteristic.InstanceId);
    }

    /// <summary>
    /// Gets the characteristic proxy for a given native Android GATT characteristic.
    /// </summary>
    /// <param name="nativeCharacteristic">The native characteristic to find.</param>
    /// <returns>The characteristic proxy for the native characteristic.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="nativeCharacteristic"/> is <c>null</c>.</exception>
    /// <exception cref="CharacteristicNotFoundException">Thrown when the characteristic is not found in the characteristics list.</exception>
    /// <exception cref="MultipleCharacteristicsFoundException">Thrown when multiple characteristics match the native characteristic.</exception>
    public BluetoothGattProxy.ICharacteristic GetCharacteristic(BluetoothGattCharacteristic? nativeCharacteristic)
    {
        ArgumentNullException.ThrowIfNull(nativeCharacteristic);

        try
        {
            var match = Characteristics.OfType<BluetoothCharacteristic>().SingleOrDefault(characteristic => AreRepresentingTheSameObject(nativeCharacteristic, characteristic));

            if (match == null)
            {
                throw new CharacteristicNotFoundException(this, nativeCharacteristic.Uuid?.ToGuid());
            }

            return match;
        }
        catch (InvalidOperationException e)
        {
            var matches = Characteristics.OfType<BluetoothCharacteristic>().Where(characteristic => AreRepresentingTheSameObject(nativeCharacteristic, characteristic)).ToArray();
            throw new MultipleCharacteristicsFoundException(this, matches, e);
        }
    }
}
