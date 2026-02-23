namespace Bluetooth.Core.Scanning.Factories;

/// <inheritdoc />
public abstract class BaseBluetoothDeviceFactory : IBluetoothDeviceFactory
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="BaseBluetoothDeviceFactory" /> class.
    /// </summary>
    protected BaseBluetoothDeviceFactory(IBluetoothServiceFactory serviceFactory, IBluetoothRssiToSignalStrengthConverter rssiToSignalStrengthConverter)
    {
        ServiceFactory = serviceFactory;
        RssiToSignalStrengthConverter = rssiToSignalStrengthConverter;
    }

    /// <summary>
    ///     Gets the service factory to pass to the new Device.
    /// </summary>
    protected IBluetoothServiceFactory ServiceFactory { get; }

    /// <summary>
    ///     Gets the RSSI to signal strength converter to pass to the new Device.
    /// </summary>
    protected IBluetoothRssiToSignalStrengthConverter RssiToSignalStrengthConverter { get; }

    /// <inheritdoc />
    public abstract IBluetoothRemoteDevice CreateDevice(IBluetoothScanner scanner, IBluetoothDeviceFactory.BluetoothDeviceFactoryRequest request);
}
