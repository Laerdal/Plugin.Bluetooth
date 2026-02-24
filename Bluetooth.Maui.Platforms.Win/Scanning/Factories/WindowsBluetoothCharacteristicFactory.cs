using Bluetooth.Core.Scanning.Factories;
using Microsoft.Extensions.Logging;

namespace Bluetooth.Maui.Platforms.Win.Scanning.Factories;

/// <inheritdoc/>
public class WindowsBluetoothCharacteristicFactory : BaseBluetoothCharacteristicFactory
{
    /// <inheritdoc/>
    public WindowsBluetoothCharacteristicFactory(IBluetoothRemoteDescriptorFactory descriptorFactory, ILoggerFactory? loggerFactory = null) : base(descriptorFactory, loggerFactory)
    {
    }

    /// <inheritdoc/>
    public override Abstractions.Scanning.IBluetoothRemoteCharacteristic Create(
        Abstractions.Scanning.IBluetoothRemoteService remoteService,
        IBluetoothRemoteCharacteristicFactory.BluetoothRemoteCharacteristicFactorySpec spec)
    {
        var logger = LoggerFactory?.CreateLogger<IBluetoothRemoteCharacteristic>();
        return new WindowsBluetoothRemoteCharacteristic(remoteService, spec, DescriptorFactory, logger);
    }
}
