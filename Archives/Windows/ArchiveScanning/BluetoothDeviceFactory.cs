using Bluetooth.Abstractions.Scanning.Factories;

namespace Bluetooth.Maui.Platforms.Windows.Scanning;

/// <inheritdoc/>
public class BluetoothDeviceFactory : IBluetoothDeviceFactory
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BluetoothDeviceFactory"/> class.
    /// </summary>
    public BluetoothDeviceFactory(IBluetoothServiceFactory serviceFactory)
    {
        ServiceFactory = serviceFactory;
    }

    private IBluetoothServiceFactory ServiceFactory { get; }

    /// <inheritdoc/>
    public IBluetoothDevice CreateDevice(IBluetoothScanner scanner, IBluetoothDeviceFactory.BluetoothDeviceFactoryRequest request)
    {
        return new BluetoothDevice(scanner, request, ServiceFactory);
    }
}
