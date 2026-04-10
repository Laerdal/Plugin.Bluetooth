namespace Bluetooth.Avalonia.Platforms.Linux.Scanning;

/// <inheritdoc />
public class LinuxBluetoothRemoteCharacteristic : BaseBluetoothRemoteCharacteristic
{
    /// <inheritdoc />
    /// <exception cref="PlatformNotSupportedException"></exception>
    public LinuxBluetoothRemoteCharacteristic(IBluetoothRemoteService parentService,
        Guid id,
        IBluetoothNameProvider? nameProvider = null,
        ILogger<IBluetoothRemoteCharacteristic>? logger = null) : base(parentService, id, nameProvider, logger)
    {
        throw new PlatformNotSupportedException("This functionality is only supported on Native platforms. Linux / Desktop BLE is not yet implemented. BlueZ integration is planned.");
    }

    #region Descriptors Exploration

    /// <inheritdoc />
    /// <exception cref="PlatformNotSupportedException"></exception>
    protected override ValueTask NativeDescriptorsExplorationAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        throw new PlatformNotSupportedException("This functionality is only supported on Native platforms. Linux / Desktop BLE is not yet implemented. BlueZ integration is planned.");
    }

    #endregion

    #region Read

    /// <inheritdoc />
    /// <exception cref="PlatformNotSupportedException"></exception>
    protected override bool NativeCanRead()
    {
        throw new PlatformNotSupportedException("This functionality is only supported on Native platforms. Linux / Desktop BLE is not yet implemented. BlueZ integration is planned.");
    }

    /// <inheritdoc />
    /// <exception cref="PlatformNotSupportedException"></exception>
    protected override ValueTask NativeReadValueAsync()
    {
        throw new PlatformNotSupportedException("This functionality is only supported on Native platforms. Linux / Desktop BLE is not yet implemented. BlueZ integration is planned.");
    }

    #endregion

    #region Write

    /// <inheritdoc />
    /// <exception cref="PlatformNotSupportedException"></exception>
    protected override bool NativeCanWrite()
    {
        throw new PlatformNotSupportedException("This functionality is only supported on Native platforms. Linux / Desktop BLE is not yet implemented. BlueZ integration is planned.");
    }

    /// <inheritdoc />
    /// <exception cref="PlatformNotSupportedException"></exception>
    protected override ValueTask NativeWriteValueAsync(ReadOnlyMemory<byte> value)
    {
        throw new PlatformNotSupportedException("This functionality is only supported on Native platforms. Linux / Desktop BLE is not yet implemented. BlueZ integration is planned.");
    }

    #endregion

    #region Listen

    /// <inheritdoc />
    /// <exception cref="PlatformNotSupportedException"></exception>
    protected override bool NativeCanListen()
    {
        throw new PlatformNotSupportedException("This functionality is only supported on Native platforms. Linux / Desktop BLE is not yet implemented. BlueZ integration is planned.");
    }

    /// <inheritdoc />
    /// <exception cref="PlatformNotSupportedException"></exception>
    protected override ValueTask NativeReadIsListeningAsync()
    {
        throw new PlatformNotSupportedException("This functionality is only supported on Native platforms. Linux / Desktop BLE is not yet implemented. BlueZ integration is planned.");
    }

    /// <inheritdoc />
    /// <exception cref="PlatformNotSupportedException"></exception>
    protected override ValueTask NativeWriteIsListeningAsync(bool shouldBeListening)
    {
        throw new PlatformNotSupportedException("This functionality is only supported on Native platforms. Linux / Desktop BLE is not yet implemented. BlueZ integration is planned.");
    }

    #endregion

    #region Reliable Write

    /// <inheritdoc />
    /// <exception cref="PlatformNotSupportedException"></exception>
    protected override ValueTask NativeBeginReliableWriteAsync()
    {
        throw new PlatformNotSupportedException("This functionality is only supported on Native platforms. Linux / Desktop BLE is not yet implemented. BlueZ integration is planned.");
    }

    /// <inheritdoc />
    /// <exception cref="PlatformNotSupportedException"></exception>
    protected override ValueTask NativeExecuteReliableWriteAsync()
    {
        throw new PlatformNotSupportedException("This functionality is only supported on Native platforms. Linux / Desktop BLE is not yet implemented. BlueZ integration is planned.");
    }

    /// <inheritdoc />
    /// <exception cref="PlatformNotSupportedException"></exception>
    protected override ValueTask NativeAbortReliableWriteAsync()
    {
        throw new PlatformNotSupportedException("This functionality is only supported on Native platforms. Linux / Desktop BLE is not yet implemented. BlueZ integration is planned.");
    }

    #endregion

}
