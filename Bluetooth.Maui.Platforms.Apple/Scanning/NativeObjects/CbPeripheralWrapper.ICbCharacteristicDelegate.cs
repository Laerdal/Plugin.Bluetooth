namespace Bluetooth.Maui.Platforms.Apple.Scanning.NativeObjects;

// Mapping native APIs leads to unclean interfaces, ignoring warnings here
#pragma warning disable CA1711 // Identifiers should not have incorrect suffix
#pragma warning disable CA1716 // Identifiers should not match keywords
public partial class CbPeripheralWrapper
{
    /// <summary>
    ///     Delegate interface for CoreBluetooth characteristic callbacks, extending the base Bluetooth characteristic interface.
    /// </summary>
    public interface ICbCharacteristicDelegate
    {
        /// <summary>
        ///     Called when a descriptor is discovered for the characteristic.
        /// </summary>
        /// <param name="error">The error that occurred during discovery, or null if successful.</param>
        /// <param name="characteristic">The characteristic for which the descriptor was discovered.</param>
        void DiscoveredDescriptor(NSError? error, CBCharacteristic characteristic);

        /// <summary>
        ///     Called when a write operation to the characteristic value completes.
        /// </summary>
        /// <param name="error">The error that occurred during writing, or null if successful.</param>
        /// <param name="characteristic">The characteristic that was written to.</param>
        void WroteCharacteristicValue(NSError? error, CBCharacteristic characteristic);

        /// <summary>
        ///     Called when the characteristic value is updated (typically from notifications or indications).
        /// </summary>
        /// <param name="error">The error that occurred during the update, or null if successful.</param>
        /// <param name="characteristic">The characteristic whose value was updated.</param>
        void UpdatedCharacteristicValue(NSError? error, CBCharacteristic characteristic);

        /// <summary>
        ///     Called when the notification state of the characteristic changes.
        /// </summary>
        /// <param name="error">The error that occurred during the state change, or null if successful.</param>
        /// <param name="characteristic">The characteristic whose notification state was updated.</param>
        void UpdatedNotificationState(NSError? error, CBCharacteristic characteristic);

        /// <summary>
        ///     Gets the delegate for a specific descriptor of this characteristic.
        /// </summary>
        /// <param name="native">The native CBDescriptor for which to get the delegate.</param>
        /// <returns>The delegate instance for the specified descriptor.</returns>
        ICbDescriptorDelegate GetDescriptor(CBDescriptor? native);
    }
}
#pragma warning restore CA1711 // Identifiers should not have incorrect suffix
#pragma warning restore CA1716 // Identifiers should not match keywords