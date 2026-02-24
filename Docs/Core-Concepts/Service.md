# Service

## Overview

A **Service** represents a collection of related functionality on a BLE device. Services group together characteristics that work together to provide a specific feature. For example, a Heart Rate Service contains characteristics for measuring heart rate, body sensor location, and heart rate control.

**Interface:** `IBluetoothRemoteService`

## What Does It Do?

A Service allows you to:
- Explore and access characteristics within the service
- Organize related device functionality
- Identify standard or custom services by UUID
- Navigate the GATT hierarchy

## GATT Hierarchy Position

```
Device
  └── Service ◄── You are here
        └── Characteristic
              └── Descriptor
```

A service sits between the device and its characteristics, acting as a logical grouping.

## Standard Services

The Bluetooth SIG defines many standard services. Here are some common ones:

| Service Name | UUID | Purpose |
|--------------|------|---------|
| Battery Service | `0000180F-0000-1000-8000-00805F9B34FB` | Battery level information |
| Heart Rate | `0000180D-0000-1000-8000-00805F9B34FB` | Heart rate measurements |
| Device Information | `0000180A-0000-1000-8000-00805F9B34FB` | Manufacturer, model, firmware version |
| Current Time | `00001805-0000-1000-8000-00805F9B34FB` | Time synchronization |
| Blood Pressure | `00001810-0000-1000-8000-00805F9B34FB` | Blood pressure measurements |

**Custom Services**: Devices can also define custom services with their own UUIDs for proprietary features.

## Getting Started

### 1. Get a Service

After connecting to a device and exploring services:

```csharp
await device.ConnectAsync();
await device.ExploreServicesAsync();

// Get service by UUID
var batteryServiceUuid = Guid.Parse("0000180F-0000-1000-8000-00805F9B34FB");
var service = device.GetService(batteryServiceUuid);

// Get service by name
var service = device.GetService(s => s.Name == "Battery Service");

// Safe retrieval
var service = device.GetServiceOrDefault(batteryServiceUuid);
if (service == null)
{
    Console.WriteLine("Battery service not found");
    return;
}
```

### 2. Explore Characteristics

Before accessing characteristics, you need to discover them:

```csharp
// Simple exploration (characteristics only)
await service.ExploreCharacteristicsAsync();

// Include descriptors
await service.ExploreCharacteristicsAsync(
    CharacteristicExplorationOptions.Full
);

// Custom options
await service.ExploreCharacteristicsAsync(new CharacteristicExplorationOptions
{
    ExploreDescriptors = true,
    UseCache = false,  // Force re-exploration
    CharacteristicUuidFilter = uuid => uuid == myCharUuid
});
```

### 3. Access Characteristics

Once explored, retrieve characteristics:

```csharp
// Get by UUID
var batteryLevelUuid = Guid.Parse("00002A19-0000-1000-8000-00805F9B34FB");
var characteristic = service.GetCharacteristic(batteryLevelUuid);

// Get by filter
var characteristic = service.GetCharacteristic(c => c.CanRead);

// Get all
var allCharacteristics = service.GetCharacteristics();

// Safe retrieval
var characteristic = service.GetCharacteristicOrDefault(batteryLevelUuid);
```

## Service Properties

### Basic Properties

```csharp
// Service UUID
Guid id = service.Id;

// Service name (human-readable)
string name = service.Name;  // e.g., "Battery Service" or "Unknown Service"

// Parent device
IBluetoothRemoteDevice device = service.Device;
```

### Example

```csharp
Console.WriteLine($"Service: {service.Name}");
Console.WriteLine($"UUID: {service.Id}");
Console.WriteLine($"Device: {service.Device}");
```

## Characteristic Exploration

### Exploration Options

```csharp
// Option 1: Default (characteristics only, with caching)
await service.ExploreCharacteristicsAsync();

// Option 2: Include descriptors
await service.ExploreCharacteristicsAsync(
    CharacteristicExplorationOptions.Full
);

// Option 3: Custom configuration
await service.ExploreCharacteristicsAsync(new CharacteristicExplorationOptions
{
    ExploreDescriptors = true,       // Include descriptors
    UseCache = false,                // Force re-exploration
    CharacteristicUuidFilter = uuid => // Only specific characteristics
        uuid == myCharacteristicUuid
});
```

### Caching Behavior

By default, exploration results are cached:

