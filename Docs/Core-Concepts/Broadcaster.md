# Broadcaster

## Overview

The **Broadcaster** (also called Peripheral or GATT Server) allows your device to act as a BLE server. Instead of scanning for and connecting to other devices, your app can advertise services and accept connections from other devices (called Central or Client devices).

**Interface:** `IBluetoothBroadcaster`

## What Does It Do?

The Broadcaster allows you to:
- Advertise your device's presence to nearby scanners
- Host GATT services that clients can discover and interact with
- Accept connections from central devices
- Provide data to connected clients
- Notify clients when data changes

## Use Cases

Broadcasting is useful for:
- **IoT Sensors**: Temperature sensor that broadcasts readings
- **Fitness Devices**: Heart rate monitor that sends data to phone
- **Smart Home**: Light bulb that accepts control commands
- **Beacons**: Proximity beacons for location services
- **Peer-to-Peer**: Direct communication between mobile devices

## Basic Workflow

```
┌───────────┐      ┌────────────┐      ┌────────────┐      ┌──────────┐
│  Request  │─────▶│   Create   │─────▶│   Start    │─────▶│ Handle   │
│Permission │      │  Services  │      │Broadcasting│      │ Clients  │
└───────────┘      └────────────┘      └────────────┘      └──────────┘
```

## Getting Started

### 1. Request Permissions

```csharp
var broadcaster = BluetoothFactory.Current.Broadcaster;

// Check permissions
bool hasPermission = await broadcaster.HasBroadcasterPermissionsAsync();

if (!hasPermission)
{
    // Request permissions
    await broadcaster.RequestBroadcasterPermissionsAsync();
}
```

**Platform Notes:**
- **Android**: Requests `BLUETOOTH_ADVERTISE` permission (API 31+)
- **iOS/macOS**: Requests Bluetooth Always + Peripheral permissions
- **Windows**: Checks adapter and peripheral role support

### 2. Create a Service

```csharp
// Create a simple service with one characteristic
var serviceSpec = new IBluetoothLocalServiceFactory.BluetoothLocalServiceSpec
{
    ServiceId = Guid.Parse("12345678-1234-1234-1234-123456789abc"),
    IsPrimary = true,
    Characteristics = new[]
    {
        new IBluetoothLocalCharacteristicFactory.BluetoothLocalCharacteristicSpec
        {
            CharacteristicId = Guid.Parse("12345678-1234-1234-1234-123456789abd"),
            Properties = BluetoothCharacteristicProperties.Read |
                         BluetoothCharacteristicProperties.Notify,
            Permissions = BluetoothCharacteristicPermissions.Read,
            InitialValue = new byte[] { 0x00 }
        }
    }
};

var service = await broadcaster.CreateServiceAsync(serviceSpec);
```

### 3. Start Broadcasting

```csharp
await broadcaster.StartBroadcastingAsync(new BroadcastingOptions
{
    DeviceName = "MyDevice",
    IsConnectable = true
});

Console.WriteLine("Broadcasting started!");
```

### 4. Handle Client Connections

```csharp
// Monitor connected clients
broadcaster.ClientDeviceConnected += (s, args) =>
{
    var client = args.Device;
    Console.WriteLine($"Client connected: {client.Name}");
};

broadcaster.ClientDeviceDisconnected += (s, args) =>
{
    var client = args.Device;
    Console.WriteLine($"Client disconnected: {client.Name}");
};
```

### 5. Stop Broadcasting

```csharp
await broadcaster.StopBroadcastingAsync();
```

## Service Management

### Create Services

