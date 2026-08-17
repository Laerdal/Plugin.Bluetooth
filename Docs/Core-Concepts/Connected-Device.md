# Connected Device

## Overview

A **Connected Device** represents a client (central device) that has connected to your BLE broadcaster (peripheral/GATT server). When your app acts as a BLE peripheral, Connected Devices are the phones, tablets, or other devices that connect to you to read your services and characteristics.

**Interface:** `IBluetoothConnectedDevice`

## What Does It Do?

A Connected Device allows you to:
- Track which clients are connected to your broadcaster
- Identify connected clients by ID and name
- Monitor connection and disconnection events
- Manage per-client state and subscriptions

## Context

When you're a **Scanner/Central** (client):
- You connect to Remote Devices
- You read from Remote Characteristics

When you're a **Broadcaster/Peripheral** (server):
- Clients connect to you (they become Connected Devices)
- You provide data through Local Characteristics
- You track these clients using `IBluetoothConnectedDevice`

## Basic Workflow

```
┌─────────────┐      ┌──────────────┐      ┌─────────────┐
│   Client    │─────▶│  Connects to │─────▶│  Becomes    │
│   Scans     │      │   Your App   │      │  Connected  │
└─────────────┘      └──────────────┘      │   Device    │
                                            └─────────────┘
```

## Getting Started

### 1. Monitor Connection Events

Client connections/disconnections are reported as batches (a client-list change can add or remove
more than one device at once), not as a single-device event:

```csharp
// broadcaster is an IBluetoothBroadcaster injected via DI into the containing class's constructor

// Listen for new connections
broadcaster.ClientDevicesAdded += (sender, args) =>
{
    foreach (var client in args.Items)
        Console.WriteLine($"Client connected: {client.Name ?? client.Id}");
};

// Listen for disconnections
broadcaster.ClientDevicesRemoved += (sender, args) =>
{
    foreach (var client in args.Items)
        Console.WriteLine($"Client disconnected: {client.Name ?? client.Id}");
};
```

### 2. Get Connected Clients

```csharp
// Get all currently connected clients
var connectedClients = broadcaster.GetClientDevices();
Console.WriteLine($"Connected clients: {connectedClients.Count}");

// Get specific client by ID
var client = broadcaster.GetClientDevice(clientId);

// Safe retrieval
var client = broadcaster.GetClientDeviceOrDefault(clientId);
if (client != null)
{
    Console.WriteLine($"Found client: {client.Name}");
}

// Check if specific client is connected
bool isConnected = broadcaster.HasClientDevice(clientId);
```

### 3. Track Client Subscriptions

```csharp
var characteristic = service.GetCharacteristic(characteristicUuid);

// See which clients are subscribed to this characteristic
var subscribedClients = characteristic.SubscribedDevices;

Console.WriteLine($"{subscribedClients.Count} clients are subscribed");

foreach (var client in subscribedClients)
{
    Console.WriteLine($"  - {client.Name ?? client.Id}");
}
```

## Connected Device Properties

### Basic Properties

```csharp
// Unique identifier for this client connection
string id = connectedDevice.Id;

// Client device name (may be null if not provided)
string? name = connectedDevice.Name;

// Parent broadcaster
IBluetoothBroadcaster broadcaster = connectedDevice.Broadcaster;
```

### Example

```csharp
Console.WriteLine($"Client ID: {connectedDevice.Id}");
Console.WriteLine($"Client Name: {connectedDevice.Name ?? "Unknown"}");
Console.WriteLine($"Broadcaster: {connectedDevice.Broadcaster}");
```

## Managing Connected Clients

### Get All Connected Clients

```csharp
var clients = broadcaster.GetClientDevices();

Console.WriteLine($"Total connected clients: {clients.Count}");

foreach (var client in clients)
{
    Console.WriteLine($"  - ID: {client.Id}");
    Console.WriteLine($"    Name: {client.Name ?? "Unknown"}");
}
```

### Filter Connected Clients

