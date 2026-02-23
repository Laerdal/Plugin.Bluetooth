using Bluetooth.Core.Scanning.Factories;

namespace Bluetooth.Maui.Platforms.Droid.Scanning.Factories;

/// <inheritdoc />
public class AndroidBluetoothDeviceFactory : BaseBluetoothDeviceFactory
{
    /// <inheritdoc />
    public AndroidBluetoothDeviceFactory(
        IBluetoothServiceFactory serviceFactory,
        IBluetoothRssiToSignalStrengthConverter rssiToSignalStrengthConverter)
        : base(serviceFactory, rssiToSignalStrengthConverter)
    {
    }

    /// <inheritdoc />
    public override IBluetoothRemoteDevice CreateDevice(
        IBluetoothScanner scanner,
        IBluetoothDeviceFactory.BluetoothDeviceFactoryRequest request)
    {
        return new AndroidBluetoothRemoteDevice(scanner, request, ServiceFactory, RssiToSignalStrengthConverter);
    }
}
