namespace Bluetooth.Maui;

/// <summary>
/// Represents a platform-agnostic Bluetooth Low Energy advertisement that is not supported on the current platform.
/// This implementation is used as a fallback when running on non-native platforms.
/// All methods throw <see cref="PlatformNotSupportedException"/>.
/// </summary>
public class BluetoothAdvertisement : BaseBluetoothAdvertisement
{
    /// <inheritdoc/>
    /// <exception cref="PlatformNotSupportedException">
    /// Always thrown. This functionality is only supported on native platforms (Android, iOS, Windows).
    /// </exception>
    protected override string InitDeviceName()
    {
        throw new PlatformNotSupportedException("This functionality is only supported on Native platforms. You called the shared version.");
    }

    /// <inheritdoc/>
    /// <exception cref="PlatformNotSupportedException">
    /// Always thrown. This functionality is only supported on native platforms (Android, iOS, Windows).
    /// </exception>
    protected override IEnumerable<Guid> InitServicesGuids()
    {
        throw new PlatformNotSupportedException("This functionality is only supported on Native platforms. You called the shared version.");
    }

    /// <inheritdoc/>
    /// <exception cref="PlatformNotSupportedException">
    /// Always thrown. This functionality is only supported on native platforms (Android, iOS, Windows).
    /// </exception>
    protected override bool InitIsConnectable()
    {
        throw new PlatformNotSupportedException("This functionality is only supported on Native platforms. You called the shared version.");
    }

    /// <inheritdoc/>
    /// <exception cref="PlatformNotSupportedException">
    /// Always thrown. This functionality is only supported on native platforms (Android, iOS, Windows).
    /// </exception>
    protected override int InitRawSignalStrengthInDBm()
    {
        throw new PlatformNotSupportedException("This functionality is only supported on Native platforms. You called the shared version.");
    }

    /// <inheritdoc/>
    /// <exception cref="PlatformNotSupportedException">
    /// Always thrown. This functionality is only supported on native platforms (Android, iOS, Windows).
    /// </exception>
    protected override int InitTransmitPowerLevelInDBm()
    {
        throw new PlatformNotSupportedException("This functionality is only supported on Native platforms. You called the shared version.");
    }

    /// <inheritdoc/>
    /// <exception cref="PlatformNotSupportedException">
    /// Always thrown. This functionality is only supported on native platforms (Android, iOS, Windows).
    /// </exception>
    protected override byte[] InitManufacturerData()
    {
        throw new PlatformNotSupportedException("This functionality is only supported on Native platforms. You called the shared version.");
    }

    /// <inheritdoc/>
    /// <exception cref="PlatformNotSupportedException">
    /// Always thrown. This functionality is only supported on native platforms (Android, iOS, Windows).
    /// </exception>
    protected override string InitBluetoothAddress()
    {
        throw new PlatformNotSupportedException("This functionality is only supported on Native platforms. You called the shared version.");
    }
}
