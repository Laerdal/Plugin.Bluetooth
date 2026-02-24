# Characteristic

## Overview

A **Characteristic** is where the real data lives in BLE. It represents a single data value that you can read, write, or monitor for changes (notifications). Think of it as a variable on the remote device that you can interact with.

**Interface:** `IBluetoothRemoteCharacteristic`

## What Does It Do?

A Characteristic allows you to:
- **Read** values from the device (e.g., read current temperature)
- **Write** values to the device (e.g., turn on a light)
- **Listen** for value changes (e.g., get notified when heart rate changes)
- Check what operations are supported (CanRead, CanWrite, CanListen)
- Access the current cached value without reading from the device

## GATT Hierarchy Position

```
Device
  └── Service
        └── Characteristic ◄── You are here
              └── Descriptor
```

Characteristics are the primary way you interact with device data.

## Standard Characteristics

The Bluetooth SIG defines many standard characteristics. Here are common examples:

| Characteristic | UUID | Service | Type |
|----------------|------|---------|------|
| Battery Level | `00002A19-0000-1000-8000-00805F9B34FB` | Battery Service | uint8 (0-100%) |
| Heart Rate Measurement | `00002A37-0000-1000-8000-00805F9B34FB` | Heart Rate | Complex |
| Temperature | `00002A6E-0000-1000-8000-00805F9B34FB` | Environmental Sensing | uint16 |
| Device Name | `00002A00-0000-1000-8000-00805F9B34FB` | Generic Access | String |
| Manufacturer Name | `00002A29-0000-1000-8000-00805F9B34FB` | Device Information | String |

## Getting Started

### 1. Get a Characteristic

After exploring a service:

```csharp
await device.ConnectAsync();
await device.ExploreServicesAsync(ServiceExplorationOptions.WithCharacteristics);

var service = device.GetService(serviceUuid);
var characteristic = service.GetCharacteristic(characteristicUuid);

// Or in one step
var batteryLevelUuid = Guid.Parse("00002A19-0000-1000-8000-00805F9B34FB");
var characteristic = device
    .GetService(Guid.Parse("0000180F-0000-1000-8000-00805F9B34FB"))
    .GetCharacteristic(batteryLevelUuid);
```

### 2. Check Capabilities

Before performing operations, check what's supported:

```csharp
if (characteristic.CanRead)
    Console.WriteLine("Can read value");

if (characteristic.CanWrite)
    Console.WriteLine("Can write value");

if (characteristic.CanListen)
    Console.WriteLine("Can subscribe to notifications");
```

### 3. Read a Value

```csharp
if (characteristic.CanRead)
{
    var value = await characteristic.ReadValueAsync();

    // Access as byte array
    byte[] bytes = value.ToArray();

    // Example: Read battery level (single byte)
    int batteryLevel = value.Span[0];
    Console.WriteLine($"Battery: {batteryLevel}%");
}
```

### 4. Write a Value

```csharp
if (characteristic.CanWrite)
{
    byte[] data = { 0x01, 0x02, 0x03 };
    await characteristic.WriteValueAsync(data);
    Console.WriteLine("Value written!");
}
```

### 5. Listen for Notifications

```csharp
if (characteristic.CanListen)
{
    // Subscribe to value changes
    characteristic.ValueUpdated += (s, args) =>
    {
        var newValue = args.Value;
        Console.WriteLine($"New value: {BitConverter.ToString(newValue.ToArray())}");
    };

    // Start listening
    await characteristic.StartListeningAsync();

    // ... device will now send notifications ...

    // Stop listening when done
    await characteristic.StopListeningAsync();
}
```

## Reading Values

### Basic Read

```csharp
var value = await characteristic.ReadValueAsync();

// Access as ReadOnlyMemory<byte>
ReadOnlyMemory<byte> memory = value;

// Access as ReadOnlySpan<byte> (more efficient)
ReadOnlySpan<byte> span = value.Span;

// Convert to byte array
byte[] bytes = value.ToArray();
```

### Skip Redundant Reads

If you've already read the value:

