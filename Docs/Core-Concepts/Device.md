# Device

## Overview

A **Device** (also called a Remote Device) represents a Bluetooth Low Energy peripheral that you've discovered through scanning. It provides methods to connect, disconnect, and explore the device's services and characteristics.

**Interface:** `IBluetoothRemoteDevice`

## What Does It Do?

The Device interface allows you to:
- Connect and disconnect from BLE devices
- Explore the device's GATT hierarchy (services, characteristics, descriptors)
- Monitor connection state changes
- Access device properties (name, address, signal strength, battery level)
- Configure connection parameters

## GATT Hierarchy

Understanding the GATT (Generic Attribute Profile) structure is key to working with BLE devices:

```
Device
  └── Service (e.g., Heart Rate Service)
        └── Characteristic (e.g., Heart Rate Measurement)
              └── Descriptor (e.g., Client Characteristic Configuration)
```

Each level serves a purpose:
- **Device**: The physical hardware
- **Service**: A collection of related functionality
- **Characteristic**: A specific data value you can read/write/listen to
- **Descriptor**: Configuration or metadata for a characteristic

## Basic Workflow

```
┌──────────┐      ┌─────────┐      ┌──────────┐      ┌────────────┐
│ Discover │─────▶│ Connect │─────▶│ Explore  │─────▶│   Access   │
│  Device  │      │         │      │ Services │      │    Data    │
└──────────┘      └─────────┘      └──────────┘      └────────────┘
                                                              │
                                                              ▼
                                                      ┌──────────────┐
                                                      │  Disconnect  │
                                                      └──────────────┘
```

## Getting Started

### 1. Discover a Device

First, use the Scanner to find your device:

```csharp
IBluetoothRemoteDevice device = null;

scanner.AdvertisementReceived += (s, args) =>
{
    if (args.Advertisement.DeviceName == "MyDevice")
    {
        device = args.Device;
    }
};

await scanner.StartScanningAsync();
await Task.Delay(TimeSpan.FromSeconds(5));
await scanner.StopScanningAsync();
```

### 2. Connect to the Device

```csharp
// Simple connection
await device.ConnectAsync();

// With options
await device.ConnectAsync(new ConnectionOptions
{
    WaitForAdvertisementBeforeConnecting = true
});

// Safe variant - only connects if not already connected
await device.ConnectIfNeededAsync();
```

### 3. Explore Services

Before you can access data, you need to discover what the device offers:

```csharp
// Discover only services
await device.ExploreServicesAsync();

// Discover services AND characteristics
await device.ExploreServicesAsync(ServiceExplorationOptions.WithCharacteristics);

// Full discovery (services + characteristics + descriptors)
await device.ExploreServicesAsync(ServiceExplorationOptions.Full);
```

### 4. Access Services

Once explored, retrieve services:

```csharp
// Get a specific service by UUID
var heartRateServiceUuid = Guid.Parse("0000180D-0000-1000-8000-00805F9B34FB");
var service = device.GetService(heartRateServiceUuid);

// Get service by filter
var service = device.GetService(s => s.Name.Contains("Battery"));

// Get all services
var allServices = device.GetServices();

// Safe retrieval (returns null if not found)
var service = device.GetServiceOrDefault(heartRateServiceUuid);
```

### 5. Disconnect

```csharp
await device.DisconnectAsync();

// Safe variant - only disconnects if connected
await device.DisconnectIfNeededAsync();
```

## Connection States

Monitor the device's connection state:

```csharp
// Check current state
bool isConnected = device.IsConnected;
bool isConnecting = device.IsConnecting;
bool isDisconnecting = device.IsDisconnecting;

// Listen for state changes
device.Connected += (s, e) =>
    Console.WriteLine("Device connected!");

device.Disconnected += (s, e) =>
    Console.WriteLine("Device disconnected!");

device.ConnectionStateChanged += (s, args) =>
    Console.WriteLine($"State: {args.NewState}");

// Wait for a specific state
await device.WaitForIsConnectedAsync(isConnected: true);
```

## Exploration Options

The `ServiceExplorationOptions` class controls how deeply you explore the device:

### Exploration Depth

