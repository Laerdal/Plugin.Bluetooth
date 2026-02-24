using Microsoft.Extensions.Logging;

namespace Bluetooth.Core.Scanning.Factories;

/// <inheritdoc />
public abstract class BaseBluetoothDeviceFactory : IBluetoothRemoteDeviceFactory
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="BaseBluetoothDeviceFactory" /> class.
    /// </summary>
    protected BaseBluetoothDeviceFactory(
        IBluetoothRemoteServiceFactory serviceFactory,
        IBluetoothRssiToSignalStrengthConverter rssiToSignalStrengthConverter,
        ILoggerFactory? loggerFactory = null)
    {
        ServiceFactory = serviceFactory;
        RssiToSignalStrengthConverter = rssiToSignalStrengthConverter;
        LoggerFactory = loggerFactory;
    }

    /// <summary>
    ///     Gets the service factory to pass to the new Device.
    /// </summary>
    protected IBluetoothRemoteServiceFactory ServiceFactory { get; }

    /// <summary>
    ///     Gets the RSSI to signal strength converter to pass to the new Device.
    /// </summary>
    protected IBluetoothRssiToSignalStrengthConverter RssiToSignalStrengthConverter { get; }

    /// <summary>
    ///     Gets the logger factory used to create loggers for devices.
    /// </summary>
    protected ILoggerFactory? LoggerFactory { get; }

    /// <inheritdoc />
    public abstract IBluetoothRemoteDevice Create(IBluetoothScanner scanner, IBluetoothRemoteDeviceFactory.BluetoothRemoteDeviceFactorySpec spec);
}
