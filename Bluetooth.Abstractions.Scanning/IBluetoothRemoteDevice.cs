namespace Bluetooth.Abstractions.Scanning;

/// <summary>
/// Interface representing a Bluetooth device, providing properties and methods for interacting with it.
/// </summary>
public partial interface IBluetoothRemoteDevice : INotifyPropertyChanged, IAsyncDisposable
{
    /// <summary>
    /// Gets the Bluetooth scanner associated with this device.
    /// </summary>
    IBluetoothScanner Scanner { get; }
}

