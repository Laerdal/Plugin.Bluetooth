namespace Bluetooth.Maui.Platforms.Win.Broadcasting;

/// <inheritdoc cref="BaseBluetoothConnectedDevice" />
public class WindowsBluetoothConnectedDevice : BaseBluetoothConnectedDevice
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="WindowsBluetoothConnectedDevice" /> class.
    /// </summary>
    /// <param name="broadcaster">The broadcaster that owns this client device.</param>
    /// <param name="spec">The connected-device specification.</param>
    public WindowsBluetoothConnectedDevice(IBluetoothBroadcaster broadcaster, IBluetoothConnectedDeviceFactory.BluetoothConnectedDeviceSpec spec)
        : base(broadcaster, spec)
    {
    }

    /// <summary>
    ///     Gets the native subscribed client reference when available.
    /// </summary>
    public GattSubscribedClient? NativeClient { get; private set; }

    internal void SetNativeClient(GattSubscribedClient? nativeClient)
    {
        NativeClient = nativeClient;
    }

    /// <inheritdoc />
    protected override ValueTask NativeDisconnectAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        throw new NotSupportedException("Windows does not provide a direct API to force-disconnect a subscribed GATT client.");
    }
}
