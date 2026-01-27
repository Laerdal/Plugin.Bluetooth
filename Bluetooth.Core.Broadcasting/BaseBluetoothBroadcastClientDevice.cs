using Bluetooth.Abstractions.Broadcasting;

namespace Bluetooth.Core.Broadcasting;

/// <summary>
/// Base class for Bluetooth broadcaster devices.
/// </summary>
public abstract class BaseBluetoothBroadcastClientDevice : BaseBindableObject, IBluetoothBroadcastClientDevice
{
    /// <inheritdoc />
    public IBluetoothBroadcaster Broadcaster { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseBluetoothBroadcastClientDevice"/> class.
    /// </summary>
    /// <param name="broadcaster">The broadcaster that owns this client device.</param>
    /// <param name="request">The request for creating the client device.</param>
    protected BaseBluetoothBroadcastClientDevice(IBluetoothBroadcaster broadcaster, IBluetoothBroadcastClientDeviceFactory.BluetoothBroadcastClientDeviceFactoryRequest request) : base()
    {
        Broadcaster = broadcaster;
        ArgumentNullException.ThrowIfNull(request);
        ClientId = request.ClientId;
    }

    /// <inheritdoc />
    public string ClientId { get; }

    /// <inheritdoc />
    public abstract ValueTask DisposeAsync();
}
