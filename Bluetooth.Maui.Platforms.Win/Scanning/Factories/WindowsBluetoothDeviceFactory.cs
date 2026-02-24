using Bluetooth.Core.Scanning.Factories;
using Microsoft.Extensions.Logging;

namespace Bluetooth.Maui.Platforms.Win.Scanning.Factories;

/// <inheritdoc/>
public class WindowsBluetoothDeviceFactory : BaseBluetoothDeviceFactory
{
    /// <inheritdoc/>
    public WindowsBluetoothDeviceFactory(IBluetoothRemoteServiceFactory serviceFactory, IBluetoothRssiToSignalStrengthConverter rssiToSignalStrengthConverter, ILoggerFactory? loggerFactory = null)
        : base(serviceFactory, rssiToSignalStrengthConverter, loggerFactory)
    {
    }

    /// <inheritdoc/>
    public override IBluetoothRemoteDevice Create(IBluetoothScanner scanner, IBluetoothRemoteDeviceFactory.BluetoothRemoteDeviceFactorySpec spec)
    {
        var logger = LoggerFactory?.CreateLogger<IBluetoothRemoteDevice>();
        return new WindowsBluetoothRemoteDevice(scanner, spec, ServiceFactory, RssiToSignalStrengthConverter, logger);
    }
}
