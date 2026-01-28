using System.Collections.ObjectModel;

using Bluetooth.Abstractions.Broadcasting;
using Bluetooth.Core.Broadcasting.Exceptions;

namespace Bluetooth.Core.Broadcasting;

public abstract partial class BaseBluetoothBroadcaster
{

    protected ReadOnlyDictionary<string, IBluetoothBroadcastClientDevice> ClientDevices { get; }

    private Dictionary<string, IBluetoothBroadcastClientDevice> WritableClientDevicesList { get; } = new Dictionary<string, IBluetoothBroadcastClientDevice>();

    /// <summary>
    /// Gets a connected client device by its unique identifier, or null if not found.
    /// </summary>
    /// <param name="clientId">The unique identifier of the client device.</param>
    /// <returns>The connected client device, or null if not found.</returns>
    public IBluetoothBroadcastClientDevice? GetClientDeviceOrDefault(string clientId)
    {
        return WritableClientDevicesList.GetValueOrDefault(clientId);
    }

    /// <summary>
    /// Gets a connected client device by its unique identifier.
    /// </summary>
    /// <param name="clientId">The unique identifier of the client device.</param>
    /// <returns>The connected client device.</returns>
    /// <exception cref="BroadcasterClientDeviceNotFoundException">Thrown if no client device with the specified ID is found.</exception>
    public IBluetoothBroadcastClientDevice GetClientDevice(string clientId)
    {
        var device = GetClientDeviceOrDefault(clientId);
        if (device == null)
        {
            throw new BroadcasterClientDeviceNotFoundException(this, clientId);
        }
        return device;
    }

    /// <summary>
    /// Gets all connected client devices, optionally filtered by a predicate.
    /// </summary>
    /// <param name="filter">An optional predicate to filter the client devices.</param>
    /// <returns>A collection of connected client devices matching the filter.</returns>
    public IEnumerable<IBluetoothBroadcastClientDevice> GetClientDevices(Func<IBluetoothBroadcastClientDevice, bool>? filter = null)
    {
        filter ??= _ => true;
        return WritableClientDevicesList.Values.Where(filter).ToArray();
    }
}
