namespace Bluetooth.Abstractions.Broadcasting;

/// <summary>
/// Represents a descriptor in the context of Bluetooth broadcasting.
/// Descriptors provide additional information about a characteristic.
/// </summary>
public partial interface IBluetoothLocalDescriptor
{
    #region Read/Write Requests

    /// <summary>
    /// Event raised when a client device requests to read this descriptor.
    /// </summary>
    event EventHandler<DescriptorReadRequestEventArgs>? ReadRequested;

    /// <summary>
    /// Event raised when a client device requests to write to this descriptor.
    /// </summary>
    event EventHandler<DescriptorWriteRequestEventArgs>? WriteRequested;

    #endregion
}
