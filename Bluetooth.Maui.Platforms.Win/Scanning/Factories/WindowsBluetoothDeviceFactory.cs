using Bluetooth.Core.Scanning.Factories;

namespace Bluetooth.Maui.Platforms.Win.Scanning.Factories;

/// <inheritdoc/>
public class WindowsBluetoothDeviceFactory : BaseBluetoothDeviceFactory
{
    /// <inheritdoc/>
    public WindowsBluetoothDeviceFactory(IBluetoothRemoteServiceFactory serviceFactory, IBluetoothRssiToSignalStrengthConverter rssiToSignalStrengthConverter)
        : base(serviceFactory, rssiToSignalStrengthConverter)
    {
    }

    /// <inheritdoc/>
    public override IBluetoothRemoteDevice Create(IBluetoothScanner scanner, IBluetoothRemoteDeviceFactory.BluetoothRemoteDeviceFactorySpec spec)
    {
        return new WindowsBluetoothRemoteDevice(scanner, spec, ServiceFactory, RssiToSignalStrengthConverter);
    }
}
