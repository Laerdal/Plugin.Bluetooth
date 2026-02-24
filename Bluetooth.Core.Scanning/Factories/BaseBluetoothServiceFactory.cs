using Microsoft.Extensions.Logging;

namespace Bluetooth.Core.Scanning.Factories;

/// <inheritdoc />
public abstract class BaseBluetoothServiceFactory : IBluetoothRemoteServiceFactory
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="BaseBluetoothServiceFactory" /> class.
    /// </summary>
    /// <param name="characteristicFactory">The characteristic factory to pass to the new Service.</param>
    /// <param name="loggerFactory">Optional logger factory for creating loggers.</param>
    protected BaseBluetoothServiceFactory(
        IBluetoothRemoteCharacteristicFactory characteristicFactory,
        ILoggerFactory? loggerFactory = null)
    {
        CharacteristicFactory = characteristicFactory;
        LoggerFactory = loggerFactory;
    }

    /// <summary>
    ///     Gets the characteristic factory to pass to the new Service.
    /// </summary>
    protected IBluetoothRemoteCharacteristicFactory CharacteristicFactory { get; }

    /// <summary>
    ///     Gets the logger factory used to create loggers for services.
    /// </summary>
    protected ILoggerFactory? LoggerFactory { get; }

    /// <inheritdoc />
    public abstract IBluetoothRemoteService Create(IBluetoothRemoteDevice device, IBluetoothRemoteServiceFactory.BluetoothRemoteServiceFactorySpec spec);
}
