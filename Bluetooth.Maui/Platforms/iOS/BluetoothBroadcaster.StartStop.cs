using Bluetooth.Maui.PlatformSpecific;

namespace Bluetooth.Maui;

public partial class BluetoothBroadcaster
{
    private TaskCompletionSource<bool>? _advertisingStartedTcs;

    /// <inheritdoc/>
    /// <remarks>
    /// On iOS, this checks if the peripheral manager's <see cref="CBPeripheralManager.Advertising"/> property is <c>true</c>.
    /// </remarks>
    protected override void NativeRefreshIsRunning()
    {
        IsRunning = CbPeripheralManagerProxy?.CbPeripheralManager.Advertising ?? false;
    }

    /// <inheritdoc/>
    /// <remarks>
    /// Starts advertising using the Core Bluetooth peripheral manager.
    /// Builds the advertisement data dictionary from the configured properties.
    /// </remarks>
    /// <exception cref="ArgumentNullException">Thrown when <see cref="CbPeripheralManagerProxy"/> is <c>null</c>.</exception>
    protected override async ValueTask NativeStartAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(CbPeripheralManagerProxy);

        // Build advertisement data
        var advertisementData = new NSMutableDictionary();

        // Add local name if set
        if (!string.IsNullOrEmpty(LocalDeviceName))
        {
            advertisementData[CBAdvertisement.DataLocalNameKey] = new NSString(LocalDeviceName);
        }

        // Add service UUIDs if set
        if (AdvertisedServiceUuids?.Count > 0)
        {
            var serviceUuids = AdvertisedServiceUuids
                .Select(uuid => CBUUID.FromString(uuid.ToString()))
                .ToArray();
            advertisementData[CBAdvertisement.DataServiceUUIDsKey] = NSArray.FromObjects(serviceUuids);
        }

        // Add manufacturer data if set
        if (ManufacturerId.HasValue && ManufacturerData.HasValue)
        {
            var manufacturerData = NSData.FromArray(ManufacturerData.Value.ToArray());
            // Note: iOS doesn't support custom manufacturer data in advertisements for peripheral role
            // This would need to be added to service characteristics instead
        }

        // Create TCS for waiting on advertising to start
        _advertisingStartedTcs = new TaskCompletionSource<bool>();

        // Start advertising
        CbPeripheralManagerProxy.CbPeripheralManager.StartAdvertising(advertisementData);

        // Wait for advertising to start
        await _advertisingStartedTcs.Task.WaitAsync(timeout ?? TimeSpan.FromSeconds(10), cancellationToken).ConfigureAwait(false);

        NativeRefreshIsRunning();
    }

    /// <inheritdoc/>
    /// <remarks>
    /// Stops advertising using the Core Bluetooth peripheral manager.
    /// </remarks>
    /// <exception cref="ArgumentNullException">Thrown when <see cref="CbPeripheralManagerProxy"/> is <c>null</c>.</exception>
    protected override ValueTask NativeStopAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(CbPeripheralManagerProxy);
        CbPeripheralManagerProxy.CbPeripheralManager.StopAdvertising();
        NativeRefreshIsRunning();
        return ValueTask.CompletedTask;
    }

    /// <summary>
    /// Called when advertising has started.
    /// </summary>
    /// <param name="error">Error that occurred during advertising start, or <c>null</c> if successful.</param>
    public void AdvertisingStarted(NSError? error)
    {
        if (error != null)
        {
            _advertisingStartedTcs?.TrySetException(new InvalidOperationException($"Failed to start advertising: {error.LocalizedDescription}"));
        }
        else
        {
            _advertisingStartedTcs?.TrySetResult(true);
        }

        _advertisingStartedTcs = null;
        NativeRefreshIsRunning();
    }

}
