namespace Bluetooth.Maui.Platforms.Apple.Broadcasting.Factories;

/// <inheritdoc />
public record AppleBluetoothLocalDescriptorSpec : IBluetoothLocalDescriptorFactory.BluetoothLocalDescriptorSpec
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="AppleBluetoothLocalDescriptorSpec" /> class with the specified Core Bluetooth mutable descriptor.
    /// </summary>
    /// <param name="cbDescriptor">The native iOS Core Bluetooth mutable descriptor from which to create the factory spec.</param>
    /// <remarks>
    ///     iOS CBMutableDescriptor does not expose permissions or value after creation, so they are set to defaults (Read/Write, no initial value).
    /// </remarks>
    public AppleBluetoothLocalDescriptorSpec(CBMutableDescriptor cbDescriptor) : base(cbDescriptor?.UUID.ToGuid() ?? throw new ArgumentNullException(nameof(cbDescriptor)))
    {
        CbDescriptor = cbDescriptor;
    }

    /// <summary>
    ///     Gets the native iOS Core Bluetooth mutable descriptor.
    /// </summary>
    public CBMutableDescriptor CbDescriptor { get; init; }
}