```csharp
// First read: actually reads from device
var value1 = await characteristic.ReadValueAsync();

// Skip read if already read
var value2 = await characteristic.ReadValueAsync(skipIfPreviouslyRead: true);
// Returns cached value immediately if available
```

### Parse Common Data Types

```csharp
var value = await characteristic.ReadValueAsync();

// String (UTF-8)
string text = Encoding.UTF8.GetString(value.ToArray());

// Single byte (uint8)
byte singleByte = value.Span[0];

// 16-bit integer (little-endian)
short int16 = BitConverter.ToInt16(value.ToArray(), 0);

// 32-bit integer (little-endian)
int int32 = BitConverter.ToInt32(value.ToArray(), 0);

// Float
float floatValue = BitConverter.ToSingle(value.ToArray(), 0);
```

### Check Current State

```csharp
// Is currently performing a read?
bool isReading = characteristic.IsReading;
```

## Writing Values

### Basic Write

```csharp
byte[] data = { 0x01, 0xFF };
await characteristic.WriteValueAsync(data);
```

### Skip Redundant Writes

```csharp
// Only write if value has changed
await characteristic.WriteValueAsync(
    data,
    skipIfOldValueMatchesNewValue: true
);
```

### Write Common Data Types

```csharp
// String
byte[] stringData = Encoding.UTF8.GetBytes("Hello");
await characteristic.WriteValueAsync(stringData);

// Single byte
byte[] byteData = { 0x01 };
await characteristic.WriteValueAsync(byteData);

// 16-bit integer
byte[] int16Data = BitConverter.GetBytes((short)1234);
await characteristic.WriteValueAsync(int16Data);

// 32-bit integer
byte[] int32Data = BitConverter.GetBytes(123456);
await characteristic.WriteValueAsync(int32Data);

// Float
byte[] floatData = BitConverter.GetBytes(3.14f);
await characteristic.WriteValueAsync(floatData);
```

### Reliable Write

For critical writes, use reliable write transactions:

```csharp
// Begin transaction
await characteristic.BeginReliableWriteAsync();

try
{
    // Queue multiple writes
    await characteristic.WriteValueAsync(data1);
    await characteristic.WriteValueAsync(data2);
    await characteristic.WriteValueAsync(data3);

    // Commit all writes atomically
    await characteristic.ExecuteReliableWriteAsync();
    Console.WriteLine("All writes succeeded!");
}
catch
{
    // Abort transaction on error
    await characteristic.AbortReliableWriteAsync();
    Console.WriteLine("Transaction aborted");
    throw;
}
```

### Check Current State

```csharp
// Is currently performing a write?
bool isWriting = characteristic.IsWriting;
```

## Listening for Notifications

### Start Listening

```csharp
// Subscribe to value changes
characteristic.ValueUpdated += OnValueUpdated;

// Enable notifications
await characteristic.StartListeningAsync();

void OnValueUpdated(object sender, ValueUpdatedEventArgs args)
{
    Console.WriteLine($"New value: {BitConverter.ToString(args.Value.ToArray())}");
    Console.WriteLine($"Timestamp: {args.Timestamp}");
}
```

### Stop Listening

```csharp
await characteristic.StopListeningAsync();
characteristic.ValueUpdated -= OnValueUpdated;
```

### Wait for Specific Value

```csharp
// Wait for value to change
var value = await characteristic.WaitForValueChangeAsync();

// Wait for specific value using filter
var value = await characteristic.WaitForValueChangeAsync(
    valueFilter: v => v.Span[0] > 50,  // Wait until first byte > 50
    timeout: TimeSpan.FromSeconds(10)
);
```

### Check Current State

```csharp
// Is currently listening for notifications?
bool isListening = characteristic.IsListening;
```

## Value Properties

Access the current cached value without reading from the device:

```csharp
// As ReadOnlyMemory<byte>
ReadOnlyMemory<byte> value = characteristic.Value;

// As ReadOnlySpan<byte> (more efficient)
ReadOnlySpan<byte> span = characteristic.ValueSpan;
```

