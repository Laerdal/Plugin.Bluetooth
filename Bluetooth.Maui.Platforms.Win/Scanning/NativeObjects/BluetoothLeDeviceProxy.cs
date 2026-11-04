using Bluetooth.Maui.Platforms.Win.Exceptions;

namespace Bluetooth.Maui.Platforms.Win.Scanning.NativeObjects;

/// <summary>
///     Proxy class for Windows Bluetooth LE device that provides event handling, lifecycle management,
///     and GATT service operations.
/// </summary>
public sealed partial class BluetoothLeDeviceProxy : IDisposable
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="BluetoothLeDeviceProxy" /> class.
    /// </summary>
    /// <param name="bluetoothLeDevice">The native Windows Bluetooth LE device instance.</param>
    /// <param name="bluetoothLeDeviceProxyDelegate">The delegate for handling Bluetooth LE device events.</param>
    private BluetoothLeDeviceProxy(BluetoothLEDevice bluetoothLeDevice, IBluetoothLeDeviceProxyDelegate bluetoothLeDeviceProxyDelegate)
    {
        BluetoothLeDeviceProxyDelegate = bluetoothLeDeviceProxyDelegate;
        BluetoothLeDevice = bluetoothLeDevice;
        BluetoothLeDevice.NameChanged += OnNameChanged;
        BluetoothLeDevice.GattServicesChanged += GattServicesChanged;
        BluetoothLeDevice.ConnectionStatusChanged += OnConnectionStatusChanged;
        BluetoothLeDevice.DeviceAccessInformation.AccessChanged += OnAccessChanged;
        BluetoothLeDevice.DeviceInformation.Pairing.Custom.PairingRequested += OnCustomPairingRequested;
    }

    /// <summary>
    ///     Gets the delegate responsible for handling Bluetooth LE device events.
    /// </summary>
    private IBluetoothLeDeviceProxyDelegate BluetoothLeDeviceProxyDelegate { get; }

    /// <summary>
    ///     Guards <see cref="_disposed" /> together with each handler's entire check-and-forward step, so that once
    ///     <see cref="Dispose" /> returns no event is being forwarded and none will be. A plain flag check is not
    ///     enough: WinRT raises events on thread-pool threads, so a handler could pass the check, pause, and only
    ///     forward a stale event after the device has disconnected (or even reconnected with a new proxy).
    /// </summary>
    /// <remarks>
    ///     Forwarding runs device code while holding this lock (including task continuations that resume inline),
    ///     so <see cref="Dispose" /> waits for an in-flight forward to finish. Code running inside a forward must
    ///     therefore not block synchronously on the thread that is disposing this proxy. The lock is re-entrant,
    ///     so disposing from within a forward on the same thread is fine.
    /// </remarks>
    private readonly Lock _callbackGate = new();

    /// <summary>
    ///     Set by <see cref="Dispose" /> under <see cref="_callbackGate" />; event handlers stop forwarding from then on.
    /// </summary>
    private bool _disposed;

    /// <summary>
    ///     Gets the native Windows Bluetooth LE device instance.
    /// </summary>
    public BluetoothLEDevice BluetoothLeDevice { get; }

    /// <summary>
    ///     Releases all resources used by the <see cref="BluetoothLeDeviceProxy" /> instance.
    /// </summary>
    public void Dispose()
    {
        // Set before unsubscribing: a handler invocation already in flight can still run after the unsubscribe
        // below. Taking the gate waits for any forward in progress and rejects every later one, so no stale event
        // reaches the delegate (e.g. a late Disconnected after DisconnectAsync finished, which would be reported
        // as an unexpected disconnection or hit the next connection attempt).
        lock (_callbackGate)
        {
            _disposed = true;
        }

        BluetoothLeDevice.NameChanged -= OnNameChanged;
        BluetoothLeDevice.GattServicesChanged -= GattServicesChanged;
        BluetoothLeDevice.ConnectionStatusChanged -= OnConnectionStatusChanged;
        BluetoothLeDevice.DeviceAccessInformation.AccessChanged -= OnAccessChanged;
        BluetoothLeDevice.DeviceInformation.Pairing.Custom.PairingRequested -= OnCustomPairingRequested;
        BluetoothLeDevice.Dispose();
    }

    /// <summary>
    ///     Creates a new <see cref="BluetoothLeDeviceProxy" /> instance for the specified Bluetooth address.
    /// </summary>
    /// <param name="bluetoothAddress">The Bluetooth address of the device to connect to.</param>
    /// <param name="bluetoothLeDeviceProxyDelegate">The delegate for handling Bluetooth LE device events.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the device proxy instance.</returns>
    /// <exception cref="WindowsNativeBluetoothException">Thrown when the Bluetooth LE device cannot be created.</exception>
    public async static Task<BluetoothLeDeviceProxy> GetInstanceAsync(ulong bluetoothAddress, IBluetoothLeDeviceProxyDelegate bluetoothLeDeviceProxyDelegate, CancellationToken cancellationToken = default)
    {
        var nativeBleDevice = await BluetoothLEDevice.FromBluetoothAddressAsync(bluetoothAddress).AsTask(cancellationToken).ConfigureAwait(false);

        if (nativeBleDevice == null)
        {
            throw new WindowsNativeBluetoothException($"Failed to get BluetoothLEDevice for address {bluetoothAddress}");
        }

        return new BluetoothLeDeviceProxy(nativeBleDevice, bluetoothLeDeviceProxyDelegate);
    }

    /// <summary>
    ///     Reads GATT services from the device with retry logic for improved reliability.
    /// </summary>
    /// <param name="bluetoothCacheMode">The cache mode to use when reading services.</param>
    /// <param name="maxNumberOfAttempts">The maximum number of retry attempts (default: 3).</param>
    /// <param name="delayBetweenAttemptsInMs">The delay between retry attempts in milliseconds (default: 100).</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the list of GATT services.</returns>
    /// <exception cref="WindowsNativeBluetoothException">Thrown when GATT communication fails.</exception>
    /// <exception cref="AggregateException">Thrown when all retry attempts fail.</exception>
    public async Task<List<GattDeviceService>> ReadGattServicesAsync(BluetoothCacheMode bluetoothCacheMode, uint maxNumberOfAttempts = 3, int delayBetweenAttemptsInMs = 100, CancellationToken cancellationToken = default)
    {
        var attempts = 0;
        var exceptions = new List<Exception>();
        while (attempts < maxNumberOfAttempts)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                cancellationToken.ThrowIfCancellationRequested();
            }

            attempts++;
            try
            {
                var result = await BluetoothLeDevice.GetGattServicesAsync(bluetoothCacheMode).AsTask(cancellationToken).ConfigureAwait(false);

                if (result.Status != GattCommunicationStatus.Success)
                {
                    throw new WindowsNativeGattCommunicationStatusException(result.Status);
                }

                return result.Services.ToList();
            }
            catch (Exception e)
            {
                await Task.Delay(delayBetweenAttemptsInMs, cancellationToken).ConfigureAwait(false);
                exceptions.Add(e);
            }
        }

        throw new AggregateException(exceptions);
    }

    /// <summary>
    ///     Handles GATT services change events and forwards them to the delegate.
    /// </summary>
    /// <param name="sender">The Bluetooth LE device whose GATT services changed.</param>
    /// <param name="args">The event arguments (not used).</param>
    private void GattServicesChanged(BluetoothLEDevice sender, object args)
    {
        lock (_callbackGate)
        {
            if (_disposed)
            {
                return;
            }

            try
            {
                BluetoothLeDeviceProxyDelegate.OnGattServicesChanged();
            }
            catch (Exception e)
            {
                BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, e);
            }
        }
    }

    /// <summary>
    ///     Handles connection status change events and forwards them to the delegate.
    /// </summary>
    /// <param name="sender">The Bluetooth LE device whose connection status changed.</param>
    /// <param name="args">The event arguments (not used).</param>
    private void OnConnectionStatusChanged(BluetoothLEDevice sender, object args)
    {
        lock (_callbackGate)
        {
            if (_disposed)
            {
                return;
            }

            try
            {
                BluetoothLeDeviceProxyDelegate.OnConnectionStatusChanged(sender.ConnectionStatus);
            }
            catch (Exception e)
            {
                BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, e);
            }
        }
    }

    /// <summary>
    ///     Handles device name change events and forwards them to the delegate.
    /// </summary>
    /// <param name="sender">The Bluetooth LE device whose name changed.</param>
    /// <param name="args">The event arguments (not used).</param>
    private void OnNameChanged(BluetoothLEDevice sender, object args)
    {
        lock (_callbackGate)
        {
            if (_disposed)
            {
                return;
            }

            try
            {
                BluetoothLeDeviceProxyDelegate.OnNameChanged(sender.Name);
            }
            catch (Exception e)
            {
                BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, e);
            }
        }
    }

    /// <summary>
    ///     Handles custom pairing spec events and forwards them to the delegate.
    /// </summary>
    /// <param name="sender">The custom pairing object that requested pairing.</param>
    /// <param name="args">The pairing spec event arguments.</param>
    private void OnCustomPairingRequested(DeviceInformationCustomPairing sender, DevicePairingRequestedEventArgs args)
    {
        lock (_callbackGate)
        {
            if (_disposed)
            {
                return;
            }

            try
            {
                BluetoothLeDeviceProxyDelegate.OnCustomPairingRequested(args);
            }
            catch (Exception e)
            {
                BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, e);
            }
        }
    }

    /// <summary>
    ///     Handles device access change events and forwards them to the delegate.
    /// </summary>
    /// <param name="sender">The device access information that changed.</param>
    /// <param name="args">The access change event arguments.</param>
    private void OnAccessChanged(DeviceAccessInformation sender, DeviceAccessChangedEventArgs args)
    {
        lock (_callbackGate)
        {
            if (_disposed)
            {
                return;
            }

            try
            {
                BluetoothLeDeviceProxyDelegate.OnAccessChanged(args.Id, args.Status);
            }
            catch (Exception e)
            {
                BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, e);
            }
        }
    }
}
