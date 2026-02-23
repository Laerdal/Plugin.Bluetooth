using Bluetooth.Core.Scanning.Factories;

namespace Bluetooth.Maui.Platforms.DotNetCore.Scanning.Factories;

/// <inheritdoc />
public class DotNetCoreBluetoothServiceFactory : BaseBluetoothServiceFactory, IBluetoothServiceFactory
{
    /// <inheritdoc />
    /// <exception cref="PlatformNotSupportedException"></exception>
    public DotNetCoreBluetoothServiceFactory(IBluetoothCharacteristicFactory characteristicFactory) : base(characteristicFactory)
    {
        throw new PlatformNotSupportedException("This functionality is only supported on Native platforms. You called the shared version.");
    }

    /// <inheritdoc />
    /// <exception cref="PlatformNotSupportedException"></exception>
    public override IBluetoothRemoteService CreateService(IBluetoothRemoteDevice device, IBluetoothServiceFactory.BluetoothServiceFactoryRequest request)
    {
        throw new PlatformNotSupportedException("This functionality is only supported on Native platforms. You called the shared version.");
    }
}
