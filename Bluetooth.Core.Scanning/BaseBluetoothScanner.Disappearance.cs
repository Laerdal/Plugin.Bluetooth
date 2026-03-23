namespace Bluetooth.Core.Scanning;

public abstract partial class BaseBluetoothScanner
{
    /// <summary>
    ///     Applies inactivity-based disappearance handling to discovered devices.
    /// </summary>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    private async Task HandleDeviceDisappearanceAsync(CancellationToken cancellationToken)
    {
        if (!IsRunning)
        {
            return;
        }

        var options = ActiveScanningOptions;
        if (options?.DeviceDisappearTimeout is not { } timeout || timeout <= TimeSpan.Zero)
        {
            return;
        }

        var now = DateTimeOffset.UtcNow;
        var disappearedDevices = GetDevices(device => now - device.LastSeen >= timeout);
        if (disappearedDevices.Count == 0)
        {
            return;
        }

        switch (options.DeviceDisappearanceBehavior)
        {
            case BluetoothDeviceDisappearanceBehavior.MarkAsStale:
                foreach (var device in disappearedDevices)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    if (device is BaseBluetoothRemoteDevice baseDevice && !baseDevice.IsStale)
                    {
                        baseDevice.SetStaleState(true);
                    }
                }
                break;

            case BluetoothDeviceDisappearanceBehavior.RemoveFromList:
                await ClearDevicesAsync(disappearedDevices).ConfigureAwait(false);
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(cancellationToken),
                                                      $"Unknown device disappearance behavior: {options.DeviceDisappearanceBehavior}");
        }
    }
}
