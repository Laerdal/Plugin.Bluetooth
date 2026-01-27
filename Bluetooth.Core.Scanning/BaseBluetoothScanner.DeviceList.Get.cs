using Bluetooth.Abstractions.Scanning;
using Bluetooth.Abstractions.Scanning.EventArgs;
using Bluetooth.Core.Exceptions;
using Bluetooth.Core.Scanning.Exceptions;

using Plugin.BaseTypeExtensions;

namespace Bluetooth.Core.Scanning;

public abstract partial class BaseBluetoothScanner
{
    private readonly static Func<IBluetoothDevice, bool> _defaultAcceptAllFilter = _ => true;

    /// <inheritdoc/>
    public IBluetoothDevice? GetClosestDeviceOrDefault(Func<IBluetoothDevice, bool>? filter = null)
    {
        filter ??= _defaultAcceptAllFilter;
        return GetDevices(filter).MaxBy(d => d.SignalStrengthPercent);
    }

    /// <inheritdoc/>
    public IBluetoothDevice? GetDeviceOrDefault(Func<IBluetoothDevice, bool>? filter = null)
    {
        filter ??= _defaultAcceptAllFilter;
        try
        {
            IBluetoothDevice? result;
            lock (Devices)
            {
                result = Devices.SingleOrDefault(filter);
            }

            return result;
        }
        catch (InvalidOperationException e)
        {
            lock (Devices)
            {
                throw new MultipleDevicesFoundException(this, Devices.Where(filter), e);
            }
        }
    }

    /// <inheritdoc/>
    public IBluetoothDevice? GetDeviceOrDefault(string id)
    {
        return GetDeviceOrDefault(d => d.Id == id);
    }

    /// <inheritdoc/>
    public IEnumerable<IBluetoothDevice> GetDevices(Func<IBluetoothDevice, bool>? filter = null)
    {
        filter ??= _defaultAcceptAllFilter;

        lock (Devices)
        {
            foreach (var device in Devices)
            {
                if (filter.Invoke(device))
                {
                    yield return device;
                }
            }
        }
    }

    /// <inheritdoc/>
    public async ValueTask<IBluetoothDevice> GetDeviceOrWaitForDeviceToAppearAsync(Func<IBluetoothDevice, bool>? filter = null, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        if (GetDeviceOrDefault(filter) is { } device)
        {
            return device;
        }

        return await WaitForDeviceToAppearAsync(filter, timeout, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async ValueTask<IBluetoothDevice> GetDeviceOrWaitForDeviceToAppearAsync(string id, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        if (GetDeviceOrDefault(id) is { } device)
        {
            return device;
        }

        return await WaitForDeviceToAppearAsync(id, timeout, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public ValueTask<IBluetoothDevice> WaitForDeviceToAppearAsync(string id, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        return WaitForDeviceToAppearAsync(device => device.Id == id, timeout, cancellationToken);
    }

    /// <inheritdoc/>
    public async ValueTask<IBluetoothDevice> WaitForDeviceToAppearAsync(Func<IBluetoothDevice, bool>? filter = null, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        filter ??= _defaultAcceptAllFilter;
        var tcs = new TaskCompletionSource<IBluetoothDevice>();
        void OnDevicesAdded(object? sender, DevicesAddedEventArgs ea)
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
            DevicesAdded += OnDevicesAdded;
            return await tcs.Task.WaitBetterAsync(timeout, cancellationToken).ConfigureAwait(false);

        }
        finally
        {
            DevicesAdded -= OnDevicesAdded;
        }
    }
}