```csharp
// Option 1: Using predefined options
await device.ExploreServicesAsync(ServiceExplorationOptions.ServicesOnly);
await device.ExploreServicesAsync(ServiceExplorationOptions.WithCharacteristics);
await device.ExploreServicesAsync(ServiceExplorationOptions.Full);

// Option 2: Custom configuration
await device.ExploreServicesAsync(new ServiceExplorationOptions
{
    Depth = ExplorationDepth.Characteristics,
    UseCache = false,  // Force re-exploration
    ServiceUuidFilter = uuid => uuid == myServiceUuid  // Only explore specific service
});
```

**Depth Levels:**
- `ExplorationDepth.ServicesOnly`: Discover services only
- `ExplorationDepth.Characteristics`: Discover services and characteristics
- `ExplorationDepth.Descriptors`: Full discovery (services + characteristics + descriptors)

### Caching

By default, exploration results are cached:

```csharp
// First call: actually explores the device
await device.ExploreServicesAsync();

// Second call: returns immediately (uses cache)
await device.ExploreServicesAsync();

// Force re-exploration
await device.ExploreServicesAsync(new ServiceExplorationOptions
{
    UseCache = false
});
```

### Filtering

Limit exploration to specific services:

```csharp
var batteryServiceUuid = Guid.Parse("0000180F-0000-1000-8000-00805F9B34FB");

await device.ExploreServicesAsync(new ServiceExplorationOptions
{
    Depth = ExplorationDepth.Characteristics,
    ServiceUuidFilter = uuid => uuid == batteryServiceUuid
});
```

## Connection Options

Customize connection behavior:

```csharp
var options = new ConnectionOptions
{
    // Wait for device to advertise before connecting
    WaitForAdvertisementBeforeConnecting = true,

    // Configure retry behavior
    ConnectionRetry = new RetryOptions
    {
        MaxRetries = 3,
        DelayBetweenRetries = TimeSpan.FromMilliseconds(200)
    },

    // Platform-specific options
    Android = new AndroidConnectionOptions
    {
        AutoConnect = false,
        ConnectionPriority = BluetoothConnectionPriority.High,
        TransportType = BluetoothTransportType.Le
    }
};

await device.ConnectAsync(options);
```

### Connection Priority (Android)

On Android, you can adjust connection parameters for different use cases:

```csharp
// After connecting, change priority
await device.RequestConnectionPriorityAsync(
    BluetoothConnectionPriority.High  // Low latency for fast transfers
);
```

**Priority Modes:**
- `Balanced`: Default, reasonable performance (30-50ms interval)
- `High`: Low latency, high power (11-15ms interval) - for gaming, real-time data
- `LowPower`: Battery optimization (100-125ms interval) - for infrequent updates

## Service List Management

Work with the list of discovered services:

```csharp
// Check if service exists
bool hasService = device.HasService(serviceUuid);

// Get multiple services
var services = device.GetServices(s => s.Name.StartsWith("Custom"));

// Clear all services (useful before re-exploration)
await device.ClearServicesAsync();

// Listen for service changes
device.ServiceListChanged += (s, args) =>
{
    Console.WriteLine($"Total services: {args.TotalCount}");
};

device.ServicesAdded += (s, args) =>
{
    foreach (var service in args.Services)
        Console.WriteLine($"Added: {service.Name}");
};

device.ServicesRemoved += (s, args) =>
{
    foreach (var service in args.Services)
        Console.WriteLine($"Removed: {service.Name}");
};
```

## Unexpected Disconnections

Handle unexpected disconnections gracefully:

```csharp
device.UnexpectedDisconnection += (s, args) =>
{
    Console.WriteLine("Device disconnected unexpectedly!");
    Console.WriteLine($"Reason: {args.Reason}");

    // Attempt reconnection
    Task.Run(async () =>
    {
        await Task.Delay(TimeSpan.FromSeconds(1));
        await device.ConnectIfNeededAsync();
    });
};

// Ignore the next unexpected disconnection
// (useful when you expect the device to disconnect)
device.IgnoreNextUnexpectedDisconnection = true;
await device.DisconnectAsync();
```

## Common Patterns

### Complete Connection Flow

