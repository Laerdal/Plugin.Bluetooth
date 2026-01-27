using System.Diagnostics.CodeAnalysis;

namespace Bluetooth.Abstractions.Broadcasting;

/// <summary>
/// Factory interface for creating Bluetooth broadcast services.
/// </summary>
[SuppressMessage("Design", "CA1034:Nested types should not be visible")]
public interface IBluetoothBroadcastServiceFactory
{
    /// <summary>
    /// Creates a Bluetooth broadcast service based on the provided request.
    /// </summary>
    /// <param name="broadcaster">The Bluetooth broadcaster to which the service will be associated.</param>
    /// <param name="request">The request containing the details for creating the service.</param>
    /// <param name="timeout">The timeout for this operation.</param>
    /// <param name="cancellationToken">A cancellation token to cancel this operation.</param>
    /// <returns>The created Bluetooth broadcast service.</returns>
    ValueTask<IBluetoothBroadcastService> CreateBroadcastServiceAsync(IBluetoothBroadcaster broadcaster, BluetoothBroadcastServiceFactoryRequest request, TimeSpan? timeout = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Record representing a request to create a Bluetooth broadcast service.
    /// </summary>
    public record BluetoothBroadcastServiceFactoryRequest
    {
        /// <summary>
        /// The unique identifier of the service.
        /// </summary>
        public Guid Id { get; init; }

        /// <summary>
        /// The name of the service.
        /// </summary>
        public string Name { get; init; } = null!;

        /// <summary>
        /// Gets if the service is primary or not (default = true)
        /// </summary>
        public bool IsPrimary { get; init; } = true;
    }

}
