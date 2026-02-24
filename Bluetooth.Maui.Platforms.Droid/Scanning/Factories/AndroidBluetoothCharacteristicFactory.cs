using Bluetooth.Core.Scanning.Factories;
using Microsoft.Extensions.Logging;

namespace Bluetooth.Maui.Platforms.Droid.Scanning.Factories;

/// <inheritdoc />
public class AndroidBluetoothCharacteristicFactory : BaseBluetoothCharacteristicFactory
{
    /// <inheritdoc />
    public AndroidBluetoothCharacteristicFactory(
        IBluetoothRemoteDescriptorFactory descriptorFactory,
        ILoggerFactory? loggerFactory = null)
        : base(descriptorFactory, loggerFactory)
    {
    }

    /// <inheritdoc />
    public override IBluetoothRemoteCharacteristic Create(
        IBluetoothRemoteService remoteService,
        IBluetoothRemoteCharacteristicFactory.BluetoothRemoteCharacteristicFactorySpec spec)
    {
        var logger = LoggerFactory?.CreateLogger<IBluetoothRemoteCharacteristic>();
        return new AndroidBluetoothRemoteCharacteristic(remoteService, spec, DescriptorFactory, logger);
    }
}
