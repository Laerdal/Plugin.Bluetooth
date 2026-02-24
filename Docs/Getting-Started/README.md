# Getting Started with Plugin.Bluetooth

Welcome to Plugin.Bluetooth! This guide will help you set up and start using Bluetooth Low Energy (BLE) in your .NET MAUI application.

## Quick Start

This guide covers:

1. [Installation](#installation)
2. [Basic Setup](#basic-setup)
3. [Scanning for Devices](#scanning-for-devices)
4. [Connecting to Devices](#connecting-to-devices)
5. [Working with Services](#working-with-services)
6. [Next Steps](#next-steps)

## Installation

Add the Plugin.Bluetooth package to your .NET MAUI project:

```bash
dotnet add package Bluetooth.Maui
```

Or via NuGet Package Manager:

```xml
<PackageReference Include="Bluetooth.Maui" Version="1.0.0" />
```

## Basic Setup

### 1. Register Services

In your `MauiProgram.cs`, register the Bluetooth services with dependency injection:

```csharp
using Bluetooth.Maui;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });

        // Register Bluetooth services
        builder.Services.AddBluetoothServices();

        // Register your app services
        builder.Services.AddSingleton<MainViewModel>();
        builder.Services.AddTransient<MainPage>();

        return builder.Build();
    }
}
```

> The `AddBluetoothServices()` method registers all necessary Bluetooth services including the scanner, broadcaster, and adapter management.

### 2. Configure Platform Permissions

Before using Bluetooth, you must configure platform-specific permissions and capabilities. See the [Platform Setup Guide](./Platform-Setup.md) for detailed instructions for each platform.

**Quick checklist:**

- **iOS/MacCatalyst**: Add Bluetooth usage descriptions to `Info.plist`
- **Android**: Add Bluetooth permissions to `AndroidManifest.xml` (varies by API level)
- **Windows**: Add Bluetooth capability to `Package.appxmanifest`

> For complete platform configuration details, see [Platform Setup](./Platform-Setup.md).

## Scanning for Devices

### Basic Scanning

Inject `IBluetoothScanner` into your view model or service:

```csharp
using Bluetooth.Abstractions.Scanning;

public class MainViewModel
{
    private readonly IBluetoothScanner _scanner;

    public MainViewModel(IBluetoothScanner scanner)
    {
        _scanner = scanner;

        // Subscribe to device discovery events
        _scanner.DeviceListChanged += OnDeviceListChanged;
    }

    public async Task StartScanningAsync()
    {
        try
        {
            // Start scanning with default options
            await _scanner.StartScanningAsync();
        }
        catch (BluetoothPermissionException ex)
        {
            // Handle permission denied
            Console.WriteLine($"Permission denied: {ex.Message}");
        }
    }

    public async Task StopScanningAsync()
    {
        await _scanner.StopScanningAsync();
    }

    private void OnDeviceListChanged(object sender, DeviceListChangedEventArgs e)
    {
        // Get all discovered devices
        var devices = _scanner.GetDevices();

        foreach (var device in devices)
        {
            Console.WriteLine($"Found: {device.Name} ({device.Id})");
        }
    }
}
```

### Scanning with Options

Customize scanning behavior with `ScanningOptions`:

```csharp
using Bluetooth.Abstractions.Scanning.Options;

public async Task StartScanningWithOptionsAsync()
{
    var options = new ScanningOptions
    {
        // Only discover devices with specific services
        ServiceUuids = new[]
        {
            Guid.Parse("0000180d-0000-1000-8000-00805f9b34fb") // Heart Rate Service
        },

        // Ignore devices without names
        IgnoreNamelessAdvertisements = true,

        // Optimize for low power consumption
        ScanMode = BluetoothScanMode.LowPower,

        // Automatically request permissions (default)
        PermissionStrategy = PermissionRequestStrategy.RequestAutomatically
    };

    await _scanner.StartScanningAsync(options);
}
```

### Getting Specific Devices

```csharp
// Get device by ID
var device = _scanner.GetDeviceOrDefault("AA:BB:CC:DD:EE:FF");

// Get device by filter
var heartRateDevice = _scanner.GetDeviceOrDefault(d =>
    d.Name.Contains("Heart Rate"));

// Get closest device (by RSSI)
var closestDevice = _scanner.GetClosestDeviceOrDefault();

// Wait for a specific device to appear
var myDevice = await _scanner.WaitForDeviceToAppearAsync(
    d => d.Name == "My BLE Device",
    timeout: TimeSpan.FromSeconds(30)
);
```

## Connecting to Devices

### Basic Connection

```csharp
using Bluetooth.Abstractions.Scanning;

public async Task ConnectToDeviceAsync(IBluetoothRemoteDevice device)
{
    try
    {
        // Subscribe to connection state changes
        device.Connected += OnDeviceConnected;
        device.Disconnected += OnDeviceDisconnected;

        // Connect with default options
        await device.ConnectAsync();

        Console.WriteLine($"Connected to {device.Name}");
    }
    catch (DeviceFailedToConnectException ex)
    {
        Console.WriteLine($"Connection failed: {ex.Message}");
    }
}

private void OnDeviceConnected(object sender, EventArgs e)
{
    Console.WriteLine("Device connected");
}

private void OnDeviceDisconnected(object sender, EventArgs e)
{
    Console.WriteLine("Device disconnected");
}
```

### Connection with Options

```csharp
using Bluetooth.Abstractions.Scanning.Options;

public async Task ConnectWithOptionsAsync(IBluetoothRemoteDevice device)
{
    var options = new ConnectionOptions
    {
        // Wait for advertisement before connecting
        WaitForAdvertisementBeforeConnecting = true,

        // Platform-specific Android options
        Android = new AndroidConnectionOptions
        {
            // Enable auto-reconnect on connection loss
            AutoConnect = true,

            // Request high performance
            ConnectionPriority = BluetoothConnectionPriority.High
        }
    };

    await device.ConnectAsync(options);
}
```

### Disconnecting

```csharp
public async Task DisconnectAsync(IBluetoothRemoteDevice device)
{
    try
    {
        await device.DisconnectAsync();
    }
    catch (DeviceFailedToDisconnectException ex)
    {
        Console.WriteLine($"Disconnect failed: {ex.Message}");
    }
}

// Alternative: Disconnect only if connected
await device.DisconnectIfNeededAsync();
```

## Working with Services

### Discovering Services

After connecting, explore the device's GATT services and characteristics:

```csharp
public async Task ExploreDeviceAsync(IBluetoothRemoteDevice device)
{
    try
    {
        // Ensure device is connected
        if (!device.IsConnected)
        {
            await device.ConnectAsync();
        }

        // Discover services only (default)
        await device.ExploreServicesAsync();

        // Or discover services and characteristics
        await device.ExploreServicesAsync(ServiceExplorationOptions.WithCharacteristics);

        // Or full exploration (services + characteristics + descriptors)
        await device.ExploreServicesAsync(ServiceExplorationOptions.Full);
    }
    catch (DeviceNotConnectedException ex)
    {
        Console.WriteLine("Device must be connected to explore services");
    }
}
```

### Accessing Services

```csharp
public async Task ReadHeartRateAsync(IBluetoothRemoteDevice device)
{
    // Get Heart Rate Service
    var heartRateServiceUuid = Guid.Parse("0000180d-0000-1000-8000-00805f9b34fb");
    var heartRateService = device.GetServiceOrDefault(heartRateServiceUuid);

    if (heartRateService == null)
    {
        Console.WriteLine("Heart Rate Service not found");
        return;
    }

    Console.WriteLine($"Found service: {heartRateService.Name}");

    // Get all services
    var allServices = device.GetServices();
    foreach (var service in allServices)
    {
        Console.WriteLine($"Service: {service.Name} ({service.Id})");
    }
}
```

> For detailed information on reading, writing, and subscribing to characteristics, see the [Characteristics Guide](../CHARACTERISTIC_INTERACTION.md).

## Permission Handling

Plugin.Bluetooth provides flexible permission handling through `PermissionRequestStrategy`:

### RequestAutomatically (Default)

Permissions are requested automatically when needed:

```csharp
// Permissions requested automatically before scanning
await _scanner.StartScanningAsync();
```

### ThrowIfNotGranted

Take explicit control over permission requests:

```csharp
// Check permissions first
if (!await _scanner.HasScannerPermissionsAsync())
{
    // Request permissions explicitly
    await _scanner.RequestScannerPermissionsAsync();
}

// Start scanning with explicit permission strategy
var options = new ScanningOptions
{
    PermissionStrategy = PermissionRequestStrategy.ThrowIfNotGranted
};

await _scanner.StartScanningAsync(options);
```

### AssumeGranted

Skip permission checks (use with caution):

```csharp
var options = new ScanningOptions
{
    PermissionStrategy = PermissionRequestStrategy.AssumeGranted
};

await _scanner.StartScanningAsync(options);
```

> For comprehensive permission details, see the [Permissions Guide](./Permissions.md).

## Best Practices

### 1. Always Handle Exceptions

BLE operations can fail for various reasons. Always wrap operations in try-catch blocks:

```csharp
try
{
    await device.ConnectAsync();
}
catch (DeviceFailedToConnectException ex)
{
    // Handle connection failure
}
catch (BluetoothPermissionException ex)
{
    // Handle permission denial
}
catch (TimeoutException ex)
{
    // Handle timeout
}
```

### 2. Manage Scanner Lifecycle

Stop scanning when not needed to conserve battery:

```csharp
public class MainViewModel : IDisposable
{
    private readonly IBluetoothScanner _scanner;

    public MainViewModel(IBluetoothScanner scanner)
    {
        _scanner = scanner;
    }

    public void Dispose()
    {
        // Stop scanning and clean up
        _ = _scanner.StopScanningIfNeededAsync();
        _scanner.DeviceListChanged -= OnDeviceListChanged;
    }
}
```

### 3. Handle Connection State

Monitor connection state to handle unexpected disconnections:

```csharp
device.UnexpectedDisconnection += async (sender, args) =>
{
    Console.WriteLine($"Unexpected disconnection: {args.Reason}");

    // Attempt reconnection
    try
    {
        await device.ConnectAsync();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Reconnection failed: {ex.Message}");
    }
};
```

### 4. Use Cancellation Tokens

Support cancellation for long-running operations:

```csharp
private CancellationTokenSource _cts = new();

public async Task ScanWithCancellationAsync()
{
    try
    {
        var device = await _scanner.WaitForDeviceToAppearAsync(
            filter: d => d.Name == "My Device",
            timeout: TimeSpan.FromSeconds(30),
            cancellationToken: _cts.Token
        );
    }
    catch (OperationCanceledException)
    {
        Console.WriteLine("Scan cancelled");
    }
}

public void CancelScan()
{
    _cts.Cancel();
}
```

## Complete Example

Here's a complete example showing scanning, connection, and service discovery:

```csharp
using Bluetooth.Abstractions.Scanning;
using Bluetooth.Abstractions.Scanning.Options;

public class BluetoothService
{
    private readonly IBluetoothScanner _scanner;

    public BluetoothService(IBluetoothScanner scanner)
    {
        _scanner = scanner;
        _scanner.DeviceListChanged += OnDeviceListChanged;
    }

    public async Task ScanAndConnectAsync()
    {
        try
        {
            // Start scanning for devices
            var options = new ScanningOptions
            {
                IgnoreNamelessAdvertisements = true,
                ScanMode = BluetoothScanMode.Balanced
            };

            await _scanner.StartScanningAsync(options);

            // Wait for specific device
            var device = await _scanner.WaitForDeviceToAppearAsync(
                d => d.Name.Contains("Heart Rate"),
                timeout: TimeSpan.FromSeconds(30)
            );

            Console.WriteLine($"Found device: {device.Name}");

            // Stop scanning to save battery
            await _scanner.StopScanningAsync();

            // Connect to device
            await device.ConnectAsync();

            // Explore services and characteristics
            await device.ExploreServicesAsync(ServiceExplorationOptions.WithCharacteristics);

            // Get Heart Rate Service
            var heartRateServiceId = Guid.Parse("0000180d-0000-1000-8000-00805f9b34fb");
            var service = device.GetServiceOrDefault(heartRateServiceId);

            if (service != null)
            {
                Console.WriteLine($"Connected to {service.Name}");
                // Continue with characteristic operations...
            }
        }
        catch (TimeoutException)
        {
            Console.WriteLine("Device not found within timeout");
        }
        catch (BluetoothPermissionException ex)
        {
            Console.WriteLine($"Permission denied: {ex.Message}");
        }
        catch (DeviceFailedToConnectException ex)
        {
            Console.WriteLine($"Connection failed: {ex.Message}");
        }
    }

    private void OnDeviceListChanged(object sender, DeviceListChangedEventArgs e)
    {
        var devices = _scanner.GetDevices();
        Console.WriteLine($"Discovered {devices.Count} devices");
    }
}
```

## Next Steps

Now that you've got the basics, explore more advanced topics:

- [Platform Setup Guide](./Platform-Setup.md) - Detailed platform configuration
- [Permissions Guide](./Permissions.md) - Platform-specific permission handling
- [Characteristic Interaction](../CHARACTERISTIC_INTERACTION.md) - Read, write, and subscribe to characteristics
- [Architecture Guidelines](../ARCHITECTURE_GUIDELINES.md) - Understand the library architecture

## Common Issues

### "Permission denied" on Android

Make sure you've added the correct permissions to `AndroidManifest.xml` based on your target API level. See [Platform Setup](./Platform-Setup.md#android).

### iOS permission dialog not appearing

Ensure you've added `NSBluetoothAlwaysUsageDescription` to your `Info.plist`. See [Platform Setup](./Platform-Setup.md#ios--maccatalyst).

### Devices not discovered on Windows

Verify that the `bluetooth` capability is declared in `Package.appxmanifest`. See [Platform Setup](./Platform-Setup.md#windows).

### Connection failures on Android

Connection errors (especially GATT error 133) are common on Android. The library automatically retries connections. You can customize retry behavior through `ConnectionOptions`.

## Support

- Report issues on [GitHub](https://github.com/laerdal/Plugin.Bluetooth/issues)
- Check [existing documentation](../)
- Review the [sample application](/Bluetooth.Maui.Sample.Scanner)
