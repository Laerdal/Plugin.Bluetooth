using Bluetooth.Abstractions.Broadcasting.Factories;

namespace Bluetooth.Maui.Platforms.Apple.Broadcasting;

/// <inheritdoc/>
public record BluetoothBroadcastServiceFactoryRequest : IBluetoothBroadcastServiceFactory.BluetoothBroadcastServiceFactoryRequest
{
    /// <summary>
    /// Gets the native iOS mutable service.
    /// </summary>
    public CBMutableService? NativeService { get; init; }
}
