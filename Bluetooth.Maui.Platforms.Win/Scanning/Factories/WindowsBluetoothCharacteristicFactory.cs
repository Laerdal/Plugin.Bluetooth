using Bluetooth.Core.Scanning.Factories;

namespace Bluetooth.Maui.Platforms.Win.Scanning.Factories;

/// <inheritdoc/>
public class WindowsBluetoothCharacteristicFactory : BaseBluetoothCharacteristicFactory
{
    /// <inheritdoc/>
    public WindowsBluetoothCharacteristicFactory(IBluetoothDescriptorFactory descriptorFactory) : base(descriptorFactory)
    {
    }

    /// <inheritdoc/>
    public override Abstractions.Scanning.IBluetoothRemoteCharacteristic CreateCharacteristic(
        Abstractions.Scanning.IBluetoothRemoteService remoteService,
        IBluetoothCharacteristicFactory.BluetoothCharacteristicFactoryRequest request)
    {
        return new WindowsBluetoothRemoteCharacteristic(remoteService, request, DescriptorFactory);
    }
}