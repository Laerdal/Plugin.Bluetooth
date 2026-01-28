namespace Bluetooth.Maui.Platforms.Apple.Scanning.NativeObjects;

// Mapping native APIs leads to unclean interfaces, ignoring warnings here
#pragma warning disable CA1034 // Nested types should not be visible
#pragma warning disable CA1711 // Identifiers should not have incorrect suffix
#pragma warning disable CA1716 // Identifiers should not match keywords

public partial class CbCentralManagerWrapper
{
    /// <summary>
    /// Delegate interface for CoreBluetooth peripheral callbacks, extending the base Bluetooth device interface.
    /// </summary>
    public interface ICbPeripheralDelegate : IBluetoothDevice
    {
        /// <summary>
        /// Called when a connection attempt to the peripheral fails.
        /// </summary>
        /// <param name="error">The error that occurred during the connection attempt, or null if successful.</param>
        void FailedToConnectPeripheral(NSError? error);

        /// <summary>
        /// Called when the peripheral is disconnected (legacy method).
        /// </summary>
        /// <param name="error">The error that caused the disconnection, or null if disconnected normally.</param>
        void DisconnectedPeripheral(NSError? error);

        /// <summary>
        /// Called when the peripheral is successfully connected.
        /// </summary>
        void ConnectedPeripheral();

        /// <summary>
        /// Called when a connection event occurs for the peripheral.
        /// </summary>
        /// <param name="connectionEvent">The connection event that occurred.</param>
        void ConnectionEventDidOccur(CBConnectionEvent connectionEvent);

        /// <summary>
        /// Called when the ANCS (Apple Notification Center Service) authorization is updated.
        /// </summary>
        void DidUpdateAncsAuthorization();

        /// <summary>
        /// Called when the peripheral is disconnected with additional timing and reconnection information.
        /// </summary>
        /// <param name="timestamp">The timestamp when the disconnection occurred.</param>
        /// <param name="isReconnecting">Whether the system is attempting to reconnect automatically.</param>
        /// <param name="error">The error that caused the disconnection, or null if disconnected normally.</param>
        void DidDisconnectPeripheral(double timestamp, bool isReconnecting, NSError? error);
    }
}

#pragma warning restore CA1034 // Nested types should not be visible
#pragma warning restore CA1711 // Identifiers should not have incorrect suffix
#pragma warning restore CA1716 // Identifiers should not match keywords
