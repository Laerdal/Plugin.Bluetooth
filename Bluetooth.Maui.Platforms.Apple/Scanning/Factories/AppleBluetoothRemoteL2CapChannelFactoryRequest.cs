using Bluetooth.Abstractions.Scanning.Factories;
using CoreBluetooth;

namespace Bluetooth.Maui.Platforms.Apple.Scanning.Factories;

/// <summary>
///     Apple-specific request for creating Bluetooth L2CAP channel instances.
/// </summary>
public record AppleBluetoothRemoteL2CapChannelFactoryRequest
    : IBluetoothRemoteL2CapChannelFactory.BluetoothRemoteL2CapChannelFactoryRequest
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="AppleBluetoothRemoteL2CapChannelFactoryRequest" /> class.
    /// </summary>
    /// <param name="psm">The Protocol/Service Multiplexer identifying the L2CAP service.</param>
    /// <param name="nativeChannel">The native CoreBluetooth L2CAP channel.</param>
    public AppleBluetoothRemoteL2CapChannelFactoryRequest(int psm, CBL2CapChannel nativeChannel)
        : base(psm)
    {
        NativeChannel = nativeChannel ?? throw new ArgumentNullException(nameof(nativeChannel));
    }

    /// <summary>
    ///     Gets the native CoreBluetooth L2CAP channel.
    /// </summary>
    public CBL2CapChannel NativeChannel { get; }
}
