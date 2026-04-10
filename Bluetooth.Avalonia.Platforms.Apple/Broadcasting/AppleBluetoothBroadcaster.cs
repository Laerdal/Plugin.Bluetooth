namespace Bluetooth.Avalonia.Platforms.Apple.Broadcasting;

/// <inheritdoc />
public class AppleBluetoothBroadcaster : BaseBluetoothBroadcaster
{
    /// <inheritdoc />
    /// <exception cref="PlatformNotSupportedException"></exception>
    public AppleBluetoothBroadcaster(IBluetoothAdapter adapter, ITicker ticker, ILoggerFactory? loggerFactory = null) : base(adapter, ticker, loggerFactory)
    {
        throw new PlatformNotSupportedException("Apple BLE implementation for Avalonia is scaffolded. Port the implementation from Bluetooth.Maui.Platforms.Apple.");
    }

    /// <inheritdoc />
    /// <exception cref="PlatformNotSupportedException"></exception>
    protected override void NativeRefreshIsRunning()
    {
        throw new PlatformNotSupportedException("Apple BLE implementation for Avalonia is scaffolded. Port the implementation from Bluetooth.Maui.Platforms.Apple.");
    }

    /// <inheritdoc />
    /// <exception cref="PlatformNotSupportedException"></exception>
    protected override ValueTask NativeStartAsync(BroadcastingOptions options, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        try
        {
            throw new PlatformNotSupportedException("Apple BLE implementation for Avalonia is scaffolded. Port the implementation from Bluetooth.Maui.Platforms.Apple.");
        }
        catch (Exception exception)
        {
            return ValueTask.FromException(exception);
        }
    }

    /// <inheritdoc />
    /// <exception cref="PlatformNotSupportedException"></exception>
    protected override ValueTask NativeStopAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        try
        {
            throw new PlatformNotSupportedException("Apple BLE implementation for Avalonia is scaffolded. Port the implementation from Bluetooth.Maui.Platforms.Apple.");
        }
        catch (Exception exception)
        {
            return ValueTask.FromException(exception);
        }
    }

    /// <inheritdoc />
    /// <exception cref="PlatformNotSupportedException"></exception>
    protected override ValueTask<IBluetoothLocalService> NativeCreateServiceAsync(Guid id,
        string? name = null,
        bool isPrimary = true,
        TimeSpan? timeout = null,
        CancellationToken cancellationToken = default)
    {
        throw new PlatformNotSupportedException("Apple BLE implementation for Avalonia is scaffolded. Port the implementation from Bluetooth.Maui.Platforms.Apple.");
    }

    /// <inheritdoc />
    /// <exception cref="PlatformNotSupportedException"></exception>
    protected override ValueTask<bool> NativeHasBroadcasterPermissionsAsync()
    {
        throw new PlatformNotSupportedException("Apple BLE implementation for Avalonia is scaffolded. Port the implementation from Bluetooth.Maui.Platforms.Apple.");
    }

    /// <inheritdoc />
    /// <exception cref="PlatformNotSupportedException"></exception>
    protected override ValueTask NativeRequestBroadcasterPermissionsAsync(CancellationToken cancellationToken)
    {
        throw new PlatformNotSupportedException("Apple BLE implementation for Avalonia is scaffolded. Port the implementation from Bluetooth.Maui.Platforms.Apple.");
    }
}
