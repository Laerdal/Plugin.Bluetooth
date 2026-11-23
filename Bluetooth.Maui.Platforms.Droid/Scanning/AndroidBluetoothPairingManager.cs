using Bluetooth.Maui.Platforms.Droid.Logging;

using Application = Android.App.Application;

namespace Bluetooth.Maui.Platforms.Droid.Scanning;

/// <summary>
///     Android implementation of <see cref="IBluetoothPairingManager" />, built on
///     <see cref="BluetoothDevice.CreateBond()" /> and the <see cref="BluetoothDevice.ActionBondStateChanged" /> broadcast.
///     Bonds are owned by the Android Bluetooth stack, not by the app: a bond made here is visible to every other app
///     and survives disconnections, reboots of the peripheral and app restarts.
/// </summary>
public class AndroidBluetoothPairingManager : BaseBluetoothPairingManager, IDisposable
{
    /// <summary>
    ///     Pairing waits for the peripheral and, for passkey based pairing, for the user to answer the system dialog, so an
    ///     unbounded wait would hang the caller forever when the callers do not pass a timeout.
    /// </summary>
    private static readonly TimeSpan DefaultPairTimeout = TimeSpan.FromSeconds(30);

    private static readonly TimeSpan DefaultUnpairTimeout = TimeSpan.FromSeconds(10);

    // BluetoothDevice.EXTRA_REASON is @hide; it carries the HCI/SMP failure reason of a failed bond.
    private const string ExtraReasonKey = "android.bluetooth.device.extra.REASON";

    /// <summary>
    ///     The bond state is also polled while an operation is pending, so an operation still completes if the
    ///     bond state broadcast does not reach the receiver.
    /// </summary>
    private static readonly TimeSpan PollInterval = TimeSpan.FromMilliseconds(250);

    private static readonly TimeSpan PollDuration = TimeSpan.FromSeconds(60);

    private enum PendingOperation
    {
        None,
        Pair,
        Unpair
    }

    private readonly Lock _gate = new();
    private readonly AndroidBluetoothAdapter _androidAdapter;
    private BondStateReceiver? _receiver;
    private PendingOperation _pendingOperation;
    private string? _pendingAddress;
    private IBluetoothRemoteDevice? _pendingDevice;

    /// <summary>
    ///     Initializes a new instance of the <see cref="AndroidBluetoothPairingManager" /> class.
    /// </summary>
    /// <param name="adapter">The Android Bluetooth adapter this pairing manager belongs to.</param>
    /// <param name="logger">The logger instance to use for logging.</param>
    public AndroidBluetoothPairingManager(AndroidBluetoothAdapter adapter, ILogger? logger = null) : base(adapter, logger)
    {
        _androidAdapter = adapter;
    }

    /// <inheritdoc />
    public override ValueTask<bool> PairAsync(IBluetoothRemoteDevice device, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        return base.PairAsync(device, timeout ?? DefaultPairTimeout, cancellationToken);
    }

    /// <inheritdoc />
    public override ValueTask UnpairAsync(IBluetoothRemoteDevice device, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        return base.UnpairAsync(device, timeout ?? DefaultUnpairTimeout, cancellationToken);
    }

    /// <inheritdoc />
    public override bool IsPaired(string deviceId)
    {
        try
        {
            return _androidAdapter.NativeBluetoothAdapter.GetRemoteDevice(deviceId)?.BondState == Bond.Bonded;
        }
        catch (Java.Lang.IllegalArgumentException)
        {
            return false; // not a Bluetooth address, so it cannot be a bonded device
        }
    }

    /// <inheritdoc />
    protected override ValueTask NativePairAsync(IBluetoothRemoteDevice device, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(device);

        var nativeDevice = GetNativeDevice(device);
        EnsureReceiverRegistered();

        switch (nativeDevice.BondState)
        {
            case Bond.Bonded:
                OnPairSucceeded(device);
                return ValueTask.CompletedTask;

            case Bond.Bonding:
                // Someone else (the system UI, another app) already started it - just wait for its outcome.
                SetPending(PendingOperation.Pair, device, nativeDevice.Address);
                _ = PollBondStateAsync(nativeDevice);
                return ValueTask.CompletedTask;

            default:
                // Armed before CreateBond: the broadcast can arrive before CreateBond returns.
                SetPending(PendingOperation.Pair, device, nativeDevice.Address);
                if (!nativeDevice.CreateBond())
                {
                    ClearPending();
                    throw new BondingFailedException(device, "BluetoothDevice.CreateBond() returned false");
                }

                _ = PollBondStateAsync(nativeDevice);
                return ValueTask.CompletedTask;
        }
    }

