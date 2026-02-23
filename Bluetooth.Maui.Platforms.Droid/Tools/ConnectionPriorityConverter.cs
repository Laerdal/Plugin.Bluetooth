namespace Bluetooth.Maui.Platforms.Droid.Tools;

/// <summary>
///     Provides extension methods for converting connection priority values to native Android types.
/// </summary>
public static class ConnectionPriorityConverter
{
    /// <summary>
    ///     Converts a <see cref="BluetoothConnectionPriority" /> value to the corresponding Android <see cref="GattConnectionPriority" /> value.
    /// </summary>
    /// <param name="priority">The abstract connection priority value.</param>
    /// <returns>The corresponding Android GATT connection priority.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the priority value is not valid.</exception>
    /// <remarks>
    ///     Connection priority affects the connection interval, slave latency, and supervision timeout.
    ///     <list type="bullet">
    ///         <item><description><see cref="BluetoothConnectionPriority.LowPower"/>: Min interval 100ms, max interval 125ms - conserves battery</description></item>
    ///         <item><description><see cref="BluetoothConnectionPriority.Balanced"/>: Min interval 30ms, max interval 50ms - balanced performance</description></item>
    ///         <item><description><see cref="BluetoothConnectionPriority.High"/>: Min interval 11.25ms, max interval 15ms - maximum throughput</description></item>
    ///     </list>
    ///     <seealso href="https://developer.android.com/reference/android/bluetooth/BluetoothGatt#requestConnectionPriority(int)">Android BluetoothGatt.requestConnectionPriority()</seealso>
    /// </remarks>
    public static GattConnectionPriority ToAndroidGattConnectionPriority(this BluetoothConnectionPriority priority)
    {
        return priority switch
        {
            BluetoothConnectionPriority.High => GattConnectionPriority.High,
            BluetoothConnectionPriority.Balanced => GattConnectionPriority.Balanced,
            BluetoothConnectionPriority.LowPower => GattConnectionPriority.LowPower,
            _ => throw new ArgumentOutOfRangeException(nameof(priority), priority, "Invalid connection priority")
        };
    }

    /// <summary>
    ///     Converts an Android <see cref="GattConnectionPriority" /> value to the corresponding <see cref="BluetoothConnectionPriority" /> value.
    /// </summary>
    /// <param name="androidPriority">The Android GATT connection priority value.</param>
    /// <returns>The corresponding abstract connection priority.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the Android priority value is not valid.</exception>
    /// <remarks>
    ///     This method provides bidirectional conversion for connection priority values.
    /// </remarks>
    public static BluetoothConnectionPriority ToSharedConnectionPriority(this GattConnectionPriority androidPriority)
    {
        return androidPriority switch
        {
            GattConnectionPriority.High => BluetoothConnectionPriority.High,
            GattConnectionPriority.Balanced => BluetoothConnectionPriority.Balanced,
            GattConnectionPriority.LowPower => BluetoothConnectionPriority.LowPower,
            _ => throw new ArgumentOutOfRangeException(nameof(androidPriority), androidPriority, "Invalid Android connection priority")
        };
    }
}
