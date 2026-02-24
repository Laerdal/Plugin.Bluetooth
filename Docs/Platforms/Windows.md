# Windows Platform Guide

Comprehensive guide for Plugin.Bluetooth on Windows platform using Windows Runtime (WinRT) APIs.

## Table of Contents
- [Overview](#overview)
- [Requirements](#requirements)
- [WinRT Bluetooth Architecture](#winrt-bluetooth-architecture)
- [Configuration](#configuration)
- [Feature Support](#feature-support)
- [Platform Limitations](#platform-limitations)
- [Best Practices](#best-practices)
- [Troubleshooting](#troubleshooting)

## Overview

Windows uses the **Windows Runtime (WinRT)** Bluetooth APIs introduced in Windows 10. These APIs provide managed access to Bluetooth Low Energy functionality through the `Windows.Devices.Bluetooth` and `Windows.Devices.Bluetooth.GenericAttributeProfile` namespaces.

### Key Characteristics
- **System-Managed**: Most operations (MTU, PHY) are automatically handled by Windows
- **Desktop-Focused**: Designed primarily for desktop and laptop scenarios
- **Limited Control**: Less configurability compared to Android/iOS
- **Most Restrictive**: Several features not supported (L2CAP, PHY, post-connection RSSI)
- **Stable**: Generally reliable when supported features are used

### Supported Windows Versions
- **Minimum**: Windows 10 version 1809 (Build 17763) - October 2018 Update
- **Recommended**: Windows 11 or Windows 10 version 22H2 or later
- **UWP & Desktop**: Supports both UWP apps and desktop apps (Win32, WPF, WinForms via .NET)

## Requirements

### Minimum Versions
- **Windows**: 10.0.17763 (October 2018 Update) or later
- **.NET**: .NET 6.0+ (for Windows app development)
- **Windows SDK**: 10.0.17763 or later
- **Bluetooth Hardware**: Bluetooth 4.0+ adapter required

### Hardware Requirements
- Bluetooth 4.0 or later adapter (Bluetooth LE support)
- Most modern laptops have built-in Bluetooth
- USB Bluetooth dongles supported (ensure Windows 10/11 compatible drivers)

## WinRT Bluetooth Architecture

### Class Hierarchy

Plugin.Bluetooth maps to Windows WinRT APIs as follows:

| Plugin.Bluetooth | Windows WinRT | Description |
|------------------|---------------|-------------|
| `IBluetoothScanner` | `BluetoothLEAdvertisementWatcher` | Scans for BLE advertisements |
| `IBluetoothRemoteDevice` | `BluetoothLEDevice` | Represents remote BLE device |
| `IBluetoothRemoteService` | `GattDeviceService` | GATT service on remote device |
| `IBluetoothRemoteCharacteristic` | `GattCharacteristic` | GATT characteristic |
| `IBluetoothRemoteDescriptor` | `GattDescriptor` | GATT descriptor |
| `IBluetoothAdapter` | `BluetoothAdapter` + `Radio` | Local Bluetooth adapter |
| N/A | `GattSession` | Session for reliable connection |

### Implementation Details
- **BluetoothLeDeviceProxy**: Wraps `BluetoothLEDevice` with event delegation
- **GattSessionWrapper**: Manages `GattSession` for maintaining connections
- **Async/Await**: All operations use native `IAsyncOperation` with cancellation token support
- **Cache Modes**: Support for cached vs. uncached GATT reads

## Configuration

### Package.appxmanifest (UWP/MSIX Packaged Apps)

For packaged Windows apps (UWP, MSIX), add Bluetooth capability:

```xml
<?xml version="1.0" encoding="utf-8"?>
<Package xmlns="http://schemas.microsoft.com/appx/manifest/foundation/windows10"
         xmlns:uap="http://schemas.microsoft.com/appx/manifest/uap/windows10"
         xmlns:rescap="http://schemas.microsoft.com/appx/manifest/foundation/windows10/restrictedcapabilities">

  <Capabilities>
    <!-- Required for Bluetooth access -->
    <DeviceCapability Name="bluetooth" />

    <!-- Optional: For accessing device information -->
    <DeviceCapability Name="bluetoothDeviceInformation" />
  </Capabilities>

</Package>
```

### Unpackaged Desktop Apps (Win32, WPF, WinForms)

For traditional desktop applications:
1. **No manifest required** - WinRT APIs available directly
2. **Runtime Components**: Ensure `.NET Desktop Runtime` includes Windows SDK components
3. **Target Framework**: Use `net6.0-windows10.0.17763.0` or later

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net8.0-windows10.0.22621.0</TargetFramework>
    <UseWPF>true</UseWPF> <!-- Or UseWindowsForms -->
    <Nullable>enable</Nullable>
  </PropertyGroup>
</Project>
```

### User Permissions

Windows automatically prompts users for Bluetooth access:
- First BLE operation triggers consent dialog
- User can allow or deny access
- Permissions managed via **Settings → Privacy → Bluetooth**

No programmatic permission request needed (unlike Android/iOS).

## Feature Support

### ✅ Fully Supported Features

#### 1. Scanning
```csharp
var options = new ScanningOptions
{
    // Windows scans continuously when started
    // No platform-specific options for Windows scanning
};

await scanner.StartScanningAsync(options);

// Device discovery via events
scanner.DeviceListChanged += (sender, args) =>
{
    foreach (var device in scanner.Devices)
    {
        Console.WriteLine($"Found: {device.Name} RSSI: {device.SignalStrengthDbm} dBm");
    }
};
```

**Scanning Characteristics:**
- Continuous scanning when active
- RSSI available during scan
- Advertisement data parsing (LocalName, ServiceUUIDs, ManufacturerData)
- Lower power consumption than Android in most cases

#### 2. Connection
```csharp
var connectionOptions = new ConnectionOptions
{
    // Windows-specific options (currently none exposed by WinRT)
    Windows = new WindowsConnectionOptions()
};

await device.ConnectAsync(connectionOptions);
```

**Connection Process:**
1. Resolve `BluetoothLEDevice` from address
2. Create `GattSession` for reliable connection
3. Set `MaintainConnection = true` to keep GATT session active
4. Request device access (permissions)
5. Read GATT services to establish connection

**Connection Reliability:**
- Generally stable once connected
- `GattSession` keeps connection alive
- Auto-reconnect not built-in (implement manually if needed)

#### 3. GATT Operations
All standard GATT operations fully supported:

```csharp
// Service discovery
await device.ExploreServicesAsync();

// Characteristic operations
var value = await characteristic.ReadValueAsync();
await characteristic.WriteValueAsync(data);
await characteristic.StartListeningAsync(); // Notifications
```

**GATT Features:**
- Service/characteristic/descriptor discovery
- Read operations with cached or uncached modes
- Write with response
- Write without response
- Notifications and indications
- Reliable write (if peripheral supports)

**Cache Modes:**
```csharp
// Windows supports cache modes for GATT operations
// Plugin.Bluetooth defaults to Uncached for fresh data
// Cached mode uses Windows-maintained cache (may be stale)
```

#### 4. MTU Reading
```csharp
// MTU negotiated automatically by Windows
var currentMtu = device.Mtu;

// Also available via GattSession
if (gattSession != null)
{
    var maxPduSize = gattSession.MaxPduSize; // Includes ATT overhead
}
```

**MTU Characteristics:**
- Automatically negotiated during connection
- Typical values: 23-512 bytes
- Cannot be requested/changed programmatically
- Updates available via `GattSession.MaxPduSizeChanged` event

#### 5. Signal Strength (RSSI) - During Scanning Only
```csharp
// RSSI available during scanning
scanner.DeviceListChanged += (sender, args) =>
{
    foreach (var device in scanner.Devices)
    {
        // Valid RSSI from advertisement
        Console.WriteLine($"RSSI: {device.SignalStrengthDbm} dBm");
    }
};
```

⚠️ **Important**: RSSI **NOT available after connection** on Windows.

#### 6. Service Discovery
```csharp
// Discover all services
await device.ExploreServicesAsync();

// Access discovered services
foreach (var service in device.Services)
{
    Console.WriteLine($"Service: {service.Id}");

    // Discover characteristics
    await service.ExploreCharacteristicsAsync();

    foreach (var characteristic in service.Characteristics)
    {
        Console.WriteLine($"  Characteristic: {characteristic.Id}");
        Console.WriteLine($"    Properties: {characteristic.Properties}");
    }
}
```

### ⚠️ Limited/Automatic Features

#### 1. MTU Negotiation
```csharp
// Cannot request specific MTU on Windows
// await device.RequestMtuAsync(512); // Throws NotSupportedException

// MTU is automatically negotiated and can only be read
var mtu = device.Mtu;
```

**Explanation**: Windows negotiates MTU during connection establishment. No API to request specific MTU value.

#### 2. Connection Priority
```csharp
// Cannot set connection priority on Windows
// await device.RequestConnectionPriorityAsync(priority); // Throws NotSupportedException
```

**Explanation**: Windows manages connection parameters automatically based on system state and power profile.

### ❌ Not Supported Features

#### 1. PHY Configuration
```csharp
// PHY cannot be read or set on Windows
// await device.SetPreferredPhyAsync(PhyMode.Le2M, PhyMode.Le2M); // Throws NotSupportedException
```

**Reason**: WinRT Bluetooth APIs do not expose PHY layer control.

#### 2. L2CAP Channels
```csharp
// L2CAP channels not supported on Windows
// await device.OpenL2CapChannelAsync(psm); // Throws NotSupportedException
```

**Reason**: `Windows.Devices.Bluetooth` does not provide L2CAP socket APIs. GATT characteristic streaming only option.

#### 3. RSSI After Connection
```csharp
// Cannot read RSSI after connection established
// await device.ReadSignalStrengthAsync(); // Throws NotSupportedException
```

**Reason**: WinRT only provides RSSI during advertisement scanning, not from connected devices.

#### 4. Broadcasting (Peripheral Mode)
```csharp
// Broadcasting not supported on Windows
var broadcaster = serviceProvider.GetRequiredService<IBluetoothBroadcaster>();
// await broadcaster.StartAsync(options); // Throws NotSupportedException or NotImplementedException
```

**Reason**: Windows WinRT APIs for GATT server (`GattServiceProvider`) exist but are not implemented in Plugin.Bluetooth due to instability and limited real-world use cases.

## Platform Limitations

### 1. No Post-Connection RSSI
**Impact**: Cannot monitor connection quality after establishing connection

**Workaround**:
- Monitor connection stability via connection state changes
- Use GATT operation success/failure as proxy for connection quality
- Re-scan device periodically (requires disconnection)

### 2. No MTU Control
**Impact**: Cannot optimize MTU for specific use cases

**Workaround**:
- Accept system-negotiated MTU (usually adequate)
- Chunk large writes based on actual MTU value
- Windows typically negotiates reasonable MTU (185-512 bytes)

### 3. No L2CAP Support
**Impact**: Cannot use high-throughput direct socket connections

**Workaround**:
- Use batch writes to characteristics
- Request larger MTU (automatic, cannot control)
- Consider alternative connection methods (USB, network) for bulk transfers

### 4. Limited Background Support
**Impact**: Background BLE operations limited compared to mobile platforms

**Characteristics**:
- App must be running (foreground or background)
- No wake-on-BLE advertising
- Background tasks available but limited

**Workaround**:
- Use Windows services for persistent BLE operations
- Minimize background scanning
- Keep app running when BLE connectivity required

### 5. Driver Dependency
**Impact**: BLE behavior varies by Bluetooth adapter and driver

**Considerations**:
- Intel, Broadcom, Realtek drivers have different characteristics
- Some cheap USB dongles have poor Windows 10/11 support
- Update drivers for best compatibility

**Recommendation**: Test with built-in laptop Bluetooth and quality USB dongles (TP-Link, ASUS, Plugable).

### 6. Pairing Complexity
**Impact**: Pairing behavior varies by adapter and Windows version

**Notes**:
- Some devices auto-pair on first connection
- Some require explicit pairing via Windows Settings
- `DeviceInformationPairing` API available but complex
- Plugin.Bluetooth doesn't explicitly handle pairing (rely on Windows)

## Best Practices

### 1. Target Appropriate Windows Version
✅ **Minimum**: Windows 10 version 1809 (17763)
```xml
<TargetPlatformMinVersion>10.0.17763.0</TargetPlatformMinVersion>
```

✅ **Recommended**: Windows 11 or Windows 10 22H2
```xml
<TargetPlatformVersion>10.0.22621.0</TargetPlatformVersion>
```

### 2. Handle Connection State Carefully
✅ **Monitor connection state changes**
```csharp
device.ConnectionStateChanged += async (sender, args) =>
{
    if (args.ConnectionState == ConnectionState.Disconnected)
    {
        // Connection lost - attempt reconnection
        await Task.Delay(1000); // Brief delay
        await device.ConnectAsync(connectionOptions);
    }
};
```

✅ **Maintain GattSession**
The `GattSession` is crucial for reliable connections. Plugin.Bluetooth manages this automatically.

### 3. Use Uncached Reads for Fresh Data
```csharp
// Plugin.Bluetooth defaults to uncached mode
var value = await characteristic.ReadValueAsync();
// Always gets fresh value from device, not Windows cache
```

### 4. Adapt to MTU Limitations
✅ **Chunk large writes based on actual MTU**
```csharp
var mtu = device.Mtu;
var maxChunkSize = mtu - 3; // Subtract ATT overhead

foreach (var chunk in data.Chunk(maxChunkSize))
{
    await characteristic.WriteValueAsync(chunk);
    await Task.Delay(10); // Brief delay between writes
}
```

### 5. Scan Efficiently
✅ **Stop scanning when not needed**
```csharp
// Don't scan continuously in background
await scanner.StartScanningAsync();
// ... find device ...
await scanner.StopScanningAsync();
```

### 6. Handle Missing Features Gracefully
✅ **Check platform before using advanced features**
```csharp
if (DeviceInfo.Platform == DevicePlatform.WinUI)
{
    // Don't attempt L2CAP, PHY control, or post-connection RSSI
    // Use GATT characteristics only
}
```

### 7. Test on Real Hardware
⚠️ **Virtual machines & emulators**:
- Bluetooth often doesn't work in VMs
- Hyper-V may conflict with Bluetooth drivers
- Always test on physical Windows 10/11 hardware

### 8. Keep Drivers Updated
✅ **Update Bluetooth drivers**
- Windows Update usually handles this
- OEM websites (Intel, Dell, HP) for specific drivers
- Generic Microsoft Bluetooth drivers usually work well

## Troubleshooting

### Device Not Found During Scan
**Problem**: Known device not appearing in scan results

**Solutions:**
1. Verify device is advertising (check with phone app like nRF Connect)
2. Ensure device not already connected to another central
3. Check Windows Bluetooth is enabled (Settings → Bluetooth & devices)
4. Restart Bluetooth service: `services.msc` → Bluetooth Support Service → Restart
5. Try different Bluetooth adapter (if using USB dongle)

### Connection Fails Immediately
**Problem**: `ConnectAsync()` throws exception or connection immediately drops

**Solutions:**
1. Verify device is in range (RSSI > -85 dBm during scan)
2. Check device access permissions (should prompt automatically)
3. Ensure device not paired to another system
4. Try removing device from Windows paired devices list
5. Restart Bluetooth service
6. Reboot Windows (clears stale Bluetooth stack state)

### Services Not Found After Connection
**Problem**: `ExploreServicesAsync()` returns empty or incomplete list

**Solutions:**
1. Wait 1-2 seconds after connection before discovering services
2. Use uncached mode (Plugin.Bluetooth default)
3. Verify device GATT database is correct (test with phone app)
4. Disconnect and reconnect
5. Remove from paired devices and reconnect

### Writes Don't Reach Device
**Problem**: Write appears successful but device doesn't respond

**Solutions:**
1. Verify characteristic has write property
2. Check MTU accommodates data size
3. Add small delay between writes (10-50ms)
4. Verify device GATT server is functioning
5. Try write-with-response instead of write-without-response

### Notifications Not Received
**Problem**: Subscribed to notifications but not receiving data

**Solutions:**
1. Verify characteristic has Notify or Indicate property
2. Check CCCD was written (automatic in Plugin.Bluetooth)
3. Confirm device is sending notifications (test with phone app)
4. Verify connection is active
5. Re-subscribe: `StopListeningAsync()` then `StartListeningAsync()`

### High CPU Usage
**Problem**: App using excessive CPU during BLE operations

**Solutions:**
1. Stop scanning when not needed
2. Reduce notification rate from peripheral
3. Don't poll characteristics in tight loop
4. Use appropriate delays between operations

### Bluetooth Adapter Not Found
**Problem**: `IBluetoothAdapter` reports no adapter available

**Solutions:**
1. Check Bluetooth is enabled in Windows Settings
2. Verify hardware Bluetooth adapter present (Device Manager)
3. Update/reinstall Bluetooth drivers
4. Disable conflicting software (some VPN or security software)
5. Check for Windows Update pending

### Permission Denied
**Problem**: Access denied when connecting to device

**Solutions:**
1. Grant Bluetooth permission in consent dialog
2. Check Settings → Privacy → Bluetooth (ensure app has access)
3. Run app as administrator (temporary test, not production solution)
4. Verify `bluetooth` DeviceCapability in manifest (packaged apps)

### Pairing Issues
**Problem**: Device requires pairing but pairing fails or doesn't prompt

**Solutions:**
1. Manually pair via Windows Settings → Bluetooth & devices
2. Remove device and re-pair
3. Check device doesn't require specific pairing method (Just Works, PIN, etc.)
4. Some devices auto-pair on connection (no user prompt)

### Works on Android/iOS but Not Windows
**Problem**: Same code works mobile but fails on Windows

**Likely Causes:**
1. Using L2CAP (not supported on Windows)
2. Relying on post-connection RSSI (not supported)
3. Requesting MTU or PHY changes (not supported)
4. Broadcasting/peripheral mode (not implemented)

**Solution**: Implement platform-specific fallbacks:
```csharp
if (DeviceInfo.Platform == DevicePlatform.WinUI)
{
    // Use GATT characteristics only
    // Accept system MTU
    // Skip RSSI monitoring after connection
}
```

## Additional Resources

### Microsoft Documentation
- [Bluetooth Low Energy Overview](https://learn.microsoft.com/en-us/windows/uwp/devices-sensors/bluetooth-low-energy-overview)
- [BluetoothLEDevice Class](https://learn.microsoft.com/en-us/uwp/api/windows.devices.bluetooth.bluetoothledevice)
- [GattDeviceService Class](https://learn.microsoft.com/en-us/uwp/api/windows.devices.bluetooth.genericattributeprofile.gattdeviceservice)
- [GattCharacteristic Class](https://learn.microsoft.com/en-us/uwp/api/windows.devices.bluetooth.genericattributeprofile.gattcharacteristic)
- [GattSession Class](https://learn.microsoft.com/en-us/uwp/api/windows.devices.bluetooth.genericattributeprofile.gattsession)

### Related Documentation
- [Platform Comparison](Comparison.md)
- [iOS Platform Guide](iOS-macOS.md)
- [Android Platform Guide](Android.md)

### Tools
- **Bluetooth LE Explorer** - Microsoft Store app for testing BLE devices on Windows
- **Device Manager** - Check Bluetooth adapter status and drivers
- **Event Viewer** - Windows logs for Bluetooth errors

### Common Windows Bluetooth Adapters
- **Intel Wireless Bluetooth** - Excellent, found in most laptops
- **Broadcom Bluetooth** - Good compatibility
- **Realtek Bluetooth** - Variable quality, update drivers
- **USB Dongles**: TP-Link, ASUS USB-BT400/500, Plugable USB-BT4LE
