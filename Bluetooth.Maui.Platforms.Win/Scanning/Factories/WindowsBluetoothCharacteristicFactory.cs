using Bluetooth.Core.Scanning.Factories;

namespace Bluetooth.Maui.Platforms.Win.Scanning.Factories;

/// <inheritdoc/>
public class WindowsBluetoothCharacteristicFactory : BaseBluetoothCharacteristicFactory
{
    /// <inheritdoc/>
    public WindowsBluetoothCharacteristicFactory(IBluetoothRemoteDescriptorFactory descriptorFactory) : base(descriptorFactory)
    {
    }

    /// <inheritdoc/>
    public override Abstractions.Scanning.IBluetoothRemoteCharacteristic Create(
        Abstractions.Scanning.IBluetoothRemoteService remoteService,
        IBluetoothRemoteCharacteristicFactory.BluetoothRemoteCharacteristicFactorySpec spec)
    {
        return new WindowsBluetoothRemoteCharacteristic(remoteService, spec, DescriptorFactory);
    }
}
