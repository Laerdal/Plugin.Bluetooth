# GitHub Copilot Instructions for Plugin.Bluetooth

## Project Overview

**Plugin.Bluetooth** is a cross-platform .NET MAUI Bluetooth Low Energy (BLE) library providing a unified API for Android, iOS, and Windows platforms. The library abstracts platform-specific BLE implementations into a consistent, easy-to-use interface for scanning, connecting, and communicating with Bluetooth devices.

### Key Information

- **License**: MIT (Copyright 2025 Laerdal Medical)
- **Target Framework**: .NET 10.0
- **Supported Platforms**: Android 36.1, iOS, macOS Catalyst, Windows 10.0.22621.0
- **Primary Author**: Francois Raminosona
- **Organization**: Laerdal Medical

## Architecture

### Project Structure

```
Plugin.Bluetooth/
├── Bluetooth.Core/              # Platform-agnostic core library
│   ├── Abstractions/            # Interfaces (IBluetoothScanner, IBluetoothDevice, etc.)
│   ├── BaseClasses/             # Base implementations with partial classes
│   ├── BluetoothSigSpecific/    # Bluetooth SIG standard definitions
│   ├── CharacteristicAccess/    # Service access layer for characteristics
│   ├── Enums/                   # Enumerations (Manufacturer, etc.)
│   ├── EventArgs/               # Event argument types
│   └── Exceptions/              # Custom exception hierarchy
└── Bluetooth.Maui/              # Platform-specific MAUI implementations
    ├── Core/                    # Fallback implementations
    └── Platforms/               # Platform-specific code
        ├── Android/             # Android BLE using BluetoothLeScanner
        ├── iOS/                 # iOS BLE using CoreBluetooth (CBCentralManager)
        └── Windows/             # Windows BLE using Windows.Devices.Bluetooth
```

### Core Components

1. **IBluetoothScanner** - Device discovery and scanning
   - Advertisement filtering and processing
   - Device list management with events
   - Scan lifecycle control (start/stop)

2. **IBluetoothDevice** - Device representation and connection
   - Connection management (connect/disconnect/status monitoring)
   - Service exploration and retrieval
   - Advertisement data and signal strength
   - Battery level and device information

3. **IBluetoothService** - GATT service wrapper
   - Characteristic exploration and discovery
   - Characteristic list management
   - Service identification (UUID, name)

4. **IBluetoothCharacteristic** - GATT characteristic operations
   - Read/write operations (async)
   - Notification/indication subscriptions
   - Value caching and conversion
   - Descriptor access

5. **IBluetoothBroadcaster** - Peripheral/advertising mode
   - GATT server implementation (Android)
   - Advertising management
   - iOS/Windows: placeholder implementations

6. **IBluetoothAdvertisement** - Advertisement data
   - Device name, address, manufacturer
   - Service UUIDs
   - Signal strength (RSSI), transmit power
   - Manufacturer-specific data

### Design Patterns

- **Partial Classes**: Complex classes split across multiple files (e.g., `BaseBluetoothDevice.*.cs`)
- **Base/Derived Pattern**: Core base classes with platform-specific overrides
- **Repository Pattern**: `IBluetoothCharacteristicAccessServicesRepository` for service definitions
- **Event-Driven**: PropertyChanged, list change events throughout
- **Async/Await**: All I/O operations are asynchronous with cancellation support
- **Native Object Wrapping**: Platform classes wrap native objects (e.g., `CBPeripheral`, `BluetoothGatt`)

## Code Conventions

### File Organization

1. **Partial Classes**: Related functionality split across files with naming convention:

   ```
   BaseBluetoothDevice.cs              # Main class definition
   BaseBluetoothDevice.Connection.cs   # Connection-related methods
   BaseBluetoothDevice.ServiceList.cs  # Service list management
   ```

2. **Platform-Specific Code**: Organized in `Platforms/{Platform}/` folders
   - Always check for platform-specific implementations before suggesting cross-platform code
   - Platform detection: `#if ANDROID`, `#if IOS`, `#if WINDOWS`

3. **Nested File Dependencies**: Configured in `.csproj` files using `<DependentUpon>`

### Naming Conventions

#### Type Prefixes and Suffixes

- **Interfaces**: Prefix with `I` (e.g., `IBluetoothDevice`, `IBluetoothScanner`)
- **Base Classes**: Prefix with `Base` (e.g., `BaseBluetoothScanner`, `BaseBluetoothRemoteDevice`)
- **Platform Natives**: `Native*` prefix for platform-specific overridable methods (e.g., `NativeConnectAsync`, `NativeStartAsync`)
- **Platform Implementations**: Platform prefix for concrete implementations:
  - Android: `Android*` (e.g., `AndroidBluetoothScanner`, `AndroidBluetoothRemoteDevice`)
  - iOS/macOS: `Apple*` (e.g., `AppleBluetoothScanner`, `AppleBluetoothRemoteDevice`)
  - Windows: `Windows*` (e.g., `WindowsBluetoothScanner`)
