
namespace Bluetooth.Core.CharacteristicAccess;

public abstract partial class CharacteristicAccessService<TRead, TWrite>
{
    private readonly Lock _listenersLock = new Lock();

    private readonly List<(IBluetoothCharacteristic, IBluetoothCharacteristicAccessService<TRead, TWrite>.OnNotificationReceived)> _listeners = [];

    /// <inheritdoc />
    public async ValueTask<bool> CanListenAsync(IBluetoothDevice device)
    {
        try
        {
            if (!await HasCharacteristicAsync(device).ConfigureAwait(false))
            {
                return false; // Avoid throwing exceptions if the characteristic doesn't exist
            }

            var characteristic = await GetCharacteristicAsync(device).ConfigureAwait(false);
            return characteristic.CanListen;
        }
        catch (Exception ex)
        {
            // LOG : ERROR - Error checking if characteristic can be listened
            _ = ex; // Suppress unused variable warning
            return false;
        }
    }

    /// <inheritdoc />
    public async Task SubscribeAsync(IBluetoothDevice device, IBluetoothCharacteristicAccessService<TRead, TWrite>.OnNotificationReceived listener)
    {
        var characteristic = await GetCharacteristicAsync(device).ConfigureAwait(false);

        var isSubscribed = await IsSubscribedAsync(device, listener).ConfigureAwait(false);
        if (isSubscribed)
        {
            // LOG : WARN - Listener already subscribed
            return;
        }

        lock (_listenersLock)
        {
            _listeners.Add((characteristic, listener));
        }
        await UpdateListeningStateAsync(characteristic).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task UnsubscribeAsync(IBluetoothDevice device, IBluetoothCharacteristicAccessService<TRead, TWrite>.OnNotificationReceived listener)
    {
        var characteristic = await GetCharacteristicAsync(device).ConfigureAwait(false);

        var isSubscribed = await IsSubscribedAsync(device, listener).ConfigureAwait(false);
        if (!isSubscribed)
        {
            // LOG : WARN - Listener already unsubscribed
            return;
        }

        lock (_listenersLock)
        {
            _listeners.Remove((characteristic, listener));
        }
        await UpdateListeningStateAsync(characteristic).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask<bool> IsSubscribedAsync(IBluetoothDevice device, IBluetoothCharacteristicAccessService<TRead, TWrite>.OnNotificationReceived listener)
    {
        try
        {
            // Check if the listener exists for the correct characteristic
            lock (_listenersLock)
            {
                if (_listeners.Any(tuple => tuple.Item2 == listener && tuple.Item1.Service.Device.Equals(device)))
                {
                    return true;
                }
            }

            // If not found, check if the characteristic exists
            if (!await HasCharacteristicAsync(device).ConfigureAwait(false))
            {
                return false;
            }

            var characteristic = await GetCharacteristicAsync(device).ConfigureAwait(false);

            // Ensure listener is registered with this characteristic
            lock (_listenersLock)
            {
                return _listeners.Any(tuple => tuple.Item1.Equals(characteristic) && tuple.Item2 == listener);
            }
        }
        catch (Exception ex)
        {
            // LOG : ERROR - Error checking if listener is subscribed
            _ = ex; // Suppress unused variable warning
            return false;
        }
    }

    /// <inheritdoc />
    public IEnumerable<(IBluetoothCharacteristic, IBluetoothCharacteristicAccessService<TRead, TWrite>.OnNotificationReceived)> GetListeners(IBluetoothDevice device)
    {
        lock (_listenersLock)
        {
            return _listeners.Where(l => l.Item1.Service.Device == device);
        }
    }

    /// <inheritdoc />
    public IEnumerable<IBluetoothCharacteristicAccessService<TRead, TWrite>.OnNotificationReceived> GetListeners(IBluetoothCharacteristic characteristic)
    {
        lock (_listenersLock)
        {
            return _listeners.Where(l => l.Item1 == characteristic).Select(l => l.Item2);
        }
    }

    private readonly SemaphoreSlim _subscriptionSemaphoreSlim = new SemaphoreSlim(initialCount: 1);

    /// <summary>
    /// Updates the listening state of a characteristic based on the number of active listeners.
    /// </summary>
    /// <param name="characteristic">The characteristic to update.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    private async ValueTask UpdateListeningStateAsync(IBluetoothCharacteristic characteristic)
    {
        try
        {
            await _subscriptionSemaphoreSlim.WaitAsync().ConfigureAwait(false);

            var isListening = characteristic.IsListening;
            var hasListeners = GetListeners(characteristic.Service.Device).Any();

            if (isListening && !hasListeners) // No listeners, but we're listening
            {
                characteristic.ValueUpdated -= CharacteristicValueUpdated;
                await characteristic.StopListeningAsync().ConfigureAwait(false);
            }

            if (!isListening && hasListeners) // We have listeners, but we're not listening
            {
                characteristic.ValueUpdated -= CharacteristicValueUpdated; // ensure handler is subscribed exactly
                characteristic.ValueUpdated += CharacteristicValueUpdated; // once per characteristic   just to be safe

                await characteristic.StartListeningAsync().ConfigureAwait(false);
            }
        }
        finally
        {
            _subscriptionSemaphoreSlim.Release();
        }
    }

    /// <summary>
    /// Handles the characteristic value updated event and notifies all registered listeners.
    /// </summary>
    /// <param name="sender">The characteristic that sent the event.</param>
    /// <param name="args">The event arguments containing the updated value.</param>
    private void CharacteristicValueUpdated(object? sender, ValueUpdatedEventArgs args)
    {
        if (sender is not IBluetoothCharacteristic characteristic)
        {
            return; // Should never happen, but just in case
        }

        var allListeners = GetListeners(characteristic).ToArray();
        if (allListeners.Length == 0)
        {
            return; // No listeners to notify
        }

        var obj = FromBytes(characteristic.Value);
        foreach (var listener in allListeners)
        {
            listener.Invoke(obj);
        }
    }

    #region IDisposable

    /// <summary>
    /// Gets a value indicating whether this instance has been disposed.
    /// </summary>
    private bool IsDisposed { get; set; }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Releases the unmanaged resources used by the <see cref="CharacteristicAccessService{TRead, TWrite}"/> and optionally releases the managed resources.
    /// </summary>
    /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!IsDisposed)
        {
            if (disposing)
            {
                _subscriptionSemaphoreSlim.Dispose();
            }

            IsDisposed = true;
        }
    }

    #endregion
}
