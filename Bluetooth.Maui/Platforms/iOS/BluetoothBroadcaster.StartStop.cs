using Bluetooth.Maui.PlatformSpecific;

namespace Bluetooth.Maui;

public partial class BluetoothBroadcaster
{
    /// <inheritdoc/>
    /// <exception cref="NotImplementedException">This method is not yet implemented.</exception>
    protected override void NativeRefreshIsRunning()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    /// <exception cref="NotImplementedException">This method is not yet implemented.</exception>
    protected override ValueTask NativeStartAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    /// <exception cref="NotImplementedException">This method is not yet implemented.</exception>
    protected override ValueTask NativeStopAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Called when advertising has started.
    /// </summary>
    /// <param name="error">Error that occurred during advertising start, or <c>null</c> if successful.</param>
    /// <exception cref="NotImplementedException">This method is not yet implemented.</exception>
    public void AdvertisingStarted(NSError? error)
    {
        throw new NotImplementedException();
    }

}
