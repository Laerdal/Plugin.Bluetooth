using Bluetooth.Core.Scanning.Factories;

namespace Bluetooth.Maui.Platforms.Apple.Scanning.Factories;

/// <inheritdoc />
public class AppleBluetoothCharacteristicFactory : BaseBluetoothCharacteristicFactory
{

    /// <summary>
    /// Initializes a new instance of the <see cref="AppleBluetoothCharacteristicFactory"/> class.
    /// </summary>
    /// <param name="descriptorFactory">The descriptor factory to pass to the new Characteristic.</param>
    public AppleBluetoothCharacteristicFactory(IBluetoothDescriptorFactory descriptorFactory) : base(descriptorFactory)
    {
    }

    /// <inheritdoc />
    public override Abstractions.Scanning.IBluetoothRemoteCharacteristic CreateCharacteristic(Abstractions.Scanning.IBluetoothRemoteService remoteService, IBluetoothCharacteristicFactory.BluetoothCharacteristicFactoryRequest request)
    {
        return new AppleBluetoothRemoteCharacteristic(remoteService, request, DescriptorFactory);
    }
}
