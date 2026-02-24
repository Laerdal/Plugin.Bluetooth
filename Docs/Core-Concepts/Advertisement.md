# Advertisement

## Overview

An **Advertisement** is a data packet that BLE devices broadcast to announce their presence and provide information without requiring a connection. Think of it as a beacon or radio broadcast that says "I'm here, and this is what I offer!"

**Interface:** `IBluetoothAdvertisement`

## What Does It Do?

Advertisements allow you to:
- Discover nearby BLE devices
- Read device name, address, and manufacturer information
- Check signal strength (RSSI) to estimate distance
- See what services a device offers
- Determine if a device is connectable
- Receive manufacturer-specific data

## Why Advertisements Matter

Advertisements are crucial because:
- **No connection required**: Read basic device info without connecting
- **Battery efficient**: Devices can broadcast without being connected
- **Quick discovery**: Find devices rapidly by listening for broadcasts
- **Proximity detection**: Use signal strength to estimate distance
- **Filtering**: Identify devices of interest before connecting

## Getting Started

### 1. Listen for Advertisements

```csharp
var scanner = BluetoothFactory.Current.Scanner;

scanner.AdvertisementReceived += (sender, args) =>
{
    IBluetoothAdvertisement ad = args.Advertisement;
    IBluetoothRemoteDevice device = args.Device;

    Console.WriteLine($"Device: {ad.DeviceName}");
    Console.WriteLine($"Address: {ad.BluetoothAddress}");
    Console.WriteLine($"Signal: {ad.RawSignalStrengthInDBm} dBm");
};

await scanner.StartScanningAsync();
```

### 2. Access Advertisement Properties

```csharp
scanner.AdvertisementReceived += (sender, args) =>
{
    var ad = args.Advertisement;

    // Basic information
    string name = ad.DeviceName;
    string address = ad.BluetoothAddress;
    DateTimeOffset timestamp = ad.DateReceived;

    // Signal strength
    int rssi = ad.RawSignalStrengthInDBm;
    int txPower = ad.TransmitPowerLevelInDBm;

    // Services
    IEnumerable<Guid> services = ad.ServicesGuids;

    // Connection capability
    bool canConnect = ad.IsConnectable;

    // Manufacturer data
    ReadOnlyMemory<byte> mfgData = ad.ManufacturerData;
    Manufacturer manufacturer = ad.Manufacturer;
    int manufacturerId = ad.ManufacturerId;
};
```

## Advertisement Properties

### Basic Information

```csharp
// Device name (if available)
string name = advertisement.DeviceName;  // e.g., "MyDevice", "" if not provided

// Bluetooth address
string address = advertisement.BluetoothAddress;  // e.g., "00:11:22:33:44:55" or device-specific ID

// Timestamp when advertisement was received
DateTimeOffset timestamp = advertisement.DateReceived;
```

### Signal Strength (RSSI)

RSSI (Received Signal Strength Indicator) indicates how strong the signal is:

```csharp
int rssi = advertisement.RawSignalStrengthInDBm;  // e.g., -65
int txPower = advertisement.TransmitPowerLevelInDBm;  // Advertised TX power

// Estimate distance using RSSI
string GetSignalQuality(int rssi)
{
    return rssi switch
    {
        >= -50 => "Excellent (< 1m)",
        >= -70 => "Good (1-5m)",
        >= -90 => "Weak (5-15m)",
        _ => "Very Weak (> 15m)"
    };
}

Console.WriteLine($"Signal: {rssi} dBm - {GetSignalQuality(rssi)}");
```

**RSSI Guide:**
- `-30 to -50 dBm`: Excellent signal (device is very close, < 1 meter)
- `-50 to -70 dBm`: Good signal (device is nearby, 1-5 meters)
- `-70 to -90 dBm`: Weak signal (device is far, 5-15 meters)
- `-90 to -100 dBm`: Very weak signal (device is at edge of range, > 15 meters)

### Services

Devices advertise services they support:

```csharp
IEnumerable<Guid> services = advertisement.ServicesGuids;

// Check if device advertises specific service
var heartRateServiceUuid = Guid.Parse("0000180D-0000-1000-8000-00805F9B34FB");
bool hasHeartRate = services.Contains(heartRateServiceUuid);

// List all advertised services
foreach (var serviceUuid in services)
{
    Console.WriteLine($"Service: {serviceUuid}");
}
```

**Note**: Not all services are typically advertised - devices usually only advertise their primary services to save bandwidth.

### Connectability

```csharp
bool isConnectable = advertisement.IsConnectable;

if (isConnectable)
    Console.WriteLine("Device accepts connections");
else
    Console.WriteLine("Device is not connectable (broadcast-only mode)");
```

### Manufacturer Data

Manufacturer-specific data in the advertisement:

