# Enumerations

Complete reference for all enumerations used in Plugin.Bluetooth.

## Table of Contents

- [Core Enums](#core-enums)
  - [CharacteristicProperties](#characteristicproperties)
  - [CharacteristicPermissions](#characteristicpermissions)
  - [PhyMode](#phymode)
  - [Manufacturer](#manufacturer)
  - [PermissionRequestStrategy](#permissionrequeststrategy)
- [Scanning Enums](#scanning-enums)
  - [BluetoothScanMode](#bluetoothscanmode)
  - [BluetoothConnectionPriority](#bluetoothconnectionpriority)
  - [BluetoothTransport](#bluetoothtransport)
- [Broadcasting Enums](#broadcasting-enums)
  - [BluetoothCharacteristicProperties](#bluetoothcharacteristicproperties)
  - [BluetoothCharacteristicPermissions](#bluetoothcharacteristicpermissions)
  - [BluetoothDescriptorPermissions](#bluetoothdescriptorpermissions)
  - [BroadcastingOptions](#broadcastingoptions)

---

## Core Enums

### CharacteristicProperties

**Namespace:** `Bluetooth.Abstractions.Enums`

Defines the operations that can be performed on a GATT characteristic.

```csharp
[Flags]
public enum CharacteristicProperties
{
    None = 0,
    Broadcast = 1 << 0,           // 0x01
    Read = 1 << 1,                // 0x02
    WriteWithoutResponse = 1 << 2, // 0x04
    Write = 1 << 3,               // 0x08
    Notify = 1 << 4,              // 0x10
    Indicate = 1 << 5,            // 0x20
    AuthenticatedSignedWrites = 1 << 6, // 0x40
    ExtendedProperties = 1 << 7,  // 0x80
    NotifyEncryptionRequired = 1 << 8,  // 0x100
    IndicateEncryptionRequired = 1 << 9 // 0x200
}
```

#### Values

| Value | Description | Usage |
|-------|-------------|-------|
| `None` | No properties | Used for initialization |
| `Broadcast` | Characteristic supports broadcast | Peripheral advertising |
| `Read` | Characteristic can be read | Central reads value |
| `WriteWithoutResponse` | Characteristic can be written without response | Fast writes, no acknowledgment |
| `Write` | Characteristic can be written with response | Reliable writes with acknowledgment |
| `Notify` | Characteristic supports notifications | Server pushes updates to client |
| `Indicate` | Characteristic supports indications | Server pushes updates with acknowledgment |
| `AuthenticatedSignedWrites` | Writes require authentication signature | Secure writes |
| `ExtendedProperties` | Additional properties in descriptor | Advanced features |
| `NotifyEncryptionRequired` | Notifications require encryption | Secure notifications |
| `IndicateEncryptionRequired` | Indications require encryption | Secure indications |

#### Usage

```csharp
// Check if characteristic supports read
if (characteristic.Properties.HasFlag(CharacteristicProperties.Read))
{
    await characteristic.ReadValueAsync();
}

// Check for write support (either type)
bool canWrite = characteristic.Properties.HasFlag(CharacteristicProperties.Write) ||
                characteristic.Properties.HasFlag(CharacteristicProperties.WriteWithoutResponse);

// Check for notification/indication support
bool canListen = characteristic.Properties.HasFlag(CharacteristicProperties.Notify) ||
                 characteristic.Properties.HasFlag(CharacteristicProperties.Indicate);

// Define properties for broadcasting
var properties = CharacteristicProperties.Read |
                 CharacteristicProperties.Write |
                 CharacteristicProperties.Notify;

await service.AddCharacteristicAsync(uuid, properties, permissions);
```

**Platform differences:**
- All platforms support standard properties (Read, Write, Notify, Indicate)
- Encryption-related properties may behave differently across platforms
- Windows has limited support for `AuthenticatedSignedWrites`

**See also:** [IBluetoothRemoteCharacteristic](./Abstractions.md#ibletoothremotecharacteristic), [CharacteristicPermissions](#characteristicpermissions)

---

### CharacteristicPermissions

**Namespace:** `Bluetooth.Abstractions.Enums`

Defines the permission requirements for accessing a GATT characteristic.

```csharp
[Flags]
public enum CharacteristicPermissions
{
    None = 0,
    Read = 1 << 0,                    // 0x01
    ReadEncrypted = 1 << 1,           // 0x02
    ReadEncryptedMitm = 1 << 2,       // 0x04
    Write = 1 << 3,                   // 0x08
    WriteEncrypted = 1 << 4,          // 0x10
    WriteEncryptedMitm = 1 << 5,      // 0x20
    WriteSigned = 1 << 6,             // 0x40
    WriteSignedMitm = 1 << 7          // 0x80
}
```

#### Values

| Value | Description | Security Level |
|-------|-------------|----------------|
| `None` | No permissions | Not accessible |
| `Read` | Read permission | No encryption |
| `ReadEncrypted` | Read with encryption | Encrypted connection |
| `ReadEncryptedMitm` | Read with encryption and MITM protection | Encrypted + authenticated |
| `Write` | Write permission | No encryption |
| `WriteEncrypted` | Write with encryption | Encrypted connection |
| `WriteEncryptedMitm` | Write with encryption and MITM protection | Encrypted + authenticated |
| `WriteSigned` | Signed write permission | Signed data |
| `WriteSignedMitm` | Signed write with MITM protection | Signed + authenticated |

#### Usage

```csharp
// Public read, authenticated write
var permissions = CharacteristicPermissions.Read |
                  CharacteristicPermissions.WriteEncryptedMitm;

await service.AddCharacteristicAsync(uuid, properties, permissions);

// Check permissions (server-side)
if (characteristic.Permissions.HasFlag(CharacteristicPermissions.ReadEncrypted))
{
    // Ensure connection is encrypted before serving data
}
```

**Security notes:**
- MITM = Man-In-The-Middle protection (pairing required)
- Encrypted permissions require bonded connection
- Permissions are enforced by OS/Bluetooth stack
- Client apps don't typically query permissions directly

**Platform differences:**
- Android: Full support for all permission levels
- iOS/macOS: Some permission flags are advisory
- Windows: Limited support for signed operations

---

### PhyMode

**Namespace:** `Bluetooth.Abstractions.Enums`

Specifies the Physical Layer (PHY) mode for Bluetooth 5+ connections.

```csharp
public enum PhyMode
{
    Le1M = 1,    // 1 Mbps, Bluetooth 4.x and 5+
    Le2M = 2,    // 2 Mbps, Bluetooth 5+
    LeCoded = 3  // Long range coded PHY, Bluetooth 5+
}
```

#### Values

| Value | Data Rate | Range | Power | Compatibility |
|-------|-----------|-------|-------|---------------|
| `Le1M` | 1 Mbps | Standard (~10-30m) | Standard | BT 4.0+ |
| `Le2M` | 2 Mbps | Reduced (~10-20m) | Higher | BT 5.0+ |
| `LeCoded` | 125-500 Kbps | Extended (~100-200m) | Lower | BT 5.0+ |

#### Usage

```csharp
// Request 2M PHY for higher throughput
await device.SetPreferredPhyAsync(
    txPhy: PhyMode.Le2M,
    rxPhy: PhyMode.Le2M
);

// Monitor PHY changes
device.PhyChanged += (s, e) =>
{
    Console.WriteLine($"TX PHY: {e.TxPhy}, RX PHY: {e.RxPhy}");
};

// Check current PHY
PhyMode currentTxPhy = device.TxPhy;
PhyMode currentRxPhy = device.RxPhy;
```

**Characteristics:**

- **Le1M (1 Mbps):**
  - Default and most compatible
  - Good balance of range and throughput
  - Supported by all BLE devices

- **Le2M (2 Mbps):**
  - Double the throughput of 1M
  - Slightly reduced range
  - Better for large data transfers
  - Requires Bluetooth 5.0+

- **LeCoded (Long Range):**
  - Up to 4x the range of 1M
  - Reduced data rate
  - Better for sensor networks
  - Requires Bluetooth 5.0+

**Platform support:**
- Android 8.0+ (API 26): Full support
- iOS 11+: System-managed, preference hints only
- macOS 10.13+: System-managed
- Windows: Limited support

**See also:** [IBluetoothRemoteDevice](./Abstractions.md#ibletoothremotedevice)

---

### Manufacturer

**Namespace:** `Bluetooth.Abstractions.Enums`

Enumerates Bluetooth SIG assigned company identifiers.

```csharp
public enum Manufacturer : short
{
    Unknown = -1,
    Ericsson_Technology_Licensing = 0,
    Microsoft = 6,
    Apple_Inc = 76,
    Nordic_Semiconductor_ASA = 89,
    Samsung_Electronics_Co_Ltd = 117,
    Google = 224,
    // ... 2000+ more, assigned in Bluetooth SIG order (see Bluetooth.Abstractions/Enums/Manufacturer.cs)
}
```

#### Common Values

| Value | Company | Typical Products |
|-------|---------|------------------|
| `Apple_Inc` (76) | Apple Inc. | iPhone, iPad, AirPods, Apple Watch |
| `Samsung_Electronics_Co_Ltd` (117) | Samsung Electronics | Galaxy devices, wearables |
| `Google` (224) | Google LLC | Pixel devices, Nest products |
| `Microsoft` (6) | Microsoft Corp. | Surface, Xbox controllers |
| `Nordic_Semiconductor_ASA` (89) | Nordic Semiconductor | nRF52 series chips |

#### Usage

```csharp
// Parse manufacturer data from advertisement
device.AdvertisementReceived += (s, e) =>
{
    var ad = e.Advertisement;
    Manufacturer manufacturer = ad.Manufacturer;
    Console.WriteLine($"Data from {manufacturer}: {BitConverter.ToString(ad.ManufacturerData.ToArray())}");

    if (manufacturer == Manufacturer.Apple_Inc)
    {
        // Parse Apple-specific advertisement format
    }
};

// Check device manufacturer (if available)
if (device.Manufacturer == Manufacturer.Nordic_Semiconductor_ASA)
{
    Console.WriteLine("Nordic-based device detected");
}
```

**Notes:**
- Backed by `short`, not `ushort` — `Unknown` is `-1`, not a large positive sentinel
- Full list contains 2400+ companies, assigned in Bluetooth SIG registration order (member names are the full company name, not short vendor nicknames — e.g. `Apple_Inc`, not `Apple`)
- Unknown/unparseable companies default to `Unknown`
- Company ID is separate from device manufacturer
- Used in manufacturer-specific advertisement data

**Reference:** [Bluetooth SIG Company Identifiers](https://www.bluetooth.com/specifications/assigned-numbers/company-identifiers/)

---

### PermissionRequestStrategy

**Namespace:** `Bluetooth.Abstractions`

Defines strategies for requesting Bluetooth permissions.

```csharp
public enum PermissionRequestStrategy
{
    Automatic,    // Request immediately when needed
    Manual,       // Require explicit permission request
    Deferred      // Delay until first use
}
```

#### Values

| Strategy | Description | Best For |
|----------|-------------|----------|
| `Automatic` | Request permissions immediately on startup | Production apps with clear UX |
| `Manual` | App explicitly calls request methods | Apps with permission explainer screens |
| `Deferred` | Request when first operation requires it | Apps with multiple entry points |

#### Usage

```csharp
// Configure during startup (typically in MauiProgram.cs)
builder.Services.AddBluetooth(options =>
{
    options.PermissionRequestStrategy = PermissionRequestStrategy.Manual;
});

// Manual strategy requires explicit requests
if (!await scanner.HasScannerPermissionsAsync())
{
    // Show explainer UI to user
    ShowPermissionExplainer();

    // Then request
    await scanner.RequestScannerPermissionsAsync();
}

// Automatic strategy handles it for you
await scanner.StartScanningAsync(); // Permissions requested if needed
```

**Recommendations:**
- **Automatic:** Simplest for most apps, but may surprise users
- **Manual:** Best UX, allows explaining permissions first
- **Deferred:** Good for complex apps, but harder to manage

**Platform considerations:**
- iOS requires permission explanation in Info.plist
- Android shows system permission dialogs
- Manual strategy gives most control over timing

---

## Scanning Enums

### BluetoothScanMode

**Namespace:** `Bluetooth.Abstractions.Scanning`

Defines the scanning mode affecting power consumption and responsiveness.

```csharp
public enum BluetoothScanMode
{
    LowPower,      // Battery efficient, slower discovery
    Balanced,      // Moderate power and responsiveness
    LowLatency,    // Fast discovery, higher power consumption
    Opportunistic  // Minimal power, only with other scans
}
```

#### Comparison

| Mode | Power Usage | Scan Interval | Scan Window | Best For |
|------|-------------|---------------|-------------|----------|
| `LowPower` | Very Low | ~5 sec | ~0.5 sec | Background monitoring |
| `Balanced` | Moderate | ~2.5 sec | ~1.5 sec | General use |
| `LowLatency` | High | ~0 sec | Continuous | Active scanning |
| `Opportunistic` | Minimal | N/A | Piggybacks | Multiple apps scanning |

#### Usage

```csharp
var options = new ScanningOptions
{
    ScanMode = BluetoothScanMode.LowLatency // Fast discovery
};

await scanner.StartScanningAsync(options);

// ScanningOptions properties are init-only and there is no runtime "update options"
// method — to change scan mode, stop and start again with new options:
await scanner.StopScanningAsync();
await scanner.StartScanningAsync(options with { ScanMode = BluetoothScanMode.LowPower });
```

**Mode details:**

- **LowPower:**
  - 512ms scan every 5.12s (Android)
  - Delayed discovery (5-10 seconds)
  - Best for: Background monitoring, sensor checks

- **Balanced:**
  - 2048ms scan every 4.096s (Android)
  - Quick discovery (2-5 seconds)
  - Best for: Most apps, general scanning

- **LowLatency:**
  - Continuous or very frequent scanning
  - Immediate discovery (<1 second)
  - Best for: Active device search, pairing

- **Opportunistic:**
  - No active scanning
  - Only gets results from other apps' scans
  - Best for: Very low power scenarios

**Platform support:**
- Android: Full support for all modes
- iOS/macOS: Modes are hints, OS manages actual behavior
- Windows: Limited control over scan parameters

**Battery impact:** LowLatency can drain battery 10-20x faster than LowPower

---

### BluetoothConnectionPriority

**Namespace:** `Bluetooth.Abstractions.Scanning`

Defines the connection priority/performance mode for active connections.

```csharp
public enum BluetoothConnectionPriority
{
    LowPower,    // Battery efficient, higher latency
    Balanced,    // Moderate power and latency
    High         // Fast data transfer, higher power
}
```

#### Comparison

| Priority | Interval | Latency | Throughput | Power |
|----------|----------|---------|------------|-------|
| `LowPower` | 100-125ms | ~125ms | Low | Minimal |
| `Balanced` | 30-50ms | ~40ms | Moderate | Moderate |
| `High` | 11.25-15ms | ~15ms | High | High |

#### Usage

```csharp
// Request high priority for large data transfer
await device.RequestConnectionPriorityAsync(BluetoothConnectionPriority.High);

// Perform data intensive operations
await TransferLargeData();

// Return to balanced to save power
await device.RequestConnectionPriorityAsync(BluetoothConnectionPriority.Balanced);
```

**Use cases:**

- **LowPower:**
  - Sensor devices with infrequent updates
  - Always-on connections
  - Battery-powered peripherals

- **Balanced:**
  - Default for most apps
  - Good for interactive apps
  - General purpose connections

- **High:**
  - Firmware updates
  - Large file transfers
  - Real-time control
  - Audio streaming

**Platform support:**
- Android: Full support via `requestConnectionPriority()`
- iOS/macOS: System-managed, method is no-op
- Windows: System-managed, method is no-op

**Notes:**
- Priority is a request, not a guarantee
- Changes take effect after 1-2 connection intervals
- High priority significantly increases power consumption

---

### BluetoothTransport

**Namespace:** `Bluetooth.Abstractions.Scanning`

Specifies the physical transport layer for connections.

```csharp
public enum BluetoothTransport
{
    Auto,      // System chooses automatically
    Le,        // Bluetooth Low Energy only
    Classic,   // Bluetooth Classic (BR/EDR) only
    Dual       // Both LE and Classic capable
}
```

#### Values

| Transport | Description | Use Case |
|-----------|-------------|----------|
| `Auto` | OS selects best transport | Default, most compatible |
| `Le` | BLE only, low energy | Sensors, wearables, IoT |
| `Classic` | BR/EDR only | Audio, HID devices |
| `Dual` | Device supports both | Smartphones, modern devices |

#### Usage

```csharp
var options = new ConnectionOptions
{
    EnableTransport = BluetoothTransport.Le
};

await device.ConnectAsync(options);
```

**Transport comparison:**

| Feature | BLE (Le) | Classic (BR/EDR) |
|---------|----------|------------------|
| Power | Very Low | Higher |
| Range | ~10-30m | ~10-100m |
| Data Rate | 1-2 Mbps | 1-3 Mbps |
| Latency | ~10-50ms | ~1-10ms |
| Use Cases | IoT, sensors | Audio, HID |

**Platform support:**
- Android: Full support for all transports
- iOS: LE only (Classic not available)
- macOS: LE primary, Classic limited
- Windows: Both supported

**Notes:**
- Most modern devices are dual-mode
- Plugin.Bluetooth focuses on BLE (Le)
- Classic support is limited in this library

---

## Broadcasting Enums

### BluetoothCharacteristicProperties

**Namespace:** `Bluetooth.Abstractions.Broadcasting.Enums`

Broadcasting-specific characteristic properties enum (similar to scanning version).

```csharp
[Flags]
public enum BluetoothCharacteristicProperties
{
    None = 0,
    Broadcast = 1 << 0,
    Read = 1 << 1,
    WriteWithoutResponse = 1 << 2,
    Write = 1 << 3,
    Notify = 1 << 4,
    Indicate = 1 << 5,
    AuthenticatedSignedWrites = 1 << 6,
    ExtendedProperties = 1 << 7
}
```

#### Usage

```csharp
// Define characteristic properties when creating service
var properties = BluetoothCharacteristicProperties.Read |
                 BluetoothCharacteristicProperties.Write |
                 BluetoothCharacteristicProperties.Indicate;

var characteristic = await service.AddCharacteristicAsync(
    uuid,
    properties,
    permissions
);
```

**See also:** [CharacteristicProperties](#characteristicproperties) for detailed descriptions

---

### BluetoothCharacteristicPermissions

**Namespace:** `Bluetooth.Abstractions.Broadcasting.Enums`

Broadcasting-specific characteristic permissions enum.

```csharp
[Flags]
public enum BluetoothCharacteristicPermissions
{
    None = 0,
    Read = 1 << 0,
    ReadEncrypted = 1 << 1,
    ReadEncryptedMitm = 1 << 2,
    Write = 1 << 3,
    WriteEncrypted = 1 << 4,
    WriteEncryptedMitm = 1 << 5,
    WriteSigned = 1 << 6,
    WriteSignedMitm = 1 << 7
}
```

#### Usage

```csharp
// Define permissions for characteristic
var permissions = BluetoothCharacteristicPermissions.Read |
                  BluetoothCharacteristicPermissions.WriteEncrypted;

var characteristic = await service.AddCharacteristicAsync(
    uuid,
    properties,
    permissions
);
```

**See also:** [CharacteristicPermissions](#characteristicpermissions) for detailed descriptions

---

### BluetoothDescriptorPermissions

**Namespace:** `Bluetooth.Abstractions.Broadcasting.Enums`

Permissions for GATT descriptors in broadcasting.

```csharp
[Flags]
public enum BluetoothDescriptorPermissions
{
    None = 0,
    Read = 1 << 0,
    ReadEncrypted = 1 << 1,
    ReadEncryptedMitm = 1 << 2,
    Write = 1 << 3,
    WriteEncrypted = 1 << 4,
    WriteEncryptedMitm = 1 << 5
}
```

#### Usage

```csharp
var permissions = BluetoothDescriptorPermissions.Read |
                  BluetoothDescriptorPermissions.Write;

var descriptor = await characteristic.AddDescriptorAsync(
    uuid,
    permissions
);
```

---

### BroadcastingOptions

**Namespace:** `Bluetooth.Abstractions.Broadcasting.Options`

There is no `BluetoothAdvertiseMode` enum, and broadcasting has no advertise-mode/interval concept exposed to callers (that's platform-managed) — configuration is a plain options record, not an enum:

```csharp
public record BroadcastingOptions
{
    string? LocalDeviceName { get; init; }
    bool IncludeDeviceName { get; init; }
    IReadOnlyList<Guid>? AdvertisedServiceUuids { get; init; }
}
```

#### Usage

```csharp
var options = new BroadcastingOptions
{
    LocalDeviceName = "MyDevice",
    IncludeDeviceName = true,
    AdvertisedServiceUuids = [myServiceUuid]
};

await broadcaster.StartBroadcastingAsync(options);

// To change options while broadcasting, stop and start again — there is no
// UpdateBroadcastingOptionsAsync method:
await broadcaster.StopBroadcastingAsync();
await broadcaster.StartBroadcastingAsync(options with { IncludeDeviceName = false });
```

**Platform support:**
- Android: Full support
- iOS/macOS: `LocalDeviceName`/`IncludeDeviceName` are advisory — the OS may substitute the device's Bluetooth name
- Windows: Full support

---

## Option Classes

`ScanningOptions`, `ConnectionOptions`, and `BroadcastingOptions` are documented alongside the rest of the options classes, not here — see [Scanning-Options.md](../Configuration/Scanning-Options.md), [Connection-Options.md](../Configuration/Connection-Options.md), and [Broadcasting-Options.md](../Configuration/Broadcasting-Options.md). (A previous version of this file duplicated a fabricated copy of these classes here — removed rather than fixed twice.)

---

## See Also

- [Overview and Conventions](./README.md)
- [Interfaces and Abstractions](./Abstractions.md)
- [Events](./Events.md)
- [Exceptions](./Exceptions.md)
