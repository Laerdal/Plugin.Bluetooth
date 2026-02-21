namespace Bluetooth.Maui.Platforms.Win.Scanning.NativeObjects;

/// <summary>
///     Proxy class for Windows GATT characteristic that provides event handling and lifecycle management
///     for GATT characteristic operations.
/// </summary>
public sealed partial class GattCharacteristicProxy : IDisposable
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="GattCharacteristicProxy" /> class.
    /// </summary>
    /// <param name="gattCharacteristic">The native Windows GATT characteristic instance.</param>
    /// <param name="bluetoothCharacteristicProxyDelegate">The delegate for handling GATT characteristic events.</param>
    public GattCharacteristicProxy(GattCharacteristic gattCharacteristic, IBluetoothCharacteristicProxyDelegate bluetoothCharacteristicProxyDelegate)
    {
        GattCharacteristicProxyDelegate = bluetoothCharacteristicProxyDelegate;
        GattCharacteristic = gattCharacteristic;
        GattCharacteristic.ValueChanged += OnValueChanged;
    }

    /// <summary>
    ///     Gets the delegate responsible for handling GATT characteristic events.
    /// </summary>
    private IBluetoothCharacteristicProxyDelegate GattCharacteristicProxyDelegate { get; }

    /// <summary>
    ///     Gets the native Windows GATT characteristic instance.
    /// </summary>
    public GattCharacteristic GattCharacteristic { get; }

    /// <summary>
    ///     Releases all resources used by the <see cref="GattCharacteristicProxy" /> instance.
    /// </summary>
    public void Dispose()
    {
        GattCharacteristic.ValueChanged -= OnValueChanged;
    }

    /// <summary>
    ///     Handles characteristic value change events and forwards them to the delegate.
    /// </summary>
    /// <param name="sender">The GATT characteristic that changed value.</param>
    /// <param name="args">The value change event arguments.</param>
    private void OnValueChanged(GattCharacteristic sender, GattValueChangedEventArgs args)
    {
        try
        {
            GattCharacteristicProxyDelegate.OnValueChanged(args.CharacteristicValue.ToArray(), args.Timestamp);
        }
        catch (Exception e)
        {
            BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, e);
        }
    }
}