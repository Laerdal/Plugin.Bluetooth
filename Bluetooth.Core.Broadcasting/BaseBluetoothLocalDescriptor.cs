namespace Bluetooth.Core.Broadcasting;

/// <summary>
///     Base class for Bluetooth broadcast descriptors.
/// </summary>
public abstract partial class BaseBluetoothLocalDescriptor : BaseBindableObject, IBluetoothLocalDescriptor
{
    /// <inheritdoc />
    public IBluetoothLocalCharacteristic LocalCharacteristic { get; }

    /// <summary>
    ///     Initializes a new instance of the <see cref="BaseBluetoothLocalDescriptor" /> class.
    /// </summary>
    /// <param name="localCharacteristic">The local characteristic this descriptor belongs to.</param>
    /// <param name="id">The unique identifier of the descriptor.</param>
    /// <param name="initialValue">The initial value of the descriptor (optional).</param>
    /// <param name="name">The name of the descriptor (optional).</param>
    /// <param name="logger">Optional logger for logging descriptor operations.</param>
    protected BaseBluetoothLocalDescriptor(IBluetoothLocalCharacteristic localCharacteristic,
        Guid id,
        ReadOnlyMemory<byte>? initialValue = null,
        string? name = null,
        ILogger<IBluetoothLocalDescriptor>? logger = null) : base(logger)
    {
        // Validate constructor arguments
        ArgumentNullException.ThrowIfNull(localCharacteristic);

        LocalCharacteristic = localCharacteristic;
        Id = id;
        Value = initialValue ?? ReadOnlyMemory<byte>.Empty;
        if (!string.IsNullOrWhiteSpace(name))
        {
            Name = name;
        }
    }

    /// <inheritdoc />
    public Guid Id { get; }

    /// <inheritdoc />
    public string Name { get; } = "Unknown Descriptor";

    #region Read/Write Requests

    /// <inheritdoc />
    public event EventHandler<DescriptorReadRequestEventArgs>? ReadRequested;

    /// <inheritdoc />
    public event EventHandler<DescriptorWriteRequestEventArgs>? WriteRequested;

    /// <summary>
    ///     Handles a read request from a client device.
    /// </summary>
    protected DescriptorReadRequestEventArgs OnReadRequested(IBluetoothConnectedDevice device, int offset)
    {
        var args = new DescriptorReadRequestEventArgs(device, this, offset)
        {
            ResponseValue = Value
        };

        ReadRequested?.Invoke(this, args);

        return args;
    }

    /// <summary>
    ///     Handles a write request from a client device.
    /// </summary>
    protected DescriptorWriteRequestEventArgs OnWriteRequested(IBluetoothConnectedDevice device,
        ReadOnlyMemory<byte> value,
        int offset,
        bool isPreparedWrite,
        bool responseNeeded)
    {
        var args = new DescriptorWriteRequestEventArgs(device,
                                                       this,
                                                       value,
                                                       offset,
                                                       isPreparedWrite,
                                                       responseNeeded);

        WriteRequested?.Invoke(this, args);

        if (args.Accept)
        {
            Value = value;
        }

        return args;
    }

    #endregion

    #region Dispose

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        await DisposeAsyncCore().ConfigureAwait(false);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    ///     Disposes the resources asynchronously.
    /// </summary>
    protected virtual ValueTask DisposeAsyncCore()
    {
        return ValueTask.CompletedTask;
    }

    #endregion

    #region ToString

    /// <inheritdoc />
    public override string ToString()
    {
        return $"[{Id}] {Name}";
    }

    #endregion

}
