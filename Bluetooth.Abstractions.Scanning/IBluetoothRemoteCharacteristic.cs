namespace Bluetooth.Abstractions.Scanning;

/// <summary>
///     Interface representing a Bluetooth characteristic, providing properties and methods for interacting with it.
/// </summary>
public partial interface IBluetoothRemoteCharacteristic : INotifyPropertyChanged, IAsyncDisposable
{
    /// <summary>
    ///     Gets the Bluetooth service associated with this characteristic.
    /// </summary>
    IBluetoothRemoteService RemoteService { get; }

    /// <summary>
    ///     Gets the universally unique identifier (UUID) of the characteristic.
    /// </summary>
    Guid Id { get; }

    /// <summary>
    ///     The name of the Bluetooth characteristic. This is typically used for debugging and logging purposes, and may not be available for all characteristics. If the characteristic is not recognized, this will default to "Unknown
    ///     Characteristic".
    /// </summary>
    string Name { get; }
}