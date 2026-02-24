# Error Handling Best Practices

Robust error handling is essential for reliable BLE applications. This guide covers exception management, retry strategies, and recovery patterns for Plugin.Bluetooth.

## Table of Contents

- [Exception Hierarchy](#exception-hierarchy)
- [Try-Catch Patterns](#try-catch-patterns)
- [RetryOptions Configuration](#retryoptions-configuration)
- [AggregateException Handling](#aggregateexception-handling)
- [Platform-Specific Errors](#platform-specific-errors)
- [Graceful Degradation](#graceful-degradation)
- [Error Logging](#error-logging)
- [Recovery Strategies](#recovery-strategies)

## Exception Hierarchy

Understanding the exception hierarchy helps you catch and handle specific errors appropriately.

### Exception Types

```
Exception
└── BluetoothException (base for all BLE exceptions)
    ├── BluetoothPermissionException
    ├── DeviceNotFoundException
    ├── DeviceNotConnectedException
    ├── ServiceNotFoundException
    ├── CharacteristicNotFoundException
    ├── DescriptorNotFoundException
    ├── ScannerException
    │   ├── ScannerIsAlreadyStartedException
    │   ├── ScannerIsAlreadyStoppedException
    │   ├── ScannerFailedToStartException
    │   ├── ScannerFailedToStopException
    │   └── ScannerUnexpectedStartException
    ├── BroadcasterException
    ├── TimeoutException
    └── OperationCanceledException
```

### Exception Properties

```csharp
public class ExceptionInspector
{
    public void InspectException(Exception ex)
    {
        if (ex is BluetoothException bleEx)
        {
            _logger.LogError(
                bleEx,
                "Bluetooth error: {Message}",
                bleEx.Message);

            // Check for platform-specific details in InnerException
            if (bleEx.InnerException != null)
            {
                _logger.LogDebug(
                    "Inner exception: {Type} - {Message}",
                    bleEx.InnerException.GetType().Name,
                    bleEx.InnerException.Message);

                // Android: BluetoothException, GattException
                // iOS: NSException, NSError
                // Windows: COMException
            }
        }
    }
}
```

## Try-Catch Patterns

### Basic Pattern

```csharp
public class BasicErrorHandling
{
    private readonly IBluetoothRemoteDevice _device;

    public async Task<byte[]?> ReadCharacteristicSafelyAsync(
        IBluetoothRemoteCharacteristic characteristic,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var value = await characteristic.ReadValueAsync(
                timeout: TimeSpan.FromSeconds(5),
                cancellationToken: cancellationToken);

            return value.ToArray();
        }
        catch (DeviceNotConnectedException ex)
        {
            // Device is not connected - specific recovery action
            _logger.LogWarning(ex, "Device not connected during read");
            await AttemptReconnectionAsync();
            return null;
        }
        catch (TimeoutException ex)
        {
            // Operation timed out - log and return null
            _logger.LogWarning(ex, "Read operation timed out");
            return null;
        }
        catch (OperationCanceledException)
        {
            // User cancelled - don't log as error
            _logger.LogInformation("Read operation cancelled by user");
            return null;
        }
        catch (BluetoothException ex)
        {
            // Generic BLE error - log and potentially retry
            _logger.LogError(ex, "Bluetooth error during read");
            throw; // Re-throw if can't recover
        }
    }
}
```

### Specific Exception Handling

```csharp
public class SpecificExceptionHandling
{
    public async Task<IBluetoothRemoteDevice> ConnectWithDetailedErrorHandlingAsync(
        IBluetoothScanner scanner,
        string deviceName,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await scanner.StartScanningAsync(cancellationToken: cancellationToken);

            var device = await WaitForDeviceAsync(deviceName, cancellationToken);

            if (device == null)
                throw new DeviceNotFoundException($"Device '{deviceName}' not found");

            await scanner.StopScanningAsync();

            await device.ConnectAsync(cancellationToken: cancellationToken);

            return device;
        }
        catch (BluetoothPermissionException ex)
        {
            // Permission denied - show user guidance
            _logger.LogError(ex, "Bluetooth permissions not granted");
            await ShowPermissionDialogAsync();
            throw;
        }
        catch (ScannerFailedToStartException ex)
        {
            // Scanner hardware issue or adapter disabled
            _logger.LogError(ex, "Failed to start scanner");
            await ShowBluetoothDisabledDialogAsync();
            throw;
        }
        catch (DeviceNotFoundException ex)
        {
            // Device not found - specific user message
            _logger.LogWarning(ex, "Device not found: {DeviceName}", deviceName);
            throw new BluetoothException(
                $"Could not find device '{deviceName}'. Make sure it's powered on and nearby.",
                ex);
        }
        catch (TimeoutException ex)
        {
            // Connection timeout - suggest retry
            _logger.LogWarning(ex, "Connection timeout for device {DeviceName}", deviceName);
            throw new BluetoothException(
                $"Connection to '{deviceName}' timed out. Please try again.",
                ex);
        }
        catch (OperationCanceledException)
        {
            // User cancelled - clean up and don't show error
            await scanner.StopScanningIfNeededAsync();
            throw;
        }
    }
}
```

### Exception Filtering with When Clause

```csharp
public class ExceptionFiltering
{
    public async Task<byte[]> ReadWithRetryAsync(
        IBluetoothRemoteCharacteristic characteristic,
        int maxRetries = 3)
    {
        var attempt = 0;

        while (true)
        {
            try
            {
                return (await characteristic.ReadValueAsync()).ToArray();
            }
            catch (TimeoutException) when (attempt < maxRetries)
            {
                // Only retry on timeout, and only up to maxRetries
                attempt++;
                _logger.LogWarning(
                    "Read timeout, retry {Attempt}/{Max}",
                    attempt,
                    maxRetries);

                await Task.Delay(200 * attempt); // Exponential backoff
            }
            catch (DeviceNotConnectedException) when (attempt == 0)
            {
                // Try to reconnect once
                _logger.LogWarning("Device disconnected, attempting reconnection");
                await ReconnectDeviceAsync();
                attempt++;
            }
            catch (Exception ex) when (attempt >= maxRetries)
            {
                // Max retries reached - wrap and throw
                throw new BluetoothException(
                    $"Failed to read characteristic after {maxRetries} attempts",
                    ex);
            }
        }
    }
}
```

## RetryOptions Configuration

### Retry Profiles

```csharp
public static class RetryProfiles
{
    // Default: 3 retries, 200ms delay, no exponential backoff
    public static RetryOptions Default => RetryOptions.Default;

    // No retry: Single attempt only
    public static RetryOptions NoRetry => RetryOptions.None;

    // Aggressive: 5 retries with exponential backoff
    public static RetryOptions Aggressive => RetryOptions.Aggressive;

    // Quick: Fast retries for time-sensitive operations
    public static RetryOptions Quick => new()
    {
        MaxRetries = 2,
        DelayBetweenRetries = TimeSpan.FromMilliseconds(100),
        ExponentialBackoff = false
    };

    // Patient: Slow retries for unreliable connections
    public static RetryOptions Patient => new()
    {
        MaxRetries = 5,
        DelayBetweenRetries = TimeSpan.FromMilliseconds(500),
        ExponentialBackoff = true
    };

    // Persistent: Many retries for critical operations
    public static RetryOptions Persistent => new()
    {
        MaxRetries = 10,
        DelayBetweenRetries = TimeSpan.FromMilliseconds(300),
        ExponentialBackoff = true
    };
}
```

### Configuring Retry Behavior

```csharp
public class RetryConfiguration
{
    public async Task ConnectWithCustomRetryAsync(
        IBluetoothRemoteDevice device,
        OperationPriority priority)
    {
        var retryOptions = priority switch
        {
            OperationPriority.Critical => RetryProfiles.Persistent,
            OperationPriority.High => RetryProfiles.Aggressive,
            OperationPriority.Normal => RetryProfiles.Default,
            OperationPriority.Low => RetryProfiles.Quick,
            _ => RetryProfiles.NoRetry
        };

        var connectionOptions = new ConnectionOptions
        {
            ConnectionRetry = retryOptions
        };

        await device.ConnectAsync(connectionOptions);
    }
}

public enum OperationPriority
{
    Critical,
    High,
    Normal,
    Low,
    BestEffort
}
```

### Platform-Specific Retry Configuration

```csharp
public class PlatformSpecificRetry
{
    public ConnectionOptions GetConnectionOptionsForPlatform()
    {
        var baseOptions = new ConnectionOptions
        {
            ConnectionRetry = RetryOptions.Default
        };

        if (DeviceInfo.Platform == DevicePlatform.Android)
        {
            // Android needs aggressive retry for GATT Error 133
            return baseOptions with
            {
                ConnectionRetry = new RetryOptions
                {
                    MaxRetries = 5,
                    DelayBetweenRetries = TimeSpan.FromMilliseconds(300),
                    ExponentialBackoff = true
                },
                Android = new AndroidConnectionOptions
                {
                    // Android-specific retry for GATT operations
                    GattWriteRetry = RetryProfiles.Aggressive,
                    GattReadRetry = RetryProfiles.Default,
                    ServiceDiscoveryRetry = new RetryOptions
                    {
                        MaxRetries = 3,
                        DelayBetweenRetries = TimeSpan.FromMilliseconds(500)
                    }
                }
            };
        }

        return baseOptions;
    }
}
```

### Custom Retry Logic

```csharp
public class CustomRetryLogic
{
    public async Task<T> ExecuteWithRetryAsync<T>(
        Func<Task<T>> operation,
        RetryOptions options,
        CancellationToken cancellationToken = default)
    {
        var attempt = 0;
        Exception? lastException = null;

        while (attempt <= options.MaxRetries)
        {
            try
            {
                return await operation();
            }
            catch (OperationCanceledException)
            {
                // Don't retry if cancelled
                throw;
            }
            catch (Exception ex)
            {
                lastException = ex;
                attempt++;

                if (attempt > options.MaxRetries)
                    break;

                // Calculate delay with optional exponential backoff
                var delay = options.ExponentialBackoff
                    ? options.DelayBetweenRetries * Math.Pow(2, attempt - 1)
                    : options.DelayBetweenRetries;

                _logger.LogWarning(
                    ex,
                    "Operation failed, retry {Attempt}/{Max} after {Delay}ms",
                    attempt,
                    options.MaxRetries,
                    delay.TotalMilliseconds);

                await Task.Delay(delay, cancellationToken);
            }
        }

        // All retries exhausted
        throw new BluetoothException(
            $"Operation failed after {options.MaxRetries} retries",
            lastException);
    }
}
```

## AggregateException Handling

When connecting to multiple devices or performing parallel operations, you may encounter AggregateException.

### Handling Parallel Operations

```csharp
public class ParallelOperationErrorHandling
{
    public async Task<Dictionary<string, IBluetoothRemoteDevice>> ConnectToMultipleDevicesAsync(
        IEnumerable<string> deviceNames,
        CancellationToken cancellationToken = default)
    {
        var results = new Dictionary<string, IBluetoothRemoteDevice>();
        var errors = new List<Exception>();

        var tasks = deviceNames.Select(async name =>
        {
            try
            {
                var device = await ConnectToDeviceAsync(name, cancellationToken);
                return (name, device, error: (Exception?)null);
            }
            catch (Exception ex)
            {
                return (name, device: (IBluetoothRemoteDevice?)null, error: ex);
            }
        });

        var outcomes = await Task.WhenAll(tasks);

        foreach (var (name, device, error) in outcomes)
        {
            if (error != null)
            {
                _logger.LogError(error, "Failed to connect to {DeviceName}", name);
                errors.Add(error);
            }
            else if (device != null)
            {
                results[name] = device;
            }
        }

        // If some connections succeeded, return partial results
        if (results.Any() && errors.Any())
        {
            _logger.LogWarning(
                "Connected to {Success}/{Total} devices",
                results.Count,
                deviceNames.Count());

            return results;
        }

        // If all failed, throw AggregateException
        if (!results.Any() && errors.Any())
        {
            throw new AggregateException(
                "Failed to connect to any device",
                errors);
        }

        return results;
    }
}
```

### Analyzing AggregateException

```csharp
public class AggregateExceptionAnalyzer
{
    public void AnalyzeErrors(AggregateException ex)
    {
        // Flatten nested AggregateExceptions
        var flattened = ex.Flatten();

        // Count exception types
        var exceptionGroups = flattened.InnerExceptions
            .GroupBy(e => e.GetType())
            .Select(g => new { Type = g.Key.Name, Count = g.Count() });

        foreach (var group in exceptionGroups)
        {
            _logger.LogError(
                "Exception type {Type} occurred {Count} times",
                group.Type,
                group.Count);
        }

        // Handle specific exceptions
        if (flattened.InnerExceptions.Any(e => e is TimeoutException))
        {
            _logger.LogWarning("Some operations timed out");
        }

        if (flattened.InnerExceptions.Any(e => e is DeviceNotConnectedException))
        {
            _logger.LogWarning("Some devices were not connected");
        }
    }
}
```

## Platform-Specific Errors

### Android GATT Errors

```csharp
public class AndroidErrorHandling
{
    public async Task HandleAndroidGattErrorsAsync(
        IBluetoothRemoteDevice device,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await device.ConnectAsync(cancellationToken: cancellationToken);
        }
        catch (BluetoothException ex)
        {
            // Check for GATT Error 133 in inner exception
            var innerMessage = ex.InnerException?.Message ?? "";

            if (innerMessage.Contains("133") || innerMessage.Contains("GATT_ERROR"))
            {
                _logger.LogWarning("Android GATT Error 133 detected, applying workaround");

                // Workaround: Wait longer and retry with different settings
                await Task.Delay(2000, cancellationToken);

                var options = new ConnectionOptions
                {
                    Android = new AndroidConnectionOptions
                    {
                        AutoConnect = true, // Try autoConnect mode
                        TransportType = BluetoothTransportType.Le
                    }
                };

                await device.ConnectAsync(options, cancellationToken: cancellationToken);
            }
            else
            {
                throw;
            }
        }
    }
}
```

### iOS/macOS Errors

```csharp
public class AppleErrorHandling
{
    public async Task HandleAppleCentralManagerErrorsAsync(
        IBluetoothScanner scanner,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await scanner.StartScanningAsync(cancellationToken: cancellationToken);
        }
        catch (ScannerFailedToStartException ex)
        {
            var innerMessage = ex.InnerException?.Message ?? "";

            if (innerMessage.Contains("powered off") || innerMessage.Contains("PoweredOff"))
            {
                _logger.LogError("Bluetooth is powered off");
                throw new BluetoothException(
                    "Bluetooth is turned off. Please enable Bluetooth in Settings.",
                    ex);
            }
            else if (innerMessage.Contains("unauthorized") || innerMessage.Contains("Unauthorized"))
            {
                _logger.LogError("Bluetooth permission denied");
                throw new BluetoothPermissionException(
                    "Bluetooth permission denied. Please grant permission in Settings.",
                    ex);
            }
            else if (innerMessage.Contains("unsupported"))
            {
                _logger.LogError("Bluetooth LE not supported");
                throw new BluetoothException(
                    "This device does not support Bluetooth Low Energy.",
                    ex);
            }
            else
            {
                throw;
            }
        }
    }
}
```

### Windows COM Errors

```csharp
public class WindowsErrorHandling
{
    public async Task HandleWindowsBluetoothErrorsAsync(
        IBluetoothScanner scanner,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await scanner.StartScanningAsync(cancellationToken: cancellationToken);
        }
        catch (BluetoothException ex) when (ex.InnerException is COMException comEx)
        {
            var hResult = comEx.HResult;

            // Common HRESULTs
            if (hResult == unchecked((int)0x80070015)) // ERROR_NOT_READY
            {
                _logger.LogError("Bluetooth adapter not ready");
                throw new BluetoothException(
                    "Bluetooth adapter is not ready. Please check if Bluetooth is enabled.",
                    ex);
            }
            else if (hResult == unchecked((int)0x80070490)) // ERROR_NOT_FOUND
            {
                _logger.LogError("Bluetooth adapter not found");
                throw new BluetoothException(
                    "No Bluetooth adapter found on this device.",
                    ex);
            }
            else
            {
                _logger.LogError("Windows Bluetooth error: HRESULT 0x{HResult:X}", hResult);
                throw;
            }
        }
    }
}
```

## Graceful Degradation

### Feature Detection

```csharp
public class GracefulDegradation
{
    private readonly IBluetoothScanner _scanner;

    public async Task<bool> TryEnableFeatureAsync(string featureName)
    {
        try
        {
            switch (featureName)
            {
                case "ExtendedAdvertising":
                    await TryExtendedAdvertisingAsync();
                    return true;

                case "PhySettings":
                    await TryPhySettingsAsync();
                    return true;

                case "L2Cap":
                    await TryL2CapAsync();
                    return true;

                default:
                    return false;
            }
        }
        catch (NotImplementedException)
        {
            _logger.LogInformation(
                "Feature {Feature} not supported on this platform",
                featureName);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(
                ex,
                "Failed to enable feature {Feature}",
                featureName);
            return false;
        }
    }

    private async Task TryExtendedAdvertisingAsync()
    {
        var options = new ScanningOptions
        {
            EnableExtendedAdvertising = true
        };

        await _scanner.StartScanningAsync(options);
    }
}
```

### Fallback Strategies

```csharp
public class FallbackStrategies
{
    public async Task<int> TryNegotiateMtuWithFallbackAsync(
        IBluetoothRemoteDevice device)
    {
        // Try to negotiate large MTU
        try
        {
            return await device.RequestMtuAsync(517);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to negotiate MTU, using default");
            return 23; // Default MTU
        }
    }

    public async Task WriteWithFallbackAsync(
        IBluetoothRemoteCharacteristic characteristic,
        byte[] data)
    {
        try
        {
            // Try write with response first (more reliable)
            await characteristic.WriteValueAsync(data);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(
                ex,
                "Write with response failed, retrying without response");

            // Fallback: Write without response (if supported)
            if (characteristic.CanWriteWithoutResponse)
            {
                // Implementation would use WriteWithoutResponse
                // await characteristic.WriteValueWithoutResponseAsync(data);
            }
            else
            {
                throw;
            }
        }
    }
}
```

## Error Logging

### Structured Logging

```csharp
public class StructuredErrorLogging
{
    private readonly ILogger<StructuredErrorLogging> _logger;

    public async Task ConnectWithDetailedLoggingAsync(
        IBluetoothRemoteDevice device,
        CancellationToken cancellationToken = default)
    {
        using var scope = _logger.BeginScope(new Dictionary<string, object>
        {
            ["DeviceId"] = device.Id,
            ["DeviceName"] = device.Name ?? "Unknown",
            ["Operation"] = "Connect"
        });

        try
        {
            _logger.LogInformation("Starting connection to device");

            await device.ConnectAsync(cancellationToken: cancellationToken);

            _logger.LogInformation("Successfully connected to device");
        }
        catch (TimeoutException ex)
        {
            _logger.LogError(
                ex,
                "Connection timeout after {Timeout}",
                TimeSpan.FromSeconds(30));
            throw;
        }
        catch (BluetoothException ex)
        {
            _logger.LogError(
                ex,
                "Bluetooth error during connection - Code: {ErrorCode}, Platform: {Platform}",
                ex.HResult,
                DeviceInfo.Platform);
            throw;
        }
    }
}
```

### Error Context Capture

```csharp
public class ErrorContextCapture
{
    public record ErrorContext
    {
        public string Operation { get; init; } = string.Empty;
        public Guid? DeviceId { get; init; }
        public string? DeviceName { get; init; }
        public int? SignalStrength { get; init; }
        public bool IsConnected { get; init; }
        public string Platform { get; init; } = string.Empty;
        public DateTime Timestamp { get; init; }
        public Dictionary<string, object> AdditionalData { get; init; } = new();
    }

    public ErrorContext CaptureContext(
        string operation,
        IBluetoothRemoteDevice? device = null,
        Exception? exception = null)
    {
        var context = new ErrorContext
        {
            Operation = operation,
            Platform = DeviceInfo.Platform.ToString(),
            Timestamp = DateTime.UtcNow
        };

        if (device != null)
        {
            context = context with
            {
                DeviceId = device.Id,
                DeviceName = device.Name,
                SignalStrength = device.SignalStrengthDbm,
                IsConnected = device.IsConnected
            };
        }

        if (exception != null)
        {
            context.AdditionalData["ExceptionType"] = exception.GetType().Name;
            context.AdditionalData["ExceptionMessage"] = exception.Message;
            context.AdditionalData["StackTrace"] = exception.StackTrace ?? "";

            if (exception.InnerException != null)
            {
                context.AdditionalData["InnerExceptionType"] =
                    exception.InnerException.GetType().Name;
                context.AdditionalData["InnerExceptionMessage"] =
                    exception.InnerException.Message;
            }
        }

        return context;
    }

    public void LogError(Exception ex, ErrorContext context)
    {
        _logger.LogError(
            ex,
            "BLE Error - Operation: {Operation}, Device: {DeviceId} ({DeviceName}), " +
            "Connected: {IsConnected}, RSSI: {RSSI}, Platform: {Platform}",
            context.Operation,
            context.DeviceId,
            context.DeviceName ?? "Unknown",
            context.IsConnected,
            context.SignalStrength,
            context.Platform);
    }
}
```

## Recovery Strategies

### Automatic Recovery

```csharp
public class AutoRecoveryManager
{
    private readonly IBluetoothRemoteDevice _device;
    private bool _isRecovering;

    public async Task<T> ExecuteWithAutoRecoveryAsync<T>(
        Func<Task<T>> operation,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await operation();
        }
        catch (DeviceNotConnectedException) when (!_isRecovering)
        {
            _isRecovering = true;

            try
            {
                _logger.LogWarning("Device not connected, attempting recovery");

                // Reconnect
                await _device.ConnectAsync(cancellationToken: cancellationToken);

                // Re-discover services
                await _device.ExploreServicesAsync(
                    ServiceExplorationOptions.WithCharacteristics,
                    cancellationToken: cancellationToken);

                _logger.LogInformation("Recovery successful, retrying operation");

                // Retry operation
                return await operation();
            }
            finally
            {
                _isRecovering = false;
            }
        }
    }
}
```

### Circuit Breaker Pattern

```csharp
public class CircuitBreaker
{
    private int _failureCount;
    private DateTime _lastFailureTime;
    private CircuitState _state = CircuitState.Closed;

    private const int FailureThreshold = 5;
    private readonly TimeSpan _openDuration = TimeSpan.FromMinutes(1);

    public async Task<T> ExecuteAsync<T>(Func<Task<T>> operation)
    {
        if (_state == CircuitState.Open)
        {
            if (DateTime.UtcNow - _lastFailureTime > _openDuration)
            {
                _logger.LogInformation("Circuit breaker entering half-open state");
                _state = CircuitState.HalfOpen;
            }
            else
            {
                throw new BluetoothException("Circuit breaker is open, operation rejected");
            }
        }

        try
        {
            var result = await operation();

            if (_state == CircuitState.HalfOpen)
            {
                _logger.LogInformation("Circuit breaker closed after successful operation");
                _state = CircuitState.Closed;
                _failureCount = 0;
            }

            return result;
        }
        catch (Exception ex)
        {
            _failureCount++;
            _lastFailureTime = DateTime.UtcNow;

            if (_failureCount >= FailureThreshold)
            {
                _logger.LogError(
                    "Circuit breaker opened after {Count} failures",
                    _failureCount);
                _state = CircuitState.Open;
            }

            throw;
        }
    }

    private enum CircuitState
    {
        Closed,   // Normal operation
        Open,     // Rejecting requests
        HalfOpen  // Testing if service recovered
    }
}
```

## Summary

### Error Handling Checklist

- [ ] Catch **specific exceptions** rather than generic Exception
- [ ] Use **RetryOptions** for transient failures
- [ ] Implement **exponential backoff** for retries
- [ ] Handle **platform-specific errors** (GATT 133, COM errors, etc.)
- [ ] Provide **meaningful error messages** to users
- [ ] Log errors with **sufficient context**
- [ ] Implement **graceful degradation** for unsupported features
- [ ] Use **circuit breaker** pattern for repeated failures
- [ ] Handle **OperationCanceledException** without logging as error
- [ ] Always **clean up resources** in finally blocks
- [ ] Test error scenarios thoroughly on all platforms

### Common Patterns Summary

```csharp
// Pattern 1: Specific exception handling
try
{
    await operation();
}
catch (DeviceNotConnectedException ex) { /* Reconnect */ }
catch (TimeoutException ex) { /* Retry */ }
catch (BluetoothException ex) { /* Log and rethrow */ }

// Pattern 2: Retry with options
var options = new ConnectionOptions
{
    ConnectionRetry = RetryOptions.Aggressive
};
await device.ConnectAsync(options);

// Pattern 3: Graceful degradation
try
{
    await TryAdvancedFeatureAsync();
}
catch (NotImplementedException)
{
    await FallbackToBasicFeatureAsync();
}

// Pattern 4: Context-rich logging
_logger.LogError(
    ex,
    "Operation {Op} failed for device {DeviceId} - {Message}",
    "Connect",
    device.Id,
    ex.Message);
```

### Related Topics

- [Connection Management](Connection-Management.md) - Handling connection errors
- [Performance](Performance.md) - Balancing retries with performance
- [Testing](Testing.md) - Testing error scenarios
