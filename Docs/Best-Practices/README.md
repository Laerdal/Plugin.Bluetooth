# Best Practices for Plugin.Bluetooth

This section provides comprehensive best practices, patterns, and recommendations for building robust, efficient, and maintainable Bluetooth Low Energy applications with Plugin.Bluetooth.

## Overview

Building reliable BLE applications requires attention to connection management, battery optimization, error handling, and performance considerations. These guides provide practical, battle-tested patterns that help you build production-ready applications.

## Best Practice Guides

### [Connection Management](Connection-Management.md)
Learn how to manage BLE connections effectively, handle unexpected disconnections, and implement connection retry strategies.

**Key Topics:**
- Connection lifecycle management
- MaxConcurrentConnections configuration
- Handling unexpected disconnections
- Connection state monitoring
- Auto-reconnection patterns
- Connection timeouts and cancellation

### [Battery Optimization](Battery-Optimization.md)
Optimize your application for battery life while maintaining functionality and responsiveness.

**Key Topics:**
- Scan mode selection (LowPower/Balanced/LowLatency)
- Scan duration management
- Background scanning strategies
- Connection priority optimization
- MTU optimization for fewer transmissions
- Service caching strategies

### [Error Handling](Error-Handling.md)
Implement robust error handling with proper exception management and recovery strategies.

**Key Topics:**
- Try-catch patterns for BLE operations
- Exception hierarchy and specific exception types
- RetryOptions configuration
- AggregateException handling
- Platform-specific error codes
- Graceful degradation strategies

### [Performance](Performance.md)
Maximize throughput and minimize latency for data-intensive BLE applications.

**Key Topics:**
- MTU negotiation and optimization
- Write without response for high throughput
- Service caching for faster reconnection
- L2CAP channels for bulk data transfer
- Connection priority tuning
- Batch operations and queuing

### [MVVM Integration](MVVM-Integration.md)
Integrate Plugin.Bluetooth seamlessly with MVVM pattern and data binding frameworks.

**Key Topics:**
- INotifyPropertyChanged support
- Data binding patterns
- ViewModel design patterns
- CommunityToolkit.Mvvm integration
- Command patterns for async operations
- ObservableCollection management

### [Testing](Testing.md)
Strategies for testing BLE functionality, including mocking and dependency injection.

**Key Topics:**
- Mocking BLE interfaces
- Dependency injection for testability
- Unit testing patterns
- Integration testing strategies
- Simulating BLE devices
- Testing edge cases and error conditions

## Quick Reference

### Common Patterns

#### Basic Scanning and Connection
```csharp
// Recommended pattern for scanning and connecting
public class BluetoothService
{
    private readonly IBluetoothScanner _scanner;

    public BluetoothService(IBluetoothScanner scanner)
    {
        _scanner = scanner;
    }

    public async Task<IBluetoothRemoteDevice> ConnectToDeviceAsync(
        string deviceName,
        CancellationToken cancellationToken = default)
    {
        // Start scanning with optimized settings
        var scanOptions = new ScanningOptions
        {
            ScanMode = BluetoothScanMode.Balanced,
            IgnoreNamelessAdvertisements = true
        };

        await _scanner.StartScanningAsync(scanOptions, cancellationToken: cancellationToken);

        // Wait for device to appear
        var device = await WaitForDeviceAsync(deviceName, cancellationToken);

        // Stop scanning to save battery
        await _scanner.StopScanningAsync();

        // Connect with retry configuration
        var connectionOptions = new ConnectionOptions
        {
            ConnectionRetry = RetryOptions.Default
        };

        await device.ConnectAsync(connectionOptions, cancellationToken: cancellationToken);

        return device;
    }
}
```

#### Robust Data Read/Write
```csharp
// Recommended pattern for reading and writing data
public async Task<byte[]> ReadDataWithRetryAsync(
    IBluetoothRemoteCharacteristic characteristic,
    CancellationToken cancellationToken = default)
{
    var retryCount = 0;
    const int maxRetries = 3;

    while (retryCount < maxRetries)
    {
        try
        {
            var value = await characteristic.ReadValueAsync(
                timeout: TimeSpan.FromSeconds(10),
                cancellationToken: cancellationToken);

            return value.ToArray();
        }
        catch (TimeoutException) when (retryCount < maxRetries - 1)
        {
            retryCount++;
            await Task.Delay(200 * retryCount, cancellationToken); // Exponential backoff
        }
        catch (DeviceNotConnectedException)
        {
            // Device disconnected - attempt reconnection
            throw; // Or implement reconnection logic
        }
    }

    throw new BluetoothException("Failed to read characteristic after retries");
}
```

### Configuration Checklist

#### Application Startup
```csharp
// MauiProgram.cs - Configure Bluetooth services
public static MauiApp CreateMauiApp()
{
    var builder = MauiApp.CreateBuilder();

    builder
        .UseMauiApp<App>()
        .UseMauiCommunityToolkit();

    // Register Bluetooth services
    builder.Services.AddBluetoothServices();

    // Configure infrastructure options
    builder.Services.Configure<BluetoothInfrastructureOptions>(options =>
    {
        options.MaxConcurrentConnections = 3;
        options.DefaultOperationTimeout = TimeSpan.FromSeconds(30);
        options.EnableVerboseLogging = false; // Enable for debugging only
        options.AutoCleanupOnStop = true;
    });

    return builder.Build();
}
```

