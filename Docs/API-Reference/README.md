# API Reference

Complete technical reference for Plugin.Bluetooth interfaces, types, and conventions.

## Quick Links

- [Interfaces and Abstractions](./Abstractions.md) - Core interfaces for scanning and broadcasting
- [Enumerations](./Enums.md) - Enums for properties, permissions, and manufacturers
- [Events](./Events.md) - Event types and event arguments
- [Exceptions](./Exceptions.md) - Exception hierarchy and error handling

## API Conventions

### Async-First Design

All I/O operations in Plugin.Bluetooth are asynchronous and follow .NET async/await patterns:

```csharp
// Use ValueTask for commonly synchronous operations
ValueTask<bool> HasScannerPermissionsAsync();

// Use Task for operations that are always asynchronous
Task StartScanningAsync(ScanningOptions? options = null);
```

**Key principles:**
- All async methods end with `Async` suffix
- Operations that may complete synchronously use `ValueTask` or `ValueTask<T>`
- Operations that are always asynchronous use `Task` or `Task<T>`
- Never block on async methods (no `.Wait()` or `.Result`)

### CancellationToken Support

Most async operations accept a `CancellationToken` parameter for cooperative cancellation:

```csharp
await scanner.StartScanningAsync(
    options: scanOptions,
    timeout: TimeSpan.FromSeconds(30),
    cancellationToken: cancellationToken
);
```

**Usage guidelines:**
- Pass `CancellationToken.None` if cancellation is not needed
- Default value is always `default(CancellationToken)`
- Operations throw `OperationCanceledException` when cancelled

### Timeout Handling

Operations support optional timeout parameters to prevent indefinite waiting:

```csharp
// Default timeout (varies by operation, typically 30s)
await device.ConnectAsync();

// Custom timeout
await device.ConnectAsync(
    timeout: TimeSpan.FromSeconds(10)
);

// No timeout
await device.ConnectAsync(
    timeout: Timeout.InfiniteTimeSpan
);
```

**Timeout behavior:**
- Most operations have sensible default timeouts
- Pass `null` to use the default timeout
- Pass `Timeout.InfiniteTimeSpan` for no timeout
- Operations throw `TimeoutException` when they exceed the timeout

### State Management

Interfaces expose state through properties and events:

```csharp
// Check current state
if (scanner.IsRunning)
{
    await scanner.StopScanningAsync();
}

// Listen to state changes
scanner.RunningStateChanged += (s, e) =>
{
    Console.WriteLine($"Scanner running: {scanner.IsRunning}");
};

// Conditional operations with "IfNeeded" variants
await scanner.StartScanningIfNeededAsync(); // No-op if already running
await scanner.StopScanningIfNeededAsync();  // No-op if already stopped
```

**State properties:**
- `IsRunning` - Whether the operation is active
- `IsStarting` - Whether the operation is starting
- `IsStopping` - Whether the operation is stopping
- `IsConnecting` / `IsConnected` / `IsDisconnecting` - Connection states

### Memory Efficiency

The API uses modern .NET memory types for efficient data handling:

```csharp
// Read characteristic value
ReadOnlyMemory<byte> value = await characteristic.ReadValueAsync();

// Use as span for zero-copy access
ReadOnlySpan<byte> span = characteristic.ValueSpan;

// Listen for value updates
characteristic.ValueUpdated += (s, e) =>
{
    ReadOnlyMemory<byte> newValue = e.NewValue;
    ReadOnlyMemory<byte> oldValue = e.OldValue;
};
```

**Memory types:**
- `ReadOnlyMemory<byte>` - For asynchronous value handling
- `ReadOnlySpan<byte>` - For synchronous, high-performance access
- No unnecessary allocations or copies

### Disposal Pattern

All major interfaces implement `IAsyncDisposable`:

```csharp
await using var scanner = adapter.CreateScanner();
await scanner.StartScanningAsync();
// Automatically stopped and disposed

// Or manually
IBluetoothRemoteDevice device = await scanner.GetKnownDeviceAsync(id);
await device.ConnectAsync();
// ... use device ...
await device.DisposeAsync(); // Disconnect and cleanup
```

**Disposal guidelines:**
- Use `await using` for automatic cleanup
- Dispose operations stop ongoing activities
- Disposal is idempotent (safe to call multiple times)
- Always dispose devices when done to free resources

### Collection Management

The API exposes collections as `IReadOnlyList<T>` with change notification:

