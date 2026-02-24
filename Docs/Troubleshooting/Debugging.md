# Debugging Bluetooth Issues

This guide provides comprehensive debugging techniques, logging strategies, and platform-specific tools to diagnose and resolve Bluetooth issues.

## Table of Contents

1. [Enabling Logging](#enabling-logging)
2. [Understanding Log EventIds](#understanding-log-eventids)
3. [Analyzing Structured Logs](#analyzing-structured-logs)
4. [Common Log Patterns](#common-log-patterns)
5. [Platform Native Tools](#platform-native-tools)
6. [Advanced Debugging Techniques](#advanced-debugging-techniques)

---

## Enabling Logging

Plugin.Bluetooth uses Microsoft.Extensions.Logging for comprehensive structured logging across all operations.

### Basic Logging Setup

```csharp
// In MauiProgram.cs
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

        // Add logging
        builder.Logging.AddDebug(); // For Debug output window
        builder.Services.AddBluetoothServices();

        return builder.Build();
    }
}
```

### Enable Verbose Logging

For detailed diagnostic information:

```csharp
builder.Services.Configure<BluetoothInfrastructureOptions>(options =>
{
    options.EnableVerboseLogging = true;
});
```

**Warning:** Verbose logging significantly increases log volume and may impact performance. Use only during development/debugging.

### Configure Log Levels

Control log verbosity by category:

```csharp
builder.Logging.SetMinimumLevel(LogLevel.Debug);

builder.Logging.AddFilter("Bluetooth", LogLevel.Debug);
builder.Logging.AddFilter("Bluetooth.Core.Scanning", LogLevel.Trace);
builder.Logging.AddFilter("Bluetooth.Maui.Platforms.Droid", LogLevel.Information);
```

### Custom Logger Configuration

```csharp
builder.Logging.AddDebug();

#if DEBUG
builder.Logging.SetMinimumLevel(LogLevel.Trace);
#else
builder.Logging.SetMinimumLevel(LogLevel.Warning);
#endif

// Add console logging
builder.Logging.AddConsole();

// Add file logging (requires additional package)
builder.Logging.AddFile("Logs/bluetooth-{Date}.txt");
```

---

## Understanding Log EventIds

Plugin.Bluetooth uses structured EventIds to categorize log entries. This enables filtering and targeted debugging.

### EventId Range Overview

| Component | EventId Range | Description |
|-----------|---------------|-------------|
| **Scanner (Core)** | 100-199 | Scanner lifecycle, device discovery |
| **Device (Core)** | 200-299 | Connection/disconnection, service exploration |
| **Service (Core)** | 300-399 | Service exploration, characteristic discovery |
| **Characteristic (Core)** | 400-499 | Read/write/notify operations |
| **Descriptor (Core)** | 500-599 | Descriptor operations |
| **L2CAP Channel (Core)** | 600-699 | L2CAP channel operations |
| **Broadcaster (Core)** | 700-799 | Broadcasting/advertising operations |
| **Local Service (Core)** | 800-899 | Local service management |
| **Local Characteristic** | 900-999 | Local characteristic operations |
| **Android Platform** | 1000-1999 | Android-specific operations |
| **iOS/macOS Platform** | 2000-2999 | Apple-specific operations |
| **Windows Platform** | 3000-3999 | Windows-specific operations |

### Core Scanner Events (100-199)

```
100 - Scanner starting
101 - Scanner started successfully
102 - Scanner failed to start
103 - Scanner stopping
104 - Scanner stopped successfully
105 - Scanner failed to stop
106 - Scanner started unexpectedly
107 - Scanner stopped unexpectedly
108 - Updating scanner configuration
109 - Scanner configuration update failed
110 - Merging concurrent start operation
111 - Merging concurrent stop operation
112 - Scanner already started
113 - Scanner already stopped
114 - Error checking scanner permissions
```

**Example Logs:**

```
[Information] EventId: 100 - Scanner starting with 1 service UUIDs
[Information] EventId: 101 - Scanner started successfully
[Warning] EventId: 112 - Scanner already started, throwing ScannerIsAlreadyStartedException
```

### Core Device Events (200-299)

```
200-209 - Connection operations
  200 - Device connecting
  201 - Device connected successfully
  202 - Device failed to connect
  203 - Device already connected
  204 - Merging concurrent connection attempts
  205 - Waiting for advertisement before connecting

210-219 - Disconnection operations
  210 - Device disconnecting
  211 - Device disconnected successfully
  212 - Device failed to disconnect
  213 - Device already disconnected
  214 - Merging concurrent disconnection attempts

220-229 - Unexpected disconnection
  220 - Device unexpectedly disconnected
  221 - Unexpected disconnection ignored

230-239 - Connection priority
  230 - Requesting connection priority
  231 - Cannot request priority - not connected

240-259 - Service exploration
  240 - Exploring services
  241 - Service exploration succeeded
  242 - Service exploration failed
  243 - Unexpected service exploration
  244 - Merging service exploration attempts
  245 - Cannot explore - not connected
  246 - Using cached services
  247 - Cascading exploration to characteristics
  248 - Services cleared
```

**Example Logs:**

```
[Information] EventId: 200 - Device AA:BB:CC:DD:EE:FF connecting
[Information] EventId: 201 - Device AA:BB:CC:DD:EE:FF connected successfully
[Warning] EventId: 220 - Device AA:BB:CC:DD:EE:FF unexpectedly disconnected
```

### Core Service Events (300-399)

```
300 - Service exploring characteristics
301 - Characteristic exploration succeeded
302 - Characteristic exploration failed
303 - Unexpected characteristic exploration
304 - Merging characteristic exploration
305 - Using cached characteristics
306 - Cascading to descriptors
307 - Characteristics cleared
```

### Core Characteristic Events (400-499)

```
400-419 - Read operations
  400 - Reading value
  401 - Read succeeded
  402 - Read failed
  403 - Unexpected read
  404 - Merging read attempts
  405 - Cannot read - not connected
  406 - Characteristic not readable

420-439 - Write operations
  420 - Writing value
  421 - Write succeeded
  422 - Write failed
  423 - Unexpected write
  424 - Queuing write operation
  425 - Cannot write - not connected
  426 - Characteristic not writable

440-459 - Notification operations
  440 - Starting notifications
  441 - Notifications started successfully
  442 - Failed to start notifications
  443 - Stopping notifications
  444 - Notifications stopped successfully
  445 - Failed to stop notifications
  446 - Cannot start - not connected
  447 - Characteristic not notifiable
  448 - Value updated
  449 - Merging notification attempts
```

**Example Logs:**

```
[Debug] EventId: 400 - Characteristic 00002a37-...-9b34fb on device AA:BB:CC:DD:EE:FF reading value
[Debug] EventId: 401 - Characteristic 00002a37-...-9b34fb on device AA:BB:CC:DD:EE:FF read succeeded - 2 bytes
[Information] EventId: 440 - Characteristic 00002a37-...-9b34fb on device AA:BB:CC:DD:EE:FF starting notifications
[Information] EventId: 441 - Characteristic 00002a37-...-9b34fb on device AA:BB:CC:DD:EE:FF notifications started successfully
[Debug] EventId: 448 - Characteristic 00002a37-...-9b34fb on device AA:BB:CC:DD:EE:FF value updated - 2 bytes
```

### Android Platform Events (1000-1999)

```
1000-1099 - Scanner events
  1000 - Starting BLE scan
  1001 - BLE scan started
  1002 - Scan start retry
  1003 - Scan start failed
  1004 - Stopping BLE scan
  1005 - BLE scan stopped
  1006 - Scan failure received
  1007 - Device discovered

2000-2099 - Connection events
  2000 - Connecting to device
  2001 - Successfully connected
  2002 - Connection retry
  2003 - Connection failed
  2004 - Disconnecting
  2005 - Successfully disconnected
  2006 - Connection priority failed
  2007 - Connection priority applied
  2008 - Disconnect error

3000-3099 - Service discovery
  3000 - Starting service discovery
  3001 - Service discovery completed
  3002 - Service discovery retry
  3003 - Service discovery failed

4000-4099 - GATT operations
  4000 - Reading characteristic
  4001 - Writing characteristic
  4002 - Characteristic write retry
  4003 - Reading descriptor
  4004 - Writing descriptor
  4005 - Descriptor write retry

5000-5099 - Notification events
  5000 - Notification state change
  5001 - Notification received

7000-7099 - L2CAP channel events
  7000 - Opening L2CAP channel
  7001 - L2CAP channel opened
  7002 - L2CAP open failed
  7003 - Closing L2CAP channel
  7004 - L2CAP channel closed
  7005 - Reading from L2CAP
  7006 - L2CAP read completed
  7007 - Writing to L2CAP
  7008 - L2CAP write completed
  7009 - L2CAP data received
  7010 - L2CAP read loop error
  7011 - L2CAP close error
```

**Example Logs:**

```
[Information] EventId: 1000 - Starting BLE scan with mode: LowLatency, callback type: AllMatches
[Information] EventId: 1001 - BLE scan started successfully
[Debug] EventId: 1007 - Device discovered: AA:BB:CC:DD:EE:FF, RSSI: -65
[Information] EventId: 2000 - Connecting to device AA:BB:CC:DD:EE:FF
[Warning] EventId: 2002 - Connection attempt 1 of 3 to device AA:BB:CC:DD:EE:FF failed
[Information] EventId: 2001 - Successfully connected to device AA:BB:CC:DD:EE:FF
```

### Filtering by EventId

#### Visual Studio Debug Output

Use search/filter in the Output window:

```
EventId: 200  // Show only device connection start events
EventId: 2    // Show all connection-related events (200-299, 2000-2099)
EventId: 4    // Show all characteristic operations (400-499, 4000-4099)
```

#### Code-based Filtering

```csharp
builder.Logging.AddFilter((category, level, eventId) =>
{
    // Only log scanner and device events
    if (eventId.Id >= 100 && eventId.Id < 300)
        return true;

    // Log all errors and warnings
    if (level >= LogLevel.Warning)
        return true;

    return false;
});
```

#### Log Analysis Tools

Use structured logging tools like Seq, Serilog, or Application Insights:

```csharp
// With Serilog
builder.Logging.AddSerilog(new LoggerConfiguration()
    .WriteTo.Seq("http://localhost:5341")
    .CreateLogger());
```

Query by EventId in Seq:
```
EventId >= 200 and EventId < 300  // Device operations
EventId = 1007  // Android device discovery
Level = 'Error'  // All errors
```

---

## Analyzing Structured Logs

### Understanding Log Structure

Plugin.Bluetooth logs include rich structured data:

```
[LogLevel] EventId: ### - Message with {Parameter1} and {Parameter2}
Exception details (if applicable)
```

**Example:**

```
[Error] EventId: 202 - Device AA:BB:CC:DD:EE:FF failed to connect
Bluetooth.Maui.Platforms.Droid.Exceptions.AndroidNativeGattStatusException: Error: GATT error. : Failure
   at Bluetooth.Maui.Platforms.Droid.Scanning.AndroidBluetoothRemoteDevice.ConnectAsync(...)
```

### Key Log Patterns for Debugging

#### Successful Connection Flow

```
[Info] EventId: 100 - Scanner starting with 0 service UUIDs
[Info] EventId: 101 - Scanner started successfully
[Debug] EventId: 1007 - Device discovered: AA:BB:CC:DD:EE:FF, RSSI: -65
[Info] EventId: 200 - Device AA:BB:CC:DD:EE:FF connecting
[Info] EventId: 201 - Device AA:BB:CC:DD:EE:FF connected successfully
[Info] EventId: 240 - Device AA:BB:CC:DD:EE:FF exploring services
[Info] EventId: 241 - Device AA:BB:CC:DD:EE:FF service exploration succeeded - 3 services found
```

#### Connection Failure with Retry

```
[Info] EventId: 200 - Device AA:BB:CC:DD:EE:FF connecting
[Warning] EventId: 2002 - Connection attempt 1 of 3 to device AA:BB:CC:DD:EE:FF failed
AndroidNativeGattStatusException: Error: GATT error. : Failure
[Warning] EventId: 2002 - Connection attempt 2 of 3 to device AA:BB:CC:DD:EE:FF failed
AndroidNativeGattStatusException: Error: GATT error. : Failure
[Info] EventId: 201 - Device AA:BB:CC:DD:EE:FF connected successfully
```

#### Unexpected Disconnection

```
[Info] EventId: 201 - Device AA:BB:CC:DD:EE:FF connected successfully
[Warning] EventId: 220 - Device AA:BB:CC:DD:EE:FF unexpectedly disconnected
AndroidNativeGattCallbackStatusConnectionException: Error: GATT connection timeout. : GattConnectionTimeout
```

#### Notification Setup

```
[Info] EventId: 440 - Characteristic 00002a37-0000-1000-8000-00805f9b34fb on device AA:BB:CC:DD:EE:FF starting notifications
[Debug] EventId: 5000 - Enabling notifications for characteristic 00002a37-...-9b34fb on device AA:BB:CC:DD:EE:FF
[Info] EventId: 441 - Characteristic 00002a37-0000-1000-8000-00805f9b34fb on device AA:BB:CC:DD:EE:FF notifications started successfully
[Trace] EventId: 5001 - Notification received for characteristic 00002a37-...-9b34fb on device AA:BB:CC:DD:EE:FF, 2 bytes
[Debug] EventId: 448 - Characteristic 00002a37-0000-1000-8000-00805f9b34fb on device AA:BB:CC:DD:EE:FF value updated - 2 bytes
```

---

## Common Log Patterns

### Pattern 1: GATT Error 133 (Android)

**Symptom Logs:**

```
[Info] EventId: 200 - Device AA:BB:CC:DD:EE:FF connecting
[Warning] EventId: 2002 - Connection attempt 1 of 3 to device AA:BB:CC:DD:EE:FF failed
AndroidNativeGattStatusException: Error: GATT error. : Failure
   GattStatus: Failure (133)
[Warning] EventId: 2002 - Connection attempt 2 of 3 to device AA:BB:CC:DD:EE:FF failed
[Warning] EventId: 2002 - Connection attempt 3 of 3 to device AA:BB:CC:DD:EE:FF failed
[Error] EventId: 202 - Device AA:BB:CC:DD:EE:FF failed to connect
[Error] EventId: 2003 - Failed to connect to device AA:BB:CC:DD:EE:FF after 3 attempts
```

**Diagnosis:** Generic GATT error, likely transient connection issue.

**Solutions:**
- Increase retry count
- Add delay before connection
- Check device range (RSSI)
- See [Common Issues - GATT Error 133](./Common-Issues.md#android-gatt-error-133)

### Pattern 2: Permission Denied

**Symptom Logs:**

```
[Info] EventId: 100 - Scanner starting with 0 service UUIDs
[Error] EventId: 114 - Error checking scanner permissions
Bluetooth.Abstractions.Exceptions.BluetoothPermissionException: Bluetooth permission request failed or was denied
[Error] EventId: 102 - Scanner failed to start
```

**Diagnosis:** Missing or denied permissions.

**Solutions:**
- Verify manifest/Info.plist configuration
- Check runtime permission status
- Request permissions explicitly
- See [Common Issues - Permission Issues](./Common-Issues.md#permission-issues)

### Pattern 3: CCCD Descriptor Not Found

**Symptom Logs:**

```
[Info] EventId: 440 - Characteristic 00002a37-...-9b34fb on device AA:BB:CC:DD:EE:FF starting notifications
[Error] EventId: 442 - Characteristic 00002a37-...-9b34fb on device AA:BB:CC:DD:EE:FF failed to start notifications
System.InvalidOperationException: CCCD descriptor not found for this characteristic
```

**Diagnosis:** Descriptors not discovered, or characteristic doesn't support notifications.

**Solutions:**
- Use full exploration: `ExploreServicesAsync(ServiceExplorationOptions.Full)`
- Verify characteristic properties: `characteristic.CanNotify`
- See [Common Issues - Notification Issues](./Common-Issues.md#notification-issues)

### Pattern 4: Service Discovery Timeout

**Symptom Logs:**

```
[Info] EventId: 240 - Device AA:BB:CC:DD:EE:FF exploring services
[Info] EventId: 3000 - Starting service discovery for device AA:BB:CC:DD:EE:FF
[Warning] EventId: 3002 - Service discovery attempt 1 of 2 failed for device AA:BB:CC:DD:EE:FF
System.TimeoutException: The operation has timed out
[Warning] EventId: 3002 - Service discovery attempt 2 of 2 failed for device AA:BB:CC:DD:EE:FF
[Error] EventId: 242 - Device AA:BB:CC:DD:EE:FF service exploration failed
[Error] EventId: 3003 - Service discovery failed for device AA:BB:CC:DD:EE:FF after 2 attempts
```

**Diagnosis:** Device taking too long to respond to service discovery request.

**Solutions:**
- Increase `DefaultOperationTimeout`
- Increase service discovery retry count
- Check connection quality (RSSI)
- Verify device is functioning correctly

### Pattern 5: Concurrent Operation Merging

**Symptom Logs:**

```
[Info] EventId: 200 - Device AA:BB:CC:DD:EE:FF connecting
[Debug] EventId: 204 - Device AA:BB:CC:DD:EE:FF merging concurrent connection attempts
[Info] EventId: 201 - Device AA:BB:CC:DD:EE:FF connected successfully
```

**Diagnosis:** Multiple connection attempts merged into single operation (expected behavior).

**Note:** This is not an error. The library efficiently merges concurrent operations to the same resource.

---

## Platform Native Tools

### Android Debugging Tools

#### 1. Bluetooth HCI Snoop Log

Captures all Bluetooth traffic at the HCI (Host Controller Interface) layer.

**Enable HCI Snoop Log:**

1. Enable Developer Options on Android device
2. Settings > Developer Options > Enable Bluetooth HCI snoop log
3. Reproduce the issue
4. Retrieve log file:
   - Location: `/sdcard/Android/data/btsnoop_hci.log` (or `/sdcard/btsnoop_hci.log`)
   - Pull via ADB: `adb pull /sdcard/btsnoop_hci.log`

**Analyze with Wireshark:**

1. Install Wireshark
2. Open `btsnoop_hci.log` in Wireshark
3. Filter by device address: `bluetooth.addr == AA:BB:CC:DD:EE:FF`
4. Look for GATT operations, ATT errors, connection events

**Common Wireshark Filters:**

```
btatt  // All GATT ATT packets
btatt.opcode == 0x12  // Read requests
btatt.opcode == 0x52  // Write requests
btatt.opcode == 0x1b  // Notifications
btatt.error_code  // ATT errors
bthci_evt.code == 0x05  // Disconnection events
```

#### 2. nRF Connect for Mobile

Best third-party BLE debugging app for Android.

**Features:**
- Scan for devices
- View advertisements
- Connect and explore services
- Read/write characteristics
- Enable notifications
- View raw data

**Use Cases:**
- Verify device is advertising correctly
- Test if characteristics are readable/writable
- Confirm notifications work outside your app
- Compare behavior with Plugin.Bluetooth

**Download:** [Google Play Store](https://play.google.com/store/apps/details?id=no.nordicsemi.android.mcp)

#### 3. Android Logcat

View real-time Android system logs:

```bash
# View all logs
adb logcat

# Filter by tag
adb logcat -s "Bluetooth"

# Filter by your app
adb logcat | grep "com.yourcompany.yourapp"

# Save to file
adb logcat > android-log.txt
```

#### 4. Android Studio Bluetooth Inspector

For apps debugged in Android Studio:
- View > Tool Windows > App Inspection > Bluetooth

### iOS/macOS Debugging Tools

#### 1. LightBlue

Excellent BLE debugging app for iOS/macOS.

**Features:**
- Device scanning and filtering
- Advertisement data viewer
- Service/characteristic exploration
- Read/write/notify operations
- Virtual peripheral mode

**Use Cases:**
- Test BLE peripheral behavior
- Verify iOS permissions are working
- Debug notification issues
- Create virtual peripherals for testing

**Download:** [App Store](https://apps.apple.com/app/lightblue/id557428110)

#### 2. Xcode Console

View iOS system and app logs:

1. Connect iOS device to Mac
2. Open Xcode > Window > Devices and Simulators
3. Select device
4. Click "View Device Logs"
5. Filter by process name or search for "Bluetooth"

**Console App (macOS):**
- Open Console.app
- Connect iOS device
- Filter by process or search terms

#### 3. PacketLogger (Xcode Additional Tools)

Captures Bluetooth packets on macOS.

**Installation:**
1. Xcode > Preferences > Downloads > Components
2. Download "Additional Tools for Xcode"
3. Open PacketLogger from Additional Tools dmg

**Usage:**
1. Launch PacketLogger
2. Start capture
3. Reproduce issue
4. Save capture for analysis

#### 4. Bluetooth Explorer (Additional Tools)

Low-level Bluetooth debugging tool.

**Features:**
- View Bluetooth hardware info
- Monitor connections
- View L2CAP channels
- Examine GATT services

### Windows Debugging Tools

#### 1. Bluetooth LE Explorer

Official Microsoft tool for BLE development.

**Installation:**
- Microsoft Store: Search "Bluetooth LE Explorer"
- Or via winget: `winget install Microsoft.BluetoothLEExplorer`

**Features:**
- Scan for devices
- Service/characteristic browser
- Read/write operations
- Notification subscription
- Pairing management

**Use Cases:**
- Test device pairing
- Verify GATT structure
- Debug Windows-specific issues

#### 2. Event Viewer

View Windows system events:

1. Windows Key + X > Event Viewer
2. Applications and Services Logs > Microsoft > Windows > Bluetooth-BthLEPrepairing/Operational

#### 3. Windows Device Portal

For debugging Windows IoT or UWP apps:
- Enable Developer Mode
- Access via browser: `http://device-ip:8080`

---

## Advanced Debugging Techniques

### 1. Custom Log Capture

Create a custom logger to capture logs programmatically:

```csharp
public class InMemoryLogger : ILogger
{
    private readonly List<LogEntry> _logs = new();

    public IReadOnlyList<LogEntry> Logs => _logs.AsReadOnly();

    public IDisposable BeginScope<TState>(TState state) => null;

    public bool IsEnabled(LogLevel logLevel) => true;

    public void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception exception,
        Func<TState, Exception, string> formatter)
    {
        _logs.Add(new LogEntry
        {
            Timestamp = DateTime.UtcNow,
            Level = logLevel,
            EventId = eventId,
            Message = formatter(state, exception),
            Exception = exception
        });
    }

    public void ExportToFile(string path)
    {
        File.WriteAllLines(path, _logs.Select(l => l.ToString()));
    }
}

public class LogEntry
{
    public DateTime Timestamp { get; set; }
    public LogLevel Level { get; set; }
    public EventId EventId { get; set; }
    public string Message { get; set; }
    public Exception Exception { get; set; }

    public override string ToString()
    {
        var sb = new StringBuilder();
        sb.Append($"[{Timestamp:yyyy-MM-dd HH:mm:ss.fff}] ");
        sb.Append($"[{Level}] ");
        sb.Append($"EventId: {EventId.Id} - {Message}");
        if (Exception != null)
            sb.Append($"\n{Exception}");
        return sb.ToString();
    }
}
```

Register custom logger:

```csharp
var inMemoryLogger = new InMemoryLogger();
builder.Logging.AddProvider(new InMemoryLoggerProvider(inMemoryLogger));
```

### 2. Correlation ID for Request Tracking

Track operations across components:

```csharp
public async Task ConnectWithTracking(IBluetoothRemoteDevice device)
{
    var correlationId = Guid.NewGuid();
    _logger.LogInformation($"[{correlationId}] Starting connection");

    try
    {
        await device.ConnectAsync();
        _logger.LogInformation($"[{correlationId}] Connection successful");
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, $"[{correlationId}] Connection failed");
        throw;
    }
}
```

### 3. Performance Profiling

Measure operation durations:

```csharp
public async Task<T> MeasureAsync<T>(
    string operationName,
    Func<Task<T>> operation)
{
    var sw = Stopwatch.StartNew();
    try
    {
        var result = await operation();
        _logger.LogDebug($"{operationName} completed in {sw.ElapsedMilliseconds}ms");
        return result;
    }
    catch (Exception ex)
    {
        _logger.LogError($"{operationName} failed after {sw.ElapsedMilliseconds}ms: {ex.Message}");
        throw;
    }
}

// Usage
var device = await MeasureAsync("Device Connection",
    () => device.ConnectAsync());
```

### 4. State Dump for Diagnostics

Create diagnostic snapshots:

```csharp
public class BluetoothDiagnostics
{
    private readonly IBluetoothScanner _scanner;

    public string CreateDiagnosticReport()
    {
        var sb = new StringBuilder();
        sb.AppendLine("=== Bluetooth Diagnostic Report ===");
        sb.AppendLine($"Timestamp: {DateTime.UtcNow:O}");
        sb.AppendLine($"Platform: {DeviceInfo.Platform}");
        sb.AppendLine($"OS Version: {DeviceInfo.VersionString}");
        sb.AppendLine();

        sb.AppendLine("Scanner State:");
        sb.AppendLine($"  IsScanning: {_scanner.IsScanning}");
        sb.AppendLine($"  Bluetooth State: {_scanner.BluetoothState}");
        sb.AppendLine($"  Device Count: {_scanner.GetDevices().Count}");
        sb.AppendLine();

        sb.AppendLine("Devices:");
        foreach (var device in _scanner.GetDevices())
        {
            sb.AppendLine($"  - {device.Name ?? "Unknown"} ({device.Id})");
            sb.AppendLine($"    Connected: {device.IsConnected}");
            sb.AppendLine($"    RSSI: {device.Rssi}");
            sb.AppendLine($"    Services: {device.GetServices().Count}");
        }

        return sb.ToString();
    }

    public async Task SaveDiagnosticReportAsync(string path)
    {
        var report = CreateDiagnosticReport();
        await File.WriteAllTextAsync(path, report);
    }
}
```

### 5. Exception Analysis Helper

Analyze exception details:

```csharp
public class BluetoothExceptionAnalyzer
{
    public static string AnalyzeException(Exception ex)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"Exception Type: {ex.GetType().Name}");
        sb.AppendLine($"Message: {ex.Message}");

        if (ex is AndroidNativeGattStatusException gattEx)
        {
            sb.AppendLine($"GATT Status: {gattEx.GattStatus} ({(int)gattEx.GattStatus})");
            sb.AppendLine("Possible Causes:");

            switch ((int)gattEx.GattStatus)
            {
                case 133:
                    sb.AppendLine("  - Device out of range");
                    sb.AppendLine("  - Previous connection not cleaned up");
                    sb.AppendLine("  - Bluetooth stack issue");
                    sb.AppendLine("Recommendation: Enable retry logic, check RSSI");
                    break;
                case 137:
                    sb.AppendLine("  - Connection timeout");
                    sb.AppendLine("Recommendation: Increase timeout, check device proximity");
                    break;
                // Add more cases...
            }
        }
        else if (ex is BluetoothPermissionException)
        {
            sb.AppendLine("Possible Causes:");
            sb.AppendLine("  - Permissions not declared in manifest");
            sb.AppendLine("  - User denied permission");
            sb.AppendLine("  - Location services disabled (Android < 12)");
            sb.AppendLine("Recommendation: Check platform setup guide");
        }

        return sb.ToString();
    }
}
```

---

## Debugging Checklist

When troubleshooting issues, work through this checklist:

### Before Connecting
- [ ] Bluetooth is enabled: `_scanner.BluetoothState == BluetoothState.On`
- [ ] Permissions granted: `await _scanner.HasScannerPermissionsAsync()`
- [ ] Device found in scan: `_scanner.GetDeviceOrDefault(id) != null`
- [ ] Device RSSI is reasonable: `device.Rssi > -90`

### During Connection
- [ ] Connection options configured correctly
- [ ] Retry logic enabled
- [ ] No concurrent connection attempts to same device
- [ ] Timeout is sufficient: `DefaultOperationTimeout`

### After Connection
- [ ] Device is connected: `device.IsConnected`
- [ ] Services explored: `device.GetServices().Count > 0`
- [ ] Characteristics have correct properties: `CanRead`, `CanWrite`, `CanNotify`
- [ ] CCCD descriptor exists for notifications

### Logging
- [ ] Verbose logging enabled: `EnableVerboseLogging = true`
- [ ] Log level set to Debug or Trace
- [ ] Logs captured to file or console
- [ ] EventIds filtered for relevant operations

### Platform-Specific
- [ ] **Android:** Manifest permissions correct for API level
- [ ] **Android:** Location services enabled (API < 31)
- [ ] **iOS:** Info.plist usage description exists
- [ ] **Windows:** Bluetooth capability declared

---

## Next Steps

- [Common Issues](./Common-Issues.md) - Solutions to frequent problems
- [Logging Documentation](../Advanced/Logging.md) - Comprehensive logging guide
- [Platform Setup](../Getting-Started/Platform-Setup.md) - Platform configuration
- [Error Handling](../Best-Practices/Error-Handling.md) - Exception handling strategies

---

## Getting Help

If you're still stuck after debugging:

1. **Enable verbose logging** and capture logs during the issue
2. **Use native tools** (nRF Connect, LightBlue) to verify device behavior
3. **Create a minimal reproduction** isolating the problem
4. **Check existing issues** on GitHub
5. **Open a new issue** with:
   - Platform and OS version
   - Device information
   - Complete logs with EventIds
   - HCI snoop log (Android) or PacketLogger capture (iOS/macOS)
   - Code sample reproducing the issue
   - Steps to reproduce

The more diagnostic information you provide, the faster the issue can be resolved.
