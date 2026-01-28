namespace Bluetooth.Maui.Platforms.Apple.Broadcasting;

/// <inheritdoc/>
public class BluetoothBroadcastService : BaseBluetoothBroadcastService
{
    /// <summary>
    /// Gets the native iOS mutable service.
    /// </summary>
    public CBMutableService NativeService { get; }

    /// <inheritdoc/>
    public BluetoothBroadcastService(IBluetoothBroadcaster broadcaster,
        IBluetoothBroadcastServiceFactory.BluetoothBroadcastServiceFactoryRequest request,
        IBluetoothBroadcastCharacteristicFactory characteristicFactory) : base(broadcaster, request, characteristicFactory)
    {
        if (request is not BluetoothBroadcastServiceFactoryRequest nativeRequest)
        {
            throw new ArgumentException($"Request must be of type {nameof(BluetoothBroadcastServiceFactoryRequest)}", nameof(request));
        }

        ArgumentNullException.ThrowIfNull(nativeRequest.NativeService);
        NativeService = nativeRequest.NativeService;
    }

    /// <inheritdoc/>
    public override ValueTask DisposeAsync()
    {
        return base.DisposeAsync();
    }
}