```csharp
async Task ConnectAndExploreAsync(IBluetoothRemoteDevice device)
{
    try
    {
        // Connect
        await device.ConnectAsync();
        Console.WriteLine("Connected!");

        // Full exploration
        await device.ExploreServicesAsync(ServiceExplorationOptions.Full);
        Console.WriteLine($"Found {device.GetServices().Count} services");

        // List all services
        foreach (var service in device.GetServices())
        {
            Console.WriteLine($"Service: {service.Name} ({service.Id})");

            foreach (var characteristic in service.GetCharacteristics())
            {
                Console.WriteLine($"  Characteristic: {characteristic.Name}");
                Console.WriteLine($"    Can Read: {characteristic.CanRead}");
                Console.WriteLine($"    Can Write: {characteristic.CanWrite}");
                Console.WriteLine($"    Can Listen: {characteristic.CanListen}");
            }
        }
    }
    catch (TimeoutException)
    {
        Console.WriteLine("Connection timed out");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
    }
}
```

### Find and Connect to Specific Device

```csharp
async Task<IBluetoothRemoteDevice> FindAndConnectAsync(string deviceName)
{
    var scanner = BluetoothFactory.Current.Scanner;
    IBluetoothRemoteDevice device = null;

    // Find device
    var handler = new EventHandler<AdvertisementReceivedEventArgs>((s, args) =>
    {
        if (args.Advertisement.DeviceName == deviceName)
            device = args.Device;
    });

    scanner.AdvertisementReceived += handler;
    await scanner.StartScanningAsync();

    // Wait for device (with timeout)
    var startTime = DateTime.UtcNow;
    while (device == null && DateTime.UtcNow - startTime < TimeSpan.FromSeconds(10))
    {
        await Task.Delay(100);
    }

    scanner.AdvertisementReceived -= handler;
    await scanner.StopScanningAsync();

    if (device == null)
        throw new TimeoutException($"Device '{deviceName}' not found");

    // Connect to device
    await device.ConnectAsync();

    return device;
}
```

### Robust Connection with Retry

```csharp
async Task RobustConnectAsync(IBluetoothRemoteDevice device, int maxRetries = 3)
{
    for (int i = 0; i < maxRetries; i++)
    {
        try
        {
            await device.ConnectAsync();
            Console.WriteLine("Connected successfully!");
            return;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Attempt {i + 1} failed: {ex.Message}");

            if (i < maxRetries - 1)
            {
                await Task.Delay(TimeSpan.FromSeconds(1));
            }
            else
            {
                throw;
            }
        }
    }
}
```

## Best Practices

1. **Always Disconnect**: Release resources when done
   ```csharp
   try
   {
       await device.ConnectAsync();
       // Work with device...
   }
   finally
   {
       await device.DisconnectIfNeededAsync();
   }
   ```

2. **Explore Before Accessing**: Always call `ExploreServicesAsync()` before trying to access services

3. **Use Appropriate Exploration Depth**: Only explore what you need
   - Need services only? Use `ServicesOnly`
   - Need characteristics? Use `WithCharacteristics`
   - Need everything? Use `Full`

4. **Handle Connection Failures**: BLE connections can be unreliable
   ```csharp
   try
   {
       await device.ConnectAsync();
   }
   catch (TimeoutException)
   {
       // Retry or inform user
   }
   ```

5. **Monitor Connection State**: Listen to `Connected`/`Disconnected` events

6. **Dispose Properly**: Device implements `IAsyncDisposable`
   ```csharp
   await using var device = args.Device;
   await device.ConnectAsync();
   // Use device...
   // Automatically disconnected and disposed
   ```

## Troubleshooting

### Connection Fails

- Ensure device is in range and powered on
- Check Bluetooth permissions
- Try increasing the connection timeout
- Enable connection retry in `ConnectionOptions`
- On Android, ensure location services are enabled

### Services Not Found

- Call `ExploreServicesAsync()` after connecting
- Use appropriate exploration depth
- Check if the device actually offers the service
- Try `UseCache = false` to force re-exploration

### Unexpected Disconnections

- Implement reconnection logic in `UnexpectedDisconnection` event
- Check signal strength (device may be out of range)
- Verify device battery isn't depleted
- Some devices disconnect after a period of inactivity

### Slow Connection

- Remove `WaitForAdvertisementBeforeConnecting` option
- On Android, use `High` connection priority for faster transfers
- Reduce exploration depth if you don't need all data

## Related Topics

- [Scanner](./Scanner.md) - Discover devices
- [Service](./Service.md) - Access device services
- [Characteristic](./Characteristic.md) - Read/write device data
- [Advertisement](./Advertisement.md) - Device advertisement data
