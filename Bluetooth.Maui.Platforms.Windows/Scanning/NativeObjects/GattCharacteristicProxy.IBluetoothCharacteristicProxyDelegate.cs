namespace Bluetooth.Maui.Platforms.Windows;

#pragma warning disable CA1034 // Nested types should not be visible
#pragma warning disable CA1711 // Identifiers should not have incorrect suffix

public partial class GattCharacteristicProxy
{
    /// <summary>
    /// Delegate interface for handling GATT characteristic operations and events.
    /// Extends the base Bluetooth characteristic interface with Windows-specific characteristic callbacks.
    /// </summary>
    public interface IBluetoothCharacteristicProxyDelegate
    {
        /// <summary>
        /// Called when the characteristic value changes.
        /// </summary>
        /// <param name="value">The new characteristic value.</param>
        /// <param name="argsTimestamp">The timestamp when the value changed.</param>
        void OnValueChanged(byte[] value, DateTimeOffset argsTimestamp);
    }
}

#pragma warning restore CA1034 // Nested types should not be visible
#pragma warning restore CA1711 // Identifiers should not have incorrect suffix
