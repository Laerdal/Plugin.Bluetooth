namespace Bluetooth.Maui.Platforms.DotNetCore.Scanning;

/// <inheritdoc/>
public class BluetoothRemoteService : Core.Scanning.BaseBluetoothRemoteService
{
    /// <inheritdoc/>
    /// <exception cref="PlatformNotSupportedException"></exception>
    public BluetoothRemoteService(IBluetoothRemoteDevice device, IBluetoothServiceFactory.BluetoothServiceFactoryRequest request, IBluetoothCharacteristicFactory characteristicFactory) : base(device, request, characteristicFactory)
    {
        throw new PlatformNotSupportedException("This functionality is only supported on Native platforms. You called the shared version.");
    }

    /// <inheritdoc/>
    /// <exception cref="PlatformNotSupportedException"></exception>
    protected override ValueTask NativeCharacteristicsExplorationAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        throw new PlatformNotSupportedException("This functionality is only supported on Native platforms. You called the shared version.");
    }
}
