// TODO: Uncomment when Core factory infrastructure exists
// using Bluetooth.Core.Broadcasting.Factories;

namespace Bluetooth.Maui.Platforms.Win.Broadcasting.Factories;

/// <inheritdoc />
public class WindowsBluetoothLocalCharacteristicFactory : IBluetoothLocalCharacteristicFactory
{
    private readonly IBluetoothLocalDescriptorFactory _localDescriptorFactory;

    /// <inheritdoc />
    public WindowsBluetoothLocalCharacteristicFactory(IBluetoothLocalDescriptorFactory localDescriptorFactory)
    {
        _localDescriptorFactory = localDescriptorFactory;
    }

    /// <inheritdoc />
    public IBluetoothLocalCharacteristic Create(IBluetoothLocalService service, IBluetoothLocalCharacteristicFactory.BluetoothLocalCharacteristicSpec spec)
    {
        throw new NotImplementedException("Windows local characteristic factory implementation pending.");
    }
}
