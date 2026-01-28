namespace Bluetooth.Maui.Platforms.Apple.Scanning;

/// <inheritdoc/>
public record BluetoothServiceFactoryRequest : IBluetoothServiceFactory.BluetoothServiceFactoryRequest
{
    /// <summary>
    /// Gets the native iOS Core Bluetooth service.
    /// </summary>
    public CBService? NativeService { get; init; }
}
