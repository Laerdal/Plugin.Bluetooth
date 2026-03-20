namespace Bluetooth.Maui.Platforms.Win.Broadcasting.Factories;

/// <inheritdoc />
public class WindowsBluetoothLocalServiceFactory : IBluetoothLocalServiceFactory
{
    private readonly IBluetoothLocalCharacteristicFactory _localCharacteristicFactory;

    /// <inheritdoc />
    public WindowsBluetoothLocalServiceFactory(IBluetoothLocalCharacteristicFactory localCharacteristicFactory)
    {
        _localCharacteristicFactory = localCharacteristicFactory;
    }

    /// <inheritdoc />
    public IBluetoothLocalService Create(IBluetoothBroadcaster broadcaster, IBluetoothLocalServiceFactory.BluetoothLocalServiceSpec spec)
    {
        throw new NotImplementedException("Windows local service factory implementation pending.");
    }
}