**Note**: These properties return the last known value. Call `ReadValueAsync()` to get the latest value from the device.

## Characteristic Properties

### Basic Properties

```csharp
// Characteristic UUID
Guid id = characteristic.Id;

// Characteristic name (human-readable)
string name = characteristic.Name;

// Parent service
IBluetoothRemoteService service = characteristic.RemoteService;

// Capability checks
bool canRead = characteristic.CanRead;
bool canWrite = characteristic.CanWrite;
bool canListen = characteristic.CanListen;
```

### Example

```csharp
Console.WriteLine($"Characteristic: {characteristic.Name}");
Console.WriteLine($"UUID: {characteristic.Id}");
Console.WriteLine($"Capabilities:");
Console.WriteLine($"  Read: {characteristic.CanRead}");
Console.WriteLine($"  Write: {characteristic.CanWrite}");
Console.WriteLine($"  Notify: {characteristic.CanListen}");
```

## Common Patterns

### Battery Level Monitor

```csharp
async Task MonitorBatteryLevelAsync(IBluetoothRemoteDevice device)
{
    var service = device.GetService(Guid.Parse("0000180F-0000-1000-8000-00805F9B34FB"));
    var characteristic = service.GetCharacteristic(Guid.Parse("00002A19-0000-1000-8000-00805F9B34FB"));

    // Read initial value
    var value = await characteristic.ReadValueAsync();
    Console.WriteLine($"Initial battery: {value.Span[0]}%");

    // Subscribe to changes (if supported)
    if (characteristic.CanListen)
    {
        characteristic.ValueUpdated += (s, args) =>
        {
            int batteryLevel = args.Value.Span[0];
            Console.WriteLine($"Battery level changed: {batteryLevel}%");

            if (batteryLevel < 20)
                Console.WriteLine("Warning: Low battery!");
        };

        await characteristic.StartListeningAsync();
        Console.WriteLine("Monitoring battery level...");
    }
}
```

### Temperature Sensor

```csharp
async Task ReadTemperatureAsync(IBluetoothRemoteCharacteristic tempCharacteristic)
{
    var value = await tempCharacteristic.ReadValueAsync();

    // Parse temperature (16-bit signed integer in 0.01°C units)
    short rawTemp = BitConverter.ToInt16(value.ToArray(), 0);
    float temperature = rawTemp * 0.01f;

    Console.WriteLine($"Temperature: {temperature:F2}°C");
}
```

### Control LED

```csharp
async Task ControlLedAsync(IBluetoothRemoteCharacteristic ledCharacteristic)
{
    // Turn LED on (0x01) or off (0x00)
    async Task SetLedAsync(bool on)
    {
        byte[] data = { (byte)(on ? 0x01 : 0x00) };
        await ledCharacteristic.WriteValueAsync(data);
        Console.WriteLine($"LED turned {(on ? "on" : "off")}");
    }

    await SetLedAsync(true);   // Turn on
    await Task.Delay(1000);
    await SetLedAsync(false);  // Turn off
}
```

### Heart Rate Monitor

```csharp
async Task MonitorHeartRateAsync(IBluetoothRemoteDevice device)
{
    var service = device.GetService(Guid.Parse("0000180D-0000-1000-8000-00805F9B34FB"));
    var characteristic = service.GetCharacteristic(Guid.Parse("00002A37-0000-1000-8000-00805F9B34FB"));

    characteristic.ValueUpdated += (s, args) =>
    {
        var data = args.Value.Span;

        // Parse heart rate measurement (complex format)
        byte flags = data[0];
        bool is16Bit = (flags & 0x01) != 0;

        int heartRate = is16Bit
            ? BitConverter.ToUInt16(data.Slice(1, 2).ToArray(), 0)
            : data[1];

        Console.WriteLine($"Heart Rate: {heartRate} bpm");
    };

    await characteristic.StartListeningAsync();
    Console.WriteLine("Monitoring heart rate...");
}
```

### Read Device Information

