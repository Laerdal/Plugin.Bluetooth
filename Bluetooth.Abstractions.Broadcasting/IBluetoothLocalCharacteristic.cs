namespace Bluetooth.Abstractions.Broadcasting;

/// <summary>
///     Represents a characteristic in the context of bluetooth broadcasting.
/// </summary>
public partial interface IBluetoothLocalCharacteristic : INotifyPropertyChanged, IAsyncDisposable
{
    /// <summary>
    ///     Gets the Bluetooth service hosting this characteristic.
    /// </summary>
    IBluetoothLocalService LocalService { get; }

    /// <summary>
    ///     Gets the name of the characteristic.
    /// </summary>
    string Name { get; }

    /// <summary>
    ///     Gets the unique identifier of the characteristic.
    /// </summary>
    Guid Id { get; }

    /// <summary>
    ///     Gets the GATT properties supported by this characteristic (Read/Write/Notify/Indicate, etc.).
    /// </summary>
    BluetoothCharacteristicProperties Properties { get; init; }

    /// <summary>
    ///     The permissions of the characteristic.
    /// </summary>
    BluetoothCharacteristicPermissions Permissions { get; init; }
}