using Bluetooth.Maui.Platforms.Droid.Tools;

namespace Bluetooth.Maui.Platforms.Droid.Broadcasting.Factories;

/// <inheritdoc />
public class AndroidBluetoothLocalDescriptorFactory : IBluetoothLocalDescriptorFactory
{
    /// <inheritdoc />
    public IBluetoothLocalDescriptor Create(IBluetoothLocalCharacteristic characteristic, IBluetoothLocalDescriptorFactory.BluetoothLocalDescriptorSpec spec)
    {
        ArgumentNullException.ThrowIfNull(characteristic);
        ArgumentNullException.ThrowIfNull(spec);

        var nativeDescriptor = new BluetoothGattDescriptor(spec.DescriptorId.ToUuid(), GattDescriptorPermission.Read | GattDescriptorPermission.Write);

        return new AndroidBluetoothLocalDescriptor(nativeDescriptor,
                                                   characteristic,
                                                   spec.DescriptorId,
                                                   null,
                                                   spec.Name,
                                                   null);
    }
}
