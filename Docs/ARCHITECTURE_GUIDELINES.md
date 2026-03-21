# Bluetooth Plugin Architecture Guidelines

This document describes the current architecture and coding rules used by this repository.

## 1. Solution Layers

```mermaid
flowchart LR
        A[Abstractions] --> B[Core]
        B --> C[Platform Implementations]
        C --> D[Bluetooth.Maui]
```

Projects by layer:
- Abstractions:
    - Bluetooth.Abstractions
    - Bluetooth.Abstractions.Scanning
    - Bluetooth.Abstractions.Broadcasting
- Core:
    - Bluetooth.Core
    - Bluetooth.Core.Scanning
    - Bluetooth.Core.Broadcasting
- Platform implementations:
    - Bluetooth.Maui.Platforms.Apple
    - Bluetooth.Maui.Platforms.Droid
    - Bluetooth.Maui.Platforms.Win
    - Bluetooth.Maui.Platforms.DotNetCore
- Facade/composition:
    - Bluetooth.Maui

Rules:
- Lower layers do not reference upper layers.
- Public contracts stay in Abstractions projects.
- Cross-platform orchestration stays in Core projects.
- Native API calls stay in platform projects.

## 2. Entity Pattern

Most entities follow this chain:
- Interface in Abstractions
- Base implementation in Core
- Platform implementation in platform project

Example:
- IBluetoothScanner -> BaseBluetoothScanner -> AndroidBluetoothScanner / AppleBluetoothScanner / WindowsBluetoothScanner

Remote vs local naming:
- Remote* types are central role entities (scanner side).
- Local* types are peripheral role entities (broadcaster side).

## 3. File Structure Pattern

Partial files are split by concern.

Pattern:
- Main type file: TypeName.cs
- Concern files: TypeName.Connection.cs, TypeName.ServiceList.cs, TypeName.StartStop.cs, TypeName.Logging.cs

Expected effects:
- Lifecycle operations are isolated.
- Collections and event handling are isolated.
- Logging declarations are isolated in Logging partials.

## 4. Naming Rules

Type naming:
- Interfaces: I*
- Base classes: Base*
- Platform classes: Apple*, Android*, Windows*, DotNetCore*
- Options records/classes: *Options
- Event args: *EventArgs
- Exceptions: *Exception

Method naming:
- Public async methods: *Async
- Platform override seam: Native*
- Event callback methods: On*
- Idempotent helpers: *IfNeededAsync

Field and property naming:
- Private fields: _camelCase
- Public/protected members: PascalCase
- Boolean state/capability: Is* or Can*

## 5. Async Coordination Rules

Core classes use TaskCompletionSource and state flags for lifecycle operations.

Expected operation shape:
1. Validate current state.
2. Merge concurrent calls when operation is already running.
3. Set transitional state flags.
4. Call Native* method.
5. Complete through On*Succeeded / On*Failed callback path.
6. Clear transitional state in finally block.

All async public methods must expose:
- Optional timeout parameter where operation can block on external/native callbacks.
- CancellationToken cancellationToken = default.

## 6. Logging Rules

Logging uses LoggerMessage-generated extension methods in platform logging files.

EventId ranges:
- 1000-1999: scanner
- 2000-2999: connection
- 3000-3999: service/characteristic discovery
- 4000-4999: GATT read/write
- 5000-5999: notifications/indications
- 6000-6999: broadcaster and MTU (platform-dependent)
- 7000-7999: platform-dependent advanced operations
- 8000-8999: Apple L2CAP range

Rules:
- Use structured logging parameters.
- Keep logger nullable and null-safe.
- Add logs at lifecycle boundaries and failure paths.

## 7. Exception Rules

Rules:
- Throw specific domain exceptions from abstraction/core APIs.
- Wrap native exceptions in platform-specific exception types.
- Keep unsupported feature behavior explicit with NotSupportedException or PlatformNotSupportedException.
- Preserve inner exception context when wrapping.

## 8. DI Registration Rules

DI composition flow in Bluetooth.Maui:
1. Core services.
2. Core scanning and broadcasting services.
3. Platform services.
4. Facade wrappers registered as default IBluetoothScanner and IBluetoothBroadcaster.

Consequence:
- Platform scanner/broadcaster services can exist in DI, but default interface resolution maps to facade wrappers.

## 9. Platform Capability Reality

- Android:
    - Scanning/connection/GATT implemented.
    - L2CAP implemented with API-level guards.
    - Broadcasting implemented.
- Apple (iOS/macOS):
    - Scanning/connection/GATT implemented.
    - L2CAP implemented.
    - Broadcasting implemented.
    - Some operations are intentionally restricted by CoreBluetooth behavior.
- Windows:
    - Scanning/connection/GATT implemented.
    - L2CAP and broadcasting not implemented.
    - Several advanced operations throw NotSupportedException.
- DotNetCore fallback:
    - Runtime BLE operations throw PlatformNotSupportedException.

## 10. Known Architectural Divergence

Broadcasting factories are not fully standardized across platforms:
- Android uses broadcasting factories.
- Apple broadcaster currently creates local services directly in broadcaster flow.
- Windows broadcaster-side factory registrations exist, but runtime behavior is not implemented.

This divergence is documented for follow-up alignment, not as a public API contract.
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
