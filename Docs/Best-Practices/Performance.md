# Performance Best Practices

Optimizing BLE performance requires balancing throughput, latency, and reliability. This guide covers strategies to maximize data transfer rates and minimize operation times.

## Table of Contents

- [MTU Negotiation and Optimization](#mtu-negotiation-and-optimization)
- [Write Without Response](#write-without-response)
- [Service Caching](#service-caching)
- [L2CAP Channels](#l2cap-channels)
- [Connection Priority Tuning](#connection-priority-tuning)
- [Batch Operations](#batch-operations)
- [Parallel Operations](#parallel-operations)
- [Performance Monitoring](#performance-monitoring)

## MTU Negotiation and Optimization

MTU (Maximum Transmission Unit) determines the maximum packet size and directly affects throughput.

### Understanding MTU

```
Default MTU: 23 bytes
├── ATT Header: 3 bytes
└── Payload: 20 bytes (actual data)

Maximum MTU: 517 bytes (BLE spec)
├── ATT Header: 3 bytes
└── Payload: 514 bytes (25x more data per packet!)
```

### Optimal MTU Negotiation

```csharp
public class MtuOptimizer
{
    private const int DefaultMtu = 23;
    private const int MaxMtu = 517;

    public async Task<int> NegotiateOptimalMtuAsync(
        IBluetoothRemoteDevice device,
        int expectedDataSize,
        CancellationToken cancellationToken = default)
    {
        if (!device.IsConnected)
        {
            throw new InvalidOperationException("Device must be connected before MTU negotiation");
        }

        // Calculate optimal MTU based on data size
        var optimalMtu = CalculateOptimalMtu(expectedDataSize);

        try
        {
            var negotiatedMtu = await device.RequestMtuAsync(
                optimalMtu,
                timeout: TimeSpan.FromSeconds(3),
                cancellationToken: cancellationToken);

            _logger.LogInformation(
                "MTU negotiated - Requested: {Requested}, Negotiated: {Negotiated}, " +
                "Improvement: {Improvement:P0}",
                optimalMtu,
                negotiatedMtu,
                (negotiatedMtu - DefaultMtu) / (double)DefaultMtu);

            return negotiatedMtu;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "MTU negotiation failed, using default");
            return DefaultMtu;
        }
    }

    private int CalculateOptimalMtu(int dataSize)
    {
        // For small data, default is fine
        if (dataSize <= 20)
            return DefaultMtu;

        // For medium data, request moderate MTU
        if (dataSize <= 200)
            return 200;

        // For large data, request maximum
        if (dataSize <= 500)
            return MaxMtu;

        // For very large data, definitely need maximum
        return MaxMtu;
    }

    public int CalculatePayloadSize(int mtu)
    {
        // Subtract ATT header (3 bytes)
        return mtu - 3;
    }
}
```

### MTU-Aware Data Transfer

```csharp
public class MtuAwareTransfer
{
    private readonly IBluetoothRemoteDevice _device;
    private int _currentMtu = 23;

    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        // Negotiate MTU at connection time
        _currentMtu = await _device.RequestMtuAsync(517, cancellationToken: cancellationToken);

        // Subscribe to MTU changes
        _device.MtuChanged += OnMtuChanged;
    }

    private void OnMtuChanged(object? sender, MtuChangedEventArgs e)
    {
        _logger.LogInformation("MTU changed: {OldMtu} -> {NewMtu}", e.OldMtu, e.NewMtu);
        _currentMtu = e.NewMtu;
    }

    public async Task TransferDataAsync(
        IBluetoothRemoteCharacteristic characteristic,
        byte[] data,
        CancellationToken cancellationToken = default)
    {
        var payloadSize = _currentMtu - 3; // Subtract ATT header
        var totalChunks = (int)Math.Ceiling(data.Length / (double)payloadSize);

        _logger.LogInformation(
            "Transferring {Size} bytes in {Chunks} chunks (MTU: {Mtu})",
            data.Length,
            totalChunks,
            _currentMtu);

        var stopwatch = Stopwatch.StartNew();

        for (int i = 0; i < data.Length; i += payloadSize)
        {
            var chunkSize = Math.Min(payloadSize, data.Length - i);
            var chunk = data.AsMemory(i, chunkSize);

            await characteristic.WriteValueAsync(
                chunk,
                cancellationToken: cancellationToken);
        }

        stopwatch.Stop();

        var throughputKbps = (data.Length * 8) / stopwatch.Elapsed.TotalSeconds / 1000;

        _logger.LogInformation(
            "Transfer complete - Duration: {Duration}ms, Throughput: {Throughput:F2} Kbps",
            stopwatch.ElapsedMilliseconds,
            throughputKbps);
    }
}
```

### Platform-Specific MTU Handling

```csharp
public class PlatformMtuHandler
{
    public async Task<int> GetEffectiveMtuAsync(IBluetoothRemoteDevice device)
    {
        if (DeviceInfo.Platform == DevicePlatform.Android)
        {
            // Android: Explicit negotiation supported
            try
            {
                return await device.RequestMtuAsync(517);
            }
            catch
            {
                return 23; // Default
            }
        }
        else if (DeviceInfo.Platform == DevicePlatform.iOS ||
                 DeviceInfo.Platform == DevicePlatform.MacCatalyst)
        {
            // iOS/macOS: System negotiates automatically
            // RequestMtuAsync returns current system-negotiated value
            return await device.RequestMtuAsync(517); // Returns actual MTU
        }
        else if (DeviceInfo.Platform == DevicePlatform.WinUI)
        {
            // Windows: Automatic negotiation
            return await device.RequestMtuAsync(517); // Returns negotiated MTU
        }

        return 23; // Default fallback
    }
}
```

## Write Without Response

Write without response eliminates the round-trip acknowledgement, significantly increasing throughput.

### Write Without Response Pattern

```csharp
public class HighThroughputWriter
{
    public async Task WriteWithOptimalStrategyAsync(
        IBluetoothRemoteCharacteristic characteristic,
        byte[] data,
        CancellationToken cancellationToken = default)
    {
        if (characteristic.CanWriteWithoutResponse && data.Length > 60)
        {
            // Use write without response for large data
            await WriteWithoutResponseAsync(characteristic, data, cancellationToken);
        }
        else
        {
            // Use regular write for small data or if not supported
            await characteristic.WriteValueAsync(data, cancellationToken: cancellationToken);
        }
    }

    private async Task WriteWithoutResponseAsync(
        IBluetoothRemoteCharacteristic characteristic,
        byte[] data,
        CancellationToken cancellationToken)
    {
        var device = characteristic.RemoteService.RemoteDevice;
        var mtu = device.Mtu;
        var payloadSize = mtu - 3;

        var stopwatch = Stopwatch.StartNew();
        var totalChunks = (int)Math.Ceiling(data.Length / (double)payloadSize);

        _logger.LogInformation(
            "Writing {Size} bytes without response ({Chunks} chunks, MTU: {Mtu})",
            data.Length,
            totalChunks,
            mtu);

        for (int i = 0; i < data.Length; i += payloadSize)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var chunkSize = Math.Min(payloadSize, data.Length - i);
            var chunk = data.AsMemory(i, chunkSize);

            // Write without waiting for response
            await characteristic.WriteValueAsync(
                chunk,
                skipIfOldValueMatchesNewValue: false,
                cancellationToken: cancellationToken);

            // Optional: Small delay to prevent overwhelming the device
            if (i + payloadSize < data.Length)
            {
                await Task.Delay(1, cancellationToken); // Yield to other operations
            }
        }

        stopwatch.Stop();

        var throughputKbps = (data.Length * 8) / stopwatch.Elapsed.TotalSeconds / 1000;

        _logger.LogInformation(
            "Write without response complete - Duration: {Duration}ms, Throughput: {Throughput:F2} Kbps",
            stopwatch.ElapsedMilliseconds,
            throughputKbps);
    }
}
```

### Throughput Comparison

```csharp
public class ThroughputBenchmark
{
    public async Task<BenchmarkResult> BenchmarkWriteMethodsAsync(
        IBluetoothRemoteCharacteristic characteristic,
        int dataSize = 10000)
    {
        var testData = GenerateTestData(dataSize);

        // Benchmark Write with Response
        var writeWithResponseTime = await BenchmarkWriteAsync(
            characteristic,
            testData,
            useWithoutResponse: false);

        await Task.Delay(1000); // Cool down

        // Benchmark Write without Response
        var writeWithoutResponseTime = await BenchmarkWriteAsync(
            characteristic,
            testData,
            useWithoutResponse: true);

        return new BenchmarkResult
        {
            DataSize = dataSize,
            WriteWithResponseDuration = writeWithResponseTime,
            WriteWithoutResponseDuration = writeWithoutResponseTime,
            SpeedupFactor = writeWithResponseTime.TotalMilliseconds /
                          writeWithoutResponseTime.TotalMilliseconds
        };
    }

    private async Task<TimeSpan> BenchmarkWriteAsync(
        IBluetoothRemoteCharacteristic characteristic,
        byte[] data,
        bool useWithoutResponse)
    {
        var stopwatch = Stopwatch.StartNew();

        if (useWithoutResponse && characteristic.CanWriteWithoutResponse)
        {
            // Implementation for write without response
            await WriteWithoutResponseAsync(characteristic, data);
        }
        else
        {
            await characteristic.WriteValueAsync(data);
        }

        stopwatch.Stop();
        return stopwatch.Elapsed;
    }

    private byte[] GenerateTestData(int size)
    {
        var data = new byte[size];
        Random.Shared.NextBytes(data);
        return data;
    }
}

public record BenchmarkResult
{
    public int DataSize { get; init; }
    public TimeSpan WriteWithResponseDuration { get; init; }
    public TimeSpan WriteWithoutResponseDuration { get; init; }
    public double SpeedupFactor { get; init; }
}
```

## Service Caching

Cache discovered services to eliminate redundant discovery operations.

### Smart Caching Strategy

```csharp
public class ServiceCacheManager
{
    private readonly IBluetoothRemoteDevice _device;
    private Dictionary<Guid, IBluetoothRemoteService>? _cachedServices;
    private DateTime? _cacheTimestamp;
    private readonly TimeSpan _cacheValidity = TimeSpan.FromMinutes(30);

    public async Task<IBluetoothRemoteService> GetServiceAsync(
        Guid serviceUuid,
        CancellationToken cancellationToken = default)
    {
        // Check if cache is valid
        if (IsCacheValid())
        {
            if (_cachedServices?.TryGetValue(serviceUuid, out var cachedService) == true)
            {
                _logger.LogDebug("Returning cached service {ServiceId}", serviceUuid);
                return cachedService;
            }
        }

        // Cache miss or expired - explore services
        await RefreshCacheAsync(cancellationToken);

        return _cachedServices?[serviceUuid]
            ?? throw new ServiceNotFoundException(serviceUuid);
    }

    public async Task<IReadOnlyList<IBluetoothRemoteService>> GetAllServicesAsync(
        CancellationToken cancellationToken = default)
    {
        if (!IsCacheValid())
        {
            await RefreshCacheAsync(cancellationToken);
        }

        return _cachedServices?.Values.ToList() ?? new List<IBluetoothRemoteService>();
    }

    private async Task RefreshCacheAsync(CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();

        // Explore with cache enabled for faster subsequent calls
        await _device.ExploreServicesAsync(
            ServiceExplorationOptions.WithCharacteristics,
            cancellationToken: cancellationToken);

        _cachedServices = _device.Services.ToDictionary(s => s.Id);
        _cacheTimestamp = DateTime.UtcNow;

        stopwatch.Stop();

        _logger.LogInformation(
            "Service cache refreshed - {Count} services in {Duration}ms",
            _cachedServices.Count,
            stopwatch.ElapsedMilliseconds);
    }

    private bool IsCacheValid()
    {
        if (_cacheTimestamp == null || _cachedServices == null)
            return false;

        var age = DateTime.UtcNow - _cacheTimestamp.Value;
        return age < _cacheValidity;
    }

    public void InvalidateCache()
    {
        _cachedServices = null;
        _cacheTimestamp = null;
        _logger.LogDebug("Service cache invalidated");
    }

    public void OnDeviceDisconnected()
    {
        // Invalidate cache on disconnection
        InvalidateCache();
    }
}
```

### Lazy Loading with Caching

```csharp
public class LazyServiceLoader
{
    private readonly IBluetoothRemoteDevice _device;
    private readonly Dictionary<Guid, Lazy<Task<IBluetoothRemoteService>>> _lazyServices = new();

    public Task<IBluetoothRemoteService> GetServiceAsync(
        Guid serviceUuid,
        CancellationToken cancellationToken = default)
    {
        if (!_lazyServices.TryGetValue(serviceUuid, out var lazyService))
        {
            lazyService = new Lazy<Task<IBluetoothRemoteService>>(
                () => LoadServiceAsync(serviceUuid, cancellationToken));

            _lazyServices[serviceUuid] = lazyService;
        }

        return lazyService.Value;
    }

    private async Task<IBluetoothRemoteService> LoadServiceAsync(
        Guid serviceUuid,
        CancellationToken cancellationToken)
    {
        // Explore services with cache
        await _device.ExploreServicesAsync(
            new ServiceExplorationOptions
            {
                UseCache = true,
                ServiceUuidFilter = uuid => uuid == serviceUuid,
                IncludeCharacteristics = true
            },
            cancellationToken: cancellationToken);

        return _device.GetService(serviceUuid)
            ?? throw new ServiceNotFoundException(serviceUuid);
    }
}
```

## L2CAP Channels

For maximum throughput, use L2CAP channels for raw data transfer.

### L2CAP vs GATT Comparison

```
GATT:
- MTU limited (typically 23-517 bytes)
- Request/response overhead
- Characteristic-based
- ~100-200 Kbps typical

L2CAP:
- MTU up to 65,535 bytes
- Stream-based, minimal overhead
- Direct socket-like interface
- ~500-1000+ Kbps possible
```

### L2CAP High-Performance Transfer

```csharp
public class L2CapHighThroughput
{
    public async Task TransferLargeDataViaL2CapAsync(
        IBluetoothRemoteDevice device,
        byte[] data,
        CancellationToken cancellationToken = default)
    {
        const int L2CapPsm = 0x0025; // Your L2CAP PSM

        try
        {
            // Open L2CAP channel
            var channel = await device.OpenL2CapChannelAsync(
                L2CapPsm,
                new L2CapChannelOptions
                {
                    OpenTimeout = TimeSpan.FromSeconds(10),
                    WriteTimeout = TimeSpan.FromSeconds(30)
                },
                cancellationToken: cancellationToken);

            var stopwatch = Stopwatch.StartNew();

            _logger.LogInformation(
                "L2CAP channel opened - MTU: {Mtu}, transferring {Size} bytes",
                channel.Mtu,
                data.Length);

            // Write data in large chunks
            var chunkSize = channel.Mtu;
            for (int i = 0; i < data.Length; i += chunkSize)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var size = Math.Min(chunkSize, data.Length - i);
                var chunk = data.AsMemory(i, size);

                await channel.WriteAsync(chunk, cancellationToken);
            }

            stopwatch.Stop();

            var throughputKbps = (data.Length * 8) / stopwatch.Elapsed.TotalSeconds / 1000;

            _logger.LogInformation(
                "L2CAP transfer complete - Duration: {Duration}ms, Throughput: {Throughput:F2} Kbps",
                stopwatch.ElapsedMilliseconds,
                throughputKbps);

            await channel.CloseAsync(cancellationToken);
        }
        catch (NotImplementedException)
        {
            _logger.LogWarning("L2CAP not supported on this platform, falling back to GATT");
            await TransferViaGattAsync(device, data, cancellationToken);
        }
    }

    private async Task TransferViaGattAsync(
        IBluetoothRemoteDevice device,
        byte[] data,
        CancellationToken cancellationToken)
    {
        // Fallback to GATT-based transfer
        var service = await device.GetServiceAsync(MyServiceUuid);
        var characteristic = service.GetCharacteristic(MyCharacteristicUuid);
        await characteristic.WriteValueAsync(data, cancellationToken: cancellationToken);
    }
}
```

## Connection Priority Tuning

Optimize connection parameters for your use case.

### Dynamic Priority Management

```csharp
public class DynamicConnectionPriority
{
    private readonly IBluetoothRemoteDevice _device;
    private BluetoothConnectionPriority _currentPriority = BluetoothConnectionPriority.Balanced;

    public async Task OptimizeForOperationAsync(OperationType operation)
    {
        var optimalPriority = operation switch
        {
            OperationType.BulkDataTransfer => BluetoothConnectionPriority.High,
            OperationType.RealtimeStreaming => BluetoothConnectionPriority.High,
            OperationType.PeriodicSensorReading => BluetoothConnectionPriority.LowPower,
            OperationType.OccasionalCommand => BluetoothConnectionPriority.Balanced,
            _ => BluetoothConnectionPriority.Balanced
        };

        if (_currentPriority != optimalPriority)
        {
            await SetPriorityAsync(optimalPriority);
        }
    }

    private async Task SetPriorityAsync(BluetoothConnectionPriority priority)
    {
        try
        {
            await _device.RequestConnectionPriorityAsync(priority);
            _currentPriority = priority;

            _logger.LogInformation(
                "Connection priority set to {Priority} - " +
                "Expected latency: {Latency}",
                priority,
                GetExpectedLatency(priority));
        }
        catch (Exception ex)
        {
            _logger.LogDebug(ex, "Connection priority not supported");
        }
    }

    private string GetExpectedLatency(BluetoothConnectionPriority priority)
    {
        return priority switch
        {
            BluetoothConnectionPriority.High => "11.25-15ms",
            BluetoothConnectionPriority.Balanced => "30-50ms",
            BluetoothConnectionPriority.LowPower => "100-125ms",
            _ => "Unknown"
        };
    }
}

public enum OperationType
{
    BulkDataTransfer,
    RealtimeStreaming,
    PeriodicSensorReading,
    OccasionalCommand
}
```

## Batch Operations

Group operations to reduce overhead.

### Batched Writes

```csharp
public class BatchWriter
{
    private readonly Queue<WriteOperation> _writeQueue = new();
    private readonly SemaphoreSlim _flushSemaphore = new SemaphoreSlim(1, 1);
    private readonly TimeSpan _batchWindow = TimeSpan.FromMilliseconds(50);

    public async Task QueueWriteAsync(
        IBluetoothRemoteCharacteristic characteristic,
        byte[] data)
    {
        _writeQueue.Enqueue(new WriteOperation
        {
            Characteristic = characteristic,
            Data = data,
            Timestamp = DateTime.UtcNow
        });

        // Trigger flush if queue is getting large
        if (_writeQueue.Count >= 10)
        {
            await FlushAsync();
        }
    }

    public async Task FlushAsync()
    {
        await _flushSemaphore.WaitAsync();

        try
        {
            if (_writeQueue.Count == 0)
                return;

            var stopwatch = Stopwatch.StartNew();
            var operations = new List<WriteOperation>();

            // Drain queue
            while (_writeQueue.TryDequeue(out var op))
            {
                operations.Add(op);
            }

            // Group by characteristic
            var grouped = operations.GroupBy(op => op.Characteristic);

            foreach (var group in grouped)
            {
                var characteristic = group.Key;

                foreach (var op in group)
                {
                    await characteristic.WriteValueAsync(op.Data);
                }
            }

            stopwatch.Stop();

            _logger.LogInformation(
                "Flushed {Count} write operations in {Duration}ms",
                operations.Count,
                stopwatch.ElapsedMilliseconds);
        }
        finally
        {
            _flushSemaphore.Release();
        }
    }

    private record WriteOperation
    {
        public required IBluetoothRemoteCharacteristic Characteristic { get; init; }
        public required byte[] Data { get; init; }
        public DateTime Timestamp { get; init; }
    }
}
```

### Reliable Write Transactions

```csharp
public class ReliableWriteManager
{
    public async Task WriteMult ipleCharacteristicsAtomicallyAsync(
        IBluetoothRemoteDevice device,
        Dictionary<IBluetoothRemoteCharacteristic, byte[]> writes,
        CancellationToken cancellationToken = default)
    {
        // Use the first characteristic to manage the transaction
        var firstCharacteristic = writes.Keys.First();

        try
        {
            // Begin reliable write transaction
            await firstCharacteristic.BeginReliableWriteAsync(cancellationToken: cancellationToken);

            // Queue all writes
            foreach (var (characteristic, data) in writes)
            {
                await characteristic.WriteValueAsync(data, cancellationToken: cancellationToken);
            }

            // Execute all writes atomically
            await firstCharacteristic.ExecuteReliableWriteAsync(cancellationToken: cancellationToken);

            _logger.LogInformation(
                "Reliable write transaction completed - {Count} characteristics",
                writes.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Reliable write transaction failed, aborting");

            // Abort transaction on error
            try
            {
                await firstCharacteristic.AbortReliableWriteAsync();
            }
            catch (Exception abortEx)
            {
                _logger.LogWarning(abortEx, "Failed to abort reliable write transaction");
            }

            throw;
        }
    }
}
```

## Parallel Operations

Execute independent operations concurrently.

### Parallel Service Discovery

```csharp
public class ParallelServiceDiscovery
{
    public async Task<Dictionary<Guid, IBluetoothRemoteService>> DiscoverServicesInParallelAsync(
        IBluetoothRemoteDevice device,
        IEnumerable<Guid> serviceUuids,
        CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();

        // First, discover all services
        await device.ExploreServicesAsync(
            ServiceExplorationOptions.WithCharacteristics,
            cancellationToken: cancellationToken);

        // Then explore characteristics for each service in parallel
        var tasks = serviceUuids.Select(async uuid =>
        {
            try
            {
                var service = device.GetService(uuid);
                if (service != null)
                {
                    await service.ExploreCharacteristicsAsync(
                        CharacteristicExplorationOptions.Full,
                        cancellationToken: cancellationToken);

                    return (uuid, service);
                }

                return (uuid, service: (IBluetoothRemoteService?)null);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to explore service {ServiceId}", uuid);
                return (uuid, service: (IBluetoothRemoteService?)null);
            }
        });

        var results = await Task.WhenAll(tasks);
        stopwatch.Stop();

        var services = results
            .Where(r => r.service != null)
            .ToDictionary(r => r.uuid, r => r.service!);

        _logger.LogInformation(
            "Discovered {Count} services in parallel in {Duration}ms",
            services.Count,
            stopwatch.ElapsedMilliseconds);

        return services;
    }
}
```

## Performance Monitoring

Track and analyze performance metrics.

### Performance Metrics Collector

```csharp
public class PerformanceMetrics
{
    private readonly List<OperationMetric> _metrics = new();

    public async Task<T> MeasureOperationAsync<T>(
        string operationName,
        Func<Task<T>> operation)
    {
        var stopwatch = Stopwatch.StartNew();
        var startMemory = GC.GetTotalMemory(false);

        try
        {
            var result = await operation();

            stopwatch.Stop();
            var endMemory = GC.GetTotalMemory(false);

            var metric = new OperationMetric
            {
                Name = operationName,
                Duration = stopwatch.Elapsed,
                MemoryDelta = endMemory - startMemory,
                Success = true,
                Timestamp = DateTime.UtcNow
            };

            _metrics.Add(metric);

            _logger.LogDebug(
                "Operation {Name} completed in {Duration}ms (Memory: {Memory:+#,##0;-#,##0;0} bytes)",
                operationName,
                stopwatch.ElapsedMilliseconds,
                metric.MemoryDelta);

            return result;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();

            var metric = new OperationMetric
            {
                Name = operationName,
                Duration = stopwatch.Elapsed,
                Success = false,
                ErrorMessage = ex.Message,
                Timestamp = DateTime.UtcNow
            };

            _metrics.Add(metric);

            throw;
        }
    }

    public PerformanceReport GetReport()
    {
        var grouped = _metrics.GroupBy(m => m.Name);

        var summary = grouped.Select(g => new OperationSummary
        {
            OperationName = g.Key,
            TotalCalls = g.Count(),
            SuccessfulCalls = g.Count(m => m.Success),
            AverageDuration = TimeSpan.FromMilliseconds(g.Average(m => m.Duration.TotalMilliseconds)),
            MinDuration = g.Min(m => m.Duration),
            MaxDuration = g.Max(m => m.Duration),
            TotalMemoryDelta = g.Sum(m => m.MemoryDelta)
        }).ToList();

        return new PerformanceReport { Operations = summary };
    }

    private record OperationMetric
    {
        public required string Name { get; init; }
        public TimeSpan Duration { get; init; }
        public long MemoryDelta { get; init; }
        public bool Success { get; init; }
        public string? ErrorMessage { get; init; }
        public DateTime Timestamp { get; init; }
    }

    public record OperationSummary
    {
        public required string OperationName { get; init; }
        public int TotalCalls { get; init; }
        public int SuccessfulCalls { get; init; }
        public TimeSpan AverageDuration { get; init; }
        public TimeSpan MinDuration { get; init; }
        public TimeSpan MaxDuration { get; init; }
        public long TotalMemoryDelta { get; init; }
    }

    public record PerformanceReport
    {
        public required List<OperationSummary> Operations { get; init; }
    }
}
```

## Summary

### Performance Optimization Checklist

- [ ] **Negotiate larger MTU** (up to 517 bytes) for bulk transfers
- [ ] Use **write without response** for high-throughput operations
- [ ] **Cache service discovery** results
- [ ] Use **L2CAP channels** for maximum throughput (when supported)
- [ ] Set **connection priority** to High for real-time/bulk transfers
- [ ] **Batch operations** to reduce overhead
- [ ] Execute **independent operations in parallel**
- [ ] **Monitor performance metrics** during development
- [ ] **Profile on target devices** - performance varies by hardware
- [ ] Test with **realistic data sizes** and patterns

### Performance Comparison

| Optimization | Throughput Improvement | Complexity |
|-------------|----------------------|------------|
| MTU 517 vs 23 | 20-25x | Low |
| Write without response | 2-3x | Low |
| Service caching | N/A (reduces latency) | Low |
| L2CAP channels | 5-10x | Medium |
| High connection priority | 1.5-2x | Low |
| Batch operations | 1.5-2x | Medium |
| Parallel operations | 2-4x | Medium-High |

### Related Topics

- [Battery Optimization](Battery-Optimization.md) - Balance performance with battery life
- [Connection Management](Connection-Management.md) - Optimize connection strategies
- [Error Handling](Error-Handling.md) - Handle performance-related errors