- **Event Args**: Suffix with `EventArgs` (e.g., `DevicesAddedEventArgs`, `ServiceDiscoveredEventArgs`)
- **Exceptions**: Suffix with `Exception` (e.g., `DeviceNotFoundException`, `CharacteristicWriteException`)
- **Options**: Suffix with `Options` (e.g., `ConnectionOptions`, `ScanningOptions`, `L2CapChannelOptions`)
- **Factories**: Suffix with `Factory` (e.g., `IBluetoothRemoteDeviceFactory`, `AndroidBluetoothRemoteServiceFactory`)
- **Factory Specs**: Suffix with `FactorySpec` or nested `Spec` class (e.g., `BluetoothRemoteDeviceFactorySpec`, `AndroidBluetoothRemoteDeviceFactorySpec`)

#### Semantic Naming

- **Remote**: Indicates a device/service/characteristic on a remote BLE device (e.g., `IBluetoothRemoteDevice`, `IBluetoothRemoteService`, `IBluetoothRemoteCharacteristic`)
- **Local**: Indicates a local GATT server resource when in peripheral/broadcaster mode (e.g., `IBluetoothLocalService`, `IBluetoothLocalCharacteristic`)
- **Spec**: Short for "specification" - used for factory parameters containing creation data (e.g., `AndroidBluetoothRemoteDeviceFactorySpec`)
- **Proxy**: Wraps native platform objects to provide additional functionality (e.g., `BluetoothGattProxy`)
- **Delegate**: Platform-specific callback interfaces (e.g., `IBluetoothGattDelegate`, `IBluetoothGattCharacteristicDelegate`)
- **Native Objects**: Platform-specific types in `NativeObjects/` folders (e.g., `BluetoothGattProxy`, `ProfileState`)

#### Method Naming

- **On*** prefix: Event handlers and callbacks (e.g., `OnConnectSucceeded`, `OnCharacteristicChanged`)
- **Native*** prefix: Platform-specific implementations meant to be overridden (e.g., `NativeConnectAsync`, `NativeReadValueAsync`)
- ***Internal suffix: Internal implementation methods separated for retry logic (e.g., `ConnectInternalAsync`, `DiscoverServicesInternal`, `ReadCharacteristicInternal`)

### Documentation Standards

**ALL public/protected members MUST have XML documentation comments including:**

1. **`<summary>`**: Clear description of purpose
2. **`<param>`**: All parameters with descriptions
3. **`<returns>`**: Return value description
4. **`<exception>`**: All exceptions that can be thrown
5. **`<remarks>`**: Platform-specific behavior, implementation notes
6. **`<inheritdoc/>`**: For overrides/implementations when base has documentation

**Example:**

```csharp
/// <inheritdoc/>
/// <remarks>
/// On Android, this uses <c>BluetoothLeScanner.StartScan</c> with scan filters.
/// On iOS, this uses <c>CBCentralManager.ScanForPeripherals</c>.
/// On Windows, this uses <c>BluetoothLEAdvertisementWatcher.Start</c>.
/// </remarks>
/// <exception cref="ActivityAlreadyRunningException">Thrown when scanning is already active.</exception>
protected override async ValueTask NativeStartAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
```

**Important:**

- Avoid ambiguous `cref` references - use `<c></c>` for method names with overloads
- Specify platform-specific APIs in `<remarks>` sections
- Document lazy-loaded properties and their initialization behavior

### Logging Standards

**Use Microsoft.Extensions.Logging.Abstractions with structured logging:**

1. **Logger Injection**: Accept `ILogger<T>?` in constructors (nullable to allow operation without logging)

```csharp
public AndroidBluetoothRemoteDevice(
    IBluetoothScanner scanner,
    IBluetoothRemoteDeviceFactory.BluetoothRemoteDeviceFactorySpec spec,
    ILogger<IBluetoothRemoteDevice>? logger = null)
    : base(scanner, spec, logger)
{
    Logger = logger;
}
```

1. **Null-Safe Logging**: Always use null-conditional operator when logging

```csharp
Logger?.LogConnecting(Id);
Logger?.LogConnectionRetry(attempt, retryOptions.MaxRetries, Id, ex);
Logger?.LogConnected(Id);
```

1. **LoggerMessage Pattern**: Use high-performance logging with `LoggerMessage.Define` for frequently called logs

```csharp
private static readonly Action<ILogger, string, Exception?> _logConnecting =
    LoggerMessage.Define<string>(
        LogLevel.Debug,
        new EventId(1, nameof(LogConnecting)),
        "Connecting to device {DeviceId}");

public static void LogConnecting(this ILogger logger, string deviceId)
    => _logConnecting(logger, deviceId, null);
```

1. **Structured Data**: Use named parameters for structured logging (not string interpolation)

```csharp
// GOOD - Structured
Logger?.LogDebug("Connecting to device {DeviceId} with priority {Priority}", deviceId, priority);

// BAD - String interpolation loses structure
Logger?.LogDebug($"Connecting to device {deviceId} with priority {priority}");
```

