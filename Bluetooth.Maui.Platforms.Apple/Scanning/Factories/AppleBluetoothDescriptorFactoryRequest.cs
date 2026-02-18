using System.Diagnostics.CodeAnalysis;

using Bluetooth.Abstractions.Scanning.Factories;
using Bluetooth.Maui.Platforms.Apple.Tools;

namespace Bluetooth.Maui.Platforms.Apple.Scanning.Factories;

/// <inheritdoc/>
public record AppleBluetoothDescriptorFactoryRequest : IBluetoothDescriptorFactory.BluetoothDescriptorFactoryRequest
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AppleBluetoothDescriptorFactoryRequest"/> class with the specified Core Bluetooth descriptor.
    /// </summary>
    public AppleBluetoothDescriptorFactoryRequest([NotNull] CBDescriptor cbDescriptor) : base(cbDescriptor.UUID.ToGuid())
    {
        CbDescriptor = cbDescriptor;
    }

    /// <summary>
    /// Gets the native iOS Core Bluetooth descriptor.
    /// </summary>
    public CBDescriptor CbDescriptor { get; init; }
}