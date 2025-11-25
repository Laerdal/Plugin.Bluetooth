namespace Bluetooth.Maui;

public partial class BluetoothDevice
{
    /// <summary>
    /// Gets the auto-reset event used to signal when the peripheral is ready to send write-without-response commands.
    /// </summary>
    private AutoResetEvent ReadyToSendWriteWithoutResponse { get; } = new AutoResetEvent(false);

    /// <summary>
    /// Called when the peripheral is ready to send more write-without-response commands on the iOS platform.
    /// </summary>
    public void IsReadyToSendWriteWithoutResponse()
    {
        ReadyToSendWriteWithoutResponse.Set();
    }

    /// <summary>
    /// Waits for the peripheral to be ready to send write-without-response commands.
    /// </summary>
    /// <param name="timeout">The timeout for the operation.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A task that completes when the peripheral is ready or immediately if already ready.</returns>
    public Task WaitForReadyToSendWriteWithoutResponseAsync(TimeSpan timeout, CancellationToken cancellationToken)
    {
        return CbPeripheralDelegateProxy.CbPeripheral.CanSendWriteWithoutResponse ? Task.CompletedTask : Task.Run(() => ReadyToSendWriteWithoutResponse.WaitOne(timeout), cancellationToken);
    }
}
