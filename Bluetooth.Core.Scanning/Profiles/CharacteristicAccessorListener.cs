namespace Bluetooth.Core.Scanning.Profiles;

/// <summary>
///     Coordinates typed notification listeners for a known Bluetooth characteristic.
/// </summary>
/// <typeparam name="TRead">The typed value produced from notification payloads.</typeparam>
public sealed class CharacteristicAccessorListener<TRead> : IBluetoothCharacteristicAccessorListener<TRead>, IAsyncDisposable
{
    private readonly ICharacteristicCodec<TRead, TRead> _codec;
    private readonly Dictionary<IBluetoothRemoteCharacteristic, SubscriptionState> _subscriptions = new();
    private readonly object _subscriptionsLock = new();

    /// <summary>
    ///     Initializes a new instance of the <see cref="CharacteristicAccessorListener{TRead}" /> class.
    /// </summary>
    /// <param name="serviceId">The UUID of the service that owns the target characteristic.</param>
    /// <param name="characteristicId">The UUID of the target characteristic.</param>
    /// <param name="codec">The codec used to decode notification payloads.</param>
    /// <param name="serviceName">Optional known service name used for diagnostics.</param>
    /// <param name="characteristicName">Optional known characteristic name used for diagnostics.</param>
    public CharacteristicAccessorListener(
        Guid serviceId,
        Guid characteristicId,
        ICharacteristicCodec<TRead, TRead> codec,
        string? serviceName = null,
        string? characteristicName = null)
    {
        ArgumentNullException.ThrowIfNull(codec);

        ServiceId = serviceId;
        CharacteristicId = characteristicId;
        _codec = codec;

        ServiceName = string.IsNullOrWhiteSpace(serviceName) ? "Unknown Service" : serviceName;
        CharacteristicName = string.IsNullOrWhiteSpace(characteristicName) ? "Unknown Characteristic" : characteristicName;
    }

    /// <inheritdoc />
    public Guid ServiceId { get; }

    /// <inheritdoc />
    public Guid CharacteristicId { get; }

    /// <inheritdoc />
    public string ServiceName { get; }

    /// <inheritdoc />
    public string CharacteristicName { get; }

    /// <inheritdoc />
    public async ValueTask<IBluetoothRemoteCharacteristic> ResolveCharacteristicAsync(
        IBluetoothRemoteDevice device,
        TimeSpan? timeout = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(device);

        var service = device.GetServiceOrDefault(ServiceId);
        if (service == null)
        {
            await device.ExploreServicesAsync(ServiceExplorationOptions.WithCharacteristics, timeout, cancellationToken).ConfigureAwait(false);
            service = device.GetServiceOrDefault(ServiceId);
        }

        if (service == null)
        {
            throw new CharacteristicAccessorResolutionException(
                ServiceId,
                CharacteristicId,
                $"Could not resolve service '{ServiceName}' ({ServiceId}) on device '{device.Id}'.");
        }

        var characteristic = service.GetCharacteristicOrDefault(CharacteristicId);
        if (characteristic == null)
        {
            await service.ExploreCharacteristicsAsync(null, timeout, cancellationToken).ConfigureAwait(false);
            characteristic = service.GetCharacteristicOrDefault(CharacteristicId);
        }

        if (characteristic == null)
        {
            throw new CharacteristicAccessorResolutionException(
                ServiceId,
                CharacteristicId,
                $"Could not resolve characteristic '{CharacteristicName}' ({CharacteristicId}) in service '{ServiceName}' ({ServiceId}) on device '{device.Id}'.");
        }

        return characteristic;
    }

