namespace Bluetooth.Maui;

/// <summary>
/// Represents a platform-agnostic Bluetooth Low Energy device that is not supported on the current platform.
/// This implementation is used as a fallback when running on non-native platforms.
/// All platform-specific methods throw <see cref="PlatformNotSupportedException"/>.
/// </summary>
public class BluetoothDevice : BaseBluetoothDevice
{

    /// <summary>
    /// Initializes a new instance of the platform-agnostic <see cref="BluetoothDevice"/> class.
    /// </summary>
    /// <param name="scanner">The Bluetooth scanner associated with this device.</param>
    /// <param name="id">The unique identifier of the device.</param>
    /// <param name="manufacturer">The manufacturer of the device.</param>
    public BluetoothDevice(IBluetoothScanner scanner, string id, Manufacturer manufacturer) : base(scanner, id, manufacturer)
    {
    }

    /// <summary>
    /// Initializes a new instance of the platform-agnostic <see cref="BluetoothDevice"/> class from an advertisement.
    /// </summary>
    /// <param name="scanner">The Bluetooth scanner associated with this device.</param>
    /// <param name="advertisement">The advertisement information used to initialize the device.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="advertisement"/> is <c>null</c>.</exception>
    public BluetoothDevice(IBluetoothScanner scanner, IBluetoothAdvertisement advertisement) : base(scanner, advertisement)
    {
    }

    #region BaseBluetoothDevice

    /// <inheritdoc/>
    /// <exception cref="PlatformNotSupportedException">
    /// Always thrown. This functionality is only supported on native platforms (Android, iOS, Windows).
    /// </exception>
    protected override ValueTask NativeServicesExplorationAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        throw new PlatformNotSupportedException("This functionality is only supported on Native platforms. You called the shared version.");
    }

    /// <inheritdoc/>
    /// <exception cref="PlatformNotSupportedException">
    /// Always thrown. This functionality is only supported on native platforms (Android, iOS, Windows).
    /// </exception>
    protected override void NativeRefreshIsConnected()
    {
        throw new PlatformNotSupportedException("This functionality is only supported on Native platforms. You called the shared version.");
    }

    /// <inheritdoc/>
    /// <exception cref="PlatformNotSupportedException">
    /// Always thrown. This functionality is only supported on native platforms (Android, iOS, Windows).
    /// </exception>
    protected override ValueTask NativeConnectAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        throw new PlatformNotSupportedException("This functionality is only supported on Native platforms. You called the shared version.");
    }

    /// <inheritdoc/>
    /// <exception cref="PlatformNotSupportedException">
    /// Always thrown. This functionality is only supported on native platforms (Android, iOS, Windows).
    /// </exception>
    protected override ValueTask NativeDisconnectAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        throw new PlatformNotSupportedException("This functionality is only supported on Native platforms. You called the shared version.");
    }

    /// <inheritdoc/>
    /// <exception cref="PlatformNotSupportedException">
    /// Always thrown. This functionality is only supported on native platforms (Android, iOS, Windows).
    /// </exception>
    protected override void NativeReadSignalStrength()
    {
        throw new PlatformNotSupportedException("This functionality is only supported on Native platforms. You called the shared version.");
    }
    #endregion
}
