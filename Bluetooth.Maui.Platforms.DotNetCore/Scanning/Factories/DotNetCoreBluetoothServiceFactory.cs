using Bluetooth.Core.Scanning.Factories;
using Microsoft.Extensions.Logging;

namespace Bluetooth.Maui.Platforms.DotNetCore.Scanning.Factories;

/// <inheritdoc cref="BaseBluetoothServiceFactory" />
public class DotNetCoreBluetoothServiceFactory : BaseBluetoothServiceFactory, IBluetoothRemoteServiceFactory
{
    /// <inheritdoc />
    /// <exception cref="PlatformNotSupportedException"></exception>
    public DotNetCoreBluetoothServiceFactory(IBluetoothRemoteCharacteristicFactory characteristicFactory, ILoggerFactory? loggerFactory = null) : base(characteristicFactory, loggerFactory)
    {
        throw new PlatformNotSupportedException("This functionality is only supported on Native platforms. You called the shared version.");
    }

    /// <inheritdoc />
    /// <exception cref="PlatformNotSupportedException"></exception>
    public override IBluetoothRemoteService Create(IBluetoothRemoteDevice device, IBluetoothRemoteServiceFactory.BluetoothRemoteServiceFactorySpec spec)
    {
        throw new PlatformNotSupportedException("This functionality is only supported on Native platforms. You called the shared version.");
    }
}
