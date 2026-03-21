# Platform Documentation

This section documents implementation-level platform behavior.

## Included Files

- [Platform Comparison](Comparison.md)
- [Android](Android.md)
- [iOS and macOS](iOS-macOS.md)
- [Windows](Windows.md)

## Capability Matrix

Legend:
- ✅ implemented
- ⚠️ implemented with platform-managed behavior or constrained support
- ❌ not implemented

| Capability | Android | iOS/macOS | Windows |
|------------|---------|-----------|---------|
| Scanning | ✅ | ✅ | ✅ |
| Connection | ✅ | ✅ | ✅ |
| GATT read/write/notify | ✅ | ✅ | ✅ |
| Descriptor access | ✅ | ✅ | ✅ |
| MTU request | ✅ API 21+ | ❌ (platform-managed) | ❌ (platform-managed) |
| Connection priority request | ✅ | ❌ (platform-managed) | ❌ |
| PHY read | ✅ API 26+ | ⚠️ (platform-managed) | ❌ |
| PHY set | ✅ API 26+ | ❌ (platform-managed) | ❌ |
| L2CAP channel | ✅ API 29+ | ✅ iOS 11+/macOS 10.13+ | ❌ |
| Connected RSSI read | ✅ | ✅ | ❌ |
| Broadcasting (GATT server/peripheral role) | ✅ | ✅ | ❌ |

## Exception Behavior For Unsupported Operations

- Windows advanced operations throw NotSupportedException for:
  - L2CAP open
  - MTU request
  - Connection priority request
  - PHY preference request
  - Connected RSSI read
  - Broadcaster start/stop/service creation
- DotNetCore fallback implementations throw PlatformNotSupportedException for runtime BLE operations.

## Cross-Platform Usage Pattern

```csharp
try
{
    await device.OpenL2CapChannelAsync(psm);
}
catch (NotSupportedException)
{
    // Fallback path for platforms without L2CAP support.
}
```

```csharp
if (DeviceInfo.Platform == DevicePlatform.Android)
{
    await device.RequestMtuAsync(512);
}
```

## Related Docs

- [Architecture Guidelines](../ARCHITECTURE_GUIDELINES.md)
- [Configuration: Connection Options](../Configuration/Connection-Options.md)
- [Configuration: L2CAP Options](../Configuration/L2CAP-Options.md)
- [Troubleshooting](../Troubleshooting/Common-Issues.md)
