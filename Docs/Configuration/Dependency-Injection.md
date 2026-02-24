# Dependency Injection Configuration

This guide covers how to register and configure Plugin.Bluetooth services using dependency injection in your .NET MAUI application.

## Table of Contents

- [Basic Registration](#basic-registration)
- [Registration Chain](#registration-chain)
- [Configuring Options](#configuring-options)
- [Advanced Configuration](#advanced-configuration)
- [Platform-Specific Services](#platform-specific-services)

---

## Basic Registration

The simplest way to register Bluetooth services is using the `AddBluetoothServices()` extension method in your `MauiProgram.cs`:

```csharp
public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts => {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });

        // Register Bluetooth services
        builder.Services.AddBluetoothServices();

        return builder.Build();
    }
}
```

This single method call registers all necessary Bluetooth services including:
- `IBluetoothAdapter` - Core adapter for checking Bluetooth state
- `IBluetoothScanner` - Scanner for discovering BLE devices
- `IBluetoothBroadcaster` - Broadcaster for advertising as a BLE peripheral
- `IBluetoothFactory` - Factory for creating Bluetooth objects
- Platform-specific implementations for your target platform

All services are registered as **singletons** for optimal performance and resource management.

---

## Registration Chain

The `AddBluetoothServices()` method internally calls several registration methods in sequence:

```csharp
public static void AddBluetoothServices(this IServiceCollection services)
{
    services.AddSingleton<ITicker, Ticker>();
    services.AddBluetoothCoreServices();
    services.AddBluetoothCoreScanningServices();
    services.AddBluetoothCoreBroadcastingServices();

#if WINDOWS
    services.AddBluetoothMauiWindowsServices();
#elif ANDROID
    services.AddBluetoothMauiAndroidServices();
#elif IOS || MACCATALYST
    services.AddBluetoothMauiAppleServices();
#else
    services.AddBluetoothMauiDotNetServices();
#endif
}
```

### Registration Methods

| Method | Purpose | Services Registered |
|--------|---------|-------------------|
| `AddBluetoothCoreServices()` | Core infrastructure services | Ticker, Infrastructure options |
| `AddBluetoothCoreScanningServices()` | Scanning-related services | RSSI converters, signal strength smoothing |
| `AddBluetoothCoreBroadcastingServices()` | Broadcasting-related services | Broadcaster services |
| Platform-specific methods | Platform implementations | Native Bluetooth managers, adapters, scanners |

---

## Configuring Options

Plugin.Bluetooth uses the standard .NET Options pattern (`IOptions<T>`) for configuration. You can configure options using the `Configure<T>()` method.

### Infrastructure Options

Configure application-wide Bluetooth infrastructure settings:

```csharp
builder.Services.AddBluetoothServices();

builder.Services.Configure<BluetoothInfrastructureOptions>(options =>
{
    options.AutoCleanupOnStop = true;
    options.DefaultOperationTimeout = TimeSpan.FromSeconds(30);
    options.MaxConcurrentConnections = 5;
    options.EnableVerboseLogging = false;
});
```

See [Infrastructure-Options.md](./Infrastructure-Options.md) for detailed documentation.

### L2CAP Channel Options

Configure L2CAP channel behavior:

```csharp
builder.Services.Configure<L2CapChannelOptions>(options =>
{
    options.OpenTimeout = TimeSpan.FromSeconds(60);
    options.ReadTimeout = TimeSpan.FromSeconds(10);
    options.WriteTimeout = TimeSpan.FromSeconds(10);
    options.DefaultMtu = 672;
    options.EnableBackgroundReading = true;
});
```

See [L2CAP-Options.md](./L2CAP-Options.md) for detailed documentation.

---

## Advanced Configuration

### Configuring Core Services with Options

You can configure infrastructure options directly when adding core services:

```csharp
builder.Services.AddBluetoothCoreServices(options =>
{
    options.EnableVerboseLogging = true;
    options.DefaultOperationTimeout = TimeSpan.FromMinutes(1);
});
```

### Configuration from appsettings.json

You can bind options from configuration files:

```json
{
  "Bluetooth": {
    "Infrastructure": {
      "AutoCleanupOnStop": true,
      "DefaultOperationTimeout": "00:00:30",
      "MaxConcurrentConnections": 5,
      "EnableVerboseLogging": false
    },
    "L2CAP": {
      "OpenTimeout": "00:01:00",
      "ReadTimeout": "00:00:10",
      "WriteTimeout": "00:00:10",
      "DefaultMtu": 672,
      "EnableBackgroundReading": true
    }
  }
}
```

Then bind in `MauiProgram.cs`:

```csharp
var config = builder.Configuration;

builder.Services.Configure<BluetoothInfrastructureOptions>(
    config.GetSection("Bluetooth:Infrastructure"));

builder.Services.Configure<L2CapChannelOptions>(
    config.GetSection("Bluetooth:L2CAP"));
```

### Validating Options

Use the Options validation pattern to ensure configuration is valid:

```csharp
builder.Services.AddOptions<BluetoothInfrastructureOptions>()
    .Configure(options =>
    {
        options.MaxConcurrentConnections = 5;
    })
    .Validate(options =>
    {
        return options.MaxConcurrentConnections > 0;
    }, "MaxConcurrentConnections must be greater than 0");
```

### Accessing Options in Your Code

Inject `IOptions<T>` into your services to access configured options:

```csharp
public class MyBluetoothService
{
    private readonly BluetoothInfrastructureOptions _options;

    public MyBluetoothService(IOptions<BluetoothInfrastructureOptions> options)
    {
        _options = options.Value;
    }

    public void DoSomething()
    {
        var timeout = _options.DefaultOperationTimeout;
        // Use the configured timeout...
    }
}
```

---

## Platform-Specific Services

Each platform registers its own implementation of the core interfaces:

### Android
```csharp
services.AddBluetoothMauiAndroidServices();
```
Registers:
- Android BluetoothManager wrapper
- Android BLE scanner
- Android BLE broadcaster
- Android GATT implementations

### iOS/macOS
```csharp
services.AddBluetoothMauiAppleServices();
```
Registers:
- CoreBluetooth manager (CBCentralManager/CBPeripheralManager)
- Apple BLE scanner
- Apple BLE broadcaster
- CoreBluetooth GATT implementations

### Windows
```csharp
services.AddBluetoothMauiWindowsServices();
```
Registers:
- Windows.Devices.Bluetooth adapter
- Windows BLE scanner
- Windows BLE broadcaster
- WinRT GATT implementations

### Fallback (.NET/Unsupported)
```csharp
services.AddBluetoothMauiDotNetServices();
```
Registers stub implementations that throw `PlatformNotSupportedException`.

---

## Operation-Specific Options

While infrastructure options are set at startup via DI, operation-specific options are passed to methods when starting operations:

```csharp
// Infrastructure options - set once at startup
builder.Services.Configure<BluetoothInfrastructureOptions>(options => { ... });

// Operation options - passed per operation
await scanner.StartScanningAsync(new ScanningOptions
{
    ScanMode = BluetoothScanMode.LowLatency,
    ServiceUuids = new[] { myServiceUuid }
});

await device.ConnectAsync(new ConnectionOptions
{
    ConnectionRetry = RetryOptions.Aggressive,
    PermissionStrategy = PermissionRequestStrategy.RequestAutomatically
});
```

See the individual options documentation for details:
- [Scanning-Options.md](./Scanning-Options.md)
- [Connection-Options.md](./Connection-Options.md)
- [Broadcasting-Options.md](./Broadcasting-Options.md)
- [Exploration-Options.md](./Exploration-Options.md)

---

## Best Practices

1. **Register once**: Call `AddBluetoothServices()` only once in your `MauiProgram.cs`
2. **Configure early**: Set all options during application startup before any Bluetooth operations
3. **Use IOptions pattern**: Leverage the standard .NET options pattern for testability and flexibility
4. **Separate concerns**: Use infrastructure options for app-wide defaults, operation options for per-operation behavior
5. **Validate configuration**: Use options validation to catch configuration errors at startup
6. **Environment-specific config**: Use `appsettings.json` or environment variables for different configurations per environment

---

## Related Documentation

- [Infrastructure-Options.md](./Infrastructure-Options.md) - Infrastructure-level configuration
- [Scanning-Options.md](./Scanning-Options.md) - Scanner configuration
- [Connection-Options.md](./Connection-Options.md) - Connection configuration
- [L2CAP-Options.md](./L2CAP-Options.md) - L2CAP channel configuration
- [Exploration-Options.md](./Exploration-Options.md) - Service exploration configuration
- [Broadcasting-Options.md](./Broadcasting-Options.md) - Broadcasting configuration
