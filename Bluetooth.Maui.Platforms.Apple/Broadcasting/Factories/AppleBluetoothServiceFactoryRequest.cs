namespace Bluetooth.Maui.Platforms.Apple.Broadcasting.Factories;

/// <inheritdoc />
public record AppleBluetoothServiceFactoryRequest : IBluetoothLocalServiceFactory.BluetoothLocalServiceSpec
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="AppleBluetoothServiceFactoryRequest" /> class with the specified Core Bluetooth service.
    /// </summary>
    public AppleBluetoothServiceFactoryRequest(CBService nativeService) : base(nativeService?.UUID.ToGuid() ?? throw new ArgumentNullException(nameof(nativeService)), nativeService.Primary)
    {
        NativeService = nativeService;
    }

    /// <summary>
    ///     Gets the native iOS Core Bluetooth service.
    /// </summary>
    public CBService NativeService { get; init; }
}
