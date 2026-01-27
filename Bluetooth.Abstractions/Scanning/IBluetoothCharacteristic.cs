
using System.ComponentModel;

using Bluetooth.Abstractions.AccessService;
using Bluetooth.Abstractions.Broadcasting.EventArgs;
using Bluetooth.Abstractions.Scanning.EventArgs;

namespace Bluetooth.Abstractions.Scanning;


/// <summary>
/// Interface representing a Bluetooth characteristic, providing properties and methods for interacting with it.
/// </summary>
public interface IBluetoothCharacteristic : INotifyPropertyChanged, IAsyncDisposable
{
    /// <summary>
    /// Gets the access service associated with this characteristic.
    /// </summary>
    IBluetoothCharacteristicAccessService AccessService { get; }

    /// <summary>
    /// Gets the Bluetooth service associated with this characteristic.
    /// </summary>
    IBluetoothService Service { get; }

    /// <summary>
    /// Gets the unique identifier of the characteristic.
    /// </summary>
    Guid Id { get; }

    /// <summary>
    /// Gets the name of the characteristic.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Gets the value of the characteristic as a read-only span. Useful for high-performance scenarios.
    /// </summary>
    ReadOnlySpan<byte> ValueSpan { get; }

    /// <summary>
    /// Gets the value of the characteristic as a read-only memory segment. Useful for asynchronous operations.
    /// </summary>
    ReadOnlyMemory<byte> Value { get; }

    /// <summary>
    /// Event raised when the value of the characteristic is updated, only triggered when IsListening is true.
    /// </summary>
    event EventHandler<ValueUpdatedEventArgs> ValueUpdated;

    /// <summary>
    /// Gets a value indicating whether the characteristic can be read.
    /// </summary>
    bool CanRead { get; }

    /// <summary>
    /// Gets a value indicating whether the characteristic can be written to.
    /// </summary>
    bool CanWrite { get; }

    /// <summary>
    /// Gets a value indicating whether the characteristic supports notifications.
    /// </summary>
    bool CanListen { get; }

    /// <summary>
    /// Gets a value indicating whether the characteristic is currently listening for notifications.
    /// </summary>
    bool IsListening { get; }

    /// <summary>
    /// Gets a value indicating whether the characteristic is currently reading its value.
    /// </summary>
    bool IsReadingValue { get; }

    /// <summary>
    /// Gets a value indicating whether the characteristic is currently writing its value.
    /// </summary>
    bool IsWritingValue { get; }

    /// <summary>
    /// Reads the value of the characteristic asynchronously.
    /// </summary>
    /// <param name="skipIfPreviouslyRead">If true, skips reading if the value was previously read.</param>
    /// <param name="timeout">The timeout for this operation</param>
    /// <param name="cancellationToken">A cancellation token to cancel this operation.</param>
    /// <returns>A task that represents the asynchronous read operation. The task result contains the value read.</returns>
    ValueTask<ReadOnlyMemory<byte>> ReadValueAsync(bool skipIfPreviouslyRead = false, TimeSpan? timeout = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Writes a value to the characteristic asynchronously.
    /// </summary>
    /// <param name="value">The value to write.</param>
    /// <param name="skipIfOldValueMatchesNewValue">If true, skips writing if the old value matches the new value.</param>
    /// <param name="timeout">The timeout for this operation</param>
    /// <param name="cancellationToken">A cancellation token to cancel this operation.</param>
    /// <returns>A task that represents the asynchronous write operation.</returns>
    Task WriteValueAsync(ReadOnlyMemory<byte> value, bool skipIfOldValueMatchesNewValue = false, TimeSpan? timeout = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Reads the listening status of the characteristic asynchronously.
    /// </summary>
    /// <param name="timeout">The timeout for this operation</param>
    /// <param name="cancellationToken">A cancellation token to cancel this operation.</param>
    /// <returns>A task that represents the asynchronous read operation. The task result contains the listening status.</returns>
    ValueTask<bool> ReadIsListeningAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Writes the listening status of the characteristic asynchronously.
    /// </summary>
    /// <param name="shouldBeListening">The desired listening status.</param>
    /// <param name="timeout">The timeout for this operation</param>
    /// <param name="cancellationToken">A cancellation token to cancel this operation.</param>
    /// <returns>A task that represents the asynchronous write operation.</returns>
    ValueTask WriteIsListeningAsync(bool shouldBeListening,TimeSpan? timeout = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Starts listening for notifications from the characteristic asynchronously.
    /// </summary>
    /// <param name="timeout">The timeout for this operation</param>
    /// <param name="cancellationToken">A cancellation token to cancel this operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask StartListeningAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Stops listening for notifications from the characteristic asynchronously.
    /// </summary>
    /// <param name="timeout">The timeout for this operation</param>
    /// <param name="cancellationToken">A cancellation token to cancel this operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    ValueTask StopListeningAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Waits for the value of the characteristic to change asynchronously.
    /// </summary>
    /// <param name="valueFilter">An optional filter function to apply to the value changes. If provided, the task completes only when the filter returns true.</param>
    /// <param name="timeout">The timeout for this operation</param>
    /// <param name="cancellationToken">A cancellation token to cancel this operation.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the new value of the characteristic.</returns>
    ValueTask<ReadOnlyMemory<byte>> WaitForValueChangeAsync(Func<ReadOnlyMemory<byte>, bool>? valueFilter = null, TimeSpan? timeout = null, CancellationToken cancellationToken = default);
}

