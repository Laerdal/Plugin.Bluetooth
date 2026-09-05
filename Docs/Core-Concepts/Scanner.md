# Scanner

## Overview

The **Scanner** is your entry point to discovering Bluetooth Low Energy (BLE) devices nearby. Think of it as a radar that continuously listens for BLE advertisement packets broadcast by devices in range.

**Interface:** `IBluetoothScanner`

## What Does It Do?

The Scanner allows you to:
- Start and stop scanning for nearby BLE devices
- Filter devices based on various criteria (name, service UUIDs, signal strength)
- Receive real-time advertisement data from devices
- Manage Bluetooth permissions

## Basic Workflow

```
┌─────────────┐      ┌──────────────┐      ┌────────────────┐
│   Request   │─────▶│    Start     │─────▶│   Receive      │
│ Permissions │      │   Scanning   │      │ Advertisements │
└─────────────┘      └──────────────┘      └────────────────┘
                                                    │
                                                    ▼
                                            ┌────────────────┐
                                            │  Stop Scanning │
                                            └────────────────┘
```

## Getting Started

### 1. Request Permissions

Before scanning, you need the user's permission to use Bluetooth:

```csharp
// scanner is an IBluetoothScanner obtained via constructor injection
// (see Docs/Configuration/Dependency-Injection.md) — it's registered as a
// singleton, so the same instance is shared across your app.

// Check if we already have permissions
bool hasPermission = await scanner.HasScannerPermissionsAsync();

if (!hasPermission)
{
    // Request permissions from the user
    await scanner.RequestScannerPermissionsAsync();
}
```

**Platform Notes:**
- **Android**: Requests `BLUETOOTH_SCAN` permission (API 31+) or location permissions (older versions)
- **iOS/macOS**: Requests "Bluetooth Always" permission
- **Windows**: Checks adapter availability and radio state

### 2. Start Scanning

Once you have permissions, start scanning:

```csharp
// Start scanning with default settings
await scanner.StartScanningAsync();

// Or with custom options
var options = new ScanningOptions
{
    ScanMode = BluetoothScanMode.LowLatency, // Fast discovery
    IgnoreDuplicateAdvertisements = false,    // Get all updates
    ServiceUuids = new[] { myServiceGuid }    // Filter by service
};

await scanner.StartScanningAsync(options);
```

### 3. Listen for Advertisements

```csharp
scanner.AdvertisementReceived += (sender, args) =>
{
    IBluetoothAdvertisement ad = args.Advertisement;

    Console.WriteLine($"Found: {ad.DeviceName}");
    Console.WriteLine($"Signal: {ad.SignalStrengthInDBm} dBm");
    Console.WriteLine($"Address: {ad.BluetoothAddress}");
};
```

### 4. Stop Scanning

```csharp
await scanner.StopScanningAsync();
```

## Scanning Options

The `ScanningOptions` class lets you customize your scanning behavior:

### Basic Filtering

```csharp
var options = new ScanningOptions
{
    // Ignore devices without a name
    IgnoreNamelessAdvertisements = true,

    // Only report each device once (not every advertisement)
    IgnoreDuplicateAdvertisements = true,

    // Custom filter function
    AdvertisementFilter = ad =>
        ad.DeviceName.Contains("Sensor") &&
        ad.SignalStrengthInDBm > -70
};
```

### Service UUID Filtering

Only discover devices advertising specific services:

```csharp
var options = new ScanningOptions
{
    ServiceUuids = new[]
    {
        Guid.Parse("0000180F-0000-1000-8000-00805F9B34FB"), // Battery Service
        Guid.Parse("0000180A-0000-1000-8000-00805F9B34FB")  // Device Info
    }
};
```

### Scan Mode (Power vs Speed)

Choose the right balance for your use case:

```csharp
// Fast discovery, higher power consumption
BluetoothScanMode.LowLatency

// Balanced performance (default)
BluetoothScanMode.Balanced

// Battery friendly, slower discovery
BluetoothScanMode.LowPower

// Only scan when other apps are scanning (Android only)
BluetoothScanMode.Opportunistic
```

### Signal Strength Filtering

Only detect nearby devices:

```csharp
var options = new ScanningOptions
{
    RssiThreshold = -70  // Typical values: -100 (far) to -30 (very close)
};
```

**RSSI Guide:**
- `-30 to -50 dBm`: Excellent signal (very close)
- `-50 to -70 dBm`: Good signal (nearby)
- `-70 to -90 dBm`: Weak signal (far)
- `-90 to -100 dBm`: Very weak signal (edge of range)

## Events

The Scanner provides several events to track its state:

```csharp
// Scanning lifecycle events
scanner.Starting += (s, e) => Console.WriteLine("Scanner is starting...");
scanner.Started += (s, e) => Console.WriteLine("Scanner started!");
scanner.Stopping += (s, e) => Console.WriteLine("Scanner is stopping...");
scanner.Stopped += (s, e) => Console.WriteLine("Scanner stopped!");
scanner.RunningStateChanged += (s, e) => Console.WriteLine($"Running: {scanner.IsRunning}");

// Advertisement received
scanner.AdvertisementReceived += (s, args) =>
{
    // Process advertisement
};
```

## State Properties

Monitor the scanner's current state:

```csharp
bool isRunning = scanner.IsRunning;      // Is actively scanning?
bool isStarting = scanner.IsStarting;    // Is currently starting?
bool isStopping = scanner.IsStopping;    // Is currently stopping?
```

## Advanced Features

### Dynamic Options Updates

Change scanning options without stopping:

