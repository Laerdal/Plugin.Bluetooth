namespace Bluetooth.Core.Scanning;

public abstract partial class BaseBluetoothRemoteDevice
{
    private TaskCompletionSource? SetPreferredPhyTcs
    {
        get => GetValue<TaskCompletionSource?>(null);
        set => SetValue(value);
    }

    /// <inheritdoc />
    public PhyMode CurrentTxPhy
    {
        get => GetValue(PhyMode.Le1M);
        private set => SetValue(value);
    }

    /// <inheritdoc />
    public PhyMode CurrentRxPhy
    {
        get => GetValue(PhyMode.Le1M);
        private set => SetValue(value);
    }

    /// <inheritdoc />
    public event EventHandler<PhyChangedEventArgs>? PhyChanged;

    /// <inheritdoc />
    public async ValueTask SetPreferredPhyAsync(PhyMode txPhy, PhyMode rxPhy, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        // Prevents multiple calls
        if (SetPreferredPhyTcs is { Task.IsCompleted: false })
        {
            await SetPreferredPhyTcs.Task.ConfigureAwait(false);
            return;
        }

        SetPreferredPhyTcs = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

        try
        {
            await NativeSetPreferredPhyAsync(txPhy, rxPhy).ConfigureAwait(false);
        }
        catch (Exception e)
        {
            OnSetPreferredPhyFailed(e);
        }

        try
        {
            await SetPreferredPhyTcs.Task.WaitBetterAsync(timeout, cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            SetPreferredPhyTcs = null;
        }
    }

    /// <summary>
    ///     Platform-specific implementation to set preferred PHY.
    /// </summary>
    protected abstract ValueTask NativeSetPreferredPhyAsync(PhyMode txPhy, PhyMode rxPhy);

    /// <summary>
    ///     Called when PHY changes.
    /// </summary>
    protected void OnPhyChanged(PhyMode txPhy, PhyMode rxPhy)
    {
        CurrentTxPhy = txPhy;
        CurrentRxPhy = rxPhy;
        PhyChanged?.Invoke(this, new PhyChangedEventArgs(this, txPhy, rxPhy));

        SetPreferredPhyTcs?.TrySetResult();
    }

    /// <summary>
    ///     Called when set preferred PHY fails.
    /// </summary>
    protected void OnSetPreferredPhyFailed(Exception e)
    {
        var success = SetPreferredPhyTcs?.TrySetException(e) ?? false;
        if (success)
        {
            return;
        }

        BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, e);
    }
}
