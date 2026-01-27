namespace Bluetooth.Maui;

public partial class BluetoothBroadcaster
{
    /// <summary>
    /// Called when the advertisement publisher's status changes.
    /// </summary>
    /// <param name="status">The new publisher status.</param>
    /// <exception cref="NotImplementedException">This method is not yet implemented.</exception>
    public void OnAdvertisementPublisherStatusChanged(BluetoothLEAdvertisementPublisherStatus status)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    /// <exception cref="NotImplementedException">This method is not yet implemented.</exception>
    protected override void NativeRefreshIsRunning()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    /// <exception cref="NotImplementedException">This method is not yet implemented.</exception>
    protected override ValueTask NativeStartAsync(IBluetoothBroadcasterStartBroadcastingOptions options, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    /// <exception cref="NotImplementedException">This method is not yet implemented.</exception>
    protected override ValueTask NativeStopAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
