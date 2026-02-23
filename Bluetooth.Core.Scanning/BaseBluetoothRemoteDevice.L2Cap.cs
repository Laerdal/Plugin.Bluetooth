namespace Bluetooth.Core.Scanning;

public abstract partial class BaseBluetoothRemoteDevice
{
    private TaskCompletionSource<IBluetoothL2CapChannel>? OpenL2CapChannelTcs
    {
        get => GetValue<TaskCompletionSource<IBluetoothL2CapChannel>?>(null);
        set => SetValue(value);
    }

    /// <inheritdoc />
    public async ValueTask<IBluetoothL2CapChannel> OpenL2CapChannelAsync(int psm, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        // Prevents multiple calls for the same PSM
        if (OpenL2CapChannelTcs is { Task.IsCompleted: false })
        {
            return await OpenL2CapChannelTcs.Task.ConfigureAwait(false);
        }

        OpenL2CapChannelTcs = new TaskCompletionSource<IBluetoothL2CapChannel>(TaskCreationOptions.RunContinuationsAsynchronously);

        try
        {
            await NativeOpenL2CapChannelAsync(psm).ConfigureAwait(false);
        }
        catch (Exception e)
        {
            OnOpenL2CapChannelFailed(e);
        }

        try
        {
            return await OpenL2CapChannelTcs.Task.WaitBetterAsync(timeout, cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            OpenL2CapChannelTcs = null;
        }
    }

    /// <summary>
    ///     Platform-specific implementation to open an L2CAP channel.
    /// </summary>
    protected abstract ValueTask NativeOpenL2CapChannelAsync(int psm);

    /// <summary>
    ///     Called when L2CAP channel is opened successfully.
    /// </summary>
    protected void OnL2CapChannelOpened(IBluetoothL2CapChannel channel)
    {
        ArgumentNullException.ThrowIfNull(channel);
        OpenL2CapChannelTcs?.TrySetResult(channel);
    }

    /// <summary>
    ///     Called when L2CAP channel open fails.
    /// </summary>
    protected void OnOpenL2CapChannelFailed(Exception e)
    {
        var success = OpenL2CapChannelTcs?.TrySetException(e) ?? false;
        if (success)
        {
            return;
        }

        BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, e);
    }
}
