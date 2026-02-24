# Battery Optimization Best Practices

Battery life is a critical concern for BLE applications, especially on mobile devices. This guide provides strategies to minimize power consumption while maintaining functionality and responsiveness.

## Table of Contents

- [Scan Mode Selection](#scan-mode-selection)
- [Scan Duration Management](#scan-duration-management)
- [Background Scanning](#background-scanning)
- [Connection Priority Optimization](#connection-priority-optimization)
- [MTU Optimization](#mtu-optimization)
- [Service Caching](#service-caching)
- [Notification Management](#notification-management)
- [Platform-Specific Optimizations](#platform-specific-optimizations)

## Scan Mode Selection

The scan mode significantly impacts battery consumption. Choose the appropriate mode based on your use case.

### Scan Mode Comparison

| Mode | Power Consumption | Discovery Latency | Use Case |
|------|------------------|-------------------|----------|
| **LowPower** | Low | ~5 seconds | Background monitoring, passive scanning |
| **Balanced** | Medium | ~2 seconds | General purpose, default mode |
| **LowLatency** | High | < 1 second | Active device search, time-critical |
| **Opportunistic** | Minimal | Unpredictable | Background apps (Android only) |

### Choosing the Right Mode

```csharp
public class BatteryOptimizedScanner
{
    private readonly IBluetoothScanner _scanner;

    public async Task ScanForDeviceAsync(DeviceSearchPriority priority)
    {
        var options = new ScanningOptions
        {
            ScanMode = priority switch
            {
                // User is actively waiting - fast discovery
                DeviceSearchPriority.Immediate => BluetoothScanMode.LowLatency,

                // Background monitoring - preserve battery
                DeviceSearchPriority.Background => BluetoothScanMode.LowPower,

                // User can wait a bit - balanced approach
                _ => BluetoothScanMode.Balanced
            },

            // Filter out noise
            IgnoreNamelessAdvertisements = true,
            IgnoreDuplicateAdvertisements = priority == DeviceSearchPriority.Background
        };

        await _scanner.StartScanningAsync(options);
    }
}

public enum DeviceSearchPriority
{
    Immediate,
    Normal,
    Background
}
```

### Dynamic Scan Mode Adjustment

```csharp
public class AdaptiveScanManager
{
    private readonly IBluetoothScanner _scanner;
    private DateTime _scanStartTime;
    private bool _deviceFound;

    public async Task ScanWithDynamicModeAsync(CancellationToken cancellationToken = default)
    {
        _scanStartTime = DateTime.UtcNow;
        _deviceFound = false;

        // Start with balanced mode
        await StartScanAsync(BluetoothScanMode.Balanced);

        // Subscribe to device discovery
        _scanner.DeviceListChanged += OnDeviceListChanged;

        try
        {
            // After 10 seconds without finding device, switch to low latency
            await Task.Delay(TimeSpan.FromSeconds(10), cancellationToken);

            if (!_deviceFound)
            {
                _logger.LogInformation("No device found, switching to LowLatency mode");
                await UpdateScanModeAsync(BluetoothScanMode.LowLatency);
            }

            // After 30 seconds total, give up or switch to low power
            await Task.Delay(TimeSpan.FromSeconds(20), cancellationToken);

            if (!_deviceFound)
            {
                _logger.LogInformation("Extended search, switching to LowPower mode");
                await UpdateScanModeAsync(BluetoothScanMode.LowPower);
            }
        }
        finally
        {
            _scanner.DeviceListChanged -= OnDeviceListChanged;
        }
    }

    private void OnDeviceListChanged(object? sender, DeviceListChangedEventArgs e)
    {
        if (e.AddedDevices.Any(d => d.Name?.Contains("MyDevice") == true))
        {
            _deviceFound = true;
        }
    }

    private async Task StartScanAsync(BluetoothScanMode mode)
    {
        await _scanner.StartScanningAsync(new ScanningOptions { ScanMode = mode });
    }

    private async Task UpdateScanModeAsync(BluetoothScanMode newMode)
    {
        if (_scanner.IsRunning)
        {
            await _scanner.UpdateScannerOptionsAsync(
                new ScanningOptions { ScanMode = newMode });
        }
    }
}
```

## Scan Duration Management

Continuous scanning drains battery quickly. Always limit scan duration.

### Time-Limited Scanning

```csharp
public class TimeLimitedScanner
{
    private readonly IBluetoothScanner _scanner;

    public async Task<IBluetoothRemoteDevice?> ScanWithTimeoutAsync(
        string targetDeviceName,
        TimeSpan scanDuration,
        CancellationToken cancellationToken = default)
    {
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        cts.CancelAfter(scanDuration);

        try
        {
            await _scanner.StartScanningAsync(
                new ScanningOptions
                {
                    ScanMode = BluetoothScanMode.Balanced,
                    IgnoreNamelessAdvertisements = true
                },
                cancellationToken: cts.Token);

            // Wait for device or timeout
            var device = await WaitForDeviceAsync(
                targetDeviceName,
                cts.Token);

            return device;
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Scan timeout after {Duration}", scanDuration);
            return null;
        }
        finally
        {
            // Always stop scanning to save battery
            await _scanner.StopScanningAsync();
        }
    }

    private async Task<IBluetoothRemoteDevice?> WaitForDeviceAsync(
        string deviceName,
        CancellationToken cancellationToken)
    {
        var tcs = new TaskCompletionSource<IBluetoothRemoteDevice>();

        void OnDeviceListChanged(object? sender, DeviceListChangedEventArgs e)
        {
            var device = e.AddedDevices.FirstOrDefault(d => d.Name == deviceName);
            if (device != null)
            {
                tcs.TrySetResult(device);
            }
        }

        _scanner.DeviceListChanged += OnDeviceListChanged;

        try
        {
            using var registration = cancellationToken.Register(() =>
                tcs.TrySetCanceled(cancellationToken));

            // Check if device already in list
            var existing = _scanner.Devices.FirstOrDefault(d => d.Name == deviceName);
            if (existing != null)
                return existing;

            return await tcs.Task;
        }
        finally
        {
            _scanner.DeviceListChanged -= OnDeviceListChanged;
        }
    }
}
```

### Stop Scanning Immediately After Finding Device

```csharp
public class EfficientScanner
{
    private readonly IBluetoothScanner _scanner;

    public async Task<IBluetoothRemoteDevice> FindDeviceQuicklyAsync(
        Func<IBluetoothRemoteDevice, bool> deviceMatcher,
        CancellationToken cancellationToken = default)
    {
        var tcs = new TaskCompletionSource<IBluetoothRemoteDevice>();

        void OnDeviceListChanged(object? sender, DeviceListChangedEventArgs e)
        {
            var matchedDevice = e.AddedDevices.FirstOrDefault(deviceMatcher);
            if (matchedDevice != null)
            {
                tcs.TrySetResult(matchedDevice);
            }
        }

        _scanner.DeviceListChanged += OnDeviceListChanged;

        try
        {
            await _scanner.StartScanningAsync(
                new ScanningOptions { ScanMode = BluetoothScanMode.Balanced },
                cancellationToken: cancellationToken);

            using var registration = cancellationToken.Register(() =>
                tcs.TrySetCanceled(cancellationToken));

            var device = await tcs.Task;

            // CRITICAL: Stop scanning immediately to save battery
            await _scanner.StopScanningAsync();

            return device;
        }
        finally
        {
            _scanner.DeviceListChanged -= OnDeviceListChanged;
        }
    }
}
```

### Periodic Scanning Pattern

```csharp
public class PeriodicScanner
{
    private readonly IBluetoothScanner _scanner;
    private CancellationTokenSource? _scanCts;

    // Scan for 10 seconds every 60 seconds
    private readonly TimeSpan _scanDuration = TimeSpan.FromSeconds(10);
    private readonly TimeSpan _scanInterval = TimeSpan.FromSeconds(60);

    public async Task StartPeriodicScanningAsync(CancellationToken cancellationToken = default)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                _logger.LogInformation("Starting scan cycle");

                // Scan for limited duration
                _scanCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
                _scanCts.CancelAfter(_scanDuration);

                await _scanner.StartScanningAsync(
                    new ScanningOptions
                    {
                        ScanMode = BluetoothScanMode.LowPower, // Battery friendly
                        IgnoreDuplicateAdvertisements = true
                    },
                    cancellationToken: _scanCts.Token);

                // Wait for scan to complete or timeout
                await Task.Delay(_scanDuration, cancellationToken);

                await _scanner.StopScanningAsync();

                _logger.LogInformation("Scan cycle complete, sleeping for {Interval}", _scanInterval);

                // Sleep until next scan cycle
                await Task.Delay(_scanInterval - _scanDuration, cancellationToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during periodic scan");
                await Task.Delay(5000, cancellationToken); // Wait before retry
            }
        }

        await _scanner.StopScanningIfNeededAsync();
    }

    public async Task StopPeriodicScanningAsync()
    {
        _scanCts?.Cancel();
        await _scanner.StopScanningIfNeededAsync();
    }
}
```

## Background Scanning

Background scanning must be extremely battery-conscious.

### Android Background Scanning

```csharp
public class AndroidBackgroundScanner
{
    private readonly IBluetoothScanner _scanner;

    public async Task StartBackgroundScanAsync()
    {
        var options = new ScanningOptions
        {
            // Low power mode for background
            ScanMode = BluetoothScanMode.LowPower,

            // Or opportunistic for minimal battery impact
            // ScanMode = BluetoothScanMode.Opportunistic,

            // Request background location if needed (Android 10-11)
            RequireBackgroundLocation = true,

            // Reduce callback frequency
            IgnoreDuplicateAdvertisements = true,

            // Only scan for specific devices
            ServiceUuids = new[] { MyServiceUuid },

            // Platform-specific Android options
            Android = new AndroidScanningOptions
            {
                // Report devices after multiple matches (battery friendly)
                MatchMode = MatchMode.Sticky,

                // Wait for 3 matches before reporting
                NumOfMatches = MatchNum.FewAdvertisements,

                // Batch results (report every 5 seconds)
                ReportDelay = TimeSpan.FromSeconds(5)
            }
        };

        await _scanner.StartScanningAsync(options);
    }
}
```

### iOS Background Scanning

```csharp
public class IosBackgroundScanner
{
    private readonly IBluetoothScanner _scanner;

    public async Task StartBackgroundScanAsync()
    {
        // iOS automatically handles background scanning efficiently
        // when you specify service UUIDs
        var options = new ScanningOptions
        {
            // IMPORTANT: Must specify service UUIDs for background scanning on iOS
            ServiceUuids = new[] { MyServiceUuid },

            // iOS manages scan mode automatically in background
            ScanMode = BluetoothScanMode.LowPower,

            // Reduce duplicate callbacks
            IgnoreDuplicateAdvertisements = true
        };

        await _scanner.StartScanningAsync(options);
    }
}
```

## Connection Priority Optimization

Connection priority affects latency and power consumption. Use appropriately for your use case.

### Connection Priority Guidelines

```csharp
public class ConnectionPriorityManager
{
    public async Task ConfigureConnectionPriorityAsync(
        IBluetoothRemoteDevice device,
        ApplicationMode mode)
    {
        var priority = mode switch
        {
            // Real-time data (gaming, audio streaming)
            ApplicationMode.RealTime => BluetoothConnectionPriority.High,

            // Regular data transfer
            ApplicationMode.Normal => BluetoothConnectionPriority.Balanced,

            // Infrequent updates (temperature sensor, etc.)
            ApplicationMode.PowerSaving => BluetoothConnectionPriority.LowPower,

            _ => BluetoothConnectionPriority.Balanced
        };

        try
        {
            await device.RequestConnectionPriorityAsync(priority);

            _logger.LogInformation(
                "Set connection priority to {Priority} for mode {Mode}",
                priority,
                mode);
        }
        catch (Exception ex)
        {
            // iOS/Windows don't support this - it's Android-only
            _logger.LogDebug(ex, "Connection priority not supported on this platform");
        }
    }
}

public enum ApplicationMode
{
    RealTime,
    Normal,
    PowerSaving
}
```

### Dynamic Priority Adjustment

```csharp
public class DynamicPriorityManager
{
    private readonly IBluetoothRemoteDevice _device;
    private BluetoothConnectionPriority _currentPriority = BluetoothConnectionPriority.Balanced;

    public async Task StartDataTransferAsync()
    {
        // Switch to high priority for data transfer
        await SetPriorityAsync(BluetoothConnectionPriority.High);
    }

    public async Task FinishDataTransferAsync()
    {
        // Switch back to low power after transfer
        await SetPriorityAsync(BluetoothConnectionPriority.LowPower);
    }

    private async Task SetPriorityAsync(BluetoothConnectionPriority priority)
    {
        if (_currentPriority == priority)
            return;

        try
        {
            await _device.RequestConnectionPriorityAsync(priority);
            _currentPriority = priority;
        }
        catch (Exception ex)
        {
            _logger.LogDebug(ex, "Failed to set connection priority");
        }
    }
}
```

## MTU Optimization

Larger MTU means fewer packets, which reduces power consumption for large data transfers.

### MTU Negotiation

```csharp
public class MtuOptimizer
{
    public async Task<int> NegotiateOptimalMtuAsync(
        IBluetoothRemoteDevice device,
        int dataSize)
    {
        // Default MTU is 23 bytes (20 bytes payload)
        // Request larger MTU for better efficiency

        int requestedMtu = dataSize switch
        {
            // Small transfers - default is fine
            < 20 => 23,

            // Medium transfers - request moderate MTU
            < 200 => 200,

            // Large transfers - request maximum MTU
            _ => 517 // Maximum for BLE
        };

        try
        {
            var negotiatedMtu = await device.RequestMtuAsync(
                requestedMtu,
                timeout: TimeSpan.FromSeconds(5));

            _logger.LogInformation(
                "MTU negotiated: requested {Requested}, negotiated {Negotiated}",
                requestedMtu,
                negotiatedMtu);

            return negotiatedMtu;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "MTU negotiation failed, using default");
            return 23; // Default MTU
        }
    }

    public async Task TransferLargeDataAsync(
        IBluetoothRemoteDevice device,
        IBluetoothRemoteCharacteristic characteristic,
        byte[] data)
    {
        // Negotiate larger MTU before transfer
        var mtu = await NegotiateOptimalMtuAsync(device, data.Length);

        // Calculate optimal chunk size (MTU - 3 bytes for ATT header)
        var chunkSize = mtu - 3;

        // Transfer in chunks
        for (int i = 0; i < data.Length; i += chunkSize)
        {
            var chunk = data.Skip(i).Take(chunkSize).ToArray();
            await characteristic.WriteValueAsync(chunk);
        }

        _logger.LogInformation(
            "Transferred {Size} bytes with MTU {Mtu}",
            data.Length,
            mtu);
    }
}
```

## Service Caching

Cache discovered services to avoid repeated discovery operations.

### Service Cache Pattern

```csharp
public class CachedServiceManager
{
    private readonly IBluetoothRemoteDevice _device;
    private bool _servicesExplored;

    public async Task<IBluetoothRemoteService> GetServiceAsync(
        Guid serviceUuid,
        CancellationToken cancellationToken = default)
    {
        // Use cached services if already explored
        if (!_servicesExplored)
        {
            await _device.ExploreServicesAsync(
                ServiceExplorationOptions.WithCharacteristics,
                cancellationToken: cancellationToken);

            _servicesExplored = true;
        }

        // Get service from cache (no device query)
        var service = _device.GetService(serviceUuid);

        if (service == null)
        {
            // Force re-exploration if service not found
            await _device.ExploreServicesAsync(
                new ServiceExplorationOptions
                {
                    UseCache = false, // Force refresh
                    IncludeCharacteristics = true
                },
                cancellationToken: cancellationToken);

            service = _device.GetService(serviceUuid);
        }

        return service ?? throw new ServiceNotFoundException(serviceUuid);
    }

    public void InvalidateCache()
    {
        _servicesExplored = false;
    }
}
```

### Smart Cache Invalidation

```csharp
public class SmartCacheManager
{
    private readonly IBluetoothRemoteDevice _device;
    private DateTime _lastExploration = DateTime.MinValue;
    private readonly TimeSpan _cacheValidity = TimeSpan.FromMinutes(10);

    public async Task<IReadOnlyList<IBluetoothRemoteService>> GetServicesAsync(
        CancellationToken cancellationToken = default)
    {
        var cacheAge = DateTime.UtcNow - _lastExploration;

        if (cacheAge > _cacheValidity)
        {
            // Cache expired - refresh
            _logger.LogInformation("Service cache expired, refreshing...");

            await _device.ExploreServicesAsync(
                new ServiceExplorationOptions
                {
                    UseCache = false,
                    IncludeCharacteristics = true
                },
                cancellationToken: cancellationToken);

            _lastExploration = DateTime.UtcNow;
        }
        else
        {
            // Use cache
            _logger.LogDebug("Using cached services (age: {Age})", cacheAge);

            await _device.ExploreServicesAsync(
                ServiceExplorationOptions.WithCharacteristics,
                cancellationToken: cancellationToken);
        }

        return _device.Services;
    }
}
```

## Notification Management

Stop notifications when not needed to reduce power consumption.

### Notification Lifecycle

```csharp
public class NotificationManager
{
    private readonly IBluetoothRemoteCharacteristic _characteristic;

    public async Task StartMonitoringAsync()
    {
        if (!_characteristic.IsListening)
        {
            _characteristic.ValueUpdated += OnValueUpdated;
            await _characteristic.StartListeningAsync();
            _logger.LogInformation("Started notifications");
        }
    }

    public async Task StopMonitoringAsync()
    {
        if (_characteristic.IsListening)
        {
            await _characteristic.StopListeningAsync();
            _characteristic.ValueUpdated -= OnValueUpdated;
            _logger.LogInformation("Stopped notifications");
        }
    }

    private void OnValueUpdated(object? sender, CharacteristicValueUpdatedEventArgs e)
    {
        // Process notification
    }
}
```

### Conditional Notifications

```csharp
public class ConditionalNotificationManager
{
    private readonly IBluetoothRemoteCharacteristic _characteristic;
    private bool _isAppInForeground;

    public async Task OnAppStateChangedAsync(bool isInForeground)
    {
        _isAppInForeground = isInForeground;

        if (isInForeground)
        {
            // App in foreground - enable notifications
            await EnableNotificationsAsync();
        }
        else
        {
            // App in background - disable to save battery
            await DisableNotificationsAsync();
        }
    }

    private async Task EnableNotificationsAsync()
    {
        if (!_characteristic.IsListening)
        {
            _characteristic.ValueUpdated += OnValueUpdated;
            await _characteristic.StartListeningAsync();
            _logger.LogInformation("Enabled notifications (foreground)");
        }
    }

    private async Task DisableNotificationsAsync()
    {
        if (_characteristic.IsListening)
        {
            await _characteristic.StopListeningAsync();
            _characteristic.ValueUpdated -= OnValueUpdated;
            _logger.LogInformation("Disabled notifications (background)");
        }
    }

    private void OnValueUpdated(object? sender, CharacteristicValueUpdatedEventArgs e)
    {
        // Process notification
    }
}
```

## Platform-Specific Optimizations

### Android Optimizations

```csharp
public class AndroidBatteryOptimizations
{
    public ScanningOptions GetOptimizedScanOptions(bool isBackgroundScan)
    {
        return new ScanningOptions
        {
            ScanMode = isBackgroundScan
                ? BluetoothScanMode.Opportunistic // Minimal battery impact
                : BluetoothScanMode.LowPower,

            IgnoreDuplicateAdvertisements = true,

            Android = new AndroidScanningOptions
            {
                // Batch results for battery savings
                ReportDelay = isBackgroundScan
                    ? TimeSpan.FromSeconds(10)
                    : TimeSpan.Zero,

                // Use sticky matching for background
                MatchMode = isBackgroundScan
                    ? MatchMode.Sticky
                    : MatchMode.Aggressive,

                // Require multiple advertisements before reporting
                NumOfMatches = isBackgroundScan
                    ? MatchNum.FewAdvertisements
                    : MatchNum.OneAdvertisement,

                // Use LE 1M PHY for better range and lower power
                Phy = PhyType.Le1M
            }
        };
    }

    public ConnectionOptions GetOptimizedConnectionOptions(bool isLongLived)
    {
        return new ConnectionOptions
        {
            Android = new AndroidConnectionOptions
            {
                // Long-lived connections benefit from autoConnect
                AutoConnect = isLongLived,

                // Use low power priority for long-lived connections
                ConnectionPriority = isLongLived
                    ? BluetoothConnectionPriority.LowPower
                    : BluetoothConnectionPriority.Balanced,

                // LE transport for BLE devices
                TransportType = BluetoothTransportType.Le
            }
        };
    }
}
```

### iOS/macOS Optimizations

```csharp
public class AppleBatteryOptimizations
{
    public ScanningOptions GetOptimizedScanOptions()
    {
        return new ScanningOptions
        {
            // iOS manages scan mode automatically, but still use LowPower as hint
            ScanMode = BluetoothScanMode.LowPower,

            // CRITICAL: Always specify service UUIDs for background scanning
            ServiceUuids = new[] { MyServiceUuid },

            // Reduce duplicate callbacks
            IgnoreDuplicateAdvertisements = true
        };
    }

    public ConnectionOptions GetOptimizedConnectionOptions(bool isBackgroundCapable)
    {
        return new ConnectionOptions
        {
            Apple = new AppleConnectionOptions
            {
                // Only enable background alerts if absolutely necessary
                NotifyOnConnection = isBackgroundCapable,
                NotifyOnDisconnection = isBackgroundCapable,
                NotifyOnNotification = isBackgroundCapable
            }
        };
    }
}
```

## Battery Life Measurement

### Monitoring Battery Impact

```csharp
public class BatteryImpactMonitor
{
    private DateTime _scanStartTime;
    private TimeSpan _totalScanTime;
    private int _connectionCount;
    private int _notificationCount;

    public void OnScanStarted()
    {
        _scanStartTime = DateTime.UtcNow;
    }

    public void OnScanStopped()
    {
        _totalScanTime += DateTime.UtcNow - _scanStartTime;
        _logger.LogInformation("Total scan time: {Duration}", _totalScanTime);
    }

    public void OnDeviceConnected()
    {
        _connectionCount++;
        _logger.LogInformation("Total connections: {Count}", _connectionCount);
    }

    public void OnNotificationReceived()
    {
        _notificationCount++;

        if (_notificationCount % 100 == 0)
        {
            _logger.LogInformation(
                "Notifications received: {Count}",
                _notificationCount);
        }
    }

    public BatteryImpactReport GetReport()
    {
        return new BatteryImpactReport
        {
            TotalScanTime = _totalScanTime,
            ConnectionCount = _connectionCount,
            NotificationCount = _notificationCount
        };
    }
}

public record BatteryImpactReport
{
    public TimeSpan TotalScanTime { get; init; }
    public int ConnectionCount { get; init; }
    public int NotificationCount { get; init; }
}
```

## Summary

### Battery Optimization Checklist

- [ ] Use **LowPower** or **Balanced** scan modes (avoid LowLatency unless necessary)
- [ ] **Stop scanning immediately** after finding device
- [ ] **Limit scan duration** (use timeouts)
- [ ] **Specify service UUIDs** when possible (especially for iOS background)
- [ ] Use **IgnoreDuplicateAdvertisements** to reduce callbacks
- [ ] Set **connection priority** to LowPower for infrequent data updates
- [ ] **Negotiate larger MTU** for bulk data transfers
- [ ] **Cache service discovery** results (use `UseCache = true`)
- [ ] **Stop notifications** when not needed
- [ ] Use **periodic scanning** instead of continuous scanning
- [ ] Implement **opportunistic scanning** for background Android apps
- [ ] **Monitor battery impact** during development and testing

### Power Consumption Hierarchy

**Highest Power Consumption:**
1. Continuous scanning with LowLatency mode
2. Multiple simultaneous connections with High priority
3. Frequent notifications with small MTU
4. Repeated service discovery

**Lowest Power Consumption:**
1. Opportunistic or periodic scanning with LowPower mode
2. Single connection with LowPower priority
3. Infrequent notifications with large MTU
4. Cached service discovery

### Related Topics

- [Connection Management](Connection-Management.md) - Optimize connection strategies
- [Performance](Performance.md) - Balance performance with battery life
- [Error Handling](Error-Handling.md) - Handle errors efficiently
