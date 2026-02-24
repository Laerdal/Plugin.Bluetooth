# Local Characteristic

## Overview

A **Local Characteristic** is a data value that your device provides when acting as a BLE peripheral (GATT server). It's the server-side counterpart to a Remote Characteristic - while Remote Characteristics are on devices you connect to, Local Characteristics are the ones you host for other devices to read from, write to, or receive notifications from.

**Interface:** `IBluetoothLocalCharacteristic`

## What Does It Do?

A Local Characteristic allows you to:
- Provide data values to connected client devices
- Accept write requests from clients
- Send notifications/indications when values change
- Configure read/write/notify permissions
- Track which clients are subscribed to notifications

## GATT Hierarchy (Server Side)

```
Broadcaster
  └── Local Service
        └── Local Characteristic ◄── You are here
```

## Getting Started

### 1. Define a Characteristic Spec

```csharp
var characteristicSpec = new IBluetoothLocalCharacteristicFactory.BluetoothLocalCharacteristicSpec
{
    CharacteristicId = Guid.Parse("00002A19-0000-1000-8000-00805F9B34FB"),
    Properties = BluetoothCharacteristicProperties.Read |
                 BluetoothCharacteristicProperties.Notify,
    Permissions = BluetoothCharacteristicPermissions.Read,
    InitialValue = new byte[] { 100 } // 100% battery
};
```

### 2. Create Service with Characteristic

```csharp
var serviceSpec = new IBluetoothLocalServiceFactory.BluetoothLocalServiceSpec
{
    ServiceId = Guid.Parse("0000180F-0000-1000-8000-00805F9B34FB"),
    IsPrimary = true,
    Characteristics = new[] { characteristicSpec }
};

var service = await broadcaster.CreateServiceAsync(serviceSpec);
var characteristic = service.GetCharacteristic(characteristicSpec.CharacteristicId);
```

### 3. Update the Value

```csharp
// Update value and notify subscribed clients
await characteristic.UpdateValueAsync(
    new byte[] { 85 },  // New value
    notifyClients: true
);
```

### 4. Handle Write Requests (if writable)

```csharp
characteristic.WriteRequestReceived += (sender, args) =>
{
    byte[] receivedData = args.Value.ToArray();
    Console.WriteLine($"Client wrote: {BitConverter.ToString(receivedData)}");

    // Process the write...
};
```

## Characteristic Properties

### Basic Properties

```csharp
// Characteristic UUID
Guid id = characteristic.Id;

// Characteristic name (human-readable)
string name = characteristic.Name;

// Parent service
IBluetoothLocalService service = characteristic.LocalService;

// GATT properties (what operations are allowed)
BluetoothCharacteristicProperties properties = characteristic.Properties;

// Permissions (security requirements)
BluetoothCharacteristicPermissions permissions = characteristic.Permissions;
```

### Value Properties

```csharp
// Current value as ReadOnlyMemory<byte>
ReadOnlyMemory<byte> value = characteristic.Value;

// Current value as ReadOnlySpan<byte> (more efficient)
ReadOnlySpan<byte> span = characteristic.ValueSpan;
```

### Example

```csharp
Console.WriteLine($"Characteristic: {characteristic.Name}");
Console.WriteLine($"UUID: {characteristic.Id}");
Console.WriteLine($"Properties: {characteristic.Properties}");
Console.WriteLine($"Permissions: {characteristic.Permissions}");
Console.WriteLine($"Current Value: {BitConverter.ToString(characteristic.Value.ToArray())}");
```

## Properties and Permissions

### Properties

Properties define what operations clients can perform:

```csharp
BluetoothCharacteristicProperties.Read       // Clients can read the value
BluetoothCharacteristicProperties.Write      // Clients can write (with response)
BluetoothCharacteristicProperties.WriteWithoutResponse  // Clients can write (no response)
BluetoothCharacteristicProperties.Notify     // Send notifications (no acknowledgment)
BluetoothCharacteristicProperties.Indicate   // Send indications (with acknowledgment)
BluetoothCharacteristicProperties.SignedWrite // Write with signature
BluetoothCharacteristicProperties.ExtendedProperties // Has extended properties
```

