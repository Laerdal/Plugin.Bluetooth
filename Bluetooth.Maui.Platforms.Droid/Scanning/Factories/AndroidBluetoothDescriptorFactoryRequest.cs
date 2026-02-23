using Bluetooth.Maui.Platforms.Droid.Tools;

namespace Bluetooth.Maui.Platforms.Droid.Scanning.Factories;

/// <inheritdoc />
public record AndroidBluetoothDescriptorFactoryRequest : IBluetoothDescriptorFactory.BluetoothDescriptorFactoryRequest
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="AndroidBluetoothDescriptorFactoryRequest" /> class with the specified Android GATT descriptor.
    /// </summary>
    /// <param name="nativeDescriptor">The native Android GATT descriptor from which to create the factory request.</param>
    public AndroidBluetoothDescriptorFactoryRequest(BluetoothGattDescriptor nativeDescriptor)
        : base(nativeDescriptor?.Uuid?.ToGuid() ?? throw new ArgumentNullException(nameof(nativeDescriptor)))
    {
        NativeDescriptor = nativeDescriptor;
    }

    /// <summary>
    ///     Gets the native Android GATT descriptor.
    /// </summary>
    public BluetoothGattDescriptor NativeDescriptor { get; init; }
}
