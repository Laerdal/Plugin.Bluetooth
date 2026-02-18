namespace Bluetooth.Abstractions.Scanning.AccessService;

/// <summary>
/// Generic interface representing a service for accessing Bluetooth characteristics with different input and output types.
/// </summary>
public partial interface IBluetoothCharacteristicAccessService<TRead, TWrite>
{
    /// <summary>
    /// Determines whether the specified device can read the characteristic asynchronously.
    /// </summary>
    /// <param name="device">The Bluetooth device.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a boolean indicating whether the device can read the characteristic.</returns>
    ValueTask<bool> CanReadAsync(IBluetoothDevice device);

    /// <summary>
    /// Reads the value of the characteristic asynchronously.
    /// </summary>
    /// <param name="device">The Bluetooth device.</param>
    /// <param name="useLastValueIfPreviouslyRead">If true, uses the last value if it was previously read.</param>
    /// <param name="timeout">The timeout for this operation</param>
    /// <param name="cancellationToken">A cancellation token to cancel this operation.</param>
    /// <returns>A task that represents the asynchronous read operation. The task result contains the value read.</returns>
    ValueTask<TRead> ReadAsync(IBluetoothDevice device, bool useLastValueIfPreviouslyRead = false, TimeSpan? timeout = null, CancellationToken cancellationToken = default);

}