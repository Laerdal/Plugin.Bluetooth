namespace Bluetooth.Abstractions.Scanning;

/// <summary>
/// Interface representing a Bluetooth characteristic, providing properties and methods for interacting with it.
/// </summary>
public partial interface IBluetoothRemoteCharacteristic
{

    /// <summary>
    /// Gets a value indicating whether the characteristic can be read.
    /// </summary>
    bool CanRead { get; }

    /// <summary>
    /// Gets a value indicating whether the characteristic is currently reading its value.
    /// </summary>
    bool IsReading { get; }

    /// <summary>
    /// Reads the value of the characteristic asynchronously.
    /// </summary>
    /// <param name="skipIfPreviouslyRead">If true, skips reading if the value was previously read.</param>
    /// <param name="timeout">The timeout for this operation</param>
    /// <param name="cancellationToken">A cancellation token to cancel this operation.</param>
    /// <returns>A task that represents the asynchronous read operation. The task result contains the value read.</returns>
    /// <exception cref="DeviceNotConnectedException">Thrown when the device is not connected.</exception>
    /// <exception cref="CharacteristicCantReadException">Thrown when the characteristic doesn't support read operations.</exception>
    /// <exception cref="CharacteristicReadException">Thrown when the read operation fails.</exception>
    /// <exception cref="TimeoutException">Thrown when the operation times out.</exception>
    /// <exception cref="OperationCanceledException">Thrown when the operation is cancelled.</exception>
    ValueTask<ReadOnlyMemory<byte>> ReadValueAsync(bool skipIfPreviouslyRead = false, TimeSpan? timeout = null, CancellationToken cancellationToken = default);

}
