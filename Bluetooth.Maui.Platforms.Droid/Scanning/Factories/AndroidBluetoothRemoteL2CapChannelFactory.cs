using Bluetooth.Abstractions.Scanning;
using Bluetooth.Abstractions.Scanning.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Bluetooth.Maui.Platforms.Droid.Scanning.Factories;

/// <summary>
///     Android implementation of the Bluetooth L2CAP channel factory.
/// </summary>
public class AndroidBluetoothRemoteL2CapChannelFactory : IBluetoothRemoteL2CapChannelFactory
{
    private readonly L2CapChannelOptions? _options;
    private readonly ILoggerFactory? _loggerFactory;

    /// <summary>
    ///     Initializes a new instance of the <see cref="AndroidBluetoothRemoteL2CapChannelFactory" /> class.
    /// </summary>
    /// <param name="options">Optional configuration options for L2CAP channel timeouts.</param>
    /// <param name="loggerFactory">Optional logger factory for creating loggers.</param>
    public AndroidBluetoothRemoteL2CapChannelFactory(
        IOptions<L2CapChannelOptions>? options = null,
        ILoggerFactory? loggerFactory = null)
    {
        _options = options?.Value;
        _loggerFactory = loggerFactory;
    }

    /// <inheritdoc />
    public IBluetoothRemoteL2CapChannel Create(
        IBluetoothRemoteDevice device,
        IBluetoothRemoteL2CapChannelFactory.BluetoothRemoteL2CapChannelFactorySpec spec)
    {
        if (device is not AndroidBluetoothRemoteDevice androidDevice)
        {
            throw new ArgumentException("Device must be AndroidBluetoothRemoteDevice", nameof(device));
        }

        if (spec is not AndroidBluetoothRemoteL2CapChannelFactorySpec nativeSpec)
        {
            throw new ArgumentException("Request must be AndroidBluetoothRemoteL2CapChannelFactorySpec", nameof(spec));
        }

        var logger = _loggerFactory?.CreateLogger<IBluetoothRemoteL2CapChannel>();
        return new AndroidBluetoothRemoteL2CapChannel(
                                                      androidDevice,
                                                      nativeSpec.NativeDevice,
                                                      nativeSpec.Psm,
                                                      _options,
                                                      logger);
    }
}
