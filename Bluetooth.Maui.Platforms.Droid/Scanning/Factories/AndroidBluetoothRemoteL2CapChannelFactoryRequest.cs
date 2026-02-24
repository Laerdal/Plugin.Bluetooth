using Android.Bluetooth;
using Bluetooth.Abstractions.Scanning.Factories;

namespace Bluetooth.Maui.Platforms.Droid.Scanning.Factories;

/// <summary>
///     Android-specific request for creating Bluetooth L2CAP channel instances.
/// </summary>
public record AndroidBluetoothRemoteL2CapChannelFactoryRequest
    : IBluetoothRemoteL2CapChannelFactory.BluetoothRemoteL2CapChannelFactoryRequest
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="AndroidBluetoothRemoteL2CapChannelFactoryRequest" /> class.
    /// </summary>
    /// <param name="psm">The Protocol/Service Multiplexer identifying the L2CAP service.</param>
    /// <param name="nativeDevice">The native Android Bluetooth device.</param>
    public AndroidBluetoothRemoteL2CapChannelFactoryRequest(int psm, BluetoothDevice nativeDevice)
        : base(psm)
    {
        NativeDevice = nativeDevice ?? throw new ArgumentNullException(nameof(nativeDevice));
    }

    /// <summary>
    ///     Gets the native Android Bluetooth device.
    /// </summary>
    public BluetoothDevice NativeDevice { get; }
}
