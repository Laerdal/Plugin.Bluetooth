# Infrastructure Options

`BluetoothInfrastructureOptions` defines infrastructure-level configuration for Bluetooth services that are set at application startup via dependency injection. These options control app-wide defaults and behaviors that rarely change during runtime.

## Table of Contents

- [Overview](#overview)
- [Key Distinction](#key-distinction)
- [Configuration](#configuration)
- [Properties](#properties)
- [Usage Examples](#usage-examples)
- [Best Practices](#best-practices)

---

## Overview

Infrastructure options are configured once during application startup and affect the behavior of all Bluetooth operations throughout the application lifecycle. They define:

- Resource cleanup behavior
- Default timeouts for operations
- Connection concurrency limits
- Logging verbosity
- Exception handling strategy

**Namespace**: `Bluetooth.Abstractions.Options`

**Configuration via**: `IOptions<BluetoothInfrastructureOptions>`

---

## Key Distinction

Understanding the difference between infrastructure options and operation options is crucial:

| Aspect | Infrastructure Options | Operation Options |
|--------|----------------------|-------------------|
| **When Set** | Once at app startup via DI | Per operation (scanning, connection, etc.) |
| **Scope** | App-wide defaults and behaviors | Specific to individual operations |
| **Pattern** | `Configure<T>()` in `MauiProgram.cs` | Passed to method calls |
| **Examples** | Timeouts, logging, cleanup | Scan mode, service filters, retry policies |
| **Mutability** | Immutable after startup | Can vary per operation |

### Infrastructure Options (This Document)
```csharp
builder.Services.Configure<BluetoothInfrastructureOptions>(options =>
{
    options.DefaultOperationTimeout = TimeSpan.FromSeconds(30);
    options.EnableVerboseLogging = true;
});
```

### Operation Options (See Operation-Specific Docs)
```csharp
await scanner.StartScanningAsync(new ScanningOptions
{
    ScanMode = BluetoothScanMode.LowLatency,
    ServiceUuids = new[] { myServiceUuid }
});
```

---

## Configuration

### Basic Configuration

Configure in your `MauiProgram.cs`:

```csharp
public static MauiApp CreateMauiApp()
{
    var builder = MauiApp.CreateBuilder();

    builder.Services.AddBluetoothServices();

    builder.Services.Configure<BluetoothInfrastructureOptions>(options =>
    {
        options.AutoCleanupOnStop = true;
        options.DefaultOperationTimeout = TimeSpan.FromSeconds(30);
        options.MaxConcurrentConnections = 5;
        options.EnableVerboseLogging = false;
    });

    return builder.Build();
}
```

### Configuration with Core Services

Alternatively, configure options directly when adding core services:

```csharp
builder.Services.AddBluetoothCoreServices(options =>
{
    options.EnableVerboseLogging = true;
    options.DefaultOperationTimeout = TimeSpan.FromMinutes(1);
});
```

### Configuration from appsettings.json

```json
{
  "Bluetooth": {
    "Infrastructure": {
      "AutoCleanupOnStop": true,
      "AutoDisposeDevicesOnRemoval": true,
      "AutoDisposeServicesOnRemoval": true,
      "DefaultOperationTimeout": "00:00:30",
      "MaxConcurrentConnections": 5,
      "EnableVerboseLogging": false,
      "ThrowOnUnhandledExceptions": false
    }
  }
}
```

Bind in `MauiProgram.cs`:

```csharp
builder.Services.Configure<BluetoothInfrastructureOptions>(
    builder.Configuration.GetSection("Bluetooth:Infrastructure"));
```

---

## Properties

### AutoCleanupOnStop

```csharp
public bool AutoCleanupOnStop { get; init; } = true;
```

**Default**: `true`

Controls whether to automatically clean up resources when scanner or broadcaster stops.

**When enabled**:
- Scanner stop will disconnect all connected devices and clear device list
- Broadcaster stop will disconnect all clients and clear service list

**Use Cases**:
- **Enable** (default): Most applications where you want clean state after stopping
- **Disable**: Applications that need to preserve device connections across scanner restarts

**Example**:
```csharp
options.AutoCleanupOnStop = true; // Clean up on stop (default)
```

---

### AutoDisposeDevicesOnRemoval

```csharp
public bool AutoDisposeDevicesOnRemoval { get; init; } = true;
```

**Default**: `true`

Controls whether to automatically dispose devices when they are removed from the scanner's device list.

**When enabled**:
- Calling `Scanner.RemoveDevice(device)` disposes the device
- Calling `Scanner.ClearDevices()` disposes all devices

**Use Cases**:
- **Enable** (default): Automatic memory management, prevents leaks
- **Disable**: Manual control over device lifecycle, keeping device references

**Example**:
```csharp
options.AutoDisposeDevicesOnRemoval = true; // Auto-dispose (default)
```

---

### AutoDisposeServicesOnRemoval

```csharp
public bool AutoDisposeServicesOnRemoval { get; init; } = true;
```

**Default**: `true`

Controls whether to automatically dispose services when they are removed from a device's service list.

**When enabled**:
- Calling `Device.ClearServices()` disposes all service instances
- Disposal cascades to characteristics and descriptors

**Use Cases**:
- **Enable** (default): Automatic resource cleanup throughout GATT hierarchy
- **Disable**: Manual control over service lifecycle

**Example**:
```csharp
options.AutoDisposeServicesOnRemoval = true; // Auto-dispose (default)
```

---

### DefaultOperationTimeout

```csharp
public TimeSpan? DefaultOperationTimeout { get; init; } = TimeSpan.FromSeconds(30);
```

**Default**: `30 seconds`

**Nullable**: `true` (set to `null` for no timeout)

Default timeout for operations that don't explicitly specify a timeout.

**Applies to**:
- Connect/Disconnect operations
- Read/Write characteristic values
- Service/Characteristic/Descriptor exploration
- Start/Stop scanning or broadcasting
- L2CAP channel operations (if not overridden by L2CapChannelOptions)

**Use Cases**:
- **30 seconds** (default): Suitable for most BLE operations
- **Longer** (60+ seconds): Slow devices, poor signal, firmware updates
- **Shorter** (10-15 seconds): Quick operations, fast failure detection
- **null**: No timeout (wait indefinitely) - use with caution

**Example**:
```csharp
// Standard configuration
options.DefaultOperationTimeout = TimeSpan.FromSeconds(30);

// For slow devices or poor connectivity
options.DefaultOperationTimeout = TimeSpan.FromMinutes(2);

// For fast failure detection
options.DefaultOperationTimeout = TimeSpan.FromSeconds(10);

// No timeout (not recommended)
options.DefaultOperationTimeout = null;
```

---

### MaxConcurrentConnections

```csharp
public int MaxConcurrentConnections { get; init; } = 5;
```

**Default**: `5`

**Range**: `0` (unlimited) or positive integer

Maximum number of concurrent connection attempts allowed to prevent resource exhaustion.

**Behavior**:
- When limit reached, new connection attempts wait until slot becomes available
- Set to `0` for unlimited concurrent connections (not recommended)

**Platform Limits** (informational):
- **Android**: Typically 4-7 simultaneous GATT connections
- **iOS**: Typically 10-15 simultaneous connections
- **Windows**: Varies by adapter and Windows version

**Use Cases**:
- **5** (default): Good balance for most applications
- **Lower** (2-3): Resource-constrained devices, simple applications
- **Higher** (10+): Multi-device management systems (be aware of platform limits)
- **0** (unlimited): Not recommended - can exhaust system resources

**Example**:
```csharp
// Conservative for resource-constrained scenarios
options.MaxConcurrentConnections = 3;

// Aggressive for multi-device management
options.MaxConcurrentConnections = 10;
```

---

### EnableVerboseLogging

```csharp
public bool EnableVerboseLogging { get; init; }
```

**Default**: `false`

Enables or disables verbose logging for all Bluetooth operations.

**When enabled, logs include**:
- State transitions (adapter, scanner, device states)
- Native API calls with parameters
- Data transfers (hex dumps of read/write operations)
- Timing information for operations
- Detailed error information

**Performance Impact**:
- Significantly increases log output
- Can impact performance in high-frequency operations
- May increase memory usage

**Use Cases**:
- **Disable** (default): Production environment
- **Enable**: Development, debugging, troubleshooting connectivity issues

**Example**:
```csharp
#if DEBUG
options.EnableVerboseLogging = true;
#else
options.EnableVerboseLogging = false;
#endif
```

---

### ThrowOnUnhandledExceptions

```csharp
public bool ThrowOnUnhandledExceptions { get; init; }
```

**Default**: `false`

Controls whether unhandled Bluetooth errors should throw exceptions or be dispatched to the exception listener.

**When false (default)**:
- Unhandled exceptions are sent to `BluetoothUnhandledExceptionListener`
- Application continues running
- Exceptions can be observed via listener

**When true**:
- Unhandled exceptions are thrown directly
- May crash application if not caught
- Useful for fail-fast debugging

**Use Cases**:
- **false** (default): Production apps, graceful error handling
- **true**: Development/debugging, finding exception sources

**Example**:
```csharp
#if DEBUG
options.ThrowOnUnhandledExceptions = true; // Fail fast in debug
#else
options.ThrowOnUnhandledExceptions = false; // Graceful in production
#endif
```

---

## Usage Examples

### Production Configuration

```csharp
builder.Services.Configure<BluetoothInfrastructureOptions>(options =>
{
    options.AutoCleanupOnStop = true;
    options.AutoDisposeDevicesOnRemoval = true;
    options.AutoDisposeServicesOnRemoval = true;
    options.DefaultOperationTimeout = TimeSpan.FromSeconds(30);
    options.MaxConcurrentConnections = 5;
    options.EnableVerboseLogging = false;
    options.ThrowOnUnhandledExceptions = false;
});
```

### Development Configuration

```csharp
builder.Services.Configure<BluetoothInfrastructureOptions>(options =>
{
    options.AutoCleanupOnStop = true;
    options.DefaultOperationTimeout = TimeSpan.FromMinutes(2); // Longer for debugging
    options.MaxConcurrentConnections = 5;
    options.EnableVerboseLogging = true; // Debug logging
    options.ThrowOnUnhandledExceptions = true; // Fail fast
});
```

### Multi-Device Management

```csharp
builder.Services.Configure<BluetoothInfrastructureOptions>(options =>
{
    options.AutoCleanupOnStop = false; // Preserve connections
    options.AutoDisposeDevicesOnRemoval = false; // Manual disposal
    options.DefaultOperationTimeout = TimeSpan.FromSeconds(45);
    options.MaxConcurrentConnections = 10; // Support more devices
    options.EnableVerboseLogging = false;
});
```

### IoT/Embedded Configuration

```csharp
builder.Services.Configure<BluetoothInfrastructureOptions>(options =>
{
    options.AutoCleanupOnStop = true;
    options.AutoDisposeDevicesOnRemoval = true;
    options.AutoDisposeServicesOnRemoval = true;
    options.DefaultOperationTimeout = TimeSpan.FromSeconds(60); // Slower devices
    options.MaxConcurrentConnections = 2; // Resource-constrained
    options.EnableVerboseLogging = false;
});
```

### Environment-Based Configuration

```csharp
var isDevelopment = builder.Environment.IsDevelopment();

builder.Services.Configure<BluetoothInfrastructureOptions>(options =>
{
    options.AutoCleanupOnStop = true;
    options.DefaultOperationTimeout = isDevelopment
        ? TimeSpan.FromMinutes(2)
        : TimeSpan.FromSeconds(30);
    options.MaxConcurrentConnections = 5;
    options.EnableVerboseLogging = isDevelopment;
    options.ThrowOnUnhandledExceptions = isDevelopment;
});
```

---

## Best Practices

### 1. Use Defaults When Possible
The default values are suitable for most applications. Only customize when you have specific requirements.

```csharp
// Only configure what you need to change
builder.Services.Configure<BluetoothInfrastructureOptions>(options =>
{
    options.EnableVerboseLogging = true; // Only change this
});
```

### 2. Environment-Specific Configuration
Use different configurations for development vs. production:

```csharp
#if DEBUG
builder.Services.Configure<BluetoothInfrastructureOptions>(options =>
{
    options.EnableVerboseLogging = true;
    options.ThrowOnUnhandledExceptions = true;
});
#endif
```

### 3. Document Custom Timeouts
When increasing timeouts, document why:

```csharp
// Increased timeout for firmware update operations which can take 60-90s
options.DefaultOperationTimeout = TimeSpan.FromMinutes(2);
```

### 4. Be Mindful of Platform Limits
Don't exceed platform connection limits:

```csharp
// Android typically supports 4-7 simultaneous connections
options.MaxConcurrentConnections = 5; // Safe for all platforms
```

### 5. Profile Before Enabling Verbose Logging
Verbose logging impacts performance. Use sparingly:

```csharp
// Only enable for specific debugging sessions
var enableDebugLogging = builder.Configuration.GetValue<bool>("Bluetooth:DebugMode");
options.EnableVerboseLogging = enableDebugLogging;
```

### 6. Consider Memory Implications
Auto-disposal options help prevent memory leaks:

```csharp
// Recommended for most apps
options.AutoDisposeDevicesOnRemoval = true;
options.AutoDisposeServicesOnRemoval = true;
```

### 7. Validate Configuration
Use options validation to catch configuration errors early:

```csharp
builder.Services.AddOptions<BluetoothInfrastructureOptions>()
    .Configure(options =>
    {
        options.MaxConcurrentConnections = 10;
    })
    .Validate(options =>
    {
        return options.MaxConcurrentConnections > 0 ||
               options.MaxConcurrentConnections == 0; // 0 = unlimited
    }, "MaxConcurrentConnections must be 0 (unlimited) or positive");
```

---

## Related Documentation

- [Dependency-Injection.md](./Dependency-Injection.md) - DI configuration guide
- [Scanning-Options.md](./Scanning-Options.md) - Scanner configuration
- [Connection-Options.md](./Connection-Options.md) - Connection configuration
- [L2CAP-Options.md](./L2CAP-Options.md) - L2CAP channel configuration
- [Exploration-Options.md](./Exploration-Options.md) - Service exploration configuration
- [Broadcasting-Options.md](./Broadcasting-Options.md) - Broadcasting configuration
