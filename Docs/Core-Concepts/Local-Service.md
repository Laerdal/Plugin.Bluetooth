# Local Service

## Overview

A **Local Service** represents a service that your device hosts when acting as a BLE peripheral (GATT server). It's the server-side counterpart to a Remote Service - while Remote Services are on devices you connect to, Local Services are the ones you provide to other devices.

**Interface:** `IBluetoothLocalService`

## What Does It Do?

A Local Service allows you to:
- Host a collection of related characteristics
- Provide functionality to connected client devices
- Group characteristics into logical units
- Identify your service with a UUID (standard or custom)

## GATT Hierarchy (Server Side)

```
Broadcaster
  └── Local Service ◄── You are here
        └── Local Characteristic
```

## Getting Started

### 1. Create a Service Specification

```csharp
var serviceSpec = new IBluetoothLocalServiceFactory.BluetoothLocalServiceSpec
{
    ServiceId = Guid.Parse("0000180F-0000-1000-8000-00805F9B34FB"), // Battery Service
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
```

### 2. Create the Service

```csharp
var broadcaster = BluetoothFactory.Current.Broadcaster;
var service = await broadcaster.CreateServiceAsync(serviceSpec);

Console.WriteLine($"Created service: {service.Name}");
```

### 3. Access Characteristics

```csharp
// Get specific characteristic
var batteryLevelChar = service.GetCharacteristic(
    Guid.Parse("00002A19-0000-1000-8000-00805F9B34FB")
);

// Update characteristic value
await batteryLevelChar.UpdateValueAsync(
    new byte[] { 85 },  // 85% battery
    notifyClients: true
);
```

## Service Properties

### Basic Properties

```csharp
// Service UUID
Guid id = service.Id;

// Service name (human-readable)
string name = service.Name;  // e.g., "Battery Service"

// Is this a primary service?
bool isPrimary = service.IsPrimary;

// Parent broadcaster
IBluetoothBroadcaster broadcaster = service.Broadcaster;
```

### Example

```csharp
Console.WriteLine($"Service: {service.Name}");
Console.WriteLine($"UUID: {service.Id}");
Console.WriteLine($"Primary: {service.IsPrimary}");
```

## Primary vs Secondary Services

**Primary Service**:
- Main services advertised to clients
- Directly accessible by clients
- Most services are primary
- Example: Battery Service, Heart Rate Service

**Secondary Service**:
- Helper services used by other services
- Not directly advertised
- Referenced by primary services
- Rare in practice

```csharp
// Most services should be primary
IsPrimary = true
```

## Characteristic Management

### Get Characteristics

```csharp
// Get specific characteristic by UUID
var characteristic = service.GetCharacteristic(characteristicUuid);

// Get by filter
var writableChar = service.GetCharacteristic(c => c.Properties.HasFlag(
    BluetoothCharacteristicProperties.Write
));

// Get all characteristics
var allChars = service.GetCharacteristics();

// Safe retrieval
var characteristic = service.GetCharacteristicOrDefault(characteristicUuid);
if (characteristic != null)
{
    // Use characteristic
}

// Check if characteristic exists
bool hasChar = service.HasCharacteristic(characteristicUuid);
```

### Monitor Characteristic List Changes

```csharp
// Any change to characteristic list
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

## Common Patterns

### Battery Service

```csharp
async Task<IBluetoothLocalService> CreateBatteryServiceAsync()
{
    var broadcaster = BluetoothFactory.Current.Broadcaster;

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

    return await broadcaster.CreateServiceAsync(serviceSpec);
}