**Common Combinations:**
```csharp
// Read-only sensor value with notifications
Properties = BluetoothCharacteristicProperties.Read |
             BluetoothCharacteristicProperties.Notify

// Read/Write configuration
Properties = BluetoothCharacteristicProperties.Read |
             BluetoothCharacteristicProperties.Write

// Write-only command
Properties = BluetoothCharacteristicProperties.Write
```

### Permissions

Permissions define security requirements:

```csharp
BluetoothCharacteristicPermissions.Read              // Allow read
BluetoothCharacteristicPermissions.ReadEncrypted     // Require encryption to read
BluetoothCharacteristicPermissions.Write             // Allow write
BluetoothCharacteristicPermissions.WriteEncrypted    // Require encryption to write
BluetoothCharacteristicPermissions.ReadEncryptedMitm // Require authenticated encryption
BluetoothCharacteristicPermissions.WriteEncryptedMitm // Require authenticated encryption
```

**Common Combinations:**
```csharp
// Public read, anyone can read
Permissions = BluetoothCharacteristicPermissions.Read

// Secure read/write (requires pairing)
Permissions = BluetoothCharacteristicPermissions.ReadEncrypted |
              BluetoothCharacteristicPermissions.WriteEncrypted
```

## Updating Values

### Basic Update

```csharp
byte[] newValue = { 0x01, 0x02, 0x03 };

// Update without notifying clients
await characteristic.UpdateValueAsync(newValue, notifyClients: false);

// Update and notify subscribed clients
await characteristic.UpdateValueAsync(newValue, notifyClients: true);
```

### Update with Type Conversion

```csharp
// Integer
int temperature = 25;
await characteristic.UpdateValueAsync(
    BitConverter.GetBytes(temperature),
    notifyClients: true
);

// Float
float humidity = 60.5f;
await characteristic.UpdateValueAsync(
    BitConverter.GetBytes(humidity),
    notifyClients: true
);

// String
string deviceName = "MySensor";
await characteristic.UpdateValueAsync(
    Encoding.UTF8.GetBytes(deviceName),
    notifyClients: true
);

// Single byte
byte batteryLevel = 85;
await characteristic.UpdateValueAsync(
    new byte[] { batteryLevel },
    notifyClients: true
);
```

## Handling Client Writes

If your characteristic has `Write` or `WriteWithoutResponse` properties, handle write requests:

```csharp
characteristic.WriteRequestReceived += (sender, args) =>
{
    // Access the written value
    ReadOnlyMemory<byte> value = args.Value;
    IBluetoothConnectedDevice client = args.Device;

    Console.WriteLine($"Client {client.Id} wrote: {BitConverter.ToString(value.ToArray())}");

    // Process the write
    ProcessCommand(value.Span[0]);
};

void ProcessCommand(byte command)
{
    switch (command)
    {
        case 0x01:
            Console.WriteLine("Turn ON");
            break;
        case 0x00:
            Console.WriteLine("Turn OFF");
            break;
        default:
            Console.WriteLine($"Unknown command: {command}");
            break;
    }
}
```

## Managing Subscriptions

Track which clients are subscribed to notifications:

```csharp
// Get list of subscribed clients
IReadOnlyList<IBluetoothConnectedDevice> subscribedDevices = characteristic.SubscribedDevices;

// Monitor subscription changes
characteristic.ClientSubscribed += (sender, args) =>
{
    var client = args.Device;
    Console.WriteLine($"Client {client.Id} subscribed to notifications");
};

characteristic.ClientUnsubscribed += (sender, args) =>
{
    var client = args.Device;
    Console.WriteLine($"Client {client.Id} unsubscribed");
};

// Only notify if clients are subscribed
if (characteristic.SubscribedDevices.Count > 0)
{
    await characteristic.UpdateValueAsync(newValue, notifyClients: true);
}
```

## Common Patterns

### Battery Level Characteristic

