# Events

The full list of events and their exact `EventArgs` shapes is generated from XML doc comments in the API reference (see <xref:Bluetooth.Abstractions.Scanning> and <xref:Bluetooth.Abstractions.Broadcasting>) — this page covers the conventions that apply across all of them and links to the type where each event actually lives, rather than duplicating a per-event listing that would just drift out of sync again.

## Event Patterns

### General Conventions

All events in Plugin.Bluetooth follow .NET event patterns:

```csharp
// Standard event pattern
event EventHandler EventName;
event EventHandler<TEventArgs> EventWithData;

// Usage
scanner.Started += (sender, e) =>
{
    Console.WriteLine("Scanner started");
};

scanner.DevicesAdded += (sender, e) =>
{
    foreach (var device in e.Items)
    {
        Console.WriteLine($"Found: {device.Name}");
    }
};
```

### Thread Safety

**Important:** Events may be raised on any thread, including background threads. Always marshal to the UI thread when updating UI components:

```csharp
scanner.DevicesAdded += async (sender, e) =>
{
    await MainThread.InvokeOnMainThreadAsync(() =>
    {
        foreach (var device in e.Items)
        {
            DeviceList.Add(device); // UI update
        }
    });
};
```

### Lifecycle Events

State-changing operations follow a consistent pattern:

```csharp
// Starting -> Started
// Stopping -> Stopped
// Connecting -> Connected
// Disconnecting -> Disconnected

device.Connecting += (s, e) => Console.WriteLine("Connecting...");
device.Connected += (s, e) => Console.WriteLine("Connected!");
```

### Collection Events

Collections provide three levels of change notification:

```csharp
// Fine-grained: Added/Removed
scanner.DevicesAdded += (s, e) => { /* new devices */ };
scanner.DevicesRemoved += (s, e) => { /* removed devices */ };

// Combined: Changed
scanner.DeviceListChanged += (s, e) =>
{
    var added = e.AddedItems;
    var removed = e.RemovedItems;
};
```

### Signal strength: no change event, poll instead

There is no `RssiChanged`-style event exposed on `IBluetoothRemoteDevice` — `RssiChangedEventArgs` exists in the codebase but isn't wired to any public event. Poll instead:

```csharp
int rssi = await device.ReadSignalStrengthAsync();
```

`device.SignalStrengthInDbm` holds the last-read value without triggering a new native read; `device.SignalStrengthInPercent` gives a normalized 0-100 view of the same reading.

## Where each event lives

| Area | Type | Key events |
|------|------|-------------|
| Scanner lifecycle & device list | <xref:Bluetooth.Abstractions.Scanning.IBluetoothScanner> | `Started`, `Stopped`, `DevicesAdded`, `DevicesRemoved`, `DeviceListChanged` |
| Device connection & advertisements | <xref:Bluetooth.Abstractions.Scanning.IBluetoothRemoteDevice> | `Connecting`, `Connected`, `Disconnecting`, `Disconnected`, `ConnectionStateChanged`, `UnexpectedDisconnection`, `AdvertisementReceived`, `PairingStateChanged`, `MtuChanged`, `PhyChanged` |
| Service discovery | <xref:Bluetooth.Abstractions.Scanning.IBluetoothRemoteDevice> | `ServiceListChanged` |
| Characteristic value changes | <xref:Bluetooth.Abstractions.Scanning.IBluetoothRemoteCharacteristic> | `ValueUpdated` |
| Descriptor list changes | <xref:Bluetooth.Abstractions.Scanning.IBluetoothRemoteService> | `DescriptorListChanged` |
| Broadcaster lifecycle & local service list | <xref:Bluetooth.Abstractions.Broadcasting.IBluetoothBroadcaster> | broadcaster state events, `ServiceListChanged` |
| Client connections (broadcaster/peripheral role) | <xref:Bluetooth.Abstractions.Broadcasting.IBluetoothBroadcaster> | `ClientDeviceListChanged`, `ClientDevicesAdded`, `ClientDevicesRemoved` |
| Local characteristic read/write requests | <xref:Bluetooth.Abstractions.Broadcasting.IBluetoothLocalCharacteristic> | `ReadRequested`, `WriteRequested` |
| Local descriptor read/write requests | <xref:Bluetooth.Abstractions.Broadcasting.IBluetoothLocalDescriptor> | `ReadRequested`, `WriteRequested` |

All `EventArgs` types are under <xref:Bluetooth.Abstractions.Scanning.EventArgs> (scanning/client side) and <xref:Bluetooth.Abstractions.Broadcasting.EventArgs> (broadcasting/server side) — see each interface's generated page for the exact event signature and its `EventArgs` shape.

## Best Practices

### Event Subscription

```csharp
// GOOD: Weak event pattern for long-lived objects
WeakEventManager<IBluetoothScanner, DevicesAddedEventArgs>
    .AddHandler(scanner, nameof(scanner.DevicesAdded), OnDevicesAdded);

// GOOD: Explicit unsubscribe
scanner.DevicesAdded += OnDevicesAdded;
// Later...
scanner.DevicesAdded -= OnDevicesAdded;

// BAD: Lambda without unsubscribe (potential memory leak)
scanner.DevicesAdded += (s, e) => { /* ... */ };
```

### Error Handling

```csharp
device.UnexpectedDisconnection += async (s, e) =>
{
    try
    {
        // Reconnection logic
        await device.ConnectAsync();
    }
    catch (Exception ex)
    {
        // Log and handle gracefully
        logger.LogError(ex, "Reconnection failed");
    }
};
```

### Performance

```csharp
// GOOD: Throttle high-frequency events
private DateTime _lastUpdate = DateTime.MinValue;

characteristic.ValueUpdated += (s, e) =>
{
    if ((DateTime.Now - _lastUpdate).TotalMilliseconds < 100)
        return; // Throttle to 10Hz

    _lastUpdate = DateTime.Now;
    ProcessValue(e.NewValue);
};

// GOOD: Use debouncing for UI updates
private CancellationTokenSource? _uiUpdateCts;

scanner.DeviceListChanged += async (s, e) =>
{
    _uiUpdateCts?.Cancel();
    _uiUpdateCts = new CancellationTokenSource();

    try
    {
        await Task.Delay(100, _uiUpdateCts.Token);
        await MainThread.InvokeOnMainThreadAsync(() => RefreshUI());
    }
    catch (OperationCanceledException) { }
};
```

## See Also

- [Overview and Conventions](./README.md)
- [Interfaces and Abstractions](./Abstractions.md)
- [Enumerations](./Enums.md)
- [Generated API Reference](xref:Bluetooth.Abstractions)
