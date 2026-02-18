namespace Bluetooth.Abstractions.Scanning.AccessService;

/// <summary>
/// Interface representing a service for accessing Bluetooth characteristics, providing methods for reading, writing, and listening to characteristics.
/// </summary>
public partial interface IBluetoothCharacteristicAccessService
{
    /// <summary>
    /// Gets the characteristic associated with the specified device asynchronously.
    /// </summary>
    /// <param name="device">The Bluetooth device.</param>
    /// <param name="timeout">The timeout for this operation.</param>
    /// <param name="cancellationToken">A cancellation token to cancel this operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the Bluetooth characteristic.</returns>
    ValueTask<IBluetoothCharacteristic> GetCharacteristicAsync(IBluetoothDevice device, TimeSpan? timeout = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Determines whether the specified device has the characteristic asynchronously.
    /// </summary>
    /// <param name="device">The Bluetooth device.</param>
    /// <param name="timeout">The timeout for this operation.</param>
    /// <param name="cancellationToken">A cancellation token to cancel this operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a boolean indicating whether the device has the characteristic.</returns>
    ValueTask<bool> HasCharacteristicAsync(IBluetoothDevice device, TimeSpan? timeout = null, CancellationToken cancellationToken = default);
}
