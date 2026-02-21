namespace Bluetooth.Abstractions.Broadcasting;

/// <summary>
///     Represents a Bluetooth device that has connected to a broadcast (GATT server).
/// </summary>
public partial interface IBluetoothConnectedDevice
{
    #region MTU (Maximum Transmission Unit)

    /// <summary>
    ///     Gets the current Maximum Transmission Unit (MTU) for this client connection.
    ///     The MTU determines the maximum size of a single packet that can be sent.
    /// </summary>
    int Mtu { get; }

    /// <summary>
    ///     Event raised when the MTU changes for this client connection.
    /// </summary>
    event EventHandler<MtuChangedEventArgs>? MtuChanged;

    #endregion
}