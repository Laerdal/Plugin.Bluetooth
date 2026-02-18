using System.Diagnostics.CodeAnalysis;

using Bluetooth.Abstractions.Broadcasting.Factories;
using Bluetooth.Maui.Platforms.Apple.Tools;

namespace Bluetooth.Maui.Platforms.Apple.Broadcasting.Factories;

/// <inheritdoc/>
public record AppleBluetoothServiceFactoryRequest : IBluetoothLocalServiceFactory.BluetoothLocalServiceSpec
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AppleBluetoothServiceFactoryRequest"/> class with the specified Core Bluetooth service.
    /// </summary>
    public AppleBluetoothServiceFactoryRequest([NotNull] CBService nativeService) : base(nativeService.UUID.ToGuid(), nativeService.Primary)
    {
        NativeService = nativeService;
    }

    /// <summary>
    /// Gets the native iOS Core Bluetooth service.
    /// </summary>
    public CBService NativeService { get; init; }
}
