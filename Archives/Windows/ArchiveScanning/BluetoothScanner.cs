using Bluetooth.Abstractions.Scanning.AccessService;
using Bluetooth.Abstractions.Scanning.Factories;

using Microsoft.Extensions.Logging;

namespace Bluetooth.Maui.Platforms.Windows.Scanning;

/// <inheritdoc/>
public class BluetoothScanner : BaseBluetoothScanner
{
    /// <inheritdoc/>
    public BluetoothScanner(IBluetoothAdapter adapter,
        IBluetoothPermissionManager permissionManager,
        IBluetoothDeviceFactory deviceFactory,
        IBluetoothCharacteristicAccessServicesRepository knownServicesAndCharacteristicsRepository,
        ILogger? logger = null) : base(adapter,
                                       permissionManager,
                                       deviceFactory,
                                       knownServicesAndCharacteristicsRepository,
                                       logger)
    {
    }

    /// <inheritdoc/>
    protected override void NativeRefreshIsRunning()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    protected override ValueTask NativeStartAsync(IBluetoothScannerStartScanningOptions scanningOptions, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    protected override ValueTask NativeStopAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
