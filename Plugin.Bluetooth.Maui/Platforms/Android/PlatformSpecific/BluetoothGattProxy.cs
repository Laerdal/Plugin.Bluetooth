using Plugin.Bluetooth.Maui.PlatformSpecific.Exceptions;
using Plugin.Bluetooth.Maui.PlatformSpecific.NativeOptions;

using Exception = System.Exception;

namespace Plugin.Bluetooth.Maui.PlatformSpecific;


/// <summary>
/// Android Bluetooth GATT callback proxy that handles GATT events.
/// Implements <see cref="BluetoothGattCallback"/> to redirect events to the device instance.
/// </summary>
/// <remarks>
/// This class wraps the Android BluetoothGattCallback and provides exception handling
/// for all callback methods. See Android documentation:
/// https://developer.android.com/reference/android/bluetooth/BluetoothGattCallback
/// </remarks>
public partial class BluetoothGattProxy : BluetoothGattCallback
{
    /// <summary>
    /// Gets the Bluetooth GATT instance for communication with the remote device.
    /// </summary>
    public BluetoothGatt BluetoothGatt { get; }

    /// <summary>
    /// Gets the device instance that will receive the callback events.
    /// </summary>
    public IDevice Device { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="BluetoothGattProxy"/> class.
    /// </summary>
    /// <param name="device">The device instance that will receive callback events.</param>
    /// <param name="connectionOptions">The connection options for the GATT connection.</param>
    /// <param name="nativeDevice">The native Android Bluetooth device.</param>
    /// <exception cref="ArgumentNullException">Thrown when any parameter is null or when GATT connection fails.</exception>
    public BluetoothGattProxy(IDevice device, ConnectionOptions? connectionOptions, Android.Bluetooth.BluetoothDevice nativeDevice)
    {
        Device = device ?? throw new ArgumentNullException(nameof(device));
        ArgumentNullException.ThrowIfNull(connectionOptions);
        ArgumentNullException.ThrowIfNull(nativeDevice);
        BluetoothGatt = nativeDevice.ConnectGatt(connectionOptions, this) ?? throw new InvalidOperationException("Failed to create GATT connection");
    }

    /// <inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            BluetoothGatt.Close();
            BluetoothGatt.Dispose();
        }
        base.Dispose(disposing);
    }

    /// <summary>
    /// Attempts to reconnect to the remote device using the existing GATT connection.
    /// </summary>
    /// <exception cref="DeviceFailedToConnectException">Thrown when the reconnection attempt fails.</exception>
    public void Reconnect()
    {
        var result = BluetoothGatt.Connect();
        if (!result)
        {
            throw new DeviceFailedToConnectException(Device, "BluetoothGatt.Connect() returned false");
        }
    }

    /// <summary>
    /// Attempts to refresh the GATT cache using Android's hidden refresh method.
    /// </summary>
    /// <returns>True if the refresh operation was successful; otherwise, false.</returns>
    public bool TryGattRefresh()
    {
        try
        {
            var nativeBluetoothGattSnapshot = BluetoothGatt;
            var javaReflectionClassForNativeBluetoothGatt = nativeBluetoothGattSnapshot.Class;

            // ReSharper disable once CanReplaceCastWithVariableType
            var method = (Method?) javaReflectionClassForNativeBluetoothGatt.GetMethod(name: "refresh", parameterTypes: null);
            var success = method?.Invoke(obj: nativeBluetoothGattSnapshot, args: null) as Java.Lang.Boolean;

            return success?.BooleanValue() ?? false;
        }
        catch (Exception e)
        {
            BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, e);
            return false;
        }
    }
    /// <param name="gatt">GATT client the characteristic is associated with</param>
    /// <param name="characteristic">Characteristic that has been updated as a result of a remote
    /// notification event.</param>
    /// <summary>Callback triggered as a result of a remote characteristic notification.</summary>
    /// <remarks>
    ///     <para>Callback triggered as a result of a remote characteristic notification.</para>
    ///     <para>This member is deprecated. Use <c>BluetoothGattCallback#onCharacteristicChanged(BluetoothGatt,
    ///     BluetoothGattCharacteristic, byte[])</c> as it is memory safe by providing the
    ///     characteristic value at the time of notification.</para>
    ///     <para>
    ///         <format type="text/html">
    ///             <a href="https://developer.android.com/reference/android/bluetooth/BluetoothGattCallback#onCharacteristicChanged(android.bluetooth.BluetoothGatt,%20android.bluetooth.BluetoothGattCharacteristic)" title="Reference documentation">Java documentation for <code>android.bluetooth.BluetoothGattCallback.onCharacteristicChanged(android.bluetooth.BluetoothGatt, android.bluetooth.BluetoothGattCharacteristic)</code>.</a>
    ///         </format>
    ///     </para>
    ///     <para>
    ///         Portions of this page are modifications based on work created and shared by the
    ///         <format type="text/html"><a href="https://developers.google.com/terms/site-policies" title="Android Open Source Project">Android Open Source Project</a></format>
    ///          and used according to terms described in the
    ///         <format type="text/html"><a href="https://creativecommons.org/licenses/by/2.5/" title="Creative Commons 2.5 Attribution License">Creative Commons 2.5 Attribution License.</a></format></para>
    /// </remarks>
    /// <since version="Added in API level 18" />
    public override void OnCharacteristicChanged(BluetoothGatt? gatt, BluetoothGattCharacteristic? characteristic)
    {
        try
        {
            if (OperatingSystem.IsAndroidVersionAtLeast(33))
            {
                throw new AndroidNativeBluetoothException("This callback OnCharacteristicChanged(BluetoothGatt?, BluetoothGattCharacteristic?) should not be triggered in Android versions above 33.");
            }

            // GET SERVICE
            var sharedService = Device.GetService(characteristic?.Service);

            // GET CHARACTERISTIC
            var sharedCharacteristic = sharedService.GetCharacteristic(characteristic);

            // ACTION
            sharedCharacteristic.OnCharacteristicChanged(characteristic, characteristic?.GetValue());
        }
        catch (Exception e)
        {
            BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, e);
        }
    }

    /// <param name="gatt">GATT client the characteristic is associated with</param>
    /// <param name="characteristic">Characteristic that has been updated as a result of a remote
    /// notification event.</param>
    /// <param name="value">notified characteristic value</param>
    /// <summary>Callback triggered as a result of a remote characteristic notification.</summary>
    /// <remarks>
    ///     <para>Callback triggered as a result of a remote characteristic notification. Note that the value
    /// within the characteristic object may have changed since receiving the remote characteristic
    /// notification, so check the parameter value for the value at the time of notification.</para>
    ///     <para>
    ///         <format type="text/html">
    ///             <a href="https://developer.android.com/reference/android/bluetooth/BluetoothGattCallback#onCharacteristicChanged(android.bluetooth.BluetoothGatt,%20android.bluetooth.BluetoothGattCharacteristic,%20byte[])" title="Reference documentation">Java documentation for <code>android.bluetooth.BluetoothGattCallback.onCharacteristicChanged(android.bluetooth.BluetoothGatt, android.bluetooth.BluetoothGattCharacteristic, byte[])</code>.</a>
    ///         </format>
    ///     </para>
    ///     <para>
    ///         Portions of this page are modifications based on work created and shared by the
    ///         <format type="text/html"><a href="https://developers.google.com/terms/site-policies" title="Android Open Source Project">Android Open Source Project</a></format>
    ///          and used according to terms described in the
    ///         <format type="text/html"><a href="https://creativecommons.org/licenses/by/2.5/" title="Creative Commons 2.5 Attribution License">Creative Commons 2.5 Attribution License.</a></format></para>
    /// </remarks>
    public override void OnCharacteristicChanged(BluetoothGatt? gatt, BluetoothGattCharacteristic? characteristic, byte[]? value)
    {
        try
        {
            // GET SERVICE
            var sharedService = Device.GetService(characteristic?.Service);

            // GET CHARACTERISTIC
            var sharedCharacteristic = sharedService.GetCharacteristic(characteristic);

            // ACTION
            sharedCharacteristic.OnCharacteristicChanged(characteristic, value);
        }
        catch (Exception e)
        {
            BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, e);
        }
    }

    /// <summary>
    ///     Callback indicating the result of a characteristic read operation.
    /// </summary>
    /// <param name="gatt">
    ///     The <see cref="BluetoothGatt" /> instance associated with the connection to the remote device.
    ///     This object manages the GATT profile and provides Bluetooth GATT functionality.
    /// </param>
    /// <param name="characteristic">
    ///     The <see cref="BluetoothGattCharacteristic" /> that was read. This represents a characteristic
    ///     on a GATT service, containing a value as well as optional descriptors.
    /// </param>
    /// <param name="status">
    ///     The <see cref="GattStatus" /> indicating the result of the read operation. Possible values include:
    ///     <list type="bullet">
    ///         <item>
    ///             <description>
    ///                 <c>GattStatus.Success</c> - The read operation was successful.
    ///             </description>
    ///         </item>
    ///         <item>
    ///             <description>
    ///                 <c>GattStatus.ReadNotPermitted</c> - Reading the characteristic is not permitted.
    ///             </description>
    ///         </item>
    ///         <item>
    ///             <description>
    ///                 Other status codes as defined in the GATT protocol or Android Bluetooth API.
    ///             </description>
    ///         </item>
    ///     </list>
    /// </param>
    /// <remarks>
    ///     This method is called when a characteristic read operation completes. The status parameter indicates
    ///     whether the operation was successful or if an error occurred. Applications should override this method
    ///     to handle the result of the read operation as needed.
    ///     For more information, see:
    ///     <list type="bullet">
    ///         <item>
    ///             <description>
    ///                 <a href="https://developer.android.com/reference/android/bluetooth/BluetoothGattCallback#onCharacteristicRead(android.bluetooth.BluetoothGatt,%20android.bluetooth.BluetoothGattCharacteristic,%20int)">
    ///                     BluetoothGattCallback.OnCharacteristicRead (Android API)
    ///                 </a>
    ///             </description>
    ///         </item>
    ///     </list>
    /// </remarks>
    public override void OnCharacteristicRead(BluetoothGatt? gatt, BluetoothGattCharacteristic? characteristic, GattStatus status)
    {
        try
        {
            if (OperatingSystem.IsAndroidVersionAtLeast(33))
            {
                throw new AndroidNativeBluetoothException("This callback OnCharacteristicRead(BluetoothGatt?, BluetoothGattCharacteristic?, GattStatus) should not be triggered in Android versions above 33.");
            }

            // GET SERVICE
            var sharedService = Device.GetService(characteristic?.Service);

            // GET CHARACTERISTIC
            var sharedCharacteristic = sharedService.GetCharacteristic(characteristic);

            // ACTION
            sharedCharacteristic.OnCharacteristicRead(status, characteristic, characteristic?.GetValue());
        }
        catch (Exception e)
        {
            BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, e);
        }
    }

    /// <summary>
    ///     Callback indicating the result of a characteristic read operation.
    /// </summary>
    /// <param name="gatt">
    ///     The <see cref="BluetoothGatt" /> instance associated with the connection to the remote device.
    ///     This object manages the GATT profile and provides Bluetooth GATT functionality.
    /// </param>
    /// <param name="characteristic">
    ///     The <see cref="BluetoothGattCharacteristic" /> that was read. This represents a characteristic
    ///     on a GATT service, containing a value as well as optional descriptors.
    /// </param>
    /// <param name="value">
    ///     A byte array containing the value read from the characteristic. This is the data received from the remote device.
    /// </param>
    /// <param name="status">
    ///     The <see cref="GattStatus" /> indicating the result of the read operation. Possible values include:
    ///     <list type="bullet">
    ///         <item>
    ///             <description>
    ///                 <c>GattStatus.Success</c> - The read operation was successful.
    ///             </description>
    ///         </item>
    ///         <item>
    ///             <description>
    ///                 <c>GattStatus.ReadNotPermitted</c> - Reading the characteristic is not permitted.
    ///             </description>
    ///         </item>
    ///         <item>
    ///             <description>
    ///                 Other status codes as defined in the GATT protocol or Android Bluetooth API.
    ///             </description>
    ///         </item>
    ///     </list>
    /// </param>
    /// <remarks>
    ///     This method is called when a characteristic read operation completes. The status parameter indicates
    ///     whether the operation was successful or if an error occurred. Applications should override this method
    ///     to handle the result of the read operation as needed.
    ///     For more information, see:
    ///     <list type="bullet">
    ///         <item>
    ///             <description>
    ///                 <a href="https://developer.android.com/reference/android/bluetooth/BluetoothGattCallback#onCharacteristicRead(android.bluetooth.BluetoothGatt,%20android.bluetooth.BluetoothGattCharacteristic,%20byte[],%20int)">
    ///                     BluetoothGattCallback.OnCharacteristicRead (Android API)
    ///                 </a>
    ///             </description>
    ///         </item>
    ///     </list>
    /// </remarks>
    public override void OnCharacteristicRead(BluetoothGatt? gatt, BluetoothGattCharacteristic? characteristic, byte[]? value, GattStatus status)
    {
        try
        {
            // GET SERVICE
            var sharedService = Device.GetService(characteristic?.Service);

            // GET CHARACTERISTIC
            var sharedCharacteristic = sharedService.GetCharacteristic(characteristic);

            // ACTION
            sharedCharacteristic.OnCharacteristicRead(status, characteristic, value);
        }
        catch (Exception e)
        {
            BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, e);
        }
    }

    /// <summary>
    ///     Callback indicating the result of a characteristic write operation.
    /// </summary>
    /// <param name="gatt">
    ///     The <see cref="BluetoothGatt" /> instance associated with the connection to the remote device.
    ///     This object manages the GATT profile and provides Bluetooth GATT functionality.
    /// </param>
    /// <param name="characteristic">
    ///     The <see cref="BluetoothGattCharacteristic" /> that was written. This represents a characteristic
    ///     on a GATT service, containing a value as well as optional descriptors.
    /// </param>
    /// <param name="status">
    ///     The <see cref="GattStatus" /> indicating the result of the write operation. Possible values include:
    ///     <list type="bullet">
    ///         <item>
    ///             <description>
    ///                 <c>GattStatus.Success</c> - The write operation was successful.
    ///             </description>
    ///         </item>
    ///         <item>
    ///             <description>
    ///                 <c>GattStatus.WriteNotPermitted</c> - Writing to the characteristic is not permitted.
    ///             </description>
    ///         </item>
    ///         <item>
    ///             <description>
    ///                 Other status codes as defined in the GATT protocol or Android Bluetooth API.
    ///             </description>
    ///         </item>
    ///     </list>
    /// </param>
    /// <remarks>
    ///     This method is called when a characteristic write operation completes. The status parameter indicates
    ///     whether the operation was successful or if an error occurred. Applications should override this method
    ///     to handle the result of the write operation as needed.
    ///     For more information, see:
    ///     <list type="bullet">
    ///         <item>
    ///             <description>
    ///                 <a href="https://developer.android.com/reference/android/bluetooth/BluetoothGattCallback#onCharacteristicWrite(android.bluetooth.BluetoothGatt,%20android.bluetooth.BluetoothGattCharacteristic,%20int)">
    ///                     BluetoothGattCallback.OnCharacteristicWrite (Android API)
    ///                 </a>
    ///             </description>
    ///         </item>
    ///     </list>
    /// </remarks>
    public override void OnCharacteristicWrite(BluetoothGatt? gatt, BluetoothGattCharacteristic? characteristic, GattStatus status)
    {
        try
        {
            // GET SERVICE
            var sharedService = Device.GetService(characteristic?.Service);

            // GET CHARACTERISTIC
            var sharedCharacteristic = sharedService.GetCharacteristic(characteristic);

            // ACTION
            sharedCharacteristic.OnCharacteristicWrite(status, characteristic);
        }
        catch (Exception e)
        {
            BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, e);
        }
    }

    /// <summary>
    ///     Callback indicating the result of a descriptor read operation.
    /// </summary>
    /// <param name="gatt">
    ///     The <see cref="BluetoothGatt" /> instance associated with the connection to the remote device.
    ///     This object manages the GATT profile and provides Bluetooth GATT functionality.
    /// </param>
    /// <param name="descriptor">
    ///     The <see cref="BluetoothGattDescriptor" /> that was read. A descriptor provides additional
    ///     metadata or configuration for a characteristic, such as enabling notifications or specifying a value type.
    /// </param>
    /// <param name="status">
    ///     The <see cref="GattStatus" /> indicating the result of the descriptor read operation. Possible values include:
    ///     <list type="bullet">
    ///         <item>
    ///             <description>
    ///                 <c>GattStatus.Success</c> - The descriptor read operation was successful.
    ///             </description>
    ///         </item>
    ///         <item>
    ///             <description>
    ///                 <c>GattStatus.ReadNotPermitted</c> - Reading the descriptor is not allowed.
    ///             </description>
    ///         </item>
    ///         <item>
    ///             <description>
    ///                 Other status codes as defined in the GATT protocol or Android Bluetooth API.
    ///             </description>
    ///         </item>
    ///     </list>
    /// </param>
    /// <remarks>
    ///     This method is called when a descriptor read operation completes. The status indicates whether
    ///     the operation was successful, and the descriptor provides information about the characteristic
    ///     or additional configuration options. Applications can override this method to handle the result
    ///     of the descriptor read operation.
    ///     For more information, see:
    ///     <list type="bullet">
    ///         <item>
    ///             <description>
    ///                 <a href="https://developer.android.com/reference/android/bluetooth/BluetoothGattCallback#onDescriptorRead(android.bluetooth.BluetoothGatt,%20android.bluetooth.BluetoothGattDescriptor,%20int)">
    ///                     BluetoothGattCallback.OnDescriptorRead (Android API)
    ///                 </a>
    ///             </description>
    ///         </item>
    ///     </list>
    /// </remarks>
    public override void OnDescriptorRead(BluetoothGatt? gatt, BluetoothGattDescriptor? descriptor, GattStatus status)
    {
        try
        {
            if (OperatingSystem.IsAndroidVersionAtLeast(33))
            {
                throw new AndroidNativeBluetoothException("This callback OnDescriptorRead(BluetoothGatt?, BluetoothGattDescriptor?, GattStatus) should not be triggered in Android versions above 33.");
            }

            // GET SERVICE
            var sharedService = Device.GetService(descriptor?.Characteristic?.Service);

            // GET CHARACTERISTIC
            var sharedCharacteristic = sharedService.GetCharacteristic(descriptor?.Characteristic);

            // ACTION
            sharedCharacteristic.OnDescriptorRead(status, descriptor, descriptor?.GetValue());
        }
        catch (Exception e)
        {
            BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, e);
        }
    }

    /// <summary>
    ///     Callback indicating the result of a descriptor read operation, including the read value.
    /// </summary>
    /// <param name="gatt">
    ///     The <see cref="BluetoothGatt" /> instance associated with the connection to the remote device.
    ///     This object manages the GATT profile and provides Bluetooth GATT functionality.
    /// </param>
    /// <param name="descriptor">
    ///     The <see cref="BluetoothGattDescriptor" /> that was read. A descriptor provides additional
    ///     metadata or configuration for a characteristic, such as enabling notifications or specifying a value type.
    /// </param>
    /// <param name="status">
    ///     The <see cref="GattStatus" /> indicating the result of the descriptor read operation. Possible values include:
    ///     <list type="bullet">
    ///         <item>
    ///             <description>
    ///                 <c>GattStatus.Success</c> - The descriptor read operation was successful.
    ///             </description>
    ///         </item>
    ///         <item>
    ///             <description>
    ///                 <c>GattStatus.ReadNotPermitted</c> - Reading the descriptor is not allowed.
    ///             </description>
    ///         </item>
    ///         <item>
    ///             <description>
    ///                 Other status codes as defined in the GATT protocol or Android Bluetooth API.
    ///             </description>
    ///         </item>
    ///     </list>
    /// </param>
    /// <param name="value">
    ///     A byte array containing the value read from the descriptor. This value represents metadata or
    ///     configuration data for the associated characteristic.
    /// </param>
    /// <remarks>
    ///     This method is called when a descriptor read operation completes, providing the status and the
    ///     value read from the descriptor. The descriptor provides additional information about the
    ///     associated characteristic or configuration data for it. Applications can override this method
    ///     to handle the result of the descriptor read operation.
    ///     For more information, see:
    ///     <list type="bullet">
    ///         <item>
    ///             <description>
    ///                 <a href="https://developer.android.com/reference/android/bluetooth/BluetoothGattCallback#onDescriptorRead(android.bluetooth.BluetoothGatt,%20android.bluetooth.BluetoothGattDescriptor,%20int,%20byte[])">
    ///                     BluetoothGattCallback.OnDescriptorRead (Android API)
    ///                 </a>
    ///             </description>
    ///         </item>
    ///     </list>
    /// </remarks>
    public override void OnDescriptorRead(BluetoothGatt? gatt, BluetoothGattDescriptor? descriptor, GattStatus status, byte[]? value)
    {
        try
        {
            // GET SERVICE
            var sharedService = Device.GetService(descriptor?.Characteristic?.Service);

            // GET CHARACTERISTIC
            var sharedCharacteristic = sharedService.GetCharacteristic(descriptor?.Characteristic);

            // ACTION
            sharedCharacteristic.OnDescriptorRead(status, descriptor, value);
        }
        catch (Exception e)
        {
            BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, e);
        }
    }

    /// <summary>
    ///     Callback indicating the result of a descriptor write operation.
    /// </summary>
    /// <param name="gatt">
    ///     The <see cref="BluetoothGatt" /> instance associated with the connection to the remote device.
    ///     This object manages the GATT profile and provides Bluetooth GATT functionality.
    /// </param>
    /// <param name="descriptor">
    ///     The <see cref="BluetoothGattDescriptor" /> that was written. A descriptor provides additional
    ///     metadata or configuration for a characteristic, such as enabling notifications or specifying a value type.
    /// </param>
    /// <param name="status">
    ///     The <see cref="GattStatus" /> indicating the result of the descriptor write operation. Possible values include:
    ///     <list type="bullet">
    ///         <item>
    ///             <description>
    ///                 <c>GattStatus.Success</c> - The descriptor write operation was successful.
    ///             </description>
    ///         </item>
    ///         <item>
    ///             <description>
    ///                 <c>GattStatus.WriteNotPermitted</c> - Writing to the descriptor is not allowed.
    ///             </description>
    ///         </item>
    ///         <item>
    ///             <description>
    ///                 Other status codes as defined in the GATT protocol or Android Bluetooth API.
    ///             </description>
    ///         </item>
    ///     </list>
    /// </param>
    /// <remarks>
    ///     This method is called when a descriptor write operation completes. The status indicates whether
    ///     the operation was successful. Descriptors are often used for configuration purposes, such as enabling
    ///     notifications or indications for a characteristic. Applications can override this method to handle
    ///     the result of the descriptor write operation.
    ///     For more information, see:
    ///     <list type="bullet">
    ///         <item>
    ///             <description>
    ///                 <a href="https://developer.android.com/reference/android/bluetooth/BluetoothGattCallback#onDescriptorWrite(android.bluetooth.BluetoothGatt,%20android.bluetooth.BluetoothGattDescriptor,%20int)">
    ///                     BluetoothGattCallback.OnDescriptorWrite (Android API)
    ///                 </a>
    ///             </description>
    ///         </item>
    ///     </list>
    /// </remarks>
    public override void OnDescriptorWrite(BluetoothGatt? gatt, BluetoothGattDescriptor? descriptor, GattStatus status)
    {
        try
        {
            // GET SERVICE
            var sharedService = Device.GetService(descriptor?.Characteristic?.Service);

            // GET CHARACTERISTIC
            var sharedCharacteristic = sharedService.GetCharacteristic(descriptor?.Characteristic);

            // ACTION
            sharedCharacteristic.OnDescriptorWrite(status, descriptor);
        }
        catch (Exception e)
        {
            BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, e);
        }
    }

    /// <inheritdoc cref="BluetoothGattCallback.OnServicesDiscovered"/>
    public override void OnServicesDiscovered(BluetoothGatt? gatt, GattStatus status)
    {
        try
        {
            // ACTION
            Device.OnServicesDiscovered(status);
        }
        catch (Exception e)
        {
            BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, e);
        }
    }

    /// <inheritdoc cref="BluetoothGattCallback.OnConnectionStateChange"/>
    public override void OnConnectionStateChange(BluetoothGatt? gatt, GattStatus status, ProfileState newState)
    {
        try
        {
            // ACTION
            Device.OnConnectionStateChange(status, newState);
        }
        catch (Exception e)
        {
            BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, e);
        }
    }

    /// <summary>
    ///     Callback reporting the RSSI (Received Signal Strength Indicator) for a remote device connection.
    /// </summary>
    /// <param name="gatt">
    ///     The <see cref="BluetoothGatt" /> instance associated with the connection to the remote device.
    ///     This object manages the GATT profile and provides Bluetooth GATT functionality.
    /// </param>
    /// <param name="rssi">
    ///     The RSSI value of the remote device, measured in dBm (decibels relative to 1 milli-watt).
    /// </param>
    /// <param name="status">
    ///     The <see cref="GattStatus" /> indicating the result of the RSSI read operation. Possible values include:
    ///     <list type="bullet">
    ///         <item>
    ///             <description>
    ///                 <c>GattStatus.Success</c> - The RSSI read operation was successful.
    ///             </description>
    ///         </item>
    ///         <item>
    ///             <description>
    ///                 Other status codes as defined in the GATT protocol or Android Bluetooth API.
    ///             </description>
    ///         </item>
    ///     </list>
    /// </param>
    /// <remarks>
    ///     This method is called when the RSSI of a connected remote device is read. Applications can use this callback
    ///     to monitor the signal strength of a device connection.
    ///     For more information, see:
    ///     <list type="bullet">
    ///         <item>
    ///             <description>
    ///                 <a href="https://developer.android.com/reference/android/bluetooth/BluetoothGattCallback#onReadRemoteRssi(android.bluetooth.BluetoothGatt,%20int,%20int)">
    ///                     BluetoothGattCallback.OnReadRemoteRssi (Android API)
    ///                 </a>
    ///             </description>
    ///         </item>
    ///     </list>
    /// </remarks>
    public override void OnReadRemoteRssi(BluetoothGatt? gatt, int rssi, GattStatus status)
    {
        try
        {
            // ACTION
            Device.OnReadRemoteRssi(status, rssi);
        }
        catch (Exception e)
        {
            BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, e);
        }
    }

    /// <summary>
    ///     Callback indicating the result of a reliable write operation.
    /// </summary>
    /// <param name="gatt">
    ///     The <see cref="BluetoothGatt" /> instance associated with the connection to the remote device.
    ///     This object manages the GATT profile and provides Bluetooth GATT functionality.
    /// </param>
    /// <param name="status">
    ///     The <see cref="GattStatus" /> indicating the result of the reliable write operation. Possible values include:
    ///     <list type="bullet">
    ///         <item>
    ///             <description>
    ///                 <c>GattStatus.Success</c> - The reliable write operation was successful.
    ///             </description>
    ///         </item>
    ///         <item>
    ///             <description>
    ///                 Other status codes as defined in the GATT protocol or Android Bluetooth API.
    ///             </description>
    ///         </item>
    ///     </list>
    /// </param>
    /// <remarks>
    ///     This method is called when a reliable write operation completes. Reliable writes are used to queue multiple
    ///     write operations and execute them atomically. Applications can override this method to handle the result
    ///     of a reliable write operation.
    ///     For more information, see:
    ///     <list type="bullet">
    ///         <item>
    ///             <description>
    ///                 <a href="https://developer.android.com/reference/android/bluetooth/BluetoothGattCallback#onReliableWriteCompleted(android.bluetooth.BluetoothGatt,%20int)">
    ///                     BluetoothGattCallback.OnReliableWriteCompleted (Android API)
    ///                 </a>
    ///             </description>
    ///         </item>
    ///     </list>
    /// </remarks>
    public override void OnReliableWriteCompleted(BluetoothGatt? gatt, GattStatus status)
    {
        try
        {
            // ACTION
            Device.OnReliableWriteCompleted(status);
        }
        catch (Exception e)
        {
            BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, e);
        }
    }

    /// <summary>
    ///     Callback indicating that the MTU (Maximum Transmission Unit) size has changed.
    /// </summary>
    /// <param name="gatt">
    ///     The <see cref="BluetoothGatt" /> instance associated with the connection to the remote device.
    ///     This object manages the GATT profile and provides Bluetooth GATT functionality.
    /// </param>
    /// <param name="mtu">
    ///     The new MTU size, in bytes, for the connection. The MTU determines the maximum size of a single data packet
    ///     that can be sent over the GATT connection.
    /// </param>
    /// <param name="status">
    ///     The <see cref="GattStatus" /> indicating the result of the MTU change operation. Possible values include:
    ///     <list type="bullet">
    ///         <item>
    ///             <description>
    ///                 <c>GattStatus.Success</c> - The MTU change operation was successful.
    ///             </description>
    ///         </item>
    ///         <item>
    ///             <description>
    ///                 Other status codes as defined in the GATT protocol or Android Bluetooth API.
    ///             </description>
    ///         </item>
    ///     </list>
    /// </param>
    /// <remarks>
    ///     This method is called when the MTU size for a GATT connection changes. Applications can override this method
    ///     to adjust communication logic based on the new MTU size.
    ///     For more information, see:
    ///     <list type="bullet">
    ///         <item>
    ///             <description>
    ///                 <a href="https://developer.android.com/reference/android/bluetooth/BluetoothGattCallback#onMtuChanged(android.bluetooth.BluetoothGatt,%20int,%20int)">
    ///                     BluetoothGattCallback.OnMtuChanged (Android API)
    ///                 </a>
    ///             </description>
    ///         </item>
    ///     </list>
    /// </remarks>
    public override void OnMtuChanged(BluetoothGatt? gatt, int mtu, GattStatus status)
    {
        try
        {
            // ACTION
            Device.OnMtuChanged(status, mtu);
        }
        catch (Exception e)
        {
            BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, e);
        }
    }

    /// <summary>
    ///     Callback indicating the result of a PHY (Physical Layer) read operation.
    /// </summary>
    /// <param name="gatt">
    ///     The <see cref="BluetoothGatt" /> instance associated with the connection to the remote device.
    ///     This object manages the GATT profile and provides Bluetooth GATT functionality.
    /// </param>
    /// <param name="txPhy">
    ///     The transmitter PHY mode. Possible values include:
    ///     <list type="bullet">
    ///         <item>
    ///             <description>
    ///                 <c>ScanSettingsPhy.Le1M</c> - 1M PHY (classic Bluetooth).
    ///             </description>
    ///         </item>
    ///         <item>
    ///             <description>
    ///                 <c>ScanSettingsPhy.Le2M</c> - 2M PHY.
    ///             </description>
    ///         </item>
    ///         <item>
    ///             <description>
    ///                 <c>ScanSettingsPhy.LeCoded</c> - Coded PHY (Long Range).
    ///             </description>
    ///         </item>
    ///     </list>
    /// </param>
    /// <param name="rxPhy">
    ///     The receiver PHY mode. Possible values include:
    ///     <list type="bullet">
    ///         <item>
    ///             <description>
    ///                 <c>ScanSettingsPhy.Le1M</c>, <c>ScanSettingsPhy.Le2M</c>, <c>ScanSettingsPhy.LeCoded</c> (same as above).
    ///             </description>
    ///         </item>
    ///     </list>
    /// </param>
    /// <param name="status">
    ///     The <see cref="GattStatus" /> indicating the result of the PHY read operation. Possible values include:
    ///     <list type="bullet">
    ///         <item>
    ///             <description>
    ///                 <c>GattStatus.Success</c> - The PHY read operation was successful.
    ///             </description>
    ///         </item>
    ///         <item>
    ///             <description>
    ///                 Other status codes as defined in the GATT protocol or Android Bluetooth API.
    ///             </description>
    ///         </item>
    ///     </list>
    /// </param>
    /// <remarks>
    ///     This method is called when the PHY settings of a GATT connection are read. Applications can use this callback
    ///     to determine the PHY mode being used for data transmission and reception.
    ///     For more information, see:
    ///     <list type="bullet">
    ///         <item>
    ///             <description>
    ///                 <a href="https://developer.android.com/reference/android/bluetooth/BluetoothGattCallback#onPhyRead(android.bluetooth.BluetoothGatt,%20int,%20int,%20int)">
    ///                     BluetoothGattCallback.OnPhyRead (Android API)
    ///                 </a>
    ///             </description>
    ///         </item>
    ///     </list>
    /// </remarks>
    public override void OnPhyRead(BluetoothGatt? gatt, ScanSettingsPhy txPhy, ScanSettingsPhy rxPhy, GattStatus status)
    {
        try
        {
            // ACTION
            Device.OnPhyRead(status, (Android.Bluetooth.BluetoothPhy) txPhy, (Android.Bluetooth.BluetoothPhy) rxPhy);
        }
        catch (Exception e)
        {
            BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, e);
        }
    }

    /// <summary>
    ///     Callback indicating the result of a PHY (Physical Layer) update operation.
    /// </summary>
    /// <param name="gatt">
    ///     The <see cref="BluetoothGatt" /> instance associated with the connection to the remote device.
    ///     This object manages the GATT profile and provides Bluetooth GATT functionality.
    /// </param>
    /// <param name="txPhy">
    ///     The updated transmitter PHY mode. Possible values are the same as in <see cref="OnPhyRead" />.
    /// </param>
    /// <param name="rxPhy">
    ///     The updated receiver PHY mode. Possible values are the same as in <see cref="OnPhyRead" />.
    /// </param>
    /// <param name="status">
    ///     The <see cref="GattStatus" /> indicating the result of the PHY update operation. Possible values include:
    ///     <list type="bullet">
    ///         <item>
    ///             <description>
    ///                 <c>GattStatus.Success</c> - The PHY update operation was successful.
    ///             </description>
    ///         </item>
    ///         <item>
    ///             <description>
    ///                 Other status codes as defined in the GATT protocol or Android Bluetooth API.
    ///             </description>
    ///         </item>
    ///     </list>
    /// </param>
    /// <remarks>
    ///     This method is called when the PHY settings of a GATT connection are updated. Applications can use this callback
    ///     to adjust their behavior based on the new PHY settings.
    ///     For more information, see:
    ///     <list type="bullet">
    ///         <item>
    ///             <description>
    ///                 <a href="https://developer.android.com/reference/android/bluetooth/BluetoothGattCallback#onPhyUpdate(android.bluetooth.BluetoothGatt,%20int,%20int,%20int)">
    ///                     BluetoothGattCallback.OnPhyUpdate (Android API)
    ///                 </a>
    ///             </description>
    ///         </item>
    ///     </list>
    /// </remarks>
    public override void OnPhyUpdate(BluetoothGatt? gatt, ScanSettingsPhy txPhy, ScanSettingsPhy rxPhy, GattStatus status)
    {
        try
        {
            // ACTION
            Device.OnPhyUpdate(status, (Android.Bluetooth.BluetoothPhy) txPhy, (Android.Bluetooth.BluetoothPhy) rxPhy);
        }
        catch (Exception e)
        {
            BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, e);
        }
    }

    /// <summary>
    ///     Callback indicating that the services of a remote device have changed.
    /// </summary>
    /// <param name="gatt">
    ///     The <see cref="BluetoothGatt" /> instance associated with the connection to the remote device.
    ///     This object manages the GATT profile and provides Bluetooth GATT functionality.
    /// </param>
    /// <remarks>
    ///     This method is called when the GATT services of the remote device have been updated. Applications can
    ///     override this method to rediscover the services and handle the new configuration as needed.
    ///     For more information, see:
    ///     <list type="bullet">
    ///         <item>
    ///             <description>
    ///                 <a href="https://developer.android.com/reference/android/bluetooth/BluetoothGattCallback#onServiceChanged(android.bluetooth.BluetoothGatt)">
    ///                     BluetoothGattCallback.OnServiceChanged (Android API)
    ///                 </a>
    ///             </description>
    ///         </item>
    ///     </list>
    /// </remarks>
    public override void OnServiceChanged(BluetoothGatt? gatt)
    {
        try
        {
            // ACTION
            Device.OnServiceChanged();
        }
        catch (Exception e)
        {
            BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, e);
        }
    }
}

