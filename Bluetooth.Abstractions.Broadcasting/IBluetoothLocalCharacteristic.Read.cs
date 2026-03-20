namespace Bluetooth.Abstractions.Broadcasting;

/// <summary>
///     Represents a characteristic in the context of bluetooth broadcasting.
/// </summary>
public partial interface IBluetoothLocalCharacteristic
{
    #region Read

    /// <summary>
    ///     Event raised when a client device requests to read this characteristic.
    /// </summary>
    event EventHandler<CharacteristicReadRequestEventArgs>? ReadRequested;

    // TODO : Implement Read request handling (response customisation) once all platforms are investigated.

    #endregion
}
