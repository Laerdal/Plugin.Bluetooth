namespace Bluetooth.Core.Scanning.Factories;

/// <inheritdoc />
public abstract class BaseBluetoothDescriptorFactory : IBluetoothRemoteDescriptorFactory
{
    /// <inheritdoc />
    public abstract IBluetoothRemoteDescriptor Create(IBluetoothRemoteCharacteristic remoteCharacteristic, IBluetoothRemoteDescriptorFactory.BluetoothRemoteDescriptorFactorySpec spec);
}
