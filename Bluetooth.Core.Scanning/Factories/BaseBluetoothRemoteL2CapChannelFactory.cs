using Bluetooth.Abstractions.Scanning;
using Bluetooth.Abstractions.Scanning.Factories;

namespace Bluetooth.Core.Scanning.Factories;

/// <summary>
///     Base implementation of the Bluetooth L2CAP channel factory.
/// </summary>
public abstract class BaseBluetoothRemoteL2CapChannelFactory : IBluetoothRemoteL2CapChannelFactory
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="BaseBluetoothRemoteL2CapChannelFactory" /> class.
    /// </summary>
    protected BaseBluetoothRemoteL2CapChannelFactory()
    {
    }

    /// <inheritdoc />
    public abstract IBluetoothL2CapChannel Create(
        IBluetoothRemoteDevice device,
        IBluetoothRemoteL2CapChannelFactory.BluetoothRemoteL2CapChannelFactorySpec spec);
}
