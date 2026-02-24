using Bluetooth.Abstractions.Scanning;
using Bluetooth.Abstractions.Scanning.Factories;
using Bluetooth.Abstractions.Scanning.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Bluetooth.Core.Scanning.Factories;

/// <summary>
///     Base implementation of the Bluetooth L2CAP channel factory.
/// </summary>
public abstract class BaseBluetoothRemoteL2CapChannelFactory : IBluetoothRemoteL2CapChannelFactory
{
    /// <summary>
    ///     Gets the options for configuring L2CAP channel timeouts.
    /// </summary>
    protected IOptions<L2CapChannelOptions>? Options { get; }

    /// <summary>
    ///     Gets the logger factory used to create loggers for L2CAP channels.
    /// </summary>
    protected ILoggerFactory? LoggerFactory { get; }

    /// <summary>
    ///     Initializes a new instance of the <see cref="BaseBluetoothRemoteL2CapChannelFactory" /> class.
    /// </summary>
    /// <param name="options">Optional configuration options for L2CAP channel timeouts.</param>
    /// <param name="loggerFactory">Optional logger factory for creating loggers.</param>
    protected BaseBluetoothRemoteL2CapChannelFactory(
        IOptions<L2CapChannelOptions>? options = null,
        ILoggerFactory? loggerFactory = null)
    {
        Options = options;
        LoggerFactory = loggerFactory;
    }

    /// <inheritdoc />
    public abstract IBluetoothL2CapChannel Create(
        IBluetoothRemoteDevice device,
        IBluetoothRemoteL2CapChannelFactory.BluetoothRemoteL2CapChannelFactorySpec spec);
}