    /// <inheritdoc />
    protected override ValueTask NativeUnpairAsync(IBluetoothRemoteDevice device, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(device);

        var nativeDevice = GetNativeDevice(device);
        if (nativeDevice.BondState == Bond.None)
        {
            OnUnpairSucceeded(device);
            return ValueTask.CompletedTask;
        }

        EnsureReceiverRegistered();
        SetPending(PendingOperation.Unpair, device, nativeDevice.Address);

        // BluetoothDevice.removeBond() is @hide - no public API removes a bond - so it is reached by reflection. Newer
        // Android versions may block that (hidden API restrictions), and then unpairing is not available there: the
        // failure is reported as a BondingFailedException instead of escaping as a raw Java exception.
        bool removed;
        try
        {
            var removeBond = nativeDevice.Class.GetMethod("removeBond");
            removed = (removeBond?.Invoke(nativeDevice) as Java.Lang.Boolean)?.BooleanValue() ?? false;
        }
        catch (Java.Lang.Exception e)
        {
            // NoSuchMethodException, IllegalAccessException, InvocationTargetException or SecurityException
            throw ClearPendingAndCreate(new BondingFailedException(device, "BluetoothDevice.removeBond() is not available on this Android version", e));
        }

        if (!removed)
        {
            throw ClearPendingAndCreate(new BondingFailedException(device, "BluetoothDevice.removeBond() returned false"));
        }

        _ = PollBondStateAsync(nativeDevice);
        return ValueTask.CompletedTask;
    }

