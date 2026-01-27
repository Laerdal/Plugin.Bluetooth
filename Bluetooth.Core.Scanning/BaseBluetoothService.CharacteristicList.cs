
using System.Collections.ObjectModel;
using System.Collections.Specialized;

using Bluetooth.Abstractions.Scanning;
using Bluetooth.Abstractions.Scanning.EventArgs;

namespace Bluetooth.Core.Scanning;

public abstract partial class BaseBluetoothService
{
    /// <inheritdoc/>
    public event EventHandler<CharacteristicsAddedEventArgs>? CharacteristicsAdded;

    /// <inheritdoc/>
    public event EventHandler<CharacteristicsRemovedEventArgs>? CharacteristicsRemoved;

    /// <inheritdoc/>
    public event EventHandler<CharacteristicListChangedEventArgs>? CharacteristicListChanged;

    /// <summary>
    /// Gets the collection of characteristics associated with this Bluetooth service.
    /// </summary>
    /// <remarks>
    /// This collection is lazily initialized and automatically hooks up collection change notifications
    /// to raise the appropriate events (<see cref="CharacteristicsAdded"/>, <see cref="CharacteristicsRemoved"/>, <see cref="CharacteristicListChanged"/>).
    /// </remarks>
    protected ObservableCollection<IBluetoothCharacteristic> Characteristics
    {
        get
        {
            if (field == null)
            {
                field = [];
                field.CollectionChanged += CharacteristicsOnCollectionChanged;
            }

            return field;
        }
    }

    /// <summary>
    /// Handles collection change notifications for the <see cref="Characteristics"/> collection.
    /// </summary>
    /// <param name="sender">The source of the event.</param>
    /// <param name="ea">Event arguments containing the collection change details.</param>
    private void CharacteristicsOnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs ea)
    {
        var listChangedEventArgs = new CharacteristicListChangedEventArgs(ea);
        if (listChangedEventArgs.AddedItems != null)
        {
            CharacteristicsAdded?.Invoke(this, new CharacteristicsAddedEventArgs(listChangedEventArgs.AddedItems));
        }
        if (listChangedEventArgs.RemovedItems != null)
        {
            CharacteristicsRemoved?.Invoke(this, new CharacteristicsRemovedEventArgs(listChangedEventArgs.RemovedItems));
        }
        CharacteristicListChanged?.Invoke(this, listChangedEventArgs);
    }
}
