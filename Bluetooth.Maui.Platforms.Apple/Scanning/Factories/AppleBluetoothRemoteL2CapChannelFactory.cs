using Bluetooth.Abstractions.Scanning;
using Bluetooth.Abstractions.Scanning.Factories;
using Bluetooth.Abstractions.Scanning.Options;
using Bluetooth.Core.Scanning.Factories;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Bluetooth.Maui.Platforms.Apple.Scanning.Factories;

/// <summary>
///     Apple implementation of the Bluetooth L2CAP channel factory.
/// </summary>
public class AppleBluetoothRemoteL2CapChannelFactory : BaseBluetoothRemoteL2CapChannelFactory
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="AppleBluetoothRemoteL2CapChannelFactory" /> class.
    /// </summary>
    /// <param name="options">Optional configuration options for L2CAP channel timeouts.</param>
    /// <param name="loggerFactory">Optional logger factory for creating loggers.</param>
    public AppleBluetoothRemoteL2CapChannelFactory(
        IOptions<L2CapChannelOptions>? options = null,
        ILoggerFactory? loggerFactory = null)
        : base(options, loggerFactory)
    {
    }

    /// <inheritdoc />
    public override IBluetoothL2CapChannel Create(
        IBluetoothRemoteDevice device,
        IBluetoothRemoteL2CapChannelFactory.BluetoothRemoteL2CapChannelFactorySpec spec)
    {
        if (device is not AppleBluetoothRemoteDevice appleDevice)
        {
            throw new ArgumentException("Device must be AppleBluetoothRemoteDevice", nameof(device));
        }

        if (spec is not AppleBluetoothRemoteL2CapChannelFactorySpec nativeSpec)
        {
            throw new ArgumentException("Request must be AppleBluetoothRemoteL2CapChannelFactorySpec", nameof(spec));
        }

        var logger = LoggerFactory?.CreateLogger<IBluetoothL2CapChannel>();
        return new AppleBluetoothRemoteL2CapChannel(appleDevice,
                                                    nativeSpec.NativeChannel,
                                                    Options,
                                                    logger);
    }
}
