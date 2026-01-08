using Bluetooth.Maui.PlatformSpecific;

namespace Bluetooth.Maui;

public partial class BluetoothBroadcaster
{
    /// <inheritdoc/>
    protected override IBluetoothService NativeCreateService(Guid serviceId, bool isPrimary)
    {
        return new BluetoothBroadcasterService(this, serviceId, isPrimary);
    }

    private readonly Dictionary<Guid, TaskCompletionSource<bool>> _serviceAddedTasks = new();

    /// <inheritdoc/>
    protected override async Task NativeAddServiceAsync(IBluetoothService service, TimeSpan? timeout, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(service);
        ArgumentNullException.ThrowIfNull(CbPeripheralManagerProxy);

        if (service is not BluetoothBroadcasterService broadcasterService)
        {
            throw new ArgumentException("Service must be a BluetoothBroadcasterService", nameof(service));
        }

        // Create TCS for waiting on service to be added
        var tcs = new TaskCompletionSource<bool>();
        _serviceAddedTasks[service.Id] = tcs;

        try
        {
            // Add the native CBMutableService to the peripheral manager
            CbPeripheralManagerProxy.CbPeripheralManager.AddService(broadcasterService.NativeMutableService);

            // Wait for the service to be added
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
        ArgumentNullException.ThrowIfNull(CbPeripheralManagerProxy);

        if (service is not BluetoothBroadcasterService broadcasterService)
        {
            throw new ArgumentException("Service must be a BluetoothBroadcasterService", nameof(service));
        }

        // Remove the native CBMutableService from the peripheral manager
        CbPeripheralManagerProxy.CbPeripheralManager.RemoveService(broadcasterService.NativeMutableService);

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

        // Convert our properties to CBCharacteristicProperties
        var cbProperties = ConvertProperties(properties);
        var cbPermissions = ConvertPermissions(permissions);

        // Create the characteristic wrapper
        var characteristic = new BluetoothBroadcasterCharacteristic(
            service,
            characteristicId,
            cbProperties,
            cbPermissions,
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
        ArgumentNullException.ThrowIfNull(CbPeripheralManagerProxy);

        if (service is not BluetoothBroadcasterService broadcasterService)
        {
            throw new ArgumentException("Service must be a BluetoothBroadcasterService", nameof(service));
        }

        // Find the characteristic
        var characteristic = broadcasterService.NativeMutableService.Characteristics?
            .FirstOrDefault(c => Guid.Parse(c.UUID.ToString()) == characteristicId);

        if (characteristic is not CBMutableCharacteristic mutableCharacteristic)
        {
            throw new InvalidOperationException($"Characteristic {characteristicId} not found in service {service.Id}");
        }

        var nsData = NSData.FromArray(value);

        // Update the characteristic value
        mutableCharacteristic.Value = nsData;

        if (notifyClients)
        {
            // Send notification to all subscribed centrals
            CbPeripheralManagerProxy.CbPeripheralManager.UpdateValue(nsData, mutableCharacteristic, null);
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
        ArgumentNullException.ThrowIfNull(CbPeripheralManagerProxy);

        if (service is not BluetoothBroadcasterService broadcasterService)
        {
            throw new ArgumentException("Service must be a BluetoothBroadcasterService", nameof(service));
        }

        // Find the characteristic
        var characteristic = broadcasterService.NativeMutableService.Characteristics?
            .FirstOrDefault(c => Guid.Parse(c.UUID.ToString()) == characteristicId);

        if (characteristic is not CBMutableCharacteristic mutableCharacteristic)
        {
            throw new InvalidOperationException($"Characteristic {characteristicId} not found in service {service.Id}");
        }

        // Find the central by ID
        if (!_connectedCentrals.TryGetValue(clientId, out var central))
        {
            throw new InvalidOperationException($"Client {clientId} not found");
        }

        var nsData = NSData.FromArray(value);

        // Update the characteristic value
        mutableCharacteristic.Value = nsData;

        // Send notification to specific central
        CbPeripheralManagerProxy.CbPeripheralManager.UpdateValue(nsData, mutableCharacteristic, new[] { central });

        return Task.CompletedTask;
    }

    private static CBCharacteristicProperties ConvertProperties(CharacteristicProperties properties)
    {
        CBCharacteristicProperties result = 0;

        if (properties.HasFlag(CharacteristicProperties.Broadcast))
        {
            result |= CBCharacteristicProperties.Broadcast;
        }
        if (properties.HasFlag(CharacteristicProperties.Read))
        {
            result |= CBCharacteristicProperties.Read;
        }
        if (properties.HasFlag(CharacteristicProperties.WriteWithoutResponse))
        {
            result |= CBCharacteristicProperties.WriteWithoutResponse;
        }
        if (properties.HasFlag(CharacteristicProperties.Write))
        {
            result |= CBCharacteristicProperties.Write;
        }
        if (properties.HasFlag(CharacteristicProperties.Notify))
        {
            result |= CBCharacteristicProperties.Notify;
        }
        if (properties.HasFlag(CharacteristicProperties.Indicate))
        {
            result |= CBCharacteristicProperties.Indicate;
        }
        if (properties.HasFlag(CharacteristicProperties.AuthenticatedSignedWrites))
        {
            result |= CBCharacteristicProperties.AuthenticatedSignedWrites;
        }
        if (properties.HasFlag(CharacteristicProperties.ExtendedProperties))
        {
            result |= CBCharacteristicProperties.ExtendedProperties;
        }

        return result;
    }

    private static CBAttributePermissions ConvertPermissions(CharacteristicPermissions permissions)
    {
        CBAttributePermissions result = 0;

        if (permissions.HasFlag(CharacteristicPermissions.Read))
        {
            result |= CBAttributePermissions.Readable;
        }
        if (permissions.HasFlag(CharacteristicPermissions.ReadEncrypted))
        {
            result |= CBAttributePermissions.ReadEncryptionRequired;
        }
        if (permissions.HasFlag(CharacteristicPermissions.Write))
        {
            result |= CBAttributePermissions.Writeable;
        }
        if (permissions.HasFlag(CharacteristicPermissions.WriteEncrypted))
        {
            result |= CBAttributePermissions.WriteEncryptionRequired;
        }

        return result;
    }
}
