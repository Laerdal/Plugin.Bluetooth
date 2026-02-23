using Bluetooth.Core.Scanning.Factories;

namespace Bluetooth.Maui.Platforms.DotNetCore.Scanning.Factories;

/// <inheritdoc />
public class DotNetCoreBluetoothDeviceFactory : BaseBluetoothDeviceFactory
{
    /// <inheritdoc />
    /// <exception cref="PlatformNotSupportedException"></exception>
    public DotNetCoreBluetoothDeviceFactory(IBluetoothServiceFactory serviceFactory, IBluetoothRssiToSignalStrengthConverter rssiToSignalStrengthConverter) : base(serviceFactory, rssiToSignalStrengthConverter)
    {
        throw new PlatformNotSupportedException("This functionality is only supported on Native platforms. You called the shared version.");
    }

    /// <inheritdoc />
    /// <exception cref="PlatformNotSupportedException"></exception>
    public override IBluetoothRemoteDevice CreateDevice(IBluetoothScanner scanner, IBluetoothDeviceFactory.BluetoothDeviceFactoryRequest request)
    {
        throw new PlatformNotSupportedException("This functionality is only supported on Native platforms. You called the shared version.");
    }
}