```csharp
// Raw manufacturer data
ReadOnlyMemory<byte> mfgData = advertisement.ManufacturerData;

// Manufacturer enum (Apple, Microsoft, etc.)
Manufacturer manufacturer = advertisement.Manufacturer;

// Manufacturer company ID (Bluetooth SIG assigned)
int manufacturerId = advertisement.ManufacturerId;

// Example: Check for Apple devices
if (manufacturer == Manufacturer.Apple)
{
    Console.WriteLine("This is an Apple device");
}

// Access raw bytes
if (mfgData.Length > 0)
{
    Console.WriteLine($"Manufacturer data: {BitConverter.ToString(mfgData.ToArray())}");
}
```

**Common Manufacturer IDs:**
- Apple: `0x004C` (76)
- Microsoft: `0x0006` (6)
- Samsung: `0x0075` (117)
- Nordic Semiconductor: `0x0059` (89)

## Common Patterns

### Find Device by Name

```csharp
async Task<IBluetoothRemoteDevice> FindDeviceByNameAsync(string targetName)
{
    var scanner = BluetoothFactory.Current.Scanner;
    IBluetoothRemoteDevice foundDevice = null;

    var handler = new EventHandler<AdvertisementReceivedEventArgs>((s, args) =>
    {
        if (args.Advertisement.DeviceName == targetName)
        {
            foundDevice = args.Device;
        }
    });

    scanner.AdvertisementReceived += handler;
    await scanner.StartScanningAsync();

    // Wait up to 10 seconds
    var timeout = DateTime.UtcNow.AddSeconds(10);
    while (foundDevice == null && DateTime.UtcNow < timeout)
    {
        await Task.Delay(100);
    }

    scanner.AdvertisementReceived -= handler;
    await scanner.StopScanningAsync();

    return foundDevice;
}
```

### Find Closest Device

```csharp
async Task<IBluetoothRemoteDevice> FindClosestDeviceAsync(TimeSpan scanDuration)
{
    var scanner = BluetoothFactory.Current.Scanner;
    IBluetoothRemoteDevice closestDevice = null;
    int strongestRssi = int.MinValue;

    scanner.AdvertisementReceived += (s, args) =>
    {
        int rssi = args.Advertisement.RawSignalStrengthInDBm;

        if (rssi > strongestRssi)
        {
            strongestRssi = rssi;
            closestDevice = args.Device;
        }
    };

    await scanner.StartScanningAsync();
    await Task.Delay(scanDuration);
    await scanner.StopScanningAsync();

    Console.WriteLine($"Closest device: {closestDevice} (RSSI: {strongestRssi})");
    return closestDevice;
}
```

### Filter by Service

```csharp
async Task FindHeartRateMonitorsAsync()
{
    var scanner = BluetoothFactory.Current.Scanner;
    var heartRateServiceUuid = Guid.Parse("0000180D-0000-1000-8000-00805F9B34FB");

    scanner.AdvertisementReceived += (s, args) =>
    {
        var ad = args.Advertisement;

        if (ad.ServicesGuids.Contains(heartRateServiceUuid))
        {
            Console.WriteLine($"Found heart rate monitor: {ad.DeviceName}");
        }
    };

    // Or use ScanningOptions to filter at system level
    await scanner.StartScanningAsync(new ScanningOptions
    {
        ServiceUuids = new[] { heartRateServiceUuid }
    });
}
```

### Monitor Signal Strength

```csharp
async Task MonitorProximityAsync(string deviceName)
{
    var scanner = BluetoothFactory.Current.Scanner;

    scanner.AdvertisementReceived += (s, args) =>
    {
        var ad = args.Advertisement;

        if (ad.DeviceName == deviceName)
        {
            int rssi = ad.RawSignalStrengthInDBm;
            string proximity = rssi switch
            {
                >= -50 => "Very Close",
                >= -70 => "Close",
                >= -90 => "Far",
                _ => "Very Far"
            };

            Console.WriteLine($"{deviceName}: {rssi} dBm ({proximity})");
        }
    };

    await scanner.StartScanningAsync(new ScanningOptions
    {
        IgnoreDuplicateAdvertisements = false  // Get all updates
    });
}
```

### Log Advertisement Details

```csharp
void LogAdvertisement(IBluetoothAdvertisement ad)
{
    Console.WriteLine("=== Advertisement ===");
    Console.WriteLine($"Device Name: {ad.DeviceName}");
    Console.WriteLine($"Address: {ad.BluetoothAddress}");
    Console.WriteLine($"RSSI: {ad.RawSignalStrengthInDBm} dBm");
    Console.WriteLine($"TX Power: {ad.TransmitPowerLevelInDBm} dBm");
    Console.WriteLine($"Is Connectable: {ad.IsConnectable}");
    Console.WriteLine($"Timestamp: {ad.DateReceived}");

    Console.WriteLine($"Services ({ad.ServicesGuids.Count()}):");
    foreach (var service in ad.ServicesGuids)
        Console.WriteLine($"  - {service}");

    Console.WriteLine($"Manufacturer: {ad.Manufacturer} (ID: {ad.ManufacturerId})");

    if (ad.ManufacturerData.Length > 0)
    {
        Console.WriteLine($"Manufacturer Data: {BitConverter.ToString(ad.ManufacturerData.ToArray())}");
    }
}
```

