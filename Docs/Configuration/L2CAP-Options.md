# L2CAP Options

`L2CapChannelOptions` defines configuration options for L2CAP (Logical Link Control and Adaptation Protocol) channel operations. These options control timeout behavior, MTU settings, and background reading for L2CAP data channels over Bluetooth.

## Table of Contents

- [Overview](#overview)
- [Configuration](#configuration)
- [Properties](#properties)
  - [Timeout Options](#timeout-options)
  - [MTU Configuration](#mtu-configuration)
  - [Background Reading](#background-reading)
  - [Write Behavior](#write-behavior)
- [Usage Examples](#usage-examples)
- [Platform Considerations](#platform-considerations)
- [Performance Tuning](#performance-tuning)
- [Best Practices](#best-practices)

---

## Overview

L2CAP channels provide connection-oriented data channels over Bluetooth, enabling efficient bulk data transfer. L2CAP options control:

- Operation timeouts (open, close, read, write)
- Maximum Transmission Unit (MTU) size
- Background read loop behavior
- Auto-flush write behavior

**Namespace**: `Bluetooth.Abstractions.Scanning.Options`

**Configuration via**: `IOptions<L2CapChannelOptions>` (DI-configured)

**Key Features**:
- Connection-oriented data channels
- Configurable MTU for optimal throughput
- Automatic or manual read modes
- Timeout control for all operations

---

## Configuration

### Basic Configuration

Configure in your `MauiProgram.cs`:

```csharp
public static MauiApp CreateMauiApp()
{
    var builder = MauiApp.CreateBuilder();

    builder.Services.AddBluetoothServices();

    builder.Services.Configure<L2CapChannelOptions>(options =>
    {
        options.OpenTimeout = TimeSpan.FromSeconds(30);
        options.CloseTimeout = TimeSpan.FromSeconds(10);
        options.ReadTimeout = TimeSpan.FromSeconds(10);
        options.WriteTimeout = TimeSpan.FromSeconds(10);
        options.DefaultMtu = 672;
        options.EnableBackgroundReading = true;
        options.AutoFlushWrites = true;
    });

    return builder.Build();
}
```

### Configuration from appsettings.json

```json
{
  "Bluetooth": {
    "L2CAP": {
      "OpenTimeout": "00:00:30",
      "CloseTimeout": "00:00:10",
      "ReadTimeout": "00:00:10",
      "WriteTimeout": "00:00:10",
      "DefaultMtu": 672,
      "ReadBufferSize": null,
      "EnableBackgroundReading": true,
      "AutoFlushWrites": true
    }
  }
}
```

Bind in `MauiProgram.cs`:

```csharp
builder.Services.Configure<L2CapChannelOptions>(
    builder.Configuration.GetSection("Bluetooth:L2CAP"));
```

### Accessing in Your Code

```csharp
public class MyL2CapService
{
    private readonly L2CapChannelOptions _options;

    public MyL2CapService(IOptions<L2CapChannelOptions> options)
    {
        _options = options.Value;
    }

    public async Task UseChannel(IL2CapChannel channel)
    {
        // Options are automatically applied to channel operations
        await channel.OpenAsync(); // Uses OpenTimeout
        await channel.WriteAsync(data); // Uses WriteTimeout
    }
}
```

---

## Properties

### Timeout Options

#### OpenTimeout

```csharp
public TimeSpan OpenTimeout { get; init; } = TimeSpan.FromSeconds(30);
```

**Default**: `30 seconds`

Timeout for opening an L2CAP channel.

**Operation**: Applies to `IL2CapChannel.OpenAsync()`

**Considerations**:
- Increase for slow devices or poor signal quality
- Increase for multi-hop or gateway connections
- Decrease for fast failure detection in testing

**Use Cases**:

| Timeout | Use Case |
|---------|----------|
| 10-15s | Fast local connections |
| 30s (default) | Standard use |
| 60-90s | Slow devices, poor signal, industrial equipment |
| 120s+ | Extreme environments, firmware update mode |

**Example**:
```csharp
// Standard configuration
options.OpenTimeout = TimeSpan.FromSeconds(30);

// Slow industrial device
options.OpenTimeout = TimeSpan.FromSeconds(90);

// Fast failure detection
options.OpenTimeout = TimeSpan.FromSeconds(10);
```

---

#### CloseTimeout

```csharp
public TimeSpan CloseTimeout { get; init; } = TimeSpan.FromSeconds(10);
```

**Default**: `10 seconds`

Timeout for closing an L2CAP channel.

**Operation**: Applies to `IL2CapChannel.CloseAsync()`

**Considerations**:
- Usually shorter than open timeout (simpler operation)
- Increase if observing incomplete cleanup
- Decrease in testing for fast cleanup

**Use Cases**:

| Timeout | Use Case |
|---------|----------|
| 5s | Fast cleanup, testing |
| 10s (default) | Standard use |
| 20-30s | Ensure complete cleanup, prevent resource leaks |

**Example**:
```csharp
// Standard configuration
options.CloseTimeout = TimeSpan.FromSeconds(10);

// Fast cleanup for testing
options.CloseTimeout = TimeSpan.FromSeconds(5);

// Ensure complete cleanup
options.CloseTimeout = TimeSpan.FromSeconds(20);
```

---

#### ReadTimeout

```csharp
public TimeSpan ReadTimeout { get; init; } = TimeSpan.FromSeconds(10);
```

**Default**: `10 seconds`

Timeout for reading data from an L2CAP channel.

**Operation**: Applies to `IL2CapChannel.ReadAsync()`

**Considerations**:
- Increase for devices with delayed responses
- Increase when reading large data blocks
- Decrease for interactive applications needing responsiveness

**Use Cases**:

| Timeout | Use Case |
|---------|----------|
| 5s | Interactive apps, quick responses expected |
| 10s (default) | Standard use |
| 30-60s | Slow devices, large data reads |
| 120s+ | Firmware downloads, bulk transfers |

**Example**:
```csharp
// Interactive application
options.ReadTimeout = TimeSpan.FromSeconds(5);

// Large data transfer
options.ReadTimeout = TimeSpan.FromSeconds(60);

// Firmware updates
options.ReadTimeout = TimeSpan.FromMinutes(5);
```

---

#### WriteTimeout

```csharp
public TimeSpan WriteTimeout { get; init; } = TimeSpan.FromSeconds(10);
```

**Default**: `10 seconds`

Timeout for writing data to an L2CAP channel.

**Operation**: Applies to `IL2CapChannel.WriteAsync()`

**Considerations**:
- Increase for large data transfers
- Increase for devices with limited throughput or flow control
- Increase when operating in high-interference environments
- Decrease for small control messages

**Use Cases**:

| Timeout | Use Case |
|---------|----------|
| 5-10s | Small control messages |
| 10s (default) | Standard use |
| 30-120s | Large transfers, firmware updates |
| 300s+ | Very large files, slow connections |

**Example**:
```csharp
// Control messages
options.WriteTimeout = TimeSpan.FromSeconds(5);

// Firmware update
options.WriteTimeout = TimeSpan.FromMinutes(2);

// Large file transfer
options.WriteTimeout = TimeSpan.FromMinutes(10);
```

---

### MTU Configuration

#### DefaultMtu

```csharp
public int DefaultMtu { get; init; } = 672;
```

**Default**: `672 bytes` (Bluetooth L2CAP minimum guaranteed)

Maximum Transmission Unit to use when platform cannot determine actual MTU.

**Platform Behavior**:

| Platform | MTU Detection | Fallback to DefaultMtu |
|----------|---------------|----------------------|
| **iOS/macOS** | Not exposed by CoreBluetooth | Always uses DefaultMtu |
| **Android** | API 29-32: MaxTransmitPacketSize | Uses DefaultMtu if unavailable |
| **Windows** | Depends on implementation | May use DefaultMtu |

**Common MTU Values**:

| MTU | Description | Use Case |
|-----|-------------|----------|
| 512 | Safe minimum | Memory-constrained devices |
| 672 (default) | BT spec minimum | Maximum compatibility |
| 1024-2048 | Good balance | Most applications |
| 4096 | High throughput | Firmware updates, file transfers |
| 23170 | BLE 5.0 common max | Modern devices with BLE 5.0 |
| 65535 | Theoretical max | Rarely supported |

**Trade-offs**:

| Aspect | Larger MTU | Smaller MTU |
|--------|-----------|-------------|
| **Throughput** | Higher | Lower |
| **Packets** | Fewer | More |
| **Overhead** | Lower | Higher |
| **Compatibility** | Lower | Higher |
| **Memory** | More | Less |
| **Retries** | Slower | Faster |

**Example**:
```csharp
// Maximum compatibility (default)
options.DefaultMtu = 672;

// Balanced performance
options.DefaultMtu = 2048;

// High-throughput firmware updates
options.DefaultMtu = 4096;

// BLE 5.0 optimal
options.DefaultMtu = 23170;

// Memory-constrained IoT device
options.DefaultMtu = 512;
```

**Real-World Performance Impact**:

Firmware update transfer (1 MB file):

| MTU | Packets | Transfer Time (est) | Improvement |
|-----|---------|-------------------|-------------|
| 672 | ~1,488 | ~60s | Baseline |
| 2048 | ~488 | ~20s | 3x faster |
| 4096 | ~244 | ~10s | 6x faster |
| 23170 | ~43 | ~2s | 30x faster |

---

#### ReadBufferSize

```csharp
public int? ReadBufferSize { get; init; }
```

**Default**: `null` (uses MTU size)

Size of the read buffer used in the background read loop.

**Behavior**:
- **null** (default): Uses MTU size
- **Specified value**: Uses exact buffer size

**Platform Support**:

| Platform | Use | Notes |
|----------|-----|-------|
| **Android** | Buffer allocation | Used for background read loop |
| **iOS/macOS** | Not used | NSStream handles buffering |
| **Windows** | Depends | Platform-dependent |

**Considerations**:
- Set smaller for memory-constrained devices with small messages
- Set larger than MTU to batch multiple packets (increases latency)
- Should generally be at least as large as expected packet size

**Use Cases**:

| Buffer Size | Use Case |
|------------|----------|
| 256 | High-frequency small packets, memory-constrained |
| null (default) | Match MTU (recommended) |
| 2x MTU | Batch small packets |
| 8192+ | Bulk streaming |

**Example**:
```csharp
// Default - use MTU size (recommended)
options.ReadBufferSize = null;

// Small packets, save memory
options.ReadBufferSize = 256;

// Batch small packets (may increase latency)
options.ReadBufferSize = 4096;
```

---

### Background Reading

#### EnableBackgroundReading

```csharp
public bool EnableBackgroundReading { get; init; } = true;
```

**Default**: `true`

Controls whether to enable automatic background reading for `DataReceived` events.

**When true (default)**:
- Background task continuously reads from channel
- Raises `DataReceived` events when data arrives (push model)
- Event-driven architecture
- One thread pool thread consumed per channel

**When false**:
- Must call `ReadAsync()` manually (pull model)
- No `DataReceived` events
- No background overhead
- Explicit control over reads

**Platform Support**:

| Platform | Behavior |
|----------|----------|
| **Android** | Starts/stops background read loop |
| **iOS/macOS** | Not applicable (NSStreamDelegate provides push automatically) |
| **Windows** | Depends on implementation |

**Resource Impact**:

| Setting | Thread Usage | Memory | CPU |
|---------|-------------|--------|-----|
| **true** | 1 thread/channel | Read buffer | Continuous polling |
| **false** | None | None | On-demand only |

**Use Cases**:

**Enable if**:
- Using `DataReceived` event for notifications
- Want push-based notifications
- Event-driven application architecture
- Real-time data monitoring

**Disable if**:
- Only using `ReadAsync()` (pull model)
- Want to reduce background overhead
- Need explicit control over read timing
- Managing many channels (resource constraints)

**Example**:
```csharp
// Enable push-based reading (default)
options.EnableBackgroundReading = true;

// Disable for pull-based reading
options.EnableBackgroundReading = false;
```

**Usage Pattern**:

Push model (EnableBackgroundReading = true):
```csharp
channel.DataReceived += (sender, data) =>
{
    // Process data as it arrives
    ProcessReceivedData(data);
};

await channel.OpenAsync();
// Data arrives via events
```

Pull model (EnableBackgroundReading = false):
```csharp
await channel.OpenAsync();

while (true)
{
    var data = await channel.ReadAsync();
    ProcessReceivedData(data);
}
```

---

### Write Behavior

#### AutoFlushWrites

```csharp
public bool AutoFlushWrites { get; init; } = true;
```

**Default**: `true`

Controls whether to automatically flush the output stream after each write.

**When true (default)**:
- Data sent immediately after each `WriteAsync()`
- Lower latency (best for real-time)
- Lower throughput (more overhead)
- Each write is a separate packet

**When false**:
- Data may be buffered before transmission
- Higher throughput (20-30% improvement for bulk transfers)
- Higher latency (data delayed until buffer fills)
- Multiple writes batched into fewer packets

**Platform Support**:

| Platform | Behavior |
|----------|----------|
| **Android** | Controls whether `FlushAsync()` called after write |
| **iOS/macOS** | NSStream handles flushing (option ignored) |
| **Windows** | Depends on implementation |

**Latency vs Throughput Trade-off**:

| Setting | Latency | Throughput | Packets | Use Case |
|---------|---------|------------|---------|----------|
| **true** | Low | Moderate | More | Real-time, interactive |
| **false** | Higher | High | Fewer | File transfers, bulk data |

**Performance Impact**:

File transfer (1 MB, 1024-byte writes):

| AutoFlush | Latency per write | Total Time | Throughput |
|-----------|------------------|------------|------------|
| true | ~10ms | 10s | 100 KB/s |
| false | ~50ms | 7s | 143 KB/s (43% faster) |

**Use Cases**:

**Enable (true) if**:
- Real-time control/telemetry
- Interactive applications
- Low-latency critical
- Small, infrequent writes

**Disable (false) if**:
- File transfers
- Firmware updates
- Bulk data transfer
- Throughput critical
- Many consecutive writes

**Example**:
```csharp
// Low latency for real-time control (default)
options.AutoFlushWrites = true;

// High throughput for file transfer
options.AutoFlushWrites = false;
```

**Manual Flushing** (when AutoFlushWrites = false):
```csharp
// Batch multiple writes
await channel.WriteAsync(packet1);
await channel.WriteAsync(packet2);
await channel.WriteAsync(packet3);

// Manually flush when done
await channel.FlushAsync();
```

---

## Usage Examples

### Standard Configuration

```csharp
builder.Services.Configure<L2CapChannelOptions>(options =>
{
    options.OpenTimeout = TimeSpan.FromSeconds(30);
    options.CloseTimeout = TimeSpan.FromSeconds(10);
    options.ReadTimeout = TimeSpan.FromSeconds(10);
    options.WriteTimeout = TimeSpan.FromSeconds(10);
    options.DefaultMtu = 672;
    options.EnableBackgroundReading = true;
    options.AutoFlushWrites = true;
});
```

### Real-Time Control Application

```csharp
builder.Services.Configure<L2CapChannelOptions>(options =>
{
    // Low latency for control messages
    options.OpenTimeout = TimeSpan.FromSeconds(15);
    options.ReadTimeout = TimeSpan.FromSeconds(5);
    options.WriteTimeout = TimeSpan.FromSeconds(5);

    // Small MTU, immediate flush
    options.DefaultMtu = 672;
    options.AutoFlushWrites = true;

    // Push-based for immediate notifications
    options.EnableBackgroundReading = true;
});
```

### Firmware Update / File Transfer

```csharp
builder.Services.Configure<L2CapChannelOptions>(options =>
{
    // Generous timeouts for large transfers
    options.OpenTimeout = TimeSpan.FromSeconds(60);
    options.ReadTimeout = TimeSpan.FromMinutes(2);
    options.WriteTimeout = TimeSpan.FromMinutes(2);

    // Large MTU for throughput
    options.DefaultMtu = 4096;

    // Buffer writes for throughput
    options.AutoFlushWrites = false;

    // Pull-based reading
    options.EnableBackgroundReading = false;
});
```

### IoT Sensor Device (Memory Constrained)

```csharp
builder.Services.Configure<L2CapChannelOptions>(options =>
{
    // Standard timeouts
    options.OpenTimeout = TimeSpan.FromSeconds(30);
    options.ReadTimeout = TimeSpan.FromSeconds(10);
    options.WriteTimeout = TimeSpan.FromSeconds(10);

    // Small MTU and buffer to save memory
    options.DefaultMtu = 512;
    options.ReadBufferSize = 256;

    // Disable background reading to save thread
    options.EnableBackgroundReading = false;
    options.AutoFlushWrites = true;
});
```

### High-Performance Streaming

```csharp
builder.Services.Configure<L2CapChannelOptions>(options =>
{
    // Long timeouts for continuous streaming
    options.OpenTimeout = TimeSpan.FromSeconds(30);
    options.ReadTimeout = TimeSpan.FromMinutes(10);
    options.WriteTimeout = TimeSpan.FromMinutes(10);

    // Maximum MTU for throughput
    options.DefaultMtu = 23170; // BLE 5.0

    // Large read buffer for batching
    options.ReadBufferSize = 8192;

    // Push-based, batch writes
    options.EnableBackgroundReading = true;
    options.AutoFlushWrites = false;
});
```

### Development / Testing

```csharp
builder.Services.Configure<L2CapChannelOptions>(options =>
{
    // Short timeouts for fast failure
    options.OpenTimeout = TimeSpan.FromSeconds(10);
    options.CloseTimeout = TimeSpan.FromSeconds(5);
    options.ReadTimeout = TimeSpan.FromSeconds(5);
    options.WriteTimeout = TimeSpan.FromSeconds(5);

    // Standard MTU
    options.DefaultMtu = 672;

    // Immediate flush for debugging
    options.EnableBackgroundReading = true;
    options.AutoFlushWrites = true;
});
```

---

## Platform Considerations

### iOS/macOS (CoreBluetooth)

**MTU**:
- CoreBluetooth does not expose actual MTU
- Always uses `DefaultMtu` value
- Set based on known device capabilities

**Background Reading**:
- NSStreamDelegate provides push automatically
- `EnableBackgroundReading` option ignored

**Auto Flush**:
- NSStream handles flushing
- `AutoFlushWrites` option ignored

**Recommendation**:
```csharp
// iOS optimized
options.DefaultMtu = 1024; // Known device MTU
options.EnableBackgroundReading = true; // No effect, but consistent
options.AutoFlushWrites = true; // No effect, but consistent
```

### Android

**MTU**:
- API 29+: Can query `MaxTransmitPacketSize`
- API < 29: Uses `DefaultMtu`
- Falls back to `DefaultMtu` if query fails

**Background Reading**:
- Starts actual background thread
- Consider disabling for many channels

**Auto Flush**:
- Directly controls `FlushAsync()` calls
- Significant performance impact

**Recommendation**:
```csharp
// Android optimized
options.DefaultMtu = 2048; // Good balance
options.EnableBackgroundReading = true; // If using events
options.AutoFlushWrites = false; // For bulk transfers
```

### Windows

**MTU**:
- Platform-dependent MTU detection
- May use `DefaultMtu` as fallback

**Background Reading**:
- Implementation-dependent

**Auto Flush**:
- Implementation-dependent

**Recommendation**:
```csharp
// Windows default
options.DefaultMtu = 672; // Safe default
options.EnableBackgroundReading = true;
options.AutoFlushWrites = true;
```

---

## Performance Tuning

### Optimizing Throughput

1. **Increase MTU**:
   ```csharp
   options.DefaultMtu = 4096; // Or higher for BLE 5.0
   ```

2. **Disable Auto-Flush**:
   ```csharp
   options.AutoFlushWrites = false;
   ```

3. **Increase Timeouts**:
   ```csharp
   options.WriteTimeout = TimeSpan.FromMinutes(2);
   ```

4. **Batch Writes**:
   ```csharp
   for (int i = 0; i < packets.Length; i++)
   {
       await channel.WriteAsync(packets[i]);
   }
   await channel.FlushAsync(); // Manual flush at end
   ```

### Optimizing Latency

1. **Smaller MTU** (if needed):
   ```csharp
   options.DefaultMtu = 672;
   ```

2. **Enable Auto-Flush**:
   ```csharp
   options.AutoFlushWrites = true;
   ```

3. **Shorter Timeouts**:
   ```csharp
   options.ReadTimeout = TimeSpan.FromSeconds(5);
   options.WriteTimeout = TimeSpan.FromSeconds(5);
   ```

4. **Enable Background Reading**:
   ```csharp
   options.EnableBackgroundReading = true;
   ```

### Optimizing Memory

1. **Smaller MTU**:
   ```csharp
   options.DefaultMtu = 512;
   ```

2. **Smaller Read Buffer**:
   ```csharp
   options.ReadBufferSize = 256;
   ```

3. **Disable Background Reading**:
   ```csharp
   options.EnableBackgroundReading = false;
   ```

---

## Best Practices

### 1. Use Defaults When Possible

Defaults are suitable for most applications:
```csharp
// Only configure what needs to change
builder.Services.Configure<L2CapChannelOptions>(options =>
{
    options.DefaultMtu = 4096; // Only change this
});
```

### 2. Match MTU to Device Capabilities

Research your target devices:
```csharp
// For known BLE 5.0 devices
options.DefaultMtu = 23170;

// For legacy devices
options.DefaultMtu = 672;
```

### 3. Tune for Your Use Case

Different use cases need different settings:
```csharp
// Real-time: Low latency
options.AutoFlushWrites = true;

// Bulk transfer: High throughput
options.AutoFlushWrites = false;
```

### 4. Set Appropriate Timeouts

Consider your worst-case scenarios:
```csharp
// Firmware update: generous timeout
options.WriteTimeout = TimeSpan.FromMinutes(5);

// Interactive: quick timeout
options.WriteTimeout = TimeSpan.FromSeconds(5);
```

### 5. Test on Target Platforms

Behavior varies by platform:
```csharp
#if ANDROID
options.DefaultMtu = 2048;
options.EnableBackgroundReading = true;
#elif IOS
options.DefaultMtu = 1024;
// iOS manages background reading
#endif
```

### 6. Monitor Performance

Profile and adjust based on actual performance:
```csharp
var stopwatch = Stopwatch.StartNew();
await channel.WriteAsync(largeData);
await channel.FlushAsync();
stopwatch.Stop();

// If too slow, increase MTU or disable auto-flush
```

### 7. Document Custom Settings

Always document why you changed defaults:
```csharp
// Increased to 4096 for firmware update performance:
// Testing showed 6x speedup on target devices
options.DefaultMtu = 4096;
```

---

## Related Documentation

- [Dependency-Injection.md](./Dependency-Injection.md) - DI configuration guide
- [Infrastructure-Options.md](./Infrastructure-Options.md) - Infrastructure configuration
- [Connection-Options.md](./Connection-Options.md) - Connection configuration
- [Scanning-Options.md](./Scanning-Options.md) - Scanner configuration
