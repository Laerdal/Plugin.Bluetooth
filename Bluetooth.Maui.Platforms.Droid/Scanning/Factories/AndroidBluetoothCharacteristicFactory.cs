using Bluetooth.Core.Scanning.Factories;

namespace Bluetooth.Maui.Platforms.Droid.Scanning.Factories;

/// <inheritdoc />
public class AndroidBluetoothCharacteristicFactory : BaseBluetoothCharacteristicFactory
{
    /// <inheritdoc />
    public AndroidBluetoothCharacteristicFactory(IBluetoothRemoteDescriptorFactory descriptorFactory)
        : base(descriptorFactory)
    {
    }

    /// <inheritdoc />
    public override IBluetoothRemoteCharacteristic Create(
        IBluetoothRemoteService remoteService,
        IBluetoothRemoteCharacteristicFactory.BluetoothRemoteCharacteristicFactorySpec spec)
    {
        return new AndroidBluetoothRemoteCharacteristic(remoteService, spec, DescriptorFactory);
    }
}
