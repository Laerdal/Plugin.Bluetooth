using Windows.Storage.Streams;

using Bluetooth.Maui.Platforms.Win.Exceptions;

namespace Bluetooth.Maui.Platforms.Win.Broadcasting;

/// <inheritdoc cref="BaseBluetoothLocalCharacteristic" />
public partial class WindowsBluetoothLocalCharacteristic : BaseBluetoothLocalCharacteristic
{
    private readonly HashSet<string> _subscribedDeviceIds = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    ///     Initializes a new instance of the <see cref="WindowsBluetoothLocalCharacteristic" /> class.
    /// </summary>
    /// <param name="nativeCharacteristic">The native Windows local characteristic.</param>
    /// <param name="service">The service that owns this characteristic.</param>
    /// <param name="id">The characteristic identifier.</param>
    /// <param name="properties">Characteristic properties.</param>
    /// <param name="permissions">Characteristic permissions.</param>
    /// <param name="name">Optional characteristic name.</param>
    public WindowsBluetoothLocalCharacteristic(GattLocalCharacteristic nativeCharacteristic,
        IBluetoothLocalService service,
        Guid id,
        BluetoothCharacteristicProperties properties,
        BluetoothCharacteristicPermissions permissions,
        string? name = null)
        : base(service, id, properties, permissions, null, name)
    {
        NativeCharacteristic = nativeCharacteristic ?? throw new ArgumentNullException(nameof(nativeCharacteristic));
        NativeCharacteristic.ReadRequested += OnReadRequested;
        NativeCharacteristic.WriteRequested += OnWriteRequested;
        NativeCharacteristic.SubscribedClientsChanged += OnSubscribedClientsChanged;
    }

    /// <summary>
    ///     Gets the native Windows local characteristic.
    /// </summary>
    public GattLocalCharacteristic NativeCharacteristic { get; }

    private WindowsBluetoothBroadcaster WindowsBroadcaster => (WindowsBluetoothBroadcaster) Service.Broadcaster;

