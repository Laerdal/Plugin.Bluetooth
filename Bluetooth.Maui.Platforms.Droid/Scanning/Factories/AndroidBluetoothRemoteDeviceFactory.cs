using Microsoft.Extensions.Logging;

namespace Bluetooth.Maui.Platforms.Droid.Scanning.Factories;

/// <inheritdoc />
public class AndroidBluetoothRemoteDeviceFactory : IBluetoothRemoteDeviceFactory
{
    private readonly IBluetoothRemoteServiceFactory _serviceFactory;
    private readonly IBluetoothRemoteL2CapChannelFactory _l2CapChannelFactory;
    private readonly IBluetoothRssiToSignalStrengthConverter _rssiToSignalStrengthConverter;
    private readonly ILoggerFactory? _loggerFactory;

    /// <inheritdoc />
    public AndroidBluetoothRemoteDeviceFactory(
        IBluetoothRemoteServiceFactory serviceFactory,
        IBluetoothRemoteL2CapChannelFactory l2CapChannelFactory,
        IBluetoothRssiToSignalStrengthConverter rssiToSignalStrengthConverter,
        ILoggerFactory? loggerFactory = null)
    {
        _serviceFactory = serviceFactory;
        _l2CapChannelFactory = l2CapChannelFactory;
        _rssiToSignalStrengthConverter = rssiToSignalStrengthConverter;
        _loggerFactory = loggerFactory;
    }

    /// <inheritdoc />
    public IBluetoothRemoteDevice Create(
        IBluetoothScanner scanner,
        IBluetoothRemoteDeviceFactory.BluetoothRemoteDeviceFactorySpec spec)
    {
        var logger = _loggerFactory?.CreateLogger<IBluetoothRemoteDevice>();
        return new AndroidBluetoothRemoteDevice(scanner, spec, _serviceFactory, _l2CapChannelFactory, _rssiToSignalStrengthConverter, logger);
    }
}
