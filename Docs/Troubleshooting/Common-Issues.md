# Common Issues and Solutions

This guide addresses frequently encountered issues when working with Plugin.Bluetooth and provides practical solutions.

## Table of Contents

1. [Connection Issues](#connection-issues)
   - [Android GATT Error 133](#android-gatt-error-133)
   - [iOS Connection Failures](#ios-connection-failures)
   - [Windows Connection Issues](#windows-connection-issues)
2. [Permission Issues](#permission-issues)
3. [Scanning Issues](#scanning-issues)
4. [Notification Issues](#notification-issues)
5. [MTU Negotiation Issues](#mtu-negotiation-issues)
6. [Platform-Specific Quirks](#platform-specific-quirks)

---

## Connection Issues

### Android GATT Error 133

**Symptoms:**
- `AndroidNativeGattStatusException` with status code 133
- Connection fails immediately or after a few seconds
- Error message: "Error: GATT error."

**Root Causes:**

GATT error 133 is one of the most common Android BLE issues. It's a generic error that can occur for several reasons:
- Device is out of range or signal is weak
- Too many concurrent connections
- Previous connection not properly closed
- Android Bluetooth stack in an inconsistent state
- Device is already connecting/disconnecting

**Solutions:**

#### 1. Use Built-in Retry Logic (Recommended)

The library includes automatic retry logic for connection failures. This is enabled by default:

```csharp
// Default configuration (3 retries, 200ms delay)
await device.ConnectAsync();
```

To customize retry behavior:

```csharp
var options = new ConnectionOptions
{
    ConnectionRetry = new RetryOptions
    {
        MaxRetries = 5,
        DelayBetweenRetries = TimeSpan.FromMilliseconds(500)
    }
};

await device.ConnectAsync(options);
```

To disable retries:

```csharp
var options = new ConnectionOptions
{
    ConnectionRetry = RetryOptions.None
};

await device.ConnectAsync(options);
```

#### 2. Wait for Advertisement Before Connecting

Some devices require waiting for an advertisement before connecting:

```csharp
var options = new ConnectionOptions
{
    WaitForAdvertisementBeforeConnecting = true
};

await device.ConnectAsync(options);
```

#### 3. Limit Concurrent Connections

Configure maximum concurrent connections in infrastructure options:

```csharp
// In MauiProgram.cs
builder.Services.Configure<BluetoothInfrastructureOptions>(options =>
{
    options.MaxConcurrentConnections = 3; // Default is 5
});
```

#### 4. Clean Up Previous Connections

```csharp
// Ensure previous connection is fully cleaned up
if (device.IsConnected)
{
    await device.DisconnectAsync();
}

// Wait a moment before reconnecting
await Task.Delay(500);

// Now attempt connection
await device.ConnectAsync();
```

#### 5. Reset Bluetooth Stack (Last Resort)

If errors persist, the user may need to:
- Toggle Bluetooth off/on in device settings
- Restart the device
- Clear Bluetooth cache (Android Settings > Apps > Bluetooth > Storage > Clear Cache)

**Advanced Configuration:**

For persistent GATT 133 errors, try Android-specific options:

```csharp
var options = new ConnectionOptions
{
    Android = new AndroidConnectionOptions
    {
        // Enable auto-reconnect
        AutoConnect = true,

        // Use LE transport explicitly
        TransportType = BluetoothTransportType.Le,

        // Increase service discovery retries
        ServiceDiscoveryRetry = new RetryOptions
        {
            MaxRetries = 3,
            DelayBetweenRetries = TimeSpan.FromMilliseconds(500)
        }
    }
};

await device.ConnectAsync(options);
```

### iOS Connection Failures

**Symptoms:**
- Connection times out
- Device appears in scan but fails to connect
- "Peripheral is unavailable" error

**Common Causes and Solutions:**

#### 1. Bluetooth Not Enabled

```csharp
try
{
    await device.ConnectAsync();
}
catch (AppleNativeBluetoothException ex)
{
    // Check if Bluetooth is off
    if (_scanner.BluetoothState == BluetoothState.Off)
    {
        // Prompt user to enable Bluetooth
        await ShowBluetoothPromptAsync();
    }
}
```

#### 2. Device Out of Range

iOS is more sensitive to signal strength than Android. Solutions:
- Ensure device is within range (typically 10 meters)
- Check RSSI value: `device.Rssi` (values below -90 dBm may be unreliable)
- Wait for stronger signal before connecting

```csharp
// Wait for device to be in range
await _scanner.WaitForDeviceToAppearAsync(
    d => d.Id == knownDeviceId && d.Rssi > -80,
    timeout: TimeSpan.FromSeconds(30)
);

await device.ConnectAsync();
```

#### 3. Permissions Not Granted

Ensure proper permission setup:

**Info.plist:**
```xml
<key>NSBluetoothAlwaysUsageDescription</key>
<string>This app uses Bluetooth to connect to BLE devices</string>
```

See [Permissions Guide](../Getting-Started/Permissions.md) for complete setup.

#### 4. Connection Options

iOS-specific connection options can improve reliability:

```csharp
var options = new ConnectionOptions
{
    Apple = new AppleConnectionOptions
    {
        // Show system alerts when connecting in background
        NotifyOnConnection = true,
        NotifyOnDisconnection = true
    }
};

await device.ConnectAsync(options);
```

### Windows Connection Issues

**Symptoms:**
- "Device not paired" errors
- Connection hangs indefinitely
- `WindowsNativeDeviceAccessStatusException`

**Common Causes and Solutions:**

#### 1. Device Pairing Required

Unlike iOS/Android, Windows often requires explicit pairing:

```csharp
try
{
    await device.ConnectAsync();
}
catch (WindowsNativeDeviceAccessStatusException ex)
{
    // Windows may require pairing
    Console.WriteLine("Pairing may be required. Check Windows Bluetooth settings.");
}
```

#### 2. Bluetooth Capability Not Declared

Ensure capability is declared in `Package.appxmanifest`:

```xml
<Capabilities>
  <DeviceCapability Name="bluetooth" />
</Capabilities>
```

#### 3. Connection Timeout

Windows connections can take longer. Increase timeout:

```csharp
// In MauiProgram.cs
builder.Services.Configure<BluetoothInfrastructureOptions>(options =>
{
    options.DefaultOperationTimeout = TimeSpan.FromSeconds(60);
});
```

---

## Permission Issues

### Permission Denied Errors

**Symptoms:**
- `BluetoothPermissionException` thrown
- Scanning or connection fails with permission error
- Permission dialog not appearing

### Android Permissions

Android permissions vary significantly by API level:

#### API 31+ (Android 12+)

**Required Manifest Entries:**

```xml
<!-- AndroidManifest.xml -->
<uses-permission android:name="android.permission.BLUETOOTH_SCAN" />
<uses-permission android:name="android.permission.BLUETOOTH_CONNECT" />

<!-- If you need location for filtering -->
<uses-permission android:name="android.permission.ACCESS_FINE_LOCATION" />
```

**Runtime Permission Request:**

```csharp
// Automatic (default)
await _scanner.StartScanningAsync();

// Manual
if (!await _scanner.HasScannerPermissionsAsync())
{
    await _scanner.RequestScannerPermissionsAsync();
}
```

#### API 28-30 (Android 9-11)

```xml
<uses-permission android:name="android.permission.BLUETOOTH" />
<uses-permission android:name="android.permission.BLUETOOTH_ADMIN" />
<uses-permission android:name="android.permission.ACCESS_FINE_LOCATION" />
```

#### Common Android Permission Issues

**Issue: Permission dialog not appearing**

Solution: Check that all required manifest permissions are declared.

**Issue: Permission denied even after granting**

Solution: Ensure location services are enabled on the device (required for API < 31).

**Issue: App crashes on permission request**

Solution: Declare permissions in manifest before requesting at runtime.

### iOS Permissions

**Required Info.plist Entry:**

```xml
<key>NSBluetoothAlwaysUsageDescription</key>
<string>This app uses Bluetooth to connect to BLE devices</string>
```

**Common Issues:**

**Issue: Permission dialog never appears**

Solutions:
- Verify Info.plist entry exists
- Clean and rebuild project
- Delete and reinstall app
- Check that key is exactly `NSBluetoothAlwaysUsageDescription`

**Issue: Permission denied, even after deleting app**

Solution: Reset privacy settings on device:
Settings > General > Reset > Reset Location & Privacy

### Windows Permissions

**Required Capability:**

```xml
<!-- Package.appxmanifest -->
<Capabilities>
  <DeviceCapability Name="bluetooth" />
</Capabilities>
```

**Issue: Access denied despite capability**

Solution: Windows Store apps need the capability declared. Sideloaded apps may have additional restrictions.

### Checking Permissions

```csharp
// Check scanner permissions
if (await _scanner.HasScannerPermissionsAsync())
{
    await _scanner.StartScanningAsync();
}
else
{
    await _scanner.RequestScannerPermissionsAsync();
}

// Use AssumeGranted to skip checks (use with caution)
var options = new ScanningOptions
{
    PermissionStrategy = PermissionRequestStrategy.AssumeGranted
};
await _scanner.StartScanningAsync(options);
```

---

## Scanning Issues

### Scan Not Finding Devices

**Symptoms:**
- Scanner starts successfully but discovers no devices
- DeviceListChanged event never fires
- Known devices not appearing

#### 1. Check Bluetooth State

```csharp
if (_scanner.BluetoothState != BluetoothState.On)
{
    Console.WriteLine($"Bluetooth state: {_scanner.BluetoothState}");
    // Prompt user to enable Bluetooth
}
```

#### 2. Verify Permissions

```csharp
if (!await _scanner.HasScannerPermissionsAsync())
{
    await _scanner.RequestScannerPermissionsAsync();
}
```

#### 3. Review Scan Filters

Overly restrictive filters can exclude devices:

```csharp
// Too restrictive - won't find devices without names
var restrictiveOptions = new ScanningOptions
{
    IgnoreNamelessAdvertisements = true,
    ServiceUuids = new[] { verySpecificServiceUuid }
};

// More permissive - finds all devices
var permissiveOptions = new ScanningOptions
{
    IgnoreNamelessAdvertisements = false
};

await _scanner.StartScanningAsync(permissiveOptions);
```

#### 4. Check Device Advertising

Use a BLE scanner app (nRF Connect, LightBlue) to verify the device is actually advertising.

#### 5. Increase Scan Power

```csharp
var options = new ScanningOptions
{
    // High power mode for better discovery
    ScanMode = BluetoothScanMode.LowLatency, // Android

    // Platform-specific options
    Android = new AndroidScanningOptions
    {
        CallbackType = BluetoothScanCallbackType.AllMatches
    }
};

await _scanner.StartScanningAsync(options);
```

### Duplicate Devices Appearing

**Symptoms:**
- Same device appears multiple times in list
- Device ID changes between scans

**Cause:** Different platforms use different device identifiers:
- **Android**: MAC address
- **iOS/macOS**: UUID (changes per app/install)
- **Windows**: Bluetooth address or UUID

**Solution:**

Use device name or advertisement data for matching instead of ID:

```csharp
var device = _scanner.GetDeviceOrDefault(d =>
    d.Name == "MyDevice" &&
    d.Advertisement.ManufacturerData.Count > 0);
```

---

## Notification Issues

### Notifications Not Working

**Symptoms:**
- `StartListeningAsync()` succeeds but no notifications received
- ValueUpdated event never fires
- First notification works, subsequent ones don't

#### 1. Verify Characteristic Supports Notifications

```csharp
if (!characteristic.CanNotify)
{
    Console.WriteLine("Characteristic does not support notifications");
    return;
}
```

#### 2. Check CCCD Descriptor Configuration

Notifications require writing to the Client Characteristic Configuration Descriptor (CCCD). The library handles this automatically, but issues can occur:

```csharp
try
{
    // Start listening
    await characteristic.StartListeningAsync();

    // Subscribe to updates
    characteristic.ValueUpdated += (sender, args) =>
    {
        Console.WriteLine($"Notification received: {args.Value.Length} bytes");
    };
}
catch (InvalidOperationException ex)
{
    // "CCCD descriptor not found" error
    Console.WriteLine($"CCCD error: {ex.Message}");
}
```

#### 3. Ensure Device is Connected

```csharp
if (!device.IsConnected)
{
    Console.WriteLine("Device must be connected to receive notifications");
    await device.ConnectAsync();
}

await characteristic.StartListeningAsync();
```

#### 4. Check Service Exploration

Descriptors must be discovered:

```csharp
// Full exploration includes descriptors
await device.ExploreServicesAsync(ServiceExplorationOptions.Full);

var service = device.GetServiceOrDefault(serviceUuid);
await service.ExploreCharacteristicsAsync(CharacteristicExplorationOptions.Full);

var characteristic = service.GetCharacteristicOrDefault(characteristicUuid);
await characteristic.StartListeningAsync();
```

#### 5. Platform-Specific Configuration

**Android:**

```csharp
var options = new ConnectionOptions
{
    Android = new AndroidConnectionOptions
    {
        // Higher priority for notifications
        ConnectionPriority = BluetoothConnectionPriority.High
    }
};
```

**iOS:**

```csharp
var options = new ConnectionOptions
{
    Apple = new AppleConnectionOptions
    {
        // Get notified of notifications in background
        NotifyOnNotification = true
    }
};
```

#### 6. Verify Peripheral Implementation

The BLE peripheral must:
- Enable notifications on the characteristic
- Send notification packets correctly
- Not have too many notifications queued

Test with a known-good BLE app like nRF Connect.

---

## MTU Negotiation Issues

**Symptoms:**
- Large writes fail or are truncated
- Poor throughput despite high connection priority
- Write operations timing out

### Understanding MTU

MTU (Maximum Transmission Unit) determines the maximum packet size:
- **Default:** 23 bytes (20 bytes payload + 3 bytes overhead)
- **Maximum:** Platform-dependent (typically 512 bytes)

### Platform Support

| Platform | MTU Negotiation | Notes |
|----------|----------------|-------|
| Android | Supported | Manual request via `RequestMtuAsync()` |
| iOS/macOS | Automatic | Negotiated by system, cannot request manually |
| Windows | Automatic | Platform-managed, cannot change |

### Android MTU Negotiation

```csharp
// Request higher MTU (Android only)
try
{
    var newMtu = await device.RequestMtuAsync(512);
    Console.WriteLine($"MTU negotiated: {newMtu} bytes");
}
catch (NotSupportedException)
{
    // Not supported on this platform
    Console.WriteLine("MTU negotiation not supported");
}
```

**Common Issues:**

#### 1. MTU Request Fails

```csharp
// Ensure device is connected first
if (!device.IsConnected)
{
    await device.ConnectAsync();
}

// Wait a moment after connection
await Task.Delay(1000);

// Now request MTU
var mtu = await device.RequestMtuAsync(247); // Common safe value
```

#### 2. Peripheral Doesn't Support Large MTU

The peripheral may not support the requested MTU. Start with a conservative value:

```csharp
// Try incrementally
int[] mtuSizes = { 247, 185, 104, 23 };

foreach (var size in mtuSizes)
{
    try
    {
        var result = await device.RequestMtuAsync(size);
        Console.WriteLine($"MTU set to: {result}");
        break;
    }
    catch
    {
        Console.WriteLine($"MTU {size} failed, trying lower...");
    }
}
```

#### 3. Writing Large Data

Even with large MTU, break writes into chunks:

```csharp
var mtu = await device.RequestMtuAsync(512);
var maxWriteSize = mtu - 3; // Account for overhead

var data = new byte[1000];
for (int offset = 0; offset < data.Length; offset += maxWriteSize)
{
    var chunkSize = Math.Min(maxWriteSize, data.Length - offset);
    var chunk = data.Skip(offset).Take(chunkSize).ToArray();

    await characteristic.WriteAsync(chunk);

    // Small delay between writes
    await Task.Delay(50);
}
```

---

## Platform-Specific Quirks

### Android

#### Connection Delays After Disconnection

**Issue:** Immediate reconnection after disconnect may fail.

**Solution:**
```csharp
await device.DisconnectAsync();
await Task.Delay(500); // Wait 500ms
await device.ConnectAsync();
```

#### Service Discovery Caching

**Issue:** Android caches service discovery results. Stale cache may cause issues.

**Workaround:**
```csharp
// Clear device from scanner and re-discover
_scanner.RemoveDevice(device);
await Task.Delay(1000);

// Wait for device to re-appear
var freshDevice = await _scanner.WaitForDeviceToAppearAsync(
    d => d.Id == deviceId,
    timeout: TimeSpan.FromSeconds(10)
);
```

#### Background Scanning Restrictions

API 29+ restricts background scanning. The library handles this, but be aware:
- Scans may stop when app is backgrounded
- Use foreground service for continuous scanning

#### Multiple Writes Queue Management

**Issue:** Rapid writes may overwhelm the GATT queue.

**Solution:** Use retry configuration:
```csharp
var options = new ConnectionOptions
{
    Android = new AndroidConnectionOptions
    {
        GattWriteRetry = new RetryOptions
        {
            MaxRetries = 5,
            DelayBetweenRetries = TimeSpan.FromMilliseconds(300)
        }
    }
};
```

### iOS/macOS

#### UUID Instability

**Issue:** Device UUIDs change between app installs.

**Solution:** Use device name, manufacturer data, or service UUIDs for identification.

#### State Restoration

iOS can restore BLE state when app launches. Handle appropriately:

```csharp
// Check if already scanning when app launches
if (_scanner.IsScanning)
{
    Console.WriteLine("Scanner was restored from background");
}

// Check for previously connected devices
var devices = _scanner.GetDevices();
var connectedDevices = devices.Where(d => d.IsConnected).ToList();
```

#### Connection Events in Background

**Issue:** App doesn't receive connection events when backgrounded.

**Solution:**
```csharp
var options = new ConnectionOptions
{
    Apple = new AppleConnectionOptions
    {
        NotifyOnConnection = true,
        NotifyOnDisconnection = true,
        NotifyOnNotification = true
    }
};
```

#### No MTU Control

iOS manages MTU automatically. You cannot request specific MTU values. The system negotiates the optimal MTU.

### Windows

#### Pairing Requirements

Some Windows devices require pairing before connection. This is handled outside the app:
- User must pair via Windows Settings > Bluetooth
- Or use Windows.Devices.Enumeration.DeviceInformation.Pairing

#### Limited Background Support

Windows UWP apps have limited background Bluetooth capabilities. Use Background Tasks if needed.

#### No Connection Priority

Windows does not expose connection priority settings. The OS manages connection parameters.

---

## General Troubleshooting Tips

### 1. Enable Verbose Logging

```csharp
// In MauiProgram.cs
builder.Services.Configure<BluetoothInfrastructureOptions>(options =>
{
    options.EnableVerboseLogging = true;
});
```

See [Debugging Guide](./Debugging.md) for log analysis details.

### 2. Check Device State

```csharp
Console.WriteLine($"Device: {device.Name}");
Console.WriteLine($"Connected: {device.IsConnected}");
Console.WriteLine($"RSSI: {device.Rssi}");
Console.WriteLine($"Services: {device.GetServices().Count}");
```

### 3. Use Native Tools

- **Android:** nRF Connect, Bluetooth HCI snoop log
- **iOS:** LightBlue, Xcode Console
- **Windows:** Bluetooth LE Explorer

### 4. Test with Known Devices

Use a standard BLE device (heart rate monitor, fitness tracker) to isolate app vs. device issues.

### 5. Verify Peripheral Implementation

Many issues stem from peripheral (device) firmware bugs or non-standard BLE implementations. Test your app with multiple BLE peripherals.

### 6. Handle Exceptions Appropriately

```csharp
try
{
    await device.ConnectAsync();
}
catch (DeviceFailedToConnectException ex)
{
    // Log details for debugging
    Console.WriteLine($"Connection failed: {ex.Message}");
    Console.WriteLine($"Inner exception: {ex.InnerException?.Message}");

    // Check for specific platform errors
    if (ex.InnerException is AndroidNativeGattStatusException gattEx)
    {
        Console.WriteLine($"GATT Status: {gattEx.GattStatus}");
    }
}
```

### 7. Monitor Bluetooth State

```csharp
_scanner.BluetoothStateChanged += (sender, args) =>
{
    Console.WriteLine($"Bluetooth state: {args.State}");

    if (args.State == BluetoothState.Off)
    {
        // Handle Bluetooth disabled
    }
};
```

---

## Next Steps

- [Debugging Guide](./Debugging.md) - Learn how to use logging and native tools
- [Best Practices](../Best-Practices/README.md) - Recommended patterns
- [Platform Setup](../Getting-Started/Platform-Setup.md) - Platform configuration details
- [Error Handling](../Best-Practices/Error-Handling.md) - Exception handling strategies

---

## Still Having Issues?

If you're still experiencing problems:

1. Enable verbose logging and review logs
2. Test with multiple devices to rule out device-specific issues
3. Check the [GitHub Issues](https://github.com/laerdal/Plugin.Bluetooth/issues) for similar problems
4. Create a minimal reproduction case
5. Open a new issue with:
   - Platform and OS version
   - Device information
   - Logs with verbose logging enabled
   - Code sample demonstrating the issue