1. **Log Levels**:
   - `LogTrace`: Internal state changes, loop iterations
   - `LogDebug`: Connection/disconnection, service discovery, characteristic operations
   - `LogInformation`: Scan start/stop, important state transitions
   - `LogWarning`: Retry attempts, recoverable errors, deprecated API usage
   - `LogError`: Operation failures, exceptions that are propagated
   - `LogCritical`: Unrecoverable errors, corrupt state

2. **Exception Logging**: Include exception parameter when logging errors

```csharp
try
{
    await ConnectInternalAsync(connectionOptions, cancellationToken);
}
catch (Exception ex)
{
    Logger?.LogConnectionFailed(Id, attempt, ex);
    throw;
}
```

1. **EventId Range Distribution**: Use consistent EventId ranges across platforms for long-term coherence

Each platform has logging files in `Bluetooth.Maui.Platforms.{Platform}/Logging/{Platform}BluetoothLoggerMessages.cs`:

**EventId Ranges by Functional Area:**

- **1000-1999**: Scanner operations (scan start/stop, device discovery, state changes)
- **2000-2999**: Connection operations (connect, disconnect, status changes, priority)
- **3000-3999**: Service/Characteristic discovery operations
- **4000-4999**: GATT operations (read/write characteristics and descriptors)
- **5000-5999**: Notification/Indication operations (enable/disable, data received)
- **6000-6999**: Broadcaster/MTU operations (platform-specific)
- **7000-7999**: L2CAP channels (Android), Adapter initialization (Windows), MTU (Apple)
- **8000-8999**: L2CAP channels (Apple/iOS)

**When adding new log messages:**

- Find the appropriate functional area range
- Use the next available EventId within that range
- Keep EventIds consistent across platforms for equivalent operations (e.g., "Connecting" is 2000 on all platforms)
- Document new ranges if adding a new functional area
- Leave gaps between ranges for future expansion

### Exception Handling Rules

**Custom Exception Hierarchy:**

```
BluetoothException
├── ActivityException
│   ├── ScannerException
│   │   ├── DeviceNotFoundException
│   │   └── MultipleDevicesFoundException
│   └── ActivityFailedToStartException
├── DeviceException
│   ├── DeviceNotConnectedException
│   └── DeviceConnectionFailedException
├── ServiceException
│   ├── ServiceNotFoundException
│   └── MultipleServicesFoundException
├── CharacteristicException
│   ├── CharacteristicNotFoundException
│   ├── MultipleCharacteristicsFoundException
│   └── CharacteristicAccessException
└── CharacteristicAccessServiceException
    ├── CharacteristicFoundInWrongServiceException
    └── CharacteristicValueConversionException
```

**Exception Handling Rules:**

1. **Throw Specific Exceptions**: Create and throw domain-specific exceptions, not generic `Exception`

```csharp
// GOOD
throw new DeviceNotFoundException(scanner, deviceId);

// BAD
throw new Exception($"Device {deviceId} not found");
```

1. **Static Throw Helpers**: Use static throw helpers for common validation scenarios

```csharp
public static class AndroidNativeGattStatusException
{
    public static void ThrowIfNotSuccess(GattStatus status)
    {
        if (status != GattStatus.Success)
        {
            throw new AndroidNativeGattStatusException(status);
        }
    }
}

// Usage
AndroidNativeGattStatusException.ThrowIfNotSuccess(status);
```

1. **Platform-Specific Exceptions**: Create platform-specific exception types for platform errors

```csharp
// Android
public class AndroidNativeGattCallbackStatusException : BluetoothException { }
public class AndroidNativeCurrentBluetoothStatusCodesException : BluetoothException { }

// iOS
public class AppleNativeBluetoothException : BluetoothException { }
```

1. **Preserve Context**: Always include relevant context in exception constructors

```csharp
public DeviceNotFoundException(IBluetoothScanner scanner, string deviceId)
    : base($"Device with ID '{deviceId}' not found in scanner")
{
    Scanner = scanner;
    DeviceId = deviceId;
}
```

1. **Catch Specific, Throw General**: Catch platform-specific exceptions internally, throw abstracted exceptions to callers

```csharp
try
{
    // Platform-specific operation
    var result = _nativeDevice.ConnectGatt(context, false, callback);
}
catch (Java.Lang.Exception ex)
{
    // Wrap platform exception in abstracted exception
    throw new DeviceConnectionFailedException(this, ex);
}
```

1. **Document All Exceptions**: Use `<exception>` XML tags for all thrown exceptions

```csharp
/// <summary>
/// Connects to the remote device.
/// </summary>
/// <exception cref="DeviceNotConnectedException">Thrown when device is not connected.</exception>
/// <exception cref="InvalidOperationException">Thrown when GATT proxy is null.</exception>
/// <exception cref="TimeoutException">Thrown when operation times out.</exception>
protected override async ValueTask NativeConnectAsync(...)
```

