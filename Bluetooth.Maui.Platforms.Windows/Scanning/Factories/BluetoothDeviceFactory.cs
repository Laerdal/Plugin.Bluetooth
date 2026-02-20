using Bluetooth.Core.Scanning.Factories;

namespace Bluetooth.Maui.Platforms.Windows.Scanning.Factories;

/// <inheritdoc />
public class BluetoothDeviceFactory : BaseBluetoothDeviceFactory
{
    /// <inheritdoc />
    public BluetoothDeviceFactory(IBluetoothServiceFactory serviceFactory, IBluetoothRssiToSignalStrengthConverter rssiToSignalStrengthConverter)
        : base(serviceFactory, rssiToSignalStrengthConverter)
    {
    }

    /// <inheritdoc />
    public override IBluetoothRemoteDevice CreateDevice(IBluetoothScanner scanner, IBluetoothDeviceFactory.BluetoothDeviceFactoryRequest request)
    {
        return new BluetoothDevice(scanner, request, ServiceFactory, RssiToSignalStrengthConverter);
    }
}