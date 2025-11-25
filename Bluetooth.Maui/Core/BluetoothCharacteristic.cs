namespace Bluetooth.Maui;

/// <summary>
/// Represents a platform-agnostic Bluetooth Low Energy GATT characteristic that is not supported on the current platform.
/// This implementation is used as a fallback when running on non-native platforms.
/// All platform-specific methods throw <see cref="PlatformNotSupportedException"/>.
/// </summary>
public class BluetoothCharacteristic : BaseBluetoothCharacteristic
{

    /// <summary>
    /// Initializes a new instance of the platform-agnostic <see cref="BluetoothCharacteristic"/> class.
    /// </summary>
    /// <param name="service">The Bluetooth service associated with this characteristic.</param>
    /// <param name="id">The unique identifier of the characteristic.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="service"/> is <c>null</c>.</exception>
    /// <exception cref="CharacteristicFoundInWrongServiceException">Thrown when the characteristic is defined for a different service than the one provided (inherited from base).</exception>
    public BluetoothCharacteristic(IBluetoothService service, Guid id) : base(service, id)
    {
    }

    #region BaseBluetoothCharacteristic
    /// <inheritdoc/>
    /// <exception cref="NotImplementedException">This platform-agnostic implementation throws NotImplementedException.</exception>
    protected override bool NativeCanListen()
    {
        throw new PlatformNotSupportedException("This functionality is only supported on Native platforms. You called the shared version.");
    }

    /// <inheritdoc/>
    /// <exception cref="PlatformNotSupportedException">
    /// Always thrown. This functionality is only supported on native platforms (Android, iOS, Windows).
    /// </exception>
    protected override ValueTask NativeReadIsListeningAsync()
    {
        throw new PlatformNotSupportedException("This functionality is only supported on Native platforms. You called the shared version.");
    }

    /// <inheritdoc/>
    /// <exception cref="PlatformNotSupportedException">
    /// Always thrown. This functionality is only supported on native platforms (Android, iOS, Windows).
    /// </exception>
    protected override ValueTask NativeWriteIsListeningAsync(bool shouldBeListening)
    {
        throw new PlatformNotSupportedException("This functionality is only supported on Native platforms. You called the shared version.");
    }

    /// <inheritdoc/>
    /// <exception cref="PlatformNotSupportedException">
    /// Always thrown. This functionality is only supported on native platforms (Android, iOS, Windows).
    /// </exception>
    protected override ValueTask NativeReadValueAsync()
    {
        throw new PlatformNotSupportedException("This functionality is only supported on Native platforms. You called the shared version.");
    }

    /// <inheritdoc/>
    /// <exception cref="PlatformNotSupportedException">
    /// Always thrown. This functionality is only supported on native platforms (Android, iOS, Windows).
    /// </exception>
    protected override bool NativeCanRead()
    {
        throw new PlatformNotSupportedException("This functionality is only supported on Native platforms. You called the shared version.");
    }

    /// <inheritdoc/>
    /// <exception cref="PlatformNotSupportedException">
    /// Always thrown. This functionality is only supported on native platforms (Android, iOS, Windows).
    /// </exception>
    protected override ValueTask NativeWriteValueAsync(ReadOnlyMemory<byte> value)
    {
        throw new PlatformNotSupportedException("This functionality is only supported on Native platforms. You called the shared version.");
    }

    /// <inheritdoc/>
    /// <exception cref="PlatformNotSupportedException">
    /// Always thrown. This functionality is only supported on native platforms (Android, iOS, Windows).
    /// </exception>
    protected override bool NativeCanWrite()
    {
        throw new PlatformNotSupportedException("This functionality is only supported on Native platforms. You called the shared version.");
    }
    #endregion
}
