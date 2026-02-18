# Bluetooth.Core & Bluetooth.Maui

<div style="max-width: 256px; margin-left: auto; margin-right: auto;">

[![Icon](icon.png)](https://github.com/laerdal/Plugin.Bluetooth)

</div>

[![CI](https://img.shields.io/github/actions/workflow/status/laerdal/Plugin.Bluetooth/ci.yml?logo=github)](https://github.com/laerdal/Plugin.Bluetooth/actions/workflows/ci.yml)
[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![NuGet](https://img.shields.io/nuget/v/Bluetooth.Maui?logo=nuget&color=004880)](https://www.nuget.org/packages/Bluetooth.Maui)
[![NuGet Downloads](https://img.shields.io/nuget/dt/Bluetooth.Maui?logo=nuget&color=004880)](https://www.nuget.org/packages/Bluetooth.Maui)
[![GitHub Release](https://img.shields.io/github/v/release/laerdal/Plugin.Bluetooth?logo=github)](https://github.com/laerdal/Plugin.Bluetooth/releases)
[![License](https://img.shields.io/github/license/laerdal/Plugin.Bluetooth?color=blue)](LICENSE.md)

A cross-platform .NET MAUI Bluetooth Low Energy (BLE) library providing a clean, unified API for **Android**, **iOS/MacCatalyst**, and **Windows** platforms.

## ✨ Recent Updates

🎉 **Windows Platform Complete** - Full BLE scanning & GATT operations now available on Windows
🔧 **API Simplified** - Exploration methods refactored for clarity and ease of use
📦 **Modern DI** - Streamlined dependency injection with `AddBluetoothServices()`

## Features

- 🔍 **BLE Scanning** - Discover nearby devices with customizable filtering
- 🔗 **Connection Management** - Robust connect/disconnect with auto-reconnect support
- 📡 **GATT Operations** - Full support for services, characteristics, and descriptors
- 📊 **Read/Write/Notify** - Read values, write data, and subscribe to notifications
- 🎯 **Cross-Platform** - Consistent API across all major platforms
- 🔄 **Modern Async** - Async/await throughout with cancellation token support
- 💉 **DI-First** - Built for MAUI dependency injection
- 🧩 **Options Pattern** - Flexible configuration via options objects
- 📝 **Well-Documented** - Comprehensive XML docs and examples

## Platform Support

| Platform | Scanning | Connection | GATT Operations | Broadcasting |
|----------|----------|------------|-----------------|--------------|
| **Android** | ✅ | ✅ | ✅ | ✅ |
| **iOS** | ✅ | ✅ | ✅ | ❌ |
| **MacCatalyst** | ✅ | ✅ | ✅ | ❌ |
| **Windows** | ✅ | ✅ | ✅ | ❌ |

> **Note**: Broadcasting (peripheral mode) is currently Android-only. iOS/MacCatalyst/Windows throw `NotImplementedException`.

## Installation

```bash
dotnet add package Bluetooth.Maui
```

Or via NuGet Package Manager:

```xml
<PackageReference Include="Bluetooth.Maui" Version="1.0.0" />
```

## Quick Start

### 1. Register Services in MauiProgram.cs

```csharp
public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit();

        // Register Bluetooth services
        builder.Services.AddBluetoothServices();

        return builder.Build();
    }
}
```

### 2. Inject and Use IBluetoothScanner

```csharp
public class ScannerViewModel
{
    private readonly IBluetoothScanner _scanner;

    public ScannerViewModel(IBluetoothScanner scanner)
    {
        _scanner = scanner;

        // Subscribe to device discovery
        _scanner.DeviceListChanged += OnDeviceListChanged;
    }

    public async Task StartScanningAsync()
    {
        var options = new ScanningOptions
        {
            // Optional: Configure scanning behavior
        };

        await _scanner.StartScanningAsync(options);
    }

    private void OnDeviceListChanged(object? sender, DeviceListChangedEventArgs e)
    {
        foreach (var device in _scanner.Devices)
        {
            Console.WriteLine($"Found: {device.Name} ({device.Id})");
            Console.WriteLine($"  RSSI: {device.SignalStrengthDbm} dBm");
        }
    }
}
```

## Usage Guide

### Scanning for Devices

```csharp
// Start scanning
await _scanner.StartScanningAsync();

// Access discovered devices
var devices = _scanner.Devices;

// Stop scanning when done
await _scanner.StopScanningAsync();
```

### Connecting to a Device

```csharp
// Get a device from the scanner
var device = _scanner.Devices.FirstOrDefault(d => d.Name == "MyDevice");

if (device != null)
{
    // Connect with options
    var connectionOptions = new ConnectionOptions
    {
        // Platform-specific connection parameters
    };

    await device.ConnectAsync(connectionOptions);

    // Check connection status
    Console.WriteLine($"Connected: {device.IsConnected}");
}
```

### Service Discovery - Simplified API

The new exploration APIs use a single, flexible method with optional configuration:

```csharp
// Simple exploration (defaults: services only, caching enabled)
await device.ExploreServicesAsync();

// Explore services AND characteristics
await device.ExploreServicesAsync(ServiceExplorationOptions.WithCharacteristics);

// Full exploration (services + characteristics + descriptors)
await device.ExploreServicesAsync(ServiceExplorationOptions.Full);

// Force re-exploration (ignore cache)
await device.ExploreServicesAsync(new ServiceExplorationOptions
{
    UseCache = false
});

// Filter by service UUID
await device.ExploreServicesAsync(new ServiceExplorationOptions
{
    ServiceUuidFilter = uuid => uuid == myServiceUuid
});
```

### Getting Services and Characteristics

```csharp
// Get a specific service by UUID
var service = device.GetService(serviceGuid);

// Or use a filter
var service = device.GetService(s => s.Id == serviceGuid);

// Explore characteristics (simple)
await service.ExploreCharacteristicsAsync();

// Explore characteristics AND descriptors
await service.ExploreCharacteristicsAsync(CharacteristicExplorationOptions.Full);

// Get a characteristic
var characteristic = service.GetCharacteristic(characteristicGuid);
```

### Reading and Writing

```csharp
// Read a characteristic value
var value = await characteristic.ReadValueAsync();
Console.WriteLine($"Value: {BitConverter.ToString(value.ToArray())}");

// Write a value
byte[] data = new byte[] { 0x01, 0x02, 0x03 };
await characteristic.WriteValueAsync(data);

// Check capabilities
if (characteristic.CanRead)
{
    // Safe to read
}

if (characteristic.CanWrite)
{
    // Safe to write
}
```

### Subscribing to Notifications

```csharp
// Subscribe to value changes
characteristic.ValueUpdated += (sender, args) =>
{
    Console.WriteLine($"New value: {BitConverter.ToString(args.NewValue.ToArray())}");
    Console.WriteLine($"Old value: {BitConverter.ToString(args.OldValue.ToArray())}");
};

// Start listening
await characteristic.StartListeningAsync();

// Check listening state
Console.WriteLine($"Listening: {characteristic.IsListening}");

// Stop listening when done
await characteristic.StopListeningAsync();
```

### Working with Descriptors

```csharp
// Explore descriptors
await characteristic.ExploreDescriptorsAsync();

// Get a specific descriptor
var descriptor = characteristic.GetDescriptor(descriptorGuid);

// Read descriptor value
var value = await descriptor.ReadValueAsync();

// Write descriptor value
await descriptor.WriteValueAsync(new byte[] { 0x01, 0x00 });
```

## Advanced Features

### Connection Priority (Android)

Optimize connection parameters for your use case:

```csharp
// High priority: Fast data transfer, low latency (11.25-15ms)
await device.RequestConnectionPriorityAsync(BluetoothConnectionPriority.High);

// Balanced: Moderate performance and power (30-50ms)
await device.RequestConnectionPriorityAsync(BluetoothConnectionPriority.Balanced);

// Low power: Battery optimization, higher latency (100-125ms)
await device.RequestConnectionPriorityAsync(BluetoothConnectionPriority.LowPower);
```

> **Note**: iOS, macOS, and Windows manage connection parameters automatically. This API is a no-op on those platforms.

### Timeout and Cancellation

All async operations support timeouts and cancellation:

```csharp
using var cts = new CancellationTokenSource();

try
{
    await device.ConnectAsync(
        connectionOptions: new ConnectionOptions(),
        timeout: TimeSpan.FromSeconds(10),
        cancellationToken: cts.Token
    );
}
catch (TimeoutException)
{
    Console.WriteLine("Connection timed out");
}
catch (OperationCanceledException)
{
    Console.WriteLine("Operation was cancelled");
}
```

### Event-Driven Architecture

Subscribe to events for reactive programming:

```csharp
// Scanner events
_scanner.RunningStateChanged += (s, e) => Console.WriteLine($"Scanning: {_scanner.IsRunning}");
_scanner.DeviceListChanged += OnDeviceListChanged;

// Device events
device.ConnectionStateChanged += (s, e) => Console.WriteLine($"State: {device.ConnectionState}");
device.Connected += (s, e) => Console.WriteLine("Connected");
device.Disconnected += (s, e) => Console.WriteLine("Disconnected");
device.UnexpectedDisconnection += (s, e) => Console.WriteLine($"Lost connection: {e.Exception}");

// Service events
service.CharacteristicListChanged += OnCharacteristicsChanged;

// Characteristic events
characteristic.ValueUpdated += OnValueUpdated;
```

### Caching and Performance

The exploration APIs use intelligent caching by default:

```csharp
// First call: Queries the device
await device.ExploreServicesAsync();  // UseCache = true (default)

// Subsequent calls: Returns cached results instantly
await device.ExploreServicesAsync();  // Cached, no device query

// Force refresh: Ignore cache
await device.ExploreServicesAsync(new ServiceExplorationOptions
{
    UseCache = false  // Forces device query
});
```

### Cleanup and Disposal

Proper cleanup ensures resources are released:

```csharp
// Clear services (stops notifications, clears cache)
await device.ClearServicesAsync();

// Clear specific service characteristics
await service.ClearCharacteristicsAsync();

// Clear characteristic descriptors
await characteristic.ClearDescriptorsAsync();

// Disconnect and dispose device
await device.DisconnectAsync();
await device.DisposeAsync();  // Implements IAsyncDisposable
```

## Platform-Specific Setup

### Android

Add Bluetooth permissions to `AndroidManifest.xml`:

```xml
<uses-permission android:name="android.permission.BLUETOOTH" />
<uses-permission android:name="android.permission.BLUETOOTH_ADMIN" />
<uses-permission android:name="android.permission.BLUETOOTH_SCAN"
                 android:usesPermissionFlags="neverForLocation" />
<uses-permission android:name="android.permission.BLUETOOTH_CONNECT" />
<uses-permission android:name="android.permission.ACCESS_FINE_LOCATION" />
```

### iOS / MacCatalyst

Add Bluetooth usage description to `Info.plist`:

```xml
<key>NSBluetoothAlwaysUsageDescription</key>
<string>This app needs Bluetooth to scan for BLE devices</string>
<key>NSBluetoothPeripheralUsageDescription</key>
<string>This app needs Bluetooth to scan for BLE devices</string>
```

### Windows

Add Bluetooth capability to `Package.appxmanifest`:

```xml
<Capabilities>
  <DeviceCapability Name="bluetooth" />
</Capabilities>
```

## Architecture

```text
Plugin.Bluetooth/
├── Bluetooth.Abstractions/              # Core interfaces (platform-agnostic)
│   ├── Exceptions/
│   ├── Extensions/
│   └── Options/
├── Bluetooth.Abstractions.Scanning/     # Scanning-specific interfaces
│   ├── Events/
│   ├── Exceptions/
│   └── Options/
├── Bluetooth.Abstractions.Broadcasting/ # Broadcasting interfaces
│   ├── Events/
│   ├── Exceptions/
│   └── Options/
├── Bluetooth.Core/                      # Base implementations
│   ├── Base classes
│   └── Infrastructure
├── Bluetooth.Core.Scanning/             # Scanning base implementations
├── Bluetooth.Core.Broadcasting/         # Broadcasting base implementations
├── Bluetooth.Maui/                      # MAUI integration & DI
└── Platform Implementations/
    ├── Bluetooth.Maui.Platforms.Apple/     # iOS & MacCatalyst
    ├── Bluetooth.Maui.Platforms.Droid/     # Android
    ├── Bluetooth.Maui.Platforms.Windows/   # Windows
    └── Bluetooth.Maui.Platforms.DotNetCore # Fallback (throws NotImplementedException)
```

## Core Interfaces

### Scanning

- **`IBluetoothScanner`** - Device discovery and scanning control
- **`IBluetoothRemoteDevice`** - Remote device representation and connection
- **`IBluetoothRemoteService`** - GATT service on a remote device
- **`IBluetoothRemoteCharacteristic`** - GATT characteristic with read/write/notify
- **`IBluetoothRemoteDescriptor`** - GATT descriptor

### Broadcasting (Android only)

- **`IBluetoothBroadcaster`** - Peripheral/advertising mode
- **`IBluetoothLocalService`** - Local GATT service
- **`IBluetoothLocalCharacteristic`** - Local characteristic for broadcasting
- **`IBluetoothConnectedDevice`** - Connected central device

## Exception Handling

Comprehensive exception hierarchy for error handling:

```csharp
try
{
    await device.ConnectAsync(connectionOptions);
}
catch (DeviceNotConnectedException ex)
{
    // Device is not connected when operation requires it
}
catch (ServiceNotFoundException ex)
{
    // Requested service not found on device
}
catch (CharacteristicNotFoundException ex)
{
    // Requested characteristic not found in service
}
catch (TimeoutException ex)
{
    // Operation timed out
}
catch (OperationCanceledException ex)
{
    // Operation was cancelled
}
catch (BluetoothException ex)
{
    // Base exception for all Bluetooth errors
}
```

## API Design Principles

1. **Async First** - All I/O operations are async with cancellation support
2. **Options Pattern** - Flexible configuration via options objects
3. **Caching** - Intelligent caching enabled by default for performance
4. **Events** - Event-driven architecture for reactive patterns
5. **IAsyncDisposable** - Proper resource cleanup with async disposal
6. **Immutability** - ReadOnlyMemory<byte> for value types
7. **Platform Parity** - Consistent API across all platforms

## Contributing

Contributions are welcome! Please:

1. Ensure all public APIs have XML documentation
2. Follow the existing code style and patterns
3. Add unit tests for new features
4. Update the README for API changes

## Requirements

- **.NET 10.0** or higher
- **.NET MAUI** application
- Platform-specific Bluetooth permissions (see setup section)

## License

MIT License - Copyright (c) 2025 Laerdal Medical

See [LICENSE.md](LICENSE.md) for details.

## Support

For issues, feature requests, or questions:
- 🐛 [GitHub Issues](https://github.com/laerdal/Plugin.Bluetooth/issues)
- 💬 [Discussions](https://github.com/laerdal/Plugin.Bluetooth/discussions)

## Changelog

### Recent Changes

**v1.0.0** (Current)
- ✅ Windows platform implementation complete
- ✅ Simplified exploration APIs (single method with options)
- ✅ Modern DI registration with `AddBluetoothServices()`
- ✅ Comprehensive XML documentation
- ✅ Options pattern for all configuration

---

Built with ❤️ by Laerdal Medical
