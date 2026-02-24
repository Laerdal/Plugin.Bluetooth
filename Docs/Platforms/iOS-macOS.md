# iOS & macOS Platform Guide

Comprehensive guide for Plugin.Bluetooth on iOS and macOS platforms using CoreBluetooth framework.

## Table of Contents
- [Overview](#overview)
- [Requirements](#requirements)
- [CoreBluetooth Framework](#corebluetooth-framework)
- [Configuration](#configuration)
- [Feature Support](#feature-support)
- [Platform Limitations](#platform-limitations)
- [Best Practices](#best-practices)
- [Troubleshooting](#troubleshooting)

## Overview

iOS and macOS use Apple's **CoreBluetooth** framework, which provides a native, high-level abstraction over Bluetooth Low Energy operations. Plugin.Bluetooth wraps CoreBluetooth to provide a consistent cross-platform API while respecting Apple's platform conventions.

### Key Characteristics
- **System-Managed**: Many operations (MTU, PHY, connection priority) are automatically optimized by iOS/macOS
- **Stable & Reliable**: CoreBluetooth is mature and well-tested
- **Permissions Required**: Explicit Info.plist entries required for all Bluetooth operations
- **Background Support**: Limited background execution capabilities

### Supported Platforms
- **iOS**: 11.0+ (L2CAP support), 13.0+ (enhanced features)
- **macOS**: 10.13+ (High Sierra and later)
- **Mac Catalyst**: 13.0+
- **tvOS**: 11.0+

## Requirements

### Minimum Versions
- **iOS**: 11.0 or later (for L2CAP support)
- **macOS**: 10.13 (High Sierra) or later
- **Xcode**: 13.0 or later
- **.NET**: .NET 8.0+

### Required Frameworks
- `CoreBluetooth.framework` (automatically linked)
- `Foundation.framework` (automatically linked)

## CoreBluetooth Framework

### Architecture Overview
Plugin.Bluetooth maps to CoreBluetooth as follows:

| Plugin.Bluetooth | CoreBluetooth | Description |
|------------------|---------------|-------------|
| `IBluetoothScanner` | `CBCentralManager` | Central role for scanning and connecting |
| `IBluetoothRemoteDevice` | `CBPeripheral` | Represents a remote BLE device |
| `IBluetoothRemoteService` | `CBService` | GATT service on remote device |
| `IBluetoothRemoteCharacteristic` | `CBCharacteristic` | GATT characteristic |
| `IBluetoothRemoteDescriptor` | `CBDescriptor` | GATT descriptor |
| `IBluetoothBroadcaster` | `CBPeripheralManager` | Peripheral role for advertising |
| `IBluetoothRemoteL2CapChannel` | `CBL2CAPChannel` | Direct L2CAP socket connection |

### Implementation Details
The iOS/macOS implementation uses delegates and event-driven patterns:
- `CbCentralManagerWrapper` - Wraps `CBCentralManager` with delegate callbacks
- `CbPeripheralWrapper` - Wraps `CBPeripheral` with typed delegate methods
- `CbPeripheralManagerWrapper` - Wraps `CBPeripheralManager` for broadcasting
- Main thread dispatching for CoreBluetooth operations (iOS requirement)

## Configuration

### Info.plist Entries

**Required** - You must add these entries to your `Info.plist` file. Without them, your app will crash on first Bluetooth access.

#### Central Mode (Scanning & Connecting)
```xml
<!-- iOS 13+ Required -->
<key>NSBluetoothAlwaysUsageDescription</key>
<string>This app needs Bluetooth to communicate with BLE devices</string>

<!-- iOS 12 and earlier (deprecated but still needed for older iOS) -->
<key>NSBluetoothPeripheralUsageDescription</key>
<string>This app needs Bluetooth to communicate with BLE devices</string>
```

#### Peripheral Mode (Broadcasting)
```xml
<key>NSBluetoothAlwaysUsageDescription</key>
<string>This app needs Bluetooth to advertise as a peripheral device</string>
```

#### Background Modes (Optional)
If you need background BLE operations, add to `Info.plist`:
```xml
<key>UIBackgroundModes</key>
<array>
    <string>bluetooth-central</string>    <!-- For scanning/connecting in background -->
    <string>bluetooth-peripheral</string> <!-- For broadcasting in background -->
</array>
```

### Capabilities in Xcode
1. Open your project in Xcode
2. Select your target
3. Go to "Signing & Capabilities"
4. Add "Background Modes" capability if needed
5. Check "Uses Bluetooth LE accessories" or "Acts as a Bluetooth LE accessory"

### macOS Considerations
On macOS (including Mac Catalyst):
- Requires user approval on first Bluetooth access
- System shows permission dialog automatically
- No Info.plist entries needed for macOS apps (but use them for Mac Catalyst)

## Feature Support

### ✅ Fully Supported Features

#### 1. Scanning
```csharp
var options = new ScanningOptions
{
    // Apple-specific: scan for specific services
    Apple = new AppleScanningOptions
    {
        // Only devices advertising these services will be discovered
        ServiceUuids = new[] { serviceGuid }
    }
};

await scanner.StartScanningAsync(options);
```

**Capabilities:**
- Service UUID filtering (recommended for battery efficiency)
- Advertisement data parsing (ServiceUUIDs, LocalName, ManufacturerData)
- RSSI reading during scan

#### 2. Connection
```csharp
var connectionOptions = new ConnectionOptions
{
    Apple = new AppleConnectionOptions
    {
        NotifyOnConnection = true,      // Alert when device connects (background)
        NotifyOnDisconnection = true,   // Alert when device disconnects
        NotifyOnNotification = true,    // Alert on incoming notifications
        EnableTransportBridging = false, // iOS 13+: Dual-mode Classic+LE
        RequiresAncs = false            // iOS 13+: Require Apple Notification Center Service
    }
};

await device.ConnectAsync(connectionOptions);
```

**Connection Options Explained:**
- `NotifyOnConnection`: Shows alert when peripheral connects while app is suspended
- `NotifyOnDisconnection`: Shows alert on unexpected disconnection
- `NotifyOnNotification`: Shows alerts for characteristic notifications
- `EnableTransportBridging`: Allow dual-mode Bluetooth (Classic + LE)
- `RequiresAncs`: Connection only succeeds if peripheral supports ANCS

#### 3. GATT Operations
All standard GATT operations are fully supported:
- Service discovery: `await device.ExploreServicesAsync()`
- Characteristic discovery: `await service.ExploreCharacteristicsAsync()`
- Descriptor discovery: `await characteristic.ExploreDescriptorsAsync()`
- Read: `await characteristic.ReadValueAsync()`
- Write: `await characteristic.WriteValueAsync(data)`
- Write without response: Automatically uses if characteristic supports it
- Notifications: `await characteristic.StartListeningAsync()`
- Indications: Automatically handled same as notifications

#### 4. MTU (Automatic)
```csharp
// Get current MTU (read-only)
var currentMtu = device.Mtu;

// MTU cannot be requested - iOS negotiates automatically
// await device.RequestMtuAsync(512); // No-op on iOS
```

**iOS MTU Behavior:**
- Automatically negotiated during connection
- Typically 185 bytes for iPhone 8 and later
- Up to 512 bytes on newer devices (iPhone 12+)
- Cannot be changed programmatically
- Read via `device.Mtu` property after connection

#### 5. PHY (Physical Layer) - Read Only
```csharp
// System automatically selects optimal PHY
// Read current PHY after connection
device.PhyChanged += (sender, args) =>
{
    Console.WriteLine($"TX PHY: {args.TxPhy}"); // e.g., Le1M, Le2M
    Console.WriteLine($"RX PHY: {args.RxPhy}");
};
```

**iOS PHY Behavior:**
- Automatically selected by system
- iOS 12+: Usually Le1M (1 Mbps)
- iOS/devices supporting Bluetooth 5.0: May use Le2M (2 Mbps)
- Cannot be set programmatically
- Available via `PhyChanged` event after connection

#### 6. L2CAP Channels (iOS 11+)
```csharp
// Open L2CAP channel for direct socket communication
await device.OpenL2CapChannelAsync(psm: 0x0080); // PSM must be advertised by peripheral

device.L2CapChannelOpened += async (sender, args) =>
{
    var channel = args.Channel;

    // Read current MTU for L2CAP channel
    Console.WriteLine($"L2CAP MTU: {channel.Mtu}"); // Default: 672 bytes

    // Write data directly to channel
    await channel.WriteAsync(data);

    // Read data from channel
    var receivedData = await channel.ReadAsync();

    // Subscribe to incoming data
    channel.DataReceived += (s, e) => ProcessData(e.Data);
};
```

**L2CAP Capabilities:**
- Available on iOS 11.0+
- PSM (Protocol/Service Multiplexer) must be advertised by peripheral
- Higher throughput than GATT characteristics
- Lower latency
- Default MTU: 672 bytes (minimum guaranteed by Bluetooth spec)
- MTU cannot be changed on iOS (see [L2CAP_ADDITIONAL_OPTIONS.md](../L2CAP_ADDITIONAL_OPTIONS.md))

#### 7. RSSI (Signal Strength)
```csharp
// Read RSSI during scanning (from advertisement)
scanner.DeviceListChanged += (sender, args) =>
{
    foreach (var device in scanner.Devices)
    {
        Console.WriteLine($"RSSI: {device.SignalStrengthDbm} dBm");
    }
};

// Read RSSI after connection
await device.ReadSignalStrengthAsync();
device.SignalStrengthRead += (sender, args) =>
{
    Console.WriteLine($"Connected RSSI: {args.SignalStrengthDbm} dBm");
};
```

#### 8. Broadcasting (Peripheral Mode)
```csharp
var broadcaster = serviceProvider.GetRequiredService<IBluetoothBroadcaster>();

// Configure advertisement data
var options = new BroadcastingOptions
{
    LocalName = "MyDevice",
    ServiceUuids = new[] { serviceGuid }
};

// Start advertising
await broadcaster.StartAsync(options);

// Add GATT services
var service = await broadcaster.AddServiceAsync(serviceGuid);
var characteristic = await service.AddCharacteristicAsync(
    characteristicGuid,
    properties: CharacteristicProperties.Read | CharacteristicProperties.Notify,
    permissions: CharacteristicPermissions.Readable
);

// Handle read requests from centrals
characteristic.ReadRequested += (sender, args) =>
{
    args.RespondWithValue(data);
};
```

**Broadcasting Features:**
- Full GATT server implementation
- Support for multiple services and characteristics
- Read/Write/Notify/Indicate operations
- Handle incoming connections from central devices
- Monitor connected centrals via `ClientDeviceConnected` event

### ⚠️ Automatic/Limited Features

#### Connection Priority
```csharp
// No-op on iOS - system manages connection parameters automatically
await device.RequestConnectionPriorityAsync(BluetoothConnectionPriority.High);
```

iOS automatically adjusts connection parameters based on:
- Current power state (Low Power Mode affects this)
- App state (foreground vs. background)
- System load
- Peripheral requirements

**Typical Connection Intervals:**
- **Active transfers**: 15-30ms (high throughput)
- **Idle connection**: 100-200ms (power saving)
- **Background**: 1-2 seconds (significant battery savings)

### ❌ Not Supported

None. All major BLE features are supported on iOS/macOS, though some are automatic rather than controllable.

## Platform Limitations

### 1. System-Managed Parameters

**Impact**: Cannot fine-tune performance
- MTU is negotiated automatically
- PHY is selected automatically
- Connection parameters adjust dynamically
- No control over connection interval, slave latency, or supervision timeout

**Workaround**: Trust iOS to optimize. In practice, iOS does an excellent job of balancing performance and battery.

### 2. Background Scanning Limitations

```csharp
// Background scanning has restrictions:
// - Only finds devices advertising specific services
// - Slower scan rate (every 10-30 seconds vs. continuous)
// - Cannot use manufacturer data filtering
```

**Background Scanning Rules:**
1. Must specify service UUIDs in scan options
2. Scan operates on slower duty cycle
3. Advertisement data may be coalesced/delayed
4. CoreBluetooth handles background automatically

**Best Practice**: Use service UUID filtering even in foreground for better battery life.

### 3. Main Thread Requirements

CoreBluetooth requires many operations on main thread:
```csharp
// Plugin.Bluetooth handles this automatically via MainThreadDispatcher
// You don't need to worry about threading
```

Internal implementation uses `MainThreadDispatcher.BeginInvokeOnMainThread()` for all CoreBluetooth calls.

### 4. Peripheral Caching

iOS caches peripheral information:
- Services/characteristics discovered once are cached
- Cache persists across connections
- May cause stale data if peripheral GATT database changes

**Workaround**: Not exposed in CoreBluetooth. Re-discovery doesn't help. Peripheral must change its GATT database version (advanced).

### 5. Write Without Response Queue Limits

```csharp
// iOS has internal queue for write-without-response
// Writing too fast may fail silently

characteristic.WriteValueAsync(data); // May queue

// Wait for ready signal before next write
await device.WaitForReadyToSendWriteWithoutResponseAsync(timeout, cancellationToken);
```

iOS notifies via `IsReadyToSendWriteWithoutResponse` when ready for next write-without-response.

## Best Practices

### 1. Info.plist Configuration
✅ **Always add required entries BEFORE first run**
```xml
<key>NSBluetoothAlwaysUsageDescription</key>
<string>Clear description of why you need Bluetooth</string>
```

❌ **Don't forget this** - App will crash on first Bluetooth access without these entries.

### 2. Service UUID Filtering
✅ **Use service filtering for better battery life**
```csharp
var options = new ScanningOptions
{
    Apple = new AppleScanningOptions
    {
        ServiceUuids = new[] { myServiceGuid } // Reduces power consumption
    }
};
```

### 3. Background Operation
✅ **Minimal operations in background**
- Keep connections alive with periodic reads/writes
- Use notifications/indications instead of polling
- Batch operations where possible

❌ **Don't scan continuously in background** - OS will throttle and drain battery

### 4. Connection Reliability
✅ **Monitor connection state changes**
```csharp
device.ConnectionStateChanged += (sender, args) =>
{
    if (args.ConnectionState == ConnectionState.Disconnected)
    {
        // Handle unexpected disconnection
        // Attempt reconnection if needed
    }
};
```

### 5. Error Handling
```csharp
try
{
    await device.ConnectAsync(options);
}
catch (DeviceFailedToConnectException ex)
{
    // Log and retry
    // iOS error details in ex.NativeError
}
```

### 6. Test on Physical Devices
⚠️ **iOS Simulator limitations:**
- No actual Bluetooth hardware
- Cannot test real BLE operations
- Dummy implementations only

**Always test on real iPhone/iPad/Mac with Bluetooth enabled.**

### 7. Low Energy Mode Considerations
iOS Low Energy Mode affects BLE:
- Slower scan rates
- Longer connection intervals
- Background operations may be suspended

Test your app with Low Energy Mode enabled.

### 8. L2CAP Channel Management
```csharp
// Always close L2CAP channels when done
await channel.CloseAsync();
await channel.DisposeAsync();

// Monitor channel closure
channel.Closed += (sender, args) =>
{
    // Peripheral closed channel or connection lost
};
```

## Troubleshooting

### App Crashes on First Bluetooth Access
**Problem**: Missing Info.plist entries

**Solution**:
```xml
<key>NSBluetoothAlwaysUsageDescription</key>
<string>Your description here</string>
<key>NSBluetoothPeripheralUsageDescription</key>
<string>Your description here</string>
```

### Can't Find Device During Scan
**Possible Causes:**
1. Device not advertising
2. Device advertising services not in scan filter
3. Bluetooth disabled on iOS device
4. Background scanning without service UUID filter

**Solution**:
- Verify device is advertising
- Check service UUIDs match
- Enable Bluetooth in Settings
- Add service UUID filter for background scanning

### Connection Immediately Disconnects
**Possible Causes:**
1. Device out of range
2. Device rejected connection (pairing required, etc.)
3. iOS Low Energy Mode active
4. Too many concurrent connections

**Solution**:
- Check signal strength (RSSI > -85 dBm recommended)
- Verify peripheral accepts connections
- Test without Low Energy Mode
- Limit to 7-10 concurrent connections

### MTU Lower Than Expected
**Problem**: iOS negotiated smaller MTU than desired

**Reality**:
- MTU depends on iOS version and device hardware
- iPhone 8+: Typically 185 bytes
- iPhone 12+: Up to 512 bytes
- Cannot be forced higher

**Solution**: Design for minimum 23-byte MTU, scale up as available

### Write Without Response Fails
**Problem**: Queue full on iOS

**Solution**: Use `WaitForReadyToSendWriteWithoutResponseAsync()`
```csharp
await device.WaitForReadyToSendWriteWithoutResponseAsync(
    timeout: TimeSpan.FromSeconds(5),
    cancellationToken
);
await characteristic.WriteValueAsync(nextPacket);
```

### L2CAP Connection Fails
**Possible Causes:**
1. iOS version < 11.0
2. PSM not advertised by peripheral
3. PSM reserved for system use

**Solution**:
- Check iOS version
- Verify peripheral advertises PSM in GATT or advertisement
- Use PSM in dynamic range (0x0080 - 0x00FF recommended)

### Background Scanning Not Working
**Problem**: Not finding devices in background

**Requirements:**
1. Add `bluetooth-central` to UIBackgroundModes
2. Specify service UUIDs in scan options
3. User granted "Always" permission

**Solution**:
```csharp
var options = new ScanningOptions
{
    Apple = new AppleScanningOptions
    {
        ServiceUuids = new[] { serviceGuid } // Required for background
    }
};
```

### Permission Denied Error
**Problem**: User denied Bluetooth permission

**Solution**:
```csharp
scanner.StateChanged += (sender, args) =>
{
    if (args.NewState == AdapterState.Unauthorized)
    {
        // Show UI directing user to Settings > Your App > Bluetooth
        // Cannot request permission again programmatically
    }
};
```

User must manually enable in Settings app.

## Additional Resources

### Apple Documentation
- [CoreBluetooth Programming Guide](https://developer.apple.com/library/archive/documentation/NetworkingInternetWeb/Conceptual/CoreBluetooth_concepts/)
- [CBCentralManager Reference](https://developer.apple.com/documentation/corebluetooth/cbcentralmanager)
- [CBPeripheral Reference](https://developer.apple.com/documentation/corebluetooth/cbperipheral)
- [CBPeripheralManager Reference](https://developer.apple.com/documentation/corebluetooth/cbperipheralmanager)
- [CBL2CAPChannel Reference](https://developer.apple.com/documentation/corebluetooth/cbl2capchannel)

### WWDC Sessions
- [WWDC 2017: What's New in Core Bluetooth](https://developer.apple.com/videos/play/wwdc2017/712/)
- [WWDC 2019: What's New in Core Bluetooth](https://developer.apple.com/videos/play/wwdc2019/901/)

### Related Documentation
- [Platform Comparison](Comparison.md)
- [Android Platform Guide](Android.md)
- [L2CAP Configuration](../L2CAP_ADDITIONAL_OPTIONS.md)
