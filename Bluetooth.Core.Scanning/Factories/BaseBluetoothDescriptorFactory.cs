namespace Bluetooth.Core.Scanning.Factories;

/// <inheritdoc />
public abstract class BaseBluetoothDescriptorFactory : IBluetoothDescriptorFactory
{
    /// <inheritdoc />
    public abstract IBluetoothRemoteDescriptor CreateDescriptor(IBluetoothRemoteCharacteristic remoteCharacteristic, IBluetoothDescriptorFactory.BluetoothDescriptorFactoryRequest request);
}
