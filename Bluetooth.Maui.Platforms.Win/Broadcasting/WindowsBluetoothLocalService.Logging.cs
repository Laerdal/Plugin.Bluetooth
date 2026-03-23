namespace Bluetooth.Maui.Platforms.Win.Broadcasting;

public partial class WindowsBluetoothLocalService
{
    [LoggerMessage(EventId = 6240, Level = LogLevel.Information,
        Message = "Created Windows local characteristic {CharacteristicId} in service {ServiceId}")]
    partial void LogNativeCharacteristicCreated(Guid characteristicId, Guid serviceId);

    [LoggerMessage(EventId = 6241, Level = LogLevel.Information,
        Message = "Stopped advertising for Windows local service {ServiceId} during dispose")]
    partial void LogServiceDisposeCleanup(Guid serviceId);
}
