namespace Bluetooth.Avalonia.Platforms.Apple.Scanning;

/// <inheritdoc />
public class AppleBluetoothRemoteCharacteristic : BaseBluetoothRemoteCharacteristic
{
    /// <inheritdoc />
    /// <exception cref="PlatformNotSupportedException"></exception>
    public AppleBluetoothRemoteCharacteristic(IBluetoothRemoteService parentService,
        Guid id,
        IBluetoothNameProvider? nameProvider = null,
        ILogger<IBluetoothRemoteCharacteristic>? logger = null) : base(parentService, id, nameProvider, logger)
    {
        throw new PlatformNotSupportedException("Apple BLE implementation for Avalonia is scaffolded. Port the implementation from Bluetooth.Maui.Platforms.Apple.");
    }

    #region Descriptors Exploration

    /// <inheritdoc />
    /// <exception cref="PlatformNotSupportedException"></exception>
    protected override ValueTask NativeDescriptorsExplorationAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        throw new PlatformNotSupportedException("Apple BLE implementation for Avalonia is scaffolded. Port the implementation from Bluetooth.Maui.Platforms.Apple.");
    }

    #endregion

    #region Read

    /// <inheritdoc />
    /// <exception cref="PlatformNotSupportedException"></exception>
    protected override bool NativeCanRead()
    {
        throw new PlatformNotSupportedException("Apple BLE implementation for Avalonia is scaffolded. Port the implementation from Bluetooth.Maui.Platforms.Apple.");
    }

    /// <inheritdoc />
    /// <exception cref="PlatformNotSupportedException"></exception>
    protected override ValueTask NativeReadValueAsync()
    {
        throw new PlatformNotSupportedException("Apple BLE implementation for Avalonia is scaffolded. Port the implementation from Bluetooth.Maui.Platforms.Apple.");
    }

    #endregion

    #region Write

    /// <inheritdoc />
    /// <exception cref="PlatformNotSupportedException"></exception>
    protected override bool NativeCanWrite()
    {
        throw new PlatformNotSupportedException("Apple BLE implementation for Avalonia is scaffolded. Port the implementation from Bluetooth.Maui.Platforms.Apple.");
    }

    /// <inheritdoc />
    /// <exception cref="PlatformNotSupportedException"></exception>
    protected override ValueTask NativeWriteValueAsync(ReadOnlyMemory<byte> value)
    {
        throw new PlatformNotSupportedException("Apple BLE implementation for Avalonia is scaffolded. Port the implementation from Bluetooth.Maui.Platforms.Apple.");
    }

    #endregion

    #region Listen

    /// <inheritdoc />
    /// <exception cref="PlatformNotSupportedException"></exception>
    protected override bool NativeCanListen()
    {
        throw new PlatformNotSupportedException("Apple BLE implementation for Avalonia is scaffolded. Port the implementation from Bluetooth.Maui.Platforms.Apple.");
    }

    /// <inheritdoc />
    /// <exception cref="PlatformNotSupportedException"></exception>
    protected override ValueTask NativeReadIsListeningAsync()
    {
        throw new PlatformNotSupportedException("Apple BLE implementation for Avalonia is scaffolded. Port the implementation from Bluetooth.Maui.Platforms.Apple.");
    }

    /// <inheritdoc />
    /// <exception cref="PlatformNotSupportedException"></exception>
    protected override ValueTask NativeWriteIsListeningAsync(bool shouldBeListening)
    {
        throw new PlatformNotSupportedException("Apple BLE implementation for Avalonia is scaffolded. Port the implementation from Bluetooth.Maui.Platforms.Apple.");
    }

    #endregion

    #region Reliable Write

    /// <inheritdoc />
    /// <exception cref="PlatformNotSupportedException"></exception>
    protected override ValueTask NativeBeginReliableWriteAsync()
    {
        throw new PlatformNotSupportedException("Apple BLE implementation for Avalonia is scaffolded. Port the implementation from Bluetooth.Maui.Platforms.Apple.");
    }

    /// <inheritdoc />
    /// <exception cref="PlatformNotSupportedException"></exception>
    protected override ValueTask NativeExecuteReliableWriteAsync()
    {
        throw new PlatformNotSupportedException("Apple BLE implementation for Avalonia is scaffolded. Port the implementation from Bluetooth.Maui.Platforms.Apple.");
    }

    /// <inheritdoc />
    /// <exception cref="PlatformNotSupportedException"></exception>
    protected override ValueTask NativeAbortReliableWriteAsync()
    {
        throw new PlatformNotSupportedException("Apple BLE implementation for Avalonia is scaffolded. Port the implementation from Bluetooth.Maui.Platforms.Apple.");
    }

    #endregion

}