```csharp
// Access current devices
IReadOnlyList<IBluetoothRemoteDevice> devices = scanner.Devices;

// Listen for additions
scanner.DevicesAdded += (s, e) =>
{
    foreach (var device in e.Items)
    {
        Console.WriteLine($"Found: {device.Name}");
    }
};

// Listen for removals
scanner.DevicesRemoved += (s, e) =>
{
    foreach (var device in e.Items)
    {
        Console.WriteLine($"Lost: {device.Name}");
    }
};

// Listen for any change
scanner.DeviceListChanged += (s, e) =>
{
    var added = e.AddedItems;
    var removed = e.RemovedItems;
};
```

**Collection features:**
- Immutable snapshots via `IReadOnlyList<T>`
- Fine-grained change events (Added, Removed, Changed)
- Thread-safe enumeration
- LINQ compatible

### Permission Handling

Permission checks and requests follow a consistent pattern:

```csharp
// Check without prompting
bool hasPermission = await scanner.HasScannerPermissionsAsync();

// Request permission (may show system dialog)
try
{
    await scanner.RequestScannerPermissionsAsync();
}
catch (BluetoothPermissionException ex)
{
    Console.WriteLine($"Permission denied: {ex.Message}");
    // Check InnerException for platform-specific details
}
```

**Permission strategies:**
- Check before requesting to avoid unnecessary prompts
- Handle `BluetoothPermissionException` for denials
- Platform differences documented in XML comments
- See `PermissionRequestStrategy` for configuration options

### Error Handling

The API uses a structured exception hierarchy:

```csharp
try
{
    await characteristic.ReadValueAsync();
}
catch (DeviceNotConnectedException ex)
{
    // Device disconnected
    await ex.Device.ConnectAsync();
}
catch (CharacteristicCantReadException ex)
{
    // Characteristic doesn't support read
    Console.WriteLine($"Can't read {ex.RemoteCharacteristic.Name}");
}
catch (CharacteristicReadException ex)
{
    // Read operation failed
    Console.WriteLine($"Read failed: {ex.Message}");
}
catch (TimeoutException)
{
    // Operation timed out
}
catch (OperationCanceledException)
{
    // Operation was cancelled
}
```

**Exception handling:**
- Specific exceptions for each error case
- Base classes for category-level catching
- Context properties (Device, Characteristic, etc.)
- Static guard methods for precondition checking
- See [Exceptions](./Exceptions.md) for full hierarchy

### Configuration Options

Operations accept configuration through options objects:

```csharp
// Scanning configuration
var scanOptions = new ScanningOptions
{
    ScanMode = BluetoothScanMode.LowLatency,
    FilterByServices = new[] { serviceUuid },
    ReportDelayMillis = 0
};
await scanner.StartScanningAsync(scanOptions);

// Connection configuration
var connectionOptions = new ConnectionOptions
{
    AutoConnect = true,
    ConnectionPriority = BluetoothConnectionPriority.Balanced,
    EnableTransport = BluetoothTransport.Le
};
await device.ConnectAsync(connectionOptions);

// Broadcasting configuration
var broadcastOptions = new BroadcastingOptions
{
    LocalName = "MyDevice",
    Connectable = true,
    AdvertiseMode = BluetoothAdvertiseMode.LowLatency
};
await broadcaster.StartBroadcastingAsync(broadcastOptions);
```

**Options pattern:**
- Immutable configuration objects
- Default values for all properties
- Platform-specific options documented
- Can be updated while running via `Update*OptionsAsync` methods

## Namespaces

### Bluetooth.Abstractions

Core abstractions and shared types:
- `IBluetoothAdapter` - Main entry point for Bluetooth functionality
- `BluetoothException` - Base exception type
- `CharacteristicProperties`, `CharacteristicPermissions` - Core enums
- `PhyMode`, `Manufacturer` - Hardware-related enums

Related concepts: [Getting Started](../README.md)

### Bluetooth.Abstractions.Scanning

Central/client role - scanning and connecting to peripherals:
- `IBluetoothScanner` - Device discovery and scanning
- `IBluetoothRemoteDevice` - Remote device connection and services
- `IBluetoothRemoteService` - GATT service representation
- `IBluetoothRemoteCharacteristic` - GATT characteristic read/write/notify
- `IBluetoothRemoteDescriptor` - GATT descriptor access
- Extensive exception hierarchy for error handling