    /// <summary>
    ///     Feeds the pending operation from the polled bond state, the same way the broadcast does.
    /// </summary>
    private async Task PollBondStateAsync(BluetoothDevice nativeDevice)
    {
        try
        {
            var address = nativeDevice.Address;
            var seenBonding = false;
            var deadline = DateTime.UtcNow + PollDuration;
            while (DateTime.UtcNow < deadline)
            {
                await Task.Delay(PollInterval).ConfigureAwait(false);

                bool unpairing;
                lock (_gate)
                {
                    if (_pendingOperation == PendingOperation.None || !string.Equals(_pendingAddress, address, StringComparison.OrdinalIgnoreCase))
                    {
                        return; // finished by the broadcast, or replaced by another operation
                    }

                    unpairing = _pendingOperation == PendingOperation.Unpair;
                }

                var state = nativeDevice.BondState;
                seenBonding |= state == Bond.Bonding;
                if (state == Bond.Bonded || (state == Bond.None && (seenBonding || unpairing)))
                {
                    OnBondStateChanged(address, state, seenBonding ? Bond.Bonding : Bond.None, -1);
                    return;
                }
            }
        }
        catch (Exception e)
        {
            BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, e);
        }
    }

    private BondingFailedException ClearPendingAndCreate(BondingFailedException exception)
    {
        ClearPending();
        return exception;
    }

    private BluetoothDevice GetNativeDevice(IBluetoothRemoteDevice device)
    {
        // The bond is keyed by address, so a native handle built from the Id is as good as the one the scan produced.
        return _androidAdapter.NativeBluetoothAdapter.GetRemoteDevice(device.Id)
               ?? throw new BondingFailedException(device, $"Android does not know a device with address {device.Id}");
    }

    /// <summary>
    ///     Records the operation the bond state changes are matched against. One operation is tracked at a time: the base
    ///     class merges concurrent calls of the same kind, so an earlier operation of the same kind is one whose caller
    ///     gave up (timeout, cancellation) and can be replaced. An operation of the other kind that is still awaited cannot:
    ///     replacing it would leave its caller waiting for a result that nothing delivers, so the new operation fails instead.
    /// </summary>
    private void SetPending(PendingOperation operation, IBluetoothRemoteDevice device, string? address)
    {
        lock (_gate)
        {
            var otherKindIsAwaited = (_pendingOperation, operation) switch
            {
                (PendingOperation.Pair, PendingOperation.Unpair) => IsPairing,
                (PendingOperation.Unpair, PendingOperation.Pair) => IsUnpairing,
                _ => false
            };
            if (otherKindIsAwaited)
            {
                throw new BondingFailedException(device, $"A {(operation == PendingOperation.Pair ? "unpairing" : "pairing")} operation is still in progress");
            }

            _pendingOperation = operation;
            _pendingDevice = device;
            _pendingAddress = address;
        }
    }

    private void ClearPending()
    {
        lock (_gate)
        {
            _pendingOperation = PendingOperation.None;
            _pendingDevice = null;
            _pendingAddress = null;
        }
    }

    #region Bond state broadcast

    private void EnsureReceiverRegistered()
    {
        lock (_gate)
        {
            if (_receiver != null)
            {
                return;
            }

            _receiver = new BondStateReceiver(this);
            using var filter = new IntentFilter(BluetoothDevice.ActionBondStateChanged);
            if (OperatingSystem.IsAndroidVersionAtLeast(33))
            {
                // Only the system can send this protected broadcast, so exporting the receiver exposes nothing.
                Application.Context.RegisterReceiver(_receiver, filter, ReceiverFlags.Exported);
            }
            else
            {
                Application.Context.RegisterReceiver(_receiver, filter);
            }
        }
    }

    private void OnBondStateChanged(string? address, Bond state, Bond previousState, int reason)
    {
        Logger.LogBondStateChanged(address, previousState, state, reason);

        PendingOperation operation;
        IBluetoothRemoteDevice device;
        lock (_gate)
        {
            if (_pendingOperation == PendingOperation.None || _pendingDevice == null || !string.Equals(_pendingAddress, address, StringComparison.OrdinalIgnoreCase))
            {
                return; // a bond change nobody here is waiting for (another app, the system UI, the stack dropping a stale bond)
            }

            var finishesOperation = (_pendingOperation, state) switch
            {
                (PendingOperation.Pair, Bond.Bonded) => true,
                (PendingOperation.Pair, Bond.None) => previousState == Bond.Bonding,
                (PendingOperation.Unpair, Bond.None) => true,
                _ => false
            };
            if (!finishesOperation)
            {
                return; // an intermediate state, the operation goes on
            }

            // Cleared in the same lock hold that matched it: an operation started in between must not be wiped.
            operation = _pendingOperation;
            device = _pendingDevice;
            _pendingOperation = PendingOperation.None;
            _pendingDevice = null;
            _pendingAddress = null;
        }

        // When the caller has already given up (timeout, cancellation) there is nothing to complete: the base class would
        // report the late result as an unexpected operation.
        switch (operation, state)
        {
            case (PendingOperation.Pair, Bond.Bonded) when IsPairing:
                OnPairSucceeded(device);
                break;

            case (PendingOperation.Pair, Bond.None) when IsPairing:
                OnPairFailed(new BondingFailedException(device, $"Bonding failed (reason {reason})"));
                break;

            case (PendingOperation.Unpair, Bond.None) when IsUnpairing:
                OnUnpairSucceeded(device);
                break;
        }
    }

    private sealed class BondStateReceiver(AndroidBluetoothPairingManager owner) : BroadcastReceiver
    {
        public override void OnReceive(Context? context, Intent? intent)
        {
            if (intent?.Action != BluetoothDevice.ActionBondStateChanged)
            {
                return;
            }

            string? address = null;
            try
            {
                var state = (Bond)intent.GetIntExtra(BluetoothDevice.ExtraBondState, (int)Bond.None);
                var previousState = (Bond)intent.GetIntExtra(BluetoothDevice.ExtraPreviousBondState, (int)Bond.None);
                var reason = intent.GetIntExtra(ExtraReasonKey, -1);
                address = GetDevice(intent)?.Address;

                owner.OnBondStateChanged(address, state, previousState, reason);
            }
            catch (Exception e)
            {
                // A throw out of OnReceive would take the whole app down.
                BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(owner, e);
            }
        }

        private static BluetoothDevice? GetDevice(Intent intent)
        {
            if (OperatingSystem.IsAndroidVersionAtLeast(33))
            {
                return intent.GetParcelableExtra(BluetoothDevice.ExtraDevice, Java.Lang.Class.FromType(typeof(BluetoothDevice))) as BluetoothDevice;
            }

#pragma warning disable CA1422 // the typed overload only exists from API 33
            return intent.GetParcelableExtra(BluetoothDevice.ExtraDevice) as BluetoothDevice;
#pragma warning restore CA1422
        }
    }

    #endregion

    /// <inheritdoc />
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    ///     Unregisters the bond state receiver.
    /// </summary>
    /// <param name="disposing">True when called from <see cref="Dispose()" />, false from a finalizer.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!disposing)
        {
            return;
        }

        lock (_gate)
        {
            if (_receiver == null)
            {
                return;
            }

            Application.Context.UnregisterReceiver(_receiver);
            _receiver.Dispose();
            _receiver = null;
        }
    }
}
