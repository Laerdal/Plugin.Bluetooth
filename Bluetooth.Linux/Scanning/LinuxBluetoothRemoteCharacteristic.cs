using Bluetooth.Linux.Scanning.Factories;
using Tmds.DBus;

namespace Bluetooth.Linux.Scanning;

/// <summary>
///     Linux implementation of a remote GATT characteristic backed by a BlueZ D-Bus <see cref="IGattCharacteristic1"/> proxy.
/// </summary>
public class LinuxBluetoothRemoteCharacteristic : BaseBluetoothRemoteCharacteristic
{
    private readonly IGattCharacteristic1 _nativeCharacteristic;
    private readonly string[] _flags;
    private IDisposable? _propertyWatcher;

    /// <inheritdoc />
    public LinuxBluetoothRemoteCharacteristic(
        IBluetoothRemoteService parentService,
        IBluetoothRemoteCharacteristicFactory.BluetoothRemoteCharacteristicFactorySpec spec,
        IBluetoothRemoteDescriptorFactory descriptorFactory,
        IBluetoothNameProvider? nameProvider = null,
        ILogger<IBluetoothRemoteCharacteristic>? logger = null)
        : base(parentService, spec, descriptorFactory, nameProvider, logger)
    {
        if (spec is LinuxBluetoothRemoteCharacteristicFactorySpec linuxSpec)
        {
            _nativeCharacteristic = linuxSpec.NativeCharacteristic;
            _flags = linuxSpec.Flags;
        }
        else
        {
            throw new ArgumentException($"Expected {nameof(LinuxBluetoothRemoteCharacteristicFactorySpec)}", nameof(spec));
        }
    }

    #region Capabilities

    /// <inheritdoc />
    protected override bool NativeCanRead() =>
        _flags.Contains("read", StringComparer.OrdinalIgnoreCase);

    /// <inheritdoc />
    protected override bool NativeCanWrite() =>
        _flags.Contains("write", StringComparer.OrdinalIgnoreCase)
        || _flags.Contains("write-without-response", StringComparer.OrdinalIgnoreCase);

    /// <inheritdoc />
    protected override bool NativeCanListen() =>
        _flags.Contains("notify", StringComparer.OrdinalIgnoreCase)
        || _flags.Contains("indicate", StringComparer.OrdinalIgnoreCase);

    #endregion

    #region Read

    /// <inheritdoc />
    protected override async ValueTask NativeReadValueAsync()
    {
        try
        {
            var options = new Dictionary<string, object>();
            var bytes = await _nativeCharacteristic.ReadValueAsync(options).ConfigureAwait(false);
            OnReadValueSucceeded(bytes);
        }
        catch (Exception ex)
        {
            OnReadValueFailed(ex);
        }
    }

    #endregion

    #region Write

    /// <inheritdoc />
    protected override async ValueTask NativeWriteValueAsync(ReadOnlyMemory<byte> value)
    {
        try
        {
            var options = new Dictionary<string, object>();
            await _nativeCharacteristic.WriteValueAsync(value.ToArray(), options).ConfigureAwait(false);
            OnWriteValueSucceeded();
        }
        catch (Exception ex)
        {
            OnWriteValueFailed(ex);
        }
    }

    #endregion

    #region Notifications

    /// <inheritdoc />
    protected override async ValueTask NativeReadIsListeningAsync()
    {
        try
        {
            var props = await _nativeCharacteristic.GetAllAsync().ConfigureAwait(false);
            OnReadIsListeningSucceeded(props.Notifying);
        }
        catch (Exception ex)
        {
            OnReadIsListeningFailed(ex);
        }
    }

    /// <inheritdoc />
    protected override async ValueTask NativeWriteIsListeningAsync(bool shouldBeListening)
    {
        if (shouldBeListening)
        {
            _propertyWatcher = await _nativeCharacteristic.WatchPropertiesAsync(OnPropertyChanged).ConfigureAwait(false);
            await _nativeCharacteristic.StartNotifyAsync().ConfigureAwait(false);
        }
        else
        {
            await _nativeCharacteristic.StopNotifyAsync().ConfigureAwait(false);
            _propertyWatcher?.Dispose();
            _propertyWatcher = null;
        }

        OnWriteIsListeningSucceeded();
    }

