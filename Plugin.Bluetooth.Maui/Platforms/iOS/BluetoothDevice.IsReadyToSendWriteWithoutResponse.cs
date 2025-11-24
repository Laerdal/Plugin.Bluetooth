namespace Plugin.Bluetooth.Maui;

public partial class BluetoothDevice
{
    private AutoResetEvent ReadyToSendWriteWithoutResponse { get; } = new AutoResetEvent(false);

    public void IsReadyToSendWriteWithoutResponse()
    {
        ReadyToSendWriteWithoutResponse.Set();
    }

    public Task WaitForReadyToSendWriteWithoutResponseAsync(TimeSpan timeout, CancellationToken cancellationToken)
    {
        return CbPeripheralDelegateProxy.CbPeripheral.CanSendWriteWithoutResponse ? Task.CompletedTask : Task.Run(() => ReadyToSendWriteWithoutResponse.WaitOne(timeout), cancellationToken);
    }
}
