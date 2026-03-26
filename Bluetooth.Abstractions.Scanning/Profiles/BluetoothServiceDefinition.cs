namespace Bluetooth.Abstractions.Scanning.Profiles;

/// <summary>
///     Immutable definition of a known Bluetooth GATT service, including its UUID and human-readable name.
/// </summary>
/// <param name="ServiceId">The UUID of the GATT service.</param>
/// <param name="ServiceName">The human-readable name of the service.</param>
public record BluetoothServiceDefinition(Guid ServiceId, string ServiceName);
