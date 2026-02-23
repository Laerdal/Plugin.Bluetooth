namespace Bluetooth.Abstractions.Broadcasting.EventArgs;

/// <summary>
///     Event arguments for descriptor write request events from a client device.
/// </summary>
public class DescriptorWriteRequestEventArgs : System.EventArgs
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="DescriptorWriteRequestEventArgs" /> class.
    /// </summary>
    /// <param name="device">The client device making the write request.</param>
    /// <param name="localDescriptor">The descriptor being written.</param>
    /// <param name="value">The value being written.</param>
    /// <param name="offset">The offset at which to start writing.</param>
    /// <param name="isPreparedWrite">Indicates whether this is a prepared write request.</param>
    /// <param name="responseNeeded">Indicates whether a response is needed.</param>
    public DescriptorWriteRequestEventArgs(
        IBluetoothConnectedDevice device,
        IBluetoothLocalDescriptor localDescriptor,
        ReadOnlyMemory<byte> value,
        int offset,
        bool isPreparedWrite,
        bool responseNeeded)
    {
        Device = device;
        LocalDescriptor = localDescriptor;
        Value = value;
        Offset = offset;
        IsPreparedWrite = isPreparedWrite;
        ResponseNeeded = responseNeeded;
    }

    /// <summary>
    ///     Gets the client device making the write request.
    /// </summary>
    public IBluetoothConnectedDevice Device { get; }

    /// <summary>
    ///     Gets the descriptor being written.
    /// </summary>
    public IBluetoothLocalDescriptor LocalDescriptor { get; }

    /// <summary>
    ///     Gets the value being written.
    /// </summary>
    public ReadOnlyMemory<byte> Value { get; }

    /// <summary>
    ///     Gets the offset at which to start writing.
    /// </summary>
    public int Offset { get; }

    /// <summary>
    ///     Gets a value indicating whether this is a prepared write request.
    /// </summary>
    public bool IsPreparedWrite { get; }

    /// <summary>
    ///     Gets a value indicating whether a response is needed.
    /// </summary>
    public bool ResponseNeeded { get; }

    /// <summary>
    ///     Gets or sets a value indicating whether the write request should be accepted.
    ///     Set to false to reject the write request.
    /// </summary>
    public bool Accept { get; set; } = true;
}
