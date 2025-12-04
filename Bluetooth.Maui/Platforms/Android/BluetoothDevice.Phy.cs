using Plugin.BaseTypeExtensions;
using Bluetooth.Maui.PlatformSpecific.Exceptions;

namespace Bluetooth.Maui;

public partial class BluetoothDevice
{

    /// <summary>
    /// Gets or sets the task completion source for the current PHY read operation.
    /// </summary>
    private TaskCompletionSource<Tuple<Android.Bluetooth.BluetoothPhy, Android.Bluetooth.BluetoothPhy>>? ReadPhyTcs { get; set; }

    /// <summary>
    /// Reads the current PHY (Physical Layer) configuration asynchronously.
    /// </summary>
    /// <param name="timeout">Optional timeout for the operation.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A tuple containing the TX and RX PHY configurations.</returns>
    /// <exception cref="NotSupportedException">Thrown when called on Android versions below 8.0 (API level 26).</exception>
    /// <exception cref="DeviceNotConnectedException">Thrown when the device is not connected.</exception>
    /// <exception cref="ArgumentNullException">Thrown when BluetoothGattProxy is <c>null</c>.</exception>
    /// <exception cref="System.OperationCanceledException">Thrown when the operation is cancelled via the cancellation token.</exception>
    /// <exception cref="TimeoutException">Thrown when the operation times out.</exception>
    /// <remarks>
    /// This method is only supported on Android 8.0 (API level 26) and later.
    /// If a read operation is already in progress, this method returns the result of that operation.
    /// </remarks>
    public async Task<Tuple<Android.Bluetooth.BluetoothPhy, Android.Bluetooth.BluetoothPhy>> ReadPhyAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        if (ReadPhyTcs != null)
        {
            return await ReadPhyTcs.Task.ConfigureAwait(false); // Merging calls
        }

        if (!OperatingSystem.IsAndroidVersionAtLeast(26))
        {
            throw new NotSupportedException("ReadPhyAsync is only supported on Android 8.0 and later");
        }

        DeviceNotConnectedException.ThrowIfNotConnected(this);
        ArgumentNullException.ThrowIfNull(BluetoothGattProxy);
        ReadPhyTcs = new TaskCompletionSource<Tuple<Android.Bluetooth.BluetoothPhy, Android.Bluetooth.BluetoothPhy>>(TaskCreationOptions.RunContinuationsAsynchronously);
        try
        {
            BluetoothGattProxy.BluetoothGatt.ReadPhy();
        }
        catch (Exception e)
        {
            ReadPhyTcs.TrySetException(e);
        }

        var output = await ReadPhyTcs.Task.WaitBetterAsync(timeout, cancellationToken).ConfigureAwait(false);
        ReadPhyTcs = null;
        return output;
    }

    /// <summary>
    /// Gets or sets the current transmit (TX) PHY configuration.
    /// </summary>
    /// <remarks>
    /// Available on Android 8.0 (API level 26) and later.
    /// </remarks>
    public Android.Bluetooth.BluetoothPhy? TxPhy
    {
        get => GetValue<Android.Bluetooth.BluetoothPhy?>(null);
        protected set => SetValue(value);
    }

    /// <summary>
    /// Gets or sets the current receive (RX) PHY configuration.
    /// </summary>
    /// <remarks>
    /// Available on Android 8.0 (API level 26) and later.
    /// </remarks>
    public Android.Bluetooth.BluetoothPhy? RxPhy
    {
        get => GetValue<Android.Bluetooth.BluetoothPhy?>(null);
        protected set => SetValue(value);
    }


    /// <summary>
    /// Called when a PHY read operation completes on the Android platform.
    /// </summary>
    /// <param name="status">The status of the PHY read operation.</param>
    /// <param name="txPhy">The transmit PHY configuration.</param>
    /// <param name="rxPhy">The receive PHY configuration.</param>
    /// <exception cref="AndroidNativeGattStatusException">Thrown when the status indicates an error.</exception>
    public void OnPhyRead(GattStatus status, Android.Bluetooth.BluetoothPhy txPhy, Android.Bluetooth.BluetoothPhy rxPhy)
    {
        try
        {
            AndroidNativeGattStatusException.ThrowIfNotSuccess(status);

            TxPhy = txPhy;
            RxPhy = rxPhy;

            ReadPhyTcs?.TrySetResult(Tuple.Create(txPhy, rxPhy));
        }
        catch (Exception e)
        {
            // Attempt to dispatch exception to the TaskCompletionSource
            var success = ReadPhyTcs?.TrySetException(e) ?? false;
            if (!success)
            {
                throw; // Failed to declare the Exception, throw it to the calling method
            }
        }
    }

    /// <summary>
    /// Called when the PHY configuration is updated on the Android platform.
    /// </summary>
    /// <param name="status">The status of the PHY update operation.</param>
    /// <param name="txPhy">The new transmit PHY configuration.</param>
    /// <param name="rxPhy">The new receive PHY configuration.</param>
    public void OnPhyUpdate(GattStatus status, Android.Bluetooth.BluetoothPhy txPhy, Android.Bluetooth.BluetoothPhy rxPhy)
    {
        try
        {
            AndroidNativeGattStatusException.ThrowIfNotSuccess(status);

            TxPhy = txPhy;
            RxPhy = rxPhy;

            ReadPhyTcs?.TrySetResult(Tuple.Create(txPhy, rxPhy));
        }
        catch (Exception e)
        {
            // Attempt to dispatch exception to the TaskCompletionSource
            var success = ReadPhyTcs?.TrySetException(e) ?? false;
            if (!success)
            {
                throw; // Failed to declare the Exception, throw it to the calling method
            }
        }
    }
}
