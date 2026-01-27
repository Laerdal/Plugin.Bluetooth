using System.Collections.ObjectModel;

using Bluetooth.Abstractions;
using Bluetooth.Abstractions.Broadcasting;

using Microsoft.Extensions.Logging;

namespace Bluetooth.Core.Broadcasting;

/// <summary>
/// Base class for Bluetooth Low Energy broadcaster implementations that advertise the device's presence.
/// </summary>
/// <remarks>
/// Broadcasters allow a device to act as a BLE peripheral, advertising its presence and services to nearby devices.
/// This is the opposite role of a scanner, which listens for advertisements.
/// </remarks>
public abstract partial class BaseBluetoothBroadcaster : BaseBindableObject, IBluetoothBroadcaster
{
    /// <inheritdoc/>
    public IBluetoothAdapter Adapter { get; }

    /// <inheritdoc/>
    public IBluetoothBroadcastServiceFactory ServiceFactory { get; }

    /// <inheritdoc/>
    public IBluetoothBroadcastClientDeviceFactory DeviceFactory { get; }

    /// <inheritdoc/>
    public IBluetoothPermissionManager PermissionManager { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseBluetoothBroadcaster"/> class.
    /// </summary>
    /// <param name="adapter">The Bluetooth adapter to associate with this broadcaster.</param>
    /// <param name="serviceFactory">The factory for creating broadcast services.</param>
    /// <param name="deviceFactory">The factory for creating broadcast client devices.</param>
    /// <param name="permissionManager">The permission manager for handling Bluetooth permissions.</param>
    /// <param name="logger">The logger instance to use for logging.</param>
    protected BaseBluetoothBroadcaster(IBluetoothAdapter adapter, IBluetoothBroadcastServiceFactory serviceFactory, IBluetoothBroadcastClientDeviceFactory deviceFactory, IBluetoothPermissionManager permissionManager, ILogger? logger = null)
        : base(logger)
    {
        Adapter = adapter;
        ServiceFactory = serviceFactory;
        DeviceFactory = deviceFactory;
        PermissionManager = permissionManager;
        Services = new ReadOnlyDictionary<Guid, IBluetoothBroadcastService>(WritableServiceList);
    }
}
