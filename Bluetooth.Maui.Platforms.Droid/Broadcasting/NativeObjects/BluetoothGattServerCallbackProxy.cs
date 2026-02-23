using Application = Android.App.Application;
using Exception = System.Exception;

namespace Bluetooth.Maui.Platforms.Droid.Broadcasting.NativeObjects;

/// <summary>
///     Android Bluetooth GATT server callback proxy that handles GATT server events.
///     Implements <see cref="BluetoothGattServerCallback" /> to redirect events to the delegate instance.
/// </summary>
/// <remarks>
///     This class wraps the Android BluetoothGattServerCallback and provides exception handling
///     for all callback methods. See Android documentation:
///     https://developer.android.com/reference/android/bluetooth/BluetoothGattServerCallback
/// </remarks>
public partial class BluetoothGattServerCallbackProxy : BluetoothGattServerCallback
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="BluetoothGattServerCallbackProxy" /> class.
    /// </summary>
    /// <param name="bluetoothGattServerCallbackProxyDelegate">The delegate instance that will receive callback events.</param>
    /// <exception cref="InvalidOperationException">Thrown when the GATT server cannot be opened.</exception>
    public BluetoothGattServerCallbackProxy(IBluetoothGattServerCallbackProxyDelegate bluetoothGattServerCallbackProxyDelegate, BluetoothManager bluetoothManager)
    {
        ArgumentNullException.ThrowIfNull(bluetoothGattServerCallbackProxyDelegate);
        ArgumentNullException.ThrowIfNull(bluetoothManager);

        BluetoothGattServerCallbackProxyDelegate = bluetoothGattServerCallbackProxyDelegate;
        BluetoothGattServer = bluetoothManager.OpenGattServer(Application.Context, this) ?? throw new InvalidOperationException("Failed to open GATT server");
    }

    /// <summary>
    ///     Gets the delegate instance that receives callback events.
    /// </summary>
    private IBluetoothGattServerCallbackProxyDelegate BluetoothGattServerCallbackProxyDelegate { get; }

    /// <summary>
    ///     Gets the Bluetooth GATT server instance for communication with remote devices.
    /// </summary>
    public BluetoothGattServer BluetoothGattServer { get; }

    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            BluetoothGattServer.Close();
            BluetoothGattServer.Dispose();
        }

        base.Dispose(disposing);
    }

    /// <inheritdoc />
    public override void OnMtuChanged(BluetoothDevice? device, int mtu)
    {
        try
        {
            // GET SERVICE
            var sharedDevice = BluetoothGattServerCallbackProxyDelegate.GetDevice(device);

            // ACTION
            sharedDevice.OnMtuChanged(mtu);
        }
        catch (Exception e)
        {
            BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, e);
        }
    }

    /// <inheritdoc />
    public override void OnExecuteWrite(BluetoothDevice? device, int requestId, bool execute)
    {
        try
        {
            // GET SERVICE
            var sharedDevice = BluetoothGattServerCallbackProxyDelegate.GetDevice(device);

            // ACTION
            sharedDevice.OnExecuteWrite(requestId, execute);
        }
        catch (Exception e)
        {
            BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, e);
        }
    }

    /// <inheritdoc />
    public override void OnNotificationSent(BluetoothDevice? device, GattStatus status)
    {
        try
        {
            // GET DEVICE
            var sharedDevice = BluetoothGattServerCallbackProxyDelegate.GetDevice(device);

            // ACTION
            sharedDevice.OnNotificationSent(status);
        }
        catch (Exception e)
        {
            BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, e);
        }
    }

    /// <summary>
    ///     Callback invoked when the PHY (Physical Layer) settings for a connection have been read.
    /// </summary>
    /// <param name="device">
    ///     The <see cref="BluetoothDevice" /> associated with the connection.
    /// </param>
    /// <param name="txPhy">
    ///     The transmitter PHY mode. Possible values include:
    ///     <c>ScanSettingsPhy.Le1M</c>, <c>ScanSettingsPhy.Le2M</c>, or <c>ScanSettingsPhy.LeCoded</c>.
    /// </param>
    /// <param name="rxPhy">
    ///     The receiver PHY mode. Possible values are the same as <paramref name="txPhy" />.
    /// </param>
    /// <param name="status">
    ///     The <see cref="GattStatus" /> indicating the result of the PHY read operation.
    /// </param>
    /// <remarks>
    ///     This method is called when the PHY settings of a GATT connection are read. Applications
    ///     can use this callback to determine the PHY mode being used for data transmission and reception.
    ///     For more information, see:
    ///     <list type="bullet">
    ///         <item>
    ///             <description>
    ///                 <a href="https://developer.android.com/reference/android/bluetooth/BluetoothGattServerCallback#onPhyRead(android.bluetooth.BluetoothDevice,%20int,%20int,%20int)">
    ///                     BluetoothGattServerCallback.OnPhyRead (Android API)
    ///                 </a>
    ///             </description>
    ///         </item>
    ///     </list>
    /// </remarks>
    public override void OnPhyRead(BluetoothDevice? device, ScanSettingsPhy txPhy, ScanSettingsPhy rxPhy, GattStatus status)
    {
        try
        {
            // GET DEVICE
            var sharedDevice = BluetoothGattServerCallbackProxyDelegate.GetDevice(device);

            // ACTION
            sharedDevice.OnPhyRead(status, txPhy, rxPhy);
        }
        catch (Exception e)
        {
            BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, e);
        }
    }

    /// <summary>
    ///     Callback invoked when the PHY (Physical Layer) settings for a connection have been updated.
    /// </summary>
    /// <param name="device">
    ///     The <see cref="BluetoothDevice" /> associated with the connection.
    /// </param>
    /// <param name="txPhy">
    ///     The updated transmitter PHY mode. Possible values include:
    ///     <c>ScanSettingsPhy.Le1M</c>, <c>ScanSettingsPhy.Le2M</c>, or <c>ScanSettingsPhy.LeCoded</c>.
    /// </param>
    /// <param name="rxPhy">
    ///     The updated receiver PHY mode. Possible values are the same as <paramref name="txPhy" />.
    /// </param>
    /// <param name="status">
    ///     The <see cref="GattStatus" /> indicating the result of the PHY update operation.
    /// </param>
    /// <remarks>
    ///     This method is called when the PHY settings of a GATT connection are updated. Applications
    ///     can use this callback to adjust behavior based on the new PHY settings.
    ///     For more information, see:
    ///     <list type="bullet">
    ///         <item>
    ///             <description>
    ///                 <a href="https://developer.android.com/reference/android/bluetooth/BluetoothGattServerCallback#onPhyUpdate(android.bluetooth.BluetoothDevice,%20int,%20int,%20int)">
    ///                     BluetoothGattServerCallback.OnPhyUpdate (Android API)
    ///                 </a>
    ///             </description>
    ///         </item>
    ///     </list>
    /// </remarks>
    public override void OnPhyUpdate(BluetoothDevice? device, ScanSettingsPhy txPhy, ScanSettingsPhy rxPhy, GattStatus status)
    {
        try
        {
            // GET DEVICE
            var sharedDevice = BluetoothGattServerCallbackProxyDelegate.GetDevice(device);

            // ACTION
            sharedDevice.OnPhyUpdate(status, txPhy, rxPhy);
        }
        catch (Exception e)
        {
            BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, e);
        }
    }

    /// <summary>
    ///     Callback indicating the result of a service addition to the GATT server.
    /// </summary>
    /// <param name="status">
    ///     The <see cref="GattStatus" /> indicating the result of the service addition operation. Possible values include:
    ///     <list type="bullet">
    ///         <item>
    ///             <description>
    ///                 <c>GattStatus.Success</c> - The service was successfully added to the GATT server.
    ///             </description>
    ///         </item>
    ///         <item>
    ///             <description>
    ///                 Other status codes as defined in the GATT protocol or Android Bluetooth API.
    ///             </description>
    ///         </item>
    ///     </list>
    /// </param>
    /// <param name="service">
    ///     The <see cref="BluetoothGattService" /> that was added to the GATT server.
    /// </param>
    /// <remarks>
    ///     This method is called when a service has been added to the GATT server. Applications can use this
    ///     callback to handle the result of the service addition operation and take appropriate action.
    ///     For more information, see:
    ///     <list type="bullet">
    ///         <item>
    ///             <description>
    ///                 <a href="https://developer.android.com/reference/android/bluetooth/BluetoothGattServerCallback#onServiceAdded(int,%20android.bluetooth.BluetoothGattService)">
    ///                     BluetoothGattServerCallback.OnServiceAdded (Android API)
    ///                 </a>
    ///             </description>
    ///         </item>
    ///     </list>
    /// </remarks>
    public override void OnServiceAdded(GattStatus status, BluetoothGattService? service)
    {
        try
        {
            // GET SERVICE
            var sharedService = BluetoothGattServerCallbackProxyDelegate.GetService(service);

            // ACTION
            sharedService.OnServiceAdded(status);
        }
        catch (Exception e)
        {
            BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, e);
        }
    }

    /// <inheritdoc />
    public override void OnConnectionStateChange(BluetoothDevice? device, ProfileState status, ProfileState newState)
    {
        try
        {
            // GET DEVICE
            var sharedDevice = BluetoothGattServerCallbackProxyDelegate.GetDevice(device);

            // ACTION
            sharedDevice.OnConnectionStateChange(status, newState);
        }
        catch (Exception e)
        {
            BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, e);
        }
    }

    /// <summary>
    ///     Callback indicating a request to read a characteristic from a remote device.
    /// </summary>
    /// <param name="device">
    ///     The <see cref="BluetoothDevice" /> requesting to read the characteristic.
    /// </param>
    /// <param name="requestId">
    ///     An integer identifying the read request.
    /// </param>
    /// <param name="offset">
    ///     The offset within the characteristic value to read from.
    /// </param>
    /// <param name="characteristic">
    ///     The <see cref="BluetoothGattCharacteristic" /> requested for reading.
    /// </param>
    /// <remarks>
    ///     This method is called when a remote device requests to read a characteristic. Applications
    ///     can override this method to handle the read request and respond appropriately.
    ///     For more information, see:
    ///     <list type="bullet">
    ///         <item>
    ///             <description>
    ///                 <a href="https://developer.android.com/reference/android/bluetooth/BluetoothGattServerCallback#onCharacteristicReadRequest(android.bluetooth.BluetoothDevice,%20int,%20int,%20android.bluetooth.BluetoothGattCharacteristic)">
    ///                     BluetoothGattServerCallback.OnCharacteristicReadRequest (Android API)
    ///                 </a>
    ///             </description>
    ///         </item>
    ///     </list>
    /// </remarks>
    public override void OnCharacteristicReadRequest(BluetoothDevice? device, int requestId, int offset, BluetoothGattCharacteristic? characteristic)
    {
        try
        {
            // GET DEVICE
            var sharedDevice = BluetoothGattServerCallbackProxyDelegate.GetDevice(device);

            // GET SERVICE
            var sharedService = BluetoothGattServerCallbackProxyDelegate.GetService(characteristic?.Service);

            // GET CHARACTERISTIC
            var sharedCharacteristic = sharedService.GetCharacteristic(characteristic);

            // ACTION
            sharedCharacteristic.OnCharacteristicReadRequest(sharedDevice, requestId, offset);
        }
        catch (Exception e)
        {
            BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, e);
        }
    }

    /// <summary>
    ///     Callback indicating a request to write to a characteristic from a remote device.
    /// </summary>
    /// <param name="device">
    ///     The <see cref="BluetoothDevice" /> requesting to write the characteristic.
    /// </param>
    /// <param name="requestId">
    ///     An integer identifying the write request.
    /// </param>
    /// <param name="characteristic">
    ///     The <see cref="BluetoothGattCharacteristic" /> requested for writing.
    /// </param>
    /// <param name="preparedWrite">
    ///     A boolean indicating if this is a prepared write operation.
    /// </param>
    /// <param name="responseNeeded">
    ///     A boolean indicating if a response is required for this write request.
    /// </param>
    /// <param name="offset">
    ///     The offset within the characteristic value where the write should begin.
    /// </param>
    /// <param name="value">
    ///     A byte array containing the value to be written to the characteristic.
    /// </param>
    /// <remarks>
    ///     This method is called when a remote device requests to write to a characteristic. Applications
    ///     can override this method to handle the write request, respond appropriately, and process the provided value.
    ///     For more information, see:
    ///     <list type="bullet">
    ///         <item>
    ///             <description>
    ///                 <a
    ///                     href="https://developer.android.com/reference/android/bluetooth/BluetoothGattServerCallback#onCharacteristicWriteRequest(android.bluetooth.BluetoothDevice,%20int,%20android.bluetooth.BluetoothGattCharacteristic,%20boolean,%20boolean,%20int,%20byte[])">
    ///                     BluetoothGattServerCallback.OnCharacteristicWriteRequest (Android API)
    ///                 </a>
    ///             </description>
    ///         </item>
    ///     </list>
    /// </remarks>
    public override void OnCharacteristicWriteRequest(BluetoothDevice? device,
        int requestId,
        BluetoothGattCharacteristic? characteristic,
        bool preparedWrite,
        bool responseNeeded,
        int offset,
        byte[]? value)
    {
        try
        {
            // GET DEVICE
            var sharedDevice = BluetoothGattServerCallbackProxyDelegate.GetDevice(device);

            // GET SERVICE
            var sharedService = BluetoothGattServerCallbackProxyDelegate.GetService(characteristic?.Service);

            // GET CHARACTERISTIC
            var sharedCharacteristic = sharedService.GetCharacteristic(characteristic);

            // ACTION
            sharedCharacteristic.OnCharacteristicWriteRequest(sharedDevice,
                requestId,
                preparedWrite,
                responseNeeded,
                offset,
                value ?? []);
        }
        catch (Exception e)
        {
            BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, e);
        }
    }

    /// <summary>
    ///     Callback indicating a request to read a descriptor from a remote device.
    /// </summary>
    /// <param name="device">
    ///     The <see cref="BluetoothDevice" /> requesting to read the descriptor.
    /// </param>
    /// <param name="requestId">
    ///     An integer identifying the read request.
    /// </param>
    /// <param name="offset">
    ///     The offset within the descriptor value to read from.
    /// </param>
    /// <param name="descriptor">
    ///     The <see cref="BluetoothGattDescriptor" /> requested for reading.
    /// </param>
    /// <remarks>
    ///     This method is called when a remote device requests to read a descriptor. Applications
    ///     can override this method to handle the read request and respond appropriately.
    ///     For more information, see:
    ///     <list type="bullet">
    ///         <item>
    ///             <description>
    ///                 <a href="https://developer.android.com/reference/android/bluetooth/BluetoothGattServerCallback#onDescriptorReadRequest(android.bluetooth.BluetoothDevice,%20int,%20int,%20android.bluetooth.BluetoothGattDescriptor)">
    ///                     BluetoothGattServerCallback.OnDescriptorReadRequest (Android API)
    ///                 </a>
    ///             </description>
    ///         </item>
    ///     </list>
    /// </remarks>
    public override void OnDescriptorReadRequest(BluetoothDevice? device, int requestId, int offset, BluetoothGattDescriptor? descriptor)
    {
        try
        {
            // GET DEVICE
            var sharedDevice = BluetoothGattServerCallbackProxyDelegate.GetDevice(device);

            // GET SERVICE
            var sharedService = BluetoothGattServerCallbackProxyDelegate.GetService(descriptor?.Characteristic?.Service);

            // GET CHARACTERISTIC
            var sharedCharacteristic = sharedService.GetCharacteristic(descriptor?.Characteristic);

            // GET DESCRIPTOR
            var sharedDescriptor = sharedCharacteristic.GetDescriptor(descriptor);

            // ACTION
            sharedDescriptor.OnDescriptorReadRequest(sharedDevice, requestId, offset, descriptor);
        }
        catch (Exception e)
        {
            BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, e);
        }
    }

    /// <summary>
    ///     Callback indicating a request to write to a descriptor from a remote device.
    /// </summary>
    /// <param name="device">
    ///     The <see cref="BluetoothDevice" /> requesting to write the descriptor.
    /// </param>
    /// <param name="requestId">
    ///     An integer identifying the write request.
    /// </param>
    /// <param name="descriptor">
    ///     The <see cref="BluetoothGattDescriptor" /> requested for writing.
    /// </param>
    /// <param name="preparedWrite">
    ///     A boolean indicating if this is a prepared write operation.
    /// </param>
    /// <param name="responseNeeded">
    ///     A boolean indicating if a response is required for this write request.
    /// </param>
    /// <param name="offset">
    ///     The offset within the descriptor value where the write should begin.
    /// </param>
    /// <param name="value">
    ///     A byte array containing the value to be written to the descriptor.
    /// </param>
    /// <remarks>
    ///     This method is called when a remote device requests to write to a descriptor. Applications
    ///     can override this method to handle the write request, respond appropriately, and process the provided value.
    ///     For more information, see:
    ///     <list type="bullet">
    ///         <item>
    ///             <description>
    ///                 <a
    ///                     href="https://developer.android.com/reference/android/bluetooth/BluetoothGattServerCallback#onDescriptorWriteRequest(android.bluetooth.BluetoothDevice,%20int,%20android.bluetooth.BluetoothGattDescriptor,%20boolean,%20boolean,%20int,%20byte[])">
    ///                     BluetoothGattServerCallback.OnDescriptorWriteRequest (Android API)
    ///                 </a>
    ///             </description>
    ///         </item>
    ///     </list>
    /// </remarks>
    public override void OnDescriptorWriteRequest(BluetoothDevice? device,
        int requestId,
        BluetoothGattDescriptor? descriptor,
        bool preparedWrite,
        bool responseNeeded,
        int offset,
        byte[]? value)
    {
        try
        {
            // GET DEVICE
            var sharedDevice = BluetoothGattServerCallbackProxyDelegate.GetDevice(device);

            // GET SERVICE
            var sharedService = BluetoothGattServerCallbackProxyDelegate.GetService(descriptor?.Characteristic?.Service);

            // GET CHARACTERISTIC
            var sharedCharacteristic = sharedService.GetCharacteristic(descriptor?.Characteristic);

            // GET DESCRIPTOR
            var sharedDescriptor = sharedCharacteristic.GetDescriptor(descriptor);

            // ACTION
            sharedDescriptor.OnDescriptorWriteRequest(sharedDevice,
                requestId,
                descriptor,
                preparedWrite,
                responseNeeded,
                offset,
                value ?? []);
        }
        catch (Exception e)
        {
            BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, e);
        }
    }
}
