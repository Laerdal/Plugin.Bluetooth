# GitHub Copilot Instructions for Plugin.Bluetooth

## Project Overview

**Plugin.Bluetooth** is a cross-platform .NET MAUI Bluetooth Low Energy (BLE) library providing a unified API for Android, iOS, and Windows platforms. The library abstracts platform-specific BLE implementations into a consistent, easy-to-use interface for scanning, connecting, and communicating with Bluetooth devices.

### Key Information

- **License**: MIT (Copyright 2025 Laerdal Medical)
- **Target Framework**: .NET 10.0
- **Supported Platforms**: Android 36.1, iOS, macOS Catalyst, Windows 10.0.19041.0
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

- **Interfaces**: Prefix with `I` (e.g., `IBluetoothDevice`)
- **Base Classes**: Prefix with `Base` (e.g., `BaseBluetoothScanner`)
- **Platform Natives**: `Native*` prefix for platform-specific overridable methods (e.g., `NativeConnectAsync`)
- **Event Args**: Suffix with `EventArgs` (e.g., `DevicesAddedEventArgs`)
- **Exceptions**: Suffix with `Exception` (e.g., `DeviceNotFoundException`)

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

### Async/Await Patterns

- Use `ValueTask<T>` for hot paths and allocation-sensitive code
- Use `Task<T>` for complex operations with multiple awaits
- Always provide `CancellationToken cancellationToken = default` parameter
- Use `TimeSpan? timeout = null` for operations that may hang
- Configure awaits: `.ConfigureAwait(false)` in library code

### Exception Handling

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

**When suggesting exception handling:**

- Catch specific exceptions, not generic `Exception`
- Document all thrown exceptions in XML comments
- Use platform-specific exception types where appropriate
- Always clean up resources in finally blocks or using statements

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

- [ ] All public/protected members have complete XML documentation
- [ ] Platform-specific behavior documented in `<remarks>`
- [ ] All exceptions documented with `<exception>` tags
- [ ] `CancellationToken` parameters added to async methods
- [ ] `TimeSpan? timeout` parameters for potentially blocking operations
- [ ] `.ConfigureAwait(false)` used in library code
- [ ] Platform-specific implementations override base `Native*` methods
- [ ] Resources disposed in `DisposeAsync()` or finally blocks
- [ ] Thread-safe access to collections using locks
- [ ] Null checks before accessing native objects
- [ ] Events unsubscribed in cleanup
- [ ] No use of lazy properties in comparison operations
- [ ] No ambiguous cref references in documentation

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

*Last Updated: December 2025*
*Maintainer: Francois Raminosona / Laerdal Medical*
