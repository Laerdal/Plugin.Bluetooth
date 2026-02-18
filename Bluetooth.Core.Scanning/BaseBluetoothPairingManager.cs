using System.Collections.Specialized;

using Microsoft.Extensions.Logging;

using Plugin.BaseTypeExtensions;

namespace Bluetooth.Core.Scanning;

/// <inheritdoc cref="IBluetoothPairingManager" />
public abstract partial class BaseBluetoothPairingManager : BaseBindableObject, IBluetoothPairingManager
{
    /// <inheritdoc />
    public IBluetoothAdapter Adapter { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseBluetoothPairingManager"/> class.
    /// </summary>
    /// <param name="adapter">The Bluetooth adapter associated with this pairing manager.</param>
    /// <param name="logger">The logger instance to use for logging.</param>
    protected BaseBluetoothPairingManager(IBluetoothAdapter adapter, ILogger? logger = null) : base(logger)
    {
        ArgumentNullException.ThrowIfNull(adapter);
        Adapter = adapter;
    }

    /// <inheritdoc />
    public event EventHandler<PairingStateChangedEventArgs>? PairingStateChanged;

    #region Paired Devices List

    private readonly static Func<IBluetoothRemoteDevice, bool> _defaultAcceptAllFilter = _ => true;

    /// <summary>
    /// Event raised when the list of paired devices changes.
    /// </summary>
    public event EventHandler<DeviceListChangedEventArgs>? PairedDeviceListChanged;

    /// <summary>
    /// Event raised when devices are added to the paired devices list.
    /// </summary>
    public event EventHandler<DevicesAddedEventArgs>? PairedDevicesAdded;

    /// <summary>
    /// Event raised when devices are removed from the paired devices list.
    /// </summary>
    public event EventHandler<DevicesRemovedEventArgs>? PairedDevicesRemoved;

    /// <summary>
    /// Gets the collection of paired Bluetooth devices.
    /// </summary>
    /// <remarks>
    /// This collection is automatically maintained and synchronized with the platform's paired device state.
    /// Use the protected methods to add/remove devices from this collection.
    /// </remarks>
    private ObservableCollection<IBluetoothRemoteDevice> PairedDevices
    {
        get
        {
            if (field == null)
            {
                field = [];
                field.CollectionChanged += PairedDevicesOnCollectionChanged;
            }

            return field;
        }
    }

    /// <summary>
    /// Handles collection change notifications for the <see cref="PairedDevices"/> collection.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="ea">Event arguments containing the collection change details.</param>
    private void PairedDevicesOnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs ea)
    {
        var listChangedEventArgs = new DeviceListChangedEventArgs(ea);
        if (listChangedEventArgs.AddedItems != null)
        {
            PairedDevicesAdded?.Invoke(this, new DevicesAddedEventArgs(listChangedEventArgs.AddedItems));
        }
        if (listChangedEventArgs.RemovedItems != null)
        {
            PairedDevicesRemoved?.Invoke(this, new DevicesRemovedEventArgs(listChangedEventArgs.RemovedItems));
        }
        PairedDeviceListChanged?.Invoke(this, listChangedEventArgs);
    }

    /// <inheritdoc />
    public IBluetoothRemoteDevice? GetClosestPairedDeviceOrDefault(Func<IBluetoothRemoteDevice, bool>? filter = null)
    {
        filter ??= _defaultAcceptAllFilter;
        return GetPairedDevices(filter).MaxBy(d => d.SignalStrengthPercent);
    }

    /// <inheritdoc />
    public IBluetoothRemoteDevice? GetPairedDeviceOrDefault(Func<IBluetoothRemoteDevice, bool>? filter = null)
    {
        filter ??= _defaultAcceptAllFilter;
        try
        {
            IBluetoothRemoteDevice? result;
            lock (PairedDevices)
            {
                result = PairedDevices.SingleOrDefault(filter);
            }

            return result;
        }
        catch (InvalidOperationException e)
        {
            lock (PairedDevices)
            {
                var matchingDevices = PairedDevices.Where(filter).ToList();
                throw new InvalidOperationException($"Multiple paired devices found matching criteria. Found {matchingDevices.Count} devices.", e);
            }
        }
    }

    /// <inheritdoc />
    public IBluetoothRemoteDevice? GetPairedDeviceOrDefault(string id)
    {
        return GetPairedDeviceOrDefault(d => d.Id == id);
    }

    /// <inheritdoc />
    public IEnumerable<IBluetoothRemoteDevice> GetPairedDevices(Func<IBluetoothRemoteDevice, bool>? filter = null)
    {
        filter ??= _defaultAcceptAllFilter;

        lock (PairedDevices)
        {
            foreach (var device in PairedDevices)
            {
                if (filter.Invoke(device))
                {
                    yield return device;
                }
            }
        }
    }

