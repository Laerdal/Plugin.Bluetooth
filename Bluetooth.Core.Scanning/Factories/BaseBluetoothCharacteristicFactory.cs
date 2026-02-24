using Microsoft.Extensions.Logging;

namespace Bluetooth.Core.Scanning.Factories;

/// <inheritdoc />
public abstract class BaseBluetoothCharacteristicFactory : IBluetoothRemoteCharacteristicFactory
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="BaseBluetoothCharacteristicFactory" /> class.
    /// </summary>
    /// <param name="descriptorFactory">The descriptor factory to pass to the new Characteristic.</param>
    /// <param name="loggerFactory">Optional logger factory for creating loggers.</param>
    protected BaseBluetoothCharacteristicFactory(
        IBluetoothRemoteDescriptorFactory descriptorFactory,
        ILoggerFactory? loggerFactory = null)
    {
        DescriptorFactory = descriptorFactory;
        LoggerFactory = loggerFactory;
    }

    /// <summary>
    ///     Gets the descriptor factory to pass to the new Characteristic.
    /// </summary>
    protected IBluetoothRemoteDescriptorFactory DescriptorFactory { get; }

    /// <summary>
    ///     Gets the logger factory used to create loggers for characteristics.
    /// </summary>
    protected ILoggerFactory? LoggerFactory { get; }

    /// <inheritdoc />
    public abstract IBluetoothRemoteCharacteristic Create(IBluetoothRemoteService remoteService, IBluetoothRemoteCharacteristicFactory.BluetoothRemoteCharacteristicFactorySpec spec);
}
