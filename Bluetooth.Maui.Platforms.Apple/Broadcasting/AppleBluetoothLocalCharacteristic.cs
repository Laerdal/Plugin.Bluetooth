using Bluetooth.Maui.Platforms.Apple.Broadcasting.NativeObjects;

namespace Bluetooth.Maui.Platforms.Apple.Broadcasting;

/// <inheritdoc cref="BaseBluetoothLocalCharacteristic" />
public class AppleBluetoothLocalCharacteristic : BaseBluetoothLocalCharacteristic, CbPeripheralManagerWrapper.ICbCharacteristicDelegate
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="AppleBluetoothLocalCharacteristic" /> class with the specified Core Bluetooth characteristic, local service, ID, properties, permissions, initial value, name, and logger.
    /// </summary>
    /// <param name="cbCharacteristic">The native iOS Core Bluetooth mutable characteristic represented by this local characteristic.</param>
    /// <param name="service">The Bluetooth local service to which this characteristic belongs.</param>
    /// <param name="id">The unique identifier for this characteristic.</param>
    /// <param name="properties">The properties of this characteristic (e.g., read, write, notify).</param>
    /// <param name="permissions">The permissions for this characteristic (e.g., readable, writable).</param>
    /// <param name="initialValue">The initial value of the characteristic, if any.</param>
    /// <param name="name">An optional name for the characteristic, used for debugging purposes.</param>
    /// <param name="logger">An optional logger for logging characteristic-related events and errors.</param>
    public AppleBluetoothLocalCharacteristic(CBMutableCharacteristic cbCharacteristic,
        IBluetoothLocalService service,
        Guid id,
        BluetoothCharacteristicProperties properties,
        BluetoothCharacteristicPermissions permissions,
        ReadOnlyMemory<byte>? initialValue = null,
        string? name = null,
        ILogger<IBluetoothLocalCharacteristic>? logger = null) : base(service,
                                                                      id,
                                                                      properties,
                                                                      permissions,
                                                                      initialValue,
                                                                      name,
                                                                      logger)
    {
        CbCharacteristic = cbCharacteristic ?? throw new ArgumentNullException(nameof(cbCharacteristic));
    }

    /// <summary>
    ///     Gets the native iOS Core Bluetooth mutable characteristic.
    /// </summary>
    public CBMutableCharacteristic CbCharacteristic { get; }

    /// <summary>
    ///     Gets the Bluetooth service to which this characteristic belongs, cast to the Apple-specific implementation.
    /// </summary>
    public AppleBluetoothLocalService AppleService => (AppleBluetoothLocalService) Service;

    /// <inheritdoc />
    public void CharacteristicSubscribed(CBCentral central, CBCharacteristic characteristic)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(central);
            var device = GetOrCreateClientDevice(central);
            OnCharacteristicSubscribed(device);
        }
        catch (Exception e)
        {
            BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, e);
        }
    }

    /// <inheritdoc />
    public void CharacteristicUnsubscribed(CBCentral central, CBCharacteristic characteristic)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(central);
            var device = GetOrCreateClientDevice(central);
            OnCharacteristicUnsubscribed(device);
        }
        catch (Exception e)
        {
            BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, e);
        }
    }

    /// <inheritdoc />
    public void ReadRequestReceived(CBATTRequest request)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(request);
            ArgumentNullException.ThrowIfNull(request.Central);

            var device = GetOrCreateClientDevice(request.Central);
            var value = OnReadRequestReceived(device);
            var bytes = value.ToArray();
            request.Value = new ReadOnlyMemory<byte>(bytes).ToNSData();

            AppleService.AppleBluetoothBroadcaster.CbPeripheralManagerWrapper.CbPeripheralManager.RespondToRequest(request, CBATTError.Success);
        }
        catch (Exception e)
        {
            BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, e);
            AppleService.AppleBluetoothBroadcaster.CbPeripheralManagerWrapper.CbPeripheralManager.RespondToRequest(request, CBATTError.RequestNotSupported);
        }
    }

    /// <inheritdoc />
    public async void WriteRequestsReceived(CBATTRequest request)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(request);
            ArgumentNullException.ThrowIfNull(request.Central);
            ArgumentNullException.ThrowIfNull(request.Value);

            var device = GetOrCreateClientDevice(request.Central);
            await OnWriteRequestReceivedAsync(device, request.Value.ToArray()).ConfigureAwait(false);

            AppleService.AppleBluetoothBroadcaster.CbPeripheralManagerWrapper.CbPeripheralManager.RespondToRequest(request, CBATTError.Success);
        }
        catch (Exception e)
        {
            BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, e);
            AppleService.AppleBluetoothBroadcaster.CbPeripheralManagerWrapper.CbPeripheralManager.RespondToRequest(request, CBATTError.RequestNotSupported);
        }
    }

    /// <inheritdoc />
    protected override ValueTask NativeUpdateValueAsync(ReadOnlyMemory<byte> value, bool notifyClients, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        var nsData = value.ToNSData();
        var centralsToNotify = notifyClients ? SubscribedDevices.Cast<AppleBluetoothConnectedDevice>().Select(d => d.CbCentral).ToArray() : null;

        var result = AppleService.AppleBluetoothBroadcaster.CbPeripheralManagerWrapper.CbPeripheralManager.UpdateValue(nsData, CbCharacteristic, centralsToNotify);

        if (!result)
        {
            throw new InvalidOperationException("Failed to update characteristic value on iOS peripheral manager. The queue may be full.");
        }

        return ValueTask.CompletedTask;
    }

    /// <summary>
    ///     Gets an existing client device or creates a new one for the specified central.
    /// </summary>
    /// <param name="central">The Core Bluetooth central device.</param>
    /// <returns>The client device corresponding to the central.</returns>
    private IBluetoothConnectedDevice GetOrCreateClientDevice(CBCentral central)
    {
        return AppleService.AppleBluetoothBroadcaster.GetOrCreateClientDevice(central);
    }
    
    /// <inheritdoc />
    protected override ValueTask<IBluetoothLocalDescriptor> NativeCreateDescriptorAsync(Guid id, string? name = null, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        // Create a new CBMutableDescriptor with the specified ID and an initial value of null (iOS requires a value at creation time, but it can be updated later)
        var cbDescriptor = new CBMutableDescriptor(id.ToCBUuid(), null);
        
        // Create the corresponding AppleBluetoothLocalDescriptor instance
        var logger = Service.Broadcaster.LoggerFactory?.CreateLogger<AppleBluetoothLocalDescriptor>();
        var localDescriptor = new AppleBluetoothLocalDescriptor(cbDescriptor, this, id, null, name, logger);
        
        // Add the descriptor to the characteristic's list of descriptors
        var descriptors = CbCharacteristic.Descriptors?.ToList() ?? new List<CBDescriptor>();
        descriptors.Add(cbDescriptor);
        CbCharacteristic.Descriptors = descriptors.ToArray();
        
        return new ValueTask<IBluetoothLocalDescriptor>(localDescriptor);
    }
}