```csharp
async Task<IBluetoothLocalCharacteristic> CreateBatteryCharacteristicAsync(
    IBluetoothLocalService service)
{
    var characteristic = service.GetCharacteristic(
        Guid.Parse("00002A19-0000-1000-8000-00805F9B34FB")
    );

    // Update battery level periodically
    _ = Task.Run(async () =>
    {
        while (true)
        {
            int batteryLevel = GetBatteryLevel(); // Your battery reading logic
            await characteristic.UpdateValueAsync(
                new byte[] { (byte)batteryLevel },
                notifyClients: true
            );

            await Task.Delay(TimeSpan.FromMinutes(1));
        }
    });

    return characteristic;
}

int GetBatteryLevel()
{
    // Simulate battery level
    return Random.Shared.Next(20, 100);
}
```

### Temperature Sensor

```csharp
async Task<IBluetoothLocalCharacteristic> CreateTemperatureCharacteristicAsync(
    IBluetoothLocalService service)
{
    var characteristic = service.GetCharacteristic(temperatureCharUuid);

    // Update temperature reading every 5 seconds
    _ = Task.Run(async () =>
    {
        while (true)
        {
            float temperature = ReadTemperatureSensor();

            // Convert to bytes (IEEE-754 format)
            byte[] tempBytes = BitConverter.GetBytes(temperature);

            // Only notify if clients are subscribed
            if (characteristic.SubscribedDevices.Count > 0)
            {
                await characteristic.UpdateValueAsync(tempBytes, notifyClients: true);
            }

            await Task.Delay(TimeSpan.FromSeconds(5));
        }
    });

    return characteristic;
}

float ReadTemperatureSensor()
{
    // Simulate temperature reading
    return 20.0f + Random.Shared.NextSingle() * 10;
}
```

### Writable Control Characteristic

```csharp
async Task<IBluetoothLocalCharacteristic> CreateControlCharacteristicAsync(
    IBluetoothLocalService service)
{
    var characteristic = service.GetCharacteristic(controlCharUuid);

    // Handle write requests
    characteristic.WriteRequestReceived += async (sender, args) =>
    {
        byte command = args.Value.Span[0];

        switch (command)
        {
            case 0x01: // Turn ON
                await TurnDeviceOnAsync();
                break;

            case 0x00: // Turn OFF
                await TurnDeviceOffAsync();
                break;

            case 0x02: // Toggle
                await ToggleDeviceAsync();
                break;

            default:
                Console.WriteLine($"Unknown command: 0x{command:X2}");
                break;
        }

        // Optionally update a status characteristic
        var statusChar = service.GetCharacteristic(statusCharUuid);
        await statusChar.UpdateValueAsync(
            new byte[] { command },
            notifyClients: true
        );
    };

    return characteristic;
}
```

### Data Logger

```csharp
async Task<IBluetoothLocalCharacteristic> CreateDataLoggerCharacteristicAsync(
    IBluetoothLocalService service)
{
    var characteristic = service.GetCharacteristic(dataLoggerCharUuid);
    var dataQueue = new Queue<byte[]>();

    // Collect data continuously
    _ = Task.Run(async () =>
    {
        while (true)
        {
            byte[] data = CollectSensorData();
            dataQueue.Enqueue(data);

            // Keep only last 100 readings
            while (dataQueue.Count > 100)
                dataQueue.Dequeue();

            await Task.Delay(TimeSpan.FromSeconds(1));
        }
    });

    // Handle read requests - send latest data
    characteristic.ReadRequestReceived += (sender, args) =>
    {
        if (dataQueue.Count > 0)
        {
            byte[] latestData = dataQueue.Peek();
            // Library handles sending the current value automatically
            Console.WriteLine($"Client read latest data: {BitConverter.ToString(latestData)}");
        }
    };

    return characteristic;
}

byte[] CollectSensorData()
{
    // Simulate sensor data collection
    return new byte[]
    {
        (byte)Random.Shared.Next(256),
        (byte)Random.Shared.Next(256),
        (byte)Random.Shared.Next(256)
    };
}
```

### Multi-Value Characteristic

