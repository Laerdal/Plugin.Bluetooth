# Bluetooth Plugin - Architecture & Coding Guidelines

> **Purpose**: This document defines the comprehensive naming conventions, architectural patterns, and coding style used throughout the Bluetooth Plugin solution. All new code must follow these guidelines to maintain consistency.

---

## 📁 PROJECT STRUCTURE

### Project Hierarchy (4 Tiers)

```
Tier 1: Abstractions (Interfaces)
├── Bluetooth.Abstractions (base types, enums, IBluetoothAdapter)
├── Bluetooth.Abstractions.Scanning (IBluetoothScanner, IBluetoothRemote*)
└── Bluetooth.Abstractions.Broadcasting (IBluetoothBroadcaster, IBluetoothLocal*)

Tier 2: Core (Base Implementations)
├── Bluetooth.Core (BaseBindableObject, infrastructure)
├── Bluetooth.Core.Scanning (BaseBluetoothScanner, BaseBluetoothRemote*)
└── Bluetooth.Core.Broadcasting (BaseBluetoothBroadcaster, BaseBluetoothLocal*)

Tier 3: Platform Implementations
├── Bluetooth.Maui.Platforms.Apple (iOS/macOS - CoreBluetooth)
├── Bluetooth.Maui.Platforms.Droid (Android)
├── Bluetooth.Maui.Platforms.Win (Windows)
└── Bluetooth.Maui.Platforms.DotNetCore (Desktop .NET stub)

Tier 4: Composition Root
└── Bluetooth.Maui (Multi-targeted DI wiring)
```

### Dependency Flow (Strict One-Directional)

```
Abstractions  ←  Core  ←  Platforms  ←  Maui  ←  Sample
```

**Rules:**
- Lower tiers NEVER reference higher tiers
- All project references use `PrivateAssets="all"`
- Namespaces mirror folder paths exactly

### Standard Folder Structure

Every platform project must use:

```
Scanning/
  Factories/          -- Platform factory + request types
  NativeObjects/      -- Wrappers around native SDK objects
  Options/            -- Platform-specific option types
Broadcasting/
  Factories/
  NativeObjects/
Logging/              -- LoggerMessage source-generated classes
Exceptions/           -- Platform-specific exception types
Permissions/          -- Platform permission handlers
Tools/                -- Extension methods, converters
Threading/            -- Main thread dispatchers (Apple only)
```

---

## 🏷️ NAMING CONVENTIONS

### Class Naming

| Layer | Pattern | Example |
|-------|---------|---------|
| Interface | `IBluetooth{Entity}` | `IBluetoothScanner` |
| Core Base | `BaseBluetooth{Entity}` | `BaseBluetoothScanner` |
| Apple | `AppleBluetooth{Entity}` | `AppleBluetoothScanner` |
| Android | `AndroidBluetooth{Entity}` | `AndroidBluetoothScanner` |
| Windows | `WindowsBluetooth{Entity}` | `WindowsBluetoothScanner` |
| DotNetCore | `DotNetCoreBluetooth{Entity}` | `DotNetCoreBluetoothScanner` |
| Factory Interface | `IBluetooth{Entity}Factory` | `IBluetoothDeviceFactory` |
| Base Factory | `BaseBluetooth{Entity}Factory` | `BaseBluetoothDeviceFactory` |
| Platform Factory | `{Platform}Bluetooth{Entity}Factory` | `AppleBluetoothDeviceFactory` |
| Factory Request | `{Platform}Bluetooth{Entity}FactoryRequest` | `AppleBluetoothDeviceFactoryRequest` |
| Exception | `{Domain}{Action}Exception` | `ScannerFailedToStartException` |
| EventArgs | `{Domain}{Event}EventArgs` | `DeviceListChangedEventArgs` |
| Options | `{Feature}Options` | `ScanningOptions` |
| Native Wrapper (Apple) | `Cb{Type}Wrapper` | `CbPeripheralWrapper` |
| Native Wrapper (Android) | `Bluetooth{Type}Proxy` | `BluetoothGattProxy` |

