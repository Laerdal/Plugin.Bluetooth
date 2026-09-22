namespace Bluetooth.Abstractions;

/// <summary>
///     Interface for managing a Bluetooth activity's lifecycle and state.
/// </summary>
public interface IBluetoothAdapter : INotifyPropertyChanged
{
    /// <summary>
    ///     Gets whether the platform's Bluetooth radio/adapter is currently enabled.
    /// </summary>
    /// <remarks>
    ///     Raises <see cref="INotifyPropertyChanged.PropertyChanged" /> for this property whenever the
    ///     underlying radio state changes, so consumers can react live (e.g. show a "Bluetooth is off"
    ///     banner) instead of only finding out the next time a scan fails to start.
    /// </remarks>
    bool IsEnabled { get; }
}
