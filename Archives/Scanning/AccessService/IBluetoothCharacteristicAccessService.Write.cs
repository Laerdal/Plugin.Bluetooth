namespace Bluetooth.Abstractions.Scanning.AccessService;

/// <summary>
/// Generic interface representing a service for accessing Bluetooth characteristics with different input and output types.
/// </summary>
public partial interface IBluetoothCharacteristicAccessService<TRead, TWrite>
{
    /// <summary>
    /// Determines whether the specified device can write to the characteristic asynchronously.
    /// </summary>
    /// <param name="device">The Bluetooth device.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a boolean indicating whether the device can write to the characteristic.</returns>
    ValueTask<bool> CanWriteAsync(IBluetoothDevice device);

    /// <summary>
    /// Writes a value to the characteristic asynchronously.
    /// </summary>
    /// <param name="device">The Bluetooth device.</param>
    /// <param name="value">The value to write.</param>
    /// <param name="skipIfOldValueMatchesNewValue">If true, skips writing if the old value matches the new value.</param>
    /// <param name="timeout">The timeout for this operation</param>
    /// <param name="cancellationToken">A cancellation token to cancel this operation.</param>
    /// <returns>A task that represents the asynchronous write operation.</returns>
    ValueTask WriteAsync(IBluetoothDevice device,
        TWrite value,
        bool skipIfOldValueMatchesNewValue = false,
        TimeSpan? timeout = null,
        CancellationToken cancellationToken = default);
}
