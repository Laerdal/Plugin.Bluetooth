namespace Bluetooth.Maui.Platforms.Apple.Permissions;

/// <summary>
/// iOS implementation of the Bluetooth permission manager.
/// </summary>
public class BluetoothPermissionManager : IBluetoothPermissionManager
{
    private readonly ILogger? _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="BluetoothPermissionManager"/> class.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    public BluetoothPermissionManager(ILogger? logger = null)
    {
        _logger = logger;
    }

    // High-performance logging using LoggerMessage delegates
    private readonly static Action<ILogger, Exception> _logErrorCheckingBluetoothPermissions =
        LoggerMessage.Define(
            LogLevel.Error,
            new EventId(1, nameof(HasBluetoothPermissionsAsync)),
            "Error checking Bluetooth permissions");

    private readonly static Action<ILogger, Exception> _logErrorCheckingBroadcasterPermissions =
        LoggerMessage.Define(
            LogLevel.Error,
            new EventId(2, nameof(HasBroadcasterPermissionsAsync)),
            "Error checking Broadcaster permissions");

    private readonly static Action<ILogger, Exception> _logErrorRequestingBluetoothPermissions =
        LoggerMessage.Define(
            LogLevel.Error,
            new EventId(3, nameof(RequestBluetoothPermissionsAsync)),
            "Error requesting Bluetooth permissions");

    private readonly static Action<ILogger, Exception> _logErrorRequestingBroadcasterPermissions =
        LoggerMessage.Define(
            LogLevel.Error,
            new EventId(4, nameof(RequestBroadcasterPermissionsAsync)),
            "Error requesting Broadcaster permissions");

    /// <inheritdoc/>
    public async ValueTask<bool> HasBluetoothPermissionsAsync()
    {
        try
        {
            if (OperatingSystem.IsIOSVersionAtLeast(13) || OperatingSystem.IsMacCatalystVersionAtLeast(10, 15))
            {
                var status = await Microsoft.Maui.ApplicationModel.Permissions.CheckStatusAsync<IosPermissionForBluetoothAlways>().ConfigureAwait(false);
                return status == Microsoft.Maui.ApplicationModel.PermissionStatus.Granted;
            }
            // On older iOS versions, Bluetooth permissions are automatically granted
            return true;
        }
        catch (Exception ex)
        {
            if (_logger is not null)
            {
                _logErrorCheckingBluetoothPermissions(_logger, ex);
            }
            return false;
        }
    }

    /// <inheritdoc/>
    public async ValueTask<bool> HasScannerPermissionsAsync()
    {
        // On iOS, scanner and broadcaster use the same Bluetooth permission
        return await HasBluetoothPermissionsAsync().ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async ValueTask<bool> HasBroadcasterPermissionsAsync()
    {
        try
        {
            if (OperatingSystem.IsIOSVersionAtLeast(13) || OperatingSystem.IsMacCatalystVersionAtLeast(10, 15))
            {
                // Check both Bluetooth Always and Peripheral permissions
                var bluetoothStatus = await Microsoft.Maui.ApplicationModel.Permissions.CheckStatusAsync<IosPermissionForBluetoothAlways>().ConfigureAwait(false);
                var peripheralStatus = await Microsoft.Maui.ApplicationModel.Permissions.CheckStatusAsync<IosPermissionForBluetoothPeripheral>().ConfigureAwait(false);
                return bluetoothStatus == Microsoft.Maui.ApplicationModel.PermissionStatus.Granted &&
                       peripheralStatus == Microsoft.Maui.ApplicationModel.PermissionStatus.Granted;
            }
            // On older iOS versions, Bluetooth permissions are automatically granted
            return true;
        }
        catch (Exception ex)
        {
            if (_logger is not null)
            {
                _logErrorCheckingBroadcasterPermissions(_logger, ex);
            }
            return false;
        }
    }

    /// <inheritdoc/>
    public async ValueTask<bool> RequestBluetoothPermissionsAsync()
    {
        try
        {
            if (OperatingSystem.IsIOSVersionAtLeast(13) || OperatingSystem.IsMacCatalystVersionAtLeast(10, 15))
            {
                var status = await Microsoft.Maui.ApplicationModel.Permissions.RequestAsync<IosPermissionForBluetoothAlways>().ConfigureAwait(false);
                return status == Microsoft.Maui.ApplicationModel.PermissionStatus.Granted;
            }
            // On older iOS versions, Bluetooth permissions are automatically granted
            return true;
        }
        catch (Exception ex)
        {
            if (_logger is not null)
            {
                _logErrorRequestingBluetoothPermissions(_logger, ex);
            }
            return false;
        }
    }

    /// <inheritdoc/>
    public async ValueTask<bool> RequestScannerPermissionsAsync()
    {
        // On iOS, scanner and broadcaster use the same Bluetooth permission
        return await RequestBluetoothPermissionsAsync().ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async ValueTask<bool> RequestBroadcasterPermissionsAsync()
    {
        try
        {
            if (OperatingSystem.IsIOSVersionAtLeast(13) || OperatingSystem.IsMacCatalystVersionAtLeast(10, 15))
            {
                // Request both Bluetooth Always and Peripheral permissions
                var bluetoothStatus = await Microsoft.Maui.ApplicationModel.Permissions.RequestAsync<IosPermissionForBluetoothAlways>().ConfigureAwait(false);
                var peripheralStatus = await Microsoft.Maui.ApplicationModel.Permissions.RequestAsync<IosPermissionForBluetoothPeripheral>().ConfigureAwait(false);
                return bluetoothStatus == Microsoft.Maui.ApplicationModel.PermissionStatus.Granted &&
                       peripheralStatus == Microsoft.Maui.ApplicationModel.PermissionStatus.Granted;
            }
            // On older iOS versions, Bluetooth permissions are automatically granted
            return true;
        }
        catch (Exception ex)
        {
            if (_logger is not null)
            {
                _logErrorRequestingBroadcasterPermissions(_logger, ex);
            }
            return false;
        }
    }
}
