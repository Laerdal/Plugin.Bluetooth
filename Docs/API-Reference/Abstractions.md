# Interfaces and Abstractions

Complete reference for all Plugin.Bluetooth interfaces organized by namespace.

## Table of Contents

- [Bluetooth.Abstractions](#bluetoothabstractions)
  - [IBluetoothAdapter](#ibluetoothadapter)
- [Bluetooth.Abstractions.Scanning](#bluetoothabstractionsscanning)
  - [IBluetoothScanner](#ibletoothscanner)
  - [IBluetoothRemoteDevice](#ibletoothremotedevice)
  - [IBluetoothRemoteService](#ibletoothremoteservice)
  - [IBluetoothRemoteCharacteristic](#ibletoothremotecharacteristic)
  - [IBluetoothRemoteDescriptor](#ibletoothremotedescriptor)
  - [IBluetoothL2CapChannel](#ibletoothL2capchannel)
  - [IBluetoothPairingManager](#ibletoothpairingmanager)
  - [IBluetoothAdvertisement](#ibluetoothadvertisement)
- [Bluetooth.Abstractions.Broadcasting](#bluetoothabstractionsbroadcasting)
  - [IBluetoothBroadcaster](#ibletoothbroadcaster)
  - [IBluetoothLocalService](#ibletoothLocalService)
  - [IBluetoothLocalCharacteristic](#ibletoothLocalCharacteristic)
  - [IBluetoothLocalDescriptor](#ibletoothlocaldescriptor)
  - [IBluetoothConnectedDevice](#ibletoothconnecteddevice)

---

## Bluetooth.Abstractions

Core namespace containing the main adapter interface and shared types.

### IBluetoothAdapter

**Namespace:** `Bluetooth.Abstractions`

Main entry point for accessing Bluetooth functionality. Provides factory methods for creating scanners and broadcasters.

#### Properties

```csharp
// Adapter state
bool IsAvailable { get; }
bool IsEnabled { get; }
string? Name { get; }
string? Address { get; }
```

#### Methods

```csharp
// Scanner creation (Central role)
IBluetoothScanner CreateScanner();

// Broadcaster creation (Peripheral role)
IBluetoothBroadcaster CreateBroadcaster();

// Adapter state management
ValueTask<bool> IsAdapterEnabledAsync();
ValueTask RequestEnableAdapterAsync(CancellationToken cancellationToken = default);
```

#### Usage

```csharp
// Get adapter instance (via dependency injection)
IBluetoothAdapter adapter = serviceProvider.GetRequiredService<IBluetoothAdapter>();

// Check availability
if (!adapter.IsAvailable)
{
    Console.WriteLine("Bluetooth not available on this device");
    return;
}

// Create scanner for central role
await using var scanner = adapter.CreateScanner();
await scanner.StartScanningAsync();

// Create broadcaster for peripheral role
await using var broadcaster = adapter.CreateBroadcaster();
await broadcaster.StartBroadcastingAsync();
```

**Platform support:** All platforms (Android, iOS, macOS, Windows)

---

## Bluetooth.Abstractions.Scanning

Interfaces for Central/Client role - scanning for and connecting to Bluetooth peripherals.

### IBluetoothScanner

**Namespace:** `Bluetooth.Abstractions.Scanning`
**Inherits:** `IAsyncDisposable`

Manages Bluetooth device scanning and discovery.

#### Properties

```csharp
// Scanner state
bool IsRunning { get; }
bool IsStarting { get; }
bool IsStopping { get; }
ScanningOptions CurrentScanningOptions { get; }

// Device collections
IReadOnlyList<IBluetoothRemoteDevice> Devices { get; }
```

#### Events

```csharp
// State events
event EventHandler? RunningStateChanged;
event EventHandler Starting;
event EventHandler Started;
event EventHandler Stopping;
event EventHandler Stopped;

// Advertisement events
event EventHandler<AdvertisementReceivedEventArgs> AdvertisementReceived;

// Device list events
event EventHandler<DeviceListChangedEventArgs> DeviceListChanged;
event EventHandler<DevicesAddedEventArgs> DevicesAdded;
event EventHandler<DevicesRemovedEventArgs> DevicesRemoved;
```

#### Methods

```csharp
// Permissions
ValueTask<bool> HasScannerPermissionsAsync();
ValueTask RequestScannerPermissionsAsync(
    bool requireBackgroundLocation = false,
    CancellationToken cancellationToken = default);

// Scanning control
Task StartScanningAsync(
    ScanningOptions? options = null,
    TimeSpan? timeout = null,
    CancellationToken cancellationToken = default);

ValueTask StartScanningIfNeededAsync(
    ScanningOptions? options = null,
    TimeSpan? timeout = null,
    CancellationToken cancellationToken = default);

Task StopScanningAsync(
    TimeSpan? timeout = null,
    CancellationToken cancellationToken = default);

ValueTask StopScanningIfNeededAsync(
    TimeSpan? timeout = null,
    CancellationToken cancellationToken = default);

// Runtime configuration
ValueTask UpdateScannerOptionsAsync(
    ScanningOptions options,
    TimeSpan? timeout = null,
    CancellationToken cancellationToken = default);

// Device retrieval
ValueTask<IBluetoothRemoteDevice> GetKnownDeviceAsync(
    string id,
    TimeSpan? timeout = null,
    CancellationToken cancellationToken = default);

ValueTask<IBluetoothRemoteDevice> GetKnownDeviceAsync(
    Guid id,
    TimeSpan? timeout = null,
    CancellationToken cancellationToken = default);

// Device querying
ValueTask<IBluetoothRemoteDevice> WaitForDeviceAsync(
    Func<IBluetoothRemoteDevice, bool> predicate,
    TimeSpan? timeout = null,
    CancellationToken cancellationToken = default);

IBluetoothRemoteDevice? TryGetDevice(
    Func<IBluetoothRemoteDevice, bool> predicate);
```

#### Usage

```csharp
await using var scanner = adapter.CreateScanner();

// Request permissions
if (!await scanner.HasScannerPermissionsAsync())
{
    await scanner.RequestScannerPermissionsAsync();
}

// Configure and start scanning
var options = new ScanningOptions
{
    ScanMode = BluetoothScanMode.LowLatency,
    FilterByServices = new[] { serviceUuid }
};

scanner.DevicesAdded += (s, e) =>
{
    foreach (var device in e.Items)
    {
        Console.WriteLine($"Found: {device.Name ?? "Unknown"}");
    }
};

await scanner.StartScanningAsync(options);

// Wait for specific device
var device = await scanner.WaitForDeviceAsync(
    d => d.Name?.Contains("MyDevice") == true,
    timeout: TimeSpan.FromSeconds(30)
);

await scanner.StopScanningAsync();
```

**See also:** [Scanning Concept](../SCANNING.md), [ScanningOptions](./Enums.md#scanningoptions)

---

### IBluetoothRemoteDevice

**Namespace:** `Bluetooth.Abstractions.Scanning`
**Inherits:** `INotifyPropertyChanged`, `IAsyncDisposable`

Represents a remote Bluetooth device (peripheral) with connection management and GATT service access.

#### Properties

```csharp
// Identity
IBluetoothScanner Scanner { get; }
string Id { get; }
Guid Uuid { get; }
string? Name { get; }
string? Address { get; }

// Connection state
bool IsConnected { get; }
bool IsConnecting { get; }
bool IsDisconnecting { get; }
bool IgnoreNextUnexpectedDisconnection { get; set; }

// Signal strength
int Rssi { get; }

// MTU (Maximum Transmission Unit)
int Mtu { get; }

// PHY (Physical Layer)
PhyMode TxPhy { get; }
PhyMode RxPhy { get; }

// Advertisement data
IBluetoothAdvertisement? Advertisement { get; }

// Battery level (if available)
int? BatteryLevel { get; }

// Version information
string? SoftwareVersion { get; }
string? HardwareVersion { get; }
string? FirmwareVersion { get; }
string? ModelNumber { get; }
Manufacturer? Manufacturer { get; }

// Service collections
IReadOnlyList<IBluetoothRemoteService> Services { get; }
```

#### Events

```csharp
// Connection events
event EventHandler Connecting;
event EventHandler Connected;
event EventHandler Disconnecting;
event EventHandler Disconnected;
event EventHandler<DeviceConnectionStateChangedEventArgs> ConnectionStateChanged;
event EventHandler<DeviceUnexpectedDisconnectionEventArgs> UnexpectedDisconnection;

// Service discovery events
event EventHandler<ServiceListChangedEventArgs> ServiceListChanged;
event EventHandler ServicesChanged;

// Advertisement updates
event EventHandler<AdvertisementReceivedEventArgs> AdvertisementReceived;

// RSSI updates
event EventHandler<RssiChangedEventArgs> RssiChanged;

// MTU changes
event EventHandler<MtuChangedEventArgs> MtuChanged;

// PHY changes
event EventHandler<PhyChangedEventArgs> PhyChanged;

// Pairing events
event EventHandler<PairingStateChangedEventArgs> PairingStateChanged;
```

#### Methods

```csharp
// Connection management
ValueTask ConnectAsync(
    ConnectionOptions? connectionOptions = null,
    TimeSpan? timeout = null,
    CancellationToken cancellationToken = default);

ValueTask ConnectIfNeededAsync(
    ConnectionOptions? connectionOptions = null,
    TimeSpan? timeout = null,
    CancellationToken cancellationToken = default);

ValueTask DisconnectAsync(
    TimeSpan? timeout = null,
    CancellationToken cancellationToken = default);

ValueTask DisconnectIfNeededAsync(
    TimeSpan? timeout = null,
    CancellationToken cancellationToken = default);

ValueTask WaitForIsConnectedAsync(
    bool isConnected,
    TimeSpan? timeout = null,
    CancellationToken cancellationToken = default);

// Connection parameters
ValueTask RequestConnectionPriorityAsync(
    BluetoothConnectionPriority priority,
    TimeSpan? timeout = null,
    CancellationToken cancellationToken = default);

// MTU negotiation
ValueTask RequestMtuAsync(
    int mtu,
    TimeSpan? timeout = null,
    CancellationToken cancellationToken = default);

// PHY management
ValueTask SetPreferredPhyAsync(
    PhyMode txPhy,
    PhyMode rxPhy,
    TimeSpan? timeout = null,
    CancellationToken cancellationToken = default);

// Service discovery
ValueTask DiscoverServicesAsync(
    TimeSpan? timeout = null,
    CancellationToken cancellationToken = default);

ValueTask<IBluetoothRemoteService> GetServiceAsync(
    Guid serviceUuid,
    TimeSpan? timeout = null,
    CancellationToken cancellationToken = default);

IBluetoothRemoteService? TryGetService(Guid serviceUuid);

// RSSI reading
ValueTask ReadRssiAsync(
    TimeSpan? timeout = null,
    CancellationToken cancellationToken = default);

// Battery level reading
ValueTask ReadBatteryLevelAsync(
    TimeSpan? timeout = null,
    CancellationToken cancellationToken = default);

// L2CAP channels (advanced)
ValueTask<IBluetoothL2CapChannel> OpenL2CapChannelAsync(
    int psm,
    TimeSpan? timeout = null,
    CancellationToken cancellationToken = default);
```

#### Usage

```csharp
// Connect to device
await device.ConnectAsync(timeout: TimeSpan.FromSeconds(10));

// Handle unexpected disconnections
device.UnexpectedDisconnection += async (s, e) =>
{
    Console.WriteLine($"Lost connection: {e.Reason}");
    await Task.Delay(1000);
    await device.ConnectIfNeededAsync();
};

// Discover services
await device.DiscoverServicesAsync();

// Get specific service
var service = await device.GetServiceAsync(serviceUuid);

// Request larger MTU for better throughput
await device.RequestMtuAsync(512);

// Read RSSI
await device.ReadRssiAsync();
Console.WriteLine($"Signal strength: {device.Rssi} dBm");

// Disconnect when done
await device.DisconnectAsync();
```

**See also:** [Connecting Concept](../CONNECTING.md), [ConnectionOptions](./Enums.md#connectionoptions)

---

### IBluetoothRemoteService

**Namespace:** `Bluetooth.Abstractions.Scanning`
**Inherits:** `INotifyPropertyChanged`, `IAsyncDisposable`

Represents a GATT service on a remote device.

#### Properties

```csharp
IBluetoothRemoteDevice Device { get; }
Guid Id { get; }
string Name { get; }
IReadOnlyList<IBluetoothRemoteCharacteristic> Characteristics { get; }
```

#### Events

```csharp
event EventHandler<CharacteristicListChangedEventArgs> CharacteristicListChanged;
```

#### Methods

```csharp
// Characteristic discovery
ValueTask DiscoverCharacteristicsAsync(
    TimeSpan? timeout = null,
    CancellationToken cancellationToken = default);

ValueTask<IBluetoothRemoteCharacteristic> GetCharacteristicAsync(
    Guid characteristicUuid,
    TimeSpan? timeout = null,
    CancellationToken cancellationToken = default);

IBluetoothRemoteCharacteristic? TryGetCharacteristic(Guid characteristicUuid);
```

#### Usage

```csharp
var service = await device.GetServiceAsync(serviceUuid);

// Discover characteristics
await service.DiscoverCharacteristicsAsync();

// Get specific characteristic
var characteristic = await service.GetCharacteristicAsync(characteristicUuid);

Console.WriteLine($"Service: {service.Name}");
foreach (var char in service.Characteristics)
{
    Console.WriteLine($"  - {char.Name}");
}
```

---

### IBluetoothRemoteCharacteristic

**Namespace:** `Bluetooth.Abstractions.Scanning`
**Inherits:** `INotifyPropertyChanged`, `IAsyncDisposable`

Represents a GATT characteristic on a remote device with read, write, and notify capabilities.

#### Properties

```csharp
// Identity
IBluetoothRemoteService RemoteService { get; }
Guid Id { get; }
string Name { get; }

// Capabilities
bool CanRead { get; }
bool CanWrite { get; }
bool CanWriteWithoutResponse { get; }
bool CanListen { get; }
CharacteristicProperties Properties { get; }

// State
bool IsReading { get; }
bool IsWriting { get; }
bool IsListening { get; }

// Value access
ReadOnlyMemory<byte> Value { get; }
ReadOnlySpan<byte> ValueSpan { get; }

// Descriptors
IReadOnlyList<IBluetoothRemoteDescriptor> Descriptors { get; }
```

#### Events

```csharp
event EventHandler<ValueUpdatedEventArgs> ValueUpdated;
event EventHandler<DescriptorListChangedEventArgs> DescriptorListChanged;
```

#### Methods

```csharp
// Read operations
ValueTask<ReadOnlyMemory<byte>> ReadValueAsync(
    bool skipIfPreviouslyRead = false,
    TimeSpan? timeout = null,
    CancellationToken cancellationToken = default);

// Write operations
ValueTask WriteValueAsync(
    ReadOnlyMemory<byte> value,
    TimeSpan? timeout = null,
    CancellationToken cancellationToken = default);

ValueTask WriteValueWithoutResponseAsync(
    ReadOnlyMemory<byte> value,
    TimeSpan? timeout = null,
    CancellationToken cancellationToken = default);

// Notification/Indication
ValueTask StartListeningAsync(
    TimeSpan? timeout = null,
    CancellationToken cancellationToken = default);

ValueTask StopListeningAsync(
    TimeSpan? timeout = null,
    CancellationToken cancellationToken = default);

ValueTask<ReadOnlyMemory<byte>> WaitForValueChangeAsync(
    Func<ReadOnlyMemory<byte>, bool>? valueFilter = null,
    TimeSpan? timeout = null,
    CancellationToken cancellationToken = default);

// Descriptor discovery
ValueTask DiscoverDescriptorsAsync(
    TimeSpan? timeout = null,
    CancellationToken cancellationToken = default);

ValueTask<IBluetoothRemoteDescriptor> GetDescriptorAsync(
    Guid descriptorUuid,
    TimeSpan? timeout = null,
    CancellationToken cancellationToken = default);

IBluetoothRemoteDescriptor? TryGetDescriptor(Guid descriptorUuid);
```

#### Usage

```csharp
var characteristic = await service.GetCharacteristicAsync(charUuid);

// Read value
if (characteristic.CanRead)
{
    var value = await characteristic.ReadValueAsync();
    Console.WriteLine($"Value: {BitConverter.ToString(value.ToArray())}");
}

// Write value
if (characteristic.CanWrite)
{
    byte[] data = { 0x01, 0x02, 0x03 };
    await characteristic.WriteValueAsync(data);
}

// Subscribe to notifications
if (characteristic.CanListen)
{
    characteristic.ValueUpdated += (s, e) =>
    {
        Console.WriteLine($"New value: {BitConverter.ToString(e.NewValue.ToArray())}");
    };

    await characteristic.StartListeningAsync();

    // Wait for specific value
    var result = await characteristic.WaitForValueChangeAsync(
        value => value.Span[0] == 0xFF,
        timeout: TimeSpan.FromSeconds(10)
    );

    await characteristic.StopListeningAsync();
}
```

**See also:** [Characteristic Interaction](../CHARACTERISTICS.md)

---

### IBluetoothRemoteDescriptor

**Namespace:** `Bluetooth.Abstractions.Scanning`
**Inherits:** `INotifyPropertyChanged`, `IAsyncDisposable`

Represents a GATT descriptor on a remote characteristic.

#### Properties

```csharp
IBluetoothRemoteCharacteristic RemoteCharacteristic { get; }
Guid Id { get; }
string Name { get; }
ReadOnlyMemory<byte> Value { get; }
ReadOnlySpan<byte> ValueSpan { get; }
```

#### Methods

```csharp
ValueTask<ReadOnlyMemory<byte>> ReadValueAsync(
    TimeSpan? timeout = null,
    CancellationToken cancellationToken = default);

ValueTask WriteValueAsync(
    ReadOnlyMemory<byte> value,
    TimeSpan? timeout = null,
    CancellationToken cancellationToken = default);
```

#### Usage

```csharp
var descriptor = await characteristic.GetDescriptorAsync(descriptorUuid);

// Read descriptor value
var value = await descriptor.ReadValueAsync();

// Write descriptor value
await descriptor.WriteValueAsync(new byte[] { 0x01, 0x00 });
```

---

### IBluetoothL2CapChannel

**Namespace:** `Bluetooth.Abstractions.Scanning`
**Inherits:** `IAsyncDisposable`

Represents a Logical Link Control and Adaptation Protocol (L2CAP) channel for advanced data streaming.

#### Properties

```csharp
IBluetoothRemoteDevice Device { get; }
int Psm { get; }
bool IsOpen { get; }
int Mtu { get; }
```

#### Events

```csharp
event EventHandler<L2CapDataReceivedEventArgs> DataReceived;
event EventHandler Closed;
```

#### Methods

```csharp
ValueTask WriteAsync(
    ReadOnlyMemory<byte> data,
    TimeSpan? timeout = null,
    CancellationToken cancellationToken = default);

ValueTask CloseAsync(
    TimeSpan? timeout = null,
    CancellationToken cancellationToken = default);
```

#### Usage

```csharp
// Open L2CAP channel
var channel = await device.OpenL2CapChannelAsync(psm: 0x0025);

channel.DataReceived += (s, e) =>
{
    Console.WriteLine($"Received {e.Data.Length} bytes");
};

// Write data
await channel.WriteAsync(data);

// Close channel
await channel.CloseAsync();
```

**Platform support:** Android 10+, iOS 11+, macOS 10.13+

**See also:** [L2CAP Documentation](../L2CAP_ADDITIONAL_OPTIONS.md)

---

### IBluetoothPairingManager

**Namespace:** `Bluetooth.Abstractions.Scanning`
**Inherits:** `IAsyncDisposable`

Manages device pairing and bonding operations.

#### Properties

```csharp
IBluetoothRemoteDevice Device { get; }
bool IsPaired { get; }
bool IsBonded { get; }
```

#### Methods

```csharp
ValueTask PairAsync(
    TimeSpan? timeout = null,
    CancellationToken cancellationToken = default);

ValueTask UnpairAsync(
    TimeSpan? timeout = null,
    CancellationToken cancellationToken = default);
```

#### Usage

```csharp
var pairingManager = device.GetPairingManager();

if (!pairingManager.IsPaired)
{
    await pairingManager.PairAsync();
}
```

**Platform differences:**
- Android: Supports both pairing and bonding
- iOS/macOS: Pairing is system-managed
- Windows: Supports pairing with UI

---

### IBluetoothAdvertisement

**Namespace:** `Bluetooth.Abstractions.Scanning`

Represents Bluetooth advertisement data received from a peripheral.

#### Properties

```csharp
string? LocalName { get; }
int? TxPowerLevel { get; }
IReadOnlyList<Guid> ServiceUuids { get; }
IReadOnlyDictionary<Guid, ReadOnlyMemory<byte>> ServiceData { get; }
IReadOnlyDictionary<ushort, ReadOnlyMemory<byte>> ManufacturerData { get; }
bool IsConnectable { get; }
```

#### Usage

```csharp
device.AdvertisementReceived += (s, e) =>
{
    var ad = e.Advertisement;
    Console.WriteLine($"Device: {ad.LocalName}");
    Console.WriteLine($"Services: {string.Join(", ", ad.ServiceUuids)}");

    foreach (var (company, data) in ad.ManufacturerData)
    {
        Console.WriteLine($"Manufacturer {company}: {BitConverter.ToString(data.ToArray())}");
    }
};
```

---

## Bluetooth.Abstractions.Broadcasting

Interfaces for Peripheral/Server role - advertising services and handling client connections.

### IBluetoothBroadcaster

**Namespace:** `Bluetooth.Abstractions.Broadcasting`
**Inherits:** `IAsyncDisposable`

Manages Bluetooth broadcasting (advertising) and GATT server functionality.

#### Properties

```csharp
// Broadcaster reference
IBluetoothAdapter Adapter { get; }

// State
bool IsRunning { get; }
bool IsStarting { get; }
bool IsStopping { get; }
BroadcastingOptions CurrentBroadcastingOptions { get; }

// Service collections
IReadOnlyList<IBluetoothLocalService> Services { get; }

// Connected clients
IReadOnlyList<IBluetoothConnectedDevice> ConnectedDevices { get; }
```

#### Events

```csharp
// State events
event EventHandler? RunningStateChanged;
event EventHandler Starting;
event EventHandler Started;
event EventHandler Stopping;
event EventHandler Stopped;

// Service management events
event EventHandler<ServiceListChangedEventArgs> ServiceListChanged;

// Client connection events
event EventHandler<ClientDeviceListChangedEventArgs> ClientDeviceListChanged;
event EventHandler<ClientConnectionStateChangedEventArgs> ClientConnected;
event EventHandler<ClientConnectionStateChangedEventArgs> ClientDisconnected;
```

#### Methods

```csharp
// Permissions
ValueTask<bool> HasBroadcasterPermissionsAsync();
ValueTask RequestBroadcasterPermissionsAsync(
    CancellationToken cancellationToken = default);

// Broadcasting control
ValueTask StartBroadcastingAsync(
    BroadcastingOptions? options = null,
    TimeSpan? timeout = null,
    CancellationToken cancellationToken = default);

ValueTask StartBroadcastingIfNeededAsync(
    BroadcastingOptions? options = null,
    TimeSpan? timeout = null,
    CancellationToken cancellationToken = default);

ValueTask StopBroadcastingAsync(
    TimeSpan? timeout = null,
    CancellationToken cancellationToken = default);

ValueTask StopBroadcastingIfNeededAsync(
    TimeSpan? timeout = null,
    CancellationToken cancellationToken = default);

// Runtime configuration
ValueTask UpdateBroadcastingOptionsAsync(
    BroadcastingOptions options,
    TimeSpan? timeout = null,
    CancellationToken cancellationToken = default);

// Service management
ValueTask<IBluetoothLocalService> AddServiceAsync(
    Guid serviceUuid,
    bool isPrimary = true,
    TimeSpan? timeout = null,
    CancellationToken cancellationToken = default);

ValueTask RemoveServiceAsync(
    IBluetoothLocalService service,
    TimeSpan? timeout = null,
    CancellationToken cancellationToken = default);

ValueTask<IBluetoothLocalService> GetServiceAsync(
    Guid serviceUuid,
    TimeSpan? timeout = null,
    CancellationToken cancellationToken = default);

IBluetoothLocalService? TryGetService(Guid serviceUuid);

// Client device access
ValueTask<IBluetoothConnectedDevice> GetConnectedDeviceAsync(
    string deviceId,
    TimeSpan? timeout = null,
    CancellationToken cancellationToken = default);

IBluetoothConnectedDevice? TryGetConnectedDevice(string deviceId);
```

#### Usage

```csharp
await using var broadcaster = adapter.CreateBroadcaster();

// Request permissions
if (!await broadcaster.HasBroadcasterPermissionsAsync())
{
    await broadcaster.RequestBroadcasterPermissionsAsync();
}

// Configure broadcasting
var options = new BroadcastingOptions
{
    LocalName = "MyPeripheral",
    Connectable = true,
    AdvertiseMode = BluetoothAdvertiseMode.LowLatency
};

// Add service
var service = await broadcaster.AddServiceAsync(serviceUuid);
var characteristic = await service.AddCharacteristicAsync(
    charUuid,
    BluetoothCharacteristicProperties.Read | BluetoothCharacteristicProperties.Notify,
    BluetoothCharacteristicPermissions.Read
);

// Start broadcasting
await broadcaster.StartBroadcastingAsync(options);

// Handle client connections
broadcaster.ClientConnected += (s, e) =>
{
    Console.WriteLine($"Client connected: {e.Device.Id}");
};

// Stop broadcasting
await broadcaster.StopBroadcastingAsync();
```

**See also:** [Broadcasting Concept](../BROADCASTING.md), [BroadcastingOptions](./Enums.md#broadcastingoptions)

---

### IBluetoothLocalService

**Namespace:** `Bluetooth.Abstractions.Broadcasting`
**Inherits:** `INotifyPropertyChanged`, `IAsyncDisposable`

Represents a local GATT service being advertised by the broadcaster.

#### Properties

```csharp
IBluetoothBroadcaster Broadcaster { get; }
string Name { get; }
Guid Id { get; }
bool IsPrimary { get; }
IReadOnlyList<IBluetoothLocalCharacteristic> Characteristics { get; }
```

#### Methods

```csharp
// Characteristic management
ValueTask<IBluetoothLocalCharacteristic> AddCharacteristicAsync(
    Guid characteristicUuid,
    BluetoothCharacteristicProperties properties,
    BluetoothCharacteristicPermissions permissions,
    TimeSpan? timeout = null,
    CancellationToken cancellationToken = default);

ValueTask RemoveCharacteristicAsync(
    IBluetoothLocalCharacteristic characteristic,
    TimeSpan? timeout = null,
    CancellationToken cancellationToken = default);

ValueTask<IBluetoothLocalCharacteristic> GetCharacteristicAsync(
    Guid characteristicUuid,
    TimeSpan? timeout = null,
    CancellationToken cancellationToken = default);

IBluetoothLocalCharacteristic? TryGetCharacteristic(Guid characteristicUuid);
```

#### Usage

```csharp
var service = await broadcaster.AddServiceAsync(serviceUuid);

var characteristic = await service.AddCharacteristicAsync(
    charUuid,
    BluetoothCharacteristicProperties.Read | BluetoothCharacteristicProperties.Write,
    BluetoothCharacteristicPermissions.Read | BluetoothCharacteristicPermissions.Write
);

Console.WriteLine($"Service: {service.Name} with {service.Characteristics.Count} characteristics");
```

---

### IBluetoothLocalCharacteristic

**Namespace:** `Bluetooth.Abstractions.Broadcasting`
**Inherits:** `INotifyPropertyChanged`, `IAsyncDisposable`

Represents a local GATT characteristic in a broadcast service. Handles read/write requests from clients.

#### Properties

```csharp
IBluetoothLocalService LocalService { get; }
Guid Id { get; }
string Name { get; }
BluetoothCharacteristicProperties Properties { get; }
BluetoothCharacteristicPermissions Permissions { get; }
ReadOnlyMemory<byte> Value { get; }
ReadOnlySpan<byte> ValueSpan { get; }
IReadOnlyList<IBluetoothLocalDescriptor> Descriptors { get; }
IReadOnlyList<IBluetoothConnectedDevice> SubscribedDevices { get; }
```

#### Events

```csharp
// Read/Write request events
event EventHandler<CharacteristicReadRequestEventArgs> ReadRequested;
event EventHandler<CharacteristicWriteRequestEventArgs> WriteRequested;
```

#### Methods

```csharp
// Value management
ValueTask SetValueAsync(
    ReadOnlyMemory<byte> value,
    TimeSpan? timeout = null,
    CancellationToken cancellationToken = default);

// Notifications
ValueTask NotifyValueAsync(
    ReadOnlyMemory<byte> value,
    IBluetoothConnectedDevice? device = null,
    TimeSpan? timeout = null,
    CancellationToken cancellationToken = default);

ValueTask IndicateValueAsync(
    ReadOnlyMemory<byte> value,
    IBluetoothConnectedDevice? device = null,
    TimeSpan? timeout = null,
    CancellationToken cancellationToken = default);

// Descriptor management
ValueTask<IBluetoothLocalDescriptor> AddDescriptorAsync(
    Guid descriptorUuid,
    BluetoothDescriptorPermissions permissions,
    TimeSpan? timeout = null,
    CancellationToken cancellationToken = default);

ValueTask RemoveDescriptorAsync(
    IBluetoothLocalDescriptor descriptor,
    TimeSpan? timeout = null,
    CancellationToken cancellationToken = default);

ValueTask<IBluetoothLocalDescriptor> GetDescriptorAsync(
    Guid descriptorUuid,
    TimeSpan? timeout = null,
    CancellationToken cancellationToken = default);

IBluetoothLocalDescriptor? TryGetDescriptor(Guid descriptorUuid);
```

#### Usage

```csharp
var characteristic = await service.AddCharacteristicAsync(
    charUuid,
    BluetoothCharacteristicProperties.Read | BluetoothCharacteristicProperties.Notify,
    BluetoothCharacteristicPermissions.Read
);

// Handle read requests
characteristic.ReadRequested += async (s, e) =>
{
    byte[] data = Encoding.UTF8.GetBytes("Hello from peripheral");
    e.RespondWithValue(data);
};

// Handle write requests
characteristic.WriteRequested += (s, e) =>
{
    Console.WriteLine($"Received: {Encoding.UTF8.GetString(e.Value.Span)}");
    e.RespondWithSuccess();
};

// Set initial value
await characteristic.SetValueAsync(Encoding.UTF8.GetBytes("Initial"));

// Send notification to subscribed clients
await characteristic.NotifyValueAsync(Encoding.UTF8.GetBytes("Update"));
```

---

### IBluetoothLocalDescriptor

**Namespace:** `Bluetooth.Abstractions.Broadcasting`
**Inherits:** `INotifyPropertyChanged`, `IAsyncDisposable`

Represents a local GATT descriptor in a broadcast characteristic.

#### Properties

```csharp
IBluetoothLocalCharacteristic LocalCharacteristic { get; }
Guid Id { get; }
string Name { get; }
BluetoothDescriptorPermissions Permissions { get; }
ReadOnlyMemory<byte> Value { get; }
ReadOnlySpan<byte> ValueSpan { get; }
```

#### Events

```csharp
event EventHandler<DescriptorReadRequestEventArgs> ReadRequested;
event EventHandler<DescriptorWriteRequestEventArgs> WriteRequested;
```

#### Methods

```csharp
ValueTask SetValueAsync(
    ReadOnlyMemory<byte> value,
    TimeSpan? timeout = null,
    CancellationToken cancellationToken = default);
```

#### Usage

```csharp
var descriptor = await characteristic.AddDescriptorAsync(
    descriptorUuid,
    BluetoothDescriptorPermissions.Read | BluetoothDescriptorPermissions.Write
);

descriptor.ReadRequested += (s, e) =>
{
    e.RespondWithValue(new byte[] { 0x01, 0x00 });
};

descriptor.WriteRequested += (s, e) =>
{
    Console.WriteLine($"Descriptor written: {BitConverter.ToString(e.Value.ToArray())}");
    e.RespondWithSuccess();
};
```

---

### IBluetoothConnectedDevice

**Namespace:** `Bluetooth.Abstractions.Broadcasting`
**Inherits:** `INotifyPropertyChanged`, `IAsyncDisposable`

Represents a central device that has connected to the broadcaster.

#### Properties

```csharp
string Id { get; }
string? Name { get; }
bool IsConnected { get; }
int Mtu { get; }
IReadOnlyList<IBluetoothLocalCharacteristic> SubscribedCharacteristics { get; }
```

#### Events

```csharp
event EventHandler<MtuChangedEventArgs> MtuChanged;
```

#### Methods

```csharp
ValueTask DisconnectAsync(
    TimeSpan? timeout = null,
    CancellationToken cancellationToken = default);
```

#### Usage

```csharp
broadcaster.ClientConnected += (s, e) =>
{
    var client = e.Device;
    Console.WriteLine($"Client {client.Name ?? client.Id} connected");
    Console.WriteLine($"MTU: {client.Mtu}");

    client.MtuChanged += (s, args) =>
    {
        Console.WriteLine($"MTU changed to: {args.NewMtu}");
    };
};

// Disconnect specific client
var client = broadcaster.TryGetConnectedDevice(deviceId);
if (client != null)
{
    await client.DisconnectAsync();
}
```

---

## See Also

- [Overview and Conventions](./README.md)
- [Enumerations](./Enums.md)
- [Events](./Events.md)
- [Exceptions](./Exceptions.md)
- [Getting Started Guide](../README.md)