```csharp
// Get clients with a specific name pattern
var matchingClients = broadcaster.GetClientDevices(
    client => client.Name?.Contains("Phone") == true
);

// Get first connected client
var firstClient = broadcaster.GetClientDevices().FirstOrDefault();
```

### Track Connection Count

```csharp
broadcaster.ClientDeviceListChanged += (sender, args) =>
{
    Console.WriteLine($"Connected clients changed. Total: {broadcaster.GetClientDevices().Count}");
};
```

## Events

Monitor client connection lifecycle. There is no per-device `Connected`/`Disconnected` event —
`ClientDevicesAdded`/`ClientDevicesRemoved` fire with a batch of devices, and
`ClientDeviceListChanged` fires on any change with both the added and removed batches:

```csharp
// New client(s) connected
broadcaster.ClientDevicesAdded += (sender, args) =>
{
    foreach (var client in args.Items)
    {
        Console.WriteLine($"✓ Client connected: {client.Id}");

        // Initialize per-client state
        clientDataStore[client.Id] = new ClientData();
    }
};

// Client(s) disconnected
broadcaster.ClientDevicesRemoved += (sender, args) =>
{
    foreach (var client in args.Items)
    {
        Console.WriteLine($"✗ Client disconnected: {client.Id}");

        // Clean up per-client state
        clientDataStore.Remove(client.Id);
    }
};

// Any change to connected device list — carries both sides of the diff
broadcaster.ClientDeviceListChanged += (sender, args) =>
{
    Console.WriteLine($"Connected clients: {broadcaster.GetClientDevices().Count}");
};
```

## Common Patterns

### Track Client State

```csharp
class ClientData
{
    public DateTime ConnectedAt { get; set; }
    public int RequestCount { get; set; }
    public DateTime LastActivityAt { get; set; }
}

var clientDataStore = new Dictionary<string, ClientData>();

broadcaster.ClientDevicesAdded += (sender, args) =>
{
    foreach (var client in args.Items)
    {
        clientDataStore[client.Id] = new ClientData
        {
            ConnectedAt = DateTime.UtcNow,
            RequestCount = 0,
            LastActivityAt = DateTime.UtcNow
        };
    }
};

broadcaster.ClientDevicesRemoved += (sender, args) =>
{
    foreach (var client in args.Items)
    {
        if (clientDataStore.TryGetValue(client.Id, out var data))
        {
            var duration = DateTime.UtcNow - data.ConnectedAt;
            Console.WriteLine($"Client {client.Id} was connected for {duration.TotalSeconds:F1}s");
            Console.WriteLine($"Total requests: {data.RequestCount}");

            clientDataStore.Remove(client.Id);
        }
    }
};

// Track requests
characteristic.ReadRequested += (sender, args) =>
{
    var clientId = args.ClientId;
    if (clientDataStore.ContainsKey(clientId))
    {
        clientDataStore[clientId].RequestCount++;
        clientDataStore[clientId].LastActivityAt = DateTime.UtcNow;
    }
};
```

### Connection Limiter

```csharp
const int MaxClients = 5;

broadcaster.ClientDevicesAdded += (sender, args) =>
{
    var clients = broadcaster.GetClientDevices();

    if (clients.Count > MaxClients)
    {
        Console.WriteLine($"Too many clients ({clients.Count}), limit is {MaxClients}");

        // Optionally disconnect oldest client
        // Note: Disconnection API varies by platform - check documentation
    }
};
```

### Per-Client Notifications

```csharp
// Send different data to different clients based on their state
async Task SendPersonalizedNotificationAsync(
    IBluetoothLocalCharacteristic characteristic,
    IBluetoothConnectedDevice client)
{
    // Get client-specific data
    byte[] personalizedData = GetDataForClient(client.Id);

    // Note: The library typically sends to all subscribed clients
    // For per-client control, you may need platform-specific APIs

    await characteristic.UpdateValueAsync(personalizedData, notifyClients: true);
}

byte[] GetDataForClient(string clientId)
{
    // Return personalized data based on client ID
    return Encoding.UTF8.GetBytes($"Hello, {clientId}!");
}
```

