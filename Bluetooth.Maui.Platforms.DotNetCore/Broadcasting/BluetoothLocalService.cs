namespace Bluetooth.Maui.Platforms.DotNetCore.Broadcasting;

/// <inheritdoc />
public class BluetoothLocalService : BaseBluetoothLocalService
{
    /// <inheritdoc />
    /// <exception cref="PlatformNotSupportedException"></exception>
    public BluetoothLocalService(IBluetoothBroadcaster broadcaster,
        IBluetoothLocalServiceFactory.BluetoothLocalServiceSpec request,
        IBluetoothLocalCharacteristicFactory localCharacteristicFactory,
        ILogger<IBluetoothLocalService>? logger = null) : base(broadcaster, request, localCharacteristicFactory, logger)
    {
        throw new PlatformNotSupportedException("This functionality is only supported on Native platforms. You called the shared version.");
    }
}