1. **Cleanup on Exception**: Always clean up resources in exception scenarios

```csharp
try
{
    await channel.OpenAsync().ConfigureAwait(false);
    OnL2CapChannelOpened(channel);
}
catch (Exception e)
{
    OnOpenL2CapChannelFailed(e);
    await channel.CloseAsync().ConfigureAwait(false);
    await channel.DisposeAsync().ConfigureAwait(false);
    // Don't rethrow - already handled via callback
}
```

### CancellationToken Standards

**All async operations must support cancellation:**

1. **Parameter Convention**: Always include `CancellationToken cancellationToken = default` as last parameter

```csharp
protected override async ValueTask NativeConnectAsync(
    ConnectionOptions connectionOptions,
    TimeSpan? timeout = null,
    CancellationToken cancellationToken = default)
```

1. **Pass Token Through**: Always pass cancellation token to awaited operations

```csharp
await RetryTools.RunWithRetriesAsync(
    async () => await ConnectInternalAsync(connectionOptions, cancellationToken),
    retryOptions,
    cancellationToken  // Always pass through
).ConfigureAwait(false);
```

1. **Check Before Long Operations**: Check `cancellationToken.ThrowIfCancellationRequested()` before expensive operations

```csharp
cancellationToken.ThrowIfCancellationRequested();
var result = await PerformExpensiveOperationAsync();
```

1. **Combined Cancellation**: Use `CancellationTokenSource.CreateLinkedTokenSource` for timeout + cancellation

```csharp
using var cts = timeout.HasValue
    ? CancellationTokenSource.CreateLinkedTokenSource(cancellationToken)
    : CancellationTokenSource.CreateLinkedTokenSource(CancellationToken.None);

if (timeout.HasValue)
{
    cts.CancelAfter(timeout.Value);
}

await operation.WaitAsync(cts.Token);
```

1. **TaskCompletionSource with Cancellation**: Register cancellation callback to complete TCS

```csharp
cancellationToken.Register(() =>
{
    _connectTcs?.TrySetCanceled(cancellationToken);
});
```

1. **Cleanup on Cancellation**: Always clean up resources when operation is cancelled

```csharp
try
{
    await operation.WaitAsync(cancellationToken);
}
catch (OperationCanceledException)
{
    // Cleanup resources
    await CleanupResourcesAsync();
    throw; // Re-throw to propagate cancellation
}
```

### TaskCompletionSource Patterns

**Use TaskCompletionSource for callback-based async operations:**

1. **Field Declaration**: Declare as nullable volatile field or use locks

```csharp
private volatile TaskCompletionSource<bool>? _connectTcs;
```

1. **Create Before Operation**: Initialize TCS before starting platform operation

```csharp
_connectTcs = new TaskCompletionSource<bool>();
_bluetoothGatt.Connect();
await _connectTcs.Task.ConfigureAwait(false);
```

1. **Complete in Callbacks**: Use `TrySet*` methods in platform callbacks

```csharp
public void OnConnectionStateChange(GattStatus status, ProfileState newState)
{
    if (newState == ProfileState.Connected)
    {
        _connectTcs?.TrySetResult(true);
    }
    else
    {
        _connectTcs?.TrySetException(new DeviceConnectionFailedException(this));
    }
}
```

1. **Timeout Support**: Use `Task.WhenAny` with `Task.Delay` for timeouts

```csharp
var timeoutTask = Task.Delay(timeout.Value, cancellationToken);
var completedTask = await Task.WhenAny(_connectTcs.Task, timeoutTask);

if (completedTask == timeoutTask)
{
    throw new TimeoutException($"Operation timed out after {timeout.Value}");
}

await _connectTcs.Task; // Propagate any exception
```

1. **Cancellation Integration**: Register cancellation callback

```csharp
_connectTcs = new TaskCompletionSource<bool>();

cancellationToken.Register(() =>
{
    _connectTcs?.TrySetCanceled(cancellationToken);
});

// Start operation
_bluetoothGatt.Connect();

// Wait for completion
await _connectTcs.Task.ConfigureAwait(false);
```

1. **Cleanup TCS**: Clear TCS reference after completion to prevent memory leaks

```csharp
try
{
    await _connectTcs.Task.ConfigureAwait(false);
}
finally
{
    _connectTcs = null;
}
```

1. **Thread-Safe Completion**: Use `TrySet*` methods (not `Set*`) to handle races

```csharp
// GOOD - Safe if called multiple times
_connectTcs?.TrySetResult(true);

// BAD - Throws if already completed
_connectTcs?.SetResult(true);
```

### Options Pattern Standards

**Use IOptions<T> pattern with record types for configuration:**

1. **Record Declaration**: Use record types with init-only properties and defaults

