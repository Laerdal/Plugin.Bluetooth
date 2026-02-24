# Exploration Options

Exploration options control how Bluetooth GATT (Generic Attribute Profile) services, characteristics, and descriptors are discovered on remote devices. These options determine the depth of exploration and caching behavior during device exploration.

## Table of Contents

- [Overview](#overview)
- [GATT Hierarchy](#gatt-hierarchy)
- [Exploration Depth](#exploration-depth)
- [Service Exploration Options](#service-exploration-options)
- [Characteristic Exploration Options](#characteristic-exploration-options)
- [Descriptor Exploration Options](#descriptor-exploration-options)
- [Usage Examples](#usage-examples)
- [Best Practices](#best-practices)

---

## Overview

GATT exploration discovers the structure of services, characteristics, and descriptors on a connected Bluetooth device. Exploration options control:

- **Depth**: How deep to explore (services only, characteristics, descriptors)
- **Caching**: Whether to skip exploration if already discovered
- **Filtering**: Which UUIDs to explore
- **Automatic cascading**: Whether to automatically explore child elements

**Namespace**: `Bluetooth.Abstractions.Scanning.Options`

**Usage Pattern**: Passed to exploration method calls (not DI-configured)

---

## GATT Hierarchy

Understanding the GATT hierarchy is essential for using exploration options:

```
Device
├── Service (e.g., Heart Rate Service)
│   ├── Characteristic (e.g., Heart Rate Measurement)
│   │   ├── Descriptor (e.g., Client Characteristic Configuration)
│   │   └── Descriptor (e.g., Characteristic User Description)
│   └── Characteristic (e.g., Body Sensor Location)
│       └── Descriptor
└── Service (e.g., Battery Service)
    └── Characteristic (e.g., Battery Level)
        └── Descriptor
```

**Exploration Levels**:
1. **Services**: Top-level containers (e.g., Heart Rate, Battery)
2. **Characteristics**: Data endpoints within services (e.g., Heart Rate Measurement, Battery Level)
3. **Descriptors**: Metadata for characteristics (e.g., CCCD for notifications, user descriptions)

---

## Exploration Depth

The `ExplorationDepth` enum controls how deeply the exploration traverses the GATT hierarchy.

```csharp
public enum ExplorationDepth
{
    ServicesOnly = 0,     // Discover only services
    Characteristics = 1,  // Discover services and characteristics
    Descriptors = 2       // Discover services, characteristics, and descriptors (full)
}
```

### ServicesOnly (0)

Discovers only services, no characteristics or descriptors.

**Use Cases**:
- Quick service discovery
- Checking if device supports specific services
- Initial exploration before detailed discovery

**Example**:
```csharp
var options = new ServiceExplorationOptions
{
    Depth = ExplorationDepth.ServicesOnly
};
await device.ExploreServicesAsync(options);

// Now device.Services is populated, but no characteristics
foreach (var service in device.Services)
{
    Console.WriteLine($"Service: {service.Uuid}");
    // service.Characteristics will be empty
}
```

### Characteristics (1)

Discovers services and their characteristics, but not descriptors.

**Use Cases**:
- Most common exploration level
- Sufficient for reading/writing characteristic values
- Descriptors often not needed

**Example**:
```csharp
var options = new ServiceExplorationOptions
{
    Depth = ExplorationDepth.Characteristics
};
await device.ExploreServicesAsync(options);

// Services and characteristics populated
foreach (var service in device.Services)
{
    Console.WriteLine($"Service: {service.Uuid}");
    foreach (var characteristic in service.Characteristics)
    {
        Console.WriteLine($"  Characteristic: {characteristic.Uuid}");
        // characteristic.Descriptors will be empty
    }
}
```

### Descriptors (2)

Full exploration: services, characteristics, and descriptors.

**Use Cases**:
- Complete device profile discovery
- Enabling notifications (requires CCCD descriptor)
- Reading characteristic user descriptions
- Comprehensive device analysis

**Example**:
```csharp
var options = new ServiceExplorationOptions
{
    Depth = ExplorationDepth.Descriptors
};
await device.ExploreServicesAsync(options);

// Full hierarchy populated
foreach (var service in device.Services)
{
    Console.WriteLine($"Service: {service.Uuid}");
    foreach (var characteristic in service.Characteristics)
    {
        Console.WriteLine($"  Characteristic: {characteristic.Uuid}");
        foreach (var descriptor in characteristic.Descriptors)
        {
            Console.WriteLine($"    Descriptor: {descriptor.Uuid}");
        }
    }
}
```

---

## Service Exploration Options

`ServiceExplorationOptions` controls discovery of services on a connected device.

### Properties

```csharp
public record ServiceExplorationOptions
{
    public bool ExploreCharacteristics { get; init; }
    public bool ExploreDescriptors { get; init; }
    public bool UseCache { get; init; } = true;
    public ExplorationDepth Depth { get; init; }
    public Func<Guid, bool>? ServiceUuidFilter { get; init; }
}
```

#### ExploreCharacteristics

```csharp
public bool ExploreCharacteristics { get; init; }
```

**Default**: `false`

When enabled, automatically explores characteristics after discovering services.

**Behavior**:
- `true`: Calls `ExploreCharacteristicsAsync()` on each discovered service
- `false`: Only services discovered, characteristics must be explored manually

**Example**:
```csharp
var options = new ServiceExplorationOptions
{
    ExploreCharacteristics = true
};
await device.ExploreServicesAsync(options);

// Characteristics are now available
var heartRateService = device.Services.FirstOrDefault(s => s.Uuid == HeartRateServiceUuid);
var heartRateChar = heartRateService?.Characteristics.FirstOrDefault(c => c.Uuid == HeartRateMeasurementUuid);
```

---

#### ExploreDescriptors

```csharp
public bool ExploreDescriptors { get; init; }
```

**Default**: `false`

When enabled, automatically explores descriptors after discovering characteristics.

**Requirement**: Only applicable when `ExploreCharacteristics` is also `true`.

**Behavior**:
- `true`: Calls `ExploreDescriptorsAsync()` on each discovered characteristic
- `false`: Only characteristics discovered, descriptors must be explored manually

**Example**:
```csharp
var options = new ServiceExplorationOptions
{
    ExploreCharacteristics = true,
    ExploreDescriptors = true // Full depth
};
await device.ExploreServicesAsync(options);

// Full hierarchy available
var cccd = heartRateChar?.Descriptors
    .FirstOrDefault(d => d.Uuid == ClientCharacteristicConfigurationUuid);
```

---

#### UseCache

```csharp
public bool UseCache { get; init; } = true;
```

**Default**: `true`

When enabled, skips exploration if services have already been discovered.

**Behavior**:
- `true`: If `device.Services.Count > 0`, exploration is skipped
- `false`: Always performs exploration, even if already discovered

**Use Cases**:
- **Enable** (default): Avoid redundant exploration, better performance
- **Disable**: Force re-exploration (device profile may have changed)

**Example**:
```csharp
// First exploration
var options = new ServiceExplorationOptions { Depth = ExplorationDepth.Characteristics };
await device.ExploreServicesAsync(options); // Explores

// Second call (cached)
await device.ExploreServicesAsync(options); // Skipped (cached)

// Force re-exploration
options = options with { UseCache = false };
await device.ExploreServicesAsync(options); // Explores again
```

---

#### Depth

```csharp
public ExplorationDepth Depth { get; init; }
```

**Default**: `ExplorationDepth.ServicesOnly`

Convenient property to set exploration depth. Automatically sets `ExploreCharacteristics` and `ExploreDescriptors`.

**Mapping**:
- `ServicesOnly`: `ExploreCharacteristics = false`, `ExploreDescriptors = false`
- `Characteristics`: `ExploreCharacteristics = true`, `ExploreDescriptors = false`
- `Descriptors`: `ExploreCharacteristics = true`, `ExploreDescriptors = true`

**Example**:
```csharp
// Using Depth property (recommended)
var options = new ServiceExplorationOptions
{
    Depth = ExplorationDepth.Characteristics
};

// Equivalent to:
var options = new ServiceExplorationOptions
{
    ExploreCharacteristics = true,
    ExploreDescriptors = false
};
```

---

#### ServiceUuidFilter

```csharp
public Func<Guid, bool>? ServiceUuidFilter { get; init; }
```

**Default**: `null` (explore all services)

Filters which services to explore. Only services matching the filter are explored.

**Use Cases**:
- Explore only specific services of interest
- Skip standard services (e.g., Generic Access, Generic Attribute)
- Improve performance by limiting exploration

**Example**:
```csharp
// Explore only Heart Rate service
var options = new ServiceExplorationOptions
{
    Depth = ExplorationDepth.Characteristics,
    ServiceUuidFilter = uuid => uuid == HeartRateServiceUuid
};
await device.ExploreServicesAsync(options);

// Explore multiple specific services
var requiredServices = new[] { HeartRateServiceUuid, BatteryServiceUuid };
var options = new ServiceExplorationOptions
{
    Depth = ExplorationDepth.Characteristics,
    ServiceUuidFilter = uuid => requiredServices.Contains(uuid)
};

// Skip standard services
var options = new ServiceExplorationOptions
{
    Depth = ExplorationDepth.Characteristics,
    ServiceUuidFilter = uuid =>
        uuid != GenericAccessServiceUuid &&
        uuid != GenericAttributeServiceUuid
};
```

---

### Static Factory Methods

Convenient factory methods for common configurations:

#### ServicesOnly

```csharp
public static ServiceExplorationOptions ServicesOnly { get; }
```

Discovers only services (no characteristics or descriptors).

**Example**:
```csharp
await device.ExploreServicesAsync(ServiceExplorationOptions.ServicesOnly);
```

#### WithCharacteristics

```csharp
public static ServiceExplorationOptions WithCharacteristics { get; }
```

Discovers services and characteristics (no descriptors).

**Example**:
```csharp
await device.ExploreServicesAsync(ServiceExplorationOptions.WithCharacteristics);
```

#### Full

```csharp
public static ServiceExplorationOptions Full { get; }
```

Full exploration: services, characteristics, and descriptors.

**Example**:
```csharp
await device.ExploreServicesAsync(ServiceExplorationOptions.Full);
```

---

## Characteristic Exploration Options

`CharacteristicExplorationOptions` controls discovery of characteristics within a service.

### Properties

```csharp
public record CharacteristicExplorationOptions
{
    public bool ExploreDescriptors { get; init; }
    public bool UseCache { get; init; } = true;
    public Func<Guid, bool>? CharacteristicUuidFilter { get; init; }
}
```

#### ExploreDescriptors

```csharp
public bool ExploreDescriptors { get; init; }
```

**Default**: `false`

When enabled, automatically explores descriptors after discovering characteristics.

**Example**:
```csharp
var options = new CharacteristicExplorationOptions
{
    ExploreDescriptors = true
};
await service.ExploreCharacteristicsAsync(options);

// Descriptors are now available
foreach (var characteristic in service.Characteristics)
{
    Console.WriteLine($"Characteristic: {characteristic.Uuid}");
    foreach (var descriptor in characteristic.Descriptors)
    {
        Console.WriteLine($"  Descriptor: {descriptor.Uuid}");
    }
}
```

---

#### UseCache

```csharp
public bool UseCache { get; init; } = true;
```

**Default**: `true`

When enabled, skips exploration if characteristics have already been discovered.

**Example**:
```csharp
// First exploration
await service.ExploreCharacteristicsAsync(
    CharacteristicExplorationOptions.CharacteristicsOnly); // Explores

// Second call (cached)
await service.ExploreCharacteristicsAsync(
    CharacteristicExplorationOptions.CharacteristicsOnly); // Skipped

// Force re-exploration
var options = new CharacteristicExplorationOptions { UseCache = false };
await service.ExploreCharacteristicsAsync(options); // Explores again
```

---

#### CharacteristicUuidFilter

```csharp
public Func<Guid, bool>? CharacteristicUuidFilter { get; init; }
```

**Default**: `null` (explore all characteristics)

Filters which characteristics to explore.

**Example**:
```csharp
// Explore only specific characteristic
var options = new CharacteristicExplorationOptions
{
    CharacteristicUuidFilter = uuid => uuid == HeartRateMeasurementUuid
};
await service.ExploreCharacteristicsAsync(options);

// Explore multiple characteristics
var requiredChars = new[] { HeartRateMeasurementUuid, BodySensorLocationUuid };
var options = new CharacteristicExplorationOptions
{
    CharacteristicUuidFilter = uuid => requiredChars.Contains(uuid)
};
```

---

### Static Factory Methods

#### CharacteristicsOnly

```csharp
public static CharacteristicExplorationOptions CharacteristicsOnly { get; }
```

Discovers only characteristics (no descriptors).

**Example**:
```csharp
await service.ExploreCharacteristicsAsync(
    CharacteristicExplorationOptions.CharacteristicsOnly);
```

#### Full

```csharp
public static CharacteristicExplorationOptions Full { get; }
```

Discovers characteristics and descriptors.

**Example**:
```csharp
await service.ExploreCharacteristicsAsync(
    CharacteristicExplorationOptions.Full);
```

---

## Descriptor Exploration Options

`DescriptorExplorationOptions` controls discovery of descriptors within a characteristic.

### Properties

```csharp
public record DescriptorExplorationOptions
{
    public bool UseCache { get; init; } = true;
    public Func<Guid, bool>? DescriptorUuidFilter { get; init; }
}
```

#### UseCache

```csharp
public bool UseCache { get; init; } = true;
```

**Default**: `true`

When enabled, skips exploration if descriptors have already been discovered.

**Example**:
```csharp
// First exploration
await characteristic.ExploreDescriptorsAsync(
    DescriptorExplorationOptions.Default); // Explores

// Second call (cached)
await characteristic.ExploreDescriptorsAsync(
    DescriptorExplorationOptions.Default); // Skipped

// Force re-exploration
var options = new DescriptorExplorationOptions { UseCache = false };
await characteristic.ExploreDescriptorsAsync(options); // Explores again
```

---

#### DescriptorUuidFilter

```csharp
public Func<Guid, bool>? DescriptorUuidFilter { get; init; }
```

**Default**: `null` (explore all descriptors)

Filters which descriptors to explore.

**Example**:
```csharp
// Explore only CCCD descriptor
var options = new DescriptorExplorationOptions
{
    DescriptorUuidFilter = uuid => uuid == ClientCharacteristicConfigurationUuid
};
await characteristic.ExploreDescriptorsAsync(options);

// Skip user description descriptors
var options = new DescriptorExplorationOptions
{
    DescriptorUuidFilter = uuid => uuid != CharacteristicUserDescriptionUuid
};
```

---

### Static Factory Methods

#### Default

```csharp
public static DescriptorExplorationOptions Default { get; }
```

Default descriptor exploration options.

**Example**:
```csharp
await characteristic.ExploreDescriptorsAsync(DescriptorExplorationOptions.Default);
```

---

## Usage Examples

### Quick Service Discovery

```csharp
// Discover only services (fastest)
await device.ExploreServicesAsync(ServiceExplorationOptions.ServicesOnly);

// Check if device has Heart Rate service
var hasHeartRate = device.Services.Any(s => s.Uuid == HeartRateServiceUuid);
```

### Standard Exploration (Characteristics)

```csharp
// Most common: discover services and characteristics
await device.ExploreServicesAsync(ServiceExplorationOptions.WithCharacteristics);

// Now can read/write characteristics
var service = device.Services.First(s => s.Uuid == HeartRateServiceUuid);
var char = service.Characteristics.First(c => c.Uuid == HeartRateMeasurementUuid);
var value = await char.ReadAsync();
```

### Full Exploration (All Levels)

```csharp
// Discover everything: services, characteristics, descriptors
await device.ExploreServicesAsync(ServiceExplorationOptions.Full);

// Can now enable notifications via CCCD
var char = device.Services
    .First(s => s.Uuid == HeartRateServiceUuid)
    .Characteristics.First(c => c.Uuid == HeartRateMeasurementUuid);

var cccd = char.Descriptors
    .First(d => d.Uuid == ClientCharacteristicConfigurationUuid);

await cccd.WriteAsync(new byte[] { 0x01, 0x00 }); // Enable notifications
```

### Filtered Exploration

```csharp
// Explore only specific services
var options = new ServiceExplorationOptions
{
    Depth = ExplorationDepth.Characteristics,
    ServiceUuidFilter = uuid =>
        uuid == HeartRateServiceUuid ||
        uuid == BatteryServiceUuid
};
await device.ExploreServicesAsync(options);
```

### Manual Step-by-Step

```csharp
// Step 1: Discover services only
await device.ExploreServicesAsync(ServiceExplorationOptions.ServicesOnly);

// Step 2: Explore characteristics for specific service
var heartRateService = device.Services.First(s => s.Uuid == HeartRateServiceUuid);
await heartRateService.ExploreCharacteristicsAsync(
    CharacteristicExplorationOptions.CharacteristicsOnly);

// Step 3: Explore descriptors for specific characteristic
var char = heartRateService.Characteristics
    .First(c => c.Uuid == HeartRateMeasurementUuid);
await char.ExploreDescriptorsAsync(DescriptorExplorationOptions.Default);
```

### Custom Depth with Filtering

```csharp
var options = new ServiceExplorationOptions
{
    Depth = ExplorationDepth.Characteristics,
    UseCache = true,
    ServiceUuidFilter = uuid =>
    {
        // Skip standard services
        if (uuid == GenericAccessServiceUuid) return false;
        if (uuid == GenericAttributeServiceUuid) return false;

        // Include custom services
        return true;
    }
};

await device.ExploreServicesAsync(options);
```

### Production Configuration

```csharp
public async Task ExploreDeviceAsync(IBluetoothDevice device)
{
    // First pass: discover all services quickly
    await device.ExploreServicesAsync(ServiceExplorationOptions.ServicesOnly);

    // Check if device has required services
    if (!device.Services.Any(s => s.Uuid == MyRequiredServiceUuid))
    {
        throw new InvalidOperationException("Device missing required service");
    }

    // Second pass: explore only required services fully
    var options = new ServiceExplorationOptions
    {
        Depth = ExplorationDepth.Descriptors,
        ServiceUuidFilter = uuid => uuid == MyRequiredServiceUuid
    };

    await device.ExploreServicesAsync(options);

    // Now work with the explored service
    var service = device.Services.First(s => s.Uuid == MyRequiredServiceUuid);
    // ...
}
```

---

## Best Practices

### 1. Use Appropriate Depth

Only explore as deep as needed:

```csharp
// If you don't need descriptors, don't explore them
// Good - only explore what's needed
await device.ExploreServicesAsync(ServiceExplorationOptions.WithCharacteristics);

// Wasteful - exploring descriptors unnecessarily
await device.ExploreServicesAsync(ServiceExplorationOptions.Full);
```

### 2. Filter Services When Possible

Improve performance by limiting exploration:

```csharp
// Good - targeted exploration
var options = new ServiceExplorationOptions
{
    Depth = ExplorationDepth.Characteristics,
    ServiceUuidFilter = uuid => uuid == MyServiceUuid
};

// Slower - explores everything
var options = new ServiceExplorationOptions
{
    Depth = ExplorationDepth.Characteristics
};
```

### 3. Use Caching for Repeated Exploration

Default caching prevents redundant exploration:

```csharp
// First call explores
await device.ExploreServicesAsync(ServiceExplorationOptions.WithCharacteristics);

// Subsequent calls use cache (fast)
await device.ExploreServicesAsync(ServiceExplorationOptions.WithCharacteristics);
```

### 4. Manual Step-by-Step for Complex Logic

For conditional exploration logic:

```csharp
// Discover services first
await device.ExploreServicesAsync(ServiceExplorationOptions.ServicesOnly);

// Conditionally explore based on available services
foreach (var service in device.Services)
{
    if (IsRequiredService(service.Uuid))
    {
        await service.ExploreCharacteristicsAsync(
            CharacteristicExplorationOptions.Full);
    }
}
```

### 5. Static Factory Methods for Common Cases

Use built-in factory methods:

```csharp
// Good - readable and concise
await device.ExploreServicesAsync(ServiceExplorationOptions.Full);

// More verbose
await device.ExploreServicesAsync(new ServiceExplorationOptions
{
    ExploreCharacteristics = true,
    ExploreDescriptors = true
});
```

### 6. Validate Required Services

Check for required services after discovery:

```csharp
await device.ExploreServicesAsync(ServiceExplorationOptions.ServicesOnly);

var requiredServices = new[] { Service1Uuid, Service2Uuid };
var missingServices = requiredServices
    .Where(uuid => !device.Services.Any(s => s.Uuid == uuid))
    .ToList();

if (missingServices.Any())
{
    throw new InvalidOperationException(
        $"Device missing required services: {string.Join(", ", missingServices)}");
}
```

### 7. Handle Exploration Errors

Exploration can fail due to disconnection or timeouts:

```csharp
try
{
    await device.ExploreServicesAsync(ServiceExplorationOptions.Full);
}
catch (BluetoothDeviceDisconnectedException)
{
    // Device disconnected during exploration
    await ReconnectAndRetry();
}
catch (TimeoutException)
{
    // Exploration timed out
    // Try with less depth or specific services only
}
```

### 8. Exploration vs Explorer Pattern

For comprehensive device analysis, consider using the Explorer pattern:

```csharp
// Exploration options approach - manual
await device.ExploreServicesAsync(ServiceExplorationOptions.Full);

// Explorer pattern - automated and detailed
var explorer = new BluetoothDeviceExplorer();
var report = await explorer.ExploreDeviceAsync(device);
// report contains detailed analysis with logging
```

---

## Related Documentation

- [Dependency-Injection.md](./Dependency-Injection.md) - DI configuration guide
- [Connection-Options.md](./Connection-Options.md) - Connection configuration
- [Scanning-Options.md](./Scanning-Options.md) - Scanner configuration
- [Infrastructure-Options.md](./Infrastructure-Options.md) - Infrastructure configuration
