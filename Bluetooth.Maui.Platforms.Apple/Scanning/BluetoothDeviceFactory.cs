namespace Bluetooth.Maui.Platforms.Apple.Scanning;

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
#pragma warning disable CA2000 // Dispose objects before losing scope
        return new BluetoothDevice(scanner, request, ServiceFactory);
#pragma warning restore CA2000 // Dispose objects before losing scope
    }
}
