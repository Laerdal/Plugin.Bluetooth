# Testing Best Practices

Comprehensive testing ensures your BLE application is reliable, maintainable, and bug-free. This guide covers unit testing, integration testing, mocking strategies, and test patterns for Plugin.Bluetooth.

## Table of Contents

- [Testing Strategy](#testing-strategy)
- [Mocking BLE Interfaces](#mocking-ble-interfaces)
- [Unit Testing Patterns](#unit-testing-patterns)
- [Integration Testing](#integration-testing)
- [Testing ViewModels](#testing-viewmodels)
- [Testing Error Scenarios](#testing-error-scenarios)
- [Testing Async Operations](#testing-async-operations)
- [Test Fixtures and Helpers](#test-fixtures-and-helpers)

## Testing Strategy

### Testing Pyramid

```
    /\
   /  \  E2E Tests (Few)
  /────\  - Real device testing
 /      \ - Manual testing
/────────\ Integration Tests (Some)
          - Mock hardware responses
          - Service layer testing
──────────── Unit Tests (Many)
            - ViewModel logic
            - Business logic
            - Utilities
```

### What to Test

**Unit Tests:**
- ViewModel logic and state management
- Data conversions and transformations
- Command execution logic
- Validation logic
- Error handling paths

**Integration Tests:**
- Scanner → Device → Service workflow
- Connection state management
- Event subscription and handling
- Resource cleanup and disposal

**Manual Testing:**
- Real device discovery and connection
- Data transfer reliability
- UI responsiveness
- Battery consumption
- Edge cases on target platforms

## Mocking BLE Interfaces

### Setting Up Moq

```bash
dotnet add package Moq
dotnet add package xunit
dotnet add package FluentAssertions
```

### Basic Mocking Patterns

```csharp
public class BluetoothMocks
{
    public static Mock<IBluetoothScanner> CreateMockScanner()
    {
        var mock = new Mock<IBluetoothScanner>();

        mock.Setup(s => s.IsRunning).Returns(false);
        mock.Setup(s => s.Devices).Returns(new List<IBluetoothRemoteDevice>());

        mock.Setup(s => s.StartScanningAsync(
            It.IsAny<ScanningOptions>(),
            It.IsAny<TimeSpan?>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(() =>
            {
                mock.Setup(s => s.IsRunning).Returns(true);
                mock.Raise(s => s.RunningStateChanged += null, mock.Object, EventArgs.Empty);
            });

        mock.Setup(s => s.StopScanningAsync(
            It.IsAny<TimeSpan?>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(() =>
            {
                mock.Setup(s => s.IsRunning).Returns(false);
                mock.Raise(s => s.RunningStateChanged += null, mock.Object, EventArgs.Empty);
            });

        return mock;
    }

    public static Mock<IBluetoothRemoteDevice> CreateMockDevice(
        string name = "Test Device",
        Guid? id = null)
    {
        var mock = new Mock<IBluetoothRemoteDevice>();

        mock.Setup(d => d.Id).Returns(id ?? Guid.NewGuid());
        mock.Setup(d => d.Name).Returns(name);
        mock.Setup(d => d.IsConnected).Returns(false);
        mock.Setup(d => d.SignalStrengthDbm).Returns(-50);
        mock.Setup(d => d.Mtu).Returns(23);

        mock.Setup(d => d.ConnectAsync(
            It.IsAny<ConnectionOptions>(),
            It.IsAny<TimeSpan?>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(() =>
            {
                mock.Setup(d => d.IsConnected).Returns(true);
                mock.Raise(d => d.Connected += null, mock.Object, EventArgs.Empty);
            });

        mock.Setup(d => d.DisconnectAsync(
            It.IsAny<TimeSpan?>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(() =>
            {
                mock.Setup(d => d.IsConnected).Returns(false);
                mock.Raise(d => d.Disconnected += null, mock.Object, EventArgs.Empty);
            });

        mock.Setup(d => d.Services).Returns(new List<IBluetoothRemoteService>());

        return mock;
    }

    public static Mock<IBluetoothRemoteService> CreateMockService(
        Guid? serviceId = null)
    {
        var mock = new Mock<IBluetoothRemoteService>();

        mock.Setup(s => s.Id).Returns(serviceId ?? Guid.NewGuid());
        mock.Setup(s => s.Characteristics).Returns(new List<IBluetoothRemoteCharacteristic>());

        return mock;
    }

    public static Mock<IBluetoothRemoteCharacteristic> CreateMockCharacteristic(
        Guid? characteristicId = null,
        bool canRead = true,
        bool canWrite = true,
        bool canNotify = true)
    {
        var mock = new Mock<IBluetoothRemoteCharacteristic>();

        mock.Setup(c => c.Id).Returns(characteristicId ?? Guid.NewGuid());
        mock.Setup(c => c.Name).Returns("Test Characteristic");
        mock.Setup(c => c.CanRead).Returns(canRead);
        mock.Setup(c => c.CanWrite).Returns(canWrite);
        mock.Setup(c => c.CanNotify).Returns(canNotify);
        mock.Setup(c => c.IsListening).Returns(false);
        mock.Setup(c => c.Value).Returns(Array.Empty<byte>().AsMemory());

        if (canRead)
        {
            mock.Setup(c => c.ReadValueAsync(
                It.IsAny<TimeSpan?>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(new byte[] { 0x01, 0x02, 0x03 }.AsMemory());
        }

        if (canWrite)
        {
            mock.Setup(c => c.WriteValueAsync(
                It.IsAny<ReadOnlyMemory<byte>>(),
                It.IsAny<bool>(),
                It.IsAny<TimeSpan?>(),
                It.IsAny<CancellationToken>()))
                .Returns(ValueTask.CompletedTask);
        }

        if (canNotify)
        {
            mock.Setup(c => c.StartListeningAsync(
                It.IsAny<TimeSpan?>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(() =>
                {
                    mock.Setup(c => c.IsListening).Returns(true);
                });

            mock.Setup(c => c.StopListeningAsync(
                It.IsAny<TimeSpan?>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(() =>
                {
                    mock.Setup(c => c.IsListening).Returns(false);
                });
        }

        return mock;
    }
}
```

## Unit Testing Patterns

### Testing Scanner ViewModel

```csharp
public class ScannerViewModelTests
{
    private readonly Mock<IBluetoothScanner> _mockScanner;
    private readonly ScannerViewModel _viewModel;

    public ScannerViewModelTests()
    {
        _mockScanner = BluetoothMocks.CreateMockScanner();
        _viewModel = new ScannerViewModel(_mockScanner.Object);
    }

    [Fact]
    public async Task StartScanning_ShouldUpdateIsScanning()
    {
        // Act
        await _viewModel.StartScanningCommand.ExecuteAsync(null);

        // Assert
        _viewModel.IsScanning.Should().BeTrue();
        _mockScanner.Verify(s => s.StartScanningAsync(
            It.IsAny<ScanningOptions>(),
            It.IsAny<TimeSpan?>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task StopScanning_ShouldUpdateIsScanning()
    {
        // Arrange
        await _viewModel.StartScanningCommand.ExecuteAsync(null);

        // Act
        await _viewModel.StopScanningCommand.ExecuteAsync(null);

        // Assert
        _viewModel.IsScanning.Should().BeFalse();
        _mockScanner.Verify(s => s.StopScanningAsync(
            It.IsAny<TimeSpan?>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public void DeviceListChanged_ShouldAddDeviceToCollection()
    {
        // Arrange
        var mockDevice = BluetoothMocks.CreateMockDevice("Device1");

        // Act
        _mockScanner.Raise(s => s.DeviceListChanged += null,
            _mockScanner.Object,
            new DeviceListChangedEventArgs
            {
                AddedDevices = new[] { mockDevice.Object },
                RemovedDevices = Array.Empty<IBluetoothRemoteDevice>()
            });

        // Assert
        _viewModel.Devices.Should().HaveCount(1);
        _viewModel.Devices[0].Name.Should().Be("Device1");
    }

    [Fact]
    public void DeviceListChanged_ShouldRemoveDeviceFromCollection()
    {
        // Arrange
        var mockDevice = BluetoothMocks.CreateMockDevice("Device1");

        _mockScanner.Raise(s => s.DeviceListChanged += null,
            _mockScanner.Object,
            new DeviceListChangedEventArgs
            {
                AddedDevices = new[] { mockDevice.Object },
                RemovedDevices = Array.Empty<IBluetoothRemoteDevice>()
            });

        // Act
        _mockScanner.Raise(s => s.DeviceListChanged += null,
            _mockScanner.Object,
            new DeviceListChangedEventArgs
            {
                AddedDevices = Array.Empty<IBluetoothRemoteDevice>(),
                RemovedDevices = new[] { mockDevice.Object }
            });

        // Assert
        _viewModel.Devices.Should().BeEmpty();
    }

    [Fact]
    public async Task StartScanning_WhenPermissionDenied_ShouldSetErrorStatus()
    {
        // Arrange
        _mockScanner
            .Setup(s => s.StartScanningAsync(
                It.IsAny<ScanningOptions>(),
                It.IsAny<TimeSpan?>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new BluetoothPermissionException("Permission denied"));

        // Act
        await _viewModel.StartScanningCommand.ExecuteAsync(null);

        // Assert
        _viewModel.StatusMessage.Should().Contain("permission");
    }
}
```

### Testing Device ViewModel

```csharp
public class DeviceViewModelTests
{
    private readonly Mock<IBluetoothRemoteDevice> _mockDevice;
    private readonly DeviceViewModel _viewModel;

    public DeviceViewModelTests()
    {
        _mockDevice = BluetoothMocks.CreateMockDevice();
        _viewModel = new DeviceViewModel(_mockDevice.Object);
    }

    [Fact]
    public async Task Connect_ShouldUpdateConnectionState()
    {
        // Act
        await _viewModel.ConnectCommand.ExecuteAsync(null);

        // Assert
        _viewModel.IsConnected.Should().BeTrue();
        _viewModel.ConnectionStatus.Should().Be("Connected");
        _mockDevice.Verify(d => d.ConnectAsync(
            It.IsAny<ConnectionOptions>(),
            It.IsAny<TimeSpan?>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Disconnect_ShouldUpdateConnectionState()
    {
        // Arrange
        await _viewModel.ConnectCommand.ExecuteAsync(null);

        // Act
        await _viewModel.DisconnectCommand.ExecuteAsync(null);

        // Assert
        _viewModel.IsConnected.Should().BeFalse();
        _viewModel.ConnectionStatus.Should().Be("Disconnected");
    }

    [Fact]
    public void PropertyChanged_OnDevice_ShouldUpdateViewModel()
    {
        // Arrange
        var propertyChangedRaised = false;
        _viewModel.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(DeviceViewModel.SignalStrength))
                propertyChangedRaised = true;
        };

        // Act
        _mockDevice.Setup(d => d.SignalStrengthDbm).Returns(-30);
        _mockDevice.Raise(d => d.PropertyChanged += null,
            _mockDevice.Object,
            new PropertyChangedEventArgs(nameof(IBluetoothRemoteDevice.SignalStrengthDbm)));

        // Assert
        propertyChangedRaised.Should().BeTrue();
        _viewModel.SignalStrength.Should().Be(-30);
    }

    [Fact]
    public async Task Connect_WhenTimeout_ShouldSetErrorStatus()
    {
        // Arrange
        _mockDevice
            .Setup(d => d.ConnectAsync(
                It.IsAny<ConnectionOptions>(),
                It.IsAny<TimeSpan?>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new TimeoutException("Connection timeout"));

        // Act
        await _viewModel.ConnectCommand.ExecuteAsync(null);

        // Assert
        _viewModel.ConnectionStatus.Should().Contain("timeout");
    }
}
```

### Testing Characteristic ViewModel

```csharp
public class CharacteristicViewModelTests
{
    private readonly Mock<IBluetoothRemoteCharacteristic> _mockCharacteristic;
    private readonly CharacteristicViewModel _viewModel;

    public CharacteristicViewModelTests()
    {
        _mockCharacteristic = BluetoothMocks.CreateMockCharacteristic();
        _viewModel = new CharacteristicViewModel(_mockCharacteristic.Object);
    }

    [Fact]
    public async Task ReadValue_ShouldUpdateValue()
    {
        // Arrange
        var expectedData = new byte[] { 0x01, 0x02, 0x03 };
        _mockCharacteristic
            .Setup(c => c.ReadValueAsync(
                It.IsAny<TimeSpan?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedData.AsMemory());

        // Act
        await _viewModel.ReadValueCommand.ExecuteAsync(null);

        // Assert
        _mockCharacteristic.Verify(c => c.ReadValueAsync(
            It.IsAny<TimeSpan?>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task WriteValue_ShouldInvokeWrite()
    {
        // Arrange
        var valueToWrite = "Hello";

        // Act
        await _viewModel.WriteValueCommand.ExecuteAsync(valueToWrite);

        // Assert
        _mockCharacteristic.Verify(c => c.WriteValueAsync(
            It.Is<ReadOnlyMemory<byte>>(v => v.ToArray().SequenceEqual(
                Encoding.UTF8.GetBytes(valueToWrite))),
            It.IsAny<bool>(),
            It.IsAny<TimeSpan?>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ToggleNotifications_WhenNotListening_ShouldStartListening()
    {
        // Act
        await _viewModel.ToggleNotificationsCommand.ExecuteAsync(null);

        // Assert
        _mockCharacteristic.Verify(c => c.StartListeningAsync(
            It.IsAny<TimeSpan?>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ToggleNotifications_WhenListening_ShouldStopListening()
    {
        // Arrange
        _mockCharacteristic.Setup(c => c.IsListening).Returns(true);

        // Act
        await _viewModel.ToggleNotificationsCommand.ExecuteAsync(null);

        // Assert
        _mockCharacteristic.Verify(c => c.StopListeningAsync(
            It.IsAny<TimeSpan?>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public void ValueUpdated_ShouldUpdateLastUpdated()
    {
        // Arrange
        var beforeUpdate = _viewModel.LastUpdated;

        // Act
        _mockCharacteristic.Raise(c => c.ValueUpdated += null,
            _mockCharacteristic.Object,
            new CharacteristicValueUpdatedEventArgs
            {
                NewValue = new byte[] { 0x04, 0x05 }.AsMemory(),
                OldValue = new byte[] { 0x01, 0x02 }.AsMemory()
            });

        // Assert
        _viewModel.LastUpdated.Should().BeAfter(beforeUpdate);
    }
}
```

## Integration Testing

### Testing Complete Workflow

```csharp
public class BluetoothWorkflowIntegrationTests
{
    [Fact]
    public async Task Complete_ScanConnectRead_Workflow()
    {
        // Arrange
        var mockScanner = BluetoothMocks.CreateMockScanner();
        var mockDevice = BluetoothMocks.CreateMockDevice("TestDevice");
        var mockService = BluetoothMocks.CreateMockService(TestServiceUuid);
        var mockCharacteristic = BluetoothMocks.CreateMockCharacteristic(TestCharacteristicUuid);

        mockService.Setup(s => s.Characteristics)
            .Returns(new[] { mockCharacteristic.Object });

        mockDevice.Setup(d => d.Services)
            .Returns(new[] { mockService.Object });

        mockDevice.Setup(d => d.GetService(TestServiceUuid))
            .Returns(mockService.Object);

        mockService.Setup(s => s.GetCharacteristic(TestCharacteristicUuid))
            .Returns(mockCharacteristic.Object);

        var viewModel = new ScannerViewModel(mockScanner.Object);

        // Act & Assert - Start Scanning
        await viewModel.StartScanningCommand.ExecuteAsync(null);
        viewModel.IsScanning.Should().BeTrue();

        // Simulate device discovery
        mockScanner.Raise(s => s.DeviceListChanged += null,
            mockScanner.Object,
            new DeviceListChangedEventArgs
            {
                AddedDevices = new[] { mockDevice.Object },
                RemovedDevices = Array.Empty<IBluetoothRemoteDevice>()
            });

        viewModel.Devices.Should().HaveCount(1);

        // Stop scanning
        await viewModel.StopScanningCommand.ExecuteAsync(null);
        viewModel.IsScanning.Should().BeFalse();

        // Connect to device
        var deviceVm = viewModel.Devices[0];
        await deviceVm.ConnectCommand.ExecuteAsync(null);
        deviceVm.IsConnected.Should().BeTrue();

        // Read characteristic
        var characteristicVm = new CharacteristicViewModel(mockCharacteristic.Object);
        await characteristicVm.ReadValueCommand.ExecuteAsync(null);

        // Verify
        mockCharacteristic.Verify(c => c.ReadValueAsync(
            It.IsAny<TimeSpan?>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    private static readonly Guid TestServiceUuid = Guid.Parse("0000180a-0000-1000-8000-00805f9b34fb");
    private static readonly Guid TestCharacteristicUuid = Guid.Parse("00002a29-0000-1000-8000-00805f9b34fb");
}
```

### Testing Resource Cleanup

```csharp
public class ResourceCleanupTests
{
    [Fact]
    public async Task Dispose_ShouldStopScanningAndCleanup()
    {
        // Arrange
        var mockScanner = BluetoothMocks.CreateMockScanner();
        var viewModel = new ScannerViewModel(mockScanner.Object);

        await viewModel.StartScanningCommand.ExecuteAsync(null);

        // Act
        await viewModel.DisposeAsync();

        // Assert
        mockScanner.Verify(s => s.StopScanningAsync(
            It.IsAny<TimeSpan?>(),
            It.IsAny<CancellationToken>()), Times.Once);

        mockScanner.Verify(s => s.DisposeAsync(), Times.Once);
    }

    [Fact]
    public async Task Disconnect_ShouldCleanupSubscriptions()
    {
        // Arrange
        var mockDevice = BluetoothMocks.CreateMockDevice();
        var viewModel = new DeviceViewModel(mockDevice.Object);

        await viewModel.ConnectCommand.ExecuteAsync(null);

        // Act
        await viewModel.DisconnectCommand.ExecuteAsync(null);

        // Assert
        mockDevice.Verify(d => d.DisconnectAsync(
            It.IsAny<TimeSpan?>(),
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
```

## Testing Error Scenarios

### Testing Exception Handling

```csharp
public class ErrorScenarioTests
{
    [Fact]
    public async Task Connect_WhenDeviceNotFound_ShouldHandleGracefully()
    {
        // Arrange
        var mockDevice = BluetoothMocks.CreateMockDevice();
        mockDevice
            .Setup(d => d.ConnectAsync(
                It.IsAny<ConnectionOptions>(),
                It.IsAny<TimeSpan?>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new DeviceNotConnectedException("Device not found"));

        var viewModel = new DeviceViewModel(mockDevice.Object);

        // Act
        await viewModel.ConnectCommand.ExecuteAsync(null);

        // Assert
        viewModel.IsConnected.Should().BeFalse();
        viewModel.ConnectionStatus.Should().Contain("failed");
    }

    [Fact]
    public async Task Read_WhenTimeout_ShouldRetry()
    {
        // Arrange
        var mockCharacteristic = BluetoothMocks.CreateMockCharacteristic();
        var attemptCount = 0;

        mockCharacteristic
            .Setup(c => c.ReadValueAsync(
                It.IsAny<TimeSpan?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(() =>
            {
                attemptCount++;
                if (attemptCount < 3)
                    throw new TimeoutException("Timeout");
                return new byte[] { 0x01 }.AsMemory();
            });

        var service = new CharacteristicReaderService(mockCharacteristic.Object);

        // Act
        var result = await service.ReadWithRetryAsync(maxRetries: 3);

        // Assert
        result.Should().NotBeNull();
        attemptCount.Should().Be(3);
    }

    [Fact]
    public async Task Write_WhenDeviceDisconnects_ShouldThrow()
    {
        // Arrange
        var mockCharacteristic = BluetoothMocks.CreateMockCharacteristic();
        mockCharacteristic
            .Setup(c => c.WriteValueAsync(
                It.IsAny<ReadOnlyMemory<byte>>(),
                It.IsAny<bool>(),
                It.IsAny<TimeSpan?>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new DeviceNotConnectedException("Device disconnected during write"));

        var viewModel = new CharacteristicViewModel(mockCharacteristic.Object);

        // Act & Assert
        await Assert.ThrowsAsync<DeviceNotConnectedException>(async () =>
        {
            await viewModel.WriteValueCommand.ExecuteAsync("test");
        });
    }
}
```

### Testing Platform-Specific Errors

```csharp
public class PlatformSpecificErrorTests
{
    [Fact]
    public async Task Android_GattError133_ShouldRetry()
    {
        // Arrange
        var mockDevice = BluetoothMocks.CreateMockDevice();
        var attemptCount = 0;

        mockDevice
            .Setup(d => d.ConnectAsync(
                It.IsAny<ConnectionOptions>(),
                It.IsAny<TimeSpan?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(() =>
            {
                attemptCount++;
                if (attemptCount < 2)
                {
                    // Simulate GATT Error 133
                    throw new BluetoothException(
                        "Connection failed",
                        new Exception("GATT error 133"));
                }
            });

        var connectionManager = new AndroidConnectionManager(mockDevice.Object);

        // Act
        await connectionManager.ConnectWithRetryAsync();

        // Assert
        attemptCount.Should().Be(2);
        mockDevice.Verify(d => d.ConnectAsync(
            It.IsAny<ConnectionOptions>(),
            It.IsAny<TimeSpan?>(),
            It.IsAny<CancellationToken>()), Times.Exactly(2));
    }
}
```

## Testing Async Operations

### Testing Cancellation

```csharp
public class AsyncOperationTests
{
    [Fact]
    public async Task Scan_WhenCancelled_ShouldStopScanning()
    {
        // Arrange
        var mockScanner = BluetoothMocks.CreateMockScanner();
        var cts = new CancellationTokenSource();
        var viewModel = new ScannerViewModel(mockScanner.Object);

        // Setup scanner to respect cancellation
        mockScanner
            .Setup(s => s.StartScanningAsync(
                It.IsAny<ScanningOptions>(),
                It.IsAny<TimeSpan?>(),
                It.IsAny<CancellationToken>()))
            .Returns(async (ScanningOptions o, TimeSpan? t, CancellationToken ct) =>
            {
                await Task.Delay(5000, ct);
            });

        // Act
        var scanTask = viewModel.StartScanningCommand.ExecuteAsync(cts.Token);
        await Task.Delay(100); // Let scan start
        cts.Cancel();

        // Assert
        await Assert.ThrowsAsync<OperationCanceledException>(() => scanTask.AsTask());
    }

    [Fact]
    public async Task Connect_WithTimeout_ShouldThrowTimeoutException()
    {
        // Arrange
        var mockDevice = BluetoothMocks.CreateMockDevice();

        mockDevice
            .Setup(d => d.ConnectAsync(
                It.IsAny<ConnectionOptions>(),
                It.IsAny<TimeSpan?>(),
                It.IsAny<CancellationToken>()))
            .Returns(async (ConnectionOptions o, TimeSpan? t, CancellationToken ct) =>
            {
                await Task.Delay(Timeout.Infinite, ct);
            });

        var viewModel = new DeviceViewModel(mockDevice.Object);

        using var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(100));

        // Act & Assert
        await Assert.ThrowsAsync<OperationCanceledException>(async () =>
        {
            await viewModel.ConnectCommand.ExecuteAsync(cts.Token);
        });
    }
}
```

## Test Fixtures and Helpers

### Reusable Test Fixtures

```csharp
public class BluetoothTestFixture : IDisposable
{
    public Mock<IBluetoothScanner> MockScanner { get; }
    public Mock<IBluetoothRemoteDevice> MockDevice { get; }
    public Mock<IBluetoothRemoteService> MockService { get; }
    public Mock<IBluetoothRemoteCharacteristic> MockCharacteristic { get; }

    public BluetoothTestFixture()
    {
        MockScanner = BluetoothMocks.CreateMockScanner();
        MockDevice = BluetoothMocks.CreateMockDevice();
        MockService = BluetoothMocks.CreateMockService();
        MockCharacteristic = BluetoothMocks.CreateMockCharacteristic();

        // Setup relationships
        MockService.Setup(s => s.Characteristics)
            .Returns(new[] { MockCharacteristic.Object });

        MockDevice.Setup(d => d.Services)
            .Returns(new[] { MockService.Object });

        MockScanner.Setup(s => s.Devices)
            .Returns(new[] { MockDevice.Object });
    }

    public void Dispose()
    {
        // Cleanup if needed
    }
}

public class ScannerViewModelTestsWithFixture : IClassFixture<BluetoothTestFixture>
{
    private readonly BluetoothTestFixture _fixture;

    public ScannerViewModelTestsWithFixture(BluetoothTestFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task StartScanning_ShouldWork()
    {
        var viewModel = new ScannerViewModel(_fixture.MockScanner.Object);
        await viewModel.StartScanningCommand.ExecuteAsync(null);
        viewModel.IsScanning.Should().BeTrue();
    }
}
```

### Test Data Builders

```csharp
public class TestDataBuilder
{
    public static byte[] CreateTestData(int size)
    {
        var data = new byte[size];
        Random.Shared.NextBytes(data);
        return data;
    }

    public static Mock<IBluetoothRemoteDevice> CreateConnectedDevice(string name = "Test")
    {
        var mock = BluetoothMocks.CreateMockDevice(name);
        mock.Setup(d => d.IsConnected).Returns(true);
        return mock;
    }

    public static Mock<IBluetoothRemoteCharacteristic> CreateReadableCharacteristic(byte[] value)
    {
        var mock = BluetoothMocks.CreateMockCharacteristic();
        mock.Setup(c => c.CanRead).Returns(true);
        mock.Setup(c => c.ReadValueAsync(
            It.IsAny<TimeSpan?>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(value.AsMemory());
        return mock;
    }

    public static DeviceListChangedEventArgs CreateDeviceAddedEvent(
        params IBluetoothRemoteDevice[] devices)
    {
        return new DeviceListChangedEventArgs
        {
            AddedDevices = devices,
            RemovedDevices = Array.Empty<IBluetoothRemoteDevice>()
        };
    }
}
```

### Assertion Helpers

```csharp
public static class BluetoothAssertions
{
    public static void ShouldBeConnected(this IBluetoothRemoteDevice device)
    {
        device.IsConnected.Should().BeTrue("device should be connected");
    }

    public static void ShouldBeDisconnected(this IBluetoothRemoteDevice device)
    {
        device.IsConnected.Should().BeFalse("device should be disconnected");
    }

    public static void ShouldHaveGoodSignalStrength(this IBluetoothRemoteDevice device)
    {
        device.SignalStrengthDbm.Should().BeGreaterThan(-70,
            "device should have good signal strength");
    }

    public static void ShouldContainDevice(
        this ObservableCollection<DeviceViewModel> collection,
        string deviceName)
    {
        collection.Should().Contain(d => d.Name == deviceName,
            $"collection should contain device '{deviceName}'");
    }

    public static async Task ShouldCompleteWithin(
        this Task task,
        TimeSpan timeout,
        string because = "")
    {
        using var cts = new CancellationTokenSource(timeout);
        var completedTask = await Task.WhenAny(task, Task.Delay(Timeout.Infinite, cts.Token));

        completedTask.Should().Be(task,
            $"task should complete within {timeout.TotalSeconds}s {because}");
    }
}
```

## Summary

### Testing Best Practices Checklist

- [ ] **Mock all BLE interfaces** using Moq or similar framework
- [ ] **Test ViewModel logic** independently of UI
- [ ] **Test event handling** and property change notifications
- [ ] **Test error scenarios** with appropriate exceptions
- [ ] **Test async operations** with cancellation and timeouts
- [ ] Use **test fixtures** for reusable mock setups
- [ ] Create **test data builders** for complex scenarios
- [ ] Write **integration tests** for complete workflows
- [ ] Test **resource cleanup** and disposal
- [ ] Use **FluentAssertions** for readable test assertions
- [ ] Test **platform-specific behaviors** where applicable
- [ ] **Mock event raising** to test event handlers
- [ ] Test **command CanExecute** logic
- [ ] Verify **method calls** and parameters with Moq.Verify

### Test Coverage Goals

- **Unit Tests**: > 80% code coverage
- **Integration Tests**: All happy path workflows
- **Error Scenarios**: All exception paths
- **ViewModels**: 100% command and property logic
- **Business Logic**: 100% coverage

### Related Topics

- [MVVM Integration](MVVM-Integration.md) - Testing ViewModels
- [Error Handling](Error-Handling.md) - Testing error scenarios
- [Connection Management](Connection-Management.md) - Testing connection workflows
