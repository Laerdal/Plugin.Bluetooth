namespace Bluetooth.Core.Broadcasting;

public abstract partial class BaseBluetoothLocalService
{
    #region LoggerMessage Definitions (EventId 600-699)

    // Characteristic list operations (600-619)
    [LoggerMessage(EventId = 600, Level = LogLevel.Information,
        Message = "Service {ServiceId} adding characteristic {CharacteristicId}")]
    partial void LogAddingCharacteristic(Guid serviceId, Guid characteristicId);

    [LoggerMessage(EventId = 601, Level = LogLevel.Information,
        Message = "Service {ServiceId} characteristic {CharacteristicId} added successfully")]
    partial void LogCharacteristicAdded(Guid serviceId, Guid characteristicId);

    [LoggerMessage(EventId = 602, Level = LogLevel.Error,
        Message = "Service {ServiceId} failed to add characteristic {CharacteristicId}")]
    partial void LogCharacteristicAddFailed(Guid serviceId, Guid characteristicId, Exception exception);

    [LoggerMessage(EventId = 603, Level = LogLevel.Information,
        Message = "Service {ServiceId} removing characteristic {CharacteristicId}")]
    partial void LogRemovingCharacteristic(Guid serviceId, Guid characteristicId);

    [LoggerMessage(EventId = 604, Level = LogLevel.Information,
        Message = "Service {ServiceId} characteristic {CharacteristicId} removed successfully")]
    partial void LogCharacteristicRemoved(Guid serviceId, Guid characteristicId);

    [LoggerMessage(EventId = 605, Level = LogLevel.Error,
        Message = "Service {ServiceId} failed to remove characteristic {CharacteristicId}")]
    partial void LogCharacteristicRemoveFailed(Guid serviceId, Guid characteristicId, Exception exception);

    [LoggerMessage(EventId = 606, Level = LogLevel.Error,
        Message = "Service {ServiceId} characteristic {CharacteristicId} not found")]
    partial void LogCharacteristicNotFound(Guid serviceId, Guid characteristicId);

    [LoggerMessage(EventId = 607, Level = LogLevel.Error,
        Message = "Service {ServiceId} characteristic {CharacteristicId} already exists")]
    partial void LogCharacteristicAlreadyExists(Guid serviceId, Guid characteristicId);

    [LoggerMessage(EventId = 608, Level = LogLevel.Information,
        Message = "Service {ServiceId} clearing all characteristics")]
    partial void LogClearingCharacteristics(Guid serviceId);

    [LoggerMessage(EventId = 609, Level = LogLevel.Information,
        Message = "Service {ServiceId} characteristics cleared - {CharacteristicCount} characteristics removed")]
    partial void LogCharacteristicsCleared(Guid serviceId, int characteristicCount);

    #endregion
}