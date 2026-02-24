# Events

Complete reference for all events and event arguments in Plugin.Bluetooth.

## Table of Contents

- [Event Patterns](#event-patterns)
- [Scanner Events](#scanner-events)
- [Device Events](#device-events)
- [Service Events](#service-events)
- [Characteristic Events](#characteristic-events)
- [Descriptor Events](#descriptor-events)
- [Broadcaster Events](#broadcaster-events)
- [Local Characteristic Events](#local-characteristic-events)
- [Local Descriptor Events](#local-descriptor-events)
- [Event Arguments Reference](#event-arguments-reference)

---

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

---

## Scanner Events

### State Events

Events for scanner lifecycle state changes.

#### RunningStateChanged

```csharp
event EventHandler? RunningStateChanged;
```

Raised when the scanner's running state changes (started or stopped).

**Usage:**
```csharp
scanner.RunningStateChanged += (s, e) =>
{
    bool isRunning = scanner.IsRunning;
    UpdateUI(isRunning);
};
```

#### Starting

```csharp
event EventHandler Starting;
```

Raised when the scanner is beginning the start process.

**Usage:**
```csharp
scanner.Starting += (s, e) =>
{
    ShowLoadingIndicator();
};
```

#### Started

```csharp
event EventHandler Started;
```

Raised when the scanner has successfully started.

**Usage:**
```csharp
scanner.Started += (s, e) =>
{
    HideLoadingIndicator();
    Console.WriteLine("Scanning for devices...");
};
```

#### Stopping

```csharp
event EventHandler Stopping;
```

Raised when the scanner is beginning the stop process.

#### Stopped

```csharp
event EventHandler Stopped;
```

Raised when the scanner has successfully stopped.

**Usage:**
```csharp
scanner.Stopped += (s, e) =>
{
    Console.WriteLine($"Found {scanner.Devices.Count} devices");
};
```

### Advertisement Events

#### AdvertisementReceived

```csharp
event EventHandler<AdvertisementReceivedEventArgs> AdvertisementReceived;
```

Raised when a Bluetooth advertisement packet is received from any device.

**Event Args:**
```csharp
public class AdvertisementReceivedEventArgs : EventArgs
{
    public IBluetoothRemoteDevice Device { get; }
    public IBluetoothAdvertisement Advertisement { get; }
    public int Rssi { get; }
}
```

**Usage:**
```csharp
scanner.AdvertisementReceived += (s, e) =>
{
    var ad = e.Advertisement;
    Console.WriteLine($"Device: {ad.LocalName}, RSSI: {e.Rssi}");

    // Parse manufacturer data
    foreach (var (company, data) in ad.ManufacturerData)
    {
        ProcessManufacturerData(company, data);
    }

    // Filter by service UUIDs
    if (ad.ServiceUuids.Contains(myServiceUuid))
    {
        Console.WriteLine("Found device with required service");
    }
};
```

**Notes:**
- High frequency event (can fire many times per second)
- Consider throttling or filtering
- RSSI updates in real-time

### Device List Events

#### DevicesAdded

```csharp
event EventHandler<DevicesAddedEventArgs> DevicesAdded;
```

Raised when one or more devices are discovered and added to the scanner's device list.

**Event Args:**
```csharp
public class DevicesAddedEventArgs : ItemsChangedEventArgs<IBluetoothRemoteDevice>
{
    public IEnumerable<IBluetoothRemoteDevice> Items { get; }
}
```

**Usage:**
```csharp
scanner.DevicesAdded += (s, e) =>
{
    foreach (var device in e.Items)
    {
        Console.WriteLine($"Discovered: {device.Name ?? device.Id}");
        AddDeviceToUI(device);
    }
};
```

#### DevicesRemoved

```csharp
event EventHandler<DevicesRemovedEventArgs> DevicesRemoved;
```

Raised when one or more devices are removed from the scanner's device list (e.g., out of range).

**Event Args:**
```csharp
public class DevicesRemovedEventArgs : ItemsChangedEventArgs<IBluetoothRemoteDevice>
{
    public IEnumerable<IBluetoothRemoteDevice> Items { get; }
}
```

**Usage:**
```csharp
scanner.DevicesRemoved += (s, e) =>
{
    foreach (var device in e.Items)
    {
        Console.WriteLine($"Lost: {device.Name ?? device.Id}");
        RemoveDeviceFromUI(device);
    }
};
```

#### DeviceListChanged

```csharp
event EventHandler<DeviceListChangedEventArgs> DeviceListChanged;
```

Raised when the device list changes (added or removed devices). Combines `DevicesAdded` and `DevicesRemoved`.

**Event Args:**
```csharp
public class DeviceListChangedEventArgs : ItemListChangedEventArgs<IBluetoothRemoteDevice>
{
    public IEnumerable<IBluetoothRemoteDevice>? AddedItems { get; }
    public IEnumerable<IBluetoothRemoteDevice>? RemovedItems { get; }
}
```

**Usage:**
```csharp
scanner.DeviceListChanged += (s, e) =>
{
    if (e.AddedItems != null)
    {
        foreach (var device in e.AddedItems)
            AddDeviceToUI(device);
    }

    if (e.RemovedItems != null)
    {
        foreach (var device in e.RemovedItems)
            RemoveDeviceFromUI(device);
    }
};
```

---

## Device Events

### Connection State Events

Events for device connection lifecycle.

#### Connecting

```csharp
event EventHandler Connecting;
```

Raised when connection to the device is being established.

**Usage:**
```csharp
device.Connecting += (s, e) =>
{
    ShowConnectingIndicator();
};
```

#### Connected

```csharp
event EventHandler Connected;
```

Raised when the device has successfully connected.

**Usage:**
```csharp
device.Connected += async (s, e) =>
{
    HideConnectingIndicator();
    await device.DiscoverServicesAsync();
};
```

#### Disconnecting

```csharp
event EventHandler Disconnecting;
```

Raised when disconnection from the device is being initiated.

#### Disconnected

```csharp
event EventHandler Disconnected;
```

Raised when the device has disconnected.

**Usage:**
```csharp
device.Disconnected += (s, e) =>
{
    Console.WriteLine("Device disconnected");
    UpdateConnectionUI(false);
};
```

#### ConnectionStateChanged

```csharp
event EventHandler<DeviceConnectionStateChangedEventArgs> ConnectionStateChanged;
```

Comprehensive event for any connection state change.

**Event Args:**
```csharp
public class DeviceConnectionStateChangedEventArgs : EventArgs
{
    public bool IsConnected { get; }
    public bool IsConnecting { get; }
    public bool IsDisconnecting { get; }
}
```

**Usage:**
```csharp
device.ConnectionStateChanged += (s, e) =>
{
    if (e.IsConnected)
        Console.WriteLine("Connected");
    else if (e.IsConnecting)
        Console.WriteLine("Connecting...");
    else if (e.IsDisconnecting)
        Console.WriteLine("Disconnecting...");
    else
        Console.WriteLine("Disconnected");
};
```

#### UnexpectedDisconnection

```csharp
event EventHandler<DeviceUnexpectedDisconnectionEventArgs> UnexpectedDisconnection;
```

Raised when the device disconnects unexpectedly (not initiated by the app).

**Event Args:**
```csharp
public class DeviceUnexpectedDisconnectionEventArgs : EventArgs
{
    public string? Reason { get; }
    public Exception? Exception { get; }
}
```

**Usage:**
```csharp
device.UnexpectedDisconnection += async (s, e) =>
{
    Console.WriteLine($"Unexpected disconnection: {e.Reason}");

    // Implement reconnection logic
    await Task.Delay(1000);
    try
    {
        await device.ConnectAsync();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Reconnection failed: {ex.Message}");
    }
};

// Temporarily ignore next unexpected disconnection
device.IgnoreNextUnexpectedDisconnection = true;
await device.DisconnectAsync(); // Won't trigger UnexpectedDisconnection
```

**Common reasons:**
- Device powered off
- Out of range
- Low battery
- Connection supervision timeout
- Link loss

### Service Discovery Events

#### ServiceListChanged

```csharp
event EventHandler<ServiceListChangedEventArgs> ServiceListChanged;
```

Raised when services are discovered or the service list changes.

**Event Args:**
```csharp
public class ServiceListChangedEventArgs : ItemListChangedEventArgs<IBluetoothRemoteService>
{
    public IEnumerable<IBluetoothRemoteService>? AddedItems { get; }
    public IEnumerable<IBluetoothRemoteService>? RemovedItems { get; }
}
```

**Usage:**
```csharp
device.ServiceListChanged += (s, e) =>
{
    if (e.AddedItems != null)
    {
        foreach (var service in e.AddedItems)
        {
            Console.WriteLine($"Found service: {service.Name}");
        }
    }
};
```

#### ServicesChanged

```csharp
event EventHandler ServicesChanged;
```

Raised when the device's GATT services are modified (GATT Server's "Service Changed" characteristic).

**Usage:**
```csharp
device.ServicesChanged += async (s, e) =>
{
    Console.WriteLine("Services changed, rediscovering...");
    await device.DiscoverServicesAsync();
};
```

**Notes:**
- Only raised if device implements Service Changed characteristic
- Typically occurs after firmware updates
- Requires re-discovery of services

### Signal and Performance Events

#### RssiChanged

```csharp
event EventHandler<RssiChangedEventArgs> RssiChanged;
```

Raised when the device's RSSI (signal strength) changes.

**Event Args:**
```csharp
public class RssiChangedEventArgs : EventArgs
{
    public int Rssi { get; }
}
```

**Usage:**
```csharp
device.RssiChanged += (s, e) =>
{
    UpdateSignalStrengthIndicator(e.Rssi);

    if (e.Rssi < -80)
        Console.WriteLine("Warning: Weak signal");
};

// Request RSSI update
await device.ReadRssiAsync();
```

**RSSI interpretation:**
- -30 to -50 dBm: Excellent
- -50 to -70 dBm: Good
- -70 to -80 dBm: Fair
- -80 to -90 dBm: Poor
- < -90 dBm: Unusable

#### MtuChanged

```csharp
event EventHandler<MtuChangedEventArgs> MtuChanged;
```

Raised when the MTU (Maximum Transmission Unit) changes.

**Event Args:**
```csharp
public class MtuChangedEventArgs : EventArgs
{
    public int NewMtu { get; }
    public int OldMtu { get; }
}
```

**Usage:**
```csharp
device.MtuChanged += (s, e) =>
{
    Console.WriteLine($"MTU changed: {e.OldMtu} -> {e.NewMtu}");
    int maxPayload = e.NewMtu - 3; // 3 bytes overhead
    Console.WriteLine($"Max payload: {maxPayload} bytes");
};

// Request larger MTU
await device.RequestMtuAsync(512);
```

**Notes:**
- Default MTU is 23 bytes (20 bytes payload)
- Maximum MTU varies by platform (typically 512)
- Larger MTU = better throughput

#### PhyChanged

```csharp
event EventHandler<PhyChangedEventArgs> PhyChanged;
```

Raised when the PHY (Physical Layer) mode changes.

**Event Args:**
```csharp
public class PhyChangedEventArgs : EventArgs
{
    public PhyMode TxPhy { get; }
    public PhyMode RxPhy { get; }
}
```

**Usage:**
```csharp
device.PhyChanged += (s, e) =>
{
    Console.WriteLine($"PHY: TX={e.TxPhy}, RX={e.RxPhy}");

    if (e.TxPhy == PhyMode.Le2M)
        Console.WriteLine("Using 2M PHY for higher throughput");
};

// Request PHY change
await device.SetPreferredPhyAsync(PhyMode.Le2M, PhyMode.Le2M);
```

**Platform support:** Android 8.0+, iOS/macOS (limited)

### Other Device Events

#### AdvertisementReceived

```csharp
event EventHandler<AdvertisementReceivedEventArgs> AdvertisementReceived;
```

Device-specific advertisement updates (same as scanner event but for individual device).

#### PairingStateChanged

```csharp
event EventHandler<PairingStateChangedEventArgs> PairingStateChanged;
```

Raised when the device's pairing/bonding state changes.

**Event Args:**
```csharp
public class PairingStateChangedEventArgs : EventArgs
{
    public bool IsPaired { get; }
    public bool IsBonded { get; }
}
```

**Usage:**
```csharp
device.PairingStateChanged += (s, e) =>
{
    if (e.IsBonded)
        Console.WriteLine("Device bonded - secure connection");
    else if (e.IsPaired)
        Console.WriteLine("Device paired");
};
```

---

## Service Events

### CharacteristicListChanged

```csharp
event EventHandler<CharacteristicListChangedEventArgs> CharacteristicListChanged;
```

Raised when characteristics are discovered or the characteristic list changes.

**Event Args:**
```csharp
public class CharacteristicListChangedEventArgs : ItemListChangedEventArgs<IBluetoothRemoteCharacteristic>
{
    public IEnumerable<IBluetoothRemoteCharacteristic>? AddedItems { get; }
    public IEnumerable<IBluetoothRemoteCharacteristic>? RemovedItems { get; }
}
```

**Usage:**
```csharp
service.CharacteristicListChanged += (s, e) =>
{
    if (e.AddedItems != null)
    {
        foreach (var characteristic in e.AddedItems)
        {
            Console.WriteLine($"Found characteristic: {characteristic.Name}");
            CheckCharacteristicCapabilities(characteristic);
        }
    }
};

await service.DiscoverCharacteristicsAsync();
```

---

## Characteristic Events

### ValueUpdated

```csharp
event EventHandler<ValueUpdatedEventArgs> ValueUpdated;
```

Raised when the characteristic's value is updated (via notifications or indications).

**Event Args:**
```csharp
public class ValueUpdatedEventArgs : EventArgs
{
    public ReadOnlyMemory<byte> NewValue { get; }
    public ReadOnlyMemory<byte> OldValue { get; }
}
```

**Usage:**
```csharp
characteristic.ValueUpdated += (s, e) =>
{
    byte[] newData = e.NewValue.ToArray();
    byte[] oldData = e.OldValue.ToArray();

    Console.WriteLine($"Value changed from {BitConverter.ToString(oldData)} " +
                     $"to {BitConverter.ToString(newData)}");

    // Parse the new value
    ProcessCharacteristicValue(e.NewValue);
};

// Start listening
await characteristic.StartListeningAsync();
```

**Notes:**
- Only fires when `IsListening` is true
- Most common event for sensor data
- High frequency - consider throttling if needed

### DescriptorListChanged

```csharp
event EventHandler<DescriptorListChangedEventArgs> DescriptorListChanged;
```

Raised when descriptors are discovered or the descriptor list changes.

**Event Args:**
```csharp
public class DescriptorListChangedEventArgs : ItemListChangedEventArgs<IBluetoothRemoteDescriptor>
{
    public IEnumerable<IBluetoothRemoteDescriptor>? AddedItems { get; }
    public IEnumerable<IBluetoothRemoteDescriptor>? RemovedItems { get; }
}
```

**Usage:**
```csharp
characteristic.DescriptorListChanged += (s, e) =>
{
    if (e.AddedItems != null)
    {
        foreach (var descriptor in e.AddedItems)
        {
            Console.WriteLine($"Found descriptor: {descriptor.Name}");
        }
    }
};
```

---

## Descriptor Events

Descriptors don't have value update events. Changes are detected through read operations.

---

## Broadcaster Events

### State Events

Similar to scanner state events:

```csharp
event EventHandler? RunningStateChanged;
event EventHandler Starting;
event EventHandler Started;
event EventHandler Stopping;
event EventHandler Stopped;
```

### Service Management Events

#### ServiceListChanged

```csharp
event EventHandler<ServiceListChangedEventArgs> ServiceListChanged;
```

Raised when services are added or removed from the broadcaster.

**Event Args:**
```csharp
public class ServiceListChangedEventArgs : ItemListChangedEventArgs<IBluetoothLocalService>
{
    public IEnumerable<IBluetoothLocalService>? AddedItems { get; }
    public IEnumerable<IBluetoothLocalService>? RemovedItems { get; }
}
```

**Usage:**
```csharp
broadcaster.ServiceListChanged += (s, e) =>
{
    if (e.AddedItems != null)
    {
        foreach (var service in e.AddedItems)
        {
            Console.WriteLine($"Added service: {service.Name}");
        }
    }
};
```

### Client Connection Events

#### ClientDeviceListChanged

```csharp
event EventHandler<ClientDeviceListChangedEventArgs> ClientDeviceListChanged;
```

Raised when the list of connected clients changes.

**Event Args:**
```csharp
public class ClientDeviceListChangedEventArgs : ItemListChangedEventArgs<IBluetoothConnectedDevice>
{
    public IEnumerable<IBluetoothConnectedDevice>? AddedItems { get; }
    public IEnumerable<IBluetoothConnectedDevice>? RemovedItems { get; }
}
```

#### ClientConnected

```csharp
event EventHandler<ClientConnectionStateChangedEventArgs> ClientConnected;
```

Raised when a central device connects to the broadcaster.

**Event Args:**
```csharp
public class ClientConnectionStateChangedEventArgs : EventArgs
{
    public IBluetoothConnectedDevice Device { get; }
}
```

**Usage:**
```csharp
broadcaster.ClientConnected += (s, e) =>
{
    var client = e.Device;
    Console.WriteLine($"Client connected: {client.Name ?? client.Id}");
    Console.WriteLine($"MTU: {client.Mtu}");

    // Track subscriptions
    client.PropertyChanged += (s, args) =>
    {
        if (args.PropertyName == nameof(client.SubscribedCharacteristics))
        {
            Console.WriteLine($"Subscriptions: {client.SubscribedCharacteristics.Count}");
        }
    };
};
```

#### ClientDisconnected

```csharp
event EventHandler<ClientConnectionStateChangedEventArgs> ClientDisconnected;
```

Raised when a central device disconnects from the broadcaster.

**Usage:**
```csharp
broadcaster.ClientDisconnected += (s, e) =>
{
    Console.WriteLine($"Client disconnected: {e.Device.Name ?? e.Device.Id}");
};
```

---

## Local Characteristic Events

Events for server-side characteristic operations.

### ReadRequested

```csharp
event EventHandler<CharacteristicReadRequestEventArgs> ReadRequested;
```

Raised when a client requests to read the characteristic's value.

**Event Args:**
```csharp
public class CharacteristicReadRequestEventArgs : EventArgs
{
    public IBluetoothConnectedDevice Device { get; }
    public int Offset { get; }

    // Response methods
    public void RespondWithValue(ReadOnlyMemory<byte> value);
    public void RespondWithError(int errorCode);
}
```

**Usage:**
```csharp
characteristic.ReadRequested += (s, e) =>
{
    Console.WriteLine($"Read request from {e.Device.Name ?? e.Device.Id}");

    // Provide the data
    byte[] data = GetCurrentSensorData();
    e.RespondWithValue(data);

    // Or respond with error
    // e.RespondWithError(0x02); // Read Not Permitted
};
```

**Common error codes:**
- 0x01: Invalid Handle
- 0x02: Read Not Permitted
- 0x03: Write Not Permitted
- 0x05: Insufficient Authentication
- 0x06: Request Not Supported
- 0x08: Invalid Offset

### WriteRequested

```csharp
event EventHandler<CharacteristicWriteRequestEventArgs> WriteRequested;
```

Raised when a client writes to the characteristic.

**Event Args:**
```csharp
public class CharacteristicWriteRequestEventArgs : EventArgs
{
    public IBluetoothConnectedDevice Device { get; }
    public ReadOnlyMemory<byte> Value { get; }
    public int Offset { get; }
    public bool ResponseNeeded { get; }

    // Response methods
    public void RespondWithSuccess();
    public void RespondWithError(int errorCode);
}
```

**Usage:**
```csharp
characteristic.WriteRequested += (s, e) =>
{
    Console.WriteLine($"Write request from {e.Device.Name ?? e.Device.Id}");
    Console.WriteLine($"Data: {BitConverter.ToString(e.Value.ToArray())}");

    // Process the data
    bool success = ProcessCommand(e.Value);

    if (e.ResponseNeeded)
    {
        if (success)
            e.RespondWithSuccess();
        else
            e.RespondWithError(0x80); // Application Error
    }
};
```

---

## Local Descriptor Events

### ReadRequested

```csharp
event EventHandler<DescriptorReadRequestEventArgs> ReadRequested;
```

Raised when a client reads the descriptor.

**Event Args:**
```csharp
public class DescriptorReadRequestEventArgs : EventArgs
{
    public IBluetoothConnectedDevice Device { get; }
    public int Offset { get; }

    public void RespondWithValue(ReadOnlyMemory<byte> value);
    public void RespondWithError(int errorCode);
}
```

**Usage:**
```csharp
descriptor.ReadRequested += (s, e) =>
{
    e.RespondWithValue(new byte[] { 0x01, 0x00 });
};
```

### WriteRequested

```csharp
event EventHandler<DescriptorWriteRequestEventArgs> WriteRequested;
```

Raised when a client writes to the descriptor.

**Event Args:**
```csharp
public class DescriptorWriteRequestEventArgs : EventArgs
{
    public IBluetoothConnectedDevice Device { get; }
    public ReadOnlyMemory<byte> Value { get; }
    public int Offset { get; }
    public bool ResponseNeeded { get; }

    public void RespondWithSuccess();
    public void RespondWithError(int errorCode);
}
```

---

## Event Arguments Reference

### Summary Table

| Event Args Type | Properties | Used By |
|----------------|------------|---------|
| `AdvertisementReceivedEventArgs` | Device, Advertisement, Rssi | Scanner, Device |
| `DeviceListChangedEventArgs` | AddedItems, RemovedItems | Scanner |
| `DevicesAddedEventArgs` | Items | Scanner |
| `DevicesRemovedEventArgs` | Items | Scanner |
| `DeviceConnectionStateChangedEventArgs` | IsConnected, IsConnecting, IsDisconnecting | Device |
| `DeviceUnexpectedDisconnectionEventArgs` | Reason, Exception | Device |
| `ServiceListChangedEventArgs` | AddedItems, RemovedItems | Device, Broadcaster |
| `CharacteristicListChangedEventArgs` | AddedItems, RemovedItems | Service |
| `ValueUpdatedEventArgs` | NewValue, OldValue | Characteristic |
| `DescriptorListChangedEventArgs` | AddedItems, RemovedItems | Characteristic |
| `RssiChangedEventArgs` | Rssi | Device |
| `MtuChangedEventArgs` | NewMtu, OldMtu | Device, ConnectedDevice |
| `PhyChangedEventArgs` | TxPhy, RxPhy | Device |
| `PairingStateChangedEventArgs` | IsPaired, IsBonded | Device |
| `ClientDeviceListChangedEventArgs` | AddedItems, RemovedItems | Broadcaster |
| `ClientConnectionStateChangedEventArgs` | Device | Broadcaster |
| `CharacteristicReadRequestEventArgs` | Device, Offset, Response methods | Local Characteristic |
| `CharacteristicWriteRequestEventArgs` | Device, Value, Offset, ResponseNeeded | Local Characteristic |
| `DescriptorReadRequestEventArgs` | Device, Offset, Response methods | Local Descriptor |
| `DescriptorWriteRequestEventArgs` | Device, Value, Offset, ResponseNeeded | Local Descriptor |
| `L2CapDataReceivedEventArgs` | Data | L2CAP Channel |

---

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

---

## See Also

- [Overview and Conventions](./README.md)
- [Interfaces and Abstractions](./Abstractions.md)
- [Enumerations](./Enums.md)
- [Exceptions](./Exceptions.md)
