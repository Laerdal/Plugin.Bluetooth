using System.Diagnostics.CodeAnalysis;

namespace Bluetooth.Maui.Platforms.Win.Scanning.Factories;

/// <summary>
///     Windows-specific implementation of the Bluetooth descriptor factory spec.
/// </summary>
public record WindowsBluetoothRemoteDescriptorFactorySpec : IBluetoothRemoteDescriptorFactory.BluetoothRemoteDescriptorFactorySpec
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="WindowsBluetoothRemoteDescriptorFactorySpec" /> record.
    /// </summary>
    /// <param name="nativeDescriptor">The native Windows GATT descriptor.</param>
    public WindowsBluetoothRemoteDescriptorFactorySpec(GattDescriptor nativeDescriptor)
        : base(nativeDescriptor?.Uuid ?? throw new ArgumentNullException(nameof(nativeDescriptor)))
    {
        ArgumentNullException.ThrowIfNull(nativeDescriptor);
        NativeDescriptor = nativeDescriptor;
    }

    /// <summary>
    ///     Gets the native Windows GATT descriptor.
    /// </summary>
    public GattDescriptor NativeDescriptor { get; }
}
