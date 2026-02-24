using Bluetooth.Maui.Platforms.Apple.Broadcasting.Factories;

namespace Bluetooth.Maui.Platforms.Apple.Broadcasting;

/// <inheritdoc cref="BaseBluetoothLocalDescriptor" />
public class AppleBluetoothLocalDescriptor : BaseBluetoothLocalDescriptor
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="AppleBluetoothLocalDescriptor" /> class with the specified characteristic and factory spec.
    /// </summary>
    /// <param name="localCharacteristic">The Bluetooth characteristic to which this descriptor belongs.</param>
    /// <param name="spec">The factory spec containing the native Core Bluetooth descriptor.</param>
    public AppleBluetoothLocalDescriptor(IBluetoothLocalCharacteristic localCharacteristic, IBluetoothLocalDescriptorFactory.BluetoothLocalDescriptorSpec spec) : base(localCharacteristic, spec)
    {
        ArgumentNullException.ThrowIfNull(spec);
        if (spec is not AppleBluetoothLocalDescriptorSpec nativeSpec)
        {
            throw new ArgumentException($"Expected spec of type {typeof(AppleBluetoothLocalDescriptorSpec)}, but got {spec.GetType()}");
        }

        CbDescriptor = nativeSpec.CbDescriptor;
    }

    /// <summary>
    ///     Gets the native iOS Core Bluetooth descriptor.
    /// </summary>
    public CBDescriptor CbDescriptor { get; }

    /// <summary>
    ///     Gets the Bluetooth characteristic to which this descriptor belongs, cast to the Apple-specific implementation.
    /// </summary>
    public AppleBluetoothLocalCharacteristic AppleBluetoothLocalCharacteristic => (AppleBluetoothLocalCharacteristic) LocalCharacteristic;

    /// <inheritdoc />
    protected override ValueTask NativeUpdateValueAsync(ReadOnlyMemory<byte> value, TimeSpan? timeout, CancellationToken cancellationToken)
    {
        // iOS CBPeripheralManager doesn't support updating descriptor values after they are created.
        // Descriptors are immutable once added to a characteristic in the peripheral manager.
        // The value must be set when the descriptor is initially created via the CBMutableDescriptor constructor.
        throw new NotSupportedException("iOS does not support updating descriptor values after they are created. Descriptor values must be set during characteristic creation.");
    }
}
