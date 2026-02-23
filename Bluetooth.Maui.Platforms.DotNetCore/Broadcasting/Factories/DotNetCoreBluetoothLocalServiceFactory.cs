using Bluetooth.Core.Broadcasting.Factories;

namespace Bluetooth.Maui.Platforms.DotNetCore.Broadcasting.Factories;

/// <inheritdoc />
public class DotNetCoreBluetoothLocalServiceFactory : BaseBluetoothLocalServiceFactory
{
    /// <inheritdoc />
    /// <exception cref="PlatformNotSupportedException"></exception>
    public DotNetCoreBluetoothLocalServiceFactory(IBluetoothLocalCharacteristicFactory localCharacteristicFactory) : base(localCharacteristicFactory)
    {
        throw new PlatformNotSupportedException("This functionality is only supported on Native platforms. You called the shared version.");
    }

    /// <inheritdoc />
    /// <exception cref="PlatformNotSupportedException"></exception>
    public override IBluetoothLocalService CreateService(IBluetoothBroadcaster broadcaster, IBluetoothLocalServiceFactory.BluetoothLocalServiceSpec spec)
    {
        throw new PlatformNotSupportedException("This functionality is only supported on Native platforms. You called the shared version.");
    }
}