**Important Qualifiers:**
- **Scanner-side entities**: Use `Remote` prefix → `IBluetoothRemoteDevice`, `BaseBluetoothRemoteService`
- **Broadcaster-side entities**: Use `Local` prefix → `IBluetoothLocalService`, `BaseBluetoothLocalCharacteristic`

### File Naming

Partial classes split by concern: `{ClassName}.{Aspect}.cs`

**Examples:**
```
IBluetoothScanner.cs
IBluetoothScanner.StartStop.cs
IBluetoothScanner.DeviceList.cs

BaseBluetoothScanner.cs
BaseBluetoothScanner.StartStop.cs
BaseBluetoothScanner.DeviceList.cs
BaseBluetoothScanner.Logging.cs
BaseBluetoothScanner.Advertisement.cs
```

Logging is ALWAYS in a separate `.Logging.cs` partial file.

### Method Naming

| Pattern | Usage | Example |
|---------|-------|---------|
| `{Action}Async` | Public async operations | `StartScanningAsync` |
| `{Action}IfNeededAsync` | Idempotent operations | `ConnectIfNeededAsync` |
| `Native{Action}Async` | Platform hook (abstract) | `NativeStartAsync` |
| `NativeRefresh{Prop}` | Platform state sync | `NativeRefreshIsRunning` |
| `NativeCan{Action}` | Platform capability check | `NativeCanRead` |
| `On{Event}Succeeded` | Native success callback | `OnStartSucceeded` |
| `On{Event}Failed` | Native failure callback | `OnStartFailed` |
| `On{Event}` | Event raiser | `OnDisconnect` |
| `WaitFor{State}Async` | Wait for condition | `WaitForIsConnectedAsync` |
| `Get{Entity}` | Throws if not found | `GetDevice(filter)` |
| `Get{Entity}OrDefault` | Returns null if not found | `GetDeviceOrDefault(filter)` |
| `Has{Entity}` | Boolean check | `HasDevice(id)` |
| `Clear{Entity}Async` | Cleanup operation | `ClearDevicesAsync` |
| `Log{Action}` | Logging method | `LogScannerStarting` |
| `AddBluetooth{Layer}{Platform}Services` | DI registration | `AddBluetoothMauiAppleScanningServices` |

### Property & Field Naming

| Type | Convention | Example |
|------|-----------|---------|
| Boolean state | `Is{State}` | `IsRunning`, `IsConnected`, `IsOpen` |
| Boolean capability | `Can{Action}` | `CanRead`, `CanWrite`, `CanListen` |
| Active config | `Current{Config}` | `CurrentScanningOptions` |
| Private field | `_camelCase` | `_logger`, `_operationSemaphore` |
| Private TCS | `{Name}Tcs` | `StartTcs`, `ConnectionTcs` |
| Protected property | `PascalCase` | `DeviceFactory`, `ServiceFactory` |
| Public property | `PascalCase` | `Scanner`, `Name`, `Adapter` |

### Event Naming

- State change: bare name → `Starting`, `Started`, `Stopping`, `Stopped`
- State changed: `{State}Changed` → `RunningStateChanged`, `ConnectionStateChanged`
- List changed: `{Entity}ListChanged` → `DeviceListChanged`, `ServiceListChanged`
- List mutations: `{Entities}Added` / `{Entities}Removed` → `DevicesAdded`, `ServicesRemoved`
- Data events: `{Entity}{Action}` → `AdvertisementReceived`, `ValueUpdated`, `DataReceived`

---

## 🏛️ ARCHITECTURAL PATTERNS

### 1. Three-Tier Entity Pattern

**Every Bluetooth entity MUST follow this hierarchy:**

```
Interface (Abstractions) → Base Class (Core) → Platform Implementation
```

