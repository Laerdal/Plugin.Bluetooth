namespace Bluetooth.Maui.Platforms.Apple.Scanning.Factories;

/// <inheritdoc />
public record AppleBluetoothDescriptorFactoryRequest : IBluetoothDescriptorFactory.BluetoothDescriptorFactoryRequest
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="AppleBluetoothDescriptorFactoryRequest" /> class with the specified Core Bluetooth descriptor.
    /// </summary>
    public AppleBluetoothDescriptorFactoryRequest(CBDescriptor cbDescriptor) : base(cbDescriptor?.UUID.ToGuid() ?? throw new ArgumentNullException(nameof(cbDescriptor)))
    {
        CbDescriptor = cbDescriptor;
    }

    /// <summary>
    ///     Gets the native iOS Core Bluetooth descriptor.
    /// </summary>
    public CBDescriptor CbDescriptor { get; init; }
}
