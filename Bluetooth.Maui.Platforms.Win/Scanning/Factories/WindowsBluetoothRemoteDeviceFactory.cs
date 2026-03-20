using Microsoft.Extensions.Logging;

namespace Bluetooth.Maui.Platforms.Win.Scanning.Factories;

/// <inheritdoc/>
public class WindowsBluetoothRemoteDeviceFactory : IBluetoothRemoteDeviceFactory
{
    private readonly IBluetoothRemoteServiceFactory _serviceFactory;
    private readonly IBluetoothRssiToSignalStrengthConverter _rssiToSignalStrengthConverter;
    private readonly ILoggerFactory? _loggerFactory;

    /// <inheritdoc/>
    public WindowsBluetoothRemoteDeviceFactory(IBluetoothRemoteServiceFactory serviceFactory, IBluetoothRssiToSignalStrengthConverter rssiToSignalStrengthConverter, ILoggerFactory? loggerFactory = null)
    {
        _serviceFactory = serviceFactory;
        _rssiToSignalStrengthConverter = rssiToSignalStrengthConverter;
        _loggerFactory = loggerFactory;
    }

    /// <inheritdoc/>
    public IBluetoothRemoteDevice Create(IBluetoothScanner scanner, IBluetoothRemoteDeviceFactory.BluetoothRemoteDeviceFactorySpec spec)
    {
        var logger = _loggerFactory?.CreateLogger<IBluetoothRemoteDevice>();
        return new WindowsBluetoothRemoteDevice(scanner, spec, _serviceFactory, _rssiToSignalStrengthConverter, logger);
    }
}
