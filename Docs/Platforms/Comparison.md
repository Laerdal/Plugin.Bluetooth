# Platform Comparison

This page provides a factual comparison of implemented behavior in current code.

## 1. Core Capability Matrix

Legend:
- ✅ implemented
- ⚠️ implemented with platform-managed behavior
- ❌ not implemented

| Capability | Android | iOS/macOS | Windows |
|------------|---------|-----------|---------|
| Scanning | ✅ | ✅ | ✅ |
| Device connection | ✅ | ✅ | ✅ |
| GATT service discovery | ✅ | ✅ | ✅ |
| Characteristic read/write | ✅ | ✅ | ✅ |
| Characteristic listen/notify | ✅ | ✅ | ✅ |
| Descriptor read/write | ✅ | ✅ | ✅ |
| Request MTU | ✅ API 21+ | ❌ | ❌ |
| Read current MTU | ✅ | ✅ | ✅ |
| Request connection priority | ✅ | ❌ | ❌ |
| Read PHY | ✅ API 26+ | ⚠️ | ❌ |
| Set PHY | ✅ API 26+ | ❌ | ❌ |
| Open L2CAP channel | ✅ API 29+ | ✅ iOS 11+/macOS 10.13+ | ❌ |
| Read RSSI while connected | ✅ | ✅ | ❌ |
| Broadcaster start/stop | ✅ | ✅ | ❌ |
| Local GATT service creation | ✅ | ✅ | ❌ |

## 2. Unsupported Operation Semantics

Windows behavior:
- Unsupported operations throw NotSupportedException.
- This includes L2CAP and broadcaster APIs, plus some advanced connected-device APIs.

DotNetCore fallback behavior:
- BLE runtime APIs throw PlatformNotSupportedException.

iOS/macOS behavior:
- Some operations that are configurable on Android are system-managed and exposed as no-op or unsupported paths depending on API.
- Reliable write transactions are not supported.

## 3. API Shape Is Shared, Behavior Varies

The same abstraction can have different runtime behavior by platform.

Example: connection priority

```csharp
await device.RequestConnectionPriorityAsync(BluetoothConnectionPriority.High);
```

Behavior:
- Android: executes native connection-priority request.
- iOS/macOS: platform-managed behavior.
- Windows: throws NotSupportedException.

Example: L2CAP

```csharp
try
{
    var channel = await device.OpenL2CapChannelAsync(psm);
    await channel.WriteAsync(payload);
}
catch (NotSupportedException)
{
    // Windows fallback path.
}
```

## 4. Configuration Differences

Android has the broadest explicit tuning surface in options objects (scan settings, connection priority, PHY, API-gated capabilities).

iOS/macOS supports platform-specific options for scanning/connection/broadcasting integration points, but many transport details are managed by CoreBluetooth.

Windows option surface is narrower for advanced transport controls because those operations are currently not implemented.

## 5. Practical Cross-Platform Baseline

If a single behavior baseline is required across all native platforms, use:
- Scanning
- Connect/disconnect
- Service/characteristic/descriptor discovery
- Characteristic read/write
- Characteristic notifications/indications

Treat the following as optional capability paths with guards:
- L2CAP
- Connection priority tuning
- PHY tuning
- Connected RSSI reads
- Broadcasting