```csharp
// Define service specification
var serviceSpec = new IBluetoothLocalServiceFactory.BluetoothLocalServiceSpec
{
    ServiceId = Guid.Parse("0000180F-0000-1000-8000-00805F9B34FB"), // Battery Service
    IsPrimary = true,
    Characteristics = new[]
    {
        new IBluetoothLocalCharacteristicFactory.BluetoothLocalCharacteristicSpec
        {
            CharacteristicId = Guid.Parse("00002A19-0000-1000-8000-00805F9B34FB"), // Battery Level
            Properties = BluetoothCharacteristicProperties.Read |
                         BluetoothCharacteristicProperties.Notify,
            Permissions = BluetoothCharacteristicPermissions.Read,
            InitialValue = new byte[] { 100 }  // 100% battery
        }
    }
};

// Create the service
var service = await broadcaster.CreateServiceAsync(serviceSpec);
```

### Get Services

```csharp
// Get specific service
var service = broadcaster.GetService(serviceUuid);

// Get all services
var allServices = broadcaster.GetServices();

// Safe retrieval
var service = broadcaster.GetServiceOrDefault(serviceUuid);
if (service != null)
{
    // Use service
}

// Check if service exists
bool hasService = broadcaster.HasService(serviceUuid);
```

### Remove Services

```csharp
// Remove specific service
await broadcaster.RemoveServiceAsync(serviceUuid);

// Remove by reference
await broadcaster.RemoveServiceAsync(service);

// Remove all services
await broadcaster.RemoveAllServicesAsync();
```

## Broadcasting Options

Customize your broadcast behavior:

```csharp
var options = new BroadcastingOptions
{
    // Device name shown to scanners
    DeviceName = "MyDevice",

    // Allow connections (vs broadcast-only)
    IsConnectable = true,

    // Service UUIDs to advertise
    ServiceUuids = new[]
    {
        Guid.Parse("0000180F-0000-1000-8000-00805F9B34FB")
    },

    // Permission strategy
    PermissionStrategy = PermissionRequestStrategy.RequestAutomatically
};

await broadcaster.StartBroadcastingAsync(options);
```

## State Management

Monitor the broadcaster's state:

```csharp
// Check current state
bool isRunning = broadcaster.IsRunning;
bool isStarting = broadcaster.IsStarting;
bool isStopping = broadcaster.IsStopping;

// Listen for state changes
broadcaster.Starting += (s, e) =>
    Console.WriteLine("Broadcaster starting...");

broadcaster.Started += (s, e) =>
    Console.WriteLine("Broadcaster started!");

broadcaster.Stopping += (s, e) =>
    Console.WriteLine("Broadcaster stopping...");

broadcaster.Stopped += (s, e) =>
    Console.WriteLine("Broadcaster stopped!");

broadcaster.RunningStateChanged += (s, e) =>
    Console.WriteLine($"Running: {broadcaster.IsRunning}");
```

## Client Device Management

Track connected clients:

```csharp
// Get all connected clients
var clients = broadcaster.GetConnectedDevices();

// Get specific client
var client = broadcaster.GetConnectedDevice(clientId);

// Check if client is connected
bool hasClient = broadcaster.HasConnectedDevice(clientId);

// Listen for connection events
broadcaster.ClientDeviceConnected += (s, args) =>
{
    Console.WriteLine($"Client {args.Device.Id} connected");
};

broadcaster.ClientDeviceDisconnected += (s, args) =>
{
    Console.WriteLine($"Client {args.Device.Id} disconnected");
};

broadcaster.ConnectedDeviceListChanged += (s, args) =>
{
    Console.WriteLine($"Total clients: {args.TotalCount}");
};
```

## Common Patterns

### Simple Battery Service

