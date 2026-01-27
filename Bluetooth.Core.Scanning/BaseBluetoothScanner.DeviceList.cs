
using System.Collections.ObjectModel;
using System.Collections.Specialized;

using Bluetooth.Abstractions.Scanning;
using Bluetooth.Abstractions.Scanning.EventArgs;

namespace Bluetooth.Core.Scanning;

public abstract partial class BaseBluetoothScanner
{
    /// <inheritdoc/>
    public event EventHandler<DeviceListChangedEventArgs>? DeviceListChanged;

    /// <inheritdoc/>
    public event EventHandler<DevicesAddedEventArgs>? DevicesAdded;

    /// <inheritdoc/>
    public event EventHandler<DevicesRemovedEventArgs>? DevicesRemoved;

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
}
