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

    private const int LinkOpen = 0;
    private const int DisconnectionReported = 1;
    private const int ProxyRetired = 2;

    /// <summary>
    ///     This proxy's connection-status state, changed only atomically. It ensures each disconnection of this
    ///     proxy's link is reported once - by a native Disconnected callback or by the device's manual completion
    ///     after an explicit teardown (see <see cref="Retire" />), never both. The state belongs to this proxy
    ///     instance, so callbacks from a retired proxy can never affect a later connection's proxy.
    ///     <list type="bullet">
    ///         <item><see cref="LinkOpen" />: a Disconnected callback may claim and report the next disconnection.</item>
    ///         <item><see cref="DisconnectionReported" />: a Disconnected callback reported it; a Connected callback (e.g. after MaintainConnection reconnects) re-opens.</item>
    ///         <item><see cref="ProxyRetired" />: terminal; no further connection-status changes are forwarded.</item>
    ///     </list>
    /// </summary>
    private int _linkState = LinkOpen;

    /// <summary>
    ///     Gets the native Windows Bluetooth LE device instance.
    /// </summary>
    public BluetoothLEDevice BluetoothLeDevice { get; }

    /// <summary>
    ///     Retires this proxy ahead of an explicit teardown: from now on it forwards no connection-status changes,
    ///     including callbacks already in flight. Call before <see cref="Dispose" />.
    /// </summary>
    /// <returns>
    ///     <c>true</c> if the caller must report the disconnection itself; <c>false</c> if a native Disconnected
    ///     callback from this proxy already claimed it (that callback reports it).
    /// </returns>
    public bool Retire()
    {
        return Interlocked.Exchange(ref _linkState, ProxyRetired) == LinkOpen;
    }

    /// <summary>
    ///     Releases all resources used by the <see cref="BluetoothLeDeviceProxy" /> instance.
    /// </summary>
    public void Dispose()
    {
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
        try
        {
            BluetoothLeDeviceProxyDelegate.OnGattServicesChanged();
        }
        catch (Exception e)
        {
            BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, e);
        }
    }

    /// <summary>
    ///     Handles connection status change events and forwards them to the delegate.
    /// </summary>
    /// <param name="sender">The Bluetooth LE device whose connection status changed.</param>
    /// <param name="args">The event arguments (not used).</param>
    private void OnConnectionStatusChanged(BluetoothLEDevice sender, object args)
    {
        var connectionStatus = sender.ConnectionStatus;
        if (connectionStatus == BluetoothConnectionStatus.Disconnected)
        {
            // Claim this disconnection; drop the callback if it was already reported or the proxy is retired.
            if (Interlocked.CompareExchange(ref _linkState, DisconnectionReported, LinkOpen) != LinkOpen)
            {
                return;
            }
        }
        else if (Interlocked.CompareExchange(ref _linkState, LinkOpen, DisconnectionReported) == ProxyRetired)
        {
            // Connected (re-)opens the link for the next disconnection, unless the proxy is retired.
            return;
        }

        try
        {
            BluetoothLeDeviceProxyDelegate.OnConnectionStatusChanged(connectionStatus);
        }
        catch (Exception e)
        {
            BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, e);
        }
    }

    /// <summary>
    ///     Handles device name change events and forwards them to the delegate.
    /// </summary>
    /// <param name="sender">The Bluetooth LE device whose name changed.</param>
    /// <param name="args">The event arguments (not used).</param>
    private void OnNameChanged(BluetoothLEDevice sender, object args)
    {
        try
        {
            BluetoothLeDeviceProxyDelegate.OnNameChanged(sender.Name);
        }
        catch (Exception e)
        {
            BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, e);
        }
    }

    /// <summary>
    ///     Handles custom pairing spec events and forwards them to the delegate.
    /// </summary>
    /// <param name="sender">The custom pairing object that requested pairing.</param>
    /// <param name="args">The pairing spec event arguments.</param>
    private void OnCustomPairingRequested(DeviceInformationCustomPairing sender, DevicePairingRequestedEventArgs args)
    {
        try
        {
            BluetoothLeDeviceProxyDelegate.OnCustomPairingRequested(args);
        }
        catch (Exception e)
        {
            BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, e);
        }
    }

    /// <summary>
    ///     Handles device access change events and forwards them to the delegate.
    /// </summary>
    /// <param name="sender">The device access information that changed.</param>
    /// <param name="args">The access change event arguments.</param>
    private void OnAccessChanged(DeviceAccessInformation sender, DeviceAccessChangedEventArgs args)
    {
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
