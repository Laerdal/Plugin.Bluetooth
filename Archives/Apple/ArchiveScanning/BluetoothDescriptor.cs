using Bluetooth.Abstractions.Scanning.Factories;

namespace Bluetooth.Maui.Platforms.Apple.Scanning;

public partial class BluetoothDescriptor : BaseBluetoothDescriptor
{
    public BluetoothDescriptor(IBluetoothCharacteristic characteristic, IBluetoothDescriptorFactory.BluetoothDescriptorFactoryRequest request) : base(characteristic, request)
    {
        throw new NotImplementedException();
    }
}
