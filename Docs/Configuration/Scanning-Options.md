# Scanning Options

`ScanningOptions` represents the configuration for Bluetooth Low Energy scanning operations. These options control how the scanner discovers and filters BLE devices during a scan session.

## Table of Contents

- [Overview](#overview)
- [Basic Usage](#basic-usage)
- [Permission Options](#permission-options)
- [Filtering Options](#filtering-options)
- [Scan Mode and Power Settings](#scan-mode-and-power-settings)
- [Signal Strength Options](#signal-strength-options)
- [Extended Advertising](#extended-advertising)
- [Retry Configuration](#retry-configuration)
- [Android-Specific Options](#android-specific-options)
- [Usage Examples](#usage-examples)
- [Best Practices](#best-practices)

---

## Overview

Scanning options are passed to `IBluetoothScanner.StartScanningAsync()` and control:
- Permission request behavior
- Device filtering (service UUIDs, signal strength, custom filters)
- Power consumption vs. scan latency trade-offs
- Duplicate advertisement handling
- Platform-specific scanning features
- Retry behavior for transient failures

**Namespace**: `Bluetooth.Abstractions.Scanning.Options`

**Usage Pattern**: Passed to method calls (not DI-configured)

---

## Basic Usage

```csharp
// Inject scanner via DI
public class MyBluetoothService
{
    private readonly IBluetoothScanner _scanner;

    public MyBluetoothService(IBluetoothScanner scanner)
    {
        _scanner = scanner;
    }

    public async Task StartScanning()
    {
        var options = new ScanningOptions
        {
            ScanMode = BluetoothScanMode.Balanced,
            IgnoreDuplicateAdvertisements = false,
            ServiceUuids = new[] { MyServiceUuid }
        };

        await _scanner.StartScanningAsync(options);
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

Controls how Bluetooth scanning permissions are requested.

**Values**:

| Value | Behavior | Use Case |
|-------|----------|----------|
| `RequestAutomatically` | Automatically requests permissions before scanning | Most applications (recommended) |
| `ThrowIfNotGranted` | Throws exception if permissions not granted | Custom permission flow |
| `AssumeGranted` | Skips permission checks (assumes granted) | Manual permission management |

**Platform Behavior**:

| Platform | RequestAutomatically | ThrowIfNotGranted | AssumeGranted |
|----------|---------------------|-------------------|---------------|
| **Android** | Requests scan permissions | Throws if not granted | Skips checks (may throw SecurityException) |
| **iOS/macOS** | No-op (system handles) | No-op (system handles) | No-op (system handles) |
| **Windows** | Checks adapter state | Checks adapter state | Skips checks |

**Example**:
```csharp
// Automatic (recommended)
options.PermissionStrategy = PermissionRequestStrategy.RequestAutomatically;

// Custom permission flow
options.PermissionStrategy = PermissionRequestStrategy.ThrowIfNotGranted;
try
{
    await _scanner.StartScanningAsync(options);
}
catch (BluetoothPermissionException)
{
    // Show custom permission UI
    await RequestPermissionsManually();
}
```

---

### RequireBackgroundLocation

```csharp
public bool RequireBackgroundLocation { get; init; }
```

**Default**: `false`

Controls whether to request background location permission on Android.

**Platform Support**:

| Platform | Support | Notes |
|----------|---------|-------|
| **Android** | API 29-30 (Android 10-11) | Requests ACCESS_BACKGROUND_LOCATION |
| **iOS/macOS** | Ignored | Handled by Info.plist |
| **Windows** | Ignored | No background permission needed |

**Use Cases**:
- Background scanning on Android 10-11
- Location-based BLE applications

**Example**:
```csharp
options.RequireBackgroundLocation = true; // For background scanning on Android
```

---

## Filtering Options

### IgnoreDuplicateAdvertisements

```csharp
public bool IgnoreDuplicateAdvertisements { get; init; }
```

**Default**: `false`

Controls whether to ignore duplicate advertisements from the same device.

**When false (default)**:
- Receive multiple advertisements from the same device
- RSSI updates continuously
- Useful for proximity/distance tracking

**When true**:
- Receive only first advertisement per device
- Reduces callback frequency
- Better battery life

**Example**:
```csharp
// Track signal strength changes (default)
options.IgnoreDuplicateAdvertisements = false;

// Discover devices once only
options.IgnoreDuplicateAdvertisements = true;
```

---

### IgnoreNamelessAdvertisements

```csharp
public bool IgnoreNamelessAdvertisements { get; init; }
```

**Default**: `false`

Controls whether to ignore advertisements without a local name.

**When true**:
- Only devices advertising a name are reported
- Reduces discovered device list
- Good for user-facing device pickers

**When false (default)**:
- All devices reported (named and unnamed)
- Necessary for headless devices

**Example**:
```csharp
// Show only named devices in device picker
options.IgnoreNamelessAdvertisements = true;
```

---

### ServiceUuids

```csharp
public IReadOnlyList<Guid>? ServiceUuids { get; init; }
```

**Default**: `null` (all devices)

Filters scanning to only discover devices advertising specific service UUIDs.

**Platform Support**:

| Platform | Support | Implementation |
|----------|---------|----------------|
| **Android** | Full | Hardware filtering via ScanFilter |
| **iOS/macOS** | Full | Hardware filtering via CBCentralManager |
| **Windows** | Partial | Software filtering after reception |

**Example**:
```csharp
// Scan for heart rate monitors only
options.ServiceUuids = new[]
{
    Guid.Parse("0000180D-0000-1000-8000-00805F9B34FB") // Heart Rate Service
};

// Scan for multiple service types
options.ServiceUuids = new[]
{
    HeartRateServiceUuid,
    BatteryServiceUuid,
    DeviceInformationServiceUuid
};
```

---

### AdvertisementFilter

```csharp
public Func<IBluetoothAdvertisement, bool> AdvertisementFilter { get; init; } = _ => true;
```

**Default**: Accept all (`_ => true`)

Custom filter predicate for fine-grained advertisement filtering.

**Use Cases**:
- Filter by manufacturer data
- Filter by RSSI threshold
- Filter by device name patterns
- Complex multi-criteria filtering

**Example**:
```csharp
// Filter by signal strength
options.AdvertisementFilter = ad => ad.Rssi >= -70;

// Filter by manufacturer ID
options.AdvertisementFilter = ad =>
    ad.ManufacturerData?.Any(m => m.CompanyId == 0x004C) ?? false; // Apple

// Complex filtering
options.AdvertisementFilter = ad =>
{
    // Must have strong signal
    if (ad.Rssi < -70) return false;

    // Must advertise heart rate service
    if (!ad.ServiceUuids.Contains(HeartRateServiceUuid)) return false;

    // Must have name starting with "MyDevice"
    if (!ad.LocalName?.StartsWith("MyDevice") ?? true) return false;

    return true;
};
```

---

### RssiThreshold

```csharp
public int? RssiThreshold { get; init; }
```

**Default**: `null` (no filtering)

Filters out devices with RSSI (signal strength) below this threshold in dBm.

**Platform Support**:

| Platform | Support | Implementation |
|----------|---------|----------------|
| **Android** | Software | Post-reception filtering |
| **iOS/macOS** | Software | Post-reception filtering |
| **Windows** | Hardware | BluetoothLEAdvertisementWatcher.SignalStrengthFilter |

**RSSI Values**:
- `-30 dBm`: Very strong (device very close)
- `-50 dBm`: Strong
- `-70 dBm`: Moderate
- `-90 dBm`: Weak
- `-100 dBm`: Very weak (at edge of range)

**Example**:
```csharp
// Only discover nearby devices
options.RssiThreshold = -70; // Moderate or stronger

// Only discover very close devices
options.RssiThreshold = -50; // Strong signal required
```

---

## Scan Mode and Power Settings

### ScanMode

```csharp
public BluetoothScanMode ScanMode { get; init; } = BluetoothScanMode.Balanced;
```

**Default**: `BluetoothScanMode.Balanced`

Controls the power consumption vs. scan latency trade-off.

**Modes**:

| Mode | Power | Latency | Scan Interval | Use Case |
|------|-------|---------|---------------|----------|
| `LowPower` | Lowest | Highest | ~5 seconds | Background scanning, long-running scans |
| `Balanced` | Moderate | Moderate | ~2 seconds | Default for most apps |
| `LowLatency` | Highest | Lowest | Continuous | Fast discovery, time-sensitive apps |
| `Opportunistic` | Minimal | Variable | Piggyback on other scans | Extreme battery saving (Android 7.0+) |

**Platform Support**:

| Platform | Support | Notes |
|----------|---------|-------|
| **Android** | Full | Direct mapping to SCAN_MODE_* constants |
| **iOS/macOS** | Partial | Mapped via CBCentralManagerScanOptions |
| **Windows** | Partial | Mapped via sampling interval settings |

**Example**:
```csharp
// Fast discovery for time-sensitive operations
options.ScanMode = BluetoothScanMode.LowLatency;

// Battery-efficient background scanning
options.ScanMode = BluetoothScanMode.LowPower;

// Extreme battery saving (Android only, falls back to Balanced on other platforms)
options.ScanMode = BluetoothScanMode.Opportunistic;
```

---

### CallbackType

```csharp
public BluetoothScanCallbackType CallbackType { get; init; } = BluetoothScanCallbackType.AllMatches;
```

**Default**: `BluetoothScanCallbackType.AllMatches`

Controls when scan result callbacks are triggered.

**Values**:

| Value | Behavior | Use Case |
|-------|----------|----------|
| `None` | No callbacks | Testing, software-only filtering |
| `AllMatches` | All advertisements | Real-time tracking, RSSI monitoring |
| `FirstMatch` | First advertisement per device | Device discovery |
| `MatchLost` | When device disappears | Proximity detection |
| `FirstMatchAndMatchLost` | Both first match and lost | Presence detection |

**Platform Support**:

| Platform | Support | Notes |
|----------|---------|-------|
| **Android** | Full | CALLBACK_TYPE_* constants |
| **iOS/macOS** | AllMatches only | Always reports all advertisements |
| **Windows** | AllMatches only | Always reports all advertisements |

**Example**:
```csharp
// Discover devices once
options.CallbackType = BluetoothScanCallbackType.FirstMatch;

// Track device presence (Android only)
options.CallbackType = BluetoothScanCallbackType.FirstMatchAndMatchLost;
```

---

## Signal Strength Options

### SignalStrengthJitterSmoothing

```csharp
public SignalStrengthSmoothingOptions SignalStrengthJitterSmoothing { get; init; } = new SignalStrengthSmoothingOptions();
```

**Default**: `new SignalStrengthSmoothingOptions()`

Configuration for smoothing signal strength (RSSI) jitter using a moving average algorithm.

**SignalStrengthSmoothingOptions Properties**:

```csharp
public record SignalStrengthSmoothingOptions
{
    public int SmoothingOnAdvertisement { get; init; } = 5;
    public int SmoothingWhenConnected { get; init; } = 3;
}
```

**SmoothingOnAdvertisement** (default: 5):
- Stores last N RSSI values and averages them
- Used when device is not connected (signal more jittery)
- Higher value = smoother but less reactive

**SmoothingWhenConnected** (default: 3):
- Used when device is connected (signal more stable)
- Lower value = more reactive

**Example**:
```csharp
// More aggressive smoothing for very noisy environments
options.SignalStrengthJitterSmoothing = new SignalStrengthSmoothingOptions
{
    SmoothingOnAdvertisement = 10, // Very smooth
    SmoothingWhenConnected = 5
};

// No smoothing (raw RSSI values)
options.SignalStrengthJitterSmoothing = new SignalStrengthSmoothingOptions
{
    SmoothingOnAdvertisement = 1,
    SmoothingWhenConnected = 1
};

// Balanced (default)
options.SignalStrengthJitterSmoothing = new SignalStrengthSmoothingOptions();
```

---

## Extended Advertising

### EnableExtendedAdvertising

```csharp
public bool EnableExtendedAdvertising { get; init; }
```

**Default**: `false`

Enables extended advertising support (Bluetooth 5.0+ feature).

**Benefits**:
- Larger advertisement payload (up to 254 bytes vs 31 bytes)
- Multiple advertising sets
- Periodic advertising support
- Better coexistence with other Bluetooth operations

**Platform Support**:

| Platform | Support | Requirements |
|----------|---------|--------------|
| **Android** | API 26+ | Bluetooth 5.0 hardware |
| **iOS/macOS** | Automatic | Bluetooth 5.0 device (system-managed) |
| **Windows** | Windows 10 2004+ | Bluetooth 5.0 adapter |

**Example**:
```csharp
// Enable extended advertising on supported platforms
options.EnableExtendedAdvertising = true;
```

---

## Retry Configuration

### ScanStartRetry

```csharp
public RetryOptions? ScanStartRetry { get; init; } = RetryOptions.Default;
```

**Default**: `RetryOptions.Default` (3 retries, 200ms delay)

Retry configuration when starting the scanner fails due to transient issues.

**Common Failures**:
- Adapter busy
- Scan already started
- Throttling errors (Android)
- CBCentralManager state issues (iOS)

**RetryOptions Properties**:
```csharp
public record RetryOptions
{
    public int MaxRetries { get; init; } = 3;
    public TimeSpan DelayBetweenRetries { get; init; } = TimeSpan.FromMilliseconds(200);
    public bool ExponentialBackoff { get; init; } = false;
}
```

**Presets**:
- `RetryOptions.Default`: 3 retries, 200ms delay
- `RetryOptions.None`: No retries
- `RetryOptions.Aggressive`: 5 retries, 100ms base delay, exponential backoff

**Example**:
```csharp
// Default retry (recommended)
options.ScanStartRetry = RetryOptions.Default;

// No retry (fail fast)
options.ScanStartRetry = RetryOptions.None;

// Aggressive retry for unreliable environments
options.ScanStartRetry = RetryOptions.Aggressive;

// Custom retry
options.ScanStartRetry = new RetryOptions
{
    MaxRetries = 5,
    DelayBetweenRetries = TimeSpan.FromMilliseconds(500),
    ExponentialBackoff = true
};
```

---

## Android-Specific Options

### Android Property

```csharp
public object? Android { get; init; }
```

**Default**: `null`

Android platform-specific scanning options. Cast to `Bluetooth.Maui.Platforms.Droid.Scanning.Options.AndroidScanningOptions`.

**AndroidScanningOptions Properties**:

```csharp
public record AndroidScanningOptions
{
    public ScanMatchMode? MatchMode { get; init; }
    public ScanMatchNumber? ScanMatchNumber { get; init; }
    public TimeSpan? ReportDelay { get; init; }
    public ScanPhy? Phy { get; init; }
    public bool? Legacy { get; init; }
}
```

#### MatchMode (API 23+)

Controls filter matching aggressiveness:
- `Aggressive`: Fewer false positives (default)
- `Sticky`: Higher discovery rate

#### ScanMatchNumber (API 23+)

Controls callback frequency:
- `One`: Report after 1 match (fastest)
- `Few`: Report after few matches
- `Max`: Report after many matches (reduces callbacks)

#### ReportDelay (API 23+)

Delay before reporting results (batching):
- `TimeSpan.Zero`: Immediate (default)
- Longer delay: Better battery life

#### Phy (API 26+)

Bluetooth 5.0 PHY selection:
- `AllSupported`: All PHYs (default)
- `Le1M`: 1M PHY only
- `Le2M`: 2M PHY (high throughput)
- `LeCoded`: Coded PHY (long range)

#### Legacy (API 26+)

- `false`: Legacy and extended (default)
- `true`: Legacy only

**Example**:
```csharp
#if ANDROID
using Bluetooth.Maui.Platforms.Droid.Scanning.Options;
#endif

var options = new ScanningOptions
{
    ScanMode = BluetoothScanMode.LowPower,
#if ANDROID
    Android = new AndroidScanningOptions
    {
        MatchMode = ScanMatchMode.Sticky,
        ScanMatchNumber = ScanMatchNumber.Few,
        ReportDelay = TimeSpan.FromSeconds(5), // Batch results
        Phy = ScanPhy.Le1M, // Standard BLE
        Legacy = false // Support extended advertising
    }
#endif
};
```

---

## Usage Examples

### Quick Device Discovery

```csharp
var options = new ScanningOptions
{
    ScanMode = BluetoothScanMode.LowLatency,
    IgnoreDuplicateAdvertisements = true,
    CallbackType = BluetoothScanCallbackType.FirstMatch
};

await _scanner.StartScanningAsync(options);
```

### Service-Specific Scanning

```csharp
var options = new ScanningOptions
{
    ServiceUuids = new[]
    {
        Guid.Parse("0000180D-0000-1000-8000-00805F9B34FB") // Heart Rate
    },
    ScanMode = BluetoothScanMode.Balanced,
    IgnoreNamelessAdvertisements = true
};

await _scanner.StartScanningAsync(options);
```

### Battery-Efficient Background Scanning

```csharp
var options = new ScanningOptions
{
    ScanMode = BluetoothScanMode.LowPower,
    IgnoreDuplicateAdvertisements = true,
    RequireBackgroundLocation = true, // Android only
    ServiceUuids = new[] { myServiceUuid } // Reduce traffic
};

await _scanner.StartScanningAsync(options);
```

### Proximity-Based Scanning

```csharp
var options = new ScanningOptions
{
    ScanMode = BluetoothScanMode.LowLatency,
    IgnoreDuplicateAdvertisements = false, // Track RSSI changes
    RssiThreshold = -70, // Nearby devices only
    SignalStrengthJitterSmoothing = new SignalStrengthSmoothingOptions
    {
        SmoothingOnAdvertisement = 10 // Smooth RSSI
    }
};

await _scanner.StartScanningAsync(options);
```

### Production Configuration with Retry

```csharp
var options = new ScanningOptions
{
    PermissionStrategy = PermissionRequestStrategy.RequestAutomatically,
    ScanMode = BluetoothScanMode.Balanced,
    ServiceUuids = requiredServiceUuids,
    IgnoreNamelessAdvertisements = true,
    ScanStartRetry = RetryOptions.Aggressive, // Handle transient failures
    SignalStrengthJitterSmoothing = new SignalStrengthSmoothingOptions()
};

await _scanner.StartScanningAsync(options);
```

---

## Best Practices

### 1. Always Specify Service UUIDs When Possible

Improves performance and battery life:
```csharp
// Good - filtered scanning
options.ServiceUuids = new[] { myServiceUuid };

// Avoid - scans for all devices
options.ServiceUuids = null;
```

### 2. Use Appropriate Scan Mode

Match scan mode to use case:
```csharp
// User waiting for device picker
options.ScanMode = BluetoothScanMode.LowLatency;

// Background monitoring
options.ScanMode = BluetoothScanMode.LowPower;
```

### 3. Enable Duplicate Filtering for Discovery

Save battery when only discovering devices:
```csharp
options.IgnoreDuplicateAdvertisements = true;
options.CallbackType = BluetoothScanCallbackType.FirstMatch;
```

### 4. Keep Duplicate Advertisements for Tracking

When monitoring RSSI or presence:
```csharp
options.IgnoreDuplicateAdvertisements = false;
options.CallbackType = BluetoothScanCallbackType.AllMatches;
```

### 5. Use Retry Options

Handle transient failures gracefully:
```csharp
options.ScanStartRetry = RetryOptions.Aggressive; // Production
```

### 6. Platform-Specific Optimization

Use Android-specific options when available:
```csharp
#if ANDROID
options.Android = new AndroidScanningOptions
{
    ReportDelay = TimeSpan.FromSeconds(5) // Batch results
};
#endif
```

### 7. Filter in Multiple Stages

Combine hardware and software filtering:
```csharp
options.ServiceUuids = new[] { myServiceUuid }; // Hardware filter
options.RssiThreshold = -70; // Software filter
options.AdvertisementFilter = ad => ValidateDevice(ad); // Custom logic
```

---

## Related Documentation

- [Dependency-Injection.md](./Dependency-Injection.md) - DI configuration guide
- [Infrastructure-Options.md](./Infrastructure-Options.md) - Infrastructure configuration
- [Connection-Options.md](./Connection-Options.md) - Connection configuration
- [Exploration-Options.md](./Exploration-Options.md) - Service exploration configuration