// Update battery level
async Task UpdateBatteryAsync(IBluetoothLocalService service, int level)
{
    var characteristic = service.GetCharacteristic(
        Guid.Parse("00002A19-0000-1000-8000-00805F9B34FB")
    );

    await characteristic.UpdateValueAsync(
        new byte[] { (byte)level },
        notifyClients: true
    );
}
```

### Device Information Service

```csharp
async Task<IBluetoothLocalService> CreateDeviceInfoServiceAsync()
{
    var serviceSpec = new IBluetoothLocalServiceFactory.BluetoothLocalServiceSpec
    {
        ServiceId = Guid.Parse("0000180A-0000-1000-8000-00805F9B34FB"),
        IsPrimary = true,
        Characteristics = new[]
        {
            // Manufacturer Name
            new IBluetoothLocalCharacteristicFactory.BluetoothLocalCharacteristicSpec
            {
                CharacteristicId = Guid.Parse("00002A29-0000-1000-8000-00805F9B34FB"),
                Properties = BluetoothCharacteristicProperties.Read,
                Permissions = BluetoothCharacteristicPermissions.Read,
                InitialValue = Encoding.UTF8.GetBytes("Acme Corp")
            },
            // Model Number
            new IBluetoothLocalCharacteristicFactory.BluetoothLocalCharacteristicSpec
            {
                CharacteristicId = Guid.Parse("00002A24-0000-1000-8000-00805F9B34FB"),
                Properties = BluetoothCharacteristicProperties.Read,
                Permissions = BluetoothCharacteristicPermissions.Read,
                InitialValue = Encoding.UTF8.GetBytes("Model X1")
            },
            // Firmware Revision
            new IBluetoothLocalCharacteristicFactory.BluetoothLocalCharacteristicSpec
            {
                CharacteristicId = Guid.Parse("00002A26-0000-1000-8000-00805F9B34FB"),
                Properties = BluetoothCharacteristicProperties.Read,
                Permissions = BluetoothCharacteristicPermissions.Read,
                InitialValue = Encoding.UTF8.GetBytes("1.0.0")
            }
        }
    };

    return await BluetoothFactory.Current.Broadcaster.CreateServiceAsync(serviceSpec);
}
```

### Custom Sensor Service

```csharp
async Task<IBluetoothLocalService> CreateCustomSensorServiceAsync()
{
    // Use a custom UUID for your proprietary service
    var serviceSpec = new IBluetoothLocalServiceFactory.BluetoothLocalServiceSpec
    {
        ServiceId = Guid.Parse("12345678-1234-1234-1234-123456789abc"),
        IsPrimary = true,
        Characteristics = new[]
        {
            // Temperature characteristic
            new IBluetoothLocalCharacteristicFactory.BluetoothLocalCharacteristicSpec
            {
                CharacteristicId = Guid.Parse("12345678-1234-1234-1234-123456789abd"),
                Properties = BluetoothCharacteristicProperties.Read |
                             BluetoothCharacteristicProperties.Notify,
                Permissions = BluetoothCharacteristicPermissions.Read,
                InitialValue = BitConverter.GetBytes(25.0f) // 25°C
            },
            // Humidity characteristic
            new IBluetoothLocalCharacteristicFactory.BluetoothLocalCharacteristicSpec
            {
                CharacteristicId = Guid.Parse("12345678-1234-1234-1234-123456789abe"),
                Properties = BluetoothCharacteristicProperties.Read |
                             BluetoothCharacteristicProperties.Notify,
                Permissions = BluetoothCharacteristicPermissions.Read,
                InitialValue = BitConverter.GetBytes(60.0f) // 60% humidity
            },
            // Configuration characteristic (writeable)
            new IBluetoothLocalCharacteristicFactory.BluetoothLocalCharacteristicSpec
            {
                CharacteristicId = Guid.Parse("12345678-1234-1234-1234-123456789abf"),
                Properties = BluetoothCharacteristicProperties.Read |
                             BluetoothCharacteristicProperties.Write,
                Permissions = BluetoothCharacteristicPermissions.Read |
                              BluetoothCharacteristicPermissions.Write,
                InitialValue = new byte[] { 0x01 } // Configuration byte
            }
        }
    };

    return await BluetoothFactory.Current.Broadcaster.CreateServiceAsync(serviceSpec);
}

