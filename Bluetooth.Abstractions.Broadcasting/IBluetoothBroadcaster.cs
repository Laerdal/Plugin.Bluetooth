namespace Bluetooth.Abstractions.Broadcasting;

/// <summary>
///     Interface for managing Bluetooth broadcasting operations.
///     Enables the device to act as a Bluetooth peripheral/server, advertising services and handling client connections.
/// </summary>
public partial interface IBluetoothBroadcaster : IAsyncDisposable
{
    /// <summary>
    ///     Gets the Bluetooth adapter associated with this broadcaster.
    /// </summary>
    IBluetoothAdapter Adapter { get; }

    #region Permissions

    /// <summary>
    ///     Checks if the application has the necessary broadcaster permissions.
    /// </summary>
    /// <returns>True if broadcaster permissions are granted, otherwise false.</returns>
    /// <remarks>
    ///     This is a read-only check. It does not trigger any permission requests.
    ///     <para>
    ///         <b>Platform-specific behavior:</b>
    ///     </para>
    ///     <list type="bullet">
    ///         <item><b>Android</b>: Checks BLUETOOTH_ADVERTISE (API 31+) or location permissions (older)</item>
    ///         <item><b>iOS/macOS</b>: Checks Bluetooth Always + Peripheral permissions</item>
    ///         <item><b>Windows</b>: Checks adapter availability and peripheral role support</item>
    ///     </list>
    /// </remarks>
    ValueTask<bool> HasBroadcasterPermissionsAsync();

    /// <summary>
    ///     Requests the necessary broadcaster permissions from the user.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token to cancel the permission request operation.</param>
    /// <returns>Task that completes when permissions are requested.</returns>
    /// <exception cref="BluetoothPermissionException">
    ///     Thrown when permission request fails or is denied.
    ///     Check InnerException for platform-specific details.
    /// </exception>
    /// <remarks>
    ///     <para>
    ///         <b>Platform-specific behavior:</b>
    ///     </para>
    ///     <list type="bullet">
    ///         <item><b>Android</b>: Shows system permission dialog; can be requested multiple times</item>
    ///         <item><b>iOS/macOS</b>: Requests both Bluetooth Always and Peripheral permissions</item>
    ///         <item><b>Windows</b>: Checks adapter state and requests radio access if needed</item>
    ///     </list>
    /// </remarks>
    ValueTask RequestBroadcasterPermissionsAsync(CancellationToken cancellationToken = default);

    #endregion
}