namespace Bluetooth.Maui.Platforms.Apple.Scanning.Factories;

/// <inheritdoc />
public record AppleBluetoothRemoteServiceFactorySpec : IBluetoothRemoteServiceFactory.BluetoothRemoteServiceFactorySpec
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="AppleBluetoothRemoteServiceFactorySpec" /> class with the specified Core Bluetooth service.
    /// </summary>
    /// <param name="cbService">The native iOS Core Bluetooth service from which to create the factory spec.</param>
    public AppleBluetoothRemoteServiceFactorySpec(CBService cbService) : base(cbService?.UUID.ToGuid() ?? throw new ArgumentNullException(nameof(cbService)))
    {
        CbService = cbService;
    }

    /// <summary>
    ///     Gets the native iOS Core Bluetooth service.
    /// </summary>
    public CBService CbService { get; init; }
}
