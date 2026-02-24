# Descriptor

## Overview

A **Descriptor** provides additional information and configuration for a characteristic. Descriptors are metadata that describe how to interact with a characteristic or provide extra details about its value. The most common descriptor is the Client Characteristic Configuration Descriptor (CCCD), which controls notifications and indications.

**Interface:** `IBluetoothRemoteDescriptor`

## What Does It Do?

Descriptors allow you to:
- Enable/disable notifications on a characteristic (via CCCD)
- Get human-readable descriptions of a characteristic
- Read extended properties and format information
- Configure how a characteristic behaves

## GATT Hierarchy Position

```
Device
  └── Service
        └── Characteristic
              └── Descriptor ◄── You are here
```

Descriptors are the deepest level in the GATT hierarchy.

## Standard Descriptors

The Bluetooth SIG defines several standard descriptors:

| Descriptor | UUID | Purpose |
|------------|------|---------|
| Client Characteristic Configuration (CCCD) | `00002902-0000-1000-8000-00805F9B34FB` | Enable/disable notifications and indications |
| Characteristic User Description | `00002901-0000-1000-8000-00805F9B34FB` | Human-readable text description |
| Characteristic Presentation Format | `00002904-0000-1000-8000-00805F9B34FB` | Data format (units, exponent, etc.) |
| Characteristic Extended Properties | `00002900-0000-1000-8000-00805F9B34FB` | Additional properties |
| Characteristic Aggregate Format | `00002905-0000-1000-8000-00805F9B34FB` | Multiple format descriptors |

**Most Common**: The CCCD is by far the most commonly used descriptor, as it's required for enabling notifications.

## Getting Started

### 1. Get a Descriptor

Descriptors are typically accessed through characteristics:

```csharp
await device.ConnectAsync();
await device.ExploreServicesAsync(ServiceExplorationOptions.Full);

var service = device.GetService(serviceUuid);
var characteristic = service.GetCharacteristic(characteristicUuid);

// Descriptors are automatically discovered with Full exploration
var descriptors = characteristic.GetDescriptors();

// Get specific descriptor (e.g., CCCD)
var cccdUuid = Guid.Parse("00002902-0000-1000-8000-00805F9B34FB");
var cccd = characteristic.GetDescriptorOrDefault(cccdUuid);
```

### 2. Check Capabilities

```csharp
if (descriptor.CanRead)
    Console.WriteLine("Can read descriptor");

if (descriptor.CanWrite)
    Console.WriteLine("Can write descriptor");
```

### 3. Read a Descriptor

```csharp
if (descriptor.CanRead)
{
    var value = await descriptor.ReadValueAsync();
    Console.WriteLine($"Descriptor value: {BitConverter.ToString(value.ToArray())}");
}
```

### 4. Write a Descriptor

```csharp
if (descriptor.CanWrite)
{
    byte[] data = { 0x01, 0x00 };  // Example: Enable notifications
    await descriptor.WriteValueAsync(data);
    Console.WriteLine("Descriptor written!");
}
```

## Descriptor Properties

### Basic Properties

```csharp
// Descriptor UUID
Guid id = descriptor.Id;

// Descriptor name (human-readable)
string name = descriptor.Name;

// Parent characteristic
IBluetoothRemoteCharacteristic characteristic = descriptor.RemoteCharacteristic;

// Capability checks
bool canRead = descriptor.CanRead;
bool canWrite = descriptor.CanWrite;
```

### Value Properties

Access the current cached value:

```csharp
// As ReadOnlyMemory<byte>
ReadOnlyMemory<byte> value = descriptor.Value;

// As ReadOnlySpan<byte>
ReadOnlySpan<byte> span = descriptor.ValueSpan;
```

## Common Descriptors

### Client Characteristic Configuration Descriptor (CCCD)

The CCCD controls notifications and indications. However, you typically don't interact with it directly - the library handles this automatically when you call `StartListeningAsync()` on a characteristic:

```csharp
// High-level API (recommended)
await characteristic.StartListeningAsync();  // Library writes to CCCD automatically
await characteristic.StopListeningAsync();   // Library disables CCCD automatically

// Low-level API (manual CCCD control)
var cccdUuid = Guid.Parse("00002902-0000-1000-8000-00805F9B34FB");
var cccd = characteristic.GetDescriptor(cccdUuid);

// Enable notifications (0x01, 0x00)
await cccd.WriteValueAsync(new byte[] { 0x01, 0x00 });

// Enable indications (0x02, 0x00)
await cccd.WriteValueAsync(new byte[] { 0x02, 0x00 });

// Disable (0x00, 0x00)
await cccd.WriteValueAsync(new byte[] { 0x00, 0x00 });
```

