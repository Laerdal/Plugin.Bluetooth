namespace Bluetooth.Maui;

public partial class BluetoothBroadcaster
{
    /// <inheritdoc/>
    protected override Task NativeDisconnectClientAsync(string clientId, TimeSpan? timeout, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
