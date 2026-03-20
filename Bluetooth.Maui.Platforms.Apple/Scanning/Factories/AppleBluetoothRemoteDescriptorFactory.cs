namespace Bluetooth.Maui.Platforms.Apple.Scanning.Factories;

/// <summary>
///     Apple implementation of <see cref="IBluetoothRemoteDescriptorFactory" />.
///     Creates <see cref="AppleBluetoothRemoteDescriptor" /> instances from Apple-specific factory specs.
/// </summary>
public class AppleBluetoothRemoteDescriptorFactory : IBluetoothRemoteDescriptorFactory
{
    private readonly ILoggerFactory? _loggerFactory;

    /// <summary>
    ///     Initializes a new instance of the <see cref="AppleBluetoothRemoteDescriptorFactory" /> class.
    /// </summary>
    /// <param name="loggerFactory">Optional logger factory for creating loggers.</param>
    public AppleBluetoothRemoteDescriptorFactory(ILoggerFactory? loggerFactory = null)
    {
        _loggerFactory = loggerFactory;
    }

    /// <inheritdoc />
    public IBluetoothRemoteDescriptor Create(IBluetoothRemoteCharacteristic characteristic, IBluetoothRemoteDescriptorFactory.BluetoothRemoteDescriptorFactorySpec spec)
    {
        if (spec is not AppleBluetoothRemoteDescriptorFactorySpec appleSpec)
        {
            throw new ArgumentException($"Spec must be {nameof(AppleBluetoothRemoteDescriptorFactorySpec)}", nameof(spec));
        }

        var logger = _loggerFactory?.CreateLogger<IBluetoothRemoteDescriptor>();
        return new AppleBluetoothRemoteDescriptor(characteristic, appleSpec, logger);
    }
}