```csharp
/// <summary>
/// Configuration options for Bluetooth scanning operations.
/// </summary>
public record ScanningOptions
{
    /// <summary>
    /// Gets or sets the timeout for scan operations.
    /// Default: 30 seconds.
    /// </summary>
    public TimeSpan ScanTimeout { get; init; } = TimeSpan.FromSeconds(30);

    /// <summary>
    /// Gets or sets retry options for starting scan operations.
    /// Default: None (no retries).
    /// </summary>
    public RetryOptions? ScanStartRetry { get; init; }

    /// <summary>
    /// Gets or sets Android-specific scanning options.
    /// </summary>
    public AndroidScanningOptions? Android { get; init; }
}
```

1. **Platform-Specific Options**: Use nested options for platform-specific configuration

```csharp
public record ConnectionOptions
{
    public AndroidConnectionOptions? Android { get; init; }
    public AppleConnectionOptions? Apple { get; init; }
    public WindowsConnectionOptions? Windows { get; init; }
}

public record AndroidConnectionOptions
{
    public BluetoothConnectionPriority? ConnectionPriority { get; init; }
    public RetryOptions? GattWriteRetry { get; init; } = RetryOptions.Default;
    public RetryOptions? GattReadRetry { get; init; }
}
```

1. **Sensible Defaults**: Provide defaults that work for common scenarios

```csharp
public record L2CapChannelOptions
{
    /// <summary>
    /// Gets or sets the default MTU (Maximum Transmission Unit) size.
    /// Default: 672 bytes (minimum required by L2CAP spec).
    /// </summary>
    public int DefaultMtu { get; init; } = 672;

    /// <summary>
    /// Gets or sets whether to enable background reading.
    /// Default: true.
    /// </summary>
    public bool EnableBackgroundReading { get; init; } = true;
}
```

1. **Validation**: Validate options in constructors or factory methods

```csharp
public record RetryOptions
{
    private int _maxRetries = 3;

    public int MaxRetries
    {
        get => _maxRetries;
        init
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException(nameof(value), "MaxRetries must be non-negative");
            _maxRetries = value;
        }
    }
}
```

1. **Static Presets**: Provide static preset configurations for common scenarios

```csharp
public record RetryOptions
{
    public static readonly RetryOptions None = new() { MaxRetries = 0 };
    public static readonly RetryOptions Default = new() { MaxRetries = 3, DelayBetweenRetries = TimeSpan.FromMilliseconds(500) };
    public static readonly RetryOptions Aggressive = new() { MaxRetries = 5, DelayBetweenRetries = TimeSpan.FromMilliseconds(100) };
    public static readonly RetryOptions Conservative = new() { MaxRetries = 2, DelayBetweenRetries = TimeSpan.FromSeconds(1) };
}
```

1. **Dependency Injection**: Accept options through constructor injection

```csharp
public AndroidBluetoothScanner(
    IOptions<ScanningOptions> scanningOptions,
    IBluetoothRemoteDeviceFactory deviceFactory,
    ILogger<IBluetoothScanner>? logger = null)
{
    _scanningOptions = scanningOptions.Value;
}
```

1. **Options Access**: Store options and access properties directly (avoid repeated null checks)

```csharp
// Store in field/property
protected L2CapChannelOptions Options { get; }

// Use directly
Mtu = Options.DefaultMtu;
var bufferSize = Options.ReadBufferSize ?? Mtu;
```

1. **Null-Safe Platform Options**: Use null-conditional and null-coalescing for platform-specific options

```csharp
var retryOptions = _connectionOptions?.Android?.GattWriteRetry ?? RetryOptions.Default;
var priority = connectionOptions.Android?.ConnectionPriority;

if (priority.HasValue)
{
    await RequestConnectionPriorityAsync(priority.Value);
}
```

### Async/Await Patterns

- Use `ValueTask<T>` for hot paths and allocation-sensitive code
- Use `Task<T>` for complex operations with multiple awaits
- Always provide `CancellationToken cancellationToken = default` parameter
- Use `TimeSpan? timeout = null` for operations that may hang
- Configure awaits: `.ConfigureAwait(false)` in library code
- See **CancellationToken Standards** section above for cancellation support
- See **TaskCompletionSource Patterns** section above for callback-based async

### Thread Safety and Locking

**Ensure thread-safe access to shared state:**

1. **Lock on Private Objects**: Always lock on private readonly objects, never `this`

```csharp
private readonly object _devicesLock = new object();

public IReadOnlyList<IBluetoothRemoteDevice> GetDevices()
{
    lock (_devicesLock)
    {
        return _devices.ToList(); // Return copy
    }
}
```

1. **Return Copies, Not References**: Return defensive copies of collections, not direct references

```csharp
// GOOD - Returns immutable copy
public IReadOnlyList<IBluetoothRemoteDevice> GetDevices()
{
    lock (_devicesLock)
    {
        return _devices.ToList();
    }
}

// BAD - Exposes internal collection
public List<IBluetoothRemoteDevice> GetDevices()
{
    return _devices; // Callers can modify!
}
```

1. **Minimize Lock Duration**: Keep lock scopes as small as possible

