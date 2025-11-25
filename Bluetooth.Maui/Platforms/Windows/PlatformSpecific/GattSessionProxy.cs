using Bluetooth.Maui.PlatformSpecific.Exceptions;

namespace Bluetooth.Maui.PlatformSpecific;


/// <summary>
/// Proxy class for Windows GATT session that provides event handling and lifecycle management
/// for GATT session operations.
/// </summary>
public sealed partial class GattSessionProxy : IDisposable
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GattSessionProxy"/> class.
    /// </summary>
    /// <param name="gattSession">The native Windows GATT session instance.</param>
    /// <param name="gattSessionProxyDelegate">The delegate for handling GATT session events.</param>
    private GattSessionProxy(GattSession gattSession, IGattSessionProxyDelegate gattSessionProxyDelegate)
    {
        GattSessionProxyDelegate = gattSessionProxyDelegate;
        GattSession = gattSession;
        GattSession.SessionStatusChanged += OnSessionStatusChanged;
        GattSession.MaxPduSizeChanged += OnMaxPduSizeChanged;
    }

    /// <summary>
    /// Gets the delegate responsible for handling GATT session events.
    /// </summary>
    private IGattSessionProxyDelegate GattSessionProxyDelegate { get; }

    /// <summary>
    /// Gets the native Windows GATT session instance.
    /// </summary>
    public GattSession GattSession { get; }

    /// <summary>
    /// Releases all resources used by the <see cref="GattSessionProxy"/> instance.
    /// </summary>
    public void Dispose()
    {
        GattSession.SessionStatusChanged -= OnSessionStatusChanged;
        GattSession.MaxPduSizeChanged -= OnMaxPduSizeChanged;
        GattSession.Dispose();
    }

    /// <summary>
    /// Handles maximum PDU size change events and forwards them to the delegate.
    /// </summary>
    /// <param name="sender">The GATT session that changed PDU size.</param>
    /// <param name="args">The event arguments (not used).</param>
    private void OnMaxPduSizeChanged(GattSession sender, object args)
    {
        try
        {
            GattSessionProxyDelegate.OnMaxPduSizeChanged();
        }
        catch (Exception e)
        {
            BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, e);
        }
    }

    /// <summary>
    /// Handles session status change events and forwards them to the delegate.
    /// </summary>
    /// <param name="sender">The GATT session that changed status.</param>
    /// <param name="args">The session status change event arguments.</param>
    private void OnSessionStatusChanged(GattSession sender, GattSessionStatusChangedEventArgs args)
    {
        try
        {
            GattSessionProxyDelegate.OnGattSessionStatusChanged(args.Status);
        }
        catch (Exception e)
        {
            BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, e);
        }
    }

    /// <summary>
    /// Creates a new <see cref="GattSessionProxy"/> instance for the specified Bluetooth LE device.
    /// </summary>
    /// <param name="bluetoothLeDevice">The Bluetooth LE device to create a GATT session for.</param>
    /// <param name="gattSessionProxyDelegate">The delegate for handling GATT session events.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the GATT session proxy instance.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="bluetoothLeDevice"/> is null.</exception>
    /// <exception cref="WindowsNativeBluetoothException">Thrown when the GATT session cannot be created.</exception>
    public async static Task<GattSessionProxy> GetInstanceAsync(BluetoothLEDevice bluetoothLeDevice, IGattSessionProxyDelegate gattSessionProxyDelegate, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(bluetoothLeDevice);

        var nativeGattSession = await GattSession.FromDeviceIdAsync(bluetoothLeDevice.BluetoothDeviceId).AsTask(cancellationToken).ConfigureAwait(false);

        if (nativeGattSession == null)
        {
            throw new WindowsNativeBluetoothException($"Failed to get GattSession for device {bluetoothLeDevice.DeviceId}");
        }

        return new GattSessionProxy(nativeGattSession, gattSessionProxyDelegate);
    }
}

