using Bluetooth.Maui.Platforms.Droid.Tools;

namespace Bluetooth.Maui.Platforms.Droid.Scanning.Factories;

/// <inheritdoc />
public record AndroidBluetoothRemoteDescriptorFactorySpec : IBluetoothRemoteDescriptorFactory.BluetoothRemoteDescriptorFactorySpec
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="AndroidBluetoothRemoteDescriptorFactorySpec" /> class with the specified Android GATT descriptor.
    /// </summary>
    /// <param name="nativeDescriptor">The native Android GATT descriptor from which to create the factory spec.</param>
    public AndroidBluetoothRemoteDescriptorFactorySpec(BluetoothGattDescriptor nativeDescriptor)
        : base(nativeDescriptor?.Uuid?.ToGuid() ?? throw new ArgumentNullException(nameof(nativeDescriptor)))
    {
        NativeDescriptor = nativeDescriptor;
    }

    /// <summary>
    ///     Gets the native Android GATT descriptor.
    /// </summary>
    public BluetoothGattDescriptor NativeDescriptor { get; init; }
}
