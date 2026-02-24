using Bluetooth.Abstractions.Scanning;
using Bluetooth.Abstractions.Scanning.Factories;
using Bluetooth.Abstractions.Scanning.Options;
using Bluetooth.Core.Scanning.Factories;
using Microsoft.Extensions.Options;

namespace Bluetooth.Maui.Platforms.Droid.Scanning.Factories;

/// <summary>
///     Android implementation of the Bluetooth L2CAP channel factory.
/// </summary>
public class AndroidBluetoothRemoteL2CapChannelFactory : BaseBluetoothRemoteL2CapChannelFactory
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="AndroidBluetoothRemoteL2CapChannelFactory" /> class.
    /// </summary>
    /// <param name="options">Optional configuration options for L2CAP channel timeouts.</param>
    public AndroidBluetoothRemoteL2CapChannelFactory(IOptions<L2CapChannelOptions>? options = null)
        : base(options)
    {
    }

    /// <inheritdoc />
    public override IBluetoothL2CapChannel Create(
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

        return new AndroidBluetoothRemoteL2CapChannel(
                                                      androidDevice,
                                                      nativeSpec.NativeDevice,
                                                      nativeSpec.Psm,
                                                      Options,
                                                      null);
    }
}
