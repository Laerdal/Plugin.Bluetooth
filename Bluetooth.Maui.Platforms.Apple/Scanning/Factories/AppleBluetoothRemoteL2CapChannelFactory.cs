using Bluetooth.Abstractions.Scanning;
using Bluetooth.Abstractions.Scanning.Factories;
using Bluetooth.Core.Scanning.Factories;

namespace Bluetooth.Maui.Platforms.Apple.Scanning.Factories;

/// <summary>
///     Apple implementation of the Bluetooth L2CAP channel factory.
/// </summary>
public class AppleBluetoothRemoteL2CapChannelFactory : BaseBluetoothRemoteL2CapChannelFactory
{
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

        return new AppleBluetoothRemoteL2CapChannel(appleDevice,
                                                    nativeSpec.NativeChannel);
    }
}