```csharp
async Task CreateBatteryServiceAsync()
{
    var broadcaster = BluetoothFactory.Current.Broadcaster;

    // Create battery service
    var serviceSpec = new IBluetoothLocalServiceFactory.BluetoothLocalServiceSpec
    {
        ServiceId = Guid.Parse("0000180F-0000-1000-8000-00805F9B34FB"),
        IsPrimary = true,
        Characteristics = new[]
        {
            new IBluetoothLocalCharacteristicFactory.BluetoothLocalCharacteristicSpec
            {
                CharacteristicId = Guid.Parse("00002A19-0000-1000-8000-00805F9B34FB"),
                Properties = BluetoothCharacteristicProperties.Read |
                             BluetoothCharacteristicProperties.Notify,
                Permissions = BluetoothCharacteristicPermissions.Read,
                InitialValue = new byte[] { 100 }
            }
        }
    };

    var service = await broadcaster.CreateServiceAsync(serviceSpec);

    // Start broadcasting
    await broadcaster.StartBroadcastingAsync(new BroadcastingOptions
    {
        DeviceName = "Battery Monitor",
        IsConnectable = true,
        ServiceUuids = new[] { serviceSpec.ServiceId }
    });

    Console.WriteLine("Battery service is broadcasting");
}
```

### Update Characteristic Value

```csharp
async Task UpdateBatteryLevelAsync(int level)
{
    var service = broadcaster.GetService(batteryServiceUuid);
    var characteristic = service.GetCharacteristic(batteryLevelUuid);

    // Update value and notify subscribers
    await characteristic.UpdateValueAsync(
        new byte[] { (byte)level },
        notifyClients: true
    );

    Console.WriteLine($"Battery level updated to {level}%");
}
```

### Temperature Sensor

```csharp
async Task CreateTemperatureSensorAsync()
{
    var broadcaster = BluetoothFactory.Current.Broadcaster;

    var serviceSpec = new IBluetoothLocalServiceFactory.BluetoothLocalServiceSpec
    {
        ServiceId = Guid.Parse("00001809-0000-1000-8000-00805F9B34FB"), // Health Thermometer
        IsPrimary = true,
        Characteristics = new[]
        {
            new IBluetoothLocalCharacteristicFactory.BluetoothLocalCharacteristicSpec
            {
                CharacteristicId = Guid.Parse("00002A1C-0000-1000-8000-00805F9B34FB"),
                Properties = BluetoothCharacteristicProperties.Indicate,
                Permissions = BluetoothCharacteristicPermissions.Read,
                InitialValue = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00 }
            }
        }
    };

    var service = await broadcaster.CreateServiceAsync(serviceSpec);
    await broadcaster.StartBroadcastingAsync(new BroadcastingOptions
    {
        DeviceName = "Temperature Sensor",
        IsConnectable = true
    });

    // Simulate temperature readings
    var characteristic = service.GetCharacteristic(Guid.Parse("00002A1C-0000-1000-8000-00805F9B34FB"));

    while (broadcaster.IsRunning)
    {
        // Read temperature (simulated)
        float temperature = 36.5f + Random.Shared.NextSingle() * 2;

        // Format according to Health Thermometer spec
        var tempBytes = FormatTemperature(temperature);
        await characteristic.UpdateValueAsync(tempBytes, notifyClients: true);

        await Task.Delay(TimeSpan.FromSeconds(5));
    }
}

byte[] FormatTemperature(float celsius)
{
    // Flags: Celsius, no timestamp, no temperature type
    byte flags = 0x00;

    // Temperature in IEEE-11073 float format
    int tempValue = (int)(celsius * 10);
    byte[] temp = BitConverter.GetBytes(tempValue);

    return new byte[] { flags, temp[0], temp[1], temp[2], temp[3] };
}
```

### Handle Client Requests

```csharp
// Track which clients are subscribed
var subscribedClients = new HashSet<string>();

characteristic.ClientSubscribed += (s, args) =>
{
    var clientId = args.Device.Id;
    subscribedClients.Add(clientId);
    Console.WriteLine($"Client {clientId} subscribed to notifications");
};

characteristic.ClientUnsubscribed += (s, args) =>
{
    var clientId = args.Device.Id;
    subscribedClients.Remove(clientId);
    Console.WriteLine($"Client {clientId} unsubscribed");
};

// Only notify subscribed clients
if (subscribedClients.Count > 0)
{
    await characteristic.UpdateValueAsync(newValue, notifyClients: true);
}
```

## Advanced Features

### Dynamic Service Updates