```csharp
// GOOD - Lock only for collection access
IBluetoothRemoteDevice? device;
lock (_devicesLock)
{
    device = _devices.FirstOrDefault(d => d.Id == deviceId);
}
// Expensive operation outside lock
if (device != null)
{
    await device.ConnectAsync();
}

// BAD - Holds lock during expensive operation
lock (_devicesLock)
{
    var device = _devices.FirstOrDefault(d => d.Id == deviceId);
    if (device != null)
    {
        await device.ConnectAsync(); // Lock held during I/O!
    }
}
```

1. **Volatile for Flags**: Use `volatile` for simple boolean flags accessed from multiple threads

```csharp
private volatile bool _isScanning;
private volatile TaskCompletionSource<bool>? _connectTcs;
```

1. **No Locks in Async Methods**: Avoid holding locks across await points - use `SemaphoreSlim` instead

```csharp
// GOOD - Use SemaphoreSlim for async
private readonly SemaphoreSlim _connectionSemaphore = new(1, 1);

public async Task ConnectAsync()
{
    await _connectionSemaphore.WaitAsync();
    try
    {
        await PerformConnectionAsync();
    }
    finally
    {
        _connectionSemaphore.Release();
    }
}

// BAD - Lock across await
private readonly object _lock = new();

public async Task ConnectAsync()
{
    lock (_lock) // Don't do this!
    {
        await PerformConnectionAsync(); // Holds lock during async!
    }
}
```

## Platform-Specific Guidelines

### Android (Bluetooth.Maui/Platforms/Android/)

**Key APIs:**

- `BluetoothAdapter` - Bluetooth radio control
- `BluetoothLeScanner` - Device scanning
- `BluetoothGatt` - GATT client operations
- `BluetoothGattServer` - GATT server (broadcaster)
- `ScanCallback` - Scan result callbacks

**Common Patterns:**

```csharp
// Scanning
bluetoothAdapter.BluetoothLeScanner.StartScan(scanCallback);

// Connection
device.ConnectGatt(context, autoConnect: false, gattCallback);

// Read characteristic
gatt.ReadCharacteristic(characteristic);
```

**Platform Considerations:**

- Check `BluetoothAdapter.IsEnabled`
- Handle permissions: `BLUETOOTH_SCAN`, `BLUETOOTH_CONNECT`, `ACCESS_FINE_LOCATION`
- API level differences (e.g., `ScanRecord.IsConnectable` requires API 26+)

### iOS (Bluetooth.Maui/Platforms/iOS/)

**Key APIs:**

- `CBCentralManager` - Central role (scanner, client)
- `CBPeripheralManager` - Peripheral role (broadcaster, server)
- `CBPeripheral` - Remote device
- `CBService` - GATT service
- `CBCharacteristic` - GATT characteristic

**Common Patterns:**

```csharp
// Scanning
centralManager.ScanForPeripherals(serviceUuids: null);

// Connection
centralManager.ConnectPeripheral(peripheral, options);

// Discover services
peripheral.DiscoverServices(serviceUuids);

// Read characteristic
peripheral.ReadValue(characteristic);
```

**Platform Considerations:**

- Check `CBCentralManager.State`
- Avoid ambiguous `cref` to overloaded methods (use `<c></c>` instead)
- Handle `CBPeripheralState` for connection status
- iOS simulator has limited BLE functionality

### Windows (Bluetooth.Maui/Platforms/Windows/)

**Key APIs:**

- `BluetoothLEAdvertisementWatcher` - Device scanning
- `BluetoothLEDevice` - Device representation
- `GattSession` - Connection management
- `GattDeviceService` - GATT service
- `GattCharacteristic` - GATT characteristic
- `BluetoothLEAdvertisementPublisher` - Advertising (peripheral)

**Common Patterns:**

```csharp
// Scanning
var watcher = new BluetoothLEAdvertisementWatcher();
watcher.Received += OnAdvertisementReceived;
watcher.Start();

// Connection
var device = await BluetoothLEDevice.FromBluetoothAddressAsync(address);
var session = await GattSession.FromDeviceIdAsync(device.DeviceId);

// Read characteristic
var result = await characteristic.ReadValueAsync();
```

**Platform Considerations:**

- Check `DeviceAccessStatus` before operations
- Use `GattSession.MaintainConnection` for persistent connections
- Handle `BluetoothConnectionStatus` changes
- Windows requires UWP capabilities in manifest

## Common Development Tasks

### Adding New Characteristics

1. Define in `IBluetoothCharacteristicAccessService` interface
2. Implement in `CharacteristicAccessService<T>` or custom service
3. Add to `CharacteristicAccessServicesRepository`
4. Register in scanner initialization
5. Document with Bluetooth SIG specification reference if applicable

### Adding New Platform Support

