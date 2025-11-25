using Plugin.BaseTypeExtensions;
using Bluetooth.Maui.PlatformSpecific.Exceptions;

namespace Bluetooth.Maui;

public partial class BluetoothDevice
{

    private TaskCompletionSource<Tuple<Android.Bluetooth.BluetoothPhy, Android.Bluetooth.BluetoothPhy>>? ReadPhyTcs { get; set; }

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

    public Android.Bluetooth.BluetoothPhy? TxPhy
    {
        get => GetValue<Android.Bluetooth.BluetoothPhy?>(null);
        protected set => SetValue(value);
    }

    public Android.Bluetooth.BluetoothPhy? RxPhy
    {
        get => GetValue<Android.Bluetooth.BluetoothPhy?>(null);
        protected set => SetValue(value);
    }


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
