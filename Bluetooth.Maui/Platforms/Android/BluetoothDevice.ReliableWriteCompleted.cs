using Bluetooth.Maui.PlatformSpecific.Exceptions;

namespace Bluetooth.Maui;

public partial class BluetoothDevice
{
    private AutoResetEvent ReliableWriteCompleted { get; } = new AutoResetEvent(false);

    public void OnReliableWriteCompleted(GattStatus status)
    {
        AndroidNativeGattStatusException.ThrowIfNotSuccess(status);
        ReliableWriteCompleted.Set();
    }

    public Task WaitForReliableWriteCompletedAsync(TimeSpan timeout, CancellationToken cancellationToken = default)
    {
        return Task.Run(() => ReliableWriteCompleted.WaitOne(timeout), cancellationToken);
    }
}
