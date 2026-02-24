# Platform-Specific Documentation

This directory contains detailed platform-specific documentation for Plugin.Bluetooth, covering implementation details, capabilities, limitations, and best practices for each supported platform.

## Documentation Structure

### Platform Guides
- **[iOS & macOS](iOS-macOS.md)** - CoreBluetooth implementation details, Info.plist configuration, platform limitations
- **[Android](Android.md)** - Android BLE stack details, API level requirements, GATT error handling, permissions
- **[Windows](Windows.md)** - WinRT implementation, manifest configuration, platform constraints
- **[Platform Comparison](Comparison.md)** - Side-by-side feature comparison and migration considerations

## Quick Feature Matrix

| Feature | iOS/macOS | Android | Windows | Notes |
|---------|-----------|---------|---------|-------|
| **Scanning** | ✅ | ✅ | ✅ | All platforms supported |
| **Connection** | ✅ | ✅ | ✅ | All platforms supported |
| **GATT Operations** | ✅ | ✅ | ✅ | Read/Write/Notify/Indicate |
| **Service Discovery** | ✅ | ✅ | ✅ | All platforms supported |
| **Descriptor Operations** | ✅ | ✅ | ✅ | All platforms supported |
| **MTU Negotiation** | ⚠️ Auto | ✅ API 21+ | ⚠️ Auto | iOS/Windows: system-managed |
| **MTU Reading** | ✅ | ✅ | ✅ | Get current MTU value |
| **Connection Priority** | ⚠️ Auto | ✅ | ⚠️ Auto | Only Android allows control |
| **PHY Configuration** | ⚠️ Auto | ✅ API 26+ | ❌ | iOS auto, Windows not supported |
| **PHY Reading** | ✅ | ✅ API 26+ | ❌ | iOS after connection |
| **L2CAP Channels** | ✅ iOS 11+ | ✅ API 29+ | ❌ | Direct socket connections |
| **RSSI (Scanning)** | ✅ | ✅ | ✅ | During device discovery |
| **RSSI (Connected)** | ✅ | ✅ | ❌ | Windows: scanning only |
| **Broadcasting** | ✅ | ✅ API 21+ | ❌ | Peripheral/GATT server mode |
| **Background Scanning** | ⚠️ Limited | ⚠️ Limited | ❌ | Platform restrictions apply |

### Legend
- ✅ **Fully Supported** - Feature fully implemented and available
- ⚠️ **Partial/Auto** - Feature exists but may be limited or automatic
- ❌ **Not Supported** - Feature not available on platform
- **API X+** - Requires minimum Android API level

## Platform Selection Guide

### Choose iOS/macOS when:
- You need reliable, stable BLE operations without fine-tuning
- Broadcasting (peripheral mode) is required
- Your app targets Apple ecosystem exclusively
- You prefer automatic PHY/MTU optimization

### Choose Android when:
- You need full control over connection parameters (MTU, PHY, priority)
- Advanced features like L2CAP channels are required
- Broadcasting with custom GATT server is needed
- You can handle GATT error 133 and retry logic

### Choose Windows when:
- Desktop BLE support is required
- Scanning and basic GATT operations are sufficient
- You don't need RSSI after connection
- Broadcasting is not required

### Multi-Platform Considerations
When targeting multiple platforms:
1. Design for the **lowest common denominator** (Windows is most limited)
2. Use **platform-specific options** for optimization where available
3. Implement **graceful degradation** when features are unavailable
4. Test thoroughly on **physical devices** (emulators have limitations)

## Common Implementation Patterns

### Feature Detection
```csharp
// Check if L2CAP is supported on current platform
try
{
    await device.OpenL2CapChannelAsync(psm);
}
catch (NotSupportedException)
{
    // Fallback to GATT characteristic streaming
}
```

### Platform-Specific Configuration
```csharp
var connectionOptions = new ConnectionOptions
{
    // Common options
    ConnectionRetry = RetryOptions.Default,

    // Android-specific
    Android = new AndroidConnectionOptions
    {
        ConnectionPriority = BluetoothConnectionPriority.High,
        ServiceDiscoveryRetry = new RetryOptions { MaxRetries = 3 }
    },

    // iOS-specific
    Apple = new AppleConnectionOptions
    {
        NotifyOnConnection = true,
        NotifyOnDisconnection = true
    }
};

await device.ConnectAsync(connectionOptions);
```

### Conditional MTU Request
```csharp
// Request MTU on platforms that support it
if (DeviceInfo.Platform == DevicePlatform.Android)
{
    await device.RequestMtuAsync(512);
}
// iOS/Windows will use system-negotiated MTU
```

## Platform-Specific Best Practices

### iOS/macOS
- Always add required Info.plist entries before testing
- Use `NSBluetoothAlwaysUsageDescription` for background operations
- Monitor system Bluetooth state changes
- Test with iOS Low Energy mode enabled

### Android
- Implement retry logic for GATT error 133 (connection failures)
- Request location permissions for scanning (Android 10+)
- Handle varied behavior across API levels (21, 23, 26, 29, 31, 33)
- Test on devices from different manufacturers
- Consider connection priority for performance vs. battery trade-offs

### Windows
- Ensure minimum Windows version 10.0.17763 (October 2018)
- Add `bluetooth` DeviceCapability to app manifest
- Don't rely on RSSI after connection
- Use cached mode sparingly (prefer uncached for fresh data)

## Getting Started

1. **Read the [Platform Comparison](Comparison.md)** to understand differences
2. **Review your target platform guide**:
   - [iOS & macOS Guide](iOS-macOS.md)
   - [Android Guide](Android.md)
   - [Windows Guide](Windows.md)
3. **Configure platform-specific settings** (permissions, manifest, Info.plist)
4. **Test on physical devices** in your target environment

## Additional Resources

- [Plugin.Bluetooth Main README](../../README.md)
- [Architecture Documentation](../ARCHITECTURE_GUIDELINES.md)
- [L2CAP Configuration Options](../L2CAP_ADDITIONAL_OPTIONS.md)
- [Timeout Analysis](../TIMEOUT_ANALYSIS.md)

## Need Help?

- Check platform-specific troubleshooting sections in each guide
- Review [GitHub Issues](https://github.com/laerdal/Plugin.Bluetooth/issues)
- Consult platform vendor documentation:
  - [Apple CoreBluetooth](https://developer.apple.com/documentation/corebluetooth)
  - [Android Bluetooth LE](https://developer.android.com/guide/topics/connectivity/bluetooth-le)
  - [Windows Bluetooth](https://learn.microsoft.com/en-us/windows/uwp/devices-sensors/bluetooth)