```csharp
async Task UpdateMultiValueCharacteristicAsync(IBluetoothLocalCharacteristic characteristic)
{
    // Combine multiple sensor values into one characteristic
    var data = new List<byte>();

    // Temperature (2 bytes, int16)
    short temp = (short)(25.5 * 10); // 25.5°C * 10
    data.AddRange(BitConverter.GetBytes(temp));

    // Humidity (2 bytes, uint16)
    ushort humidity = (ushort)(60.5 * 10); // 60.5% * 10
    data.AddRange(BitConverter.GetBytes(humidity));

    // Pressure (4 bytes, uint32)
    uint pressure = 101325; // 101.325 kPa in Pascals
    data.AddRange(BitConverter.GetBytes(pressure));

    await characteristic.UpdateValueAsync(
        data.ToArray(),
        notifyClients: true
    );
}
```

## Events

Monitor characteristic interactions:

```csharp
// Client subscribed to notifications
characteristic.ClientSubscribed += (s, args) =>
{
    Console.WriteLine($"Client {args.Device.Id} subscribed");
};

// Client unsubscribed
characteristic.ClientUnsubscribed += (s, args) =>
{
    Console.WriteLine($"Client {args.Device.Id} unsubscribed");
};

// Client wrote to characteristic
characteristic.WriteRequestReceived += (s, args) =>
{
    Console.WriteLine($"Client {args.Device.Id} wrote {args.Value.Length} bytes");
};

// Client read from characteristic (if supported)
characteristic.ReadRequestReceived += (s, args) =>
{
    Console.WriteLine($"Client {args.Device.Id} read characteristic");
};
```

## Best Practices

1. **Choose Appropriate Properties**: Match properties to your use case
   ```csharp
   // Sensor reading: Read + Notify
   Properties = BluetoothCharacteristicProperties.Read |
                BluetoothCharacteristicProperties.Notify

   // Configuration: Read + Write
   Properties = BluetoothCharacteristicProperties.Read |
                BluetoothCharacteristicProperties.Write

   // Command: Write only
   Properties = BluetoothCharacteristicProperties.Write
   ```

2. **Set Meaningful Initial Values**: Provide valid initial data
   ```csharp
   InitialValue = new byte[] { 0 } // Valid initial state
   ```

3. **Check for Subscribers Before Notifying**: Save battery by not notifying when no one is listening
   ```csharp
   if (characteristic.SubscribedDevices.Count > 0)
       await characteristic.UpdateValueAsync(value, notifyClients: true);
   ```

4. **Handle Writes Promptly**: Process write requests quickly
   ```csharp
   characteristic.WriteRequestReceived += async (s, args) =>
   {
       // Process quickly
       ProcessCommand(args.Value);

       // Don't block the event handler with long operations
   };
   ```

5. **Use Appropriate Notify vs Indicate**:
   - **Notify**: Faster, no acknowledgment (good for sensor readings)
   - **Indicate**: Slower, acknowledgment required (good for critical data)

6. **Document Data Format**: Clearly document the byte format of your characteristic values

## Troubleshooting

### Clients Can't Read/Write

- Check `Properties` includes appropriate flags
- Verify `Permissions` allow the operation
- Ensure characteristic was added to service correctly

### Notifications Not Received

- Check `Properties` includes `Notify` or `Indicate`
- Verify clients are subscribed: `characteristic.SubscribedDevices`
- Ensure `notifyClients: true` when updating

### Write Requests Not Received

- Verify `WriteRequestReceived` event handler is attached
- Check `Properties` includes `Write` or `WriteWithoutResponse`
- Ensure `Permissions` allow writes

### Value Not Updated

- Call `UpdateValueAsync()` to change the value
- The `Value` property reflects the last set value

## Related Topics

- [Local Service](./Local-Service.md) - Hosting characteristics
- [Connected Device](./Connected-Device.md) - Managing client connections
- [Broadcaster](./Broadcaster.md) - Managing the GATT server
- [Characteristic](./Characteristic.md) - Client-side equivalent (Remote Characteristic)
