using Bluetooth.Maui.PlatformSpecific;

namespace Bluetooth.Maui;

public partial class BluetoothBroadcaster
{
    protected async override Task<IBluetoothBroadcastService> NativeAddServiceAsync(Guid id,
        string name,
        bool isPrimary,
        TimeSpan? timeout,
        CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    protected async override Task NativeRemoveServiceAsync(Guid id, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public BluetoothGattServerCallbackProxy.IService GetService(BluetoothGattService? native)
    {
        throw new NotImplementedException();
    }

    /*
    private readonly Dictionary<Guid, TaskCompletionSource<bool>> _serviceAddedTasks = new();

    /// <inheritdoc/>
    protected override async Task NativeAddServiceAsync(IBluetoothService service, TimeSpan? timeout, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(service);
        ArgumentNullException.ThrowIfNull(BluetoothGattServerCallbackProxy);

        if (service is not BluetoothBroadcasterService broadcasterService)
        {
            throw new ArgumentException("Service must be a BluetoothBroadcasterService", nameof(service));
        }

        // Create TCS for waiting on service to be added
        var tcs = new TaskCompletionSource<bool>();
        _serviceAddedTasks[service.Id] = tcs;

        try
        {
            // Add the native BluetoothGattService to the GATT server
            BluetoothGattServerCallbackProxy.BluetoothGattServer.AddService(broadcasterService.NativeGattService);

            // Wait for the service to be added (OnServiceAdded callback)
            await tcs.Task.WaitAsync(timeout ?? TimeSpan.FromSeconds(10), cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            _serviceAddedTasks.Remove(service.Id);
        }
    }

    /// <inheritdoc/>
    protected override Task NativeRemoveServiceAsync(IBluetoothService service, TimeSpan? timeout, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(service);
        ArgumentNullException.ThrowIfNull(BluetoothGattServerCallbackProxy);

        if (service is not BluetoothBroadcasterService broadcasterService)
        {
            throw new ArgumentException("Service must be a BluetoothBroadcasterService", nameof(service));
        }

        // Remove the native BluetoothGattService from the GATT server
        BluetoothGattServerCallbackProxy.BluetoothGattServer.RemoveService(broadcasterService.NativeGattService);

        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    protected override Task<IBluetoothCharacteristic> NativeAddCharacteristicAsync(IBluetoothService service,
        Guid characteristicId,
        CharacteristicProperties properties,
        CharacteristicPermissions permissions,
        byte[]? initialValue,
        TimeSpan? timeout,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(service);

        if (service is not BluetoothBroadcasterService broadcasterService)
        {
            throw new ArgumentException("Service must be a BluetoothBroadcasterService", nameof(service));
        }

        // Convert our properties to GattProperty
        var gattProperties = ConvertProperties(properties);
        var gattPermissions = ConvertPermissions(permissions);

        // Create the characteristic wrapper
        var characteristic = new BluetoothBroadcasterCharacteristic(
            service,
            characteristicId,
            gattProperties,
            gattPermissions,
            initialValue);

        // Add to the service
        broadcasterService.AddCharacteristic(characteristic);

        return Task.FromResult<IBluetoothCharacteristic>(characteristic);
    }

    /// <inheritdoc/>
    protected override Task NativeUpdateCharacteristicValueAsync(IBluetoothService service,
        Guid characteristicId,
        byte[] value,
        bool notifyClients,
        TimeSpan? timeout,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(service);
        ArgumentNullException.ThrowIfNull(value);
        ArgumentNullException.ThrowIfNull(BluetoothGattServerCallbackProxy);

        if (service is not BluetoothBroadcasterService broadcasterService)
        {
            throw new ArgumentException("Service must be a BluetoothBroadcasterService", nameof(service));
        }

        // Find the characteristic
        var characteristic = broadcasterService.NativeGattService.Characteristics?
            .FirstOrDefault(c => Guid.Parse(c.Uuid?.ToString() ?? string.Empty) == characteristicId);

        if (characteristic == null)
        {
            throw new InvalidOperationException($"Characteristic {characteristicId} not found in service {service.Id}");
        }

        // Update the characteristic value
#pragma warning disable CA1422 // Validate platform compatibility
        characteristic.SetValue(value);
#pragma warning restore CA1422 // Validate platform compatibility

        if (notifyClients)
        {
            // Send notification to all connected devices
            foreach (var device in _connectedDevices.Values)
            {
                BluetoothGattServerCallbackProxy.BluetoothGattServer.NotifyCharacteristicChanged(device, characteristic, false);
            }
        }

        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    protected override Task NativeNotifyClientAsync(string clientId,
        IBluetoothService service,
        Guid characteristicId,
        byte[] value,
        TimeSpan? timeout,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(clientId);
        ArgumentNullException.ThrowIfNull(service);
        ArgumentNullException.ThrowIfNull(value);
        ArgumentNullException.ThrowIfNull(BluetoothGattServerCallbackProxy);

        if (service is not BluetoothBroadcasterService broadcasterService)
        {
            throw new ArgumentException("Service must be a BluetoothBroadcasterService", nameof(service));
        }

        // Find the characteristic
        var characteristic = broadcasterService.NativeGattService.Characteristics?
            .FirstOrDefault(c => Guid.Parse(c.Uuid?.ToString() ?? string.Empty) == characteristicId);

        if (characteristic == null)
        {
            throw new InvalidOperationException($"Characteristic {characteristicId} not found in service {service.Id}");
        }

        // Find the device by ID
        if (!_connectedDevices.TryGetValue(clientId, out var device))
        {
            throw new InvalidOperationException($"Client {clientId} not found");
        }

        // Update the characteristic value
#pragma warning disable CA1422 // Validate platform compatibility
        characteristic.SetValue(value);
#pragma warning restore CA1422 // Validate platform compatibility

        // Send notification to specific device
        BluetoothGattServerCallbackProxy.BluetoothGattServer.NotifyCharacteristicChanged(device, characteristic, false);

        return Task.CompletedTask;
    }

    private static GattProperty ConvertProperties(CharacteristicProperties properties)
    {
        GattProperty result = 0;

        if (properties.HasFlag(CharacteristicProperties.Broadcast))
        {
            result |= GattProperty.Broadcast;
        }
        if (properties.HasFlag(CharacteristicProperties.Read))
        {
            result |= GattProperty.Read;
        }
        if (properties.HasFlag(CharacteristicProperties.WriteWithoutResponse))
        {
            result |= GattProperty.WriteNoResponse;
        }
        if (properties.HasFlag(CharacteristicProperties.Write))
        {
            result |= GattProperty.Write;
        }
        if (properties.HasFlag(CharacteristicProperties.Notify))
        {
            result |= GattProperty.Notify;
        }
        if (properties.HasFlag(CharacteristicProperties.Indicate))
        {
            result |= GattProperty.Indicate;
        }
        if (properties.HasFlag(CharacteristicProperties.AuthenticatedSignedWrites))
        {
            result |= GattProperty.SignedWrite;
        }
        if (properties.HasFlag(CharacteristicProperties.ExtendedProperties))
        {
            result |= GattProperty.ExtendedProps;
        }

        return result;
    }

    private static GattPermission ConvertPermissions(CharacteristicPermissions permissions)
    {
        GattPermission result = 0;

        if (permissions.HasFlag(CharacteristicPermissions.Read))
        {
            result |= GattPermission.Read;
        }
        if (permissions.HasFlag(CharacteristicPermissions.ReadEncrypted))
        {
            result |= GattPermission.ReadEncrypted;
        }
        if (permissions.HasFlag(CharacteristicPermissions.Write))
        {
            result |= GattPermission.Write;
        }
        if (permissions.HasFlag(CharacteristicPermissions.WriteEncrypted))
        {
            result |= GattPermission.WriteEncrypted;
        }

        return result;
    }

    private Dictionary<Guid, TaskCompletionSource<BluetoothGattService>> AddServiceTasks { get; } = new Dictionary<Guid, TaskCompletionSource<BluetoothGattService>>();

    protected async override Task<IBluetoothBroadcasterService> NativeAddServiceAsync(Guid id,
        string name,
        bool isPrimary = true,
        TimeSpan? timeout = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(BluetoothGattServerCallbackProxy);
        var service = new BluetoothGattService(id.ToUuid(), isPrimary ? GattServiceType.Primary : GattServiceType.Secondary);
        var result = BluetoothGattServerCallbackProxy.BluetoothGattServer.AddService(service);
        if(!result)
        {
            throw new Exception("Failed to add service"); // TODO : Write better exception
        }

        return new BluetoothBroadcasterService(this, id, name, service);
    }

    protected override Task<IBluetoothBroadcasterService> NativeRemoveServiceAsync(Guid id, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Called when a service has been added to the GATT server.
    /// </summary>
    /// <param name="status">The status of the service add operation.</param>
    /// <param name="service">The service that was added.</param>
    public void OnServiceAdded(GattStatus status, BluetoothGattService? service)
    {
        AndroidNativeGattCallbackStatusException.ThrowIfNotSuccess(status);
        if (service == null)
        {
            return;
        }

        var serviceId = Guid.Parse(service.Uuid?.ToString() ?? string.Empty);

        if (_serviceAddedTasks.TryGetValue(serviceId, out var tcs))
        {
            if (status == GattStatus.Success)
            {
                tcs.TrySetResult(true);
            }
            else
            {
                tcs.TrySetException(new InvalidOperationException($"Failed to add service: {status}"));
            }
        }
    }

}

public class BluetoothBroadcasterService : BaseBluetoothBroadcasterService
{
    public BluetoothGattService NativeService { get; }

    public BluetoothBroadcasterService(IBluetoothBroadcaster broadcaster, Guid id, string name, BluetoothGattService nativeService) : base(broadcaster, id, name)
    {
        NativeService = nativeService;
    }

    protected override Task<IBluetoothBroadcasterCharacteristic> NativeAddCharacteristicAsync(Guid id,
        string name,
        CharacteristicProperties properties,
        CharacteristicPermissions permissions,
        ReadOnlyMemory<byte>? initialValue = null,
        TimeSpan? timeout = null,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }*/
}
