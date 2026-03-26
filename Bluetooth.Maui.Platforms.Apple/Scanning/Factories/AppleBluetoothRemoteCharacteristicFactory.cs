namespace Bluetooth.Maui.Platforms.Apple.Scanning.Factories;

/// <summary>
///     Apple implementation of <see cref="IBluetoothRemoteCharacteristicFactory" />.
///     Creates <see cref="AppleBluetoothRemoteCharacteristic" /> instances from Apple-specific factory specs.
/// </summary>
public class AppleBluetoothRemoteCharacteristicFactory : IBluetoothRemoteCharacteristicFactory
{
    private readonly IBluetoothRemoteDescriptorFactory _descriptorFactory;
    private readonly IBluetoothNameProvider? _nameProvider;
    private readonly ILoggerFactory? _loggerFactory;

    /// <summary>
    ///     Initializes a new instance of the <see cref="AppleBluetoothRemoteCharacteristicFactory" /> class.
    /// </summary>
    /// <param name="descriptorFactory">The factory for creating remote descriptors.</param>
    /// <param name="nameProvider">An optional provider for characteristic names.</param>
    /// <param name="loggerFactory">Optional logger factory for creating loggers.</param>
    public AppleBluetoothRemoteCharacteristicFactory(
        IBluetoothRemoteDescriptorFactory descriptorFactory,
        IBluetoothNameProvider? nameProvider = null,
        ILoggerFactory? loggerFactory = null)
    {
        _descriptorFactory = descriptorFactory;
        _nameProvider = nameProvider;
        _loggerFactory = loggerFactory;
    }

    /// <inheritdoc />
    public IBluetoothRemoteCharacteristic Create(IBluetoothRemoteService service, IBluetoothRemoteCharacteristicFactory.BluetoothRemoteCharacteristicFactorySpec spec)
    {
        if (spec is not AppleBluetoothRemoteCharacteristicFactorySpec appleSpec)
        {
            throw new ArgumentException($"Spec must be {nameof(AppleBluetoothRemoteCharacteristicFactorySpec)}", nameof(spec));
        }

        var logger = _loggerFactory?.CreateLogger<IBluetoothRemoteCharacteristic>();
        return new AppleBluetoothRemoteCharacteristic(service, appleSpec, _descriptorFactory, _nameProvider, logger);
    }
}