```csharp
// Remove old service
await broadcaster.RemoveServiceAsync(oldServiceUuid);

// Add new service
var service = await broadcaster.CreateServiceAsync(newServiceSpec);

// Update advertising options
await broadcaster.UpdateBroadcastingOptionsAsync(new BroadcastingOptions
{
    ServiceUuids = new[] { newServiceSpec.ServiceId }
});
```

### Multiple Services

```csharp
// Create multiple services
var batteryService = await broadcaster.CreateServiceAsync(batterySpec);
var heartRateService = await broadcaster.CreateServiceAsync(heartRateSpec);
var customService = await broadcaster.CreateServiceAsync(customSpec);

// Advertise all services
await broadcaster.StartBroadcastingAsync(new BroadcastingOptions
{
    ServiceUuids = new[]
    {
        batterySpec.ServiceId,
        heartRateSpec.ServiceId,
        customSpec.ServiceId
    }
});
```

## Best Practices

1. **Request Permissions Early**: Check and request broadcaster permissions before creating services
   ```csharp
   if (!await broadcaster.HasBroadcasterPermissionsAsync())
       await broadcaster.RequestBroadcasterPermissionsAsync();
   ```

2. **Advertise Primary Services**: Only advertise your main services to save advertising space
   ```csharp
   new BroadcastingOptions
   {
       ServiceUuids = new[] { primaryServiceUuid }
   }
   ```

3. **Clean Up Resources**: Stop broadcasting and remove services when done
   ```csharp
   try
   {
       await broadcaster.StartBroadcastingAsync();
       // Use broadcaster...
   }
   finally
   {
       await broadcaster.StopBroadcastingIfNeededAsync();
       await broadcaster.RemoveAllServicesAsync();
   }
   ```

4. **Handle Client Disconnections**: Track connected clients and clean up state
   ```csharp
   broadcaster.ClientDeviceDisconnected += (s, args) =>
   {
       // Clean up client-specific state
   };
   ```

5. **Use Standard Services**: When possible, use Bluetooth SIG standard services and characteristics

6. **Notify Efficiently**: Only notify when values actually change
   ```csharp
   if (newValue != oldValue)
       await characteristic.UpdateValueAsync(newValue, notifyClients: true);
   ```

## Platform Support

Broadcasting support varies by platform:

| Platform | Support | Notes |
|----------|---------|-------|
| **Android** | Full | Requires Android 5.0+ (API 21+) |
| **iOS** | Full | Background mode requires capabilities |
| **macOS** | Full | Peripheral mode supported |
| **Windows** | Partial | Limited peripheral support, check device capabilities |

Check support before implementing:
```csharp
try
{
    await broadcaster.StartBroadcastingAsync();
}
catch (PlatformNotSupportedException ex)
{
    Console.WriteLine("Broadcasting not supported on this device");
}
```

## Troubleshooting

### Broadcasting Won't Start

- Check permissions: `await broadcaster.HasBroadcasterPermissionsAsync()`
- Ensure Bluetooth adapter supports peripheral role
- On Windows, check if device supports BLE peripheral mode
- Verify no other app is using peripheral mode

### Clients Can't Connect

- Check `IsConnectable = true` in `BroadcastingOptions`
- Verify services are created before broadcasting
- Ensure device name is set
- Check platform-specific connection limits

### Notifications Not Sent

- Verify characteristic has `Notify` or `Indicate` property
- Check that client subscribed to notifications
- Ensure `notifyClients: true` when updating value

### Services Not Visible

- Advertise service UUIDs in `BroadcastingOptions`
- Use standard UUIDs when possible
- Some platforms limit advertising data size

## Related Topics

- [Local Service](./Local-Service.md) - Managing hosted services
- [Local Characteristic](./Local-Characteristic.md) - Providing data to clients
- [Connected Device](./Connected-Device.md) - Tracking connected clients
- [Advertisement](./Advertisement.md) - Understanding advertisements
