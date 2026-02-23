namespace Bluetooth.Core.Scanning;

public abstract partial class BaseBluetoothRemoteDevice
{
    private TaskCompletionSource<int>? RequestMtuTcs
    {
        get => GetValue<TaskCompletionSource<int>?>(null);
        set => SetValue(value);
    }

    /// <inheritdoc />
    public int Mtu
    {
        get => GetValue(23); // Default MTU is 23 bytes
        private set => SetValue(value);
    }

    /// <inheritdoc />
    public event EventHandler<MtuChangedEventArgs>? MtuChanged;

    /// <inheritdoc />
    public async ValueTask<int> RequestMtuAsync(int requestedMtu, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        // Prevents multiple calls
        if (RequestMtuTcs is { Task.IsCompleted: false })
        {
            return await RequestMtuTcs.Task.ConfigureAwait(false);
        }

        RequestMtuTcs = new TaskCompletionSource<int>(TaskCreationOptions.RunContinuationsAsynchronously);

        try
        {
            await NativeRequestMtuAsync(requestedMtu).ConfigureAwait(false);
        }
        catch (Exception e)
        {
            OnRequestMtuFailed(e);
        }

        try
        {
            return await RequestMtuTcs.Task.WaitBetterAsync(timeout, cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            RequestMtuTcs = null;
        }
    }

    /// <summary>
    ///     Platform-specific implementation to request MTU.
    /// </summary>
    protected abstract ValueTask NativeRequestMtuAsync(int requestedMtu);

    /// <summary>
    ///     Called when MTU request succeeds or MTU changes.
    /// </summary>
    protected void OnMtuChanged(int mtu)
    {
        Mtu = mtu;
        MtuChanged?.Invoke(this, new MtuChangedEventArgs(this, mtu));

        RequestMtuTcs?.TrySetResult(mtu);
    }

    /// <summary>
    ///     Called when MTU request fails.
    /// </summary>
    protected void OnRequestMtuFailed(Exception e)
    {
        var success = RequestMtuTcs?.TrySetException(e) ?? false;
        if (success)
        {
            return;
        }

        BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, e);
    }
}