#### Platform-Specific Setup

**Android (AndroidManifest.xml)**
```xml
<uses-permission android:name="android.permission.BLUETOOTH" />
<uses-permission android:name="android.permission.BLUETOOTH_ADMIN" />
<uses-permission android:name="android.permission.BLUETOOTH_SCAN"
                 android:usesPermissionFlags="neverForLocation" />
<uses-permission android:name="android.permission.BLUETOOTH_CONNECT" />
<uses-permission android:name="android.permission.ACCESS_FINE_LOCATION" />
```

**iOS/macOS (Info.plist)**
```xml
<key>NSBluetoothAlwaysUsageDescription</key>
<string>This app uses Bluetooth to connect to BLE devices</string>
<key>NSBluetoothPeripheralUsageDescription</key>
<string>This app uses Bluetooth to communicate with peripherals</string>
```

**Windows (Package.appxmanifest)**
```xml
<Capabilities>
  <DeviceCapability Name="bluetooth" />
</Capabilities>
```

## Design Principles

### 1. Fail Fast, Recover Gracefully
- Use RetryOptions for transient failures
- Catch specific exceptions rather than generic Exception
- Provide meaningful error messages to users
- Log errors with context for debugging

### 2. Optimize for Battery Life
- Use appropriate scan modes (LowPower when possible)
- Limit scan duration
- Stop scanning when not needed
- Use connection priority appropriately
- Cache service/characteristic discovery results

### 3. Be Responsive
- Use CancellationToken for all async operations
- Set reasonable timeouts
- Provide progress feedback to users
- Don't block the UI thread

### 4. Clean Up Resources
- Always disconnect devices when done
- Dispose of devices properly (IAsyncDisposable)
- Stop scanning when device found
- Clear caches when appropriate

### 5. Test Thoroughly
- Test on multiple devices and platforms
- Test edge cases (disconnections, timeouts, errors)
- Use dependency injection for testability
- Mock BLE interfaces for unit tests

## Anti-Patterns to Avoid

### ❌ Don't: Scan Continuously
```csharp
// BAD: Drains battery
await _scanner.StartScanningAsync();
// Scanning forever...
```

### ✅ Do: Scan with Duration Limit
```csharp
// GOOD: Stop scanning after timeout or device found
await _scanner.StartScanningAsync();
using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
var device = await WaitForDeviceAsync(targetName, cts.Token);
await _scanner.StopScanningAsync();
```

### ❌ Don't: Ignore Disconnections
```csharp
// BAD: No handling for unexpected disconnection
await device.ConnectAsync();
await characteristic.WriteValueAsync(data);
```

### ✅ Do: Handle Unexpected Disconnections
```csharp
// GOOD: Subscribe to disconnection events
device.UnexpectedDisconnection += async (s, e) =>
{
    _logger.LogWarning("Device disconnected unexpectedly: {Exception}", e.Exception);
    await AttemptReconnectionAsync();
};
```

### ❌ Don't: Use Default MTU for Large Transfers
```csharp
// BAD: Using default MTU (20 bytes) for large transfers - slow!
await characteristic.WriteValueAsync(largeData);
```

### ✅ Do: Negotiate Larger MTU
```csharp
// GOOD: Request larger MTU for better throughput
await device.RequestMtuAsync(512);
await characteristic.WriteValueAsync(largeData);
```

### ❌ Don't: Swallow Exceptions
```csharp
// BAD: Silent failure
try
{
    await device.ConnectAsync();
}
catch { }
```

### ✅ Do: Handle Exceptions Appropriately
```csharp
// GOOD: Log and handle specific exceptions
try
{
    await device.ConnectAsync();
}
catch (TimeoutException ex)
{
    _logger.LogWarning("Connection timeout: {Message}", ex.Message);
    // Retry or notify user
}
catch (BluetoothException ex)
{
    _logger.LogError(ex, "Bluetooth error during connection");
    throw;
}
```

## Platform Considerations

### Android
- **GATT Error 133**: Most common connection error, usually requires retry
- **Connection Priority**: Use High priority for real-time apps, LowPower for sensors
- **MTU Negotiation**: Explicitly request MTU (up to 517 bytes)
- **Scan Throttling**: Android limits scan frequency; respect throttling limits

### iOS/macOS
- **Automatic Management**: System manages MTU and connection parameters
- **Background Modes**: Requires proper Info.plist configuration
- **CBCentralManager State**: Always check state before operations
- **Service Caching**: System caches services aggressively

### Windows
- **Adapter State**: Check adapter availability before operations
- **Pairing**: Some operations may require device pairing
- **Background Support**: Limited compared to mobile platforms

## Additional Resources

- [Core Concepts](../Core-Concepts/README.md) - Understanding fundamental concepts
- [Advanced Topics](../Advanced/README.md) - Deep dives into advanced features
- [Troubleshooting](../Troubleshooting/README.md) - Solutions to common issues
- [API Reference](../API-Reference/README.md) - Complete API documentation

## Contributing

Have a best practice to share? Please contribute to this documentation by opening a pull request or issue in the repository.

---

**Next Steps:**
1. Read [Connection Management](Connection-Management.md) for connection best practices
2. Study [Battery Optimization](Battery-Optimization.md) for power efficiency
3. Review [Error Handling](Error-Handling.md) for robust error management
