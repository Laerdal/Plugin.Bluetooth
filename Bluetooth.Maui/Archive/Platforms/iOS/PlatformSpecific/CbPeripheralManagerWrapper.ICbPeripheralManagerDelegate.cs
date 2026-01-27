namespace Bluetooth.Maui.PlatformSpecific;

// Mapping native APIs leads to unclean interfaces, ignoring warnings here
#pragma warning disable CA1034 // Nested types should not be visible
#pragma warning disable CA1711 // Identifiers should not have incorrect suffix
#pragma warning disable CA1716 // Identifiers should not match keywords

public partial class CbPeripheralManagerWrapper
{
    /// <summary>
    /// Delegate interface for CoreBluetooth peripheral manager callbacks, extending the base Bluetooth broadcaster interface.
    /// </summary>
    public interface ICbPeripheralManagerDelegate : IBluetoothBroadcaster
    {
        /// <summary>
        /// Called when advertising starts or fails to start.
        /// </summary>
        /// <param name="error">The error that occurred during advertising startup, or null if successful.</param>
        void AdvertisingStarted(NSError? error);

        /// <summary>
        /// Called when a service is successfully added to the peripheral manager.
        /// </summary>
        /// <param name="service">The service that was added.</param>
        void ServiceAdded(CBService service);

        /// <summary>
        /// Called when the peripheral manager will restore state from a previous session.
        /// </summary>
        /// <param name="dict">The dictionary containing the state information to restore.</param>
        void WillRestoreState(NSDictionary dict);

        /// <summary>
        /// Called when an L2CAP channel is opened.
        /// </summary>
        /// <param name="error">The error that occurred during channel opening, or null if successful.</param>
        /// <param name="channel">The L2CAP channel that was opened, or null if failed.</param>
        void DidOpenL2CapChannel(NSError? error, CBL2CapChannel? channel);

        /// <summary>
        /// Called when an L2CAP channel is published.
        /// </summary>
        /// <param name="error">The error that occurred during channel publishing, or null if successful.</param>
        /// <param name="psm">The Protocol/Service Multiplexer (PSM) value for the published channel.</param>
        void DidPublishL2CapChannel(NSError? error, ushort psm);

        /// <summary>
        /// Called when an L2CAP channel is unpublished.
        /// </summary>
        /// <param name="error">The error that occurred during channel unpublishing, or null if successful.</param>
        /// <param name="psm">The Protocol/Service Multiplexer (PSM) value for the unpublished channel.</param>
        void DidUnpublishL2CapChannel(NSError? error, ushort psm);

        /// <summary>
        /// Called when the peripheral manager's state is updated.
        /// </summary>
        /// <param name="peripheralState"></param>
        void StateUpdated(CBManagerState peripheralState);

        /// <summary>
        /// Gets the service delegate for the specified CoreBluetooth service.
        /// </summary>
        /// <param name="characteristicService">The CoreBluetooth service to get the delegate for.</param>
        /// <returns>The service delegate for the specified service.</returns>
        ICbServiceDelegate GetService(CBService? characteristicService);
    }
}

#pragma warning restore CA1034 // Nested types should not be visible
#pragma warning restore CA1711 // Identifiers should not have incorrect suffix
#pragma warning restore CA1716 // Identifiers should not match keywords
