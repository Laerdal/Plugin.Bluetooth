namespace Bluetooth.Abstractions.Broadcasting.EventArgs;

/// <summary>
///     Event arguments for descriptor read request events from a client device.
/// </summary>
public class DescriptorReadRequestEventArgs : System.EventArgs
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="DescriptorReadRequestEventArgs" /> class.
    /// </summary>
    /// <param name="device">The client device making the read request.</param>
    /// <param name="localDescriptor">The descriptor being read.</param>
    /// <param name="offset">The offset at which to start reading.</param>
    public DescriptorReadRequestEventArgs(
        IBluetoothConnectedDevice device,
        IBluetoothLocalDescriptor localDescriptor,
        int offset)
    {
        Device = device;
        LocalDescriptor = localDescriptor;
        Offset = offset;
    }

    /// <summary>
    ///     Gets the client device making the read request.
    /// </summary>
    public IBluetoothConnectedDevice Device { get; }

    /// <summary>
    ///     Gets the descriptor being read.
    /// </summary>
    public IBluetoothLocalDescriptor LocalDescriptor { get; }

    /// <summary>
    ///     Gets the offset at which to start reading.
    /// </summary>
    public int Offset { get; }

    /// <summary>
    ///     Gets or sets the value to return to the client.
    ///     Set this property to provide the response data.
    /// </summary>
    public ReadOnlyMemory<byte>? ResponseValue { get; set; }
}
