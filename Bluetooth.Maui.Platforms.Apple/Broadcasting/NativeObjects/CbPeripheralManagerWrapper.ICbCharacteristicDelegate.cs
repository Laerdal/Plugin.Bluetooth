namespace Bluetooth.Maui.Platforms.Apple.Broadcasting.NativeObjects;

// Mapping native APIs leads to unclean interfaces, ignoring warnings here
#pragma warning disable CA1711 // Identifiers should not have incorrect suffix
#pragma warning disable CA1716 // Identifiers should not match keywords
public partial class CbPeripheralManagerWrapper
{
    /// <summary>
    ///     Delegate interface for CoreBluetooth characteristic callbacks, extending the base Bluetooth broadcast characteristic interface.
    /// </summary>
    public interface ICbCharacteristicDelegate
    {
        /// <summary>
        ///     Called when a central subscribes to a characteristic.
        /// </summary>
        /// <param name="central">The central that subscribed to the characteristic.</param>
        /// <param name="characteristic">The characteristic that was subscribed to.</param>
        void CharacteristicSubscribed(CBCentral central, CBCharacteristic characteristic);

        /// <summary>
        ///     Called when a central unsubscribes from a characteristic.
        /// </summary>
        /// <param name="central">The central that unsubscribed from the characteristic.</param>
        /// <param name="characteristic">The characteristic that was unsubscribed from.</param>
        void CharacteristicUnsubscribed(CBCentral central, CBCharacteristic characteristic);

        /// <summary>
        ///     Called when a read request is received from a central.
        /// </summary>
        /// <param name="request">The read request from the central.</param>
        void ReadRequestReceived(CBATTRequest request);

        /// <summary>
        ///     Called when write requests are received from centrals.
        /// </summary>
        /// <param name="request">The array of write requests from centrals.</param>
        void WriteRequestsReceived(CBATTRequest request);
    }
}
#pragma warning restore CA1711 // Identifiers should not have incorrect suffix
#pragma warning restore CA1716 // Identifiers should not match keywords