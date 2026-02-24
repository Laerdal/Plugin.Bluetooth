namespace Bluetooth.Maui.Platforms.Apple.Scanning.Factories;

/// <inheritdoc />
public record AppleBluetoothRemoteDescriptorFactorySpec : IBluetoothRemoteDescriptorFactory.BluetoothRemoteDescriptorFactorySpec
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="AppleBluetoothRemoteDescriptorFactorySpec" /> class with the specified Core Bluetooth descriptor.
    /// </summary>
    public AppleBluetoothRemoteDescriptorFactorySpec(CBDescriptor cbDescriptor) : base(cbDescriptor?.UUID.ToGuid() ?? throw new ArgumentNullException(nameof(cbDescriptor)))
    {
        CbDescriptor = cbDescriptor;
    }

    /// <summary>
    ///     Gets the native iOS Core Bluetooth descriptor.
    /// </summary>
    public CBDescriptor CbDescriptor { get; init; }
}
