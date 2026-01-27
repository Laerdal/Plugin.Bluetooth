namespace Bluetooth.Maui;

public partial class BluetoothDevice
{
    /// <summary>
    /// Gets the auto-reset event used to signal completion of reliable write operations.
    /// </summary>
    private AutoResetEvent ReliableWriteCompleted { get; } = new AutoResetEvent(false);

    /// <summary>
    /// Called when a reliable write operation completes on the Android platform.
    /// </summary>
    /// <param name="status">The status of the reliable write operation.</param>
    /// <exception cref="AndroidNativeGattStatusException">Thrown when the status indicates an error.</exception>
    public void OnReliableWriteCompleted(GattStatus status)
    {
        AndroidNativeGattStatusException.ThrowIfNotSuccess(status);
        ReliableWriteCompleted.Set();
    }

    /// <summary>
    /// Waits for a reliable write operation to complete.
    /// </summary>
    /// <param name="timeout">The timeout for the operation.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A task that completes when the reliable write operation finishes.</returns>
    public Task WaitForReliableWriteCompletedAsync(TimeSpan timeout, CancellationToken cancellationToken = default)
    {
        return Task.Run(() => ReliableWriteCompleted.WaitOne(timeout), cancellationToken);
    }
}
