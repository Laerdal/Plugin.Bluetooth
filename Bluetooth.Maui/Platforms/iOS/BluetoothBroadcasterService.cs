using System.ComponentModel;
using Bluetooth.Core.EventArgs;
using Bluetooth.Maui.PlatformSpecific;

namespace Bluetooth.Maui;

/// <summary>
/// iOS implementation of a mutable Bluetooth service for the broadcaster/peripheral role.
/// </summary>
/// <remarks>
/// This implementation wraps iOS's <see cref="CBMutableService"/> for hosting GATT services.
/// Unlike <see cref="BluetoothService"/>, this is used when the device acts as a peripheral.
/// </remarks>
public partial class BluetoothBroadcasterService : BaseBluetoothService
{
    /// <summary>
    /// Gets the native iOS Core Bluetooth mutable service.
    /// </summary>
    public CBMutableService NativeMutableService { get; }

    private readonly IBluetoothBroadcaster _broadcaster;

    /// <summary>
    /// Initializes a new instance of the <see cref="BluetoothBroadcasterService"/> class.
    /// </summary>
    /// <param name="broadcaster">The Bluetooth broadcaster hosting this service.</param>
    /// <param name="serviceUuid">The unique identifier of the service.</param>
    /// <param name="isPrimary">Indicates whether this is a primary service.</param>
    public BluetoothBroadcasterService(IBluetoothBroadcaster broadcaster, Guid serviceUuid, bool isPrimary)
        : base(CreateProxyDevice(broadcaster), serviceUuid)
    {
        _broadcaster = broadcaster;
        NativeMutableService = new CBMutableService(CBUUID.FromString(serviceUuid.ToString()), isPrimary);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BluetoothBroadcasterService"/> class with an existing mutable service.
    /// </summary>
    /// <param name="broadcaster">The Bluetooth broadcaster hosting this service.</param>
    /// <param name="serviceUuid">The unique identifier of the service.</param>
    /// <param name="nativeMutableService">The native iOS Core Bluetooth mutable service.</param>
    public BluetoothBroadcasterService(IBluetoothBroadcaster broadcaster, Guid serviceUuid, CBMutableService nativeMutableService)
        : base(CreateProxyDevice(broadcaster), serviceUuid)
    {
        _broadcaster = broadcaster;
        NativeMutableService = nativeMutableService;
    }

    private static IBluetoothDevice CreateProxyDevice(IBluetoothBroadcaster broadcaster)
    {
        // Return a minimal proxy - the device is not actually used in broadcaster scenarios
        return new BluetoothBroadcasterDeviceProxy(broadcaster);
    }

    /// <summary>
    /// Adds a characteristic to this service.
    /// </summary>
    /// <param name="characteristic">The characteristic to add.</param>
    public void AddCharacteristic(BluetoothBroadcasterCharacteristic characteristic)
    {
        ArgumentNullException.ThrowIfNull(characteristic);

        // Add to the native service's characteristics array
        var currentCharacteristics = NativeMutableService.Characteristics ?? Array.Empty<CBCharacteristic>();
        var newCharacteristics = currentCharacteristics.Append(characteristic.NativeMutableCharacteristic).ToArray();
        NativeMutableService.Characteristics = newCharacteristics;
    }

    /// <inheritdoc/>
    /// <remarks>
    /// For broadcaster services, characteristic exploration is not applicable as we define the characteristics ourselves.
    /// </remarks>
    protected override ValueTask NativeCharacteristicsExplorationAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        // For broadcaster/peripheral role, we don't discover characteristics - we define them
        return ValueTask.CompletedTask;
    }
}

/// <summary>
/// A minimal proxy device implementation for broadcaster services.
/// </summary>
/// <remarks>
/// This is a placeholder needed because services require an IBluetoothDevice,
/// but broadcasters don't have a traditional device concept. Most members throw NotSupportedException.
/// </remarks>
internal class BluetoothBroadcasterDeviceProxy : IBluetoothDevice
{
    private readonly IBluetoothBroadcaster _broadcaster;

    public BluetoothBroadcasterDeviceProxy(IBluetoothBroadcaster broadcaster)
    {
        _broadcaster = broadcaster;
    }

    public IBluetoothScanner Scanner => _broadcaster as IBluetoothScanner ?? throw new NotSupportedException("Broadcaster doesn't have a scanner.");
    public string Id => "Broadcaster";
    public Manufacturer Manufacturer => 0;
    public DateTimeOffset LastSeen => DateTimeOffset.UtcNow;
    public IBluetoothAdvertisement? LastAdvertisement => null;
    public TimeSpan IntervalBetweenAdvertisement => TimeSpan.Zero;
    public event EventHandler<AdvertisementReceivedEventArgs>? AdvertisementReceived;
    public ValueTask<IBluetoothAdvertisement> WaitForAdvertisementAsync(Func<IBluetoothAdvertisement, bool>? filter = null, TimeSpan? timeout = null, CancellationToken cancellationToken = default) => throw new NotSupportedException();
    public void OnAdvertisementReceived(IBluetoothAdvertisement advertisement) => throw new NotSupportedException();

    public bool IsConnected => true;
    public bool IsConnecting => false;
    public bool IsDisconnecting => false;
    public event EventHandler? Connecting;
    public event EventHandler? Connected;
    public event EventHandler? Disconnecting;
    public event EventHandler? Disconnected;
    public event EventHandler<DeviceUnexpectedDisconnectionEventArgs>? UnexpectedDisconnection;
    public bool IgnoreNextUnexpectedDisconnection { get; set; }