```csharp
async Task ReadDeviceInfoAsync(IBluetoothRemoteDevice device)
{
    var service = device.GetService(Guid.Parse("0000180A-0000-1000-8000-00805F9B34FB"));
    await service.ExploreCharacteristicsAsync();

    // Helper to read string characteristic
    async Task<string> ReadStringAsync(Guid uuid)
    {
        var char = service.GetCharacteristicOrDefault(uuid);
        if (char != null && char.CanRead)
        {
            var value = await char.ReadValueAsync();
            return Encoding.UTF8.GetString(value.ToArray());
        }
        return "N/A";
    }

    var manufacturer = await ReadStringAsync(Guid.Parse("00002A29-0000-1000-8000-00805F9B34FB"));
    var model = await ReadStringAsync(Guid.Parse("00002A24-0000-1000-8000-00805F9B34FB"));
    var serial = await ReadStringAsync(Guid.Parse("00002A25-0000-1000-8000-00805F9B34FB"));
    var firmware = await ReadStringAsync(Guid.Parse("00002A26-0000-1000-8000-00805F9B34FB"));
    var hardware = await ReadStringAsync(Guid.Parse("00002A27-0000-1000-8000-00805F9B34FB"));

    Console.WriteLine($"Manufacturer: {manufacturer}");
    Console.WriteLine($"Model: {model}");
    Console.WriteLine($"Serial: {serial}");
    Console.WriteLine($"Firmware: {firmware}");
    Console.WriteLine($"Hardware: {hardware}");
}
```

## Best Practices

1. **Always Check Capabilities**: Before reading/writing, check if supported
   ```csharp
   if (!characteristic.CanRead)
       throw new InvalidOperationException("Characteristic doesn't support read");
   ```

2. **Unsubscribe from Events**: Always clean up event handlers
   ```csharp
   try
   {
       characteristic.ValueUpdated += handler;
       await characteristic.StartListeningAsync();
       // Use notifications...
   }
   finally
   {
       await characteristic.StopListeningAsync();
       characteristic.ValueUpdated -= handler;
   }
   ```

3. **Handle Disconnections**: Operations fail if device disconnects
   ```csharp
   try
   {
       await characteristic.ReadValueAsync();
   }
   catch (DeviceNotConnectedException)
   {
       Console.WriteLine("Device disconnected");
   }
   ```

4. **Use Timeouts**: Prevent operations from hanging
   ```csharp
   await characteristic.ReadValueAsync(
       timeout: TimeSpan.FromSeconds(5)
   );
   ```

5. **Parse Data Correctly**: Understand the characteristic's data format
   - Check the Bluetooth specification for standard characteristics
   - Consult device documentation for custom characteristics

6. **Optimize Notifications**: Only listen when needed to save battery
   ```csharp
   // Start listening
   await characteristic.StartListeningAsync();

   // Do work...

   // Stop when done
   await characteristic.StopListeningAsync();
   ```

## Troubleshooting

### Read/Write Fails

- Check `CanRead`/`CanWrite` before attempting operations
- Ensure device is connected: `service.Device.IsConnected`
- Some characteristics require authentication/pairing first
- Verify you have the correct characteristic UUID

### Notifications Not Received

- Check `CanListen` is true
- Ensure you called `StartListeningAsync()`
- Verify the device is actually sending notifications
- Some characteristics need to be configured via descriptors first

### Wrong Data Received

- Check byte order (little-endian vs big-endian)
- Verify data format from device documentation
- For standard characteristics, consult Bluetooth SIG specifications
- Use a BLE sniffer tool to inspect raw data

### Operation Times Out

- Increase timeout: `ReadValueAsync(timeout: TimeSpan.FromSeconds(10))`
- Check signal strength (weak signal = slow operations)
- Some devices are just slow - increase timeout appropriately
- Verify device is responsive

### Value Property is Empty

- Call `ReadValueAsync()` first to populate the cache
- Or start listening to receive values: `StartListeningAsync()`

## Related Topics

- [Service](./Service.md) - Organizing characteristics
- [Descriptor](./Descriptor.md) - Characteristic configuration
- [Device](./Device.md) - Connection management