```csharp
// First call: explores the device
await service.ExploreCharacteristicsAsync();

// Second call: returns immediately (uses cache)
await service.ExploreCharacteristicsAsync();

// Force re-exploration
await service.ExploreCharacteristicsAsync(new CharacteristicExplorationOptions
{
    UseCache = false
});
```

## Characteristic List Management

### Retrieve Characteristics

```csharp
// Get single characteristic
var characteristic = service.GetCharacteristic(characteristicUuid);

// Get with filter
var writableChar = service.GetCharacteristic(c => c.CanWrite);

// Get all characteristics
var allChars = service.GetCharacteristics();

// Get with custom filter
var readableChars = service.GetCharacteristics(c => c.CanRead);
```

### Check for Characteristics

```csharp
// Check if characteristic exists
bool hasChar = service.HasCharacteristic(characteristicUuid);

// Check with filter
bool hasWritable = service.HasCharacteristic(c => c.CanWrite);
```

### Clear Characteristics

```csharp
// Clear all characteristics and stop notifications
await service.ClearCharacteristicsAsync();
```

This is useful when you want to force a complete re-discovery.

## Events

Monitor changes to the characteristic list:

```csharp
// Any change to the list
service.CharacteristicListChanged += (s, args) =>
{
    Console.WriteLine($"Total characteristics: {args.TotalCount}");
};

// Characteristics added
service.CharacteristicsAdded += (s, args) =>
{
    foreach (var characteristic in args.Characteristics)
    {
        Console.WriteLine($"Added: {characteristic.Name}");
    }
};

// Characteristics removed
service.CharacteristicsRemoved += (s, args) =>
{
    foreach (var characteristic in args.Characteristics)
    {
        Console.WriteLine($"Removed: {characteristic.Name}");
    }
};
```

## State Properties

Check the service's current state:

```csharp
// Is currently exploring characteristics?
bool isExploring = service.IsExploringCharacteristics;
```

## Common Patterns

### Complete Service Exploration

```csharp
async Task ExploreServiceAsync(IBluetoothRemoteService service)
{
    Console.WriteLine($"Exploring: {service.Name}");

    // Explore characteristics and descriptors
    await service.ExploreCharacteristicsAsync(
        CharacteristicExplorationOptions.Full
    );

    // List all characteristics
    var characteristics = service.GetCharacteristics();
    Console.WriteLine($"Found {characteristics.Count} characteristics:");

    foreach (var characteristic in characteristics)
    {
        Console.WriteLine($"  - {characteristic.Name} ({characteristic.Id})");
        Console.WriteLine($"    Can Read: {characteristic.CanRead}");
        Console.WriteLine($"    Can Write: {characteristic.CanWrite}");
        Console.WriteLine($"    Can Listen: {characteristic.CanListen}");
    }
}
```

### Find Specific Characteristic

```csharp
async Task<IBluetoothRemoteCharacteristic> FindCharacteristicAsync(
    IBluetoothRemoteService service,
    Guid characteristicUuid)
{
    // Ensure characteristics are explored
    await service.ExploreCharacteristicsAsync();

    // Try to get the characteristic
    var characteristic = service.GetCharacteristicOrDefault(characteristicUuid);

    if (characteristic == null)
    {
        throw new InvalidOperationException(
            $"Characteristic {characteristicUuid} not found in service {service.Name}"
        );
    }

    return characteristic;
}
```

### Read All Readable Characteristics

```csharp
async Task ReadAllCharacteristicsAsync(IBluetoothRemoteService service)
{
    await service.ExploreCharacteristicsAsync();

    var readableChars = service.GetCharacteristics(c => c.CanRead);

    foreach (var characteristic in readableChars)
    {
        try
        {
            var value = await characteristic.ReadValueAsync();
            Console.WriteLine($"{characteristic.Name}: {BitConverter.ToString(value.ToArray())}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to read {characteristic.Name}: {ex.Message}");
        }
    }
}
```

### Battery Service Example

```csharp
async Task ReadBatteryLevelAsync(IBluetoothRemoteDevice device)
{
    // Get Battery Service
    var batteryServiceUuid = Guid.Parse("0000180F-0000-1000-8000-00805F9B34FB");
    var service = device.GetServiceOrDefault(batteryServiceUuid);

    if (service == null)
    {
        Console.WriteLine("Device does not have a battery service");
        return;
    }

    // Explore characteristics
    await service.ExploreCharacteristicsAsync();

    // Get Battery Level characteristic
    var batteryLevelUuid = Guid.Parse("00002A19-0000-1000-8000-00805F9B34FB");
    var characteristic = service.GetCharacteristic(batteryLevelUuid);

    // Read battery level
    var value = await characteristic.ReadValueAsync();
    int batteryLevel = value.Span[0];  // First byte is the percentage

    Console.WriteLine($"Battery Level: {batteryLevel}%");
}
```

