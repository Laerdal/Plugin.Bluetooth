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

### 1. Create the Service

```csharp
// broadcaster is an IBluetoothBroadcaster obtained via constructor injection
// (see Docs/Configuration/Dependency-Injection.md).
var service = await broadcaster.CreateServiceAsync(
    id: Guid.Parse("0000180F-0000-1000-8000-00805F9B34FB"), // Battery Service
    isPrimary: true);

Console.WriteLine($"Created service: {service.Name}");
```

### 2. Add Characteristics

```csharp
var batteryLevelChar = await service.CreateCharacteristicAsync(
    id: Guid.Parse("00002A19-0000-1000-8000-00805F9B34FB"),
    properties: BluetoothCharacteristicProperties.Read | BluetoothCharacteristicProperties.Notify,
    permissions: BluetoothCharacteristicPermissions.Read);

// There's no "initial value" at creation time — set it explicitly afterwards.
await batteryLevelChar.UpdateValueAsync(new byte[] { 100 }, notifyClients: false);
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

### Characteristic List Changes

`IBluetoothLocalService` does not currently expose list-changed events for its hosted characteristics (unlike the broadcaster's `ServiceListChanged`/`ServicesAdded`/`ServicesRemoved` for services, or the remote-side `IBluetoothRemoteService.CharacteristicListChanged`). To track additions/removals yourself, call `CreateCharacteristicAsync`/`RemoveCharacteristicAsync` and react at the call site, or poll `GetCharacteristics()`.

## Common Patterns

### Battery Service

```csharp
// broadcaster is an IBluetoothBroadcaster obtained via constructor injection
// (see Docs/Configuration/Dependency-Injection.md).
async Task<IBluetoothLocalService> CreateBatteryServiceAsync(IBluetoothBroadcaster broadcaster)
{
    var service = await broadcaster.CreateServiceAsync(
        id: Guid.Parse("0000180F-0000-1000-8000-00805F9B34FB"),
        isPrimary: true);

    var batteryLevelChar = await service.CreateCharacteristicAsync(
        id: Guid.Parse("00002A19-0000-1000-8000-00805F9B34FB"),
        properties: BluetoothCharacteristicProperties.Read | BluetoothCharacteristicProperties.Notify,
        permissions: BluetoothCharacteristicPermissions.Read);
    await batteryLevelChar.UpdateValueAsync(new byte[] { 100 }, notifyClients: false);

    return service;
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
async Task<IBluetoothLocalService> CreateDeviceInfoServiceAsync(IBluetoothBroadcaster broadcaster)
{
    var service = await broadcaster.CreateServiceAsync(
        id: Guid.Parse("0000180A-0000-1000-8000-00805F9B34FB"),
        isPrimary: true);

    // Manufacturer Name
    var manufacturerChar = await service.CreateCharacteristicAsync(
        id: Guid.Parse("00002A29-0000-1000-8000-00805F9B34FB"),
        properties: BluetoothCharacteristicProperties.Read,
        permissions: BluetoothCharacteristicPermissions.Read);
    await manufacturerChar.UpdateValueAsync(Encoding.UTF8.GetBytes("Acme Corp"), notifyClients: false);

    // Model Number
    var modelChar = await service.CreateCharacteristicAsync(
        id: Guid.Parse("00002A24-0000-1000-8000-00805F9B34FB"),
        properties: BluetoothCharacteristicProperties.Read,
        permissions: BluetoothCharacteristicPermissions.Read);
    await modelChar.UpdateValueAsync(Encoding.UTF8.GetBytes("Model X1"), notifyClients: false);

    // Firmware Revision
    var firmwareChar = await service.CreateCharacteristicAsync(
        id: Guid.Parse("00002A26-0000-1000-8000-00805F9B34FB"),
        properties: BluetoothCharacteristicProperties.Read,
        permissions: BluetoothCharacteristicPermissions.Read);
    await firmwareChar.UpdateValueAsync(Encoding.UTF8.GetBytes("1.0.0"), notifyClients: false);

    return service;
}
```

### Custom Sensor Service

```csharp
async Task<IBluetoothLocalService> CreateCustomSensorServiceAsync(IBluetoothBroadcaster broadcaster)
{
    // Use a custom UUID for your proprietary service
    var service = await broadcaster.CreateServiceAsync(
        id: Guid.Parse("12345678-1234-1234-1234-123456789abc"),
        isPrimary: true);

    // Temperature characteristic
    var tempChar = await service.CreateCharacteristicAsync(
        id: Guid.Parse("12345678-1234-1234-1234-123456789abd"),
        properties: BluetoothCharacteristicProperties.Read | BluetoothCharacteristicProperties.Notify,
        permissions: BluetoothCharacteristicPermissions.Read);
    await tempChar.UpdateValueAsync(BitConverter.GetBytes(25.0f), notifyClients: false); // 25°C

    // Humidity characteristic
    var humidityChar = await service.CreateCharacteristicAsync(
        id: Guid.Parse("12345678-1234-1234-1234-123456789abe"),
        properties: BluetoothCharacteristicProperties.Read | BluetoothCharacteristicProperties.Notify,
        permissions: BluetoothCharacteristicPermissions.Read);
    await humidityChar.UpdateValueAsync(BitConverter.GetBytes(60.0f), notifyClients: false); // 60% humidity

    // Configuration characteristic (writeable)
    var configChar = await service.CreateCharacteristicAsync(
        id: Guid.Parse("12345678-1234-1234-1234-123456789abf"),
        properties: BluetoothCharacteristicProperties.Read | BluetoothCharacteristicProperties.Write,
        permissions: BluetoothCharacteristicPermissions.Read | BluetoothCharacteristicPermissions.Write);
    await configChar.UpdateValueAsync(new byte[] { 0x01 }, notifyClients: false); // Configuration byte

    return service;
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
async Task<IBluetoothLocalService> CreateControlServiceAsync(IBluetoothBroadcaster broadcaster)
{
    var service = await broadcaster.CreateServiceAsync(
        id: Guid.Parse("AAAAAAAA-BBBB-CCCC-DDDD-EEEEEEEEEEEE"),
        isPrimary: true);

    // Status (read-only)
    var statusChar = await service.CreateCharacteristicAsync(
        id: Guid.Parse("AAAAAAAA-BBBB-CCCC-DDDD-EEEEEEEEEE01"),
        properties: BluetoothCharacteristicProperties.Read,
        permissions: BluetoothCharacteristicPermissions.Read);
    await statusChar.UpdateValueAsync(new byte[] { 0x00 }, notifyClients: false); // OFF

    // Control (read/write)
    var controlChar = await service.CreateCharacteristicAsync(
        id: Guid.Parse("AAAAAAAA-BBBB-CCCC-DDDD-EEEEEEEEEE02"),
        properties: BluetoothCharacteristicProperties.Read | BluetoothCharacteristicProperties.Write,
        permissions: BluetoothCharacteristicPermissions.Read | BluetoothCharacteristicPermissions.Write);
    await controlChar.UpdateValueAsync(new byte[] { 0x00 }, notifyClients: false);

    // Handle control characteristic writes
    controlChar.WriteRequested += async (s, args) =>
    {
        var command = args.Value.Span[0];

        if (command == 0x01) // Turn ON
        {
            Console.WriteLine("Received ON command");
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
   id: Guid.Parse("0000180F-0000-1000-8000-00805F9B34FB")

   // Use custom UUIDs only when necessary
   id: Guid.Parse("12345678-1234-1234-1234-123456789abc")
   ```

2. **Mark Primary Services as Primary**: Most services should be primary
   ```csharp
   isPrimary: true
   ```

3. **Group Related Characteristics**: A service should contain logically related characteristics
   - Battery Service → Battery Level, Battery Status
   - Heart Rate Service → Heart Rate Measurement, Body Sensor Location
   - Custom Sensor Service → Temperature, Humidity, Pressure

4. **Set a Value Right After Creating a Characteristic**: `CreateCharacteristicAsync` doesn't take an initial value — call `UpdateValueAsync` immediately after
   ```csharp
   await characteristic.UpdateValueAsync(new byte[] { 100 }, notifyClients: false); // 100% battery
   ```

5. **Handle Lifecycle Properly**: Clean up when removing services
   ```csharp
   await broadcaster.RemoveServiceAsync(service);
   ```

## Troubleshooting

### Service Not Visible to Clients

- Ensure broadcaster is running: `broadcaster.IsRunning`
- Advertise the service UUID via `BroadcastingOptions.AdvertisedServiceUuids`
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
