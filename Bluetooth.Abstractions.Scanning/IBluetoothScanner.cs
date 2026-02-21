namespace Bluetooth.Abstractions.Scanning;

/// <summary>
///     Interface for managing and scanning Bluetooth devices.
/// </summary>
public partial interface IBluetoothScanner : IAsyncDisposable
{
    #region Advertisement

    /// <summary>
    ///     Event triggered when a Bluetooth advertisement is received.
    /// </summary>
    event EventHandler<AdvertisementReceivedEventArgs> AdvertisementReceived;

    #endregion

    #region Permissions

    /// <summary>
    ///     Checks if the application has the necessary scanner permissions.
    /// </summary>
    /// <returns>True if scanner permissions are granted, otherwise false.</returns>
    /// <remarks>
    ///     This is a read-only check. It does not trigger any permission requests.
    ///     <para>
    ///         <b>Platform-specific behavior:</b>
    ///     </para>
    ///     <list type="bullet">
    ///         <item><b>Android</b>: Checks BLUETOOTH_SCAN (API 31+) or location permissions (older)</item>
    ///         <item><b>iOS/macOS</b>: Checks Bluetooth Always permission</item>
    ///         <item><b>Windows</b>: Checks adapter availability and radio state</item>
    ///     </list>
    /// </remarks>
    ValueTask<bool> HasScannerPermissionsAsync();

    /// <summary>
    ///     Requests the necessary scanner permissions from the user.
    /// </summary>
    /// <param name="requireBackgroundLocation">Android-only: whether to request background location permission (API 29-30).</param>
    /// <param name="cancellationToken">Cancellation token to cancel the permission request operation.</param>
    /// <returns>Task that completes when permissions are requested.</returns>
    /// <exception cref="BluetoothPermissionException">
    ///     Thrown when permission request fails or is denied.
    ///     Check InnerException for platform-specific details (COMException, SecurityException, etc.).
    /// </exception>
    /// <remarks>
    ///     <para>
    ///         <b>Platform-specific behavior:</b>
    ///     </para>
    ///     <list type="bullet">
    ///         <item><b>Android</b>: Shows system permission dialog; can be requested multiple times</item>
    ///         <item><b>iOS/macOS</b>: Shows permission dialog on first call; subsequent denials require Settings</item>
    ///         <item><b>Windows</b>: Checks adapter state and requests radio access if needed</item>
    ///     </list>
    /// </remarks>
    ValueTask RequestScannerPermissionsAsync(bool requireBackgroundLocation = false, CancellationToken cancellationToken = default);

    #endregion
}