**Example:**
```
IBluetoothScanner → BaseBluetoothScanner → AppleBluetoothScanner
                                        → AndroidBluetoothScanner
                                        → WindowsBluetoothScanner
```

**Each tier has specific responsibilities:**

1. **Interface** (Abstractions):
   - Defines public contract via partial interfaces
   - Split across multiple files by concern
   - Contains XML documentation for end users

2. **Base Class** (Core):
   - Implements cross-platform orchestration logic
   - Contains TCS-based async coordination
   - Declares `abstract` methods prefixed `Native*` for platform hooks
   - Contains logging via partial `.Logging.cs` file

3. **Platform Class** (Platforms):
   - Implements `Native*` abstract methods
   - Implements native delegate interfaces
   - Wraps platform-specific SDK objects

### 2. Factory Pattern with Request Records

**Structure:**

```csharp
// 1. Interface with nested request record (Abstractions)
public interface IBluetoothDeviceFactory
{
    IBluetoothRemoteDevice CreateDevice(
        IBluetoothScanner scanner,
        BluetoothDeviceFactoryRequest request);

    record BluetoothDeviceFactoryRequest
    {
        protected BluetoothDeviceFactoryRequest(string id, string? name) { ... }
        public string Id { get; }
        public string? Name { get; }
    }
}

// 2. Base factory (Core)
public abstract class BaseBluetoothDeviceFactory : IBluetoothDeviceFactory
{
    protected IBluetoothServiceFactory ServiceFactory { get; }
    protected IBluetoothRssiToSignalStrengthConverter RssiConverter { get; }

    public abstract IBluetoothRemoteDevice CreateDevice(...);
}

// 3. Platform request (extends nested record) (Platform)
public sealed record AppleBluetoothDeviceFactoryRequest(
    string Id,
    string? Name,
    CBPeripheral NativePeripheral)  // Platform-specific!
    : IBluetoothDeviceFactory.BluetoothDeviceFactoryRequest(Id, Name);

// 4. Platform factory (Platform)
public class AppleBluetoothDeviceFactory : BaseBluetoothDeviceFactory
{
    private readonly IBluetoothRemoteL2CapChannelFactory _l2CapFactory;

    public override IBluetoothRemoteDevice CreateDevice(
        IBluetoothScanner scanner,
        IBluetoothDeviceFactory.BluetoothDeviceFactoryRequest request)
    {
        if (request is not AppleBluetoothDeviceFactoryRequest appleRequest)
            throw new ArgumentException("Must be Apple request");

        return new AppleBluetoothRemoteDevice(
            scanner,
            appleRequest,
            ServiceFactory,
            _l2CapFactory,
            RssiConverter);
    }
}
```

**Key Rules:**
- Nested request records MUST have `protected` constructors
- Platform requests MUST extend the nested base record
- Platform requests contain native SDK objects
- Factories are registered as `Singleton` in DI

### 3. TaskCompletionSource (TCS) Async Coordination Pattern

**This is THE central async pattern. Every async operation MUST follow these steps:**

