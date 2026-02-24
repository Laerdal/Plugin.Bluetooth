# Android Platform Guide

Comprehensive guide for Plugin.Bluetooth on Android platform using Android's Bluetooth Low Energy APIs.

## Table of Contents
- [Overview](#overview)
- [Requirements](#requirements)
- [Android Bluetooth Architecture](#android-bluetooth-architecture)
- [Configuration](#configuration)
- [API Level Requirements](#api-level-requirements)
- [Feature Support](#feature-support)
- [Platform-Specific Concerns](#platform-specific-concerns)
- [Best Practices](#best-practices)
- [Troubleshooting](#troubleshooting)

## Overview

Android uses its native **Bluetooth Low Energy (BLE) APIs** introduced in Android 4.3 (API 18) and significantly enhanced in later versions. Plugin.Bluetooth provides a managed wrapper around these APIs with retry logic, error handling, and cross-platform abstractions.

### Key Characteristics
- **Highly Configurable**: Full control over MTU, PHY, connection priority, and GATT operations
- **API Level Sensitive**: Many features require specific minimum API levels
- **Manufacturer Variance**: BLE stack implementations vary by device manufacturer
- **GATT Error 133**: Common connection issue requiring retry logic
- **Complex Permissions**: Permission requirements vary significantly by API level

### Supported API Levels
- **Minimum**: API 21 (Android 5.0 Lollipop) - Basic BLE support
- **Recommended**: API 23+ (Android 6.0) - Runtime permissions
- **Enhanced Features**: API 26+ (Android 8.0) - PHY control
- **L2CAP**: API 29+ (Android 10) - Direct socket connections
- **Latest**: API 33+ (Android 13) - Refined permissions

## Requirements

### Minimum API Levels by Feature
| Feature | Minimum API Level | Android Version |
|---------|-------------------|-----------------|
| Basic BLE (Scan, Connect, GATT) | API 21 | Android 5.0 (Lollipop) |
| Runtime Permissions | API 23 | Android 6.0 (Marshmallow) |
| PHY Control | API 26 | Android 8.0 (Oreo) |
| L2CAP Channels | API 29 | Android 10 |
| Refined BLE Permissions | API 31 | Android 12 |
| Enhanced MTU APIs | API 33 | Android 13 |

### Target SDK Version
```xml
<TargetFramework>net8.0-android34.0</TargetFramework>
```

## Android Bluetooth Architecture

### Class Hierarchy
Plugin.Bluetooth maps to Android BLE APIs as follows:

| Plugin.Bluetooth | Android API | Description |
|------------------|-------------|-------------|
| `IBluetoothScanner` | `BluetoothLeScanner` | Scans for BLE devices |
| `IBluetoothRemoteDevice` | `BluetoothDevice` + `BluetoothGatt` | Remote device and GATT client |
| `IBluetoothRemoteService` | `BluetoothGattService` | GATT service |
| `IBluetoothRemoteCharacteristic` | `BluetoothGattCharacteristic` | GATT characteristic |
| `IBluetoothRemoteDescriptor` | `BluetoothGattDescriptor` | GATT descriptor |
| `IBluetoothBroadcaster` | `BluetoothLeAdvertiser` + `BluetoothGattServer` | Peripheral mode |
| `IBluetoothRemoteL2CapChannel` | `BluetoothSocket` (L2CAP) | Direct socket connection |

### Implementation Details
- **BluetoothGattProxy**: Wraps `BluetoothGatt` with callback delegation
- **Retry Logic**: Built-in retry for GATT error 133 and service discovery failures
- **Thread Safety**: Operations synchronized to avoid GATT queue conflicts
- **CCCD Handling**: Automatic Client Characteristic Configuration Descriptor (0x2902) management

## Configuration

### AndroidManifest.xml

Required permissions vary significantly by Android version:

#### API 21-22 (Android 5.0-5.1)
```xml
<uses-permission android:name="android.permission.BLUETOOTH" />
<uses-permission android:name="android.permission.BLUETOOTH_ADMIN" />
```

#### API 23-30 (Android 6.0-10)
```xml
<uses-permission android:name="android.permission.BLUETOOTH" />
<uses-permission android:name="android.permission.BLUETOOTH_ADMIN" />
<uses-permission android:name="android.permission.ACCESS_FINE_LOCATION" />
<uses-permission android:name="android.permission.ACCESS_COARSE_LOCATION" />
```

**Why location?** Android considers BLE scanning capable of determining user location, thus requiring location permission.

#### API 31+ (Android 12+) - Recommended
```xml
<!-- Legacy permissions (for API < 31) -->
<uses-permission android:name="android.permission.BLUETOOTH"
                 android:maxSdkVersion="30" />
<uses-permission android:name="android.permission.BLUETOOTH_ADMIN"
                 android:maxSdkVersion="30" />
<uses-permission android:name="android.permission.ACCESS_FINE_LOCATION"
                 android:maxSdkVersion="30" />

<!-- New permissions for API 31+ -->
<uses-permission android:name="android.permission.BLUETOOTH_SCAN"
                 android:usesPermissionFlags="neverForLocation" /> <!-- Opt out of location -->
<uses-permission android:name="android.permission.BLUETOOTH_CONNECT" />
<uses-permission android:name="android.permission.BLUETOOTH_ADVERTISE" /> <!-- For broadcasting -->

<!-- Optional: Remove location requirement if you don't use BLE for positioning -->
<uses-permission android:name="android.permission.ACCESS_FINE_LOCATION"
                 android:maxSdkVersion="30" />
```

**API 31+ Benefits:**
- `BLUETOOTH_SCAN` with `neverForLocation` flag removes location requirement
- `BLUETOOTH_CONNECT` for device connections
- `BLUETOOTH_ADVERTISE` for peripheral mode
- More granular permissions

#### Background Location (API 29+)
For background scanning:
```xml
<uses-permission android:name="android.permission.ACCESS_BACKGROUND_LOCATION" />
```

**Note**: Very restrictive user approval process. Only request if absolutely necessary.

### Feature Declarations
```xml
<uses-feature android:name="android.hardware.bluetooth_le" android:required="true" />
```

### Runtime Permission Requests

Plugin.Bluetooth handles permissions automatically via `PermissionStrategy`:

```csharp
var options = new ScanningOptions
{
    // Automatically request required permissions before scanning
    PermissionStrategy = PermissionRequestStrategy.RequestAutomatically
};

await scanner.StartScanningAsync(options);
```

**Permission Flow:**
1. Check if permissions granted
2. If not, request using Android permission dialogs
3. If denied, throw `PermissionException`
4. If granted, proceed with operation

## API Level Requirements

### Feature Availability by API Level

#### API 21 (Android 5.0) - Baseline
- ✅ BLE scanning with filters
- ✅ GATT client operations (read/write/notify)
- ✅ MTU negotiation via `requestMtu()`
- ✅ Connection priority via `requestConnectionPriority()`
- ✅ Broadcasting (peripheral mode) via `BluetoothLeAdvertiser`

#### API 23 (Android 6.0) - Runtime Permissions
- ✅ Runtime permission requests
- ✅ Location permission required for scanning

#### API 26 (Android 8.0) - PHY Control
- ✅ PHY preference via `setPreferredPhy()`
- ✅ Read current PHY via `readPhy()`
- ✅ Up to 2 Mbps with LE 2M PHY (hardware dependent)

#### API 29 (Android 10) - L2CAP
- ✅ L2CAP channels via `connectL2capChannel()`
- ✅ Direct socket communication bypassing GATT
- ✅ Higher throughput for bulk data transfer

#### API 31 (Android 12) - Refined Permissions
- ✅ Granular Bluetooth permissions (`BLUETOOTH_SCAN`, `BLUETOOTH_CONNECT`)
- ✅ Optional location requirement for scanning
- ⚠️ Breaking change: Old permissions insufficient

#### API 33 (Android 13) - Enhanced APIs
- ✅ `MaxTransmitPacketSize` for L2CAP MTU
- ✅ Additional PHY options

### Handling API Level Differences

Plugin.Bluetooth automatically handles API level differences:

```csharp
// MTU request (API 21+)
if (OperatingSystem.IsAndroidVersionAtLeast(21))
{
    await device.RequestMtuAsync(512); // Automatically checks API level
}

// PHY control (API 26+)
if (OperatingSystem.IsAndroidVersionAtLeast(26))
{
    await device.SetPreferredPhyAsync(PhyMode.Le2M, PhyMode.Le2M);
}

// L2CAP (API 29+)
if (OperatingSystem.IsAndroidVersionAtLeast(29))
{
    await device.OpenL2CapChannelAsync(psm: 0x0080);
}
```

**Unsupported API Level**: Throws `PlatformNotSupportedException` with clear error message.

## Feature Support

### ✅ Fully Supported Features

#### 1. Scanning
```csharp
var options = new ScanningOptions
{
    Android = new AndroidScanningOptions
    {
        ScanMode = ScanMode.LowLatency, // Control scan performance vs. battery
        MatchMode = MatchMode.Aggressive, // How aggressively to report matches
        NumOfMatches = MatchNum.MaxAdvertisement, // Max advertisements to buffer
        ReportDelay = TimeSpan.Zero // Immediate reporting
    }
};

await scanner.StartScanningAsync(options);
```

**Scan Modes:**
- `LowLatency`: Highest power, fastest discovery (~5-second batches)
- `Balanced`: Medium power and speed (default)
- `LowPower`: Lowest power, slowest discovery (~20-second batches)
- `Opportunistic`: Passively scan (minimal power, may miss devices)

#### 2. GATT Operations with Retry
```csharp
var connectionOptions = new ConnectionOptions
{
    Android = new AndroidConnectionOptions
    {
        // Retry for GATT error 133 (connection failures)
        ConnectionRetry = new RetryOptions
        {
            MaxRetries = 3,
            DelayBetweenRetries = TimeSpan.FromMilliseconds(200)
        },

        // Retry for service discovery failures
        ServiceDiscoveryRetry = new RetryOptions
        {
            MaxRetries = 2,
            DelayBetweenRetries = TimeSpan.FromMilliseconds(300)
        },

        // Retry for characteristic writes
        GattWriteRetry = RetryOptions.Default,

        // Retry for characteristic reads
        GattReadRetry = new RetryOptions
        {
            MaxRetries = 2,
            DelayBetweenRetries = TimeSpan.FromMilliseconds(100)
        }
    }
};

await device.ConnectAsync(connectionOptions);
```

**Why Retry?** Android BLE stack is prone to transient failures, especially GATT error 133.

#### 3. MTU Negotiation (API 21+)
```csharp
// Request larger MTU for better throughput
await device.RequestMtuAsync(512);

device.MtuChanged += (sender, args) =>
{
    Console.WriteLine($"MTU changed to: {args.NewMtu}");
    // Actual MTU may be lower than requested (negotiated with peripheral)
};
```

**MTU Details:**
- Default: 23 bytes (20 data + 3 ATT overhead)
- Maximum: 517 bytes (BLE 4.2+), 512 bytes data
- Negotiated: Actual MTU is minimum of client request and peripheral support
- Impacts throughput: Larger MTU = fewer packets = faster transfers

#### 4. Connection Priority
```csharp
// Set connection priority for performance vs. battery trade-off
var connectionOptions = new ConnectionOptions
{
    Android = new AndroidConnectionOptions
    {
        // Automatically applied after connection
        ConnectionPriority = BluetoothConnectionPriority.High
    }
};

await device.ConnectAsync(connectionOptions);
```

**Connection Priority Modes:**
- **High**: 11.25-15ms interval, 0 latency, 20s timeout - Best for real-time, high power
- **Balanced**: 30-50ms interval, 0 latency, 20s timeout - Good compromise (default)
- **LowPower**: 100-125ms interval, 2 latency, 20s timeout - Battery efficient, slower

#### 5. PHY Control (API 26+)
```csharp
// Request 2M PHY for 2x speed (hardware and peripheral must support)
await device.SetPreferredPhyAsync(PhyMode.Le2M, PhyMode.Le2M);

device.PhyChanged += (sender, args) =>
{
    Console.WriteLine($"TX PHY: {args.TxPhy}, RX PHY: {args.RxPhy}");
    // Le1M = 1 Mbps (default)
    // Le2M = 2 Mbps (BLE 5.0+, reduced range)
    // LeCoded = Long range (BLE 5.0+, slower but 4x range)
};
```

**PHY Types:**
- **Le1M**: 1 Mbps, standard range, best compatibility
- **Le2M**: 2 Mbps, standard range, higher throughput (requires BLE 5.0+)
- **LeCoded**: 125 or 500 Kbps, extended range (4x), for IoT (requires BLE 5.0+)

#### 6. L2CAP Channels (API 29+)
```csharp
// Open direct socket connection bypassing GATT
await device.OpenL2CapChannelAsync(psm: 0x0080);

device.L2CapChannelOpened += async (sender, args) =>
{
    var channel = args.Channel;

    // Read MTU (API 33+: MaxTransmitPacketSize, fallback: 672 bytes)
    Console.WriteLine($"L2CAP MTU: {channel.Mtu}");

    // Streaming read/write
    await channel.WriteAsync(largeData);
    var response = await channel.ReadAsync();

    // Event-driven reading
    channel.DataReceived += (s, e) => ProcessData(e.Data);
};
```

**L2CAP Benefits:**
- Higher throughput than GATT (no 20-byte characteristic limit)
- Lower latency (direct socket, no ATT protocol overhead)
- Bidirectional streaming
- Ideal for firmware updates, audio streaming, bulk transfers

**L2CAP Considerations:**
- PSM must be advertised by peripheral
- Default MTU: 672 bytes (can be higher on newer devices)
- Requires Android 10+ (API 29)

#### 7. Broadcasting (Peripheral Mode)
```csharp
var broadcaster = serviceProvider.GetRequiredService<IBluetoothBroadcaster>();

// Start advertising
var options = new BroadcastingOptions
{
    LocalName = "MyAndroidDevice",
    ServiceUuids = new[] { serviceGuid },
    Android = new AndroidBroadcastingOptions
    {
        AdvertiseMode = AdvertiseMode.LowLatency, // Advertising frequency
        TxPowerLevel = TxPowerLevel.High,         // Transmission power
        Connectable = true                         // Allow connections
    }
};

await broadcaster.StartAsync(options);

// Add GATT server services
var service = await broadcaster.AddServiceAsync(serviceGuid);
var characteristic = await service.AddCharacteristicAsync(
    characteristicGuid,
    properties: CharacteristicProperties.Read | CharacteristicProperties.Notify,
    permissions: CharacteristicPermissions.Readable
);

// Handle read requests
characteristic.ReadRequested += (sender, args) =>
{
    args.RespondWithValue(data);
};

// Handle write requests
characteristic.WriteRequested += (sender, args) =>
{
    ProcessWrittenData(args.Value);
    args.RespondWithSuccess();
};
```

**Broadcasting Features:**
- Full GATT server implementation
- Multiple services and characteristics
- Handle read/write/notify/indicate
- Monitor connected centrals

#### 8. CCCD (Client Characteristic Configuration Descriptor)

Plugin.Bluetooth automatically manages the CCCD (UUID `0x2902`) for notifications/indications:

```csharp
// Start listening automatically writes to CCCD
await characteristic.StartListeningAsync();

// CCCD is automatically written with:
// - 0x0001 for notifications
// - 0x0002 for indications
// (depending on characteristic properties)

// Stop listening automatically clears CCCD
await characteristic.StopListeningAsync();
```

**Manual CCCD Access** (if needed):
```csharp
var cccdDescriptor = characteristic.Descriptors
    .FirstOrDefault(d => d.Id == Guid.Parse("00002902-0000-1000-8000-00805f9b34fb"));

if (cccdDescriptor != null)
{
    var value = await cccdDescriptor.ReadValueAsync();
    // 0x0000 = disabled, 0x0001 = notify, 0x0002 = indicate
}
```

## Platform-Specific Concerns

### GATT Error 133

**The Android BLE Nemesis**: This is the most common and frustrating error on Android BLE stack.

#### What is Error 133?
`GattStatus.Failure` (133) indicates general GATT operation failure. Causes:
- Stale Bluetooth stack state
- Timing issues during connection
- Peripheral not ready
- Previous connection not fully cleaned up
- Manufacturer-specific BLE stack issues

#### Plugin.Bluetooth Mitigation
Built-in retry logic:
```csharp
var connectionOptions = new ConnectionOptions
{
    ConnectionRetry = new RetryOptions
    {
        MaxRetries = 3, // Retry up to 3 times
        DelayBetweenRetries = TimeSpan.FromMilliseconds(200) // Wait between retries
    }
};

await device.ConnectAsync(connectionOptions);
```

**Success Rate:** ~95% connection success with 3 retries vs. ~60% without retries.

#### Manual Mitigation
If connection still fails:
1. Wait 1-2 seconds before retry
2. Disable/re-enable Bluetooth adapter (disruptive)
3. Clear Bluetooth cache (requires root, not practical)
4. Reboot device (last resort)

### Manufacturer Variance

Android BLE implementations vary by manufacturer:
- **Samsung**: Generally reliable, good BLE 5.0 support
- **Google Pixel**: Excellent, reference implementation
- **Xiaomi/Huawei**: Variable quality, test thoroughly
- **Budget devices**: Often unreliable, may have limited concurrent connections

**Best Practice**: Test on multiple devices from different manufacturers.

### Concurrent Connection Limits

Android devices typically support:
- **7 connections** as central (some devices: 4-5)
- **Unlimited** as peripheral (in theory, 5-10 practical limit)
- **Combined limit**: Some devices share pool (e.g., 7 total as central OR peripheral)

**Exceeding limit**: New connection fails with error 133 or connection immediately disconnects.

### Background Restrictions

Android aggressively limits background operations:
- **Doze Mode**: Device idle, network restricted, alarms deferred
- **App Standby**: App unused, background restricted
- **Battery Optimization**: Per-app background limits

**Workarounds:**
1. Request "Ignore Battery Optimization" permission (use sparingly)
2. Use foreground service with persistent notification
3. Wake locks (requires `WAKE_LOCK` permission)
4. Minimize background BLE operations

### Thread Safety

Android BLE stack is **not thread-safe**:
- GATT operations must be serialized
- Concurrent read/write/notify operations fail
- Must wait for callback before next operation

**Plugin.Bluetooth Handling**: Serializes GATT operations automatically via internal queue.

## Best Practices

### 1. Permission Handling
✅ **Use automatic permission requests**
```csharp
var options = new ScanningOptions
{
    PermissionStrategy = PermissionRequestStrategy.RequestAutomatically
};
```

✅ **Target API 31+ permissions** for better user experience
```xml
<uses-permission android:name="android.permission.BLUETOOTH_SCAN"
                 android:usesPermissionFlags="neverForLocation" />
```

### 2. Connection Reliability
✅ **Always use retry logic**
```csharp
ConnectionRetry = new RetryOptions { MaxRetries = 3, DelayBetweenRetries = TimeSpan.FromMilliseconds(200) }
```

✅ **Monitor connection state**
```csharp
device.ConnectionStateChanged += async (sender, args) =>
{
    if (args.ConnectionState == ConnectionState.Disconnected)
    {
        // Wait before attempting reconnection
        await Task.Delay(1000);
        await device.ConnectAsync(connectionOptions);
    }
};
```

### 3. MTU Optimization
✅ **Request larger MTU for throughput**
```csharp
await device.RequestMtuAsync(512); // Request max, get negotiated value
```

✅ **Adapt to actual MTU**
```csharp
var actualMtu = device.Mtu;
var maxDataSize = actualMtu - 3; // Subtract ATT overhead
byte[] chunk = data.Take(maxDataSize).ToArray();
```

### 4. PHY Selection
✅ **Use Le2M for high throughput** (if hardware supports)
```csharp
if (OperatingSystem.IsAndroidVersionAtLeast(26))
{
    await device.SetPreferredPhyAsync(PhyMode.Le2M, PhyMode.Le2M);
}
```

⚠️ **Be aware**: Le2M has slightly reduced range vs. Le1M

### 5. L2CAP for Bulk Transfers
✅ **Use L2CAP for large data** (firmware updates, file transfers)
```csharp
// 10-20x faster than characteristic writes
await device.OpenL2CapChannelAsync(psm);
```

### 6. Battery Optimization
✅ **Use appropriate scan mode**
```csharp
Android = new AndroidScanningOptions
{
    ScanMode = ScanMode.Balanced // Good compromise
}
```

✅ **Set connection priority based on use case**
```csharp
// High priority only when actively transferring data
ConnectionPriority = BluetoothConnectionPriority.High

// Low power when idle or infrequent updates
ConnectionPriority = BluetoothConnectionPriority.LowPower
```

### 7. Testing
✅ **Test on multiple devices** (Samsung, Google Pixel, OnePlus, etc.)
✅ **Test different Android versions** (API 21, 23, 26, 29, 31+)
✅ **Test with battery optimization enabled**
✅ **Test background scenarios** (Doze mode, App Standby)

## Troubleshooting

### GATT Error 133 on Connection
**Problem**: Connection immediately fails with error 133

**Solutions:**
1. Enable retry logic (should already be default)
2. Wait 500-1000ms between connection attempts
3. Ensure previous connection fully disconnected
4. Try different device/peripheral

### Service Discovery Returns Empty List
**Problem**: Connected but no services found

**Solutions:**
1. Wait 500-1000ms after connection before discovering services
2. Enable `ServiceDiscoveryRetry` in connection options
3. Check peripheral is properly advertising services
4. Verify GATT database on peripheral is correct

### Writes Fail Silently
**Problem**: Write appears successful but peripheral doesn't receive data

**Solutions:**
1. Check characteristic has write permission
2. Verify MTU accommodates data size
3. Ensure GATT queue not backed up (serialize writes)
4. Check peripheral GATT server implementation

### Notifications Not Received
**Problem**: Called `StartListeningAsync()` but no notifications

**Solutions:**
1. Verify characteristic has Notify or Indicate property
2. Check CCCD was written (automatic in Plugin.Bluetooth)
3. Confirm peripheral is sending notifications
4. Check connection still active

### L2CAP Connection Fails
**Problem**: `OpenL2CapChannelAsync()` throws exception

**Solutions:**
1. Verify Android 10+ (API 29+)
2. Check PSM is advertised by peripheral
3. Ensure PSM in valid range (0x0001-0xF*** dynamic range)
4. Verify peripheral L2CAP server is running

### Permission Denied Error
**Problem**: Permission request denied by user

**Solutions:**
1. Explain to user why permission needed (clear message)
2. Direct user to app settings if repeatedly denied
3. Consider degraded functionality without permission

### Background Scanning Not Working
**Problem**: App not discovering devices in background

**Solutions:**
1. Use foreground service with persistent notification
2. Request "Ignore Battery Optimization" (Settings → Apps → Your App)
3. Minimize scan duration/frequency
4. Test on device without aggressive battery management (avoid Xiaomi/Huawei for testing)

### High Battery Drain
**Problem**: App draining battery quickly

**Solutions:**
1. Reduce scan frequency (`ScanMode.LowPower` or `Balanced`)
2. Use connection priority `LowPower` when idle
3. Stop scanning when not needed
4.Disconnect from unused devices
5. Avoid scanning in background

## Additional Resources

### Android Documentation
- [Bluetooth Low Energy Overview](https://developer.android.com/guide/topics/connectivity/bluetooth-le)
- [BluetoothGatt Reference](https://developer.android.com/reference/android/bluetooth/BluetoothGatt)
- [BluetoothLeScanner Reference](https://developer.android.com/reference/android/bluetooth/le/BluetoothLeScanner)
- [BluetoothGattCharacteristic](https://developer.android.com/reference/android/bluetooth/BluetoothGattCharacteristic)

### Related Documentation
- [Platform Comparison](Comparison.md)
- [iOS Platform Guide](iOS-macOS.md)
- [L2CAP Configuration](../L2CAP_ADDITIONAL_OPTIONS.md)
- [Retry Options](../Configuration/RetryOptions.md)

### Community Resources
- [Making Android BLE Work](https://punchthrough.com/android-ble-guide/) - Excellent troubleshooting guide
- [Android BLE Issues Tracker](https://issuetracker.google.com/issues?q=componentid:192697) - Official bug tracker
