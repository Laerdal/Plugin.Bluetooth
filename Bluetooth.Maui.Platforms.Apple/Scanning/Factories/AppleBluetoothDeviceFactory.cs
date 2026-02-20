using Bluetooth.Core.Scanning.Factories;

namespace Bluetooth.Maui.Platforms.Apple.Scanning.Factories;

/// <inheritdoc />
public class AppleBluetoothDeviceFactory : BaseBluetoothDeviceFactory
{
    /// <inheritdoc />
    public AppleBluetoothDeviceFactory(IBluetoothServiceFactory serviceFactory, IBluetoothRssiToSignalStrengthConverter rssiToSignalStrengthConverter) : base(serviceFactory, rssiToSignalStrengthConverter)
    {
    }

    /// <inheritdoc />
    public override IBluetoothRemoteDevice CreateDevice(IBluetoothScanner scanner, IBluetoothDeviceFactory.BluetoothDeviceFactoryRequest request)
    {
        return new AppleBluetoothRemoteDevice(scanner, request, ServiceFactory, RssiToSignalStrengthConverter);
    }
}