```csharp
public async Task StartScanningAsync(
    ScanningOptions options,
    TimeSpan? timeout = null,
    CancellationToken cancellationToken = default)
{
    // 1. Check preconditions
    ScannerIsAlreadyStartedException.ThrowIfAlreadyStarting(this);
    ScannerIsAlreadyStartedException.ThrowIfAlreadyStarted(this);

    // 2. Merge concurrent attempts
    if (StartTcs is { Task.IsCompleted: false })
    {
        LogMergingScanAttempts();
        return await StartTcs.Task.ConfigureAwait(false);
    }

    // 3. Create new TCS
    StartTcs = new TaskCompletionSource();

    try
    {
        // 4. Set transitional state
        IsStarting = true;

        // 5. Raise event
        Starting?.Invoke(this, EventArgs.Empty);

        // 6. Call platform hook (with exception handling)
        try
        {
            await NativeStartAsync(options, timeout, cancellationToken)
                .ConfigureAwait(false);
        }
        catch (Exception e)
        {
            OnStartFailed(new ScannerFailedToStartException("...", e));
            throw;
        }

        // 7. Wait for native callback
        await StartTcs.Task.WaitBetterAsync(timeout, cancellationToken)
            .ConfigureAwait(false);

        // 8. Verify final state
        if (!IsRunning)
            throw new ScannerFailedToStartException("...");
    }
    finally
    {
        // 9. Reset transitional state
        IsStarting = false;
        Started?.Invoke(this, EventArgs.Empty);
        StartTcs = null;
    }
}

// Called by platform implementation from native callback
protected void OnStartSucceeded()
{
    if (StartTcs is { Task.IsCompleted: false })
    {
        IsRunning = true;
        StartTcs.TrySetResult();
    }
    else
    {
        // Unexpected callback - log warning
        LogUnexpectedScanStarted();
    }
}

protected void OnStartFailed(Exception exception)
{
    if (StartTcs is { Task.IsCompleted: false })
        StartTcs.TrySetException(exception);
    else
        BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, exception);
}
```

**Critical Rules:**
- ALWAYS check for pending TCS and merge concurrent attempts
- ALWAYS use try/finally for state cleanup
- ALWAYS call `OnXxxSucceeded()` from platform callbacks
- ALWAYS handle unexpected callbacks (no pending TCS) gracefully
- ALWAYS use `.ConfigureAwait(false)`

### 4. BaseBindableObject Pattern

**All Core base classes MUST inherit from `BaseBindableObject`.**

This provides:
- `ConcurrentDictionary<string, object?>` property store
- `GetValue<T>(defaultValue)` / `SetValue<T>(value)` with `INotifyPropertyChanged`
- `WaitForPropertyToBeOfValue<T>()` async property waiting
- Automatic `[CallerMemberName]` property naming

**Example:**
```csharp
public bool IsRunning
{
    get => GetValue(false);
    protected set => SetValue(value);
}
```

**When to use backing fields instead:**
- When the property is used in hot paths (frequent access)
- When you need thread-local or mutable reference types
- When you need private readonly fields (e.g., `_logger`)

### 5. Native Delegate/Wrapper Pattern

**Platform-specific native objects MUST be wrapped.**

**Structure:**
```csharp
// Wrapper defines delegate interface
public class CbPeripheralWrapper
{
    public interface ICbPeripheralDelegate
    {
        void DidDiscoverServices(NSError? error);
        void DidDiscoverCharacteristics(NSError? error, CBService service);
        // ...
    }

    private ICbPeripheralDelegate _delegate;

    // Wrapper implements native delegate protocol
    private class NativeDelegate : CBPeripheralDelegate
    {
        public override void DiscoveredService(CBPeripheral peripheral, NSError error)
        {
            _wrapper._delegate.DidDiscoverServices(error);
        }
    }
}

// Platform class implements wrapper delegate interface
public class AppleBluetoothRemoteDevice : BaseBluetoothRemoteDevice,
    CbPeripheralWrapper.ICbPeripheralDelegate,
    CbCentralManagerWrapper.ICbPeripheralDelegate
{
    public CbPeripheralWrapper CbPeripheralWrapper { get; }

    // Implement delegate methods
    public void DidDiscoverServices(NSError? error) { ... }
}
```

---

## 💻 CODING STYLE

### Async/Await Rules

**MUST:**
- Use `.ConfigureAwait(false)` on EVERY `await`
- Prefer `ValueTask` for frequently-synchronous operations
- Use `Task` for always-async operations
- Return `ValueTask.CompletedTask` from synchronous `Native*Async` methods
- Use `.WaitBetterAsync(timeout, cancellationToken)` for TCS awaiting
- Use `.StartAndForget(onException)` for fire-and-forget