### Activity Monitor

```csharp
class ConnectionMonitor
{
    private readonly Dictionary<string, DateTime> _lastActivity = new();
    private readonly TimeSpan _timeout = TimeSpan.FromMinutes(5);
    private readonly Timer _checkTimer;

    public ConnectionMonitor(IBluetoothBroadcaster broadcaster)
    {
        broadcaster.ClientDevicesAdded += (s, args) =>
        {
            foreach (var client in args.Items)
                _lastActivity[client.Id] = DateTime.UtcNow;
        };

        broadcaster.ClientDevicesRemoved += (s, args) =>
        {
            foreach (var client in args.Items)
                _lastActivity.Remove(client.Id);
        };

        // Monitor activity
        _checkTimer = new Timer(_ => CheckInactiveClients(), null,
            TimeSpan.FromMinutes(1), TimeSpan.FromMinutes(1));
    }

    public void RecordActivity(string clientId)
    {
        _lastActivity[clientId] = DateTime.UtcNow;
    }

    private void CheckInactiveClients()
    {
        var now = DateTime.UtcNow;

        foreach (var kvp in _lastActivity.ToList())
        {
            var inactive = now - kvp.Value;

            if (inactive > _timeout)
            {
                Console.WriteLine($"Client {kvp.Key} inactive for {inactive.TotalMinutes:F1} minutes");
                // Optionally take action
            }
        }
    }
}

// Usage
var monitor = new ConnectionMonitor(broadcaster);

characteristic.ReadRequested += (s, args) =>
{
    monitor.RecordActivity(args.ClientId);
};

characteristic.WriteRequested += (s, args) =>
{
    monitor.RecordActivity(args.ClientId);
};
```

### Client Registry

```csharp
class ClientRegistry
{
    private readonly Dictionary<string, IBluetoothConnectedDevice> _clients = new();
    private readonly IBluetoothBroadcaster _broadcaster;

    public ClientRegistry(IBluetoothBroadcaster broadcaster)
    {
        _broadcaster = broadcaster;

        broadcaster.ClientDevicesAdded += (s, args) =>
        {
            foreach (var client in args.Items)
            {
                _clients[client.Id] = client;
                Console.WriteLine($"Registered client: {client.Id}");
            }
        };

        broadcaster.ClientDevicesRemoved += (s, args) =>
        {
            foreach (var client in args.Items)
            {
                _clients.Remove(client.Id);
                Console.WriteLine($"Unregistered client: {client.Id}");
            }
        };
    }

    public int ClientCount => _clients.Count;

    public IEnumerable<IBluetoothConnectedDevice> GetAllClients()
        => _clients.Values.ToList();

    public IBluetoothConnectedDevice? GetClient(string clientId)
        => _clients.TryGetValue(clientId, out var client) ? client : null;

    public bool IsClientConnected(string clientId)
        => _clients.ContainsKey(clientId);

    public void LogStatus()
    {
        Console.WriteLine($"\n=== Connected Clients ({ClientCount}) ===");

        foreach (var client in _clients.Values)
        {
            Console.WriteLine($"  - {client.Name ?? client.Id}");
        }
    }
}
```

### Subscription Tracker