1. Create platform folder: `Bluetooth.Maui/Platforms/{Platform}/`
2. Implement abstract methods from base classes:
   - `NativeInitializeAsync()` - Initialize platform adapter
   - `NativeStartAsync()` - Start operation (scan, broadcast, etc.)
   - `NativeStopAsync()` - Stop operation
   - `NativeCreateDevice()` - Create platform device wrapper
3. Add platform-specific event handlers and callbacks
4. Update `Bluetooth.Maui.csproj` with target framework
5. Add comprehensive XML documentation with platform remarks

### Implementing Read/Write Characteristics

**Read Pattern:**

```csharp
protected override async ValueTask<ReadOnlyMemory<byte>> NativeReadAsync(
    TimeSpan? timeout = null,
    CancellationToken cancellationToken = default)
{
    // 1. Validate connection state
    // 2. Call platform API
    // 3. Wait for callback/result
    // 4. Return byte array
}
```

**Write Pattern:**

```csharp
protected override async ValueTask NativeWriteAsync(
    ReadOnlyMemory<byte> data,
    bool withoutResponse = false,
    TimeSpan? timeout = null,
    CancellationToken cancellationToken = default)
{
    // 1. Validate connection state
    // 2. Convert data for platform
    // 3. Call platform write API
    // 4. Wait for callback if withoutResponse=false
}
```

### Implementing Notifications

**Subscribe Pattern:**

```csharp
protected override async ValueTask NativeSubscribeAsync(
    TimeSpan? timeout = null,
    CancellationToken cancellationToken = default)
{
    // 1. Enable notifications on characteristic
    // 2. Write to CCCD (Client Characteristic Configuration Descriptor)
    // 3. Register callback handler
}
```

**Callback Handler:**

```csharp
private void OnCharacteristicChanged(/* platform-specific args */)
{
    var newValue = /* extract bytes from platform event */;
    OnNotificationReceived(newValue);
}
```

## Testing Guidelines

When suggesting tests or test modifications:

1. Use **xUnit** framework (`[Fact]`, `[Theory]`)
2. Use **FluentAssertions** for assertions (`.Should().Be()`, `.Should().Throw<>()`)
3. Test structure: Arrange-Act-Assert
4. Name pattern: `MethodName_Scenario_ExpectedBehavior`
5. Mock platform dependencies when possible
6. Test both success and failure paths
7. Test cancellation token handling
8. Test timeout behavior

## Dependencies

### Core Dependencies

- `Plugin.ByteArrays` (1.0.25) - Byte array utilities
- `Plugin.BaseTypeExtensions` (1.0.17) - Extension methods
- `Plugin.ExceptionListeners` (1.0.2 / 1.0.2 Maui) - Exception handling
- `Microsoft.Extensions.Logging.Abstractions` (10.0.0)

### MAUI Dependencies

- `Microsoft.Maui.Core` - MAUI core functionality
- `Microsoft.Maui.Controls` - MAUI controls

### Build Tools

- `Microsoft.SourceLink.GitHub` (8.0.0) - Source link
- `Microsoft.CodeAnalysis.NetAnalyzers` (10.0.0) - Code analysis

## Build and Tasks

**Available Tasks** (via `.vscode/tasks.json` or VS):

- `Clean Documentation` - Remove generated docs
- `Generate API Metadata` - Generate DocFX metadata
- `Build Documentation Site` - Build DocFX site
- `Regenerate Documentation` - Full documentation rebuild

**Build Commands:**

```bash
# Build entire solution
dotnet build

# Build specific project
dotnet build Bluetooth.Core/Bluetooth.Core.csproj
dotnet build Bluetooth.Maui/Bluetooth.Maui.csproj

# Run tests
dotnet test

# Pack for NuGet
dotnet pack
```

## Common Pitfalls to Avoid

1. **Lazy Property Evaluation**: Don't use lazy-loaded properties in `Equals()` or `GetHashCode()` - compare native objects instead
2. **Ambiguous cref**: Avoid `<see cref="Method"/>` for overloaded methods - use `<c>Method</c>` or specify full signature
3. **Platform Assumptions**: Always check which platform code will run on - don't assume Android patterns work on iOS
4. **Resource Cleanup**: Bluetooth resources must be disposed properly - use `IAsyncDisposable`
5. **Thread Safety**: Device/service/characteristic lists may be accessed from multiple threads - use locks
6. **Null Checks**: Native platform objects may be null - always validate before use
7. **Event Unsubscription**: Always unsubscribe from events in cleanup to prevent memory leaks
8. **Cancellation**: Respect `CancellationToken` in all async operations

## Code Review Checklist

When reviewing or generating code, ensure:

### Documentation

- [ ] All public/protected members have complete XML documentation
- [ ] Platform-specific behavior documented in `<remarks>`
- [ ] All exceptions documented with `<exception>` tags
- [ ] No ambiguous cref references in documentation
- [ ] Lazy properties and initialization behavior documented

### Naming Conventions