```csharp
await scanner.UpdateScannerOptionsAsync(new ScanningOptions
{
    ScanMode = BluetoothScanMode.LowPower  // Switch to battery-saving mode
});
```

### Safe Start/Stop

Use the "IfNeeded" variants to avoid exceptions:

```csharp
// Only starts if not already running
await scanner.StartScanningIfNeededAsync();

// Only stops if currently running
await scanner.StopScanningIfNeededAsync();
```

### Clean Restart

`CleanRestartScanningAsync` stops the scanner, discards every device in the registry, then starts scanning again:

```csharp
await scanner.CleanRestartScanningAsync();
```

Use it when a device changes identity while the scanner is running - typically a device rebooting into or out of
firmware-update mode, where a stale registry entry plus an in-flight scan session stop it from being rediscovered under
its new advertisement. It also accepts a replacement advertisement filter, applied while the scanner is stopped so it is
already in effect when scanning resumes:

```csharp
await scanner.CleanRestartScanningAsync(ad => ad.DeviceName.Contains("DfuTarg"));
```

Every device is disconnected, removed and disposed, so `IBluetoothRemoteDevice` instances do not survive the restart.
Keep the device id instead and re-acquire the instance afterwards:

```csharp
var deviceId = device.Id;
await scanner.CleanRestartScanningAsync();
device = await scanner.WaitForDeviceToAppearAsync(deviceId, TimeSpan.FromSeconds(30));
```

### Timeouts and Cancellation

All operations support timeouts and cancellation:

```csharp
var cts = new CancellationTokenSource();
cts.CancelAfter(TimeSpan.FromSeconds(10));

try
{
    await scanner.StartScanningAsync(
        options: null,
        timeout: TimeSpan.FromSeconds(5),
        cancellationToken: cts.Token
    );
}
catch (TimeoutException)
{
    Console.WriteLine("Scanner took too long to start");
}
catch (OperationCanceledException)
{
    Console.WriteLine("Scan was cancelled");
}
```

## Common Patterns

### Simple Device Discovery

```csharp
// The scanner already tracks discovered devices for you — no need to
// collect them manually from the event args.
scanner.AdvertisementReceived += (s, args) =>
{
    Console.WriteLine($"Discovered: {args.Advertisement.DeviceName}");
};

await scanner.StartScanningAsync(new ScanningOptions
{
    IgnoreDuplicateAdvertisements = true
});

// Scan for 10 seconds
await Task.Delay(TimeSpan.FromSeconds(10));
await scanner.StopScanningAsync();

var foundDevices = scanner.GetDevices();
Console.WriteLine($"Found {foundDevices.Count} devices");
```

### Find Specific Device

```csharp
var targetDevice = await FindDeviceByNameAsync("MySensor");

async Task<IBluetoothRemoteDevice> FindDeviceByNameAsync(string name)
{
    // WaitForDeviceToAppearAsync does the wait-with-timeout/filter dance for you.
    await scanner.StartScanningAsync();

    try
    {
        return await scanner.WaitForDeviceToAppearAsync(
            device => device.Name == name,
            timeout: TimeSpan.FromSeconds(30));
    }
    finally
    {
        await scanner.StopScanningAsync();
    }
}
```

### Monitor Signal Strength

```csharp
scanner.AdvertisementReceived += (s, args) =>
{
    var rssi = args.Advertisement.SignalStrengthInDBm;
    var quality = rssi switch
    {
        >= -50 => "Excellent",
        >= -70 => "Good",
        >= -90 => "Weak",
        _ => "Very Weak"
    };

    Console.WriteLine($"{args.Advertisement.DeviceName}: {rssi} dBm ({quality})");
};
```

## Best Practices

1. **Always Stop Scanning**: Scanning drains battery - stop when you're done
   ```csharp
   try
   {
       await scanner.StartScanningAsync();
       // Do work...
   }
   finally
   {
       await scanner.StopScanningIfNeededAsync();
   }
   ```

2. **Use Appropriate Scan Mode**: Choose based on your needs
   - Quick discovery? Use `LowLatency`
   - Background monitoring? Use `LowPower`
   - Most cases? Use `Balanced` (default)

3. **Filter Early**: Use `ScanningOptions` to filter at the system level rather than in your handler

4. **Handle Permissions Properly**: Always check and request permissions before scanning

5. **Don't Dispose It Yourself**: `IBluetoothScanner` is registered as a DI singleton
   (`services.AddSingleton<IBluetoothScanner, BluetoothScanner>()`), so the container
   owns its lifetime and disposal — just take it via constructor injection and call
   `StopScanningAsync()`/`StopScanningIfNeededAsync()` when you're done scanning.

## Troubleshooting

### No Devices Found

- Check permissions are granted
- Ensure Bluetooth is enabled on the device
- Move closer to the target device
- Check if service UUID filtering is too restrictive
- Try `ScanMode.LowLatency` for faster discovery

### Too Many Duplicate Advertisements

Set `IgnoreDuplicateAdvertisements = true` in `ScanningOptions`

### Battery Drain

- Use `ScanMode.LowPower`
- Stop scanning when not needed
- Use service UUID filtering to reduce processing

### Scanner Won't Start

- Check permissions: `await scanner.HasScannerPermissionsAsync()`
- Ensure Bluetooth adapter is available
- Check if already running: `scanner.IsRunning`

## Related Topics

- [Device](./Device.md) - Connect to discovered devices
- [Advertisement](./Advertisement.md) - Understanding advertisement data
- [Service](./Service.md) - Explore device services after connection
