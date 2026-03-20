namespace Bluetooth.Abstractions.Broadcasting;

/// <summary>
///     Represents a characteristic in the context of bluetooth broadcasting.
/// </summary>
public partial interface IBluetoothLocalCharacteristic
{
    #region Write

    /// <summary>
    ///     Event raised when a client device requests to write to this characteristic.
    /// </summary>
    event EventHandler<CharacteristicWriteRequestEventArgs>? WriteRequested;

    // TODO : Implement Write request handling (response customisation) once all platforms are investigated.

    #endregion
}
