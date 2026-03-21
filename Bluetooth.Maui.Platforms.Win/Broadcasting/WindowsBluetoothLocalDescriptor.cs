namespace Bluetooth.Maui.Platforms.Win.Broadcasting;

/// <inheritdoc cref="BaseBluetoothLocalDescriptor" />
public class WindowsBluetoothLocalDescriptor : BaseBluetoothLocalDescriptor
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="WindowsBluetoothLocalDescriptor" /> class.
    /// </summary>
    /// <param name="nativeDescriptor">The native Windows local descriptor.</param>
    /// <param name="characteristic">The local characteristic that owns this descriptor.</param>
    /// <param name="id">The descriptor identifier.</param>
    /// <param name="name">Optional descriptor name.</param>
    public WindowsBluetoothLocalDescriptor(GattLocalDescriptor nativeDescriptor,
        IBluetoothLocalCharacteristic characteristic,
        Guid id,
        string? name = null)
        : base(characteristic, id, null, name)
    {
        NativeDescriptor = nativeDescriptor ?? throw new ArgumentNullException(nameof(nativeDescriptor));
    }

    /// <summary>
    ///     Gets the native Windows local descriptor.
    /// </summary>
    public GattLocalDescriptor NativeDescriptor { get; }

    /// <inheritdoc />
    protected override ValueTask NativeUpdateValueAsync(ReadOnlyMemory<byte> value, TimeSpan? timeout, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return ValueTask.CompletedTask;
    }
}