**CCCD Values:**
- `0x00, 0x00`: Notifications and indications disabled
- `0x01, 0x00`: Notifications enabled
- `0x02, 0x00`: Indications enabled
- `0x03, 0x00`: Both enabled (rare)

### Characteristic User Description

Provides a human-readable text description:

```csharp
var descriptionUuid = Guid.Parse("00002901-0000-1000-8000-00805F9B34FB");
var descriptor = characteristic.GetDescriptorOrDefault(descriptionUuid);

if (descriptor != null && descriptor.CanRead)
{
    var value = await descriptor.ReadValueAsync();
    string description = Encoding.UTF8.GetString(value.ToArray());
    Console.WriteLine($"Characteristic description: {description}");
}
```

### Characteristic Presentation Format

Describes the format of the characteristic value:

```csharp
var formatUuid = Guid.Parse("00002904-0000-1000-8000-00805F9B34FB");
var descriptor = characteristic.GetDescriptorOrDefault(formatUuid);

if (descriptor != null && descriptor.CanRead)
{
    var value = await descriptor.ReadValueAsync();
    var span = value.Span;

    // Parse presentation format (7 bytes)
    byte format = span[0];      // Data format (e.g., uint8, uint16, float)
    byte exponent = span[1];    // Power of 10 exponent
    ushort unit = BitConverter.ToUInt16(span.Slice(2, 2).ToArray(), 0);  // Units
    byte namespaceId = span[4]; // Namespace (1 = Bluetooth SIG)
    ushort description = BitConverter.ToUInt16(span.Slice(5, 2).ToArray(), 0);

    Console.WriteLine($"Format: {format}, Exponent: {exponent}, Unit: {unit}");
}
```

## Reading Descriptors

### Basic Read

```csharp
var value = await descriptor.ReadValueAsync();

// Access as byte array
byte[] bytes = value.ToArray();

// Access as span
ReadOnlySpan<byte> span = value.Span;
```

### Skip Redundant Reads

```csharp
// First read: actually reads from device
var value1 = await descriptor.ReadValueAsync();

// Skip if already read
var value2 = await descriptor.ReadValueAsync(skipIfPreviouslyRead: true);
```

### Check Current State

```csharp
// Is currently performing a read?
bool isReading = descriptor.IsReadingValue;
```

## Writing Descriptors

### Basic Write

```csharp
byte[] data = { 0x01, 0x00 };
await descriptor.WriteValueAsync(data);
```

### Skip Redundant Writes

```csharp
// Only write if value has changed
await descriptor.WriteValueAsync(
    data,
    skipIfOldValueMatchesNewValue: true
);
```

### Check Current State

```csharp
// Is currently performing a write?
bool isWriting = descriptor.IsWritingValue;
```

## Events

Monitor value changes:

```csharp
descriptor.ValueUpdated += (s, args) =>
{
    Console.WriteLine($"Descriptor value changed: {BitConverter.ToString(args.Value.ToArray())}");
    Console.WriteLine($"Timestamp: {args.Timestamp}");
};
```

### Wait for Value Change

```csharp
// Wait for value to change
var value = await descriptor.WaitForValueChangeAsync();

// Wait for specific value
var value = await descriptor.WaitForValueChangeAsync(
    valueFilter: v => v.Span[0] == 0x01,
    timeout: TimeSpan.FromSeconds(5)
);
```

## Common Patterns

### Read All Descriptors

```csharp
async Task ReadAllDescriptorsAsync(IBluetoothRemoteCharacteristic characteristic)
{
    var descriptors = characteristic.GetDescriptors();

    Console.WriteLine($"Found {descriptors.Count} descriptors:");

    foreach (var descriptor in descriptors)
    {
        Console.WriteLine($"  {descriptor.Name} ({descriptor.Id})");

        if (descriptor.CanRead)
        {
            try
            {
                var value = await descriptor.ReadValueAsync();
                Console.WriteLine($"    Value: {BitConverter.ToString(value.ToArray())}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"    Error reading: {ex.Message}");
            }
        }
    }
}
```

### Manually Control CCCD