**NEVER:**
- Forget `.ConfigureAwait(false)`
- Use `Task.Run()` in public APIs (only in platform implementations if necessary)
- Use `.Result` or `.Wait()` (deadlock risk)

### Error Handling Rules

**Exception Hierarchy:**
```
BluetoothException (abstract base)
├── BluetoothScanningException
│   ├── ScannerException
│   ├── DeviceException
│   └── CharacteristicException
├── BluetoothBroadcastingException
├── BluetoothPermissionException
└── AdapterException
```

**Platform Native Exceptions:**
- `AppleNativeBluetoothException`
- `AndroidNativeBluetoothException`
- `WindowsNativeBluetoothException`

**Static ThrowIf Helper Pattern:**
```csharp
// In exception class
public static void ThrowIfNotConnected(IBluetoothRemoteDevice device)
{
    if (!device.IsConnected)
        throw new DeviceNotConnectedException(device);
}

// Usage
DeviceNotConnectedException.ThrowIfNotConnected(RemoteService.Device);
CharacteristicCantReadException.ThrowIfCantRead(this);
AppleNativeBluetoothException.ThrowIfError(error);
```

**Unhandled Exception Dispatch:**
```csharp
BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, ex);
```

### Logging Rules

**TWO logging approaches (BOTH are used):**

#### 1. LoggerMessage on Partial Classes (Core Layer)

**File:** `BaseBluetoothScanner.Logging.cs`

```csharp
public abstract partial class BaseBluetoothScanner
{
    #region LoggerMessage Definitions (EventId 100-199)

    [LoggerMessage(EventId = 100, Level = LogLevel.Information,
        Message = "Scanner starting with {ServiceUuidCount} service UUIDs")]
    partial void LogScannerStarting(int serviceUuidCount);

    [LoggerMessage(EventId = 101, Level = LogLevel.Information,
        Message = "Scanner started successfully")]
    partial void LogScannerStarted();

    #endregion
}
```

**Requirements:**
- MUST be `partial void` instance methods
- MUST have private readonly `ILogger _logger` field in main class
- MUST initialize with `NullLogger<T>.Instance` fallback
- EventId ranges by class (100s = scanner, 200s = device, etc.)
- Call directly: `LogScannerStarting(count);`

#### 2. LoggerMessage as Extension Methods (Platform Layer)

**File:** `AppleBluetoothLoggerMessages.cs`

```csharp
internal static partial class AppleBluetoothLoggerMessages
{
    #region Scanner Logging (EventId 1000-1099)

    [LoggerMessage(EventId = 1000, Level = LogLevel.Information,
        Message = "Starting BLE scan with mode: {ScanMode}")]
    public static partial void LogScanStarting(
        this ILogger logger,
        BluetoothScanMode scanMode);

    #endregion
}
```

**Requirements:**
- MUST be `public static partial` extension methods
- MUST be in `internal static partial class`
- EventId ranges: 1000s = scanner, 2000s = connection, 3000s = services, etc.
- Call with null-conditional: `Logger?.LogScanStarting(mode);`

### Documentation Comment Rules

**MUST have on all public APIs:**
- `<summary>` on classes, interfaces, methods, properties
- `<param>` on method parameters
- `<returns>` on methods
- `<exception cref="...">` for thrown exceptions
- `<remarks>` for platform-specific behavior

**Platform behavior documentation pattern:**
```csharp
/// <remarks>
/// <b>Platform Support:</b>
/// <list type="bullet">
/// <item><description><b>iOS/macOS:</b> Fully supported via CoreBluetooth</description></item>
/// <item><description><b>Android:</b> Requires API 21+ (Android 5.0+)</description></item>
/// <item><description><b>Windows:</b> Requires Windows 10 version 1803+</description></item>
/// </list>
/// </remarks>
```

**Use `/// <inheritdoc />` extensively in implementations.**

### Region Usage Rules

**MUST use regions to organize code:**

