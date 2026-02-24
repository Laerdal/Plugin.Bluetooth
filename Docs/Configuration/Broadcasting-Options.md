# Broadcasting Options

`BroadcastingOptions` represents the configuration for Bluetooth Low Energy peripheral advertising operations. These options control how your device advertises itself to nearby scanners and what information is included in the advertisement.

## Table of Contents

- [Overview](#overview)
- [Basic Usage](#basic-usage)
- [Permission Options](#permission-options)
- [Advertisement Content](#advertisement-content)
  - [Device Name](#device-name)
  - [Connectability](#connectability)
  - [Transmit Power](#transmit-power)
  - [Manufacturer Data](#manufacturer-data)
  - [Service UUIDs](#service-uuids)
  - [Service Data](#service-data)
- [Extended Advertising (Bluetooth 5.0+)](#extended-advertising-bluetooth-50)
- [Platform Support](#platform-support)
- [Usage Examples](#usage-examples)
- [Best Practices](#best-practices)

---

## Overview

Broadcasting options are passed to `IBluetoothBroadcaster.StartBroadcastingAsync()` and control:
- Permission request behavior
- Advertisement content (device name, manufacturer data, service UUIDs)
- Connectability and discoverability
- Transmit power level
- Extended advertising features (Bluetooth 5.0+)
- Advertising interval
- Anonymity settings

**Namespace**: `Bluetooth.Abstractions.Broadcasting.Options`

**Usage Pattern**: Passed to method calls (not DI-configured)

---

## Basic Usage

```csharp
// Inject broadcaster via DI
public class MyBluetoothService
{
    private readonly IBluetoothBroadcaster _broadcaster;

    public MyBluetoothService(IBluetoothBroadcaster broadcaster)
    {
        _broadcaster = broadcaster;
    }

    public async Task StartAdvertising()
    {
        var options = new BroadcastingOptions
        {
            LocalDeviceName = "MyDevice",
            IncludeDeviceName = true,
            IsConnectable = true,
            AdvertisedServiceUuids = new[] { MyServiceUuid }
        };

        await _broadcaster.StartBroadcastingAsync(options);
    }
}
```

---

## Permission Options

### PermissionStrategy

```csharp
public PermissionRequestStrategy PermissionStrategy { get; init; } = PermissionRequestStrategy.RequestAutomatically;
```

**Default**: `PermissionRequestStrategy.RequestAutomatically`

Controls how Bluetooth advertising permissions are requested.

**Values**:

| Value | Behavior | Use Case |
|-------|----------|----------|
| `RequestAutomatically` | Automatically requests permissions before broadcasting | Most applications (recommended) |
| `ThrowIfNotGranted` | Throws exception if permissions not granted | Custom permission flow |
| `AssumeGranted` | Skips permission checks (assumes granted) | Manual permission management |

**Platform Requirements**:

| Platform | Permission | When Required |
|----------|-----------|---------------|
| **Android** | BLUETOOTH_ADVERTISE | Android 12+ (API 31+) |
| **iOS/macOS** | Bluetooth access | System handles automatically |
| **Windows** | Radio access | Checked before broadcasting |

**Example**:
```csharp
// Automatic (recommended)
options.PermissionStrategy = PermissionRequestStrategy.RequestAutomatically;

// Custom permission flow
options.PermissionStrategy = PermissionRequestStrategy.ThrowIfNotGranted;
try
{
    await _broadcaster.StartBroadcastingAsync(options);
}
catch (BluetoothPermissionException)
{
    // Handle permission error
    await RequestPermissionsManually();
}
```

---

## Advertisement Content

### Device Name

#### LocalDeviceName

```csharp
public string? LocalDeviceName { get; init; }
```

**Default**: `null` (uses system device name)

Sets the device name included in the advertisement.

**Example**:
```csharp
options.LocalDeviceName = "My BLE Sensor";
```

#### IncludeDeviceName

```csharp
public bool IncludeDeviceName { get; init; }
```

**Default**: `false`

Controls whether to include the device name in the advertisement.

**Advertisement Size Impact**:
- Device names consume valuable advertisement payload space
- Legacy advertising: limited to 31 bytes total
- Extended advertising: up to 254 bytes

**Use Cases**:
- **Enable**: User-facing device pickers, identification needed
- **Disable**: Beacon applications, save payload space, privacy

**Example**:
```csharp
// Include custom name
options.LocalDeviceName = "Heart Rate Monitor #42";
options.IncludeDeviceName = true;

// Don't advertise name (beacon mode)
options.IncludeDeviceName = false;
```

---

### Connectability

#### IsConnectable

```csharp
public bool IsConnectable { get; init; }
```

**Default**: `false`

Controls whether remote devices can establish GATT connections.

**Platform Support**:

| Platform | Support | Implementation |
|----------|---------|----------------|
| **Android** | Full | `AdvertiseSettings.setConnectable()` |
| **iOS/macOS** | Always connectable | Cannot be disabled when advertising services |
| **Windows** | Full | `BluetoothLEAdvertisementPublisher.IsDiscoverable` |

**Use Cases**:
- **Enable**: Full peripheral functionality, GATT server
- **Disable**: Beacon-only applications, broadcast-only data

**Example**:
```csharp
// Full peripheral with GATT services
options.IsConnectable = true;

// Beacon-only (no connections)
options.IsConnectable = false;
```

---

### Transmit Power

#### TxPowerLevel

```csharp
public int? TxPowerLevel { get; init; }
```

**Default**: `null` (platform default)

Sets the transmit power level in dBm.

**Typical Range**: `-21 dBm` to `+20 dBm`

**Power vs Range Trade-off**:

| Power | Range | Battery Impact | Use Case |
|-------|-------|---------------|----------|
| Low (-20 to -10 dBm) | Short (~1-5m) | Minimal | Close proximity, battery-sensitive |
| Medium (-9 to 0 dBm) | Moderate (~5-20m) | Moderate | Indoor use, balanced |
| High (1 to 20 dBm) | Long (~20-100m) | High | Outdoor, maximum range |

**Example**:
```csharp
// Low power for close proximity
options.TxPowerLevel = -20;

// High power for maximum range
options.TxPowerLevel = 10;
```

#### IncludeTxPowerLevel

```csharp
public bool IncludeTxPowerLevel { get; init; }
```

**Default**: `false`

Controls whether to include transmit power level in the advertisement.

**Use Cases**:
- Enable for distance calculation (RSSI + TX Power)
- Disable to save advertisement payload space

**Example**:
```csharp
options.TxPowerLevel = 0;
options.IncludeTxPowerLevel = true; // Include in advertisement for distance calc
```

---

### Manufacturer Data

Manufacturer data is custom binary data identified by a company identifier.

#### ManufacturerId / ManufacturerData (Single Entry)

```csharp
public ushort? ManufacturerId { get; init; }
public ReadOnlyMemory<byte>? ManufacturerData { get; init; }
```

**Default**: `null`

Simple API for a single manufacturer data entry.

**Company Identifiers**: Registered with Bluetooth SIG
- Example: `0x004C` = Apple Inc.
- Example: `0x0075` = Samsung Electronics Co. Ltd.

**Example**:
```csharp
// Single manufacturer data entry
options.ManufacturerId = 0x004C; // Apple
options.ManufacturerData = new byte[] { 0x02, 0x15, 0x01, 0x02, 0x03, ... };
```

#### ManufacturerDataEntries (Multiple Entries)

```csharp
public IReadOnlyDictionary<ushort, ReadOnlyMemory<byte>>? ManufacturerDataEntries { get; init; }
```

**Default**: `null`

Advanced API for multiple manufacturer data entries.

**Example**:
```csharp
// Multiple manufacturer data entries
options.ManufacturerDataEntries = new Dictionary<ushort, ReadOnlyMemory<byte>>
{
    [0x004C] = new byte[] { 0x02, 0x15, ... }, // Apple iBeacon
    [0x0075] = new byte[] { 0x42, 0x00, ... }  // Samsung
};
```

---

### Service UUIDs

#### AdvertisedServiceUuids

```csharp
public IReadOnlyList<Guid>? AdvertisedServiceUuids { get; init; }
```

**Default**: `null` (no services advertised)

List of GATT service UUIDs to advertise.

**Benefits**:
- Allows scanners to filter for specific services
- Indicates device capabilities
- Standard way to advertise device type

**Advertisement Space**:
- 16-bit UUIDs: 2 bytes each
- 128-bit UUIDs: 16 bytes each
- Use 16-bit for standard services

**Example**:
```csharp
// Advertise standard service (16-bit UUID)
options.AdvertisedServiceUuids = new[]
{
    Guid.Parse("0000180D-0000-1000-8000-00805F9B34FB") // Heart Rate Service
};

// Advertise custom service (128-bit UUID)
options.AdvertisedServiceUuids = new[]
{
    Guid.Parse("12345678-1234-5678-1234-567812345678") // Custom service
};

// Advertise multiple services
options.AdvertisedServiceUuids = new[]
{
    Guid.Parse("0000180D-0000-1000-8000-00805F9B34FB"), // Heart Rate
    Guid.Parse("0000180F-0000-1000-8000-00805F9B34FB"), // Battery
    Guid.Parse("0000181A-0000-1000-8000-00805F9B34FB")  // Device Information
};
```

---

### Service Data

#### ServiceData

```csharp
public IReadOnlyDictionary<Guid, ReadOnlyMemory<byte>>? ServiceData { get; init; }
```

**Default**: `null`

Service-specific data to include in the advertisement.

**Use Cases**:
- Broadcast sensor readings without connection
- Advertise device state
- Eddystone beacon (URL, UID, TLM)
- Custom broadcast data

**Example**:
```csharp
// Broadcast battery level without connection
var batteryServiceUuid = Guid.Parse("0000180F-0000-1000-8000-00805F9B34FB");
options.ServiceData = new Dictionary<Guid, ReadOnlyMemory<byte>>
{
    [batteryServiceUuid] = new byte[] { 95 } // 95% battery
};

// Broadcast sensor data
var customServiceUuid = Guid.Parse("12345678-1234-5678-1234-567812345678");
options.ServiceData = new Dictionary<Guid, ReadOnlyMemory<byte>>
{
    [customServiceUuid] = new byte[]
    {
        0x01, // Sensor type
        0x15, 0x00, // Temperature (21°C)
        0x42, 0x00  // Humidity (66%)
    }
};

// Multiple service data entries
options.ServiceData = new Dictionary<Guid, ReadOnlyMemory<byte>>
{
    [batteryServiceUuid] = new byte[] { 95 },
    [customServiceUuid] = new byte[] { 0x01, 0x02, 0x03 }
};
```

---

## Extended Advertising (Bluetooth 5.0+)

Extended advertising provides larger payloads and additional features in Bluetooth 5.0+.

### UseExtendedAdvertising

```csharp
public bool UseExtendedAdvertising { get; init; }
```

**Default**: `false`

Enables extended advertising (Bluetooth 5.0+).

**Benefits**:
- Larger payload (up to 254 bytes vs 31 bytes legacy)
- Multiple advertising sets support
- Periodic advertising support
- Better coexistence with other operations

**Platform Support**:

| Platform | Support | Requirements |
|----------|---------|--------------|
| **Android** | API 26+ | Bluetooth 5.0 hardware |
| **iOS/macOS** | Automatic | BT 5.0 device (system-managed) |
| **Windows** | Windows 10 2004+ | Bluetooth 5.0 adapter |

**Example**:
```csharp
// Enable extended advertising on supported platforms
options.UseExtendedAdvertising = true;
```

---

### PrimaryPhy

```csharp
public int? PrimaryPhy { get; init; }
```

**Default**: `null`

Primary PHY mode for advertising (extended advertising only).

**Platform Support**: Android only (API 26+)

**Values** (Android BluetoothDevice constants):
- `LE_1M` (1): Standard BLE
- `LE_CODED` (3): Long range

**Note**: Primary PHY must be either 1M or Coded PHY (for long range).

**Example**:
```csharp
#if ANDROID
options.UseExtendedAdvertising = true;
options.PrimaryPhy = Android.Bluetooth.BluetoothDevice.PhyLe1m; // 1M PHY
#endif
```

---

### SecondaryPhy

```csharp
public int? SecondaryPhy { get; init; }
```

**Default**: `null`

Secondary PHY mode for advertising (extended advertising only).

**Platform Support**: Android only (API 26+)

**Values** (Android BluetoothDevice constants):
- `LE_1M` (1): Standard BLE
- `LE_2M` (2): High throughput
- `LE_CODED` (3): Long range

**Note**: Secondary PHY carries advertisement data and can be 1M, 2M, or Coded PHY.

**Example**:
```csharp
#if ANDROID
options.UseExtendedAdvertising = true;
options.PrimaryPhy = Android.Bluetooth.BluetoothDevice.PhyLe1m;
options.SecondaryPhy = Android.Bluetooth.BluetoothDevice.PhyLe2m; // 2M for high throughput
#endif
```

---

### IsAnonymous

```csharp
public bool IsAnonymous { get; init; }
```

**Default**: `false`

Controls whether advertisement is anonymous (no device address).

**Platform Support**: Android only (API 26+)

**Use Cases**:
- Privacy-sensitive beacon applications
- Anonymous presence detection
- Comply with privacy regulations

**Example**:
```csharp
#if ANDROID
options.UseExtendedAdvertising = true;
options.IsAnonymous = true; // Don't include device address
#endif
```

---

### AdvertisingInterval

```csharp
public int? AdvertisingInterval { get; init; }
```

**Default**: `null` (platform default)

Advertising interval in milliseconds.

**Platform Support**: Android only (converted to advertisement interval units: 0.625ms)

**Trade-offs**:

| Interval | Discovery Speed | Battery Impact | Use Case |
|----------|----------------|---------------|----------|
| ~100ms | Fast | High | User-facing device discovery |
| ~500ms | Moderate | Moderate | Balanced mode |
| ~1000ms+ | Slow | Low | Background beacons, battery-sensitive |

**Range**: 100ms - 10000ms (typical)

**Example**:
```csharp
#if ANDROID
// Fast discovery
options.AdvertisingInterval = 100; // 100ms

// Battery-efficient beacon
options.AdvertisingInterval = 2000; // 2 seconds
#endif
```

---

## Platform Support

### Android

**Full Support**:
- All advertisement content options
- Extended advertising (API 26+)
- PHY selection (API 26+)
- Anonymous advertising (API 26+)
- Advertising interval control

**Example**:
```csharp
#if ANDROID
var options = new BroadcastingOptions
{
    LocalDeviceName = "Android Device",
    IncludeDeviceName = true,
    IsConnectable = true,
    ManufacturerId = 0x00E0,
    ManufacturerData = customData,
    AdvertisedServiceUuids = new[] { serviceUuid },
    UseExtendedAdvertising = true,
    PrimaryPhy = Android.Bluetooth.BluetoothDevice.PhyLe1m,
    SecondaryPhy = Android.Bluetooth.BluetoothDevice.PhyLe2m,
    AdvertisingInterval = 500,
    IsAnonymous = false
};
#endif
```

---

### iOS/macOS (CoreBluetooth)

**Support**:
- Device name (via `localName` key)
- Service UUIDs
- Always connectable when advertising services
- Extended advertising automatic on BT 5.0 devices

**Limitations**:
- Cannot control connectability (always connectable)
- No manufacturer data in advertisement (can add to services)
- No TX power control
- No PHY selection (system-managed)
- No advertising interval control

**Example**:
```csharp
#if IOS || MACCATALYST
var options = new BroadcastingOptions
{
    LocalDeviceName = "iOS Device",
    IncludeDeviceName = true,
    AdvertisedServiceUuids = new[] { serviceUuid },
    ServiceData = serviceDataDict
    // IsConnectable always true
    // Extended advertising automatic
};
#endif
```

---

### Windows

**Support**:
- Device name
- Manufacturer data
- Service UUIDs
- TX power level
- Connectability (via IsDiscoverable)
- Extended advertising (Windows 10 2004+)

**Limitations**:
- No PHY selection (system-managed)
- Limited advertising interval control

**Example**:
```csharp
#if WINDOWS
var options = new BroadcastingOptions
{
    LocalDeviceName = "Windows Device",
    IncludeDeviceName = true,
    IsConnectable = true,
    ManufacturerId = 0x0000,
    ManufacturerData = customData,
    AdvertisedServiceUuids = new[] { serviceUuid },
    TxPowerLevel = 0,
    IncludeTxPowerLevel = true,
    UseExtendedAdvertising = true
};
#endif
```

---

## Usage Examples

### Simple Device Advertisement

```csharp
var options = new BroadcastingOptions
{
    LocalDeviceName = "My Heart Rate Monitor",
    IncludeDeviceName = true,
    IsConnectable = true,
    AdvertisedServiceUuids = new[]
    {
        Guid.Parse("0000180D-0000-1000-8000-00805F9B34FB") // Heart Rate Service
    }
};

await _broadcaster.StartBroadcastingAsync(options);
```

### iBeacon (Apple Format)

```csharp
var uuid = Guid.Parse("12345678-1234-5678-1234-567812345678");
var major = (ushort)1;
var minor = (ushort)2;

var beaconData = new byte[23];
beaconData[0] = 0x02; // iBeacon type
beaconData[1] = 0x15; // Length
Array.Copy(uuid.ToByteArray(), 0, beaconData, 2, 16);
beaconData[18] = (byte)(major >> 8);
beaconData[19] = (byte)(major & 0xFF);
beaconData[20] = (byte)(minor >> 8);
beaconData[21] = (byte)(minor & 0xFF);
beaconData[22] = 0xC5; // TX power

var options = new BroadcastingOptions
{
    ManufacturerId = 0x004C, // Apple
    ManufacturerData = beaconData,
    IsConnectable = false,
    TxPowerLevel = -6
};

await _broadcaster.StartBroadcastingAsync(options);
```

### Sensor Beacon (Connectionless)

```csharp
var sensorServiceUuid = Guid.Parse("12345678-1234-5678-1234-567812345678");

// Encode temperature and humidity
var sensorData = new byte[]
{
    0x01, // Data version
    0x15, 0x00, // Temperature: 21°C (little-endian)
    0x3C, 0x00  // Humidity: 60% (little-endian)
};

var options = new BroadcastingOptions
{
    IncludeDeviceName = false,
    IsConnectable = false,
    ServiceData = new Dictionary<Guid, ReadOnlyMemory<byte>>
    {
        [sensorServiceUuid] = sensorData
    },
    AdvertisingInterval = 2000 // 2 seconds for battery savings
};

await _broadcaster.StartBroadcastingAsync(options);
```

### Full Peripheral with Extended Advertising

```csharp
var options = new BroadcastingOptions
{
    LocalDeviceName = "Advanced Sensor v2",
    IncludeDeviceName = true,
    IsConnectable = true,
    TxPowerLevel = 0,
    IncludeTxPowerLevel = true,

    AdvertisedServiceUuids = new[]
    {
        HeartRateServiceUuid,
        BatteryServiceUuid,
        DeviceInformationServiceUuid
    },

    ServiceData = new Dictionary<Guid, ReadOnlyMemory<byte>>
    {
        [BatteryServiceUuid] = new byte[] { 87 } // 87% battery
    },

    ManufacturerDataEntries = new Dictionary<ushort, ReadOnlyMemory<byte>>
    {
        [0x00E0] = new byte[] { 0x01, 0x02, 0x03, 0x04 }
    },

    UseExtendedAdvertising = true,

    #if ANDROID
    PrimaryPhy = Android.Bluetooth.BluetoothDevice.PhyLe1m,
    SecondaryPhy = Android.Bluetooth.BluetoothDevice.PhyLe2m,
    AdvertisingInterval = 500,
    IsAnonymous = false
    #endif
};

await _broadcaster.StartBroadcastingAsync(options);
```

### Production Configuration

```csharp
public class BroadcasterService
{
    private readonly IBluetoothBroadcaster _broadcaster;

    public async Task StartAdvertisingAsync(DeviceConfig config)
    {
        var options = new BroadcastingOptions
        {
            PermissionStrategy = PermissionRequestStrategy.RequestAutomatically,
            LocalDeviceName = config.DeviceName,
            IncludeDeviceName = true,
            IsConnectable = true,
            TxPowerLevel = config.RequireLongRange ? 10 : 0,
            IncludeTxPowerLevel = true,

            AdvertisedServiceUuids = config.Services.Select(s => s.Uuid).ToArray(),

            ServiceData = config.BroadcastData,

            ManufacturerId = CompanyId,
            ManufacturerData = BuildManufacturerData(config),

            UseExtendedAdvertising = config.UseExtendedAdvertising,

            #if ANDROID
            AdvertisingInterval = config.AdvertisingInterval,
            PrimaryPhy = Android.Bluetooth.BluetoothDevice.PhyLe1m,
            SecondaryPhy = config.RequireHighThroughput
                ? Android.Bluetooth.BluetoothDevice.PhyLe2m
                : Android.Bluetooth.BluetoothDevice.PhyLe1m
            #endif
        };

        try
        {
            await _broadcaster.StartBroadcastingAsync(options);
        }
        catch (BluetoothPermissionException)
        {
            await HandlePermissionError();
        }
        catch (Exception ex)
        {
            await HandleBroadcastError(ex);
        }
    }
}
```

---

## Best Practices

### 1. Minimize Advertisement Payload

Legacy advertising is limited to 31 bytes:

```csharp
// Good - minimal payload
options.IncludeDeviceName = false; // Save ~10-20 bytes
options.AdvertisedServiceUuids = new[] { requiredServiceUuid }; // 2-16 bytes

// Wasteful - unnecessary data
options.IncludeDeviceName = true; // Uses precious bytes
options.AdvertisedServiceUuids = allServiceUuids; // May exceed 31 bytes
```

### 2. Use Extended Advertising for Rich Content

When you need more data:

```csharp
options.UseExtendedAdvertising = true; // 254 bytes available
options.IncludeDeviceName = true;
options.ServiceData = richServiceData;
options.ManufacturerDataEntries = multipleManufacturers;
```

### 3. Balance Advertising Interval

Consider battery vs discoverability:

```csharp
// User-facing discovery (fast, high battery)
options.AdvertisingInterval = 100;

// Background beacon (slow, low battery)
options.AdvertisingInterval = 2000;
```

### 4. Use Standard Service UUIDs

Improves interoperability:

```csharp
// Good - standard service
options.AdvertisedServiceUuids = new[]
{
    Guid.Parse("0000180D-0000-1000-8000-00805F9B34FB") // Heart Rate
};

// Works but less discoverable - custom service
options.AdvertisedServiceUuids = new[]
{
    Guid.Parse("12345678-1234-5678-1234-567812345678")
};
```

### 5. Set Connectability Appropriately

```csharp
// Peripheral with GATT services
options.IsConnectable = true;

// Beacon-only
options.IsConnectable = false;
```

### 6. Platform-Specific Configuration

Use conditional compilation:

```csharp
var options = new BroadcastingOptions
{
    LocalDeviceName = "Device",
    IncludeDeviceName = true,
    IsConnectable = true,

    #if ANDROID
    UseExtendedAdvertising = true,
    AdvertisingInterval = 500,
    PrimaryPhy = Android.Bluetooth.BluetoothDevice.PhyLe1m,
    SecondaryPhy = Android.Bluetooth.BluetoothDevice.PhyLe2m,
    #elif IOS || MACCATALYST
    // iOS manages extended advertising automatically
    #endif
};
```

### 7. Handle Permission Errors

Always handle permission exceptions:

```csharp
try
{
    await _broadcaster.StartBroadcastingAsync(options);
}
catch (BluetoothPermissionException ex)
{
    // Prompt user or show rationale
    var granted = await RequestPermissionsWithRationale();
    if (granted)
    {
        // Retry
    }
}
```

### 8. Validate Advertisement Size

Ensure your advertisement fits:

```csharp
int EstimateAdvertisementSize(BroadcastingOptions options)
{
    int size = 0;

    // Device name
    if (options.IncludeDeviceName && options.LocalDeviceName != null)
        size += options.LocalDeviceName.Length + 2;

    // Service UUIDs
    if (options.AdvertisedServiceUuids != null)
        size += options.AdvertisedServiceUuids.Count * 16 + 2; // Assume 128-bit

    // Manufacturer data
    if (options.ManufacturerData != null)
        size += options.ManufacturerData.Value.Length + 4;

    // Service data
    if (options.ServiceData != null)
        foreach (var entry in options.ServiceData)
            size += entry.Value.Length + 18; // UUID + length + data

    return size;
}

// Check before advertising
int size = EstimateAdvertisementSize(options);
if (!options.UseExtendedAdvertising && size > 31)
{
    throw new InvalidOperationException($"Advertisement too large: {size} bytes (max 31)");
}
```

---

## Related Documentation

- [Dependency-Injection.md](./Dependency-Injection.md) - DI configuration guide
- [Scanning-Options.md](./Scanning-Options.md) - Scanner configuration
- [Connection-Options.md](./Connection-Options.md) - Connection configuration
- [Infrastructure-Options.md](./Infrastructure-Options.md) - Infrastructure configuration
