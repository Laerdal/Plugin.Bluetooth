# Exceptions

Complete reference for the exception hierarchy and error handling in Plugin.Bluetooth.

## Table of Contents

- [Exception Hierarchy](#exception-hierarchy)
- [Base Exceptions](#base-exceptions)
- [Scanning Exceptions](#scanning-exceptions)
- [Broadcasting Exceptions](#broadcasting-exceptions)
- [Guard Methods](#guard-methods)
- [Error Handling Patterns](#error-handling-patterns)
- [Platform-Specific Exceptions](#platform-specific-exceptions)

---

## Exception Hierarchy

```
BluetoothException (abstract)
├── BluetoothPermissionException
├── AdapterException
├── BluetoothScanningException (abstract)
│   ├── ScannerException (abstract)
│   │   ├── ScannerIsAlreadyStartedException
│   │   ├── ScannerIsAlreadyStoppedException
│   │   ├── ScannerFailedToStartException
│   │   ├── ScannerFailedToStopException
│   │   ├── ScannerUnexpectedStartException
│   │   ├── ScannerUnexpectedStopException
│   │   ├── ScannerConfigurationUpdateFailedException
│   │   ├── DeviceNotFoundException
│   │   ├── MultipleDevicesFoundException
│   │   ├── DeviceExplorationException
│   │   └── UnexpectedDeviceExplorationException
│   ├── DeviceException (abstract)
│   │   ├── DeviceNotConnectedException
│   │   ├── DeviceIsAlreadyConnectedException
│   │   ├── DeviceIsAlreadyDisconnectedException
│   │   ├── DeviceFailedToConnectException
│   │   ├── DeviceFailedToDisconnectException
│   │   ├── DeviceUnexpectedDisconnectionException
│   │   ├── DeviceReconnectingException
│   │   ├── DeviceAdvertisementParsingException
│   │   ├── BatteryTooLowException
│   │   ├── PairingFailedException
│   │   ├── BondingFailedException
│   │   ├── ServiceNotFoundException
│   │   ├── MultipleServicesFoundException
│   │   ├── ServiceExplorationException
│   │   └── UnexpectedServiceExplorationException
│   ├── ServiceException (abstract)
│   │   ├── CharacteristicNotFoundException
│   │   ├── MultipleCharacteristicsFoundException
│   │   ├── CharacteristicExplorationException
│   │   └── UnexpectedCharacteristicExplorationException
│   ├── CharacteristicException (abstract)
│   │   ├── CharacteristicCantReadException
│   │   ├── CharacteristicCantWriteException
│   │   ├── CharacteristicCantListenException
│   │   ├── CharacteristicReadException
│   │   ├── CharacteristicAlreadyReadingException
│   │   ├── CharacteristicUnexpectedReadException
│   │   ├── CharacteristicWriteException
│   │   ├── CharacteristicAlreadyWritingException
│   │   ├── CharacteristicUnexpectedWriteException
│   │   ├── CharacteristicNotifyException
│   │   ├── CharacteristicAlreadyNotifyingException
│   │   ├── CharacteristicUnexpectedReadNotifyException
│   │   ├── CharacteristicUnexpectedWriteNotifyException
│   │   ├── DescriptorNotFoundException
│   │   ├── MultipleDescriptorsFoundException
│   │   ├── DescriptorExplorationException
│   │   └── UnexpectedDescriptorExplorationException
│   └── DescriptorException (abstract)
│       ├── DescriptorCantReadException
│       ├── DescriptorCantWriteException
│       ├── DescriptorReadException
│       └── DescriptorWriteException
└── BluetoothBroadcastingException (abstract)
    ├── BroadcasterException (abstract)
    │   ├── BroadcasterIsAlreadyStartedException
    │   ├── BroadcasterIsAlreadyStoppedException
    │   ├── BroadcasterFailedToStartException
    │   ├── BroadcasterFailedToStopException
    │   ├── BroadcasterUnexpectedStartException
    │   ├── BroadcasterUnexpectedStopException
    │   ├── BroadcasterConfigurationUpdateFailedException
    │   ├── ServiceAlreadyExistsException
    │   ├── ServiceNotFoundException
    │   ├── MultipleServicesFoundException
    │   ├── UnexpectedServiceCreationException
    │   ├── ClientDeviceNotFoundException
    │   ├── MultipleClientDevicesFoundException
    │   └── UnexpectedClientDeviceExplorationException
    ├── ServiceException (abstract)
    │   ├── CharacteristicAlreadyExistsException
    │   ├── CharacteristicNotFoundException
    │   └── MultipleCharacteristicsFoundException
    ├── CharacteristicException (abstract)
    │   ├── DescriptorAlreadyExistsException
    │   ├── DescriptorNotFoundException
    │   └── MultipleDescriptorsFoundException
    └── ClientDeviceException (abstract)
```

---

## Base Exceptions

### BluetoothException

**Namespace:** `Bluetooth.Abstractions.Exceptions`

Base class for all Bluetooth-related exceptions.

```csharp
public abstract class BluetoothException : Exception
{
    protected BluetoothException();
    protected BluetoothException(string message);
    protected BluetoothException(string message, Exception? innerException);
}
```

**Usage:**
```csharp
try
{
    await device.ConnectAsync();
}
catch (BluetoothException ex)
{
    // Catch any Bluetooth-related error
    Console.WriteLine($"Bluetooth error: {ex.Message}");

    // Check for specific inner exceptions
    if (ex.InnerException != null)
    {
        Console.WriteLine($"Caused by: {ex.InnerException.Message}");
    }
}
```

**Properties:**
- Inherits standard `Exception` properties: `Message`, `InnerException`, `StackTrace`
- Platform-specific details available in `InnerException`

---

### BluetoothPermissionException

**Namespace:** `Bluetooth.Abstractions.Exceptions`

Thrown when Bluetooth permission requests fail or are denied.

```csharp
public class BluetoothPermissionException : BluetoothException
{
    public BluetoothPermissionException();
    public BluetoothPermissionException(string message);
    public BluetoothPermissionException(string message, Exception? innerException);
}
```

**Usage:**
```csharp
try
{
    await scanner.RequestScannerPermissionsAsync();
}
catch (BluetoothPermissionException ex)
{
    Console.WriteLine("Permission denied");

    // Check platform-specific inner exception
    if (ex.InnerException is SecurityException se)
    {
        // Android: SecurityException
        Console.WriteLine("Android: Check manifest permissions");
    }
    else if (ex.InnerException is COMException ce)
    {
        // Windows: COMException
        Console.WriteLine($"Windows error code: {ce.HResult}");
    }

    // Guide user to settings
    ShowPermissionSettingsDialog();
}
```

**Common scenarios:**
- User denies permission dialog
- Permissions not declared in manifest (Android)
- Info.plist missing usage description (iOS)
- Radio access denied (Windows)

**Platform inner exceptions:**
- Android: `SecurityException`, `IllegalStateException`
- iOS/macOS: `NSErrorException`
- Windows: `COMException`, `UnauthorizedAccessException`

---

### BluetoothUnhandledExceptionListener

**Namespace:** `Bluetooth.Abstractions.Exceptions`

Provides a mechanism for listening to and handling unhandled Bluetooth exceptions globally.

```csharp
public class BluetoothUnhandledExceptionListener : ExceptionListener
{
    public BluetoothUnhandledExceptionListener(EventHandler<ExceptionEventArgs> received);
    public static void OnBluetoothUnhandledException(object? sender, Exception e);
}
```

**Usage:**
```csharp
// Register global exception handler
var listener = new BluetoothUnhandledExceptionListener((sender, e) =>
{
    var exception = e.Exception;
    Console.WriteLine($"Unhandled Bluetooth exception: {exception.Message}");

    // Log to analytics
    Analytics.TrackError(exception);

    // Show user-friendly message
    ShowErrorDialog("Bluetooth operation failed. Please try again.");
});

// Listener is active until disposed
// ...
listener.Dispose();
```

**Notes:**
- If no listeners registered, exceptions are rethrown
- Multiple listeners can be registered
- Useful for global error logging and analytics
- Should not replace try-catch for expected errors

---

## Scanning Exceptions

### ScannerException

**Namespace:** `Bluetooth.Abstractions.Scanning.Exceptions`

Base class for scanner-related exceptions. All scanner exceptions include a reference to the scanner.

```csharp
public abstract class ScannerException : BluetoothScanningException
{
    protected ScannerException(IBluetoothScanner scanner, string message, Exception? innerException);
    public IBluetoothScanner? Scanner { get; }
}
```

**Derived exceptions:**

#### ScannerIsAlreadyStartedException

Thrown when attempting to start a scanner that is already running.

```csharp
// BAD
await scanner.StartScanningAsync();
await scanner.StartScanningAsync(); // Throws!

// GOOD
await scanner.StartScanningIfNeededAsync(); // No-op if already running
```

#### ScannerIsAlreadyStoppedException

Thrown when attempting to stop a scanner that is already stopped.

```csharp
// BAD
await scanner.StopScanningAsync();
await scanner.StopScanningAsync(); // Throws!

// GOOD
await scanner.StopScanningIfNeededAsync(); // No-op if already stopped
```

#### ScannerFailedToStartException

Thrown when scanner fails to start.

**Common causes:**
- Bluetooth adapter disabled
- Insufficient permissions
- Platform-specific errors

```csharp
try
{
    await scanner.StartScanningAsync();
}
catch (ScannerFailedToStartException ex)
{
    Console.WriteLine($"Failed to start: {ex.Message}");

    // Check adapter state
    if (!adapter.IsEnabled)
    {
        await adapter.RequestEnableAdapterAsync();
    }
}
```

#### ScannerFailedToStopException

Thrown when scanner fails to stop.

#### ScannerUnexpectedStartException

Thrown when an unexpected error occurs during scanner start.

#### ScannerUnexpectedStopException

Thrown when an unexpected error occurs during scanner stop.

#### ScannerConfigurationUpdateFailedException

Thrown when updating scanner options fails.

```csharp
try
{
    await scanner.UpdateScannerOptionsAsync(newOptions);
}
catch (ScannerConfigurationUpdateFailedException ex)
{
    Console.WriteLine("Failed to update scanner options");
    // Continue with old options
}
```

---

### Device Exploration Exceptions

#### DeviceNotFoundException

Thrown when a requested device is not found.

```csharp
try
{
    var device = await scanner.GetKnownDeviceAsync(deviceId);
}
catch (DeviceNotFoundException ex)
{
    Console.WriteLine($"Device {deviceId} not found");
    // Start scanning to find it
    await scanner.StartScanningAsync();
}
```

#### MultipleDevicesFoundException

Thrown when multiple devices match the search criteria.

```csharp
try
{
    var device = scanner.TryGetDevice(d => d.Name == "Sensor");
}
catch (MultipleDevicesFoundException ex)
{
    Console.WriteLine("Multiple devices with name 'Sensor' found");
    // Use more specific criteria
}
```

---

### DeviceException

**Namespace:** `Bluetooth.Abstractions.Scanning.Exceptions`

Base class for device-related exceptions. All device exceptions include a reference to the device.

```csharp
public abstract class DeviceException : BluetoothScanningException
{
    protected DeviceException(IBluetoothRemoteDevice device, string message, Exception? innerException);
    public IBluetoothRemoteDevice Device { get; }
}
```

#### DeviceNotConnectedException

Thrown when attempting an operation that requires a connected device.

**Static guard method:**
```csharp
public static void ThrowIfNotConnected(IBluetoothRemoteDevice device);
```

**Usage:**
```csharp
// Manual check
if (!device.IsConnected)
{
    throw new DeviceNotConnectedException(device);
}

// Guard method (recommended)
DeviceNotConnectedException.ThrowIfNotConnected(device);
await characteristic.ReadValueAsync();

// Automatic throwing by API
try
{
    await service.DiscoverCharacteristicsAsync(); // Requires connection
}
catch (DeviceNotConnectedException ex)
{
    Console.WriteLine("Device disconnected, reconnecting...");
    await ex.Device.ConnectAsync();
    await service.DiscoverCharacteristicsAsync();
}
```

#### DeviceIsAlreadyConnectedException

Thrown when attempting to connect to an already connected device.

**Static guard method:**
```csharp
public static void ThrowIfAlreadyConnected(IBluetoothRemoteDevice device);
```

**Usage:**
```csharp
// BAD
await device.ConnectAsync();
await device.ConnectAsync(); // Throws!

// GOOD
await device.ConnectIfNeededAsync(); // No-op if already connected

// Guard method
DeviceIsAlreadyConnectedException.ThrowIfAlreadyConnected(device);
```

#### DeviceIsAlreadyDisconnectedException

Thrown when attempting to disconnect from an already disconnected device.

**Static guard method:**
```csharp
public static void ThrowIfAlreadyDisconnected(IBluetoothRemoteDevice device);
```

#### DeviceFailedToConnectException

Thrown when connection attempt fails.

**Common causes:**
- Device out of range
- Device powered off
- Connection timeout
- Insufficient bonding/pairing

```csharp
try
{
    await device.ConnectAsync(timeout: TimeSpan.FromSeconds(10));
}
catch (DeviceFailedToConnectException ex)
{
    Console.WriteLine($"Connection failed: {ex.Message}");

    // Check inner exception for details
    if (ex.InnerException != null)
    {
        Console.WriteLine($"Reason: {ex.InnerException.Message}");
    }

    // Retry with longer timeout
    await device.ConnectAsync(timeout: TimeSpan.FromSeconds(30));
}
```

#### DeviceFailedToDisconnectException

Thrown when disconnection fails.

#### DeviceUnexpectedDisconnectionException

Thrown when device disconnects unexpectedly (not initiated by app).

**Usage:**
```csharp
try
{
    await PerformOperations();
}
catch (DeviceUnexpectedDisconnectionException ex)
{
    Console.WriteLine($"Lost connection: {ex.Message}");

    // Device property available
    var device = ex.Device;

    // Attempt reconnection
    await device.ConnectAsync();
}

// Better: Use event instead
device.UnexpectedDisconnection += async (s, e) =>
{
    await Task.Delay(1000);
    await device.ConnectAsync();
};
```

#### DeviceReconnectingException

Thrown when device is in reconnecting state.

#### DeviceAdvertisementParsingException

Thrown when advertisement data parsing fails.

#### BatteryTooLowException

Thrown when device battery is too low for the requested operation.

```csharp
try
{
    await device.StartFirmwareUpdate();
}
catch (BatteryTooLowException ex)
{
    Console.WriteLine($"Battery too low: {device.BatteryLevel}%");
    ShowUserMessage("Please charge the device before updating");
}
```

#### PairingFailedException / BondingFailedException

Thrown when pairing or bonding fails.

```csharp
try
{
    await pairingManager.PairAsync();
}
catch (PairingFailedException ex)
{
    Console.WriteLine("Pairing failed - user may have cancelled");
}
catch (BondingFailedException ex)
{
    Console.WriteLine("Bonding failed - encryption issue");
}
```

---

### Service/Characteristic Exploration Exceptions

#### ServiceNotFoundException

Thrown when a requested service is not found.

```csharp
try
{
    var service = await device.GetServiceAsync(serviceUuid);
}
catch (ServiceNotFoundException ex)
{
    Console.WriteLine($"Service {serviceUuid} not found");

    // Device might not have been discovered yet
    await device.DiscoverServicesAsync();
    var service = await device.GetServiceAsync(serviceUuid);
}
```

#### CharacteristicNotFoundException

Thrown when a requested characteristic is not found.

#### MultipleServicesFoundException / MultipleCharacteristicsFoundException

Thrown when multiple items match the search criteria.

---

### CharacteristicException

**Namespace:** `Bluetooth.Abstractions.Scanning.Exceptions`

Base class for characteristic-related exceptions.

```csharp
public abstract class CharacteristicException : BluetoothScanningException
{
    protected CharacteristicException(IBluetoothRemoteCharacteristic characteristic, string message, Exception? innerException);
    public IBluetoothRemoteCharacteristic RemoteCharacteristic { get; }
}
```

#### CharacteristicCantReadException

Thrown when attempting to read a characteristic that doesn't support read.

**Static guard method:**
```csharp
public static void ThrowIfCantRead(IBluetoothRemoteCharacteristic characteristic);
```

**Usage:**
```csharp
// Check manually
if (!characteristic.CanRead)
{
    Console.WriteLine("Characteristic doesn't support read");
    return;
}

// Guard method
CharacteristicCantReadException.ThrowIfCantRead(characteristic);
await characteristic.ReadValueAsync();

// API throws automatically
try
{
    await characteristic.ReadValueAsync();
}
catch (CharacteristicCantReadException ex)
{
    Console.WriteLine($"{ex.RemoteCharacteristic.Name} doesn't support read");
}
```

#### CharacteristicCantWriteException

Thrown when attempting to write to a characteristic that doesn't support write.

**Static guard method:**
```csharp
public static void ThrowIfCantWrite(IBluetoothRemoteCharacteristic characteristic);
```

#### CharacteristicCantListenException

Thrown when attempting to listen to a characteristic that doesn't support notifications.

**Static guard method:**
```csharp
public static void ThrowIfCantListen(IBluetoothRemoteCharacteristic characteristic);
```

#### CharacteristicReadException

Thrown when a read operation fails.

```csharp
try
{
    var value = await characteristic.ReadValueAsync();
}
catch (CharacteristicReadException ex)
{
    Console.WriteLine($"Read failed: {ex.Message}");

    // Check device connection
    if (!device.IsConnected)
    {
        await device.ConnectAsync();
    }

    // Retry
    var value = await characteristic.ReadValueAsync();
}
```

#### CharacteristicAlreadyReadingException

Thrown when a read is already in progress.

**Usage:**
```csharp
// BAD - concurrent reads
var task1 = characteristic.ReadValueAsync();
var task2 = characteristic.ReadValueAsync(); // May throw!

// GOOD - sequential reads
var value1 = await characteristic.ReadValueAsync();
var value2 = await characteristic.ReadValueAsync();
```

#### CharacteristicWriteException

Thrown when a write operation fails.

#### CharacteristicAlreadyWritingException

Thrown when a write is already in progress.

**Static guard method:**
```csharp
public static void ThrowIfAlreadyWriting(IBluetoothRemoteCharacteristic characteristic);
```

#### CharacteristicNotifyException

Thrown when enabling/disabling notifications fails.

#### CharacteristicAlreadyNotifyingException

Thrown when attempting to start listening when already listening.

```csharp
// BAD
await characteristic.StartListeningAsync();
await characteristic.StartListeningAsync(); // Throws!

// GOOD - check first
if (!characteristic.IsListening)
{
    await characteristic.StartListeningAsync();
}
```

---

### DescriptorException

**Namespace:** `Bluetooth.Abstractions.Scanning.Exceptions`

Base class for descriptor-related exceptions.

```csharp
public abstract class DescriptorException : BluetoothScanningException
{
    protected DescriptorException(IBluetoothRemoteDescriptor descriptor, string message, Exception? innerException);
    public IBluetoothRemoteDescriptor Descriptor { get; }
}
```

#### DescriptorCantReadException / DescriptorCantWriteException

Thrown when attempting unsupported operations on descriptors.

**Static guard methods:**
```csharp
public static void ThrowIfCantRead(IBluetoothRemoteDescriptor descriptor);
public static void ThrowIfCantWrite(IBluetoothRemoteDescriptor descriptor);
```

#### DescriptorReadException / DescriptorWriteException

Thrown when descriptor read/write operations fail.

---

## Broadcasting Exceptions

### BroadcasterException

**Namespace:** `Bluetooth.Abstractions.Broadcasting.Exceptions`

Base class for broadcaster-related exceptions.

```csharp
public abstract class BroadcasterException : BluetoothBroadcastingException
{
    protected BroadcasterException(IBluetoothBroadcaster broadcaster, string message, Exception? innerException);
    public IBluetoothBroadcaster Broadcaster { get; }
}
```

#### BroadcasterIsAlreadyStartedException / BroadcasterIsAlreadyStoppedException

Similar to scanner start/stop exceptions.

**Static guard methods:**
```csharp
public static void ThrowIfIsStarted(IBluetoothBroadcaster broadcaster);
public static void ThrowIfIsStopped(IBluetoothBroadcaster broadcaster);
```

#### BroadcasterFailedToStartException

Thrown when broadcaster fails to start.

**Common causes:**
- Platform doesn't support peripheral role
- Insufficient permissions
- Advertising not supported

```csharp
try
{
    await broadcaster.StartBroadcastingAsync(options);
}
catch (BroadcasterFailedToStartException ex)
{
    if (ex.InnerException is PlatformNotSupportedException)
    {
        Console.WriteLine("This device doesn't support peripheral mode");
    }
}
```

#### ServiceAlreadyExistsException

Thrown when trying to add a service with a UUID that already exists.

```csharp
try
{
    await broadcaster.AddServiceAsync(serviceUuid);
    await broadcaster.AddServiceAsync(serviceUuid); // Throws!
}
catch (ServiceAlreadyExistsException ex)
{
    Console.WriteLine("Service already added");
    var existing = broadcaster.TryGetService(serviceUuid);
}
```

#### CharacteristicAlreadyExistsException

Thrown when trying to add a characteristic with a UUID that already exists in the service.

---

## Guard Methods

Guard methods provide defensive programming for precondition checking.

### Pattern

```csharp
// Throws specific exception if condition not met
public static void ThrowIfCondition(object target);
```

### Available Guard Methods

| Method | Exception | Condition |
|--------|-----------|-----------|
| `DeviceNotConnectedException.ThrowIfNotConnected(device)` | DeviceNotConnectedException | Device is not connected |
| `DeviceIsAlreadyConnectedException.ThrowIfAlreadyConnected(device)` | DeviceIsAlreadyConnectedException | Device is already connected |
| `DeviceIsAlreadyDisconnectedException.ThrowIfAlreadyDisconnected(device)` | DeviceIsAlreadyDisconnectedException | Device is already disconnected |
| `CharacteristicCantReadException.ThrowIfCantRead(characteristic)` | CharacteristicCantReadException | Characteristic can't be read |
| `CharacteristicCantWriteException.ThrowIfCantWrite(characteristic)` | CharacteristicCantWriteException | Characteristic can't be written |
| `CharacteristicCantListenException.ThrowIfCantListen(characteristic)` | CharacteristicCantListenException | Characteristic can't notify |
| `CharacteristicAlreadyWritingException.ThrowIfAlreadyWriting(characteristic)` | CharacteristicAlreadyWritingException | Write already in progress |
| `DescriptorCantReadException.ThrowIfCantRead(descriptor)` | DescriptorCantReadException | Descriptor can't be read |
| `DescriptorCantWriteException.ThrowIfCantWrite(descriptor)` | DescriptorCantWriteException | Descriptor can't be written |
| `BroadcasterIsAlreadyStartedException.ThrowIfIsStarted(broadcaster)` | BroadcasterIsAlreadyStartedException | Broadcaster already started |
| `BroadcasterIsAlreadyStoppedException.ThrowIfIsStopped(broadcaster)` | BroadcasterIsAlreadyStoppedException | Broadcaster already stopped |

### Usage Examples

```csharp
// Validate preconditions before operations
public async Task<byte[]> ReadSensorData(IBluetoothRemoteDevice device, Guid charUuid)
{
    // Ensure device is connected
    DeviceNotConnectedException.ThrowIfNotConnected(device);

    var service = await device.GetServiceAsync(sensorServiceUuid);
    var characteristic = await service.GetCharacteristicAsync(charUuid);

    // Ensure characteristic supports read
    CharacteristicCantReadException.ThrowIfCantRead(characteristic);

    return (await characteristic.ReadValueAsync()).ToArray();
}

// Use in validation methods
public void ValidateDeviceState(IBluetoothRemoteDevice device)
{
    DeviceNotConnectedException.ThrowIfNotConnected(device);

    if (device.BatteryLevel < 10)
        throw new BatteryTooLowException(device);
}
```

---

## Error Handling Patterns

### Specific Exception Handling

```csharp
try
{
    await characteristic.ReadValueAsync();
}
catch (DeviceNotConnectedException ex)
{
    // Reconnect and retry
    await ex.Device.ConnectAsync();
    return await characteristic.ReadValueAsync();
}
catch (CharacteristicCantReadException ex)
{
    // Log and return default
    logger.LogWarning($"{ex.RemoteCharacteristic.Name} doesn't support read");
    return Array.Empty<byte>();
}
catch (CharacteristicReadException ex)
{
    // Retry with backoff
    await Task.Delay(500);
    return await characteristic.ReadValueAsync();
}
```

### Category Handling

```csharp
try
{
    await device.ConnectAsync();
}
catch (DeviceException ex)
{
    // Handle any device-related error
    logger.LogError(ex, $"Device error: {ex.Device.Name}");
    UpdateDeviceStatus(ex.Device, "Error");
}
catch (BluetoothException ex)
{
    // Handle any Bluetooth error
    logger.LogError(ex, "Bluetooth operation failed");
}
```

### Retry Pattern

```csharp
public async Task<T> RetryAsync<T>(
    Func<Task<T>> operation,
    int maxRetries = 3,
    TimeSpan? delay = null)
{
    delay ??= TimeSpan.FromSeconds(1);

    for (int i = 0; i < maxRetries; i++)
    {
        try
        {
            return await operation();
        }
        catch (BluetoothException ex) when (i < maxRetries - 1)
        {
            logger.LogWarning($"Attempt {i + 1} failed: {ex.Message}");
            await Task.Delay(delay.Value);
        }
    }

    throw new InvalidOperationException("All retry attempts failed");
}

// Usage
var value = await RetryAsync(() => characteristic.ReadValueAsync());
```

### Wrap and Rethrow

```csharp
public async Task<SensorData> ReadSensorDataAsync()
{
    try
    {
        var value = await characteristic.ReadValueAsync();
        return ParseSensorData(value);
    }
    catch (BluetoothException ex)
    {
        // Add context and rethrow
        throw new InvalidOperationException(
            $"Failed to read sensor data from {device.Name}",
            ex);
    }
}
```

### Graceful Degradation

```csharp
public async Task<DeviceInfo> GetDeviceInfoAsync(IBluetoothRemoteDevice device)
{
    var info = new DeviceInfo { Name = device.Name };

    // Try to get battery level, but don't fail if unavailable
    try
    {
        await device.ReadBatteryLevelAsync();
        info.BatteryLevel = device.BatteryLevel;
    }
    catch (ServiceNotFoundException)
    {
        // Battery service not available, that's OK
        logger.LogDebug("Battery service not available");
    }

    return info;
}
```

---

## Platform-Specific Exceptions

Platform-specific exceptions appear as `InnerException` on `BluetoothException` types.

### Android Exceptions

- `AndroidNativeGattStatusException` - GATT status codes
- `AndroidNativeGattCallbackStatusException` - GATT callback errors
- `AndroidNativeScanFailureException` - Scan failures
- `AndroidNativeAdvertiseFailureException` - Advertising failures

**Usage:**
```csharp
catch (DeviceFailedToConnectException ex)
{
    if (ex.InnerException is AndroidNativeGattCallbackStatusConnectionException android)
    {
        // Check Android-specific status
        Console.WriteLine($"GATT status: {android.Status}");
    }
}
```

### iOS/macOS Exceptions

- `AppleNativeBluetoothException` - CoreBluetooth errors wrapped

**Usage:**
```csharp
catch (BluetoothPermissionException ex)
{
    if (ex.InnerException is NSErrorException nsError)
    {
        Console.WriteLine($"iOS error code: {nsError.Code}");
    }
}
```

### Windows Exceptions

- `WindowsNativeBluetoothException` - Windows.Devices.Bluetooth errors
- `WindowsNativeGattCommunicationStatusException` - GATT communication status
- `WindowsNativeDeviceAccessStatusException` - Device access errors

**Usage:**
```csharp
catch (CharacteristicReadException ex)
{
    if (ex.InnerException is WindowsNativeGattCommunicationStatusException win)
    {
        Console.WriteLine($"GATT status: {win.Status}");

        if (win.ProtocolErrorCode.HasValue)
        {
            Console.WriteLine($"Protocol error: {win.ProtocolErrorCode}");
        }
    }
}
```

---

## Best Practices

### Do's

```csharp
// ✓ Use specific exceptions for different error scenarios
try
{
    await device.ConnectAsync();
}
catch (DeviceNotConnectedException) { /* reconnect */ }
catch (TimeoutException) { /* extend timeout */ }

// ✓ Use guard methods for precondition validation
DeviceNotConnectedException.ThrowIfNotConnected(device);
CharacteristicCantReadException.ThrowIfCantRead(characteristic);

// ✓ Check InnerException for platform details
catch (BluetoothException ex)
{
    logger.LogError(ex, "Bluetooth error");
    if (ex.InnerException != null)
        logger.LogError(ex.InnerException, "Platform error");
}

// ✓ Use conditional variants to avoid exceptions
await device.ConnectIfNeededAsync(); // Instead of ConnectAsync
await scanner.StartScanningIfNeededAsync(); // Instead of StartScanningAsync
```

### Don'ts

```csharp
// ✗ Don't catch and ignore without logging
try
{
    await device.ConnectAsync();
}
catch { } // BAD - silently swallows errors

// ✗ Don't catch Exception when you mean BluetoothException
catch (Exception ex) // BAD - too broad
{
    // This catches everything, including programming errors
}

// ✗ Don't use exceptions for control flow
while (!device.IsConnected)
{
    try
    {
        await device.ConnectAsync();
    }
    catch (DeviceIsAlreadyConnectedException)
    {
        break; // BAD - use conditional methods instead
    }
}

// GOOD alternative
await device.ConnectIfNeededAsync();
```

---

## See Also

- [Overview and Conventions](./README.md)
- [Interfaces and Abstractions](./Abstractions.md)
- [Events](./Events.md)
- [Troubleshooting Guide](../TROUBLESHOOTING.md)
