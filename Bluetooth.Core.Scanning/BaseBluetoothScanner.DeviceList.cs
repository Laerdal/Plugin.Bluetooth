using Plugin.BaseTypeExtensions;

namespace Bluetooth.Core.Scanning;

public abstract partial class BaseBluetoothScanner
{
    /// <summary>
    /// Gets the collection of Bluetooth devices managed by this scanner.
    /// </summary>
    /// <remarks>
    /// This collection is lazily initialized and automatically hooks up collection change notifications
    /// to raise the appropriate events (<see cref="DevicesAdded"/>, <see cref="DevicesRemoved"/>, <see cref="DeviceListChanged"/>).
    /// </remarks>
    private ObservableCollection<IBluetoothRemoteDevice> Devices
    {
        get
        {
            if (field == null)
            {
                field = [];
                field.CollectionChanged += DevicesOnCollectionChanged;
            }

            return field;
        }
    }

    #region Devices - Events

    /// <summary>
    /// Handles collection change notifications for the <see cref="Devices"/> collection.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="ea">Event arguments containing the collection change details.</param>
    private void DevicesOnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs ea)
    {
        var listChangedEventArgs = new DeviceListChangedEventArgs(ea);
        if (listChangedEventArgs.AddedItems != null)
        {
            DevicesAdded?.Invoke(this, new DevicesAddedEventArgs(listChangedEventArgs.AddedItems));
        }
        if (listChangedEventArgs.RemovedItems != null)
        {
            DevicesRemoved?.Invoke(this, new DevicesRemovedEventArgs(listChangedEventArgs.RemovedItems));
        }
        DeviceListChanged?.Invoke(this, listChangedEventArgs);
    }

    /// <inheritdoc/>
    public event EventHandler<DeviceListChangedEventArgs>? DeviceListChanged;

    /// <inheritdoc/>
    public event EventHandler<DevicesAddedEventArgs>? DevicesAdded;

    /// <inheritdoc/>
    public event EventHandler<DevicesRemovedEventArgs>? DevicesRemoved;

    #endregion

    #region Devices - Clear

    /// <inheritdoc/>
    public async ValueTask ClearDevicesAsync(IEnumerable<IBluetoothRemoteDevice>? devices = null)
    {
        devices ??= GetDevices().ToList();
        foreach (var device in devices)
        {
            await ClearDeviceAsync(device).ConfigureAwait(false);
        }
    }

    /// <inheritdoc/>
    public async ValueTask ClearDeviceAsync(IBluetoothRemoteDevice? device)
    {
        if (device == null)
        {
            return;
        }

        if (device.IsConnected)
        {
            await device.DisconnectAsync().ConfigureAwait(false);
        }

        lock (Devices)
        {
            Devices.Remove(device);
        }

        await device.DisposeAsync().ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public ValueTask ClearDeviceAsync(string deviceId)
    {
        return ClearDeviceAsync(GetDeviceOrDefault(deviceId));
    }

    #endregion

    #region Devices - Get

    /// <inheritdoc/>
    public IBluetoothRemoteDevice GetDevice(Func<IBluetoothRemoteDevice, bool> filter)
    {
        return GetDeviceOrDefault(filter) ?? throw new DeviceNotFoundException(this);
    }

    /// <inheritdoc/>
    public IBluetoothRemoteDevice GetDevice(string id)
    {
        return GetDeviceOrDefault(id) ?? throw new DeviceNotFoundException(this);
    }

    /// <inheritdoc/>
    public IBluetoothRemoteDevice? GetDeviceOrDefault(Func<IBluetoothRemoteDevice, bool> filter)
    {
        try
        {
            return Devices.SingleOrDefault(filter);
        }
        catch (InvalidOperationException e)
        {
            throw new MultipleDevicesFoundException(this, Devices.Where(filter).ToList(), e);
        }
    }

    /// <inheritdoc/>
    public IBluetoothRemoteDevice? GetDeviceOrDefault(string id)
    {
        try
        {
            return Devices.SingleOrDefault(d => d.Id == id);
        }
        catch (InvalidOperationException e)
        {
            throw new MultipleDevicesFoundException(this, id, Devices.Where(d => d.Id == id).ToList(), e);
        }
    }

    private readonly static Func<IBluetoothRemoteDevice, bool> _defaultAcceptAllFilter = _ => true;

    /// <inheritdoc/>
    public IEnumerable<IBluetoothRemoteDevice> GetDevices(Func<IBluetoothRemoteDevice, bool>? filter = null)
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

    #endregion

    #region Devices - Extras

    /// <inheritdoc/>
    public ValueTask<IBluetoothRemoteDevice> WaitForDeviceToAppearAsync(string id, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        return WaitForDeviceToAppearAsync(device => device.Id == id, timeout, cancellationToken);
    }

    /// <inheritdoc/>
    public async ValueTask<IBluetoothRemoteDevice> WaitForDeviceToAppearAsync(Func<IBluetoothRemoteDevice, bool>? filter = null, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        filter ??= _defaultAcceptAllFilter;
        var tcs = new TaskCompletionSource<IBluetoothRemoteDevice>();

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

    /// <inheritdoc/>
    public IBluetoothRemoteDevice? GetClosestDeviceOrDefault(Func<IBluetoothRemoteDevice, bool>? filter = null)
    {
        filter ??= _defaultAcceptAllFilter;

        lock (Devices)
        {
            return Devices.Where(filter).OrderByDescending(d => d.SignalStrengthPercent).FirstOrDefault();
        }
    }

    #endregion

    /// <inheritdoc/>
    public bool HasDevice(Func<IBluetoothRemoteDevice, bool> filter)
    {
        return Devices.Any(filter);
    }

    /// <inheritdoc/>
    public bool HasDevice(string id)
    {
        return HasDevice(d => d.Id == id);
    }

}
