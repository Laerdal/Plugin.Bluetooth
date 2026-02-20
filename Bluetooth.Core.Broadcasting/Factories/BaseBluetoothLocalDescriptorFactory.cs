namespace Bluetooth.Core.Broadcasting.Factories;

/// <inheritdoc />
public abstract class BaseBluetoothLocalDescriptorFactory : IBluetoothLocalDescriptorFactory
{
    /// <inheritdoc />
    public abstract IBluetoothLocalDescriptor CreateDescriptor(IBluetoothLocalCharacteristic localCharacteristic, IBluetoothLocalDescriptorFactory.BluetoothLocalDescriptorSpec spec);
}