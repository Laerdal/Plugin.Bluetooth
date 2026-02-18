using System.Collections.Concurrent;

using Plugin.BaseTypeExtensions;

namespace Bluetooth.Core.Scanning;

public abstract partial class BaseBluetoothRemoteDevice
{
    #region Properties

    private readonly ConcurrentQueue<int> _rssiHistory = new ConcurrentQueue<int>();

    /// <inheritdoc />
    public int SignalStrengthDbm
    {
        get => GetValue(0);
        private set
        {
            SetValue(value);
            _rssiHistory.Enqueue(value, IsConnected ? Scanner.CurrentScanningOptions.SignalStrengthJitterSmoothing.SmoothingWhenConnected : Scanner.CurrentScanningOptions.SignalStrengthJitterSmoothing.SmoothingOnAdvertisement);
            SignalStrengthPercent = RssiToSignalStrengthConverter.Convert(_rssiHistory.Average());
        }
    }

    /// <inheritdoc />
    public double SignalStrengthPercent
    {
        get => GetValue(0d);
        private set => SetValue(value);
    }

    private TaskCompletionSource<int>? SignalStrengthReadingTcs
    {
        get => GetValue<TaskCompletionSource<int>?>(null);
        set => SetValue(value);
    }

    #endregion

    #region Public API

    /// <inheritdoc />
    public async ValueTask<int> ReadSignalStrengthAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        // Prevents multiple calls to ReadValueAsync, if already reading signal strength, we merge the calls
        if (SignalStrengthReadingTcs is { Task.IsCompleted: false })
        {
            return await SignalStrengthReadingTcs.Task.ConfigureAwait(false);
        }

        SignalStrengthReadingTcs = new TaskCompletionSource<int>(TaskCreationOptions.RunContinuationsAsynchronously); // Reset the TCS

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
            return await SignalStrengthReadingTcs.Task.WaitBetterAsync(timeout, cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            // Reset the reading flag
            SignalStrengthReadingTcs = null;
        }
    }

    #endregion

    #region Callbacks

    /// <summary>
    /// Called when signal strength reading succeeds. Updates the SignalStrengthDbm property and completes the task.
    /// </summary>
    /// <param name="rssi">The signal strength value in dBm.</param>
    protected void OnSignalStrengthRead(int rssi)
    {
        SignalStrengthDbm = rssi;
        SignalStrengthReadingTcs?.TrySetResult(rssi);
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

    #endregion

    #region Native Abstracts

    /// <summary>
    /// Platform-specific implementation to initiate a signal strength reading.
    /// </summary>
    protected abstract void NativeReadSignalStrength();

    #endregion
}
