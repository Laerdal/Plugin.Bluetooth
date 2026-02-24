using Bluetooth.Abstractions.Scanning;
using Bluetooth.Abstractions.Scanning.Factories;
using Bluetooth.Core.Scanning.Factories;

namespace Bluetooth.Maui.Platforms.Droid.Scanning.Factories;

/// <summary>
///     Android implementation of the Bluetooth L2CAP channel factory.
/// </summary>
public class AndroidBluetoothRemoteL2CapChannelFactory : BaseBluetoothRemoteL2CapChannelFactory
{
    /// <inheritdoc />
    public override IBluetoothL2CapChannel CreateL2CapChannel(
        IBluetoothRemoteDevice device,
        IBluetoothRemoteL2CapChannelFactory.BluetoothRemoteL2CapChannelFactoryRequest request)
    {
        if (device is not AndroidBluetoothRemoteDevice androidDevice)
            throw new ArgumentException("Device must be AndroidBluetoothRemoteDevice", nameof(device));

        if (request is not AndroidBluetoothRemoteL2CapChannelFactoryRequest androidRequest)
            throw new ArgumentException("Request must be AndroidBluetoothRemoteL2CapChannelFactoryRequest", nameof(request));

        return new AndroidBluetoothRemoteL2CapChannel(
            androidDevice,
            androidRequest.NativeDevice,
            androidRequest.Psm,
            null);
    }
}