// Update sensor readings
async Task UpdateSensorReadingsAsync(IBluetoothLocalService service)
{
    var tempChar = service.GetCharacteristic(
        Guid.Parse("12345678-1234-1234-1234-123456789abd")
    );
    var humidityChar = service.GetCharacteristic(
        Guid.Parse("12345678-1234-1234-1234-123456789abe")
    );

    while (true)
    {
        // Read from actual sensors (simulated here)
        float temperature = 20.0f + Random.Shared.NextSingle() * 10;
        float humidity = 50.0f + Random.Shared.NextSingle() * 20;

        // Update characteristics
        await tempChar.UpdateValueAsync(
            BitConverter.GetBytes(temperature),
            notifyClients: true
        );

        await humidityChar.UpdateValueAsync(
            BitConverter.GetBytes(humidity),
            notifyClients: true
        );

        await Task.Delay(TimeSpan.FromSeconds(5));
    }
}
```

### Service with Multiple Read/Write Characteristics

```csharp
async Task<IBluetoothLocalService> CreateControlServiceAsync()
{
    var serviceSpec = new IBluetoothLocalServiceFactory.BluetoothLocalServiceSpec
    {
        ServiceId = Guid.Parse("AAAAAAAA-BBBB-CCCC-DDDD-EEEEEEEEEEEE"),
        IsPrimary = true,
        Characteristics = new[]
        {
            // Status (read-only)
            new IBluetoothLocalCharacteristicFactory.BluetoothLocalCharacteristicSpec
            {
                CharacteristicId = Guid.Parse("AAAAAAAA-BBBB-CCCC-DDDD-EEEEEEEEEE01"),
                Properties = BluetoothCharacteristicProperties.Read,
                Permissions = BluetoothCharacteristicPermissions.Read,
                InitialValue = new byte[] { 0x00 } // OFF
            },
            // Control (read/write)
            new IBluetoothLocalCharacteristicFactory.BluetoothLocalCharacteristicSpec
            {
                CharacteristicId = Guid.Parse("AAAAAAAA-BBBB-CCCC-DDDD-EEEEEEEEEE02"),
                Properties = BluetoothCharacteristicProperties.Read |
                             BluetoothCharacteristicProperties.Write,
                Permissions = BluetoothCharacteristicPermissions.Read |
                              BluetoothCharacteristicPermissions.Write,
                InitialValue = new byte[] { 0x00 }
            }
        }
    };

    var service = await BluetoothFactory.Current.Broadcaster.CreateServiceAsync(serviceSpec);

    // Handle control characteristic writes
    var controlChar = service.GetCharacteristic(
        Guid.Parse("AAAAAAAA-BBBB-CCCC-DDDD-EEEEEEEEEE02")
    );

    controlChar.WriteRequestReceived += async (s, args) =>
    {
        var command = args.Value.Span[0];

        if (command == 0x01) // Turn ON
        {
            Console.WriteLine("Received ON command");

            // Update status characteristic
            var statusChar = service.GetCharacteristic(
                Guid.Parse("AAAAAAAA-BBBB-CCCC-DDDD-EEEEEEEEEE01")
            );
            await statusChar.UpdateValueAsync(new byte[] { 0x01 }, notifyClients: true);
        }
        else if (command == 0x00) // Turn OFF
        {
            Console.WriteLine("Received OFF command");
        }
    };

    return service;
}
```

## Best Practices

1. **Use Standard Services When Possible**: Prefer Bluetooth SIG standard service UUIDs
   ```csharp
   // Good: Standard Battery Service
   ServiceId = Guid.Parse("0000180F-0000-1000-8000-00805F9B34FB")

   // Use custom UUIDs only when necessary
   ServiceId = Guid.Parse("12345678-1234-1234-1234-123456789abc")
   ```

2. **Mark Primary Services as Primary**: Most services should be primary
   ```csharp
   IsPrimary = true
   ```

3. **Group Related Characteristics**: A service should contain logically related characteristics
   - Battery Service → Battery Level, Battery Status
   - Heart Rate Service → Heart Rate Measurement, Body Sensor Location
   - Custom Sensor Service → Temperature, Humidity, Pressure

4. **Provide Initial Values**: Set meaningful initial values for characteristics
   ```csharp
   InitialValue = new byte[] { 100 } // 100% battery
   ```

5. **Handle Lifecycle Properly**: Clean up when removing services
   ```csharp
   await broadcaster.RemoveServiceAsync(service);
   ```

## Troubleshooting

### Service Not Visible to Clients

- Ensure broadcaster is running: `broadcaster.IsRunning`
- Advertise the service UUID in `BroadcastingOptions`
- Check that service is marked as primary
- Verify client is looking for the correct UUID

### Characteristics Not Accessible

- Check characteristic properties and permissions
- Ensure characteristic was added to service spec
- Verify the characteristic UUID is correct

### Updates Not Received by Clients

- Check that characteristic has `Notify` or `Indicate` property
- Ensure clients are subscribed to notifications
- Use `notifyClients: true` when updating values

## Related Topics

- [Broadcaster](./Broadcaster.md) - Hosting services
- [Local Characteristic](./Local-Characteristic.md) - Providing data
- [Connected Device](./Connected-Device.md) - Managing clients
- [Service](./Service.md) - Client-side equivalent (Remote Service)