**Base classes:**
```csharp
#region Constructor
#region Properties
#region Start
#region Stop
#region Devices - Events
#region Devices - Get
#region Devices - Has
#region Devices - Clear
#region Services - Exploration
#region Connection
#region Dispose
#region LoggerMessage Definitions (EventId 100-199)
```

**Platform logging classes:**
```csharp
#region Scanner Logging (EventId 1000-1099)
#region Connection Logging (EventId 2000-2099)
#region GATT Operation Logging (EventId 4000-4099)
```

### Event Handling Rules

**Raise events with null-conditional:**
```csharp
Starting?.Invoke(this, EventArgs.Empty);
ConnectionStateChanged?.Invoke(this, new DeviceConnectionStateChangedEventArgs(this, value));
```

**Clean up in disposal:**
```csharp
protected virtual async ValueTask DisposeAsyncCore()
{
    // ... cleanup logic ...

    // Null out events
    Starting = null;
    Started = null;
    AdvertisementReceived = null;
}
```

### Disposal Pattern Rules

**MUST implement consistent `IAsyncDisposable`:**

```csharp
public async ValueTask DisposeAsync()
{
    await DisposeAsyncCore().ConfigureAwait(false);
    GC.SuppressFinalize(this);
}

protected virtual async ValueTask DisposeAsyncCore()
{
    // 1. Graceful shutdown (stop scanning, disconnect)
    if (IsRunning)
        await StopScanningAsync().ConfigureAwait(false);

    // 2. Cancel pending TCS operations
    StartTcs?.TrySetCanceled();
    StopTcs?.TrySetCanceled();

    // 3. Unsubscribe from events
    Devices.CollectionChanged -= DevicesOnCollectionChanged;

    // 4. Dispose resources
    _operationSemaphore?.Dispose();

    // 5. Clear child collections
    await ClearDevicesAsync().ConfigureAwait(false);

    // 6. Null out event handlers
    Starting = null;
    Started = null;
}
```

### Options/Configuration Pattern

**MUST use `record` for immutable options:**

```csharp
public record ScanningOptions
{
    public TimeSpan? ScanTimeout { get; init; }
    public IReadOnlyList<Guid>? ServiceUuids { get; init; }
    public object? Android { get; init; }  // Platform-specific escape hatch
    public object? Apple { get; init; }

    public static ScanningOptions Default => new();
}
```

**Use `init`-only properties with defaults.**

### Semaphore Usage Rules

**MUST serialize concurrent operations with `SemaphoreSlim`:**

```csharp
private readonly SemaphoreSlim _operationSemaphore = new(1, 1);

public async Task PerformOperationAsync()
{
    await _operationSemaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
    try
    {
        // Serialized operation
    }
    finally
    {
        _operationSemaphore.Release();
    }
}
```

### Guard Clause Rules

**MUST use modern .NET style:**

```csharp
ArgumentNullException.ThrowIfNull(device);
ArgumentNullException.ThrowIfNull(request);
```

**NOT the old style:**
```csharp
// ❌ Don't use this:
if (device == null)
    throw new ArgumentNullException(nameof(device));
```

---

## 🔧 DEPENDENCY INJECTION PATTERN

**Structure:**
```
AddBluetoothServices()
  ├─ AddBluetoothCoreServices()
  │   ├─ AddBluetoothCoreScanningServices()
  │   └─ AddBluetoothCoreBroadcastingServices()
  └─ [Platform-specific via #if]
      ├─ AddBluetoothMauiAppleServices()
      │   ├─ AddBluetoothMauiAppleScanningServices()
      │   └─ AddBluetoothMauiAppleBroadcastingServices()
      └─ etc.
```

**Extension method naming:**
```csharp
public static IServiceCollection AddBluetooth{Layer}{Platform}{Feature}Services(
    this IServiceCollection services)
```

**All registrations MUST be `Singleton`.**

---

## 📝 CSPROJ ORGANIZATION

