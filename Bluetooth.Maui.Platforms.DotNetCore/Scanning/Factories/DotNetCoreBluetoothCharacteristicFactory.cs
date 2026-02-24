using Bluetooth.Core.Scanning.Factories;
using Microsoft.Extensions.Logging;

namespace Bluetooth.Maui.Platforms.DotNetCore.Scanning.Factories;

/// <inheritdoc />
public class DotNetCoreBluetoothCharacteristicFactory : BaseBluetoothCharacteristicFactory
{
    /// <inheritdoc />
    /// <exception cref="PlatformNotSupportedException"></exception>
    public DotNetCoreBluetoothCharacteristicFactory(IBluetoothRemoteDescriptorFactory descriptorFactory, ILoggerFactory? loggerFactory = null) : base(descriptorFactory, loggerFactory)
    {
        throw new PlatformNotSupportedException("This functionality is only supported on Native platforms. You called the shared version.");
    }

    /// <inheritdoc />
    /// <exception cref="PlatformNotSupportedException"></exception>
    public override IBluetoothRemoteCharacteristic Create(IBluetoothRemoteService remoteService, IBluetoothRemoteCharacteristicFactory.BluetoothRemoteCharacteristicFactorySpec spec)
    {
        throw new PlatformNotSupportedException("This functionality is only supported on Native platforms. You called the shared version.");
    }
}
