# Plugin.Bluetooth - Implementation Guide

**Last Updated**: 2026-02-18
**Status**: Multi-platform BLE library with platform-specific implementations

---

## Table of Contents

1. [Project Overview](#project-overview)
2. [Architecture](#architecture)
3. [Platform Status](#platform-status)
4. [Project Structure](#project-structure)
5. [Key Patterns & Conventions](#key-patterns--conventions)
6. [Implementation Guide](#implementation-guide)
7. [Testing & Verification](#testing--verification)
8. [Future Work](#future-work)

---

## Project Overview

**Plugin.Bluetooth** is a cross-platform .NET MAUI Bluetooth Low Energy (BLE) library that provides:

- **Scanning** (Central/Client role): Discover and connect to BLE peripherals
- **Broadcasting** (Peripheral/Server role): Advertise services and handle client connections

### Supported Platforms

| Platform | Status | Scanning | Broadcasting |
|----------|--------|----------|--------------|
| **iOS/macOS** (Apple) | ✅ Complete | Full implementation | Full implementation |
| **Android** | ✅ Complete | Full implementation | Stub (deferred) |
| **Windows** | ✅ Complete | Stub | Stub |
| **DotNetCore** | ✅ Complete | Stub | Stub |

---

## Architecture

### Layer Structure

```
┌─────────────────────────────────────────────────────┐
│  Bluetooth.Abstractions / Bluetooth.Abstractions.*  │  ← Interfaces & contracts
├─────────────────────────────────────────────────────┤
│  Bluetooth.Core / Bluetooth.Core.*                  │  ← Base implementations
├─────────────────────────────────────────────────────┤
│  Bluetooth.Maui.Platforms.*                         │  ← Platform-specific code
└─────────────────────────────────────────────────────┘
```

### Key Abstractions

**Scanning (Central/Client Role)**:

- `IBluetoothScanner` - Discovers BLE devices
- `IBluetoothRemoteDevice` - Represents discovered peripheral
- `IBluetoothRemoteService` - GATT service on remote device
- `IBluetoothRemoteCharacteristic` - GATT characteristic for read/write/notify

**Broadcasting (Peripheral/Server Role)**:

- `IBluetoothBroadcaster` - Advertises services
- `IBluetoothLocalService` - Local GATT service
- `IBluetoothLocalCharacteristic` - Local characteristic that handles client requests
- `IBluetoothConnectedDevice` - Represents connected client

**Common**:

- `IBluetoothAdapter` - Platform adapter (state, permissions)
- `IBluetoothPermissionManager` - Permission handling
- `ITicker` - Timing/scheduling infrastructure

---

## Platform Status

### ✅ Apple (iOS/macOS) - **FULLY IMPLEMENTED**

**Location**: `Bluetooth.Maui.Platforms.Apple/`

#### Scanning

- ✅ Device discovery via CoreBluetooth `CBCentralManager`
- ✅ GATT operations (read, write, notify/indicate)
- ✅ Connection management with options (`CBConnectPeripheralOptions`)
- ✅ MTU negotiation
- ✅ Service and characteristic exploration
- ✅ RSSI monitoring

**Key Files**:

- `Scanning/BluetoothScanner.cs` - Scanner implementation with `CBCentralManager`
- `Scanning/BluetoothDevice.cs` - Device with `CBPeripheral` wrapper
- `Scanning/BluetoothCharacteristic.cs` - Characteristic operations
- `Scanning/Options/ConnectionOptions.cs` - iOS-specific connection options

#### Broadcasting

- ✅ GATT server via CoreBluetooth `CBPeripheralManager`
- ✅ Service/characteristic creation
- ✅ Client connection handling
- ✅ Characteristic read/write request handling
- ✅ Notify/indicate operations
- ✅ MTU tracking

**Key Files**:

- `Broadcasting/BluetoothBroadcaster.cs` - Broadcaster with `CBPeripheralManager`
- `Broadcasting/BluetoothBroadcastService.cs` - Local service implementation
- `Broadcasting/BluetoothBroadcastCharacteristic.cs` - Local characteristic
- `Broadcasting/BluetoothBroadcastClientDevice.cs` - Connected client representation

---

### ✅ Android - **SCANNER COMPLETE, BROADCASTER DEFERRED**

**Location**: `Bluetooth.Maui.Platforms.Droid/`

#### Scanning - ✅ **FULLY IMPLEMENTED**

- ✅ Device discovery via `BluetoothLeScanner`
- ✅ GATT operations with `BluetoothGatt`
- ✅ Connection management with PHY/transport options
- ✅ Characteristic read/write/notify
- ✅ Service exploration

**Key Files**:

- `Scanning/BluetoothScanner.cs` - Scanner with `ScanCallbackProxy`
- `Scanning/Options/ConnectionOptions.cs` - Android-specific connection options (PHY)
- `Scanning/Factories/BluetoothDeviceFactoryRequest.cs` - Device factory request

**Implementation Notes**:

- Uses `ScanCallbackProxy.IScanCallbackProxyDelegate` for scan callbacks
- `BluetoothDeviceExtensions.ConnectGatt()` handles API level differences (21/23/26+)
- Advertisement processing via `OnAdvertisementReceived()`

#### Broadcasting - ⏸️ **STUB (Deferred)**

- ⏸️ Infrastructure in place but methods throw `NotImplementedException`
- ⏸️ Requires `BluetoothGattServer` implementation
- ⏸️ Needs physical device testing (emulators don't support GATT server well)

**Why Deferred**:

1. Broadcasting/GATT server is less commonly used (most apps are Central/Client)
2. Requires extensive physical device testing across Android versions
3. Scanner (more critical feature) is fully functional

**Stub Files** (ready for implementation):

- `Broadcasting/BluetoothBroadcaster.cs` - Constructor updated, native methods stubbed
- `Broadcasting/BluetoothBroadcastService.cs` - Inherits `BaseBluetoothLocalService`
- `Broadcasting/BluetoothBroadcastCharacteristic.cs` - Inherits `BaseBluetoothLocalCharacteristic`
- `Broadcasting/BluetoothBroadcastClientDevice.cs` - Inherits `BaseBluetoothConnectedDevice`

**To Complete Android Broadcasting**:

1. Implement `NativeStartAsync()` in `BluetoothBroadcaster.cs`:
   - Initialize `BluetoothGattServer` via `BluetoothGattServerCallbackProxy`
   - Add services using `BluetoothGattServer.AddService()`
   - Start advertising with `BluetoothLeAdvertiser`
2. Implement characteristic read/write handlers in `BluetoothBroadcastCharacteristic.cs`
3. Implement client connection handling in `BluetoothBroadcastClientDevice.cs`
4. Test on physical devices (various Android versions)

---

### ✅ Windows - **STUBS (Ready for Implementation)**

**Location**: `Bluetooth.Maui.Platforms.Windows/`

#### Status

- ⏸️ Both Scanner and Broadcaster are stubs
- ⏸️ All methods throw `NotImplementedException`
- ✅ Infrastructure and DI setup complete

**Key Files**:

- `Scanning/BluetoothScanner.cs` - Stub with correct constructor
- `Broadcasting/BluetoothBroadcaster.cs` - Stub with correct constructor

**To Implement**:

1. Use Windows Runtime APIs: `BluetoothLEAdvertisementWatcher`, `BluetoothLEDevice`, etc.
2. Follow Apple/Android patterns for `NativeStartAsync`, `NativeStopAsync`, etc.
3. Implement `CreateDeviceFactoryRequestFromAdvertisement()` in Scanner

---

### ✅ DotNetCore - **STUBS (Testing/Mock Platform)**

**Location**: `Bluetooth.Maui.Platforms.DotNetCore/`

**Purpose**: Stub platform for testing and development without physical Bluetooth hardware

---

## Project Structure

### Core Projects

```
Bluetooth.Abstractions/                    # Core interfaces
├── IBluetoothAdapter.cs
├── Options/BluetoothInfrastructureOptions.cs
└── Enums/                                 # Shared enums

Bluetooth.Abstractions.Scanning/           # Scanning-specific abstractions
├── IBluetoothScanner.cs
├── IBluetoothRemoteDevice.cs
├── IBluetoothRemoteService.cs
├── IBluetoothRemoteCharacteristic.cs
├── Options/ScanningOptions.cs
├── EventArgs/                             # Event args for scanning events
├── Exceptions/                            # Scanning-specific exceptions
└── Factories/IBluetoothDeviceFactory.cs

Bluetooth.Abstractions.Broadcasting/       # Broadcasting-specific abstractions
├── IBluetoothBroadcaster.cs
├── IBluetoothLocalService.cs
├── IBluetoothLocalCharacteristic.cs
├── IBluetoothConnectedDevice.cs
├── Options/BroadcastingOptions.cs
├── EventArgs/                             # Event args for broadcasting events
├── Exceptions/                            # Broadcasting-specific exceptions
└── Factories/
    ├── IBluetoothLocalServiceFactory.cs
    ├── IBluetoothLocalCharacteristicFactory.cs
    └── IBluetoothConnectedDeviceFactory.cs

Bluetooth.Core/                            # Base implementations
├── BaseBluetoothAdapter.cs
├── Infrastructure/Scheduling/
│   ├── ITicker.cs                         # Timing abstraction
│   └── Ticker.cs
└── ServiceCollectionExtensions.cs         # DI setup for core services

Bluetooth.Core.Scanning/                   # Base scanning implementations
├── BaseBluetoothScanner.cs
├── BaseBluetoothRemoteDevice.cs
├── BaseBluetoothRemoteService.cs
└── BaseBluetoothRemoteCharacteristic.cs

Bluetooth.Core.Broadcasting/               # Base broadcasting implementations
├── BaseBluetoothBroadcaster.cs
├── BaseBluetoothLocalService.cs
├── BaseBluetoothLocalCharacteristic.cs
└── BaseBluetoothConnectedDevice.cs
```

### Platform Projects

```
Bluetooth.Maui.Platforms.Apple/
├── GlobalUsings.cs                        # Platform-specific imports
├── BluetoothAdapter.cs                    # iOS/macOS adapter
├── Scanning/
│   ├── BluetoothScanner.cs
│   ├── BluetoothDevice.cs                 # Wraps CBPeripheral
│   ├── Options/ConnectionOptions.cs       # iOS-specific options
│   └── Factories/
├── Broadcasting/
│   ├── BluetoothBroadcaster.cs            # Wraps CBPeripheralManager
│   ├── BluetoothBroadcastService.cs
│   └── BluetoothBroadcastCharacteristic.cs
├── Permissions/BluetoothPermissionManager.cs
└── ServiceCollectionExtensions.cs         # DI registration

Bluetooth.Maui.Platforms.Droid/
├── GlobalUsings.cs
├── BluetoothAdapter.cs                    # Android adapter
├── Scanning/
│   ├── BluetoothScanner.cs                # Uses BluetoothLeScanner
│   ├── Options/ConnectionOptions.cs       # Android-specific (PHY, transport)
│   ├── Factories/BluetoothDeviceFactoryRequest.cs
│   └── NativeObjects/                     # Proxy wrappers for Android APIs
│       ├── ScanCallbackProxy.cs
│       └── BluetoothDeviceExtensions.cs
├── Broadcasting/
│   ├── BluetoothBroadcaster.cs            # STUB - BluetoothGattServer
│   ├── BluetoothBroadcastService.cs       # STUB
│   ├── BluetoothBroadcastCharacteristic.cs # STUB
│   └── NativeObjects/
│       ├── BluetoothGattServerCallbackProxy.cs
│       └── AdvertiseCallbackProxy.cs
├── Permissions/BluetoothPermissionManager.cs
└── ServiceCollectionExtensions.cs

Bluetooth.Maui.Platforms.Windows/
├── GlobalUsings.cs
├── BluetoothAdapter.cs
├── Scanning/BluetoothScanner.cs           # STUB
├── Broadcasting/BluetoothBroadcaster.cs   # STUB
└── ServiceCollectionExtensions.cs

Bluetooth.Maui.Platforms.DotNetCore/
├── BluetoothAdapter.cs                    # STUB for testing
├── Scanning/BluetoothScanner.cs           # STUB
└── Broadcasting/BluetoothBroadcaster.cs   # STUB
```

### Archives

```
Archives/
├── Android/
│   ├── Archive/                           # Old implementation (deprecated)
│   ├── ArchiveScanning/                   # Old scanner (replaced)
│   └── ArchiveBroadcasting/               # Old broadcaster (partially restored)
├── Apple/
│   ├── Archive/                           # Old implementation (deprecated)
│   ├── ArchiveScanning/                   # Old scanner (replaced)
│   └── ArchiveBroadcasting/               # Old broadcaster (replaced)
└── Windows/
    ├── Archive/                           # Old implementation (deprecated)
    ├── ArchiveScanning/                   # Old scanner (basis for stub)
    └── ArchiveBroadcasting/               # Old broadcaster (basis for stub)
```

**Note**: Archives contain previous implementations that were refactored. Some were restored and updated with new constructors/interfaces (Android/Windows stubs). Do NOT use archived code directly - it uses old API patterns.

---

## Key Patterns & Conventions

### 1. Factory Pattern

**All device, service, and characteristic creation goes through factories**:

```csharp
// Scanning
public interface IBluetoothDeviceFactory
{
    IBluetoothRemoteDevice CreateDevice(
        IBluetoothScanner scanner,
        BluetoothDeviceFactoryRequest request);

    public record BluetoothDeviceFactoryRequest(
        string DeviceId,
        Manufacturer Manufacturer,
        IBluetoothAdvertisement? Advertisement);
}

// Broadcasting
public interface IBluetoothLocalServiceFactory
{
    IBluetoothLocalService CreateService(
        IBluetoothBroadcaster broadcaster,
        BluetoothLocalServiceSpec spec);

    public record BluetoothLocalServiceSpec(
        Guid Id,
        BluetoothServiceType ServiceType,
        ...);
}
```

**Platform Implementation**:

- Extend base record with platform-specific properties
- Example: Android adds `Android.Bluetooth.BluetoothDevice? NativeDevice`

### 2. Constructor Pattern (CRITICAL)

**All base classes now require these parameters** (as of 2026-02-18):

```csharp
// Scanner
public BaseBluetoothScanner(
    IBluetoothAdapter adapter,
    IBluetoothPermissionManager permissionManager,
    IBluetoothDeviceFactory deviceFactory,
    IBluetoothRssiToSignalStrengthConverter rssiToSignalStrengthConverter,
    ITicker ticker,
    ILogger<IBluetoothScanner>? logger = null)

// Broadcaster
public BaseBluetoothBroadcaster(
    IBluetoothAdapter adapter,
    IBluetoothLocalServiceFactory localServiceFactory,
    IBluetoothConnectedDeviceFactory connectedDeviceFactory,
    IBluetoothPermissionManager permissionManager,
    ITicker ticker,
    ILogger<IBluetoothBroadcaster>? logger = null)
```

**Important Changes**:

- ❌ Old: `IBluetoothCharacteristicAccessServicesRepository` (REMOVED)
- ✅ New: `IBluetoothRssiToSignalStrengthConverter` + `ITicker` (REQUIRED)
- ❌ Old: `IBluetoothBroadcastServiceFactory` (RENAMED)
- ✅ New: `IBluetoothLocalServiceFactory` + `IBluetoothConnectedDeviceFactory` (REQUIRED)
- ❌ Old: `ILogger?` (CHANGED)
- ✅ New: `ILogger<T>?` (Generic logger interface)

### 3. Options Pattern

**Scanning Options**:

```csharp
// Cross-platform base
Bluetooth.Abstractions.Scanning.Options.ScanningOptions

// Platform extensions
// Android adds: PreferredPhy
Bluetooth.Maui.Platforms.Droid.Scanning.Options.ConnectionOptions
    : Abstractions.Scanning.Options.ConnectionOptions
{
    public Android.Bluetooth.BluetoothPhy? PreferredPhy { get; set; }
}

// Apple uses base directly (no extensions needed)
```

**Broadcasting Options**:

```csharp
// Cross-platform base
Bluetooth.Abstractions.Broadcasting.Options.BroadcastingOptions

// Platform extensions as needed
```

### 4. Proxy/Delegate Pattern (Android-specific)

Android uses proxy wrappers for callback-based APIs:

```csharp
// Scanning
public partial class ScanCallbackProxy : ScanCallback
{
    public interface IScanCallbackProxyDelegate
    {
        void OnScanResult(ScanCallbackType callbackType, ScanResult result);
        void OnScanFailed(ScanFailure errorCode);
    }
}

// Scanner implements the delegate
public class BluetoothScanner : BaseBluetoothScanner,
    ScanCallbackProxy.IScanCallbackProxyDelegate
{
    public void OnScanResult(ScanCallbackType callbackType, ScanResult result)
    {
        var advertisement = new BluetoothAdvertisement(result);
        OnAdvertisementReceived(advertisement);  // Base class method
    }
}
```

**Why**: Android's callback-based APIs don't fit the async/await pattern well. Proxies bridge the gap.

### 5. Native Abstract Methods

Platform implementations must override these:

```csharp
// Scanner
protected abstract void NativeRefreshIsRunning();
protected abstract ValueTask NativeStartAsync(
    ScanningOptions scanningOptions,
    TimeSpan? timeout = null,
    CancellationToken cancellationToken = default);
protected abstract ValueTask NativeStopAsync(
    TimeSpan? timeout = null,
    CancellationToken cancellationToken = default);
protected abstract IBluetoothDeviceFactory.BluetoothDeviceFactoryRequest
    CreateDeviceFactoryRequestFromAdvertisement(IBluetoothAdvertisement advertisement);

// Broadcaster
protected abstract void NativeRefreshIsRunning();
protected abstract ValueTask NativeStartAsync(
    BroadcastingOptions options,
    TimeSpan? timeout = null,
    CancellationToken cancellationToken = default);
protected abstract ValueTask NativeStopAsync(
    TimeSpan? timeout = null,
    CancellationToken cancellationToken = default);
```

### 6. Dependency Injection Setup

**Every platform must register** (in `ServiceCollectionExtensions.cs`):

```csharp
public static void AddBluetooth[Platform]Services(this IServiceCollection services)
{
    // Core infrastructure (REQUIRED)
    services.AddSingleton<ITicker, Ticker>();
    services.AddBluetoothCoreServices();
    services.AddBluetoothCoreScanningServices();
    services.AddBluetoothCoreBroadcastingServices();

    // Platform-specific (REQUIRED)
    services.AddSingleton<IBluetoothAdapter, BluetoothAdapter>();
    services.AddSingleton<IBluetoothPermissionManager, BluetoothPermissionManager>();
    services.AddSingleton<IBluetoothScanner, Scanning.BluetoothScanner>();
    services.AddSingleton<IBluetoothBroadcaster, Broadcasting.BluetoothBroadcaster>();

    // Platform factories (if needed)
    // Android/Apple have these in separate ServiceCollectionExtensions in Scanning/Broadcasting folders
}
```

### 7. GlobalUsings Pattern

Each platform has a `GlobalUsings.cs` with common imports:

```csharp
global using System;
global using System.Collections.ObjectModel;
// ... standard .NET

#if ANDROID
global using Android.Bluetooth;
global using Android.Bluetooth.LE;
// ... Android-specific
#endif

global using Bluetooth.Core;
global using Bluetooth.Core.Scanning;
global using Bluetooth.Core.Broadcasting;
global using Bluetooth.Abstractions;
global using Bluetooth.Abstractions.Broadcasting;
global using Bluetooth.Abstractions.Scanning;
global using Bluetooth.Abstractions.Scanning.Converters;  // ← REQUIRED (added recently)
global using Bluetooth.Abstractions.Enums;
```

---

## Implementation Guide

### Adding a New Platform

1. **Create Platform Project**:

   ```
   Bluetooth.Maui.Platforms.[PlatformName]/
   ```

2. **Add GlobalUsings.cs** with required namespaces

3. **Implement BluetoothAdapter**:

   ```csharp
   public class BluetoothAdapter : BaseBluetoothAdapter
   {
       // Platform-specific adapter state
   }
   ```

4. **Implement BluetoothScanner**:

   ```csharp
   public class BluetoothScanner : BaseBluetoothScanner
   {
       public BluetoothScanner(/* correct constructor params */) : base(...)
       {
       }

       protected override void NativeRefreshIsRunning() { /* ... */ }
       protected override ValueTask NativeStartAsync(...) { /* ... */ }
       protected override ValueTask NativeStopAsync(...) { /* ... */ }
       protected override IBluetoothDeviceFactory.BluetoothDeviceFactoryRequest
           CreateDeviceFactoryRequestFromAdvertisement(...) { /* ... */ }
   }
   ```

5. **Implement BluetoothBroadcaster**:

   ```csharp
   public class BluetoothBroadcaster : BaseBluetoothBroadcaster
   {
       public BluetoothBroadcaster(/* correct constructor params */) : base(...)
       {
       }

       protected override void NativeRefreshIsRunning() { /* ... */ }
       protected override ValueTask NativeStartAsync(...) { /* ... */ }
       protected override ValueTask NativeStopAsync(...) { /* ... */ }
   }
   ```

6. **Setup DI in ServiceCollectionExtensions.cs**

7. **Test with stub implementations first** (throw `NotImplementedException`)

### Implementing Android Broadcasting (Next Steps)

**Goal**: Complete the GATT server functionality on Android

**Prerequisites**:

- Physical Android device (API 21+)
- Android device with BLE peripheral mode support
- Familiarity with Android's `BluetoothGattServer` API

**Step-by-Step**:

1. **Implement `BluetoothBroadcaster.NativeStartAsync()`**:

   ```csharp
   protected override async ValueTask NativeStartAsync(
       BroadcastingOptions options,
       TimeSpan? timeout = null,
       CancellationToken cancellationToken = default)
   {
       // 1. Initialize GATT server via BluetoothGattServerCallbackProxy
       _gattServerProxy = new BluetoothGattServerCallbackProxy(this, BluetoothManager);

       // 2. Add all local services
       foreach (var service in GetServices())
       {
           if (service is BluetoothBroadcastService droidService)
           {
               _gattServerProxy.BluetoothGattServer.AddService(droidService.NativeService);
               await WaitForServiceAdded(cancellationToken); // Wait for callback
           }
       }

       // 3. Start advertising
       _advertiser = BluetoothManager.Adapter.BluetoothLeAdvertiser;
       var settings = BuildAdvertiseSettings(options);
       var data = BuildAdvertiseData(options);
       _advertiser.StartAdvertising(settings, data, _advertiseProxy);

       // 4. Wait for advertising to start
       await WaitForAdvertisingStarted(timeout, cancellationToken);
   }
   ```

2. **Implement `BluetoothBroadcastCharacteristic` read/write handlers**:

   ```csharp
   public void OnCharacteristicReadRequest(
       IBluetoothDeviceDelegate device,
       int requestId,
       int offset)
   {
       // Get value from base class
       var value = OnReadRequestReceived(device as BaseBluetoothConnectedDevice);

       // Send response via GATT server
       _gattServer.SendResponse(device.NativeDevice, requestId,
           GattStatus.Success, offset, value.ToArray());
   }
   ```

3. **Implement `BluetoothBroadcastService` initialization**:

   ```csharp
   public BluetoothBroadcastService(...)
   {
       // Create native BluetoothGattService
       var serviceType = request.ServiceType == BluetoothServiceType.Primary
           ? GattServiceType.Primary
           : GattServiceType.Secondary;

       NativeService = new BluetoothGattService(
           UUID.FromString(request.Id.ToString()),
           serviceType);

       // Add characteristics will be done via factory
   }
   ```

4. **Test on physical devices**:
   - Test with different Android versions (21, 23, 26, 29, 31+)
   - Test with different client devices (iOS, Android, Windows)
   - Test MTU negotiation
   - Test multiple simultaneous client connections

**Reference Implementation**: See Apple's `BluetoothBroadcaster.cs` for patterns

### Implementing Windows Support

**Windows Runtime BLE APIs**:

- `BluetoothLEAdvertisementWatcher` - For scanning
- `BluetoothLEDevice` - For device connections
- `GattDeviceService` - For GATT services
- `GattCharacteristic` - For characteristics
- `BluetoothLEAdvertisementPublisher` - For broadcasting

**Starting Point**: Current stubs in `Bluetooth.Maui.Platforms.Windows/`

**Challenges**:

- Windows BLE APIs are async-first (easier than Android)
- Permission model different from mobile
- Limited documentation compared to mobile platforms

---

## Testing & Verification

### Build Verification

```bash
# Build all platforms
dotnet build

# Build specific platform
dotnet build Bluetooth.Maui.Platforms.Apple/
dotnet build Bluetooth.Maui.Platforms.Droid/
dotnet build Bluetooth.Maui.Platforms.Windows/
dotnet build Bluetooth.Maui.Platforms.DotNetCore/
```

**Success Criteria**:

- ✅ 0 compilation errors
- ✅ Warnings are acceptable (code analysis suggestions like CA1062, CA1725, etc.)

### Runtime Testing

**Scanner Testing** (Apple, Android):

1. Deploy to physical device
2. Call `StartScanningAsync()` with options
3. Verify `DeviceDiscovered` events fire
4. Connect to a known BLE device
5. Discover services and characteristics
6. Read/write/subscribe to characteristics
7. Disconnect gracefully

**Broadcaster Testing** (Apple only currently):

1. Deploy to physical device
2. Create local services with `CreateServiceAsync()`
3. Add characteristics to services
4. Call `StartBroadcastingAsync()`
5. Connect from another device (iOS/Android client app)
6. Verify client can discover services
7. Verify read/write requests work
8. Test notify/indicate operations
9. Test multiple concurrent clients

### Common Issues

**Android**:

- 🔴 **Permissions**: Ensure `ACCESS_FINE_LOCATION`, `BLUETOOTH_SCAN`, `BLUETOOTH_CONNECT` granted
- 🔴 **Location Services**: Must be enabled for BLE scanning on Android 6+
- 🔴 **Emulator Limitations**: Android emulators don't support GATT server (Broadcasting)

**iOS/macOS**:

- 🔴 **Permissions**: `NSBluetoothAlwaysUsageDescription` in Info.plist
- 🔴 **Background Modes**: May need `bluetooth-central` or `bluetooth-peripheral` background modes
- 🔴 **Simulator Limitations**: iOS simulator doesn't support BLE at all

**Windows**:

- 🔴 **Capabilities**: Ensure `bluetooth` capability in Package.appxmanifest
- 🔴 **Permissions**: User must grant Bluetooth access

---

## Future Work

### High Priority

1. **✅ Complete Android Broadcasting** ← Next immediate task
   - Implement GATT server operations
   - Test on physical devices
   - Document Android-specific quirks

2. **Windows Platform Implementation**
   - Basic scanner for device discovery
   - GATT client operations
   - Consider broadcasting (less critical)

### Medium Priority

1. **Additional Features**:
   - L2CAP channels (direct data streams)
   - Extended advertising (Android 8+, iOS 13+)
   - Advertisement filtering improvements
   - Bond/pairing management
   - Background operation optimization

2. **Testing & Quality**:
   - Unit tests for base classes
   - Integration tests with mock Bluetooth
   - Performance benchmarks
   - Memory leak detection

### Low Priority

1. **Documentation**:
   - API documentation (XML comments)
   - Sample applications for each platform
   - Migration guide from old APIs
   - Troubleshooting guide

2. **Optimization**:
   - Reduce allocations in hot paths
   - Optimize advertisement parsing
   - Connection pooling for multiple devices

---

## Migration Notes (for developers updating old code)

### From Old API (Pre-2026-02-18)

**Constructor Changes**:

```csharp
// ❌ OLD
public BluetoothScanner(
    IBluetoothAdapter adapter,
    IBluetoothPermissionManager permissionManager,
    IBluetoothDeviceFactory deviceFactory,
    IBluetoothCharacteristicAccessServicesRepository repository,  // REMOVED
    ILogger? logger = null)

// ✅ NEW
public BluetoothScanner(
    IBluetoothAdapter adapter,
    IBluetoothPermissionManager permissionManager,
    IBluetoothDeviceFactory deviceFactory,
    IBluetoothRssiToSignalStrengthConverter rssiToSignalStrengthConverter,  // NEW
    ITicker ticker,  // NEW
    ILogger<IBluetoothScanner>? logger = null)  // Changed to generic
```

**Factory Record Names**:

```csharp
// ❌ OLD
IBluetoothLocalCharacteristicFactory.BluetoothCharacteristicSpec
IBluetoothLocalDescriptorFactory.BluetoothDescriptorSpec
IBluetoothLocalServiceFactory.BluetoothServiceFactoryRequest

// ✅ NEW
IBluetoothLocalCharacteristicFactory.BluetoothLocalCharacteristicSpec
IBluetoothLocalDescriptorFactory.BluetoothLocalDescriptorSpec
IBluetoothLocalServiceFactory.BluetoothLocalServiceSpec
```

**Base Class Names**:

```csharp
// ❌ OLD (Broadcasting)
BaseBluetoothBroadcastCharacteristic
BaseBluetoothBroadcastClientDevice
BaseBluetoothBroadcastService

// ✅ NEW
BaseBluetoothLocalCharacteristic
BaseBluetoothConnectedDevice
BaseBluetoothLocalService
```

---

## Quick Reference

### File Locations by Feature

| Feature | File Path |
|---------|-----------|
| Scanner Base | `Bluetooth.Core.Scanning/BaseBluetoothScanner.cs` |
| Broadcaster Base | `Bluetooth.Core.Broadcasting/BaseBluetoothBroadcaster.cs` |
| Apple Scanner | `Bluetooth.Maui.Platforms.Apple/Scanning/BluetoothScanner.cs` |
| Apple Broadcaster | `Bluetooth.Maui.Platforms.Apple/Broadcasting/BluetoothBroadcaster.cs` |
| Android Scanner | `Bluetooth.Maui.Platforms.Droid/Scanning/BluetoothScanner.cs` |
| Android Broadcaster | `Bluetooth.Maui.Platforms.Droid/Broadcasting/BluetoothBroadcaster.cs` (STUB) |
| Windows Scanner | `Bluetooth.Maui.Platforms.Windows/Scanning/BluetoothScanner.cs` (STUB) |
| Windows Broadcaster | `Bluetooth.Maui.Platforms.Windows/Broadcasting/BluetoothBroadcaster.cs` (STUB) |

### Common Commands

```bash
# Build all
dotnet build

# Build specific platform
dotnet build Bluetooth.Maui.Platforms.Apple/
dotnet build Bluetooth.Maui.Platforms.Droid/

# Clean build
dotnet clean && dotnet build --no-incremental

# Check for errors only
dotnet build 2>&1 | grep error
```

---

## Contributing

When implementing new features:

1. ✅ Start with interface changes in `Bluetooth.Abstractions.*`
2. ✅ Update base implementations in `Bluetooth.Core.*`
3. ✅ Update each platform implementation
4. ✅ Test on physical devices (not simulators/emulators)
5. ✅ Update this documentation

**Questions?** Check Archives for historical context, but don't copy-paste old code - APIs have changed significantly.

---

**Document Version**: 1.0
**Last Updated By**: Claude (AI Assistant)
**Next Review**: When Android Broadcasting is completed
