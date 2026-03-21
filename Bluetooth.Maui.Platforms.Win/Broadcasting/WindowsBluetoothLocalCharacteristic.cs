using Windows.Storage.Streams;

using Bluetooth.Maui.Platforms.Win.Exceptions;

namespace Bluetooth.Maui.Platforms.Win.Broadcasting;

/// <inheritdoc cref="BaseBluetoothLocalCharacteristic" />
public class WindowsBluetoothLocalCharacteristic : BaseBluetoothLocalCharacteristic
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
        try
        {
            var request = await args.GetRequestAsync().AsTask().ConfigureAwait(false);
            if (request == null)
            {
                return;
            }

            var device = WindowsBroadcaster.GetOrCreateClientDevice(null, null);
            var value = OnReadRequestReceived(device).ToArray();
            request.RespondWithValue(value.AsBuffer());
        }
        catch (Exception e)
        {
            BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, e);
        }
    }

    private async void OnWriteRequested(GattLocalCharacteristic sender, GattWriteRequestedEventArgs args)
    {
        try
        {
            var request = await args.GetRequestAsync().AsTask().ConfigureAwait(false);
            if (request == null)
            {
                return;
            }

            var data = ReadBufferBytes(request.Value);
            var device = WindowsBroadcaster.GetOrCreateClientDevice(null, null);
            await OnWriteRequestReceivedAsync(device, data).ConfigureAwait(false);
            request.Respond();
        }
        catch (Exception e)
        {
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
            return;
        }

        await NativeCharacteristic.NotifyValueAsync(value.ToArray().AsBuffer()).AsTask(cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    protected override ValueTask DisposeAsyncCore()
    {
        NativeCharacteristic.ReadRequested -= OnReadRequested;
        NativeCharacteristic.WriteRequested -= OnWriteRequested;
        NativeCharacteristic.SubscribedClientsChanged -= OnSubscribedClientsChanged;
        return base.DisposeAsyncCore();
    }
}
