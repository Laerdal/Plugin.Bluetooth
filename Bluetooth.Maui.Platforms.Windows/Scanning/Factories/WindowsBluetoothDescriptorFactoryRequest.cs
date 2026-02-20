using System.Diagnostics.CodeAnalysis;

namespace Bluetooth.Maui.Platforms.Windows.Scanning.Factories;

/// <summary>
///     Windows-specific implementation of the Bluetooth descriptor factory request.
/// </summary>
public record WindowsBluetoothDescriptorFactoryRequest : IBluetoothDescriptorFactory.BluetoothDescriptorFactoryRequest
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="WindowsBluetoothDescriptorFactoryRequest" /> record.
    /// </summary>
    /// <param name="nativeDescriptor">The native Windows GATT descriptor.</param>
    public WindowsBluetoothDescriptorFactoryRequest([NotNull] GattDescriptor nativeDescriptor)
        : base(nativeDescriptor.Uuid)
    {
        ArgumentNullException.ThrowIfNull(nativeDescriptor);
        NativeDescriptor = nativeDescriptor;
    }

    /// <summary>
    ///     Gets the native Windows GATT descriptor.
    /// </summary>
    public GattDescriptor NativeDescriptor { get; }
}