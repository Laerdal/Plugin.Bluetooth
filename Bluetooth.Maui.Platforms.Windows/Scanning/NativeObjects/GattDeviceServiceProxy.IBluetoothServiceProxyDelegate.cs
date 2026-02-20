namespace Bluetooth.Maui.Platforms.Windows;

#pragma warning disable CA1034 // Nested types should not be visible
#pragma warning disable CA1711 // Identifiers should not have incorrect suffix

public partial class GattDeviceServiceProxy
{
    /// <summary>
    ///     Delegate interface for handling GATT device service operations and events.
    ///     Extends the base Bluetooth service interface with Windows-specific service callbacks.
    /// </summary>
    public interface IBluetoothServiceProxyDelegate
    {
        /// <summary>
        ///     Called when the device access status changes for the service.
        /// </summary>
        /// <param name="argsId">The service identifier.</param>
        /// <param name="argsStatus">The new access status.</param>
        void OnAccessChanged(string argsId, DeviceAccessStatus argsStatus);
    }
}

#pragma warning restore CA1034 // Nested types should not be visible
#pragma warning restore CA1711 // Identifiers should not have incorrect suffix