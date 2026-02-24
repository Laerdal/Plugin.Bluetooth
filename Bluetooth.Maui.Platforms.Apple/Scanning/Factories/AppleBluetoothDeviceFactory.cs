using Bluetooth.Core.Scanning.Factories;

namespace Bluetooth.Maui.Platforms.Apple.Scanning.Factories;

/// <inheritdoc />
public class AppleBluetoothDeviceFactory : BaseBluetoothDeviceFactory
{
    private readonly IBluetoothRemoteL2CapChannelFactory _l2CapChannelFactory;

    /// <inheritdoc />
    public AppleBluetoothDeviceFactory(IBluetoothServiceFactory serviceFactory, IBluetoothRemoteL2CapChannelFactory l2CapChannelFactory, IBluetoothRssiToSignalStrengthConverter rssiToSignalStrengthConverter) : base(serviceFactory, rssiToSignalStrengthConverter)
    {
        _l2CapChannelFactory = l2CapChannelFactory;
    }

    /// <inheritdoc />
    public override IBluetoothRemoteDevice CreateDevice(IBluetoothScanner scanner, IBluetoothDeviceFactory.BluetoothDeviceFactoryRequest request)
    {
        return new AppleBluetoothRemoteDevice(scanner, request, ServiceFactory, _l2CapChannelFactory, RssiToSignalStrengthConverter);
    }
}
