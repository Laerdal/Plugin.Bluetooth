using Bluetooth.Core.Broadcasting.Factories;

namespace Bluetooth.Maui.Platforms.DotNetCore.Broadcasting.Factories;

/// <inheritdoc/>
public class DotNetCoreBluetoothConnectedClientDeviceFactory : BaseBluetoothConnectedClientDeviceFactory
{
    /// <inheritdoc />
    /// <exception cref="PlatformNotSupportedException"></exception>
    public DotNetCoreBluetoothConnectedClientDeviceFactory()
    {
        throw new PlatformNotSupportedException("This functionality is only supported on Native platforms. You called the shared version.");
    }

    /// <inheritdoc />
    /// <exception cref="PlatformNotSupportedException"></exception>
    public override IBluetoothConnectedDevice CreateConnectedDevice(IBluetoothBroadcaster broadcaster, IBluetoothConnectedDeviceFactory.BluetoothConnectedDeviceSpec spec)
    {
        throw new PlatformNotSupportedException("This functionality is only supported on Native platforms. You called the shared version.");
    }
}
