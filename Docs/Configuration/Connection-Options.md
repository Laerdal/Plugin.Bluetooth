# Connection Options

`ConnectionOptions` represents the configuration for Bluetooth Low Energy device connection operations. These options control how connections are established, including retry behavior, permissions, and platform-specific connection parameters.

## Table of Contents

- [Overview](#overview)
- [Basic Usage](#basic-usage)
- [Permission Options](#permission-options)
- [Connection Behavior](#connection-behavior)
- [Retry Configuration](#retry-configuration)
- [Platform-Specific Options](#platform-specific-options)
  - [Apple (iOS/macOS)](#apple-iosmacos-options)
  - [Android](#android-options)
  - [Windows](#windows-options)
- [Connection Priority and Transport](#connection-priority-and-transport)
- [Usage Examples](#usage-examples)
- [Best Practices](#best-practices)

---

## Overview

Connection options are passed to `IBluetoothDevice.ConnectAsync()` and control:
- Permission request behavior
- Connection retry logic (critical for Android GATT error 133)
- Wait for advertisement before connecting
- Platform-specific connection parameters
- Connection priority (Android)
- Transport type (Android)
- Background notification (iOS/macOS)

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

    public async Task ConnectToDevice(IBluetoothDevice device)
    {
        var options = new ConnectionOptions
        {
            PermissionStrategy = PermissionRequestStrategy.RequestAutomatically,
            ConnectionRetry = RetryOptions.Default,
            WaitForAdvertisementBeforeConnecting = false
        };

        await device.ConnectAsync(options);
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

Controls how Bluetooth connection permissions are requested.

**Values**:

| Value | Behavior | Use Case |
|-------|----------|----------|
| `RequestAutomatically` | Automatically requests permissions before connecting | Most applications (recommended) |
| `ThrowIfNotGranted` | Throws exception if permissions not granted | Custom permission flow |
| `AssumeGranted` | Skips permission checks (assumes granted) | Manual permission management |

**Platform Requirements**:

| Platform | Permission | When Required |
|----------|-----------|---------------|
| **Android** | BLUETOOTH_CONNECT | Android 12+ (API 31+) |
| **iOS/macOS** | Bluetooth access | System handles automatically |
| **Windows** | Radio access | Checked before connection |

**Example**:
```csharp
// Automatic (recommended)
options.PermissionStrategy = PermissionRequestStrategy.RequestAutomatically;

// Custom permission flow
options.PermissionStrategy = PermissionRequestStrategy.ThrowIfNotGranted;
try
{
    await device.ConnectAsync(options);
}
catch (BluetoothPermissionException)
{
    // Handle permission error
    await RequestPermissionsManually();
}
```

---

## Connection Behavior

### WaitForAdvertisementBeforeConnecting

```csharp
public bool WaitForAdvertisementBeforeConnecting { get; init; }
```

**Default**: `false`

Controls whether to wait for a recent advertisement before attempting connection.

**When true**:
- Scanner waits for fresh advertisement from device
- Ensures device is in range and advertising
- May improve connection success rate
- Adds latency to connection

**When false (default)**:
- Connects immediately without waiting
- Faster connection attempt
- May fail if device is out of range

**Use Cases**:
- **Enable**: Devices with intermittent advertising, long-range connections
- **Disable** (default): Reliable nearby devices, fast connection required

**Example**:
```csharp
// Wait for advertisement (slower but more reliable)
options.WaitForAdvertisementBeforeConnecting = true;

// Connect immediately (faster but may fail if device gone)
options.WaitForAdvertisementBeforeConnecting = false;
```

---

## Retry Configuration

### ConnectionRetry

```csharp
public RetryOptions? ConnectionRetry { get; init; } = RetryOptions.Default;
```

**Default**: `RetryOptions.Default` (3 retries, 200ms delay)

Retry configuration when device connection fails due to transient issues.

**Critical for Android**: GATT error 133 (connection failures) is common and often resolves with retry.

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
- `RetryOptions.Default`: 3 retries, 200ms delay (recommended)
- `RetryOptions.None`: No retries (fail fast)
- `RetryOptions.Aggressive`: 5 retries, 100ms base delay, exponential backoff

**Platform-Specific Failures**:

| Platform | Common Failures | Retry Helps? |
|----------|----------------|--------------|
| **Android** | GATT error 133 | Yes (critical) |
| **iOS/macOS** | Connection timeout, peripheral unavailable | Yes |
| **Windows** | Device connection failures | Yes |

**Example**:
```csharp
// Default retry (recommended for production)
options.ConnectionRetry = RetryOptions.Default;

// No retry (fail fast in development)
options.ConnectionRetry = RetryOptions.None;

// Aggressive retry for unreliable connections
options.ConnectionRetry = RetryOptions.Aggressive;

// Custom retry with exponential backoff
options.ConnectionRetry = new RetryOptions
{
    MaxRetries = 5,
    DelayBetweenRetries = TimeSpan.FromMilliseconds(500),
    ExponentialBackoff = true // 500ms, 1s, 2s, 4s, 8s
};
```

---

## Platform-Specific Options

### Apple (iOS/macOS) Options

#### AppleConnectionOptions

```csharp
public AppleConnectionOptions? Apple { get; init; }
```

**Default**: `null`

iOS/macOS platform-specific connection options using CoreBluetooth.

```csharp
public record AppleConnectionOptions
{
    public bool? NotifyOnConnection { get; init; }
    public bool? NotifyOnDisconnection { get; init; }
    public bool? NotifyOnNotification { get; init; }
    public bool? EnableTransportBridging { get; init; } // iOS 13+
    public bool? RequiresAncs { get; init; } // iOS 13+
}
```

#### NotifyOnConnection

Shows system alert when peripheral connects while app is suspended.

**Use Case**: Background connection monitoring

#### NotifyOnDisconnection

Shows system alert when peripheral disconnects while app is suspended.

**Use Case**: Critical device monitoring (medical, industrial)

#### NotifyOnNotification

Shows system alerts for incoming notifications while app is suspended.

**Use Case**: Real-time notification monitoring

#### EnableTransportBridging (iOS 13+)

Allows peripheral to be used over both Classic and LE transports.

**Use Case**: Dual-mode Bluetooth devices

#### RequiresAncs (iOS 13+)

Connection only succeeds if peripheral supports Apple Notification Center Service.

**Use Case**: Apple Watch accessories, notification-dependent devices

**Example**:
```csharp
options.Apple = new AppleConnectionOptions
{
    NotifyOnConnection = true,
    NotifyOnDisconnection = true,
    NotifyOnNotification = false,
    EnableTransportBridging = false,
    RequiresAncs = false
};
```

---

### Android Options

#### AndroidConnectionOptions

```csharp
public AndroidConnectionOptions? Android { get; init; }
```

**Default**: `null`

Android platform-specific connection options using Android BLE stack.

```csharp
public record AndroidConnectionOptions
{
    public bool? AutoConnect { get; init; }
    public BluetoothConnectionPriority? ConnectionPriority { get; init; }
    public BluetoothTransportType? TransportType { get; init; }
    public object? PreferredPhy { get; init; } // API 26+
    public RetryOptions? ServiceDiscoveryRetry { get; init; }
    public RetryOptions? GattWriteRetry { get; init; }
    public RetryOptions? GattReadRetry { get; init; }
}
```

#### AutoConnect

```csharp
public bool? AutoConnect { get; init; }
```

**Default**: `null` (uses platform default)

Controls automatic reconnection behavior.

**When true**:
- System automatically reconnects when device becomes available
- Longer initial connection time
- Automatic reconnection after disconnection

**When false** (recommended):
- Direct connection
- Shorter initial connection time
- Manual reconnection required

**Example**:
```csharp
options.Android = new AndroidConnectionOptions
{
    AutoConnect = false // Direct connection (recommended)
};
```

#### ConnectionPriority

```csharp
public BluetoothConnectionPriority? ConnectionPriority { get; init; }
```

**Default**: `null` (uses Balanced)

Controls connection parameters affecting latency, throughput, and power consumption.

**Values**:

| Priority | Interval | Latency | Throughput | Power | Use Case |
|----------|----------|---------|------------|-------|----------|
| `Balanced` | 30-50ms | Low | Moderate | Moderate | Default, general use |
| `High` | 11.25-15ms | Lowest | Highest | High | Real-time, gaming, audio |
| `LowPower` | 100-125ms | High | Lowest | Lowest | Infrequent updates, sensors |

**Example**:
```csharp
// High performance for real-time data
options.Android = new AndroidConnectionOptions
{
    ConnectionPriority = BluetoothConnectionPriority.High
};

// Battery-efficient for sensor monitoring
options.Android = new AndroidConnectionOptions
{
    ConnectionPriority = BluetoothConnectionPriority.LowPower
};
```

#### TransportType

```csharp
public BluetoothTransportType? TransportType { get; init; }
```

**Default**: `null` (uses Auto)

Controls which transport layer to use for connection.

**Values**:

| Type | Description | Use Case |
|------|-------------|----------|
| `Auto` | Automatically select (default) | Most devices |
| `Le` | Bluetooth Low Energy only | BLE devices (recommended) |
| `BrEdr` | Classic Bluetooth only | Not for BLE |

**Example**:
```csharp
options.Android = new AndroidConnectionOptions
{
    TransportType = BluetoothTransportType.Le // BLE only
};
```

#### PreferredPhy (API 26+)

```csharp
public object? PreferredPhy { get; init; }
```

**Default**: `null`

Specifies preferred PHY for connections on Android 8.0+ with Bluetooth 5.0 hardware.

**Values** (Android native constants):
- `LE_1M`: Standard BLE
- `LE_2M`: High throughput (Bluetooth 5.0+)
- `LE_CODED`: Long range (Bluetooth 5.0+)

**Example**:
```csharp
#if ANDROID
options.Android = new AndroidConnectionOptions
{
    PreferredPhy = Android.Bluetooth.BluetoothDevice.Phy_LE_2M // Type cast to Android type
};
#endif
```

#### GATT Operation Retry Options

Android-specific retry configuration for GATT operations:

**ServiceDiscoveryRetry** (default: 2 retries, 300ms delay):
```csharp
public RetryOptions? ServiceDiscoveryRetry { get; init; }
```

Used when DiscoverServices fails. Android BLE stack can fail service discovery due to timing issues.

**GattWriteRetry** (default: 3 retries, 200ms delay):
```csharp
public RetryOptions? GattWriteRetry { get; init; }
```

Used for characteristic write operations.

**GattReadRetry** (default: 2 retries, 100ms delay):
```csharp
public RetryOptions? GattReadRetry { get; init; }
```

Used for characteristic read operations.

**Example**:
```csharp
options.Android = new AndroidConnectionOptions
{
    AutoConnect = false,
    ConnectionPriority = BluetoothConnectionPriority.Balanced,
    TransportType = BluetoothTransportType.Le,

    // Increase retries for unreliable device
    ServiceDiscoveryRetry = new RetryOptions
    {
        MaxRetries = 5,
        DelayBetweenRetries = TimeSpan.FromMilliseconds(500)
    },
    GattWriteRetry = RetryOptions.Aggressive,
    GattReadRetry = RetryOptions.Default
};
```

---

### Windows Options

#### WindowsConnectionOptions

```csharp
public WindowsConnectionOptions? Windows { get; init; }
```

**Default**: `null`

Windows platform-specific connection options.

**Note**: Windows currently does not expose connection options through the WinRT API. Connection parameters are managed automatically by the Windows Bluetooth stack. This class is provided for consistency and future extensibility.

```csharp
public record WindowsConnectionOptions
{
    // Reserved for future Windows-specific connection options
}
```

---

## Connection Priority and Transport

### BluetoothConnectionPriority Enum

```csharp
public enum BluetoothConnectionPriority
{
    Balanced = 0,  // Default: 30-50ms interval
    High = 1,      // Low latency: 11.25-15ms interval
    LowPower = 2   // Power saving: 100-125ms interval
}
```

**Platform Support**: Android only (ignored on other platforms)

**Connection Parameters**:

| Priority | Interval | Slave Latency | Supervision Timeout |
|----------|----------|---------------|-------------------|
| Balanced | 30-50ms | 0 | 20s |
| High | 11.25-15ms | 0 | 20s |
| LowPower | 100-125ms | 2 | 20s |

---

### BluetoothTransportType Enum

```csharp
public enum BluetoothTransportType
{
    Auto = 0,   // Automatic selection
    Le = 2,     // Bluetooth Low Energy
    BrEdr = 1   // Classic Bluetooth (BR/EDR)
}
```

**Platform Support**: Android only (iOS/macOS always use LE)

---

## Usage Examples

### Basic Connection (Recommended)

```csharp
var options = new ConnectionOptions
{
    PermissionStrategy = PermissionRequestStrategy.RequestAutomatically,
    ConnectionRetry = RetryOptions.Default
};

await device.ConnectAsync(options);
```

### Reliable Connection with Retries

```csharp
var options = new ConnectionOptions
{
    PermissionStrategy = PermissionRequestStrategy.RequestAutomatically,
    ConnectionRetry = RetryOptions.Aggressive,
    WaitForAdvertisementBeforeConnecting = true
};

await device.ConnectAsync(options);
```

### High-Performance Android Connection

```csharp
var options = new ConnectionOptions
{
    ConnectionRetry = RetryOptions.Default,
    Android = new AndroidConnectionOptions
    {
        AutoConnect = false,
        ConnectionPriority = BluetoothConnectionPriority.High,
        TransportType = BluetoothTransportType.Le,
        GattWriteRetry = RetryOptions.Aggressive
    }
};

await device.ConnectAsync(options);
```

### Battery-Efficient Android Connection

```csharp
var options = new ConnectionOptions
{
    ConnectionRetry = RetryOptions.Default,
    Android = new AndroidConnectionOptions
    {
        AutoConnect = false,
        ConnectionPriority = BluetoothConnectionPriority.LowPower,
        TransportType = BluetoothTransportType.Le
    }
};

await device.ConnectAsync(options);
```

### Background Monitoring (iOS)

```csharp
var options = new ConnectionOptions
{
    ConnectionRetry = RetryOptions.Default,
    Apple = new AppleConnectionOptions
    {
        NotifyOnConnection = true,
        NotifyOnDisconnection = true,
        NotifyOnNotification = true
    }
};

await device.ConnectAsync(options);
```

### Production Configuration

```csharp
var options = new ConnectionOptions
{
    PermissionStrategy = PermissionRequestStrategy.RequestAutomatically,
    ConnectionRetry = RetryOptions.Aggressive,
    WaitForAdvertisementBeforeConnecting = false,

    #if ANDROID
    Android = new AndroidConnectionOptions
    {
        AutoConnect = false,
        ConnectionPriority = BluetoothConnectionPriority.Balanced,
        TransportType = BluetoothTransportType.Le,
        ServiceDiscoveryRetry = new RetryOptions
        {
            MaxRetries = 5,
            DelayBetweenRetries = TimeSpan.FromMilliseconds(500)
        },
        GattWriteRetry = RetryOptions.Aggressive,
        GattReadRetry = RetryOptions.Default
    },
    #elif IOS || MACCATALYST
    Apple = new AppleConnectionOptions
    {
        NotifyOnDisconnection = true
    }
    #endif
};

await device.ConnectAsync(options);
```

### Custom Permission Handling

```csharp
var options = new ConnectionOptions
{
    PermissionStrategy = PermissionRequestStrategy.ThrowIfNotGranted,
    ConnectionRetry = RetryOptions.Default
};

try
{
    await device.ConnectAsync(options);
}
catch (BluetoothPermissionException ex)
{
    // Show custom permission rationale UI
    var granted = await ShowPermissionRationaleAndRequest();

    if (granted)
    {
        options = options with
        {
            PermissionStrategy = PermissionRequestStrategy.AssumeGranted
        };
        await device.ConnectAsync(options);
    }
}
```

---

## Best Practices

### 1. Always Use Retry Logic

Connection failures are common, especially on Android:
```csharp
// Good - handles transient failures
options.ConnectionRetry = RetryOptions.Aggressive;

// Avoid - fails on first error
options.ConnectionRetry = RetryOptions.None;
```

### 2. Disable AutoConnect on Android

Direct connections are faster and more predictable:
```csharp
options.Android = new AndroidConnectionOptions
{
    AutoConnect = false // Recommended
};
```

### 3. Use Appropriate Connection Priority

Match priority to use case:
```csharp
// Real-time audio/control
options.Android = new AndroidConnectionOptions
{
    ConnectionPriority = BluetoothConnectionPriority.High
};

// Periodic sensor readings
options.Android = new AndroidConnectionOptions
{
    ConnectionPriority = BluetoothConnectionPriority.LowPower
};
```

### 4. Request Permissions Automatically

Simplifies permission handling:
```csharp
options.PermissionStrategy = PermissionRequestStrategy.RequestAutomatically;
```

### 5. Add GATT Operation Retries on Android

Especially important for unreliable devices:
```csharp
options.Android = new AndroidConnectionOptions
{
    ServiceDiscoveryRetry = RetryOptions.Aggressive,
    GattWriteRetry = RetryOptions.Aggressive
};
```

### 6. Wait for Advertisement When Needed

For devices with intermittent advertising:
```csharp
options.WaitForAdvertisementBeforeConnecting = true;
```

### 7. Handle Platform-Specific Scenarios

Use conditional compilation for platform-specific options:
```csharp
#if ANDROID
options.Android = new AndroidConnectionOptions { ... };
#elif IOS
options.Apple = new AppleConnectionOptions { ... };
#endif
```

### 8. Test Connection Failures

Simulate failures to ensure retry logic works:
```csharp
// Development: fail fast to find issues
#if DEBUG
options.ConnectionRetry = RetryOptions.None;
#else
options.ConnectionRetry = RetryOptions.Aggressive;
#endif
```

---

## Related Documentation

- [Dependency-Injection.md](./Dependency-Injection.md) - DI configuration guide
- [Infrastructure-Options.md](./Infrastructure-Options.md) - Infrastructure configuration
- [Scanning-Options.md](./Scanning-Options.md) - Scanner configuration
- [Exploration-Options.md](./Exploration-Options.md) - Service exploration configuration
- [L2CAP-Options.md](./L2CAP-Options.md) - L2CAP channel configuration
