using System.Diagnostics.CodeAnalysis;

using Bluetooth.Abstractions.AccessService;

namespace Bluetooth.Abstractions.Scanning;

/// <summary>
/// Factory interface for creating Bluetooth characteristic instances.
/// </summary>
[SuppressMessage("Design", "CA1034:Nested types should not be visible")]
public interface IBluetoothCharacteristicFactory
{
    /// <summary>
    /// Creates a new instance of a Bluetooth characteristic.
    /// </summary>
    /// <param name="service">The Bluetooth service to which the characteristic belongs.</param>
    /// <param name="request">The request containing information needed to create the characteristic.</param>
    /// <param name="timeout">An optional timeout for the creation operation.</param>
    /// <param name="cancellationToken">A cancellation token to cancel the operation.</param>
    /// <returns>A new instance of <see cref="IBluetoothCharacteristic"/>.</returns>
    ValueTask<IBluetoothCharacteristic> CreateCharacteristicAsync(IBluetoothService service, BluetoothCharacteristicFactoryRequest request, TimeSpan? timeout = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Request object for creating Bluetooth characteristic instances.
    /// </summary>
    public record BluetoothCharacteristicFactoryRequest
    {
        /// <summary>
        /// The access service for the characteristic.
        /// </summary>
        public IBluetoothCharacteristicAccessService AccessService { get; init; } = null!;
    }

}
