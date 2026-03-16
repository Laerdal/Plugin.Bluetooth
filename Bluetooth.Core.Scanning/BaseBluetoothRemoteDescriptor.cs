namespace Bluetooth.Core.Scanning;

/// <inheritdoc cref="IBluetoothRemoteDescriptor" />
public abstract partial class BaseBluetoothRemoteDescriptor : BaseBindableObject, IBluetoothRemoteDescriptor
{
    /// <inheritdoc />
    public IBluetoothRemoteCharacteristic Characteristic { get; }

    /// <summary>
    ///     Initializes a new instance of the <see cref="BaseBluetoothRemoteDescriptor" /> class.
    /// </summary>
    /// <param name="parentCharacteristic">The Bluetooth characteristic associated with this descriptor.</param>
    /// <param name="id">The unique identifier (UUID) of the descriptor.</param>
    /// <param name="nameProvider">An optional provider for descriptor names, used to resolve the name based on the ID.</param>
    /// <param name="logger">The logger instance to use for logging (optional).</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="parentCharacteristic" /> is null.</exception>
    protected BaseBluetoothRemoteDescriptor(IBluetoothRemoteCharacteristic parentCharacteristic,
        Guid id,
        IBluetoothNameProvider? nameProvider = null,
        ILogger<IBluetoothRemoteDescriptor>? logger = null) : base(logger)
    {
        // Validate constructor arguments
        ArgumentNullException.ThrowIfNull(parentCharacteristic);

        Characteristic = parentCharacteristic;
        Id = id;

        // Name
        if (nameProvider != null)
        {
            Name = nameProvider.GetKnownDescriptorName(Id) ?? Name;
        }

        LazyCanRead = new Lazy<bool>(NativeCanRead);
        LazyCanWrite = new Lazy<bool>(NativeCanWrite);
    }

    /// <inheritdoc />
    public Guid Id { get; }

    /// <inheritdoc />
    public string Name { get; } = "Unknown Descriptor";

    #region Dispose

    /// <summary>
    ///     Performs the core disposal logic for the descriptor.
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

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        await DisposeAsyncCore().ConfigureAwait(false);
        GC.SuppressFinalize(this);
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
