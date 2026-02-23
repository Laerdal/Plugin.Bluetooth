namespace Bluetooth.Maui.Platforms.Apple.Scanning.NativeObjects;

// Mapping native APIs leads to unclean interfaces, ignoring warnings here
#pragma warning disable CA1711 // Identifiers should not have incorrect suffix
#pragma warning disable CA1716 // Identifiers should not match keywords

public partial class CbPeripheralWrapper
{
    /// <summary>
    ///     Delegate interface for CoreBluetooth service callbacks, extending the base Bluetooth service interface.
    /// </summary>
    public interface ICbServiceDelegate
    {
        /// <summary>
        ///     Called when an included service is discovered within this service.
        /// </summary>
        /// <param name="error">The error that occurred during discovery, or null if successful.</param>
        /// <param name="service">The service within which the included service was discovered.</param>
        void DiscoveredIncludedService(NSError? error, CBService service);

        /// <summary>
        ///     Called when characteristics are discovered for this service.
        /// </summary>
        /// <param name="error">The error that occurred during characteristic discovery, or null if successful.</param>
        /// <param name="service">The service for which characteristics were discovered.</param>
        void DiscoveredCharacteristics(NSError? error, CBService service);

        /// <summary>
        ///     Gets the characteristic delegate for the specified CoreBluetooth characteristic.
        /// </summary>
        /// <param name="characteristic">The CoreBluetooth characteristic to get the delegate for.</param>
        /// <returns>The characteristic delegate for the specified characteristic.</returns>
        ICbCharacteristicDelegate GetCharacteristic(CBCharacteristic? characteristic);
    }
}

#pragma warning restore CA1711 // Identifiers should not have incorrect suffix
#pragma warning restore CA1716 // Identifiers should not match keywords
