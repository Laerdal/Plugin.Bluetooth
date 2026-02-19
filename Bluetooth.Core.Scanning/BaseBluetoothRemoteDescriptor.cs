namespace Bluetooth.Core.Scanning;

/// <inheritdoc cref="IBluetoothRemoteDescriptor" />
public abstract partial class BaseBluetoothRemoteDescriptor : BaseBindableObject, IBluetoothRemoteDescriptor
{
    /// <inheritdoc/>
    public IBluetoothRemoteCharacteristic RemoteCharacteristic { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseBluetoothRemoteDescriptor"/> class.
    /// </summary>
    /// <param name="remoteCharacteristic">The Bluetooth characteristic associated with this descriptor.</param>
    /// <param name="request">The factory request containing initialization data for the descriptor.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="remoteCharacteristic"/> is null.</exception>
    protected BaseBluetoothRemoteDescriptor(IBluetoothRemoteCharacteristic remoteCharacteristic, IBluetoothDescriptorFactory.BluetoothDescriptorFactoryRequest request)
    {
        ArgumentNullException.ThrowIfNull(remoteCharacteristic);
        ArgumentNullException.ThrowIfNull(request);
        
        RemoteCharacteristic = remoteCharacteristic;
        Id = request.DescriptorId;

        LazyCanRead = new Lazy<bool>(NativeCanRead);
        LazyCanWrite = new Lazy<bool>(NativeCanWrite);
    }

    /// <inheritdoc/>
    public Guid Id { get; }

    /// <inheritdoc/>
    public string Name { get; } = "Unknown Descriptor";

    #region Dispose

    /// <summary>
    /// Performs the core disposal logic for the descriptor.
    /// </summary>
    protected virtual ValueTask DisposeAsyncCore()
    {
        // Cancel any pending operations
        ReadValueTcs?.TrySetCanceled();
        WriteValueTcs?.TrySetCanceled();

        // Unsubscribe from events
        ValueUpdated = null;

        return ValueTask.CompletedTask;
    }

    /// <inheritdoc/>
    public async ValueTask DisposeAsync()
    {
        await DisposeAsyncCore().ConfigureAwait(false);
        GC.SuppressFinalize(this);
    }

    #endregion

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"[{Id}] {Name}";
    }
}