```csharp
class SubscriptionTracker
{
    private readonly Dictionary<string, HashSet<Guid>> _subscriptions = new();

    public void TrackCharacteristic(IBluetoothLocalCharacteristic characteristic)
    {
        characteristic.ClientSubscribed += (s, args) =>
        {
            var clientId = args.ClientId;

            if (!_subscriptions.ContainsKey(clientId))
                _subscriptions[clientId] = new HashSet<Guid>();

            _subscriptions[clientId].Add(characteristic.Id);

            Console.WriteLine($"Client {clientId} subscribed to {characteristic.Name}");
            Console.WriteLine($"  Total subscriptions for this client: {_subscriptions[clientId].Count}");
        };

        characteristic.ClientUnsubscribed += (s, args) =>
        {
            var clientId = args.ClientId;

            if (_subscriptions.ContainsKey(clientId))
            {
                _subscriptions[clientId].Remove(characteristic.Id);

                if (_subscriptions[clientId].Count == 0)
                    _subscriptions.Remove(clientId);
            }

            Console.WriteLine($"Client {clientId} unsubscribed from {characteristic.Name}");
        };
    }

    public int GetSubscriptionCount(string clientId)
    {
        return _subscriptions.TryGetValue(clientId, out var subs) ? subs.Count : 0;
    }

    public void RemoveClient(string clientId)
    {
        _subscriptions.Remove(clientId);
    }
}
```

## Best Practices

1. **Track Connected Clients**: Maintain a registry of connected clients
   ```csharp
   var connectedClients = new Dictionary<string, ClientState>();

   broadcaster.ClientDevicesAdded += (s, args) =>
   {
       foreach (var client in args.Items)
           connectedClients[client.Id] = new ClientState();
   };

   broadcaster.ClientDevicesRemoved += (s, args) =>
   {
       foreach (var client in args.Items)
           connectedClients.Remove(client.Id);
   };
   ```

2. **Clean Up on Disconnect**: Always clean up client-specific state
   ```csharp
   broadcaster.ClientDevicesRemoved += (s, args) =>
   {
       foreach (var client in args.Items)
           CleanupClientData(client.Id);
   };
   ```

3. **Handle Anonymous Clients**: Not all clients provide names
   ```csharp
   string displayName = client.Name ?? $"Client-{client.Id.Substring(0, 8)}";
   ```

4. **Monitor Connection Count**: Track how many clients are connected
   ```csharp
   broadcaster.ClientDeviceListChanged += (s, args) =>
   {
       Console.WriteLine($"Active clients: {broadcaster.GetClientDevices().Count}");
   };
   ```

5. **Log Client Activity**: Track when clients connect/disconnect for debugging
   ```csharp
   broadcaster.ClientDevicesAdded += (s, args) =>
   {
       foreach (var client in args.Items)
           logger.LogInformation($"Client {client.Id} connected at {DateTime.UtcNow}");
   };
   ```

## Limitations

### Platform Differences

Connected Device support varies by platform:

- **Android**: Full support, can track multiple clients
- **iOS/macOS**: Full support in peripheral mode
- **Windows**: Full support; a narrow set of operations (e.g. force-disconnecting a subscribed client) throw `NotSupportedException` due to Windows API limits

### What You Can't Do

With `IBluetoothConnectedDevice`, you typically **cannot**:
- **Disconnect a specific client** (most platforms don't expose this)
- **Send notifications to specific clients only** (broadcasts to all subscribed)
- **Get client RSSI or signal strength**
- **Initiate connections** (clients connect to you, not vice versa)

These are limitations of the underlying Bluetooth stack, not the library.

## Troubleshooting

### Client ID is Not Human-Readable

- Use `client.Name` for display, fall back to `client.Id` if name is null
- Most clients don't provide names by default
- The ID is typically a platform-specific identifier (MAC address, UUID, etc.)

### Client Name is Always Null

- Not all platforms provide client names
- Clients may not expose their device name
- This is normal behavior - use IDs instead

### Can't Disconnect Specific Clients

- Most platforms don't expose per-client disconnection APIs
- Clients disconnect themselves
- Stop broadcasting to disconnect all clients

### Events Not Firing

- Ensure broadcaster is running: `broadcaster.IsRunning`
- Check that event handlers are attached before starting
- Verify clients are actually connecting (check with BLE scanner app)

## Related Topics

- [Broadcaster](./Broadcaster.md) - Managing client connections
- [Local Characteristic](./Local-Characteristic.md) - Providing data to clients
- [Local Service](./Local-Service.md) - Organizing characteristics
- [Device](./Device.md) - Client-side equivalent (Remote Device)