    /// <inheritdoc />
    public async ValueTask<IBluetoothRemoteDevice> GetPairedDeviceOrWaitForDeviceToBePairedAsync(Func<IBluetoothRemoteDevice, bool>? filter = null, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        if (GetPairedDeviceOrDefault(filter) is { } device)
        {
            return device;
        }

        return await WaitForDeviceToBePairedAsync(filter, timeout, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask<IBluetoothRemoteDevice> GetPairedDeviceOrWaitForDeviceToBePairedAsync(string id, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        if (GetPairedDeviceOrDefault(id) is { } device)
        {
            return device;
        }

        return await WaitForDeviceToBePairedAsync(id, timeout, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public ValueTask<IBluetoothRemoteDevice> WaitForDeviceToBePairedAsync(string id, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        return WaitForDeviceToBePairedAsync(device => device.Id == id, timeout, cancellationToken);
    }

    /// <inheritdoc />
    public async ValueTask<IBluetoothRemoteDevice> WaitForDeviceToBePairedAsync(Func<IBluetoothRemoteDevice, bool>? filter = null, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        filter ??= _defaultAcceptAllFilter;
        var tcs = new TaskCompletionSource<IBluetoothRemoteDevice>();

        void OnPairedDevicesAdded(object? sender, DevicesAddedEventArgs ea)
        {
            foreach (var device in ea.Items)
            {
                if (filter.Invoke(device))
                {
                    tcs.TrySetResult(device);
                    break;
                }
            }
        }

        try
        {
            PairedDevicesAdded += OnPairedDevicesAdded;
            return await tcs.Task.WaitBetterAsync(timeout, cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            PairedDevicesAdded -= OnPairedDevicesAdded;
        }
    }

    /// <inheritdoc />
    public virtual bool IsPaired(IBluetoothRemoteDevice device)
    {
        ArgumentNullException.ThrowIfNull(device);
        return IsPaired(device.Id);
    }

    /// <inheritdoc />
    public virtual bool IsPaired(string deviceId)
    {
        lock (PairedDevices)
        {
            return PairedDevices.Any(d => d.Id == deviceId);
        }
    }

    #endregion

    /// <summary>
    /// Raises the <see cref="PairingStateChanged"/> event.
    /// </summary>
    /// <param name="device">The device whose pairing state changed.</param>
    /// <param name="isPaired">Indicates whether the device is now paired.</param>
    protected virtual void OnPairingStateChanged(IBluetoothRemoteDevice device, bool isPaired)
    {
        var args = new PairingStateChangedEventArgs(device, isPaired);
        PairingStateChanged?.Invoke(this, args);
    }

    #region Pairing - Pair

    /// <summary>
    /// Gets a value indicating whether a pairing operation is currently in progress.
    /// </summary>
    public bool IsPairing
    {
        get => GetValue(false);
        private set => SetValue(value);
    }

    /// <summary>
    /// Event raised when a pairing operation starts.
    /// </summary>
    public event EventHandler? Pairing;

    /// <summary>
    /// Event raised when a pairing operation completes successfully.
    /// </summary>
    public event EventHandler? Paired;

    private TaskCompletionSource<bool>? PairingTcs
    {
        get => GetValue<TaskCompletionSource<bool>?>(null);
        set => SetValue(value);
    }

    /// <summary>
    /// Called when a pairing attempt succeeds. Completes the pairing task with success result.
    /// </summary>
    /// <param name="device">The device that was successfully paired.</param>
    protected void OnPairSucceeded(IBluetoothRemoteDevice device)
    {
        // Attempt to dispatch success to the TaskCompletionSource
        var success = PairingTcs?.TrySetResult(true) ?? false;
        if (success)
        {
            Paired?.Invoke(this, EventArgs.Empty);
            OnPairingStateChanged(device, true);
            return;
        }

        // If the TaskCompletionSource was already completed, dispatch the exception to the listener
        BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, new InvalidOperationException("Pairing succeeded but no operation was in progress"));
    }

    /// <summary>
    /// Called when a pairing attempt fails. Completes the pairing task with an exception.
    /// </summary>
    /// <param name="e">The exception that occurred during the pairing attempt.</param>
    protected void OnPairFailed(Exception e)
    {
        // Attempt to dispatch exception to the TaskCompletionSource
        var success = PairingTcs?.TrySetException(e) ?? false;
        if (success)
        {
            return;
        }

        // If the TaskCompletionSource was already completed, dispatch the exception to the listener
        BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, e);
    }

    /// <inheritdoc />
    public async virtual ValueTask<bool> PairAsync(IBluetoothRemoteDevice device, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(device);

        // Prevents multiple calls to PairAsync, if already in progress, we merge the calls
        if (PairingTcs is { Task.IsCompleted: false })
        {
            return await PairingTcs.Task.ConfigureAwait(false);
        }

        PairingTcs = new TaskCompletionSource<bool>(); // Reset the TCS
        IsPairing = true; // Set the pairing state to true
        Pairing?.Invoke(this, EventArgs.Empty);

        try // try-catch to dispatch exceptions rising from native call
        {
            await NativePairAsync(device, timeout, cancellationToken).ConfigureAwait(false); // actual native call
        }
        catch (Exception e)
        {
            OnPairFailed(e); // if exception is thrown during pairing, we trigger the failure
        }

        // try-finally to ensure disposal and release of resources
        try
        {
            // Wait for OnPairSucceeded to be called
            return await PairingTcs.Task.WaitBetterAsync(timeout, cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            IsPairing = false; // Set the pairing state to false
            PairingTcs = null;
        }
    }

    /// <summary>
    /// Platform-specific implementation to initiate a pairing operation with the device.
    /// </summary>
    /// <param name="device">The device to pair with.</param>
    /// <param name="timeout">Optional timeout for the operation.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    protected abstract ValueTask NativePairAsync(IBluetoothRemoteDevice device, TimeSpan? timeout = null, CancellationToken cancellationToken = default);

    #endregion

    #region Pairing - Unpair

    /// <summary>
    /// Gets a value indicating whether an unpairing operation is currently in progress.
    /// </summary>
    public bool IsUnpairing
    {
        get => GetValue(false);
        private set => SetValue(value);
    }

    /// <summary>
    /// Event raised when an unpairing operation starts.
    /// </summary>
    public event EventHandler? Unpairing;

    /// <summary>
    /// Event raised when an unpairing operation completes successfully.
    /// </summary>
    public event EventHandler? Unpaired;

    private TaskCompletionSource? UnpairingTcs
    {
        get => GetValue<TaskCompletionSource?>(null);
        set => SetValue(value);
    }

    /// <summary>
    /// Called when an unpairing attempt succeeds. Completes the unpairing task.
    /// </summary>
    /// <param name="device">The device that was successfully unpaired.</param>
    protected void OnUnpairSucceeded(IBluetoothRemoteDevice device)
    {
        // Attempt to dispatch success to the TaskCompletionSource
        var success = UnpairingTcs?.TrySetResult() ?? false;
        if (success)
        {
            Unpaired?.Invoke(this, EventArgs.Empty);
            OnPairingStateChanged(device, false);
            return;
        }

        // If the TaskCompletionSource was already completed, dispatch the exception to the listener
        BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, new InvalidOperationException("Unpairing succeeded but no operation was in progress"));
    }

    /// <summary>
    /// Called when an unpairing attempt fails. Completes the unpairing task with an exception.
    /// </summary>
    /// <param name="e">The exception that occurred during the unpairing attempt.</param>
    protected void OnUnpairFailed(Exception e)
    {
        // Attempt to dispatch exception to the TaskCompletionSource
        var success = UnpairingTcs?.TrySetException(e) ?? false;
        if (success)
        {
            return;
        }

        // If the TaskCompletionSource was already completed, dispatch the exception to the listener
        BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, e);
    }

    /// <inheritdoc />
    public async virtual ValueTask UnpairAsync(IBluetoothRemoteDevice device, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(device);

        // Prevents multiple calls to UnpairAsync, if already in progress, we merge the calls
        if (UnpairingTcs is { Task.IsCompleted: false })
        {
            await UnpairingTcs.Task.ConfigureAwait(false);
            return;
        }

        UnpairingTcs = new TaskCompletionSource(); // Reset the TCS
        IsUnpairing = true; // Set the unpairing state to true
        Unpairing?.Invoke(this, EventArgs.Empty);

        try // try-catch to dispatch exceptions rising from native call
        {
            await NativeUnpairAsync(device, timeout, cancellationToken).ConfigureAwait(false); // actual native call
        }
        catch (Exception e)
        {
            OnUnpairFailed(e); // if exception is thrown during unpairing, we trigger the failure
        }

        // try-finally to ensure disposal and release of resources
        try
        {
            // Wait for OnUnpairSucceeded to be called
            await UnpairingTcs.Task.WaitBetterAsync(timeout, cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            IsUnpairing = false; // Set the unpairing state to false
            UnpairingTcs = null;
        }
    }

    /// <summary>
    /// Platform-specific implementation to initiate an unpairing operation with the device.
    /// </summary>
    /// <param name="device">The device to unpair.</param>
    /// <param name="timeout">Optional timeout for the operation.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    protected abstract ValueTask NativeUnpairAsync(IBluetoothRemoteDevice device, TimeSpan? timeout = null, CancellationToken cancellationToken = default);

    #endregion
}
