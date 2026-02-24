using Bluetooth.Core.Scanning.Factories;

namespace Bluetooth.Maui.Platforms.Apple.Scanning.Factories;

/// <inheritdoc />
public class AppleBluetoothCharacteristicFactory : BaseBluetoothCharacteristicFactory
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="AppleBluetoothCharacteristicFactory" /> class.
    /// </summary>
    /// <param name="descriptorFactory">The descriptor factory to pass to the new Characteristic.</param>
    public AppleBluetoothCharacteristicFactory(IBluetoothRemoteDescriptorFactory descriptorFactory) : base(descriptorFactory)
    {
    }

    /// <inheritdoc />
    public override IBluetoothRemoteCharacteristic Create(IBluetoothRemoteService remoteService, IBluetoothRemoteCharacteristicFactory.BluetoothRemoteCharacteristicFactorySpec spec)
    {
        return new AppleBluetoothRemoteCharacteristic(remoteService, spec, DescriptorFactory);
    }
}
