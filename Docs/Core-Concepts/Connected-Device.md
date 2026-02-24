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

```csharp
var broadcaster = BluetoothFactory.Current.Broadcaster;

// Listen for new connections
broadcaster.ClientDeviceConnected += (sender, args) =>
{
    IBluetoothConnectedDevice client = args.Device;
    Console.WriteLine($"Client connected: {client.Name ?? client.Id}");
};

// Listen for disconnections
broadcaster.ClientDeviceDisconnected += (sender, args) =>
{
    IBluetoothConnectedDevice client = args.Device;
    Console.WriteLine($"Client disconnected: {client.Name ?? client.Id}");
};
```

### 2. Get Connected Clients

```csharp
// Get all currently connected clients
var connectedClients = broadcaster.GetConnectedDevices();
Console.WriteLine($"Connected clients: {connectedClients.Count}");

// Get specific client by ID
var client = broadcaster.GetConnectedDevice(clientId);

// Safe retrieval
var client = broadcaster.GetConnectedDeviceOrDefault(clientId);
if (client != null)
{
    Console.WriteLine($"Found client: {client.Name}");
}

// Check if specific client is connected
bool isConnected = broadcaster.HasConnectedDevice(clientId);
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
var clients = broadcaster.GetConnectedDevices();

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
var matchingClients = broadcaster.GetConnectedDevices(
    client => client.Name?.Contains("Phone") == true
);

// Get first connected client
var firstClient = broadcaster.GetConnectedDevices().FirstOrDefault();
```

### Track Connection Count

```csharp
broadcaster.ConnectedDeviceListChanged += (sender, args) =>
{
    Console.WriteLine($"Connected clients changed. Total: {args.TotalCount}");
};
```

## Events

Monitor client connection lifecycle:

```csharp
// New client connected
broadcaster.ClientDeviceConnected += (sender, args) =>
{
    var client = args.Device;
    Console.WriteLine($"✓ Client connected: {client.Id}");

    // Initialize per-client state
    clientDataStore[client.Id] = new ClientData();
};

// Client disconnected
broadcaster.ClientDeviceDisconnected += (sender, args) =>
{
    var client = args.Device;
    Console.WriteLine($"✗ Client disconnected: {client.Id}");

    // Clean up per-client state
    clientDataStore.Remove(client.Id);
};

// Any change to connected device list
broadcaster.ConnectedDeviceListChanged += (sender, args) =>
{
    Console.WriteLine($"Connected clients: {args.TotalCount}");
};

// Clients added (may include multiple clients)
broadcaster.ConnectedDevicesAdded += (sender, args) =>
{
    foreach (var client in args.Devices)
        Console.WriteLine($"Added: {client.Id}");
};

// Clients removed (may include multiple clients)
broadcaster.ConnectedDevicesRemoved += (sender, args) =>
{
    foreach (var client in args.Devices)
        Console.WriteLine($"Removed: {client.Id}");
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

broadcaster.ClientDeviceConnected += (sender, args) =>
{
    var client = args.Device;
    clientDataStore[client.Id] = new ClientData
    {
        ConnectedAt = DateTime.UtcNow,
        RequestCount = 0,
        LastActivityAt = DateTime.UtcNow
    };
};

broadcaster.ClientDeviceDisconnected += (sender, args) =>
{
    var client = args.Device;

    if (clientDataStore.TryGetValue(client.Id, out var data))
    {
        var duration = DateTime.UtcNow - data.ConnectedAt;
        Console.WriteLine($"Client {client.Id} was connected for {duration.TotalSeconds:F1}s");
        Console.WriteLine($"Total requests: {data.RequestCount}");

        clientDataStore.Remove(client.Id);
    }
};

// Track requests
characteristic.ReadRequestReceived += (sender, args) =>
{
    var clientId = args.Device.Id;
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

broadcaster.ClientDeviceConnected += async (sender, args) =>
{
    var clients = broadcaster.GetConnectedDevices();

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
        broadcaster.ClientDeviceConnected += (s, args) =>
        {
            _lastActivity[args.Device.Id] = DateTime.UtcNow;
        };

        broadcaster.ClientDeviceDisconnected += (s, args) =>
        {
            _lastActivity.Remove(args.Device.Id);
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

characteristic.ReadRequestReceived += (s, args) =>
{
    monitor.RecordActivity(args.Device.Id);
};

characteristic.WriteRequestReceived += (s, args) =>
{
    monitor.RecordActivity(args.Device.Id);
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

        broadcaster.ClientDeviceConnected += (s, args) =>
        {
            _clients[args.Device.Id] = args.Device;
            Console.WriteLine($"Registered client: {args.Device.Id}");
        };

        broadcaster.ClientDeviceDisconnected += (s, args) =>
        {
            _clients.Remove(args.Device.Id);
            Console.WriteLine($"Unregistered client: {args.Device.Id}");
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
            var clientId = args.Device.Id;

            if (!_subscriptions.ContainsKey(clientId))
                _subscriptions[clientId] = new HashSet<Guid>();

            _subscriptions[clientId].Add(characteristic.Id);

            Console.WriteLine($"Client {clientId} subscribed to {characteristic.Name}");
            Console.WriteLine($"  Total subscriptions for this client: {_subscriptions[clientId].Count}");
        };

        characteristic.ClientUnsubscribed += (s, args) =>
        {
            var clientId = args.Device.Id;

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

   broadcaster.ClientDeviceConnected += (s, args) =>
   {
       connectedClients[args.Device.Id] = new ClientState();
   };

   broadcaster.ClientDeviceDisconnected += (s, args) =>
   {
       connectedClients.Remove(args.Device.Id);
   };
   ```

2. **Clean Up on Disconnect**: Always clean up client-specific state
   ```csharp
   broadcaster.ClientDeviceDisconnected += (s, args) =>
   {
       CleanupClientData(args.Device.Id);
   };
   ```

3. **Handle Anonymous Clients**: Not all clients provide names
   ```csharp
   string displayName = client.Name ?? $"Client-{client.Id.Substring(0, 8)}";
   ```

4. **Monitor Connection Count**: Track how many clients are connected
   ```csharp
   broadcaster.ConnectedDeviceListChanged += (s, args) =>
   {
       Console.WriteLine($"Active clients: {args.TotalCount}");
   };
   ```

5. **Log Client Activity**: Track when clients connect/disconnect for debugging
   ```csharp
   broadcasters.ClientDeviceConnected += (s, args) =>
   {
       logger.LogInformation($"Client {args.Device.Id} connected at {DateTime.UtcNow}");
   };
   ```

## Limitations

### Platform Differences

Connected Device support varies by platform:

- **Android**: Good support, can track multiple clients
- **iOS**: Full support in peripheral mode
- **Windows**: Limited support depending on adapter

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
