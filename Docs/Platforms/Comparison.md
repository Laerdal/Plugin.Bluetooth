# Platform Comparison

Comprehensive side-by-side comparison of Plugin.Bluetooth features across iOS/macOS, Android, and Windows platforms.

## Table of Contents
- [Quick Reference](#quick-reference)
- [Detailed Feature Comparison](#detailed-feature-comparison)
- [Platform Trade-offs](#platform-trade-offs)
- [Migration Considerations](#migration-considerations)
- [Best Platform for Your Use Case](#best-platform-for-your-use-case)

## Quick Reference

### Feature Support Matrix

| Feature | iOS/macOS | Android | Windows | Winner |
|---------|-----------|---------|---------|--------|
| **Core Operations** |
| Scanning | ✅ | ✅ | ✅ | 🟰 Tie |
| Connection | ✅ | ✅ | ✅ | 🟰 Tie |
| GATT Read/Write | ✅ | ✅ | ✅ | 🟰 Tie |
| Notifications/Indications | ✅ | ✅ | ✅ | 🟰 Tie |
| Service Discovery | ✅ | ✅ | ✅ | 🟰 Tie |
| Descriptor Operations | ✅ | ✅ | ✅ | 🟰 Tie |
| **Advanced Features** |
| MTU Negotiation | ⚠️ Auto | ✅ Manual | ⚠️ Auto | 🏆 Android |
| MTU Reading | ✅ | ✅ | ✅ | 🟰 Tie |
| Connection Priority | ⚠️ Auto | ✅ Manual | ⚠️ Auto | 🏆 Android |
| PHY Configuration | ⚠️ Auto | ✅ API 26+ | ❌ | 🏆 Android |
| PHY Reading | ✅ | ✅ API 26+ | ❌ | 🟰 iOS/Android |
| L2CAP Channels | ✅ iOS 11+ | ✅ API 29+ | ❌ | 🟰 iOS/Android |
| RSSI (Scanning) | ✅ | ✅ | ✅ | 🟰 Tie |
| RSSI (Connected) | ✅ | ✅ | ❌ | 🏆 iOS/Android |
| Broadcasting (Peripheral) | ✅ | ✅ | ❌ | 🟰 iOS/Android |
| **Developer Experience** |
| Stability | 🟢 Excellent | 🟡 Good | 🟢 Good | 🏆 iOS |
| Configuration Complexity | 🟡 Medium | 🔴 High | 🟢 Low | 🏆 Windows |
| Permission Management | 🟡 Info.plist | 🔴 Complex | 🟢 Simple | 🏆 Windows |
| Error Handling | 🟢 Predictable | 🔴 GATT 133 | 🟢 Straightforward | 🏆 iOS/Windows |
| Cross-Device Consistency | 🟢 Excellent | 🟡 Variable | 🟢 Good | 🏆 iOS |
| Background Support | 🟡 Limited | 🟡 Limited | 🔴 Minimal | 🏆 iOS/Android |

### Legend
- ✅ **Fully Supported** - Feature available and controllable
- ⚠️ **Automatic** - Feature exists but system-managed
- ❌ **Not Supported** - Feature unavailable
- 🟢 **Excellent** - Best in class
- 🟡 **Good** - Adequate with caveats
- 🔴 **Challenging** - Requires significant effort

## Detailed Feature Comparison

### 1. Scanning & Discovery

| Aspect | iOS/macOS | Android | Windows |
|--------|-----------|---------|---------|
| **Start/Stop** | ✅ | ✅ | ✅ |
| **Service UUID Filtering** | ✅ Recommended | ✅ Optional | ✅ Optional |
| **Manufacturer Data** | ✅ | ✅ | ✅ |
| **RSSI During Scan** | ✅ | ✅ | ✅ |
| **Scan Mode Control** | ⚠️ Auto | ✅ Manual | ⚠️ Auto |
| **Background Scanning** | ⚠️ Limited | ⚠️ Limited | ❌ Minimal |
| **Duplicate Filtering** | ⚠️ Auto | ✅ Configurable | ⚠️ Auto |
| **Battery Impact** | 🟢 Low | 🟡 Configurable | 🟢 Low |

**iOS/macOS Details:**
```csharp
var options = new ScanningOptions
{
    Apple = new AppleScanningOptions
    {
        ServiceUuids = new[] { serviceGuid } // Highly recommended for battery
    }
};
```
- System-managed scan intervals
- Background scan requires service UUID filter
- Excellent power efficiency

**Android Details:**
```csharp
var options = new ScanningOptions
{
    Android = new AndroidScanningOptions
    {
        ScanMode = ScanMode.LowLatency,      // Control frequency
        MatchMode = MatchMode.Aggressive,     // Match reporting
        NumOfMatches = MatchNum.MaxAdvertisement
    }
};
```
- Full control over scan parameters
- Trade-off between speed and battery
- API level affects behavior

**Windows Details:**
- Simple start/stop, no advanced configuration
- Continuous scanning when active
- Lower power than Android LowLatency mode

### 2. Connection Management

| Aspect | iOS/macOS | Android | Windows |
|--------|-----------|---------|---------|
| **Basic Connect/Disconnect** | ✅ | ✅ | ✅ |
| **Connection Timeout** | ✅ | ✅ | ✅ |
| **Auto-Reconnect** | ⚠️ Manual | ✅ Configurable | ⚠️ Manual |
| **Connection Priority** | ⚠️ Auto | ✅ Manual | ⚠️ Auto |
| **Concurrent Connections** | ~7-10 | ~7 (varies) | ~7 |
| **Retry Logic** | ✅ | ✅ GATT 133 | ✅ |
| **Connection Options** | 🟡 Limited | 🟢 Extensive | 🔴 None |

**iOS/macOS Connection:**
```csharp
var options = new ConnectionOptions
{
    Apple = new AppleConnectionOptions
    {
        NotifyOnConnection = true,
        NotifyOnDisconnection = true,
        EnableTransportBridging = false,
        RequiresAncs = false
    }
};
```
- System optimizes connection parameters
- Best for "just works" scenarios
- Background notifications available

**Android Connection:**
```csharp
var options = new ConnectionOptions
{
    ConnectionRetry = RetryOptions.Default, // Critical for GATT 133
    Android = new AndroidConnectionOptions
    {
        ConnectionPriority = BluetoothConnectionPriority.High,
        AutoConnect = false,
        TransportType = BluetoothTransportType.Le,
        ServiceDiscoveryRetry = new RetryOptions { MaxRetries = 2 }
    }
};
```
- Full control over all parameters
- Retry logic essential
- Performance vs. battery trade-offs

**Windows Connection:**
```csharp
var options = new ConnectionOptions
{
    Windows = new WindowsConnectionOptions()
    // No platform-specific options currently
};
```
- Simple, automatic connection
- `GattSession` maintains connection
- Reliable once connected

### 3. MTU (Maximum Transmission Unit)

| Aspect | iOS/macOS | Android | Windows |
|--------|-----------|---------|---------|
| **Request MTU** | ❌ Auto | ✅ API 21+ | ❌ Auto |
| **Read MTU** | ✅ | ✅ | ✅ |
| **Typical MTU** | 185-512 | 23-512 | 23-512 |
| **Maximum MTU** | 512 | 517 | 512 |
| **MTU Change Event** | ✅ | ✅ | ✅ |

**Comparison:**
```csharp
// iOS/macOS: Cannot request, only read
var iosMtu = device.Mtu; // e.g., 185 on iPhone 8+, 512 on iPhone 12+

// Android: Can request specific value
await device.RequestMtuAsync(512); // Negotiates with peripheral
device.MtuChanged += (s, e) => Console.WriteLine($"MTU: {e.NewMtu}");

// Windows: Cannot request, only read
var winMtu = device.Mtu; // System-negotiated
```

**Impact:**
- **Android advantage**: Optimize for firmware updates, bulk transfers
- **iOS/Windows**: Accept what system negotiates (usually adequate)
- **Best Practice**: Design for 23-byte minimum, scale to available MTU

### 4. PHY (Physical Layer)

| Aspect | iOS/macOS | Android | Windows |
|--------|-----------|---------|---------|
| **Set Preferred PHY** | ❌ Auto | ✅ API 26+ | ❌ |
| **Read Current PHY** | ✅ | ✅ API 26+ | ❌ |
| **Le1M (1 Mbps)** | ✅ | ✅ | ✅ |
| **Le2M (2 Mbps)** | ✅ Auto | ✅ API 26+ | ⚠️ Unknown |
| **LeCoded (Long Range)** | ✅ Auto | ✅ API 26+ | ❌ |
| **PHY Change Event** | ✅ | ✅ API 26+ | ❌ |

**Comparison:**
```csharp
// iOS/macOS: System selects, can read after connection
device.PhyChanged += (s, e) =>
{
    Console.WriteLine($"TX: {e.TxPhy}, RX: {e.RxPhy}");
    // Typically Le1M, Le2M on BLE 5.0 devices
};

// Android API 26+: Full control
await device.SetPreferredPhyAsync(PhyMode.Le2M, PhyMode.Le2M);
device.PhyChanged += (s, e) =>
{
    Console.WriteLine($"TX: {e.TxPhy}, RX: {e.RxPhy}");
};

// Windows: No support
// Cannot read or set PHY
```

**Use Cases:**
- **Le1M**: Default, best compatibility, standard range
- **Le2M**: 2x speed, same range (high throughput)
- **LeCoded**: 4x range, slower (IoT, outdoor)

**Android advantage**: Control PHY for specific scenarios (speed vs. range)

### 5. L2CAP Channels

| Aspect | iOS/macOS | Android | Windows |
|--------|-----------|---------|---------|
| **Supported** | ✅ iOS 11+ | ✅ API 29+ | ❌ |
| **Open Channel** | ✅ | ✅ | ❌ |
| **Read/Write** | ✅ | ✅ | ❌ |
| **MTU** | 672 (fixed) | 672+ (readable) | N/A |
| **Event-Driven Reading** | ✅ | ✅ | N/A |
| **Use Cases** | Bulk transfers | Bulk transfers | N/A |

**Comparison:**
```csharp
// iOS/macOS: iOS 11+
await device.OpenL2CapChannelAsync(psm: 0x0080);
device.L2CapChannelOpened += async (s, e) =>
{
    var channel = e.Channel;
    Console.WriteLine($"MTU: {channel.Mtu}"); // 672 bytes
    await channel.WriteAsync(data);
};

// Android: API 29+ (Android 10)
await device.OpenL2CapChannelAsync(psm: 0x0080);
device.L2CapChannelOpened += async (s, e) =>
{
    var channel = e.Channel;
    // API 33+: MaxTransmitPacketSize available
    Console.WriteLine($"MTU: {channel.Mtu}");
    await channel.WriteAsync(data);
};

// Windows: Not supported
// Must use GATT characteristics for all data transfer
```

**Benefits of L2CAP:**
- 10-20x faster than GATT characteristics for bulk data
- Lower latency (direct socket, no ATT/GATT overhead)
- Ideal for firmware updates, file transfers, audio streaming

**Windows Workaround:** Batch GATT writes with optimized MTU

### 6. RSSI (Signal Strength)

| Aspect | iOS/macOS | Android | Windows |
|--------|-----------|---------|---------|
| **During Scanning** | ✅ | ✅ | ✅ |
| **After Connection** | ✅ | ✅ | ❌ |
| **Polling** | ✅ Manual | ✅ Manual | N/A |
| **Event-Driven** | ✅ | ✅ | N/A |
| **Use Cases** | Range estimation | Range estimation | Limited |

**Comparison:**
```csharp
// During scanning (all platforms)
scanner.DeviceListChanged += (s, e) =>
{
    foreach (var device in scanner.Devices)
    {
        Console.WriteLine($"RSSI: {device.SignalStrengthDbm} dBm");
    }
};

// After connection (iOS/macOS and Android only)
await device.ReadSignalStrengthAsync();
device.SignalStrengthRead += (s, e) =>
{
    Console.WriteLine($"RSSI: {e.SignalStrengthDbm} dBm");
};

// Windows: RSSI not available after connection
// Must disconnect and re-scan to get updated RSSI
```

**Windows Limitation:** Cannot monitor connection quality via RSSI. Must rely on:
- Connection state changes
- GATT operation success/failure
- Timeout monitoring

### 7. Broadcasting (Peripheral Mode)

| Aspect | iOS/macOS | Android | Windows |
|--------|-----------|---------|---------|
| **Supported** | ✅ | ✅ API 21+ | ❌ |
| **Start/Stop Advertising** | ✅ | ✅ | ❌ |
| **GATT Server** | ✅ | ✅ | ❌ |
| **Multiple Services** | ✅ | ✅ | ❌ |
| **Custom Characteristics** | ✅ | ✅ | ❌ |
| **Handle Reads/Writes** | ✅ | ✅ | ❌ |
| **Client Notifications** | ✅ | ✅ | ❌ |

**iOS/macOS Broadcasting:**
```csharp
var broadcaster = services.GetRequiredService<IBluetoothBroadcaster>();

var options = new BroadcastingOptions
{
    LocalName = "MyiOSDevice",
    ServiceUuids = new[] { serviceGuid }
};

await broadcaster.StartAsync(options);

// Add GATT services
var service = await broadcaster.AddServiceAsync(serviceGuid);
var characteristic = await service.AddCharacteristicAsync(
    characteristicGuid,
    CharacteristicProperties.Read | CharacteristicProperties.Notify,
    CharacteristicPermissions.Readable
);
```

**Android Broadcasting:**
```csharp
var broadcaster = services.GetRequiredService<IBluetoothBroadcaster>();

var options = new BroadcastingOptions
{
    LocalName = "MyAndroidDevice",
    ServiceUuids = new[] { serviceGuid },
    Android = new AndroidBroadcastingOptions
    {
        AdvertiseMode = AdvertiseMode.LowLatency,
        TxPowerLevel = TxPowerLevel.High,
        Connectable = true
    }
};

await broadcaster.StartAsync(options);
// Add services similarly to iOS
```

**Windows:** Not supported. GATT server APIs exist in WinRT but not implemented in Plugin.Bluetooth.

### 8. Permission Management

| Aspect | iOS/macOS | Android | Windows |
|--------|-----------|---------|---------|
| **Complexity** | 🟡 Medium | 🔴 High | 🟢 Low |
| **Configuration File** | Info.plist | AndroidManifest.xml | Package.appxmanifest |
| **Runtime Requests** | ⚠️ Auto | ⚠️ Auto | ⚠️ Auto |
| **Location Permission** | ❌ Not required | ⚠️ API 23-30 | ❌ Not required |
| **API Level Variance** | 🟢 Consistent | 🔴 High | 🟢 Consistent |

**iOS/macOS:**
```xml
<!-- Required in Info.plist -->
<key>NSBluetoothAlwaysUsageDescription</key>
<string>Why you need Bluetooth</string>
```
- Simple, consistent
- Two entries (Always, Peripheral)
- User prompt on first access

**Android:**
```xml
<!-- API 31+ (Android 12+) -->
<uses-permission android:name="android.permission.BLUETOOTH_SCAN"
                 android:usesPermissionFlags="neverForLocation" />
<uses-permission android:name="android.permission.BLUETOOTH_CONNECT" />
<uses-permission android:name="android.permission.BLUETOOTH_ADVERTISE" />

<!-- API 23-30 (Android 6-10) -->
<uses-permission android:name="android.permission.BLUETOOTH" />
<uses-permission android:name="android.permission.BLUETOOTH_ADMIN" />
<uses-permission android:name="android.permission.ACCESS_FINE_LOCATION" />
```
- Complex, API level dependent
- Requires location permission (API 23-30)
- Can opt-out of location (API 31+)

**Windows:**
```xml
<!-- In Package.appxmanifest -->
<DeviceCapability Name="bluetooth" />
```
- Simplest of all platforms
- Automatic user prompt
- No manifest needed for unpackaged apps

## Platform Trade-offs

### iOS/macOS

**Strengths:**
- ✅ Excellent stability and reliability
- ✅ Automatic optimization (MTU, PHY, connection parameters)
- ✅ Comprehensive feature support (L2CAP, RSSI, Broadcasting)
- ✅ Consistent behavior across devices
- ✅ Good documentation and examples

**Weaknesses:**
- ❌ No manual control over MTU, PHY, connection priority
- ❌ Background operations limited
- ❌ Info.plist configuration required
- ❌ Cannot fine-tune for specific performance scenarios

**Best For:**
- Apps where "it just works" is priority
- Consumer-facing apps
- When consistent UX across all Apple devices is important
- Broadcasting/peripheral mode requirements

### Android

**Strengths:**
- ✅ Full control over all BLE parameters (MTU, PHY, priority)
- ✅ Comprehensive feature support
- ✅ Broadcasting with full GATT server
- ✅ L2CAP support (API 29+)
- ✅ Can optimize for specific use cases

**Weaknesses:**
- ❌ GATT error 133 requires retry logic
- ❌ Behavior varies by manufacturer/device
- ❌ Complex permission model (especially across API levels)
- ❌ More testing required (device variance)
- ❌ Background restrictions aggressive

**Best For:**
- Apps requiring performance optimization
- Firmware update utilities
- Power users who need control
- Industrial/enterprise apps
- When you can handle complexity

### Windows

**Strengths:**
- ✅ Simple configuration
- ✅ Good stability (when features work)
- ✅ Desktop/laptop ecosystem
- ✅ Straightforward permission model

**Weaknesses:**
- ❌ Most limited feature set
- ❌ No L2CAP support
- ❌ No PHY control
- ❌ No post-connection RSSI
- ❌ No broadcasting
- ❌ No MTU control
- ❌ Driver dependency issues

**Best For:**
- Desktop apps with basic BLE needs
- Scanning and GATT operations only
- When advanced features not required
- Corporate/enterprise desktop environments

## Migration Considerations

### Android to iOS/macOS

**What You Lose:**
- Manual MTU negotiation
- Manual connection priority control
- Manual PHY selection

**What You Gain:**
- Automatic optimization
- Better stability
- Simpler error handling

**Code Changes:**
```csharp
// Android-specific code to remove or make conditional
#if ANDROID
await device.RequestMtuAsync(512);
await device.RequestConnectionPriorityAsync(BluetoothConnectionPriority.High);
await device.SetPreferredPhyAsync(PhyMode.Le2M, PhyMode.Le2M);
#endif

// Cross-platform code (works everywhere)
var mtu = device.Mtu; // Read only on iOS
```

### iOS/macOS to Android

**What You Gain:**
- Full control over BLE parameters
- Can optimize for specific scenarios
- Better performance tuning

**What You Lose:**
- Simplicity
- Automatic optimization

**Code Changes:**
```csharp
// Add Android-specific optimizations
var connectionOptions = new ConnectionOptions
{
    Android = new AndroidConnectionOptions
    {
        ConnectionPriority = BluetoothConnectionPriority.High,
        ServiceDiscoveryRetry = RetryOptions.Default
    }
};
```

### iOS/Android to Windows

**What You Lose:**
- L2CAP channels
- PHY control
- Post-connection RSSI
- Broadcasting
- Manual MTU negotiation
- Connection priority control

**What You Gain:**
- Simpler development (fewer options)
- Desktop deployment

**Code Changes:**
```csharp
// Make advanced features conditional
if (DeviceInfo.Platform != DevicePlatform.WinUI)
{
    // Use L2CAP
    await device.OpenL2CapChannelAsync(psm);
}
else
{
    // Fallback to GATT characteristics
    await characteristic.WriteValueAsync(data);
}
```

### Windows to iOS/Android

**What You Gain:**
- L2CAP support
- PHY monitoring/control (Android)
- Post-connection RSSI
- Broadcasting capability
- Better mobile support

**What You Lose:**
- Desktop-native deployment

**Code Changes:**
```csharp
// Enable previously unavailable features
await device.OpenL2CapChannelAsync(psm); // Now works
await device.ReadSignalStrengthAsync();  // Now works

#if ANDROID
await device.SetPreferredPhyAsync(PhyMode.Le2M, PhyMode.Le2M);
#endif
```

## Best Platform for Your Use Case

### Consumer Mobile Apps
**Winner: iOS/macOS or Android**
- iOS: Best for consistent UX, "just works"
- Android: Best if you need fine control

**Criteria:**
- User-friendly experience
- Reliable connections
- Background support (limited on both)

### Firmware Update Tools
**Winner: Android**
- Full MTU control for speed
- L2CAP for maximum throughput
- PHY control for optimization

**Criteria:**
- Maximum throughput
- Bulk data transfer
- Performance tuning

### Desktop Utilities
**Winner: Windows**
- Native desktop platform
- Scanning and GATT sufficient
- Simple deployment

**Criteria:**
- Desktop environment
- Basic BLE operations
- No advanced features needed

### IoT Central Hub
**Winner: Android or iOS**
- Android: Better control, Broadcasting
- iOS: Better stability

**Criteria:**
- Multiple concurrent connections
- Broadcasting capability
- Reliable 24/7 operation

### Medical Devices
**Winner: iOS/macOS**
- FDA prefers iOS stability
- Predictable behavior
- Excellent device consistency

**Criteria:**
- Regulatory compliance
- Reliability critical
- Consistent UX

### Industrial Monitoring
**Winner: Android**
- Full parameter control
- Can optimize for environment
- Long-range modes (LeCoded PHY)

**Criteria:**
- Environmental adaptation
- Performance optimization
- Cost-effective hardware

### Cross-Platform Apps
**Winner: Plugin.Bluetooth (all platforms)**
- Unified API
- Platform-specific optimizations available
- Graceful feature degradation

**Strategy:**
```csharp
// Design for Windows (most limited)
// Use platform-specific features as enhancements

var options = new ConnectionOptions();

#if ANDROID
// Android optimizations
options.Android = new AndroidConnectionOptions
{
    ConnectionPriority = BluetoothConnectionPriority.High
};
#elif iOS
// iOS-specific options
options.Apple = new AppleConnectionOptions
{
    NotifyOnDisconnection = true
};
#endif

await device.ConnectAsync(options);

// Use lowest common denominator for core functionality
// Enhance with platform-specific features where available
```

## Summary

| Factor | iOS/macOS | Android | Windows |
|--------|-----------|---------|---------|
| **Overall** | 🟢 Best Stability | 🟡 Best Control | 🔴 Most Limited |
| **Learning Curve** | 🟢 Easy | 🟡 Moderate | 🟢 Easy |
| **Feature Set** | 🟢 Comprehensive | 🟢 Comprehensive | 🔴 Basic |
| **Reliability** | 🟢 Excellent | 🟡 Good (with retry) | 🟢 Good |
| **Performance** | 🟢 Automatic | 🟢 Controllable | 🟡 Adequate |
| **Developer Experience** | 🟢 Good | 🟡 Challenging | 🟢 Simple |
| **Deployment** | 🟢 Mobile/Desktop | 🟢 Mobile | 🟢 Desktop |

**Recommendation**: Use iOS for stability, Android for control, Windows for desktop-only apps. Plugin.Bluetooth provides unified API with platform-specific enhancements.
