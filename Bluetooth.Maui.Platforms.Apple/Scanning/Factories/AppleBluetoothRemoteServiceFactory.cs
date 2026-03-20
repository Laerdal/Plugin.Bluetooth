namespace Bluetooth.Maui.Platforms.Apple.Scanning.Factories;

/// <summary>
///     Apple implementation of <see cref="IBluetoothRemoteServiceFactory" />.
///     Creates <see cref="AppleBluetoothRemoteService" /> instances from Apple-specific factory specs.
/// </summary>
public class AppleBluetoothRemoteServiceFactory : IBluetoothRemoteServiceFactory
{
    private readonly IBluetoothRemoteCharacteristicFactory _characteristicFactory;
    private readonly ILoggerFactory? _loggerFactory;

    /// <summary>
    ///     Initializes a new instance of the <see cref="AppleBluetoothRemoteServiceFactory" /> class.
    /// </summary>
    /// <param name="characteristicFactory">The factory for creating remote characteristics.</param>
    /// <param name="loggerFactory">Optional logger factory for creating loggers.</param>
    public AppleBluetoothRemoteServiceFactory(
        IBluetoothRemoteCharacteristicFactory characteristicFactory,
        ILoggerFactory? loggerFactory = null)
    {
        _characteristicFactory = characteristicFactory;
        _loggerFactory = loggerFactory;
    }

    /// <inheritdoc />
    public IBluetoothRemoteService Create(IBluetoothRemoteDevice device, IBluetoothRemoteServiceFactory.BluetoothRemoteServiceFactorySpec spec)
    {
        if (spec is not AppleBluetoothRemoteServiceFactorySpec appleSpec)
        {
            throw new ArgumentException($"Spec must be {nameof(AppleBluetoothRemoteServiceFactorySpec)}", nameof(spec));
        }

        var logger = _loggerFactory?.CreateLogger<IBluetoothRemoteService>();
        return new AppleBluetoothRemoteService(device, appleSpec, _characteristicFactory, logger);
    }
}
