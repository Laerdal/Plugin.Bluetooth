
namespace Bluetooth.Core.BaseClasses;

/// <inheritdoc cref="IBluetoothBroadcaster" />
/// <summary>
/// Base class for Bluetooth Low Energy broadcaster implementations that advertise the device's presence.
/// </summary>
/// <remarks>
/// Broadcasters allow a device to act as a BLE peripheral, advertising its presence and services to nearby devices.
/// This is the opposite role of a scanner, which listens for advertisements.
/// </remarks>
public abstract class BaseBluetoothBroadcaster : BaseBluetoothActivity, IBluetoothBroadcaster
{
}
