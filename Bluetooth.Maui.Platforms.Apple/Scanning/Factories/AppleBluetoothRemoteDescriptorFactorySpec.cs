using Bluetooth.Maui.Platforms.Apple.Scanning.NativeObjects;

namespace Bluetooth.Maui.Platforms.Apple.Scanning.Factories;

/// <summary>
///     Apple-specific factory spec for creating <see cref="AppleBluetoothRemoteDescriptor" /> instances.
///     Extends the base spec with the native <see cref="CBDescriptor" /> required for CoreBluetooth.
/// </summary>
public record AppleBluetoothRemoteDescriptorFactorySpec(Guid DescriptorId, CBDescriptor CbDescriptor)
    : IBluetoothRemoteDescriptorFactory.BluetoothRemoteDescriptorFactorySpec(DescriptorId)
{
    /// <summary>
    ///     Initializes a new instance from a native Core Bluetooth descriptor.
    /// </summary>
    /// <param name="cbDescriptor">The native iOS Core Bluetooth descriptor.</param>
    public AppleBluetoothRemoteDescriptorFactorySpec(CBDescriptor cbDescriptor)
        : this((cbDescriptor ?? throw new ArgumentNullException(nameof(cbDescriptor))).UUID.ToGuid(), cbDescriptor)
    {
    }
}
