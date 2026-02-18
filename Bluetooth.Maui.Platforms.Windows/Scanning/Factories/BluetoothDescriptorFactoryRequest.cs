using System.Diagnostics.CodeAnalysis;

using Bluetooth.Abstractions.Scanning.Factories;

namespace Bluetooth.Maui.Platforms.Windows.Scanning.Factories;

/// <summary>
/// Windows-specific implementation of the Bluetooth descriptor factory request.
/// </summary>
public record BluetoothDescriptorFactoryRequest : IBluetoothDescriptorFactory.BluetoothDescriptorFactoryRequest
{
    /// <summary>
    /// Gets the native Windows GATT descriptor.
    /// </summary>
    public GattDescriptor NativeDescriptor { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="BluetoothDescriptorFactoryRequest"/> record.
    /// </summary>
    /// <param name="nativeDescriptor">The native Windows GATT descriptor.</param>
    public BluetoothDescriptorFactoryRequest([NotNull]GattDescriptor nativeDescriptor)
        : base(nativeDescriptor.Uuid)
    {
        ArgumentNullException.ThrowIfNull(nativeDescriptor);
        NativeDescriptor = nativeDescriptor;
    }
}