Related concepts: [Device Scanning](../SCANNING.md), [Connecting to Devices](../CONNECTING.md)

### Bluetooth.Abstractions.Broadcasting

Peripheral/server role - advertising and serving GATT services:
- `IBluetoothBroadcaster` - Advertising and service management
- `IBluetoothLocalService` - Local GATT service creation
- `IBluetoothLocalCharacteristic` - Local characteristic with read/write callbacks
- `IBluetoothLocalDescriptor` - Local descriptor management
- `IBluetoothConnectedDevice` - Connected central device information

Related concepts: [Broadcasting](../BROADCASTING.md)

## Platform-Specific Behavior

The API is designed for cross-platform use but respects platform differences:

```csharp
// This call works on all platforms
await device.RequestConnectionPriorityAsync(BluetoothConnectionPriority.High);

// But behavior differs:
// - Android: Actually changes connection parameters
// - iOS/macOS: System-managed, method is no-op
// - Windows: System-managed, method is no-op
```

**How to handle platform differences:**
1. Check XML documentation for platform-specific remarks
2. Platform differences are noted with `<b>Platform Support:</b>` sections
3. No-op implementations don't throw exceptions
4. Use conditional compilation if platform-specific behavior is required

## Thread Safety

**General thread safety:**
- All public methods are thread-safe
- Events may be raised on any thread
- Property reads are thread-safe
- Collections are thread-safe for enumeration

**UI thread marshalling:**
```csharp
// Events may fire on background threads
scanner.DevicesAdded += async (s, e) =>
{
    // Marshal to UI thread if needed
    await MainThread.InvokeOnMainThreadAsync(() =>
    {
        foreach (var device in e.Items)
        {
            DeviceList.Add(device);
        }
    });
};
```

## Best Practices

### Resource Management

```csharp
// GOOD: Automatic cleanup
await using var scanner = adapter.CreateScanner();
await scanner.StartScanningAsync();

// GOOD: Manual cleanup
var device = await scanner.GetKnownDeviceAsync(id);
try
{
    await device.ConnectAsync();
    // Use device...
}
finally
{
    await device.DisposeAsync();
}

// BAD: No cleanup (resource leak)
var scanner = adapter.CreateScanner();
await scanner.StartScanningAsync();
// Scanner keeps running, battery drain
```

### Connection Reliability

```csharp
// GOOD: Handle unexpected disconnections
device.UnexpectedDisconnection += async (s, e) =>
{
    logger.LogWarning($"Lost connection: {e.Reason}");

    // Attempt reconnect
    await Task.Delay(1000);
    await device.ConnectIfNeededAsync();
};

// GOOD: Verify connection before operations
DeviceNotConnectedException.ThrowIfNotConnected(device);
await characteristic.ReadValueAsync();

// GOOD: Use conditional connect
await device.ConnectIfNeededAsync(); // Safe to call multiple times
```

### Error Recovery

```csharp
// GOOD: Specific exception handling with retry
int retries = 3;
while (retries > 0)
{
    try
    {
        return await characteristic.ReadValueAsync();
    }
    catch (CharacteristicReadException ex) when (retries > 1)
    {
        logger.LogWarning($"Read failed, retrying... ({retries} left)");
        retries--;
        await Task.Delay(500);
    }
}
```

### Performance Optimization

```csharp
// GOOD: Use ValueSpan for synchronous access
byte[] ProcessValue()
{
    ReadOnlySpan<byte> span = characteristic.ValueSpan;
    return span.ToArray(); // Only allocate if needed
}

// GOOD: Batch operations
await device.ConnectAsync();
var service = await device.GetServiceAsync(serviceUuid);
var chars = await Task.WhenAll(
    service.GetCharacteristicAsync(uuid1),
    service.GetCharacteristicAsync(uuid2),
    service.GetCharacteristicAsync(uuid3)
);

// BAD: Sequential when parallel would work
await device.ConnectAsync();
var service = await device.GetServiceAsync(serviceUuid);
var char1 = await service.GetCharacteristicAsync(uuid1);
var char2 = await service.GetCharacteristicAsync(uuid2);
var char3 = await service.GetCharacteristicAsync(uuid3);
```

## See Also

- [Interfaces and Abstractions](./Abstractions.md)
- [Enumerations](./Enums.md)
- [Events](./Events.md)
- [Exceptions](./Exceptions.md)
- [Getting Started Guide](../README.md)
- [Troubleshooting](../TROUBLESHOOTING.md)