### Detect iBeacons (Apple)

```csharp
scanner.AdvertisementReceived += (s, args) =>
{
    var ad = args.Advertisement;

    if (ad.Manufacturer == Manufacturer.Apple && ad.ManufacturerData.Length >= 23)
    {
        var data = ad.ManufacturerData.Span;

        // iBeacon format check
        if (data[0] == 0x02 && data[1] == 0x15)
        {
            // Extract UUID (16 bytes)
            var uuid = new Guid(data.Slice(2, 16).ToArray());

            // Extract Major (2 bytes)
            ushort major = (ushort)((data[18] << 8) | data[19]);

            // Extract Minor (2 bytes)
            ushort minor = (ushort)((data[20] << 8) | data[21]);

            // Extract TX Power (1 byte, signed)
            sbyte txPower = (sbyte)data[22];

            Console.WriteLine($"iBeacon: {uuid}, Major: {major}, Minor: {minor}, TX: {txPower}");
        }
    }
};
```

## Filtering Advertisements

You can filter advertisements in two ways:

### 1. Scanner-Level Filtering (Recommended)

```csharp
await scanner.StartScanningAsync(new ScanningOptions
{
    // Filter by service UUID
    ServiceUuids = new[] { myServiceUuid },

    // Ignore devices without names
    IgnoreNamelessAdvertisements = true,

    // Only report each device once
    IgnoreDuplicateAdvertisements = true,

    // Signal strength threshold
    RssiThreshold = -70,  // Only nearby devices

    // Custom filter
    AdvertisementFilter = ad =>
        ad.DeviceName.Contains("Sensor") &&
        ad.RawSignalStrengthInDBm > -80
});
```

### 2. Event Handler Filtering

```csharp
scanner.AdvertisementReceived += (s, args) =>
{
    var ad = args.Advertisement;

    // Filter in code
    if (ad.DeviceName.StartsWith("My") && ad.IsConnectable)
    {
        // Process advertisement
    }
};
```

**Recommendation**: Use scanner-level filtering when possible for better performance and battery life.

## Best Practices

1. **Filter Early**: Use `ScanningOptions` to filter at the system level
   ```csharp
   new ScanningOptions
   {
       ServiceUuids = new[] { myServiceUuid },
       RssiThreshold = -70
   }
   ```

2. **Handle Missing Data**: Not all fields are always present
   ```csharp
   string name = ad.DeviceName ?? "Unknown Device";
   ```

3. **Use Signal Strength Wisely**: RSSI varies with environment and obstacles
   - Metal objects and walls weaken signal
   - RSSI fluctuates - average multiple readings for accuracy
   - Different devices have different transmit powers

4. **Don't Trust Timestamps Absolutely**: On some platforms, timestamps may be approximations

5. **Cache Device List**: Track seen devices to avoid processing duplicates
   ```csharp
   var seenDevices = new HashSet<string>();

   scanner.AdvertisementReceived += (s, args) =>
   {
       if (seenDevices.Add(args.Advertisement.BluetoothAddress))
       {
           // First time seeing this device
       }
   };
   ```

6. **Understand Manufacturer Data Format**: Consult device documentation for interpretation

## Troubleshooting

### Empty Device Name

- Some devices don't advertise names
- Name might be in the scan response (enable `ScanningOptions.IgnoreNamelessAdvertisements = false`)
- Connect to device and read from GAP service to get name

### No Services Listed

- Devices may not advertise all services
- Only primary services are typically advertised
- Connect and explore to discover all services

### Signal Strength Fluctuations

- RSSI naturally varies due to radio interference
- Average multiple readings for more stable values
- Use signal strength smoothing in `ScanningOptions`

### Manufacturer Data Empty

- Not all devices provide manufacturer data
- Check if device actually sends this information
- Manufacturer data is optional in BLE spec

### Advertisements Not Received

- Ensure scanner is running: `scanner.IsRunning`
- Check Bluetooth permissions
- Verify device is advertising (some devices only advertise when in pairing mode)
- Check if filtering is too restrictive

## Related Topics

- [Scanner](./Scanner.md) - Discovering devices via advertisements
- [Device](./Device.md) - Connecting to advertised devices
- [Broadcaster](./Broadcaster.md) - Creating your own advertisements
