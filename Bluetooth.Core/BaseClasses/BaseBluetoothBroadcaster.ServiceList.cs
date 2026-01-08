namespace Bluetooth.Core.BaseClasses;

public abstract partial class BaseBluetoothBroadcaster
{
    /// <inheritdoc/>
    public event EventHandler<ServiceListChangedEventArgs>? HostedServiceListChanged;

    /// <inheritdoc/>
    public event EventHandler<ServicesAddedEventArgs>? ServicesAdded;

    /// <inheritdoc/>
    public event EventHandler<ServicesRemovedEventArgs>? ServicesRemoved;

    /// <inheritdoc/>
    public IEnumerable<IBluetoothService> HostedServices
    {
        get
        {
            lock (HostedServicesInternal)
            {
                return HostedServicesInternal.ToList();
            }
        }
    }

    /// <summary>
    /// Gets the internal collection of hosted Bluetooth services managed by this broadcaster.
    /// </summary>
    /// <remarks>
    /// This collection is lazily initialized and automatically hooks up collection change notifications
    /// to raise the appropriate events (<see cref="ServicesAdded"/>, <see cref="ServicesRemoved"/>, <see cref="HostedServiceListChanged"/>).
    /// </remarks>
    protected ObservableCollection<IBluetoothService> HostedServicesInternal
    {
        get
        {
            if (field == null)
            {
                field = [];
                field.CollectionChanged += HostedServicesOnCollectionChanged;
            }

            return field;
        }
    }

    /// <summary>
    /// Handles collection change notifications for the <see cref="HostedServicesInternal"/> collection.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="ea">Event arguments containing the collection change details.</param>
    private void HostedServicesOnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs ea)
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
        HostedServiceListChanged?.Invoke(this, listChangedEventArgs);
    }
}
