namespace Bluetooth.Avalonia.Platforms.Android.Scanning;

/// <inheritdoc />
public class AndroidBluetoothRemoteCharacteristic : BaseBluetoothRemoteCharacteristic
{
    /// <inheritdoc />
    /// <exception cref="PlatformNotSupportedException"></exception>
    public AndroidBluetoothRemoteCharacteristic(IBluetoothRemoteService parentService,
        Guid id,
        IBluetoothNameProvider? nameProvider = null,
        ILogger<IBluetoothRemoteCharacteristic>? logger = null) : base(parentService, id, nameProvider, logger)
    {
        throw new PlatformNotSupportedException("Android BLE implementation for Avalonia is scaffolded. Port the implementation from Bluetooth.Maui.Platforms.Droid.");
    }

    #region Descriptors Exploration

    /// <inheritdoc />
    /// <exception cref="PlatformNotSupportedException"></exception>
    protected override ValueTask NativeDescriptorsExplorationAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        throw new PlatformNotSupportedException("Android BLE implementation for Avalonia is scaffolded. Port the implementation from Bluetooth.Maui.Platforms.Droid.");
    }

    #endregion

    #region Read

    /// <inheritdoc />
    /// <exception cref="PlatformNotSupportedException"></exception>
    protected override bool NativeCanRead()
    {
        throw new PlatformNotSupportedException("Android BLE implementation for Avalonia is scaffolded. Port the implementation from Bluetooth.Maui.Platforms.Droid.");
    }

    /// <inheritdoc />
    /// <exception cref="PlatformNotSupportedException"></exception>
    protected override ValueTask NativeReadValueAsync()
    {
        throw new PlatformNotSupportedException("Android BLE implementation for Avalonia is scaffolded. Port the implementation from Bluetooth.Maui.Platforms.Droid.");
    }

    #endregion

    #region Write

    /// <inheritdoc />
    /// <exception cref="PlatformNotSupportedException"></exception>
    protected override bool NativeCanWrite()
    {
        throw new PlatformNotSupportedException("Android BLE implementation for Avalonia is scaffolded. Port the implementation from Bluetooth.Maui.Platforms.Droid.");
    }

    /// <inheritdoc />
    /// <exception cref="PlatformNotSupportedException"></exception>
    protected override ValueTask NativeWriteValueAsync(ReadOnlyMemory<byte> value)
    {
        throw new PlatformNotSupportedException("Android BLE implementation for Avalonia is scaffolded. Port the implementation from Bluetooth.Maui.Platforms.Droid.");
    }

    #endregion

    #region Listen

    /// <inheritdoc />
    /// <exception cref="PlatformNotSupportedException"></exception>
    protected override bool NativeCanListen()
    {
        throw new PlatformNotSupportedException("Android BLE implementation for Avalonia is scaffolded. Port the implementation from Bluetooth.Maui.Platforms.Droid.");
    }

    /// <inheritdoc />
    /// <exception cref="PlatformNotSupportedException"></exception>
    protected override ValueTask NativeReadIsListeningAsync()
    {
        throw new PlatformNotSupportedException("Android BLE implementation for Avalonia is scaffolded. Port the implementation from Bluetooth.Maui.Platforms.Droid.");
    }

    /// <inheritdoc />
    /// <exception cref="PlatformNotSupportedException"></exception>
    protected override ValueTask NativeWriteIsListeningAsync(bool shouldBeListening)
    {
        throw new PlatformNotSupportedException("Android BLE implementation for Avalonia is scaffolded. Port the implementation from Bluetooth.Maui.Platforms.Droid.");
    }

    #endregion

    #region Reliable Write

    /// <inheritdoc />
    /// <exception cref="PlatformNotSupportedException"></exception>
    protected override ValueTask NativeBeginReliableWriteAsync()
    {
        throw new PlatformNotSupportedException("Android BLE implementation for Avalonia is scaffolded. Port the implementation from Bluetooth.Maui.Platforms.Droid.");
    }

    /// <inheritdoc />
    /// <exception cref="PlatformNotSupportedException"></exception>
    protected override ValueTask NativeExecuteReliableWriteAsync()
    {
        throw new PlatformNotSupportedException("Android BLE implementation for Avalonia is scaffolded. Port the implementation from Bluetooth.Maui.Platforms.Droid.");
    }

    /// <inheritdoc />
    /// <exception cref="PlatformNotSupportedException"></exception>
    protected override ValueTask NativeAbortReliableWriteAsync()
    {
        throw new PlatformNotSupportedException("Android BLE implementation for Avalonia is scaffolded. Port the implementation from Bluetooth.Maui.Platforms.Droid.");
    }

    #endregion

}
