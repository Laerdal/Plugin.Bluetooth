namespace Bluetooth.Maui.Platforms.Apple.Scanning.NativeObjects;

// Mapping native APIs leads to unclean interfaces, ignoring warnings here
#pragma warning disable CA1711 // Identifiers should not have incorrect suffix
#pragma warning disable CA1716 // Identifiers should not match keywords
public partial class CbPeripheralWrapper
{
    /// <summary>
    ///     Delegate interface for CoreBluetooth characteristic callbacks, extending the base Bluetooth characteristic interface.
    /// </summary>
    public interface ICbDescriptorDelegate
    {
        /// <summary>
        ///     Called when a descriptor value is updated.
        /// </summary>
        /// <param name="error">The error that occurred during the update, or null if successful.</param>
        /// <param name="descriptor">The descriptor whose value was updated.</param>
        void UpdatedValue(NSError? error, CBDescriptor descriptor);

        /// <summary>
        ///     Called when a write operation to a descriptor value completes.
        /// </summary>
        /// <param name="error">The error that occurred during writing, or null if successful.</param>
        /// <param name="descriptor">The descriptor that was written to.</param>
        void WroteDescriptorValue(NSError? error, CBDescriptor descriptor);
    }
}
#pragma warning restore CA1711 // Identifiers should not have incorrect suffix
#pragma warning restore CA1716 // Identifiers should not match keywords