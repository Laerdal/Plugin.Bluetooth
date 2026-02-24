using Bluetooth.Core.Scanning.Factories;
using Microsoft.Extensions.Logging;

namespace Bluetooth.Maui.Platforms.Apple.Scanning.Factories;

/// <inheritdoc />
public class AppleBluetoothCharacteristicFactory : BaseBluetoothCharacteristicFactory
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="AppleBluetoothCharacteristicFactory" /> class.
    /// </summary>
    /// <param name="descriptorFactory">The descriptor factory to pass to the new Characteristic.</param>
    /// <param name="loggerFactory">Optional logger factory for creating loggers.</param>
    public AppleBluetoothCharacteristicFactory(
        IBluetoothRemoteDescriptorFactory descriptorFactory,
        ILoggerFactory? loggerFactory = null)
        : base(descriptorFactory, loggerFactory)
    {
    }

    /// <inheritdoc />
    public override IBluetoothRemoteCharacteristic Create(IBluetoothRemoteService remoteService, IBluetoothRemoteCharacteristicFactory.BluetoothRemoteCharacteristicFactorySpec spec)
    {
        var logger = LoggerFactory?.CreateLogger<IBluetoothRemoteCharacteristic>();
        return new AppleBluetoothRemoteCharacteristic(remoteService, spec, DescriptorFactory, logger);
    }
}
