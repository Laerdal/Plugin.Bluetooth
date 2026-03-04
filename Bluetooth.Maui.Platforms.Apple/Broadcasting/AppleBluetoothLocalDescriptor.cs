namespace Bluetooth.Maui.Platforms.Apple.Broadcasting;

/// <inheritdoc cref="BaseBluetoothLocalDescriptor" />
public class AppleBluetoothLocalDescriptor : BaseBluetoothLocalDescriptor
{

    /// <summary>
    ///     Initializes a new instance of the <see cref="AppleBluetoothLocalDescriptor" /> class with the specified Core Bluetooth descriptor, characteristic, ID, initial value, name, and logger.
    /// </summary>
    /// <param name="cbDescriptor">The native iOS Core Bluetooth descriptor represented by this local descriptor.</param>
    /// <param name="characteristic">The Bluetooth characteristic to which this descriptor belongs.</param>
    /// <param name="id">The unique identifier for this descriptor.</param>
    /// <param name="initialValue">The initial value of the descriptor, if any.</param>
    /// <param name="name">An optional name for the descriptor, used for debugging purposes.</param>
    /// <param name="logger">An optional logger for logging descriptor-related events and errors.</param>
    public AppleBluetoothLocalDescriptor(CBMutableDescriptor cbDescriptor,
        IBluetoothLocalCharacteristic characteristic,
        Guid id,
        ReadOnlyMemory<byte>? initialValue = null,
        string? name = null,
        ILogger<IBluetoothLocalDescriptor>? logger = null) : base(characteristic,
                                                                  id,
                                                                  initialValue,
                                                                  name,
                                                                  logger)
    {
        CbDescriptor = cbDescriptor ?? throw new ArgumentNullException(nameof(cbDescriptor));
    }

    /// <summary>
    ///     Gets the native iOS Core Bluetooth descriptor.
    /// </summary>
    public CBMutableDescriptor CbDescriptor { get; }

    /// <summary>
    ///     Gets the Bluetooth characteristic to which this descriptor belongs, cast to the Apple-specific implementation.
    /// </summary>
    public AppleBluetoothLocalCharacteristic AppleCharacteristic => (AppleBluetoothLocalCharacteristic) Characteristic;

    /// <inheritdoc />
    protected override ValueTask NativeUpdateValueAsync(ReadOnlyMemory<byte> value, TimeSpan? timeout, CancellationToken cancellationToken)
    {
        // iOS CBPeripheralManager doesn't support updating descriptor values after they are created.
        // Descriptors are immutable once added to a characteristic in the peripheral manager.
        // The value must be set when the descriptor is initially created via the CBMutableDescriptor constructor.
        throw new NotSupportedException("iOS does not support updating descriptor values after they are created. Descriptor values must be set during characteristic creation.");
    }
}
