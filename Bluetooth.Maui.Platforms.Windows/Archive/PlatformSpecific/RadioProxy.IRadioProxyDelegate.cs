namespace Bluetooth.Maui.PlatformSpecific;

#pragma warning disable CA1034 // Nested types should not be visible
#pragma warning disable CA1711 // Identifiers should not have incorrect suffix

public partial class RadioProxy
{
    /// <summary>
    /// Delegate interface for handling radio operations and events.
    /// Extends the base Bluetooth manager interface with Windows-specific radio callbacks.
    /// </summary>
    public interface IRadioProxyDelegate
    {
        /// <summary>
        /// Called when the radio state changes (e.g., turning on/off).
        /// </summary>
        /// <param name="senderState">The new radio state.</param>
        void OnRadioStateChanged(RadioState senderState);
    }
}

#pragma warning restore CA1034 // Nested types should not be visible
#pragma warning restore CA1711 // Identifiers should not have incorrect suffix