    private void OnPropertyChanged(PropertyChanges changes)
    {
        foreach (var pair in changes.Changed)
        {
            if (pair.Key == "Value" && pair.Value is byte[] bytes)
            {
                OnReadValueSucceeded(bytes);
            }
        }
    }

    #endregion

    #region Descriptor Exploration

    /// <inheritdoc />
    protected override async ValueTask NativeDescriptorsExplorationAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var nativeDescriptors = await GetDescriptorsAsync(_nativeCharacteristic).ConfigureAwait(false);

            // Pre-fetch UUID and Flags asynchronously before entering the synchronous conversion lambda
            var descriptorEntries = await Task.WhenAll(nativeDescriptors.Select(async desc =>
            {
                var props = await desc.GetAllAsync().ConfigureAwait(false);
                var normalized = BlueZManager.NormalizeUUID(props.UUID ?? string.Empty);
                var id = Guid.TryParse(normalized, out var g) ? g : Guid.Empty;
                // GattDescriptor1Properties.Flags was added in v6+; fall back to GetAsync for older packages
                string[] flags;
                try { flags = await desc.GetAsync<string[]>("Flags").ConfigureAwait(false) ?? []; }
                catch { flags = []; }
                return (Native: desc, Id: id, Flags: flags);
            })).ConfigureAwait(false);

            var validEntries = descriptorEntries.Where(e => e.Id != Guid.Empty).ToList();

            OnDescriptorsExplorationSucceeded(
                validEntries,
                areRepresentingTheSameObject: (entry, shared) => entry.Id == shared.Id,
                fromInputTypeToOutputTypeConversion: entry =>
                {
                    var spec = new LinuxBluetoothRemoteDescriptorFactorySpec(entry.Id, entry.Native, entry.Flags);
                    return (DescriptorFactory ?? throw new InvalidOperationException("DescriptorFactory must be set")).Create(this, spec);
                });
        }
        catch (Exception ex)
        {
            OnDescriptorsExplorationFailed(ex);
        }
    }

    // BlueZManager.GetProxiesAsync is internal; replicate the query for descriptors using public D-Bus ObjectManager.
    private static async Task<IReadOnlyList<IGattDescriptor1>> GetDescriptorsAsync(IGattCharacteristic1 characteristic)
    {
        var objectManager = Tmds.DBus.Connection.System.CreateProxy<IObjectManager>(BluezConstants.DbusService, "/");
        var objects = await objectManager.GetManagedObjectsAsync().ConfigureAwait(false);
        var charPath = characteristic.ObjectPath.ToString();

        return objects
            .Where(obj =>
                obj.Key.ToString().StartsWith($"{charPath}/") &&
                obj.Value.ContainsKey(BluezConstants.GattDescriptorInterface))
            .Select(obj => Tmds.DBus.Connection.System.CreateProxy<IGattDescriptor1>(BluezConstants.DbusService, obj.Key))
            .ToList<IGattDescriptor1>();
    }

    #endregion

    #region Reliable Write (unsupported)

    /// <inheritdoc />
    protected override ValueTask NativeBeginReliableWriteAsync() =>
        ValueTask.FromException(new NotSupportedException("Reliable writes are not supported on Linux via BlueZ."));

    /// <inheritdoc />
    protected override ValueTask NativeExecuteReliableWriteAsync() =>
        ValueTask.FromException(new NotSupportedException("Reliable writes are not supported on Linux via BlueZ."));

    /// <inheritdoc />
    protected override ValueTask NativeAbortReliableWriteAsync() =>
        ValueTask.FromException(new NotSupportedException("Reliable writes are not supported on Linux via BlueZ."));

    #endregion

    #region Dispose

    /// <inheritdoc />
    protected override async ValueTask DisposeAsyncCore()
    {
        _propertyWatcher?.Dispose();
        _propertyWatcher = null;
        await base.DisposeAsyncCore().ConfigureAwait(false);
    }

    #endregion
}
