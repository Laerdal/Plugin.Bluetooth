
namespace Bluetooth.Core.BaseClasses;

public abstract partial class BaseBluetoothScanner
{
    /// <inheritdoc/>
    public event EventHandler<DeviceListChangedEventArgs>? DeviceListChanged;

    /// <inheritdoc/>
    public event EventHandler<DevicesAddedEventArgs>? DevicesAdded;

    /// <inheritdoc/>
    public event EventHandler<DevicesRemovedEventArgs>? DevicesRemoved;

    /// <summary>
    /// Gets the collection of Bluetooth devices managed by this scanner.
    /// </summary>
    protected ObservableCollection<IBluetoothDevice> Devices
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
}