**Every `.csproj` MUST follow this exact section layout:**

```xml
<Project Sdk="Microsoft.NET.Sdk">

    <!-- ==================== CORE VARIABLES ==================== -->
    <PropertyGroup>
        <TargetFrameworks>...</TargetFrameworks>
        <AssemblyName>...</AssemblyName>
        ...
    </PropertyGroup>

    <!-- ==================== NOWARNINGS ==================== -->
    <PropertyGroup>
        <NoWarn>...</NoWarn>
    </PropertyGroup>

    <!-- ==================== TARGET FRAMEWORKS ==================== -->
    <ItemGroup Condition="'$(TargetFramework)' == 'net10.0-ios'">
        ...
    </ItemGroup>

    <!-- ==================== PACKAGE REFERENCES / DEPENDENCIES ==================== -->
    <ItemGroup>
        <PackageReference Include="..." />
    </ItemGroup>

    <!-- ==================== PROJECT REFERENCES / DEPENDENCIES ==================== -->
    <ItemGroup>
        <ProjectReference Include="..." PrivateAssets="all" />
    </ItemGroup>

    <!-- ==================== FILE DEPENDENCIES ==================== -->
    <ItemGroup>
        <Compile Update="BaseBluetoothScanner.StartStop.cs">
            <DependentUpon>BaseBluetoothScanner.cs</DependentUpon>
        </Compile>
        <Compile Update="BaseBluetoothScanner.Logging.cs">
            <DependentUpon>BaseBluetoothScanner.cs</DependentUpon>
        </Compile>
    </ItemGroup>

</Project>
```

---

## 🎯 CHECKLIST FOR NEW FEATURES

**Before implementing any new feature, ensure:**

- [ ] Interface defined in `Bluetooth.Abstractions.*`
- [ ] Interface split into partial files by concern
- [ ] Base class in `Bluetooth.Core.*` with `abstract Native*` methods
- [ ] Base class uses TCS pattern for async coordination
- [ ] Base class has `.Logging.cs` partial file with LoggerMessage
- [ ] Base class has private readonly `ILogger _logger` field
- [ ] Platform implementations for Apple, Android, Windows
- [ ] Platform implementations have platform-specific logging
- [ ] Factory pattern with nested request record
- [ ] Platform-specific requests extend nested base
- [ ] Factories registered as Singleton in DI
- [ ] Exceptions follow hierarchy with static ThrowIf helpers
- [ ] All async methods use `.ConfigureAwait(false)`
- [ ] All properties use `BaseBindableObject` or have backing fields
- [ ] Disposal implemented with `DisposeAsyncCore()`
- [ ] Events nulled out in disposal
- [ ] Semaphores used for concurrent operation serialization
- [ ] XML docs on all public APIs
- [ ] Platform support documented in `<remarks>`
- [ ] Regions used to organize code
- [ ] Unit tests added (if applicable)

---

## 🚫 ANTI-PATTERNS TO AVOID

**NEVER:**
- ❌ Create interfaces without base implementations
- ❌ Skip `.ConfigureAwait(false)` on await
- ❌ Use `Task.Result` or `.Wait()`
- ❌ Forget to merge concurrent TCS operations
- ❌ Create public async methods without timeout/cancellation
- ❌ Use old-style null checks (use `ArgumentNullException.ThrowIfNull`)
- ❌ Put logging in main class file (use `.Logging.cs`)
- ❌ Register services as Transient or Scoped (always Singleton)
- ❌ Reference higher-tier projects from lower tiers
- ❌ Omit PrivateAssets="all" on project references
- ❌ Create backing fields when BaseBindableObject can be used
- ❌ Forget to dispose semaphores/native resources
- ❌ Forget to null out events in disposal
- ❌ Use emojis (unless user explicitly requests)

---

**Document Version:** 1.0
**Last Updated:** 2026-02-24
**Authors:** Analysis of entire Bluetooth Plugin codebase
