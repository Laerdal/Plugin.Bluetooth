namespace Bluetooth.Core.Broadcasting;

/// <summary>
///     Base class for Bluetooth broadcast descriptors.
/// </summary>
public abstract partial class BaseBluetoothLocalDescriptor : BaseBindableObject, IBluetoothLocalDescriptor
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="BaseBluetoothLocalDescriptor" /> class.
    /// </summary>
    protected BaseBluetoothLocalDescriptor(
        IBluetoothLocalCharacteristic localCharacteristic,
        IBluetoothLocalDescriptorFactory.BluetoothLocalDescriptorSpec request)
    {
        ArgumentNullException.ThrowIfNull(localCharacteristic);
        ArgumentNullException.ThrowIfNull(request);

        LocalCharacteristic = localCharacteristic;

        Id = request.Id;
        Value = request.InitialValue ?? ReadOnlyMemory<byte>.Empty;
    }


    /// <inheritdoc />
    public IBluetoothLocalCharacteristic LocalCharacteristic { get; }

    /// <inheritdoc />
    public Guid Id { get; }

    /// <inheritdoc />
    public string Name { get; } = "Unknown Descriptor";

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        await DisposeAsyncCore().ConfigureAwait(false);
        GC.SuppressFinalize(this);
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"[{Id}] {Name}";
    }

    /// <summary>
    ///     Disposes the resources asynchronously.
    /// </summary>
    protected virtual ValueTask DisposeAsyncCore()
    {
        return ValueTask.CompletedTask;
    }

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
    protected DescriptorWriteRequestEventArgs OnWriteRequested(
        IBluetoothConnectedDevice device,
        ReadOnlyMemory<byte> value,
        int offset,
        bool isPreparedWrite,
        bool responseNeeded)
    {
        var args = new DescriptorWriteRequestEventArgs(
            device,
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
}