### Device Information Service Example

```csharp
async Task ReadDeviceInfoAsync(IBluetoothRemoteDevice device)
{
    var deviceInfoUuid = Guid.Parse("0000180A-0000-1000-8000-00805F9B34FB");
    var service = device.GetService(deviceInfoUuid);

    await service.ExploreCharacteristicsAsync();

    // Manufacturer Name
    var mfgNameChar = service.GetCharacteristicOrDefault(
        Guid.Parse("00002A29-0000-1000-8000-00805F9B34FB")
    );
    if (mfgNameChar != null)
    {
        var value = await mfgNameChar.ReadValueAsync();
        string manufacturer = Encoding.UTF8.GetString(value.ToArray());
        Console.WriteLine($"Manufacturer: {manufacturer}");
    }

    // Model Number
    var modelChar = service.GetCharacteristicOrDefault(
        Guid.Parse("00002A24-0000-1000-8000-00805F9B34FB")
    );
    if (modelChar != null)
    {
        var value = await modelChar.ReadValueAsync();
        string model = Encoding.UTF8.GetString(value.ToArray());
        Console.WriteLine($"Model: {model}");
    }

    // Firmware Revision
    var fwChar = service.GetCharacteristicOrDefault(
        Guid.Parse("00002A26-0000-1000-8000-00805F9B34FB")
    );
    if (fwChar != null)
    {
        var value = await fwChar.ReadValueAsync();
        string firmware = Encoding.UTF8.GetString(value.ToArray());
        Console.WriteLine($"Firmware: {firmware}");
    }
}
```

## Best Practices

1. **Always Explore First**: Call `ExploreCharacteristicsAsync()` before accessing characteristics
   ```csharp
   await service.ExploreCharacteristicsAsync();
   var characteristic = service.GetCharacteristic(uuid);
   ```

2. **Use Appropriate Exploration Depth**: Only explore what you need
   - Need characteristics only? Use default options
   - Need descriptors too? Use `CharacteristicExplorationOptions.Full`

3. **Handle Missing Characteristics**: Not all devices implement all characteristics
   ```csharp
   var characteristic = service.GetCharacteristicOrDefault(uuid);
   if (characteristic != null)
   {
       // Use characteristic
   }
   ```

4. **Cache Exploration Results**: The default `UseCache = true` avoids redundant explorations

5. **Check Characteristic Capabilities**: Before reading/writing, check `CanRead`/`CanWrite`
   ```csharp
   if (characteristic.CanRead)
   {
       var value = await characteristic.ReadValueAsync();
   }
   ```

6. **Graceful Error Handling**: Operations can fail
   ```csharp
   try
   {
       await service.ExploreCharacteristicsAsync();
   }
   catch (DeviceNotConnectedException)
   {
       Console.WriteLine("Device disconnected during exploration");
   }
   catch (TimeoutException)
   {
       Console.WriteLine("Exploration timed out");
   }
   ```

## Troubleshooting

### Characteristics Not Found

- Ensure you called `ExploreCharacteristicsAsync()` first
- Check if the device actually implements the characteristic
- Try `UseCache = false` to force re-exploration
- Verify the device is still connected

### Exploration Fails

- Check device connection: `service.Device.IsConnected`
- Ensure sufficient timeout
- Some devices require authentication before exposing all characteristics
- Check if device firmware is up to date

### Unknown Service Name

If `service.Name` returns "Unknown Service", it means:
- The service uses a custom UUID (not a standard Bluetooth SIG service)
- You're working with a proprietary service

You can still work with it using its UUID.

### Performance Issues

- Only explore characteristics when needed
- Use `UseCache = true` (default) to avoid repeated explorations
- Don't explore descriptors unless you need them
- Use UUID filters to limit exploration scope

## Related Topics

- [Device](./Device.md) - Managing device connections
- [Characteristic](./Characteristic.md) - Reading and writing data
- [Descriptor](./Descriptor.md) - Characteristic metadata
