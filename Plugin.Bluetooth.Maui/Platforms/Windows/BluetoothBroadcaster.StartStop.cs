using Plugin.Bluetooth.Maui.PlatformSpecific;

namespace Plugin.Bluetooth.Maui;

public partial class BluetoothBroadcaster
{

    public void OnAdvertisementPublisherStatusChanged(BluetoothLEAdvertisementPublisherStatus status)
    {
        throw new NotImplementedException();
    }

    protected override void NativeRefreshIsRunning()
    {
        throw new NotImplementedException();
    }

    protected override ValueTask NativeStartAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    protected override ValueTask NativeStopAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
