using Bluetooth.Core.Scanning.Factories;

namespace Bluetooth.Maui.Platforms.Win.Scanning.Factories;

/// <inheritdoc/>
public class WindowsBluetoothDeviceFactory : BaseBluetoothDeviceFactory
{
    /// <inheritdoc/>
    public WindowsBluetoothDeviceFactory(IBluetoothServiceFactory serviceFactory, IBluetoothRssiToSignalStrengthConverter rssiToSignalStrengthConverter)
        : base(serviceFactory, rssiToSignalStrengthConverter)
    {
    }

    /// <inheritdoc/>
    public override IBluetoothRemoteDevice CreateDevice(IBluetoothScanner scanner, IBluetoothDeviceFactory.BluetoothDeviceFactoryRequest request)
    {
        return new WindowsBluetoothRemoteDevice(scanner, request, ServiceFactory, RssiToSignalStrengthConverter);
    }
}