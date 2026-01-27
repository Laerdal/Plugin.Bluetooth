
using System.Collections.ObjectModel;
using System.Collections.Specialized;

using Bluetooth.Abstractions.Scanning;
using Bluetooth.Abstractions.Scanning.EventArgs;

namespace Bluetooth.Core.Scanning;

public abstract partial class BaseBluetoothDevice
{
    /// <inheritdoc/>
    public event EventHandler<ServicesAddedEventArgs>? ServicesAdded;

    /// <inheritdoc/>
    public event EventHandler<ServicesRemovedEventArgs>? ServicesRemoved;

    /// <inheritdoc/>
    public event EventHandler<ServiceListChangedEventArgs>? ServiceListChanged;

    /// <summary>
    /// The services collection.
    /// </summary>
    protected ObservableCollection<IBluetoothService> Services
    {
        get
        {
            if (field == null)
            {
                field = [];
                field.CollectionChanged += ServicesOnCollectionChanged;
            }

            return field;
        }
    }

    private void ServicesOnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs ea)
    {
        var listChangedEventArgs = new ServiceListChangedEventArgs(ea);
        if (listChangedEventArgs.AddedItems != null)
        {
            ServicesAdded?.Invoke(this, new ServicesAddedEventArgs(listChangedEventArgs.AddedItems));
        }
        if (listChangedEventArgs.RemovedItems != null)
        {
            ServicesRemoved?.Invoke(this, new ServicesRemovedEventArgs(listChangedEventArgs.RemovedItems));
        }
        ServiceListChanged?.Invoke(this, listChangedEventArgs);
    }
}
