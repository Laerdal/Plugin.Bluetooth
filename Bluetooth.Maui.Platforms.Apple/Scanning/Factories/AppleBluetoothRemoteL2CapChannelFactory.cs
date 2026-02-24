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
    public override IBluetoothL2CapChannel CreateL2CapChannel(
        IBluetoothRemoteDevice device,
        IBluetoothRemoteL2CapChannelFactory.BluetoothRemoteL2CapChannelFactoryRequest request)
    {
        if (device is not AppleBluetoothRemoteDevice appleDevice)
            throw new ArgumentException("Device must be AppleBluetoothRemoteDevice", nameof(device));

        if (request is not AppleBluetoothRemoteL2CapChannelFactoryRequest appleRequest)
            throw new ArgumentException("Request must be AppleBluetoothRemoteL2CapChannelFactoryRequest", nameof(request));

        return new AppleBluetoothRemoteL2CapChannel(
            appleDevice,
            appleRequest.NativeChannel,
            null);
    }
}