    /// <inheritdoc />
    public async ValueTask SubscribeAsync(
        IBluetoothRemoteDevice device,
        Action<TRead> listener,
        TimeSpan? timeout = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(listener);

        var characteristic = await ResolveCharacteristicAsync(device, timeout, cancellationToken).ConfigureAwait(false);
        var state = GetOrCreateState(characteristic);

        await state.Gate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            if (!state.TryAddListener(listener))
            {
                return;
            }

            if (state.ListenerCount == 1)
            {
                characteristic.ValueUpdated += state.Handler;

                try
                {
                    await characteristic.StartListeningAsync(timeout, cancellationToken).ConfigureAwait(false);
                }
                catch
                {
                    state.TryRemoveListener(listener);
                    characteristic.ValueUpdated -= state.Handler;
                    throw;
                }
            }
        }
        finally
        {
            state.Gate.Release();
        }
    }

    /// <inheritdoc />
    public async ValueTask UnsubscribeAsync(
        IBluetoothRemoteDevice device,
        Action<TRead> listener,
        TimeSpan? timeout = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(listener);

        var characteristic = await ResolveCharacteristicAsync(device, timeout, cancellationToken).ConfigureAwait(false);
        var state = GetState(characteristic);
        if (state == null)
        {
            return;
        }

        await state.Gate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            state.TryRemoveListener(listener);

            if (state.IsEmpty)
            {
                await StopAndRemoveSubscriptionAsync(characteristic, state, timeout, cancellationToken).ConfigureAwait(false);
            }
        }
        finally
        {
            state.Gate.Release();
        }
    }

    /// <inheritdoc />
    public async ValueTask UnsubscribeAllAsync(
        IBluetoothRemoteDevice device,
        TimeSpan? timeout = null,
        CancellationToken cancellationToken = default)
    {
        var characteristic = await ResolveCharacteristicAsync(device, timeout, cancellationToken).ConfigureAwait(false);
        var state = GetState(characteristic);
        if (state == null)
        {
            return;
        }

        await state.Gate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            state.ClearListeners();
            await StopAndRemoveSubscriptionAsync(characteristic, state, timeout, cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            state.Gate.Release();
        }
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        List<KeyValuePair<IBluetoothRemoteCharacteristic, SubscriptionState>> snapshot;
        lock (_subscriptionsLock)
        {
            snapshot = _subscriptions.ToList();
        }

        foreach (var kvp in snapshot)
        {
            var characteristic = kvp.Key;
            var state = kvp.Value;

            await state.Gate.WaitAsync().ConfigureAwait(false);
            try
            {
                state.ClearListeners();
                await StopAndRemoveSubscriptionAsync(characteristic, state, null, CancellationToken.None).ConfigureAwait(false);
            }
            finally
            {
                state.Gate.Release();
                state.Gate.Dispose();
            }
        }
    }

    private SubscriptionState CreateState(IBluetoothRemoteCharacteristic characteristic)
    {
        return new SubscriptionState(
            (_, args) =>
            {
                try
                {
                    var typedValue = _codec.FromBytes(args.NewValue);
                    var callbacks = GetListenerSnapshot(characteristic);
                    foreach (var callback in callbacks)
                    {
                        callback(typedValue);
                    }
                }
                catch (Exception ex)
                {
                    BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, ex);
                }
            });
    }

    private Action<TRead>[] GetListenerSnapshot(IBluetoothRemoteCharacteristic characteristic)
    {
        SubscriptionState? state;
        lock (_subscriptionsLock)
        {
            _subscriptions.TryGetValue(characteristic, out state);
        }

        return state?.GetListenerSnapshot() ?? [];
    }

    private SubscriptionState GetOrCreateState(IBluetoothRemoteCharacteristic characteristic)
    {
        lock (_subscriptionsLock)
        {
            if (!_subscriptions.TryGetValue(characteristic, out var state))
            {
                state = CreateState(characteristic);
                _subscriptions[characteristic] = state;
            }

            return state;
        }
    }

    private SubscriptionState? GetState(IBluetoothRemoteCharacteristic characteristic)
    {
        lock (_subscriptionsLock)
        {
            return _subscriptions.TryGetValue(characteristic, out var state) ? state : null;
        }
    }

    private async ValueTask StopAndRemoveSubscriptionAsync(
        IBluetoothRemoteCharacteristic characteristic,
        SubscriptionState state,
        TimeSpan? timeout,
        CancellationToken cancellationToken)
    {
        characteristic.ValueUpdated -= state.Handler;

        if (characteristic.IsListening)
        {
            await characteristic.StopListeningAsync(timeout, cancellationToken).ConfigureAwait(false);
        }

        lock (_subscriptionsLock)
        {
            _subscriptions.Remove(characteristic);
        }
    }

    private sealed class SubscriptionState
    {
        private readonly object _listenersLock = new();
        private readonly HashSet<Action<TRead>> _listeners = new();

        public SubscriptionState(EventHandler<ValueUpdatedEventArgs> handler)
        {
            Handler = handler;
        }

        public SemaphoreSlim Gate { get; } = new(1, 1);

        public EventHandler<ValueUpdatedEventArgs> Handler { get; }

        public int ListenerCount
        {
            get
            {
                lock (_listenersLock)
                {
                    return _listeners.Count;
                }
            }
        }

        public bool IsEmpty
        {
            get
            {
                lock (_listenersLock)
                {
                    return _listeners.Count == 0;
                }
            }
        }

        public bool TryAddListener(Action<TRead> listener)
        {
            lock (_listenersLock)
            {
                return _listeners.Add(listener);
            }
        }

        public bool TryRemoveListener(Action<TRead> listener)
        {
            lock (_listenersLock)
            {
                return _listeners.Remove(listener);
            }
        }

        public void ClearListeners()
        {
            lock (_listenersLock)
            {
                _listeners.Clear();
            }
        }

        public Action<TRead>[] GetListenerSnapshot()
        {
            lock (_listenersLock)
            {
                return _listeners.ToArray();
            }
        }
    }
}
