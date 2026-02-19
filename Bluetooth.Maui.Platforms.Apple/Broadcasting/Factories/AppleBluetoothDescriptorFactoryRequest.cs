using System.Diagnostics.CodeAnalysis;

namespace Bluetooth.Maui.Platforms.Apple.Broadcasting.Factories;

/// <inheritdoc/>
public record AppleBluetoothDescriptorSpec : IBluetoothLocalDescriptorFactory.BluetoothLocalDescriptorSpec
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AppleBluetoothDescriptorSpec"/> class with the specified Core Bluetooth mutable descriptor.
    /// </summary>
    /// <param name="cbDescriptor">The native iOS Core Bluetooth mutable descriptor from which to create the factory request.</param>
    /// <remarks>
    /// iOS CBMutableDescriptor does not expose permissions or value after creation, so they are set to defaults (Read/Write, no initial value).
    /// </remarks>
    public AppleBluetoothDescriptorSpec([NotNull] CBMutableDescriptor cbDescriptor) : base(
        cbDescriptor.UUID.ToGuid(),
        BluetoothDescriptorPermissions.Read | BluetoothDescriptorPermissions.Write,
        null)
    {
        CbDescriptor = cbDescriptor;
    }

    /// <summary>
    /// Gets the native iOS Core Bluetooth mutable descriptor.
    /// </summary>
    public CBMutableDescriptor CbDescriptor { get; init; }
}