```csharp
async Task ManualCccdControlAsync(IBluetoothRemoteCharacteristic characteristic)
{
    var cccdUuid = Guid.Parse("00002902-0000-1000-8000-00805F9B34FB");
    var cccd = characteristic.GetDescriptorOrDefault(cccdUuid);

    if (cccd == null)
    {
        Console.WriteLine("CCCD not found - notifications not supported");
        return;
    }

    // Subscribe to characteristic value changes
    characteristic.ValueUpdated += (s, args) =>
    {
        Console.WriteLine($"Notification: {BitConverter.ToString(args.Value.ToArray())}");
    };

    // Enable notifications by writing to CCCD
    await cccd.WriteValueAsync(new byte[] { 0x01, 0x00 });
    Console.WriteLine("Notifications enabled");

    // Wait for notifications...
    await Task.Delay(TimeSpan.FromSeconds(10));

    // Disable notifications
    await cccd.WriteValueAsync(new byte[] { 0x00, 0x00 });
    Console.WriteLine("Notifications disabled");
}
```

### Read Characteristic Description

```csharp
async Task<string> GetCharacteristicDescriptionAsync(IBluetoothRemoteCharacteristic characteristic)
{
    var descriptionUuid = Guid.Parse("00002901-0000-1000-8000-00805F9B34FB");
    var descriptor = characteristic.GetDescriptorOrDefault(descriptionUuid);

    if (descriptor != null && descriptor.CanRead)
    {
        var value = await descriptor.ReadValueAsync();
        return Encoding.UTF8.GetString(value.ToArray());
    }

    return characteristic.Name;  // Fallback to characteristic name
}
```

## Best Practices

1. **Use High-Level APIs**: For notifications, use `StartListeningAsync()` instead of manually managing CCCD
   ```csharp
   // Recommended
   await characteristic.StartListeningAsync();

   // Not recommended (unless you have a specific need)
   var cccd = characteristic.GetDescriptor(cccdUuid);
   await cccd.WriteValueAsync(new byte[] { 0x01, 0x00 });
   ```

2. **Explore Descriptors Only When Needed**: Use `ExplorationDepth.Descriptors` only if you actually need descriptor access
   ```csharp
   // Only if you need descriptors
   await device.ExploreServicesAsync(ServiceExplorationOptions.Full);
   ```

3. **Check Capabilities**: Always check `CanRead`/`CanWrite`
   ```csharp
   if (descriptor.CanRead)
       await descriptor.ReadValueAsync();
   ```

4. **Handle Errors Gracefully**: Descriptor operations can fail
   ```csharp
   try
   {
       await descriptor.ReadValueAsync();
   }
   catch (DescriptorCantReadException)
   {
       Console.WriteLine("Descriptor doesn't support read");
   }
   catch (DeviceNotConnectedException)
   {
       Console.WriteLine("Device disconnected");
   }
   ```

5. **Use Timeouts**: Prevent operations from hanging
   ```csharp
   await descriptor.ReadValueAsync(
       timeout: TimeSpan.FromSeconds(5)
   );
   ```

## Troubleshooting

### Descriptor Not Found

- Ensure you used `ExplorationDepth.Descriptors` when exploring
- Not all characteristics have descriptors
- Check if the descriptor UUID is correct

### Read/Write Fails

- Check `CanRead`/`CanWrite` before attempting operations
- Ensure device is connected
- Some descriptors require authentication/pairing
- Verify correct descriptor UUID

### CCCD Issues

- Use the high-level `StartListeningAsync()` API instead of manual CCCD control
- If manual control is needed, ensure correct byte order (little-endian)
- Check that the characteristic actually supports notifications

### Performance

- Descriptors are rarely used in typical applications
- Only explore descriptors if you actually need them
- Reading descriptors adds overhead - cache results if needed

## When Do You Need Descriptors?

In most cases, you **don't** need to work with descriptors directly:

**You DON'T need descriptors for:**
- Reading characteristic values (use `ReadValueAsync()`)
- Writing characteristic values (use `WriteValueAsync()`)
- Receiving notifications (use `StartListeningAsync()` - CCCD is handled automatically)

**You MIGHT need descriptors for:**
- Reading user-friendly characteristic descriptions
- Understanding the data format and units
- Advanced custom protocol implementations
- Debugging and device exploration tools

## Related Topics

- [Characteristic](./Characteristic.md) - The parent of descriptors
- [Service](./Service.md) - Exploring services and characteristics
- [Device](./Device.md) - Connection and exploration
