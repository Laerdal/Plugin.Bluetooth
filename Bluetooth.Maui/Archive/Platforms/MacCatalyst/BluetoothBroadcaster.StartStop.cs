namespace Bluetooth.Maui;

public partial class BluetoothBroadcaster
{
    /// <inheritdoc/>
    /// <remarks>
    /// On iOS, this checks if the central manager's <see cref="CBCentralManager.IsScanning"/> property is <c>true</c>.
    /// </remarks>
    protected override void NativeRefreshIsRunning()
    {
        IsRunning = Adapter is BluetoothAdapter { CbCentralManagerIsScanning: true };
    }

    /// <inheritdoc />
    protected async override ValueTask NativeStartAsync(IBluetoothBroadcasterStartBroadcastingOptions options, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    protected async override ValueTask NativeStopAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public void AdvertisingStarted(NSError? error)
    {
        throw new NotImplementedException();
    }
}
