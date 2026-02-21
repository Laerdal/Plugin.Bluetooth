namespace Bluetooth.Abstractions.Scanning;

/// <summary>
///     Interface representing a Bluetooth service, providing properties and methods for interacting with it.
/// </summary>
public partial interface IBluetoothRemoteService : INotifyPropertyChanged, IAsyncDisposable
{
    /// <summary>
    ///     Gets the Bluetooth device associated with this service.
    /// </summary>
    IBluetoothRemoteDevice Device { get; }

    /// <summary>
    ///     Gets the universally unique identifier (UUID) of the service.
    /// </summary>
    Guid Id { get; }

    /// <summary>
    ///     The name of the Bluetooth service. This is typically used for debugging and logging purposes, and may not be available for all services. If the service is not recognized, this will default to "Unknown Service".
    /// </summary>
    string Name { get; }
}