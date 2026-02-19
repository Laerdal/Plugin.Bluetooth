using Bluetooth.Core.Scanning.Factories;

namespace Bluetooth.Maui.Platforms.Windows.Scanning.Factories;

/// <inheritdoc/>
public class BluetoothCharacteristicFactory : BaseBluetoothCharacteristicFactory
{
    /// <inheritdoc/>
    public BluetoothCharacteristicFactory(IBluetoothDescriptorFactory descriptorFactory) : base(descriptorFactory)
    {
    }

    /// <inheritdoc/>
    public override Abstractions.Scanning.IBluetoothRemoteCharacteristic CreateCharacteristic(
        Abstractions.Scanning.IBluetoothRemoteService remoteService,
        IBluetoothCharacteristicFactory.BluetoothCharacteristicFactoryRequest request)
    {
        return new BluetoothCharacteristic(remoteService, request, DescriptorFactory);
    }
}
