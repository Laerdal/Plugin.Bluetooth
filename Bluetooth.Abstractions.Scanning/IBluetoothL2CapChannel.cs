namespace Bluetooth.Abstractions.Scanning;

/// <summary>
///     Interface representing a Bluetooth L2CAP (Logical Link Control and Adaptation Protocol) channel.
///     L2CAP channels provide connection-oriented data transfer with higher throughput than GATT characteristics.
/// </summary>
public interface IBluetoothL2CapChannel : INotifyPropertyChanged, IAsyncDisposable
{
    /// <summary>
    ///     Gets the device associated with this L2CAP channel.
    /// </summary>
    IBluetoothRemoteDevice Device { get; }

    /// <summary>
    ///     Gets the Protocol Service Multiplexer (PSM) value for this channel.
    /// </summary>
    int Psm { get; }

    /// <summary>
    ///     Gets a value indicating whether the channel is currently open.
    /// </summary>
    bool IsOpen { get; }

    /// <summary>
    ///     Gets the Maximum Transmission Unit (MTU) for this channel.
    /// </summary>
    int Mtu { get; }

    /// <summary>
    ///     Opens the L2CAP channel asynchronously.
    /// </summary>
    /// <param name="timeout">The timeout for this operation.</param>
    /// <param name="cancellationToken">A cancellation token to cancel this operation.</param>
    /// <returns>A task that represents the asynchronous open operation.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the channel is already open.</exception>
    /// <exception cref="TimeoutException">Thrown when the operation times out.</exception>
    /// <exception cref="OperationCanceledException">Thrown when the operation is cancelled.</exception>
    ValueTask OpenAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Closes the L2CAP channel asynchronously.
    /// </summary>
    /// <param name="timeout">The timeout for this operation.</param>
    /// <param name="cancellationToken">A cancellation token to cancel this operation.</param>
    /// <returns>A task that represents the asynchronous close operation.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the channel is not open.</exception>
    /// <exception cref="TimeoutException">Thrown when the operation times out.</exception>
    /// <exception cref="OperationCanceledException">Thrown when the operation is cancelled.</exception>
    ValueTask CloseAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Reads data from the L2CAP channel asynchronously.
    /// </summary>
    /// <param name="buffer">The buffer to read data into.</param>
    /// <param name="timeout">The timeout for this operation.</param>
    /// <param name="cancellationToken">A cancellation token to cancel this operation.</param>
    /// <returns>A task that represents the asynchronous read operation. The task result contains the number of bytes read.</returns>
    ValueTask<int> ReadAsync(Memory<byte> buffer, TimeSpan? timeout = null, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Writes data to the L2CAP channel asynchronously.
    /// </summary>
    /// <param name="data">The data to write.</param>
    /// <param name="timeout">The timeout for this operation.</param>
    /// <param name="cancellationToken">A cancellation token to cancel this operation.</param>
    /// <returns>A task that represents the asynchronous write operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="data" /> is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the channel is not open.</exception>
    /// <exception cref="TimeoutException">Thrown when the operation times out.</exception>
    /// <exception cref="OperationCanceledException">Thrown when the operation is cancelled.</exception>
    ValueTask WriteAsync(ReadOnlyMemory<byte> data, TimeSpan? timeout = null, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Event raised when data is received on the channel.
    /// </summary>
    event EventHandler<L2CapDataReceivedEventArgs>? DataReceived;
}