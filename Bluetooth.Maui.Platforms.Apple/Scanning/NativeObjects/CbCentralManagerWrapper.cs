using Microsoft.Extensions.Options;

namespace Bluetooth.Maui.Platforms.Apple.Scanning.NativeObjects;

/// <summary>
/// Proxy class for CoreBluetooth central manager delegate callbacks.
/// https://developer.apple.com/documentation/corebluetooth/cbcentralmanagerdelegate
/// </summary>
public partial class CbCentralManagerWrapper : CBCentralManagerDelegate
{
    /// <summary>
    /// The underlying CBCentralManager instance.
    /// </summary>
    public CBCentralManager CbCentralManager { get; }

    /// <summary>
    /// The delegate proxy for handling central manager events.
    /// </summary>
    private readonly CbCentralManagerWrapper.ICbCentralManagerProxyDelegate _scanner;

    /// <summary>
    /// Initializes a new instance of the CbCentralManagerWrapper class.
    /// </summary>
    /// <param name="scanner"></param>
    /// <param name="queue">The dispatch queue for central manager events.</param>
    /// <param name="options">The initialization options for the central manager.</param>
    public CbCentralManagerWrapper(CbCentralManagerWrapper.ICbCentralManagerProxyDelegate scanner, DispatchQueue queue, CBCentralInitOptions options)
    {
        _scanner = scanner;
        CbCentralManager = new CBCentralManager(this, queue, options)
        {
            Delegate = this
        };
    }

    /// <summary>
    /// Releases the unmanaged resources used by the CbCentralManagerWrapper and optionally releases the managed resources.
    /// </summary>
    /// <param name="disposing">True to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            CbCentralManager.Dispose();
        }

        base.Dispose(disposing);
    }

    #region CBCentralManager

    /// <inheritdoc cref="CBCentralManagerDelegate.DiscoveredPeripheral" />

    // ReSharper disable once InconsistentNaming
    public override void DiscoveredPeripheral(CBCentralManager central, CBPeripheral peripheral, NSDictionary advertisementData, NSNumber RSSI)
    {
        try
        {
            _scanner.DiscoveredPeripheral(peripheral, advertisementData, RSSI);
        }
        catch (Exception e)
        {
            BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, e);
        }
    }

    /// <inheritdoc cref="CBCentralManagerDelegate.UpdatedState" />
    public override void UpdatedState(CBCentralManager central)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(central);
            _scanner.UpdatedState(central.State);
        }
        catch (Exception e)
        {
            BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, e);
        }
    }

    /// <inheritdoc cref="CBCentralManagerDelegate.WillRestoreState" />
    public override void WillRestoreState(CBCentralManager central, NSDictionary dict)
    {
        try
        {
            _scanner.WillRestoreState(dict);
        }
        catch (Exception e)
        {
            BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, e);
        }
    }

    #endregion

    #region CBPeripheral

    /// <inheritdoc cref="CBCentralManagerDelegate.FailedToConnectPeripheral" />
    public override void FailedToConnectPeripheral(CBCentralManager central, CBPeripheral peripheral, NSError? error)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(peripheral);

            // GET DEVICE
            var sharedDevice = _scanner.GetDevice(peripheral);

            // ACTION
            sharedDevice.FailedToConnectPeripheral(error);
        }
        catch (Exception e)
        {
            BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, e);
        }
    }

    /// <inheritdoc cref="CBCentralManagerDelegate.DidDisconnectPeripheral" />
    public override void DidDisconnectPeripheral(CBCentralManager central,
        CBPeripheral peripheral,
        double timestamp,
        bool isReconnecting,
        NSError? error)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(peripheral);

            // GET DEVICE
            var sharedDevice = _scanner.GetDevice(peripheral);

            // ACTION
            sharedDevice.DidDisconnectPeripheral(timestamp, isReconnecting, error);
        }
        catch (Exception e)
        {
            BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, e);
        }
    }

    /// <inheritdoc cref="CBCentralManagerDelegate.DisconnectedPeripheral" />
    public override void DisconnectedPeripheral(CBCentralManager central, CBPeripheral peripheral, NSError? error)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(peripheral);

            // GET DEVICE
            var sharedDevice = _scanner.GetDevice(peripheral);

            // ACTION
            sharedDevice.DisconnectedPeripheral(error);
        }
        catch (Exception e)
        {
            BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, e);
        }
    }

    /// <inheritdoc cref="CBCentralManagerDelegate.ConnectedPeripheral" />
    public override void ConnectedPeripheral(CBCentralManager central, CBPeripheral peripheral)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(peripheral);

            // GET DEVICE
            var sharedDevice = _scanner.GetDevice(peripheral);

            // ACTION
            sharedDevice.ConnectedPeripheral();
        }
        catch (Exception e)
        {
            BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, e);
        }
    }

    /// <inheritdoc cref="CBCentralManagerDelegate.ConnectionEventDidOccur" />
    public override void ConnectionEventDidOccur(CBCentralManager central, CBConnectionEvent connectionEvent, CBPeripheral peripheral)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(peripheral);

            // GET DEVICE
            var sharedDevice = _scanner.GetDevice(peripheral);

            // ACTION
            sharedDevice.ConnectionEventDidOccur(connectionEvent);
        }
        catch (Exception e)
        {
            BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, e);
        }
    }

    /// <inheritdoc cref="CBCentralManagerDelegate.DidUpdateAncsAuthorization" />
    public override void DidUpdateAncsAuthorization(CBCentralManager central, CBPeripheral peripheral)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(peripheral);

            // GET DEVICE
            var sharedDevice = _scanner.GetDevice(peripheral);

            // ACTION
            sharedDevice.DidUpdateAncsAuthorization();
        }
        catch (Exception e)
        {
            BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, e);
        }
    }

    #endregion

}