    private async void OnReadRequested(GattLocalCharacteristic sender, GattReadRequestedEventArgs args)
    {
        GattReadRequest? request = null;

        try
        {
            request = await args.GetRequestAsync().AsTask().ConfigureAwait(false);
            if (request == null)
            {
                return;
            }

            var offset = (int) request.Offset;
            var device = WindowsBroadcaster.GetOrCreateClientDevice(null, null);

            LogReadRequestReceived(Id, Service.Id, device.Id, offset);

            var fullValue = OnReadRequestReceived(device).ToArray();
            if (offset < 0 || offset > fullValue.Length)
            {
                LogReadRequestInvalidOffset(Id, Service.Id, offset, fullValue.Length);

                request.RespondWithProtocolError(GattProtocolError.InvalidOffset);
                return;
            }

            var response = offset == 0 ? fullValue : fullValue[offset..];
            request.RespondWithValue(response.AsBuffer());

            LogReadRequestSucceeded(Id, Service.Id, response.Length);
        }
        catch (NotSupportedException e)
        {
            LogReadRequestMappedToRequestNotSupported(Id, Service.Id, e);
            request?.RespondWithProtocolError(GattProtocolError.RequestNotSupported);
        }
        catch (ArgumentException e)
        {
            LogReadRequestMappedToInvalidValueLength(Id, Service.Id, e);
            request?.RespondWithProtocolError(GattProtocolError.InvalidAttributeValueLength);
        }
        catch (Exception e)
        {
            LogReadRequestFailed(Id, Service.Id, e);
            request?.RespondWithProtocolError(GattProtocolError.UnlikelyError);
            BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, e);
        }
    }

    private async void OnWriteRequested(GattLocalCharacteristic sender, GattWriteRequestedEventArgs args)
    {
        GattWriteRequest? request = null;

        try
        {
            request = await args.GetRequestAsync().AsTask().ConfigureAwait(false);
            if (request == null)
            {
                return;
            }

            var requiresResponse = request.Option == GattWriteOption.WriteWithResponse;
            var offset = (int) request.Offset;
            var data = ReadBufferBytes(request.Value);
            var device = WindowsBroadcaster.GetOrCreateClientDevice(null, null);

            LogWriteRequestReceived(Id, Service.Id, device.Id, request.Option, offset, data.Length);

            var supportsWriteWithResponse = Properties.HasFlag(BluetoothCharacteristicProperties.Write);
            var supportsWriteWithoutResponse = Properties.HasFlag(BluetoothCharacteristicProperties.WriteWithoutResponse) ||
                                               Properties.HasFlag(BluetoothCharacteristicProperties.SignedWrite);
            if ((request.Option == GattWriteOption.WriteWithResponse && !supportsWriteWithResponse) ||
                (request.Option == GattWriteOption.WriteWithoutResponse && !supportsWriteWithoutResponse))
            {
                LogWriteRequestNotPermitted(Id, Service.Id, request.Option);

                if (requiresResponse)
                {
                    request.RespondWithProtocolError(GattProtocolError.WriteNotPermitted);
                }

                return;
            }

            if (offset != 0)
            {
                LogWriteRequestInvalidOffset(Id, Service.Id, offset);

                if (requiresResponse)
                {
                    request.RespondWithProtocolError(GattProtocolError.InvalidOffset);
                }

                return;
            }

            await OnWriteRequestReceivedAsync(device, data).ConfigureAwait(false);

            if (requiresResponse)
            {
                request.Respond();
            }

            LogWriteRequestSucceeded(Id, Service.Id);
        }
        catch (NotSupportedException e)
        {
            LogWriteRequestMappedToRequestNotSupported(Id, Service.Id, e);
            if (request?.Option == GattWriteOption.WriteWithResponse)
            {
                request.RespondWithProtocolError(GattProtocolError.RequestNotSupported);
            }
        }
        catch (ArgumentException e)
        {
            LogWriteRequestMappedToInvalidValueLength(Id, Service.Id, e);
            if (request?.Option == GattWriteOption.WriteWithResponse)
            {
                request.RespondWithProtocolError(GattProtocolError.InvalidAttributeValueLength);
            }
        }
        catch (Exception e)
        {
            LogWriteRequestFailed(Id, Service.Id, e);
            if (request?.Option == GattWriteOption.WriteWithResponse)
            {
                request.RespondWithProtocolError(GattProtocolError.UnlikelyError);
            }
            BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, e);
        }
    }

    private void OnSubscribedClientsChanged(GattLocalCharacteristic sender, object args)
    {
        try
        {
            var currentClients = sender.SubscribedClients.ToDictionary(GetClientId, client => client, StringComparer.OrdinalIgnoreCase);
            foreach (var subscribedClient in currentClients)
            {
                if (_subscribedDeviceIds.Contains(subscribedClient.Key))
                {
                    continue;
                }

                var device = WindowsBroadcaster.GetOrCreateClientDevice(subscribedClient.Key, subscribedClient.Value);
                OnCharacteristicSubscribed(device);
                _subscribedDeviceIds.Add(subscribedClient.Key);

                LogSubscriptionAdded(subscribedClient.Key, Id, Service.Id);
            }

            var removedDeviceIds = _subscribedDeviceIds.Where(id => !currentClients.ContainsKey(id)).ToList();
            foreach (var removedDeviceId in removedDeviceIds)
            {
                var device = WindowsBroadcaster.GetClientDeviceOrDefault(removedDeviceId);
                if (device != null)
                {
                    OnCharacteristicUnsubscribed(device);
                }

                _subscribedDeviceIds.Remove(removedDeviceId);
                WindowsBroadcaster.RemoveClientDeviceIfNoSubscriptions(removedDeviceId);

                LogSubscriptionRemoved(removedDeviceId, Id, Service.Id);
            }
        }
        catch (Exception e)
        {
            BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, e);
        }
    }

    private static string GetClientId(GattSubscribedClient client)
    {
        var sessionDeviceId = client.Session?.DeviceId?.Id;
        return !string.IsNullOrWhiteSpace(sessionDeviceId) ? sessionDeviceId : client.GetHashCode().ToString(CultureInfo.InvariantCulture);
    }

    private static byte[] ReadBufferBytes(IBuffer? buffer)
    {
        if (buffer == null || buffer.Length == 0)
        {
            return [];
        }

        var data = new byte[buffer.Length];
        using var reader = DataReader.FromBuffer(buffer);
        reader.ReadBytes(data);
        return data;
    }

    /// <inheritdoc />
    protected override async ValueTask<IBluetoothLocalDescriptor> NativeCreateDescriptorAsync(Guid id,
        string? name = null,
        TimeSpan? timeout = null,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var parameters = new GattLocalDescriptorParameters
        {
            ReadProtectionLevel = GattProtectionLevel.Plain,
            WriteProtectionLevel = GattProtectionLevel.Plain
        };

        var result = await NativeCharacteristic.CreateDescriptorAsync(id, parameters).AsTask(cancellationToken).ConfigureAwait(false);
        WindowsNativeBluetoothErrorException.ThrowIfNotSuccess(result.Error);

        return new WindowsBluetoothLocalDescriptor(result.Descriptor, this, id, name);
    }

    /// <inheritdoc />
    protected override async ValueTask NativeUpdateValueAsync(ReadOnlyMemory<byte> value,
        bool notifyClients,
        TimeSpan? timeout = null,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (!notifyClients)
        {
            LogNotifySkipped(Id, Service.Id);
            return;
        }

        LogNotifyDispatch(Id, Service.Id, value.Length, NativeCharacteristic.SubscribedClients.Count);

        await NativeCharacteristic.NotifyValueAsync(value.ToArray().AsBuffer()).AsTask(cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    protected override ValueTask DisposeAsyncCore()
    {
        foreach (var deviceId in _subscribedDeviceIds.ToArray())
        {
            var device = WindowsBroadcaster.GetClientDeviceOrDefault(deviceId);
            if (device != null)
            {
                OnCharacteristicUnsubscribed(device);
            }

            WindowsBroadcaster.RemoveClientDeviceIfNoSubscriptions(deviceId);

            LogSubscriptionDisposeCleanup(deviceId, Id, Service.Id);
        }

        _subscribedDeviceIds.Clear();
        NativeCharacteristic.ReadRequested -= OnReadRequested;
        NativeCharacteristic.WriteRequested -= OnWriteRequested;
        NativeCharacteristic.SubscribedClientsChanged -= OnSubscribedClientsChanged;
        return base.DisposeAsyncCore();
    }
}
