namespace Bluetooth.Core.BaseClasses;

public abstract partial class BaseBluetoothDevice
{
    // Max and Min are dynamic, meaning if a value higher or lower is detected it will push those bounds
    private static double _closeRssiValue = -50;
    private static double _farRssiValue = -100;

    private readonly ConcurrentQueue<int> _rssiHistory = new ConcurrentQueue<int>();

    /// <summary>
    ///     The algorithm will store the last <see cref="SignalStrengthJitterSmoothingStrengthOnAdvertisement" /> values and
    ///     average them out.
    ///     This has the effect of smoothing out the jitter in the signal strength.
    ///     The higher the value the smoother the signal strength will be, but also the less reactive it will be.
    ///     This is the value to use when not connected : the signal strength is more jittery when not connected.
    /// </summary>
    public static int SignalStrengthJitterSmoothingStrengthOnAdvertisement { get; set; } = 5;

    /// <summary>
    ///     The algorithm will store the last <see cref="SignalStrengthJitterSmoothingStrengthConnected" /> values and average
    ///     them out.
    ///     This has the effect of smoothing out the jitter in the signal strength.
    ///     The higher the value the smoother the signal strength will be, but also the less reactive it will be.
    ///     This is the value to use when connected : the signal strength is more stable when connected.
    /// </summary>
    public static int SignalStrengthJitterSmoothingStrengthConnected { get; set; } = 3;

    /// <summary>
    /// Gets a value indicating whether a signal strength reading operation is currently in progress.
    /// </summary>
    public bool IsReadingSignalStrength
    {
        get => GetValue(false);
        protected set => SetValue(value);
    }

    private TaskCompletionSource? SignalStrengthReadingTcs
    {
        get => GetValue<TaskCompletionSource?>(null);
        set => SetValue(value);
    }

    #region IBluetoothDevice Members

    /// <inheritdoc />
    public int SignalStrengthDbm
    {
        get => GetValue(0);
        private set
        {
            SetValue(value);
            _rssiHistory.Enqueue(value, IsConnected ? SignalStrengthJitterSmoothingStrengthConnected : SignalStrengthJitterSmoothingStrengthOnAdvertisement);
            SignalStrengthPercent = RssiToSignalStrengthConverter(_rssiHistory.Average());
        }
    }

    /// <inheritdoc />
    public double SignalStrengthPercent
    {
        get => GetValue(0d);
        private set => SetValue(value);
    }

    #endregion

    /// <summary>
    ///     Turns a DBM RSSI value into a percentage, compared to the highest RSSI recorded and tha lowest.
    /// </summary>
    /// <param name="rssi">Input in DBM.</param>
    /// <returns>Percentage.</returns>
    protected static double RssiToSignalStrengthConverter(double rssi)
    {
        if (rssi <= _farRssiValue)
        {
            _farRssiValue = rssi;
            return 0;
        }

        if (rssi >= _closeRssiValue)
        {
            _closeRssiValue = rssi;
            return 1;
        }

        // Distance = ( rssi - Far ) / ( Close - Far)

        // Example :
        // Rssi : -64
        // ( -64 - ( -100 )) / (( -26 ) - ( -100 ))
        // (-64+100) / (-26+100)
        // 36/74
        // 0.49
        return (rssi - _farRssiValue) / (_closeRssiValue - _farRssiValue);
    }

    /// <summary>
    /// Called when signal strength reading succeeds. Updates the SignalStrengthDbm property and completes the task.
    /// </summary>
    /// <param name="rssi">The signal strength value in dBm.</param>
    protected void OnSignalStrengthRead(int rssi)
    {
        SignalStrengthDbm = rssi;
        SignalStrengthReadingTcs?.TrySetResult();
    }

    /// <summary>
    /// Called when signal strength reading fails. Completes the task with an exception or dispatches to the unhandled exception listener.
    /// </summary>
    /// <param name="e">The exception that occurred during the signal strength reading.</param>
    protected void OnSignalStrengthReadFailed(Exception e)
    {
        // Attempt to dispatch exception to the TaskCompletionSource
        var success = SignalStrengthReadingTcs?.TrySetException(e) ?? false;
        if (success)
        {
            return;
        }

        // If the TaskCompletionSource was already completed, dispatch the exception to the listener
        BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, e);
    }

    /// <summary>
    /// Platform-specific implementation to initiate a signal strength reading.
    /// </summary>
    protected abstract void NativeReadSignalStrength();

    /// <summary>
    /// Attempts to read the device's signal strength, suppressing any exceptions that occur.
    /// </summary>
    /// <param name="timeout">Optional timeout for the operation.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A task that completes when the signal strength reading attempt finishes.</returns>
    public async Task TryReadSignalStrengthAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        try
        {
            await ReadSignalStrengthAsync(timeout, cancellationToken).ConfigureAwait(false);
        }
        catch (Exception e)
        {
            BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, e);
        }
    }

    /// <inheritdoc/>
    public async Task ReadSignalStrengthAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        // Ensure Device is Connected
        DeviceNotConnectedException.ThrowIfNotConnected(this);

        // Prevents multiple calls to ReadValueAsync, if already reading signal strength, we merge the calls
        if (SignalStrengthReadingTcs is { Task.IsCompleted: false })
        {
            await SignalStrengthReadingTcs.Task.ConfigureAwait(false);
            return;
        }

        SignalStrengthReadingTcs = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously); // Reset the TCS
        IsReadingSignalStrength = true;

        // try-catch to dispatch exceptions rising from start
        try
        {
            // Actual signal strength reading native call
            NativeReadSignalStrength();
        }
        catch (Exception e)
        {
            // if exception is thrown during start, we trigger the failure
            OnSignalStrengthReadFailed(e);
        }

        // try-finally to ensure disposal and release of resources
        try
        {
            // Wait for OnSignalStrengthReadSucceeded to be called
            await SignalStrengthReadingTcs.Task.WaitBetterAsync(timeout, cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            // Reset the reading flag
            IsReadingSignalStrength = false;
            SignalStrengthReadingTcs = null;
        }
    }
}
