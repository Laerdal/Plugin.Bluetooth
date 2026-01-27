namespace Bluetooth.Maui.Platforms.Windows.Scanning;

/// <inheritdoc/>
public class BluetoothDevice : BaseBluetoothDevice
{
    /// <inheritdoc/>
    public BluetoothDevice(IBluetoothScanner scanner, IBluetoothDeviceFactory.BluetoothDeviceFactoryRequest request, IBluetoothServiceFactory serviceFactory) : base(scanner, request, serviceFactory)
    {
    }

    /// <inheritdoc/>
    protected override void NativeRefreshIsConnected()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    protected async override ValueTask NativeConnectAsync(IBluetoothDeviceConnectionOptions connectionOptions, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    protected async override ValueTask NativeDisconnectAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    protected async override ValueTask NativeServicesExplorationAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    protected override void NativeReadSignalStrength()
    {
        throw new NotImplementedException();
    }
}
