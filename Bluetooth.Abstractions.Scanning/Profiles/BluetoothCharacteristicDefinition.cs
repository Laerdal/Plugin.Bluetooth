namespace Bluetooth.Abstractions.Scanning.Profiles;

/// <summary>
///     Immutable definition of a known Bluetooth GATT characteristic, scoped to its owning service.
/// </summary>
/// <param name="ServiceId">The UUID of the owning GATT service.</param>
/// <param name="CharacteristicId">The UUID of the GATT characteristic.</param>
/// <param name="CharacteristicName">The human-readable name of the characteristic.</param>
public record BluetoothCharacteristicDefinition(Guid ServiceId, Guid CharacteristicId, string CharacteristicName);
