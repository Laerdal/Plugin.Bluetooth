# Dependency Injection Configuration

This guide covers how to register and configure Plugin.Bluetooth services using dependency injection in your .NET MAUI application.

## Table of Contents

- [Basic Registration](#basic-registration)
- [Registration Chain](#registration-chain)
- [Service Definitions](#service-definitions)
- [Configuring Options](#configuring-options)
- [Platform-Specific Services](#platform-specific-services)
- [Operation-Specific Options](#operation-specific-options)

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
- Platform-specific implementations for your target platform

All services are registered as **singletons** for optimal performance and resource management.

---

## Registration Chain

The `AddBluetoothServices()` method internally calls several registration methods in sequence:

```csharp
public static void AddBluetoothServices(this IServiceCollection services)
{
    ArgumentNullException.ThrowIfNull(services);

    services.AddSingleton<ITicker, Ticker>();
    services.AddBluetoothCoreServices();
    services.AddBluetoothSigProfiles();  // Registers Bluetooth SIG service definitions
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

    // Register unified facade wrappers as the default implementations —
    // these let client projects inherit a single class across all platforms
    services.AddSingleton<IBluetoothScanner, BluetoothScanner>();
    services.AddSingleton<IBluetoothBroadcaster, BluetoothBroadcaster>();
}
```

### Registration Methods

| Method | Purpose | Services Registered |
|--------|---------|-------------------|
| `AddBluetoothCoreServices()` | Core infrastructure services | Ticker (`TickerOptions`) |
| `AddBluetoothSigProfiles()` | Bluetooth SIG service definitions | Battery, Device Information, Generic Access/Attribute, Heart Rate, Health Thermometer, Environmental Sensing |
| `AddBluetoothCoreScanningServices()` | Scanning-related services | RSSI converters, signal strength smoothing, service definition registry, name provider |
| `AddBluetoothCoreBroadcastingServices()` | Broadcasting-related services | Broadcaster services |
| Platform-specific methods | Platform implementations | Native Bluetooth managers, adapters, scanners |
| Facade registration (last two lines above) | Unified cross-platform wrappers | `IBluetoothScanner` → `BluetoothScanner`, `IBluetoothBroadcaster` → `BluetoothBroadcaster` |

---

## Service Definitions

The `AddBluetoothSigProfiles()` method registers standard Bluetooth SIG service definitions, providing typed accessors for known characteristics. This is automatically called by `AddBluetoothServices()`.

### Built-in SIG Services

The following Bluetooth SIG services are automatically registered:

- **Battery Service** (0x180F) - Battery level monitoring
- **Device Information** (0x180A) - Manufacturer, model, firmware/hardware versions
- **Generic Access** (0x1800) - Device name, appearance
- **Generic Attribute** (0x1801) - Service change notifications
- **Heart Rate** (0x180D) - Heart rate measurements and control
- **Health Thermometer** (0x1809) - Temperature measurements
- **Environmental Sensing** (0x181A) - Temperature, humidity, pressure, UV index

For details on using service definitions, see [Service Definitions and Profiles](../Core-Concepts/Service-Definitions-And-Profiles.md).

### Registering Custom Service Definitions

You can register your own service definitions:

```csharp
builder.Services.AddBluetoothServices();

// Register custom service definition
builder.Services.AddSingleton<BluetoothServiceDefinitionRegistration>(_ => registry =>
{
    BluetoothServiceDefinitionRegistrar.Register(registry, typeof(MyCustomServiceDefinition));
});
```

**Note:** Custom service definitions must be marked with `[BluetoothServiceDefinition]` attribute and follow the service definition pattern. See [Service Definitions and Profiles](../Core-Concepts/Service-Definitions-And-Profiles.md#defining-a-service) for details.

---

## Configuring Options

Plugin.Bluetooth uses the standard .NET Options pattern (`IOptions<T>`) for per-operation configuration classes (`ScanningOptions`, `ConnectionOptions`, `L2CapChannelOptions`, etc. — see [Related Documentation](#related-documentation)). Most of these are passed directly to the relevant method call rather than registered via `Configure<T>()`; see each option class's own doc page for its actual configuration surface.

There is no `BluetoothInfrastructureOptions` type — see [Infrastructure-Options.md](./Infrastructure-Options.md) for what app-wide configuration actually exists (`TickerOptions`, and it's narrow).

### L2CAP Channel Options

`L2CapChannelOptions` is configured via the standard `IOptions<T>` pattern and injected into the platform L2CAP channel factory — it isn't passed to `OpenL2CapChannelAsync()` directly:

```csharp
builder.Services.Configure<L2CapChannelOptions>(options =>
{
    options.Mtu = 512;
    options.EnableBackgroundReading = true;
    options.AutoFlushWrites = true;
    options.ReadBufferSize = 512;
});
```

See [L2CAP-Options.md](./L2CAP-Options.md) for the full property reference.

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

A small number of options (`TickerOptions`, `L2CapChannelOptions`) are configured once at startup via DI, as shown above. Most option classes, however, are passed directly to the method call that uses them:

```csharp
// DI-configured options - set once at startup
builder.Services.Configure<L2CapChannelOptions>(options => { ... });

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
4. **Separate concerns**: Use DI-configured options (`TickerOptions`, `L2CapChannelOptions`) for app-wide defaults, operation options (`ScanningOptions`, `ConnectionOptions`, etc.) for per-operation behavior
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