- [ ] Types follow prefix/suffix conventions (I*, Base*, Android*, Apple*, Windows*,*Exception, *Options,*Factory, *Spec)
- [ ] Methods use appropriate prefixes (On*, Native*, *Internal)
- [ ] Semantic naming used correctly (Remote/Local, Spec, Proxy, Delegate)

### Async/Await and Cancellation

- [ ] `CancellationToken cancellationToken = default` parameter added to async methods
- [ ] `TimeSpan? timeout = null` parameter for potentially blocking operations
- [ ] Cancellation tokens passed through to all awaited operations
- [ ] `.ConfigureAwait(false)` used in library code
- [ ] `cancellationToken.ThrowIfCancellationRequested()` checked before expensive operations
- [ ] Resources cleaned up on cancellation

### TaskCompletionSource Usage

- [ ] TCS fields declared as nullable volatile or protected by locks
- [ ] TCS initialized before starting platform operations
- [ ] TCS completed using `TrySet*` methods (not `Set*`)
- [ ] TCS cleared after completion to prevent memory leaks
- [ ] Cancellation integrated with TCS via `cancellationToken.Register()`
- [ ] Timeouts implemented with `Task.WhenAny` + `Task.Delay`

### Exception Handling

- [ ] Specific custom exceptions thrown, not generic `Exception`
- [ ] All exceptions documented with XML `<exception>` tags
- [ ] Platform exceptions wrapped in abstracted exceptions
- [ ] Exception context preserved (device IDs, state, etc.)
- [ ] Static throw helpers used for common validations
- [ ] Resources disposed in exception scenarios (finally blocks or using)

### Logging

- [ ] Logger injected as `ILogger<T>?` in constructors
- [ ] Null-conditional operator used for all logging calls
- [ ] Structured logging used (named parameters, not string interpolation)
- [ ] Appropriate log levels used (Trace/Debug/Info/Warning/Error/Critical)
- [ ] Exceptions included in error logs
- [ ] LoggerMessage pattern used for high-frequency logs

### Options Pattern

- [ ] Options declared as record types with init-only properties
- [ ] Sensible defaults provided for all options
- [ ] Options validated in constructors/setters
- [ ] Static presets provided for common configurations
- [ ] Platform-specific options nested under Android/Apple/Windows properties
- [ ] Null-safe access to platform options using `?.` and `??`
- [ ] Options stored in fields/properties to avoid repeated null checks

### Thread Safety

- [ ] Thread-safe access to collections using locks on private objects
- [ ] Defensive copies returned, not direct collection references
- [ ] Lock durations minimized (no expensive operations in locks)
- [ ] Volatile used for simple flags accessed from multiple threads
- [ ] `SemaphoreSlim` used instead of locks for async methods
- [ ] No locks held across await points

### Platform-Specific

- [ ] Platform-specific implementations override base `Native*` methods
- [ ] Platform detection used correctly (`#if ANDROID`, `#if IOS`, `#if WINDOWS`)
- [ ] API level checks for Android version-specific features
- [ ] iOS state checks (`CBCentralManager.State`, `CBPeripheralState`)
- [ ] Windows capability checks (`DeviceAccessStatus`, `BluetoothConnectionStatus`)

### Resource Management

- [ ] Resources disposed in `DisposeAsync()` or finally blocks
- [ ] Event subscriptions cleaned up in disposal
- [ ] Native objects nulled after disposal
- [ ] Null checks before accessing native objects
- [ ] `IAsyncDisposable` implemented for types with async cleanup

### General Best Practices

- [ ] No use of lazy properties in `Equals()` or `GetHashCode()`
- [ ] Retry logic extracted to `*Internal` methods
- [ ] Factory specs used for object creation
- [ ] GATT operations respect ConfigurableRetry options
- [ ] API documentation links included for platform APIs

## Additional Resources

### Bluetooth SIG Specifications

- [GATT Specifications](https://www.bluetooth.com/specifications/specs/)
- [Assigned Numbers](https://www.bluetooth.com/specifications/assigned-numbers/)

### Platform Documentation

- [Android Bluetooth LE](https://developer.android.com/develop/connectivity/bluetooth/ble/ble-overview)
- [iOS Core Bluetooth](https://developer.apple.com/documentation/corebluetooth)
- [Windows Bluetooth APIs](https://learn.microsoft.com/en-us/windows/uwp/devices-sensors/bluetooth)

### .NET MAUI

- [.NET MAUI Documentation](https://learn.microsoft.com/en-us/dotnet/maui/)
- [Platform Integration](https://learn.microsoft.com/en-us/dotnet/maui/platform-integration/)

## Questions and Clarifications

When uncertain about implementation details:

1. **Check existing patterns** - Look at similar implementations in the codebase
2. **Verify platform APIs** - Consult official platform documentation
3. **Consider cross-platform** - Ensure approach works on all three platforms
4. **Ask for clarification** - If architectural decision needed, ask the user
5. **Document assumptions** - If making assumptions, document them clearly

---

*Last Updated: February 2025*
*Maintainer: Francois Raminosona / Laerdal Medical*
