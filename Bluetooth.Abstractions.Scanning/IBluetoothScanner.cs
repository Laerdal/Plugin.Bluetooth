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

    /// <summary>
    ///     Gets or sets the advertisement filter used to determine which Bluetooth advertisements should be processed.
    /// </summary>
    /// <remarks>
    ///     When <c>null</c> (default), all advertisements are accepted.
    ///     When set, only advertisements where the filter returns <c>true</c> are processed.
    /// </remarks>
    Func<IBluetoothAdvertisement, bool>? AdvertisementFilter { get; set; }

    /// <summary>
    ///     Waits for a raw advertisement from the device with the specified Bluetooth address to be received.
    /// </summary>
    /// <param name="bluetoothAddress">The Bluetooth address to wait for.</param>
    /// <param name="timeout">The timeout for this operation.</param>
    /// <param name="cancellationToken">A cancellation token to cancel this operation.</param>
    /// <returns>The matching <see cref="IBluetoothAdvertisement" /> when it is received.</returns>
    ValueTask<IBluetoothAdvertisement> WaitForAdvertisementAsync(string bluetoothAddress, TimeSpan? timeout = null, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Waits for the first raw advertisement that matches the specified filter to be received.
    /// </summary>
    /// <param name="filter">A function to filter advertisements. Should return true for a matching advertisement. Defaults to null for the first advertisement received.</param>
    /// <param name="timeout">The timeout for this operation.</param>
    /// <param name="cancellationToken">A cancellation token to cancel this operation.</param>
    /// <returns>The <see cref="IBluetoothAdvertisement" /> that matches the filter when it is received.</returns>
    /// <remarks>
    ///     Unlike <see cref="IBluetoothScanner.WaitForDeviceToAppearAsync(string, TimeSpan?, CancellationToken)" />,
    ///     this matches directly on the raw advertisement as it comes in, before it is folded into the device
    ///     registry - useful when the identity being waited for isn't known to have a registry entry yet
    ///     (e.g. a device rebooting into a different advertised identity), or when a device-level filter would
    ///     race a device's own first-sight registration.
    /// </remarks>
    ValueTask<IBluetoothAdvertisement> WaitForAdvertisementAsync(Func<IBluetoothAdvertisement, bool>? filter = null, TimeSpan? timeout = null, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Gets or sets an optional function that wraps a newly created device before it is added to the device list.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Called once per new device, after the platform has created the raw <see cref="IBluetoothRemoteDevice"/>.
    ///         Return a richer subtype (e.g. a product-specific device class) or <see langword="null"/> to keep
    ///         the original device unchanged.
    ///     </para>
    ///     <para>
    ///         This follows the same pattern as <see cref="AdvertisementFilter"/>: set it once on the scanner
    ///         instance, and the scanner calls it automatically for every new device.
    ///     </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// scanner.DeviceWrapper = (device, advertisement) =>
    ///     advertisement.Manufacturer == Manufacturer.Laerdal_Medical_AS
    ///         ? new LaerdalDevice(device, advertisement)
    ///         : null;
    /// </code>
    /// </example>
    Func<IBluetoothRemoteDevice, IBluetoothAdvertisement, IBluetoothRemoteDevice?>? DeviceWrapper { get; set; }

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
