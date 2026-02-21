namespace Bluetooth.Abstractions.Broadcasting;

/// <summary>
///     Represents a service in the context of bluetooth broadcasting.
/// </summary>
public partial interface IBluetoothLocalService : INotifyPropertyChanged, IAsyncDisposable
{
    /// <summary>
    ///     Gets the Bluetooth broadcaster hosting this service.
    /// </summary>
    IBluetoothBroadcaster Broadcaster { get; }

    /// <summary>
    ///     Gets the name of the service.
    /// </summary>
    string Name { get; }

    /// <summary>
    ///     Gets the service UUID.
    /// </summary>
    Guid Id { get; }

    /// <summary>
    ///     Gets a value indicating whether this service is a primary service.
    /// </summary>
    bool IsPrimary { get; }
}