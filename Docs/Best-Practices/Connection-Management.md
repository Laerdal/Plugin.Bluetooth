# Connection Management Best Practices

Effective connection management is crucial for building reliable BLE applications. This guide covers connection lifecycle management, handling disconnections, and implementing robust connection strategies.

## Table of Contents

- [Connection Lifecycle](#connection-lifecycle)
- [MaxConcurrentConnections](#maxconcurrentconnections)
- [Handling Unexpected Disconnections](#handling-unexpected-disconnections)
- [Connection State Monitoring](#connection-state-monitoring)
- [Auto-Reconnection Patterns](#auto-reconnection-patterns)
- [Connection Timeouts](#connection-timeouts)
- [Platform-Specific Considerations](#platform-specific-considerations)
- [Advanced Patterns](#advanced-patterns)

## Connection Lifecycle

### Basic Connection Pattern

```csharp
public class DeviceManager
{
    private readonly IBluetoothScanner _scanner;
    private IBluetoothRemoteDevice? _connectedDevice;

    public DeviceManager(IBluetoothScanner scanner)
    {
        _scanner = scanner;
    }

    public async Task ConnectToDeviceAsync(
        string deviceName,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // 1. Start scanning
            await _scanner.StartScanningAsync(cancellationToken: cancellationToken);

            // 2. Find target device
            var device = await FindDeviceAsync(deviceName, cancellationToken);

            // 3. Stop scanning to save battery
            await _scanner.StopScanningAsync();

            // 4. Connect with options
            var connectionOptions = new ConnectionOptions
            {
                ConnectionRetry = RetryOptions.Default,
                WaitForAdvertisementBeforeConnecting = false
            };

            await device.ConnectAsync(
                connectionOptions,
                timeout: TimeSpan.FromSeconds(30),
                cancellationToken: cancellationToken);

            _connectedDevice = device;

            // 5. Subscribe to disconnection events BEFORE any operations
            SetupConnectionEventHandlers(device);

            // 6. Discover services
            await device.ExploreServicesAsync(
                ServiceExplorationOptions.WithCharacteristics,
                cancellationToken: cancellationToken);
        }
        catch (TimeoutException ex)
        {
            // Connection timed out
            throw new BluetoothException("Failed to connect: timeout", ex);
        }
        catch (OperationCanceledException)
        {
            // User cancelled operation
            await _scanner.StopScanningIfNeededAsync();
            throw;
        }
    }

    private void SetupConnectionEventHandlers(IBluetoothRemoteDevice device)
    {
        device.Connected += OnDeviceConnected;
        device.Disconnected += OnDeviceDisconnected;
        device.UnexpectedDisconnection += OnUnexpectedDisconnection;
        device.ConnectionStateChanged += OnConnectionStateChanged;
    }
}
```

### Proper Disconnection

```csharp
public async Task DisconnectDeviceAsync()
{
    if (_connectedDevice == null)
        return;

    try
    {
        // 1. Stop any active notifications
        foreach (var service in _connectedDevice.Services)
        {
            foreach (var characteristic in service.Characteristics)
            {
                if (characteristic.IsListening)
                {
                    await characteristic.StopListeningAsync();
                }
            }
        }

        // 2. Clear services (stops notifications, clears cache)
        await _connectedDevice.ClearServicesAsync();

        // 3. Disconnect
        await _connectedDevice.DisconnectAsync(timeout: TimeSpan.FromSeconds(10));

        // 4. Dispose (implements IAsyncDisposable)
        await _connectedDevice.DisposeAsync();
    }
    catch (Exception ex)
    {
        // Log but don't throw - we're cleaning up
        _logger.LogError(ex, "Error during device cleanup");
    }
    finally
    {
        _connectedDevice = null;
    }
}
```

## MaxConcurrentConnections

The `MaxConcurrentConnections` setting limits simultaneous connection attempts to prevent resource exhaustion.

### Configuration

```csharp
// MauiProgram.cs
builder.Services.Configure<BluetoothInfrastructureOptions>(options =>
{
    // Default is 5 - reasonable for most applications
    options.MaxConcurrentConnections = 3;

    // Set to 0 for unlimited (NOT recommended)
    // options.MaxConcurrentConnections = 0;
});
```

### When to Adjust

**Reduce (1-3 connections):**
- Mobile devices with limited resources
- Applications connecting to a single device
- Battery-sensitive applications
- Simple sensor monitoring

**Increase (5-10 connections):**
- Desktop/tablet applications
- Gateway/hub applications
- Multi-device monitoring systems
- Applications requiring redundancy

**Never use unlimited (0):**
- Risks memory exhaustion
- Can overwhelm BLE stack
- Degrades connection quality
- Battery drain

### Managing Multiple Connections

```csharp
public class MultiDeviceManager
{
    private readonly IBluetoothScanner _scanner;
    private readonly SemaphoreSlim _connectionSemaphore;
    private readonly List<IBluetoothRemoteDevice> _connectedDevices = new();

    public MultiDeviceManager(
        IBluetoothScanner scanner,
        IOptions<BluetoothInfrastructureOptions> options)
    {
        _scanner = scanner;
        var maxConnections = options.Value.MaxConcurrentConnections;
        _connectionSemaphore = new SemaphoreSlim(maxConnections, maxConnections);
    }

    public async Task ConnectToMultipleDevicesAsync(
        IEnumerable<string> deviceNames,
        CancellationToken cancellationToken = default)
    {
        var connectionTasks = deviceNames.Select(name =>
            ConnectWithSemaphoreAsync(name, cancellationToken));

        // Wait for all connections
        var results = await Task.WhenAll(connectionTasks);

        _connectedDevices.AddRange(results.Where(d => d != null));
    }

    private async Task<IBluetoothRemoteDevice?> ConnectWithSemaphoreAsync(
        string deviceName,
        CancellationToken cancellationToken)
    {
        // Wait for connection slot
        await _connectionSemaphore.WaitAsync(cancellationToken);

        try
        {
            var device = await FindAndConnectDeviceAsync(deviceName, cancellationToken);
            return device;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to connect to {DeviceName}", deviceName);
            return null;
        }
        finally
        {
            // Release connection slot
            _connectionSemaphore.Release();
        }
    }

    public async Task DisconnectAllAsync()
    {
        var disconnectTasks = _connectedDevices
            .Select(async device =>
            {
                try
                {
                    await device.DisconnectAsync();
                    await device.DisposeAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error disconnecting device {DeviceId}", device.Id);
                }
            });

        await Task.WhenAll(disconnectTasks);
        _connectedDevices.Clear();
    }
}
```

## Handling Unexpected Disconnections

### Event-Based Handling

```csharp
public class RobustDeviceConnection
{
    private readonly IBluetoothRemoteDevice _device;
    private bool _isIntentionalDisconnection;
    private int _reconnectionAttempts;
    private const int MaxReconnectionAttempts = 3;

    public RobustDeviceConnection(IBluetoothRemoteDevice device)
    {
        _device = device;
        SetupEventHandlers();
    }

    private void SetupEventHandlers()
    {
        _device.UnexpectedDisconnection += OnUnexpectedDisconnection;
        _device.Disconnected += OnDisconnected;
        _device.Connected += OnConnected;
    }

    private async void OnUnexpectedDisconnection(
        object? sender,
        DeviceUnexpectedDisconnectionEventArgs e)
    {
        _logger.LogWarning(
            "Device {DeviceId} disconnected unexpectedly: {Error}",
            _device.Id,
            e.Exception?.Message);

        // Don't reconnect if this was intentional
        if (_isIntentionalDisconnection)
            return;

        // Attempt reconnection with exponential backoff
        await AttemptReconnectionAsync();
    }

    private void OnDisconnected(object? sender, EventArgs e)
    {
        _logger.LogInformation("Device {DeviceId} disconnected", _device.Id);
        DeviceDisconnected?.Invoke(this, EventArgs.Empty);
    }

    private void OnConnected(object? sender, EventArgs e)
    {
        _logger.LogInformation("Device {DeviceId} connected", _device.Id);
        _reconnectionAttempts = 0; // Reset counter on successful connection
        DeviceConnected?.Invoke(this, EventArgs.Empty);
    }

    public async Task DisconnectAsync()
    {
        _isIntentionalDisconnection = true;
        try
        {
            await _device.DisconnectAsync();
        }
        finally
        {
            _isIntentionalDisconnection = false;
        }
    }

    private async Task AttemptReconnectionAsync()
    {
        if (_reconnectionAttempts >= MaxReconnectionAttempts)
        {
            _logger.LogError(
                "Max reconnection attempts reached for device {DeviceId}",
                _device.Id);
            ReconnectionFailed?.Invoke(this, EventArgs.Empty);
            return;
        }

        _reconnectionAttempts++;

        // Exponential backoff: 1s, 2s, 4s
        var delayMs = (int)Math.Pow(2, _reconnectionAttempts - 1) * 1000;
        await Task.Delay(delayMs);

        try
        {
            _logger.LogInformation(
                "Attempting reconnection {Attempt}/{Max} to device {DeviceId}",
                _reconnectionAttempts,
                MaxReconnectionAttempts,
                _device.Id);

            var options = new ConnectionOptions
            {
                ConnectionRetry = RetryOptions.Aggressive
            };

            await _device.ConnectAsync(options, timeout: TimeSpan.FromSeconds(30));

            // Re-discover services after reconnection
            await _device.ExploreServicesAsync(ServiceExplorationOptions.WithCharacteristics);

            _logger.LogInformation("Reconnection successful for device {DeviceId}", _device.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Reconnection attempt {Attempt} failed for device {DeviceId}",
                _reconnectionAttempts,
                _device.Id);

            // Try again
            await AttemptReconnectionAsync();
        }
    }

    public event EventHandler? DeviceConnected;
    public event EventHandler? DeviceDisconnected;
    public event EventHandler? ReconnectionFailed;
}
```

### Ignoring Expected Disconnections

```csharp
public async Task DisconnectWithoutReconnectAsync(IBluetoothRemoteDevice device)
{
    // Tell the device to ignore the next unexpected disconnection
    device.IgnoreNextUnexpectedDisconnection = true;

    await device.DisconnectAsync();

    // The flag is automatically reset after the disconnection
}
```

## Connection State Monitoring

### State Machine Pattern

```csharp
public class ConnectionStateMachine
{
    private readonly IBluetoothRemoteDevice _device;

    public ConnectionStateMachine(IBluetoothRemoteDevice device)
    {
        _device = device;
        _device.ConnectionStateChanged += OnConnectionStateChanged;
    }

    private void OnConnectionStateChanged(
        object? sender,
        DeviceConnectionStateChangedEventArgs e)
    {
        _logger.LogInformation(
            "Connection state changed: {OldState} -> {NewState}",
            e.OldState,
            e.NewState);

        switch (e.NewState)
        {
            case DeviceConnectionState.Disconnected:
                HandleDisconnectedState();
                break;

            case DeviceConnectionState.Connecting:
                HandleConnectingState();
                break;

            case DeviceConnectionState.Connected:
                HandleConnectedState();
                break;

            case DeviceConnectionState.Disconnecting:
                HandleDisconnectingState();
                break;
        }
    }

    private void HandleConnectedState()
    {
        // Device is ready for operations
        IsReady = true;
        ReadyStateChanged?.Invoke(this, EventArgs.Empty);
    }

    private void HandleDisconnectedState()
    {
        // Device is not available
        IsReady = false;
        ReadyStateChanged?.Invoke(this, EventArgs.Empty);
    }

    public bool IsReady { get; private set; }
    public event EventHandler? ReadyStateChanged;
}
```

### Waiting for Connection State

```csharp
public async Task WaitForConnectionAsync(
    IBluetoothRemoteDevice device,
    CancellationToken cancellationToken = default)
{
    if (device.IsConnected)
        return;

    // Wait for device to connect (with timeout)
    await device.WaitForIsConnectedAsync(
        isConnected: true,
        timeout: TimeSpan.FromSeconds(30),
        cancellationToken: cancellationToken);

    _logger.LogInformation("Device {DeviceId} is now connected", device.Id);
}
```

## Auto-Reconnection Patterns

### Persistent Connection Manager

```csharp
public class PersistentConnectionManager : IDisposable
{
    private readonly IBluetoothScanner _scanner;
    private readonly string _targetDeviceName;
    private IBluetoothRemoteDevice? _device;
    private CancellationTokenSource? _reconnectionCts;
    private bool _shouldMaintainConnection = true;

    public PersistentConnectionManager(
        IBluetoothScanner scanner,
        string targetDeviceName)
    {
        _scanner = scanner;
        _targetDeviceName = targetDeviceName;
    }

    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        _shouldMaintainConnection = true;
        await ConnectWithAutoReconnectAsync(cancellationToken);
    }

    private async Task ConnectWithAutoReconnectAsync(CancellationToken cancellationToken)
    {
        while (_shouldMaintainConnection && !cancellationToken.IsCancellationRequested)
        {
            try
            {
                // Find and connect to device
                await _scanner.StartScanningAsync(cancellationToken: cancellationToken);
                _device = await FindDeviceAsync(_targetDeviceName, cancellationToken);
                await _scanner.StopScanningAsync();

                await _device.ConnectAsync(
                    new ConnectionOptions { ConnectionRetry = RetryOptions.Aggressive },
                    cancellationToken: cancellationToken);

                // Setup reconnection on disconnection
                _device.UnexpectedDisconnection += OnDeviceDisconnected;

                _logger.LogInformation("Connected to {DeviceName}", _targetDeviceName);

                // Notify listeners
                ConnectionEstablished?.Invoke(this, _device);

                // Wait until disconnected
                await _device.WaitForIsConnectedAsync(
                    isConnected: false,
                    timeout: null, // Wait indefinitely
                    cancellationToken: cancellationToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Connection error, retrying in 5 seconds...");
                await Task.Delay(5000, cancellationToken);
            }
        }
    }

    private async void OnDeviceDisconnected(
        object? sender,
        DeviceUnexpectedDisconnectionEventArgs e)
    {
        _logger.LogWarning("Device disconnected, attempting reconnection...");

        if (_device != null)
        {
            _device.UnexpectedDisconnection -= OnDeviceDisconnected;
        }

        // Trigger reconnection
        _reconnectionCts?.Cancel();
        _reconnectionCts = new CancellationTokenSource();

        await ConnectWithAutoReconnectAsync(_reconnectionCts.Token);
    }

    public async Task StopAsync()
    {
        _shouldMaintainConnection = false;
        _reconnectionCts?.Cancel();

        if (_device != null)
        {
            await _device.DisconnectAsync();
            await _device.DisposeAsync();
        }

        await _scanner.StopScanningIfNeededAsync();
    }

    public event EventHandler<IBluetoothRemoteDevice>? ConnectionEstablished;

    public void Dispose()
    {
        _reconnectionCts?.Dispose();
    }
}
```

## Connection Timeouts

### Adaptive Timeout Pattern

```csharp
public class AdaptiveConnectionManager
{
    private TimeSpan _currentTimeout = TimeSpan.FromSeconds(10);
    private int _consecutiveTimeouts;
    private const int MaxTimeoutSeconds = 60;

    public async Task<bool> ConnectWithAdaptiveTimeoutAsync(
        IBluetoothRemoteDevice device,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await device.ConnectAsync(
                connectionOptions: new ConnectionOptions
                {
                    ConnectionRetry = RetryOptions.Default
                },
                timeout: _currentTimeout,
                cancellationToken: cancellationToken);

            // Success - reset timeout
            _currentTimeout = TimeSpan.FromSeconds(10);
            _consecutiveTimeouts = 0;
            return true;
        }
        catch (TimeoutException)
        {
            _consecutiveTimeouts++;

            // Increase timeout for next attempt (up to maximum)
            var newTimeoutSeconds = Math.Min(
                _currentTimeout.TotalSeconds * 1.5,
                MaxTimeoutSeconds);

            _currentTimeout = TimeSpan.FromSeconds(newTimeoutSeconds);

            _logger.LogWarning(
                "Connection timeout #{Count}, increasing timeout to {Timeout}s",
                _consecutiveTimeouts,
                newTimeoutSeconds);

            return false;
        }
    }
}
```

### Operation-Specific Timeouts

```csharp
public class TimeoutConfiguration
{
    public static readonly Dictionary<string, TimeSpan> OperationTimeouts = new()
    {
        ["Scan"] = TimeSpan.FromSeconds(30),
        ["Connect"] = TimeSpan.FromSeconds(30),
        ["Disconnect"] = TimeSpan.FromSeconds(10),
        ["ServiceDiscovery"] = TimeSpan.FromSeconds(15),
        ["CharacteristicRead"] = TimeSpan.FromSeconds(5),
        ["CharacteristicWrite"] = TimeSpan.FromSeconds(5),
        ["StartNotifications"] = TimeSpan.FromSeconds(5),
        ["MtuNegotiation"] = TimeSpan.FromSeconds(3),
    };

    public static TimeSpan GetTimeout(string operation)
    {
        return OperationTimeouts.TryGetValue(operation, out var timeout)
            ? timeout
            : TimeSpan.FromSeconds(30); // Default
    }
}
```

## Platform-Specific Considerations

### Android: GATT Error 133

```csharp
public async Task ConnectWithGattError133HandlingAsync(
    IBluetoothRemoteDevice device,
    CancellationToken cancellationToken = default)
{
    // GATT Error 133 is the most common Android BLE error
    // It's a transient error that usually resolves with retry

    var options = new ConnectionOptions
    {
        ConnectionRetry = new RetryOptions
        {
            MaxRetries = 5,
            DelayBetweenRetries = TimeSpan.FromMilliseconds(300),
            ExponentialBackoff = true
        },
        Android = new AndroidConnectionOptions
        {
            AutoConnect = false, // Direct connection is usually faster
            TransportType = BluetoothTransportType.Le
        }
    };

    try
    {
        await device.ConnectAsync(options, cancellationToken: cancellationToken);
    }
    catch (BluetoothException ex)
    {
        _logger.LogError(ex, "Connection failed after retries (GATT Error 133?)");

        // Last resort: Wait longer and try once more
        await Task.Delay(2000, cancellationToken);

        options = options with
        {
            ConnectionRetry = RetryOptions.None,
            Android = new AndroidConnectionOptions
            {
                AutoConnect = true // Try autoConnect as fallback
            }
        };

        await device.ConnectAsync(options, cancellationToken: cancellationToken);
    }
}
```

### iOS: Background Connections

```csharp
public async Task ConnectInBackgroundAsync(
    IBluetoothRemoteDevice device,
    CancellationToken cancellationToken = default)
{
    var options = new ConnectionOptions
    {
        Apple = new AppleConnectionOptions
        {
            // Show alerts when app is in background
            NotifyOnConnection = true,
            NotifyOnDisconnection = true,
            NotifyOnNotification = true
        }
    };

    await device.ConnectAsync(options, cancellationToken: cancellationToken);
}
```

## Advanced Patterns

### Connection Pool

```csharp
public class DeviceConnectionPool
{
    private readonly ConcurrentDictionary<Guid, IBluetoothRemoteDevice> _connections = new();
    private readonly SemaphoreSlim _poolSemaphore;
    private readonly int _maxPoolSize;

    public DeviceConnectionPool(int maxPoolSize = 5)
    {
        _maxPoolSize = maxPoolSize;
        _poolSemaphore = new SemaphoreSlim(maxPoolSize, maxPoolSize);
    }

    public async Task<IBluetoothRemoteDevice> GetOrCreateConnectionAsync(
        Guid deviceId,
        Func<Task<IBluetoothRemoteDevice>> connectionFactory,
        CancellationToken cancellationToken = default)
    {
        // Return existing connection if available and connected
        if (_connections.TryGetValue(deviceId, out var existingDevice) &&
            existingDevice.IsConnected)
        {
            return existingDevice;
        }

        // Wait for pool slot
        await _poolSemaphore.WaitAsync(cancellationToken);

        try
        {
            // Double-check after acquiring semaphore
            if (_connections.TryGetValue(deviceId, out existingDevice) &&
                existingDevice.IsConnected)
            {
                _poolSemaphore.Release();
                return existingDevice;
            }

            // Create new connection
            var device = await connectionFactory();
            _connections[deviceId] = device;

            // Setup cleanup on disconnection
            device.Disconnected += (s, e) =>
            {
                _connections.TryRemove(deviceId, out _);
                _poolSemaphore.Release();
            };

            return device;
        }
        catch
        {
            _poolSemaphore.Release();
            throw;
        }
    }

    public async Task DisconnectAllAsync()
    {
        var disconnectTasks = _connections.Values.Select(device =>
        {
            try
            {
                return device.DisconnectAsync().AsTask();
            }
            catch
            {
                return Task.CompletedTask;
            }
        });

        await Task.WhenAll(disconnectTasks);
        _connections.Clear();
    }
}
```

## Summary

### Key Takeaways

1. **Always handle unexpected disconnections** - Subscribe to `UnexpectedDisconnection` event
2. **Configure MaxConcurrentConnections appropriately** - Default of 5 works for most apps
3. **Use RetryOptions for transient failures** - Especially important on Android
4. **Stop scanning after device found** - Saves battery
5. **Clean up resources properly** - Use `DisposeAsync()` and clear services
6. **Monitor connection state** - Use events and state properties
7. **Implement reconnection logic** - Essential for production apps
8. **Set appropriate timeouts** - Different operations need different timeouts
9. **Test on target platforms** - Each platform has unique characteristics
10. **Log connection events** - Critical for debugging production issues

### Related Topics

- [Battery Optimization](Battery-Optimization.md) - Optimize connection parameters for battery life
- [Error Handling](Error-Handling.md) - Handle connection errors and exceptions
- [Performance](Performance.md) - Optimize connection throughput
- [MVVM Integration](MVVM-Integration.md) - Integrate connections with ViewModels
