namespace Bluetooth.Maui.Platforms.Droid.Scanning;

public record BluetoothDeviceConnectionOptions : BaseBluetoothDeviceConnectionOptions
{
    /// <summary>
    /// Whether to directly connect to the remote device (false) or to automatically connect as soon as the remote device becomes available (true).
    /// </summary>
    public bool UseAutoConnect { get; init; }

    /// <summary>
    /// preferred PHY for connections to remote LE device. Bitwise OR of any of PHY_LE_1M_MASK, BluetoothDevice.PHY_LE_2M_MASK, an dPHY_LE_CODED_MASK. This option does not take effect if autoConnect is set to true.
    /// </summary>
    public Android.Bluetooth.BluetoothPhy? BluetoothPhy { get; init; }

    /// <summary>
    /// Preferred transport for GATT connections to remote dual-mode devices <see cref="Android.Bluetooth.BluetoothTransports.Auto"/> or <see cref="Android.Bluetooth.BluetoothTransports.Bredr"/> or <see cref="Android.Bluetooth.BluetoothTransports.Le"/>
    /// </summary>
    public BluetoothTransports? BluetoothTransports { get; init; }
}
