using System.ComponentModel;

namespace Bluetooth.Abstractions;

/// <summary>
/// Interface for managing Bluetooth permissions.
/// </summary>
public interface IBluetoothPermissionManager
{
    /// <summary>
    /// Checks if the application has the necessary Bluetooth permissions.
    /// </summary>
    /// <returns>True if permissions are granted, otherwise false.</returns>
    ValueTask<bool> HasBluetoothPermissionsAsync();

    /// <summary>
    /// Checks if the application has the necessary Bluetooth scanner permissions.
    /// </summary>
    /// <returns>True if scanner permissions are granted, otherwise false.</returns>
    ValueTask<bool> HasScannerPermissionsAsync();

    /// <summary>
    /// Checks if the application has the necessary Bluetooth broadcaster permissions.
    /// </summary>
    /// <returns>True if broadcaster permissions are granted, otherwise false.</returns>
    ValueTask<bool> HasBroadcasterPermissionsAsync();

    /// <summary>
    /// Requests the necessary Bluetooth permissions from the user.
    /// </summary>
    /// <returns>True if permissions are granted, otherwise false.</returns>
    ValueTask<bool> RequestBluetoothPermissionsAsync();

    /// <summary>
    /// Requests the necessary Bluetooth scanner permissions from the user.
    /// </summary>
    /// <returns>True if scanner permissions are granted, otherwise false.</returns>
    ValueTask<bool> RequestScannerPermissionsAsync();

    /// <summary>
    /// Requests the necessary Bluetooth broadcaster permissions from the user.
    /// </summary>
    /// <returns>True if broadcaster permissions are granted, otherwise false.</returns>
    ValueTask<bool> RequestBroadcasterPermissionsAsync();
}