    public ValueTask WaitForIsConnectedAsync(bool isConnected, TimeSpan? timeout = null, CancellationToken cancellationToken = default) => ValueTask.CompletedTask;
    public Task ConnectAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default) => Task.CompletedTask;
    public Task DisconnectAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default) => Task.CompletedTask;
    public ValueTask ConnectIfNeededAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default) => ValueTask.CompletedTask;
    public ValueTask DisconnectIfNeededAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default) => ValueTask.CompletedTask;

    public string Name => "Broadcaster";
    public string AdvertisedName => "Broadcaster";
    public string CachedName => "Broadcaster";
    public string DebugName => "Broadcaster";
    public ValueTask WaitForNameToChangeAsync(Func<string?, bool>? filter = null, TimeSpan? timeout = null, CancellationToken cancellationToken = default) => throw new NotSupportedException();
    public Task WaitForNameToChangeAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default) => throw new NotSupportedException();

    public Task RequestMtuAsync(int mtu, TimeSpan? timeout = null, CancellationToken cancellationToken = default) => Task.CompletedTask;
    public int Mtu => 512;
    public int? BatteryLevel => null;
    public double? BatteryLevelPercent => null;
    public ValueTask WaitForBatteryLevelAsync(Func<int?, bool> filter, TimeSpan? timeout = null, CancellationToken cancellationToken = default) => throw new NotSupportedException();
    public Task<double?> ReadBatteryLevelAsync() => Task.FromResult<double?>(null);

    public int? SignalStrength => null;
    public int SignalStrengthDbm => 0;
    public double SignalStrengthPercent => 0;
    public int? TxPowerLevel => null;
    public bool IsReadingSignalStrength => false;
    public Task ReadSignalStrengthAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default) => Task.CompletedTask;
    public Task TryReadSignalStrengthAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default) => Task.CompletedTask;

    public Version? FirmwareVersion => null;
    public Task<Version> ReadFirmwareVersionAsync() => Task.FromResult(new Version(0, 0));
    public Version? SoftwareVersion => null;
    public Task<Version> ReadSoftwareVersionAsync() => Task.FromResult(new Version(0, 0));
    public string? HardwareVersion => null;
    public Task<string> ReadHardwareVersionAsync() => Task.FromResult(string.Empty);
    public Task ReadVersionsAsync() => Task.CompletedTask;

    public IEnumerable<IBluetoothService> Services => Array.Empty<IBluetoothService>();
    public event EventHandler<ServiceListChangedEventArgs>? ServiceListChanged;
    public event EventHandler<ServicesAddedEventArgs>? ServicesAdded;
    public event EventHandler<ServicesRemovedEventArgs>? ServicesRemoved;
    public bool IsExploringServices => false;

    public Task<IEnumerable<IBluetoothService>> ExploreServicesAsync(bool clearBeforeExploring = false, TimeSpan? timeout = null, CancellationToken cancellationToken = default) => Task.FromResult(Enumerable.Empty<IBluetoothService>());
    public Task ExploreServicesAsync(bool clearBeforeExploring, bool exploreCharacteristics, TimeSpan? timeout = null, CancellationToken cancellationToken = default) => Task.CompletedTask;
    public IBluetoothService? GetServiceOrDefault(Guid id) => null;
    public IBluetoothService? GetServiceOrDefault(Func<IBluetoothService, bool> filter) => null;
    public IBluetoothService GetService(Guid id) => throw new NotSupportedException();
    public IBluetoothService GetService(Func<IBluetoothService, bool> filter) => throw new NotSupportedException();
    public ValueTask<IBluetoothService?> GetServiceOrDefaultAsync(Guid id, TimeSpan? timeout = null, CancellationToken cancellationToken = default) => ValueTask.FromResult<IBluetoothService?>(null);
    public ValueTask<IBluetoothService?> GetServiceOrDefaultAsync(Func<IBluetoothService, bool> filter, TimeSpan? timeout = null, CancellationToken cancellationToken = default) => ValueTask.FromResult<IBluetoothService?>(null);
    public ValueTask<IBluetoothService> GetServiceAsync(Guid id, TimeSpan? timeout = null, CancellationToken cancellationToken = default) => throw new NotSupportedException();
    public ValueTask<IBluetoothService> GetServiceAsync(Func<IBluetoothService, bool> filter, TimeSpan? timeout = null, CancellationToken cancellationToken = default) => throw new NotSupportedException();
    public IEnumerable<IBluetoothService> GetServices(Guid id) => Array.Empty<IBluetoothService>();
    public IEnumerable<IBluetoothService> GetServices(Func<IBluetoothService, bool>? filter = null) => Array.Empty<IBluetoothService>();
    public ValueTask<IEnumerable<IBluetoothService>> GetServicesAsync(Guid id, TimeSpan? timeout = null, CancellationToken cancellationToken = default) => ValueTask.FromResult(Enumerable.Empty<IBluetoothService>());
    public ValueTask<IEnumerable<IBluetoothService>> GetServicesAsync(Func<IBluetoothService, bool>? filter = null, TimeSpan? timeout = null, CancellationToken cancellationToken = default) => ValueTask.FromResult(Enumerable.Empty<IBluetoothService>());
    public bool HasService(Guid serviceId) => false;
    public bool HasService(Func<IBluetoothService, bool> filter) => false;
    public ValueTask<bool> HasServiceAsync(Guid serviceId, TimeSpan? timeout = null, CancellationToken cancellationToken = default) => ValueTask.FromResult(false);
    public ValueTask<bool> HasServiceAsync(Func<IBluetoothService, bool> filter, TimeSpan? timeout = null, CancellationToken cancellationToken = default) => ValueTask.FromResult(false);
    public ValueTask ClearServicesAsync() => ValueTask.CompletedTask;
    public ValueTask DisposeAsync() => ValueTask.CompletedTask;
    public event PropertyChangedEventHandler? PropertyChanged;
}
