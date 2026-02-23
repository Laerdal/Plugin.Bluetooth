using System.Diagnostics.CodeAnalysis;

namespace Bluetooth.Maui.Platforms.Apple.Scanning.Factories;

/// <inheritdoc />
public record AppleBluetoothServiceFactoryRequest : IBluetoothServiceFactory.BluetoothServiceFactoryRequest
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="AppleBluetoothServiceFactoryRequest" /> class with the specified Core Bluetooth service.
    /// </summary>
    /// <param name="cbService">The native iOS Core Bluetooth service from which to create the factory request.</param>
    public AppleBluetoothServiceFactoryRequest(CBService cbService) : base(cbService?.UUID.ToGuid() ?? throw new ArgumentNullException(nameof(cbService)))
    {
        CbService = cbService;
    }

    /// <summary>
    ///     Gets the native iOS Core Bluetooth service.
    /// </summary>
    public CBService CbService { get; init; }
}