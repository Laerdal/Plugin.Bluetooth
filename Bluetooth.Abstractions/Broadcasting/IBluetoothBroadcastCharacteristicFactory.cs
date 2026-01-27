using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;

using Bluetooth.Abstractions.Enums;

namespace Bluetooth.Abstractions.Broadcasting;

/// <summary>
/// Factory interface for creating Bluetooth broadcast characteristics.
/// </summary>
[SuppressMessage("Design", "CA1034:Nested types should not be visible")]
public interface IBluetoothBroadcastCharacteristicFactory
{
    /// <summary>
    /// Creates a Bluetooth broadcast characteristic based on the provided request.
    /// </summary>
    /// <param name="service">The Bluetooth broadcast service to which the characteristic will belong.</param>
    /// <param name="request">The request containing the details for creating the characteristic.</param>
    /// <param name="timeout">An optional timeout for the creation operation.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>The created Bluetooth broadcast characteristic.</returns>
    ValueTask<IBluetoothBroadcastCharacteristic> CreateBroadcastCharacteristicAsync(IBluetoothBroadcastService service, BluetoothBroadcastCharacteristicFactoryRequest request, TimeSpan? timeout = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Record representing a request to create a Bluetooth broadcast characteristic.
    /// </summary>
    public record BluetoothBroadcastCharacteristicFactoryRequest
    {
        /// <summary>
        /// The unique identifier of the characteristic.
        /// </summary>
        public Guid Id { get; init; }

        /// <summary>
        /// The name of the characteristic.
        /// </summary>
        public string Name { get; init; } = null!;

        /// <summary>
        /// Indicates whether the service is a primary service.
        /// </summary>
        public bool IsPrimary { get; init; }

        /// <summary>
        /// The properties of the characteristic (read, write, notify, etc.).
        /// </summary>
        public CharacteristicProperties Properties { get; init; }

        /// <summary>
        /// The permissions required to access the characteristic.
        /// </summary>
        public CharacteristicPermissions Permissions { get; init; }

        /// <summary>
        /// The initial value of the characteristic.
        /// </summary>
        public ReadOnlyMemory<byte>? InitialValue { get; init; }
    }

}
