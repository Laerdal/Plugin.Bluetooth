namespace Bluetooth.Core.Scanning;

/// <summary>
///     Abstract base class for platform-specific L2CAP channel implementations.
///     Provides common functionality for opening, closing, reading, and writing to L2CAP channels.
/// </summary>
public abstract class BaseBluetoothL2CapChannel : BaseBindableObject, IBluetoothL2CapChannel, IAsyncDisposable
{
    /// <summary>
    ///     Gets the logger for this channel, if any.
    /// </summary>
    protected new ILogger? Logger { get; }

    /// <summary>
    ///     Initializes a new instance of the <see cref="BaseBluetoothL2CapChannel"/> class.
    /// </summary>
    /// <param name="device">The Bluetooth device this channel belongs to.</param>
    /// <param name="psm">The Protocol/Service Multiplexer (PSM) for this channel.</param>
    /// <param name="logger">Optional logger for logging channel operations.</param>
    /// <exception cref="ArgumentNullException">Thrown when device is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when PSM is not positive.</exception>
    protected BaseBluetoothL2CapChannel(
        IBluetoothRemoteDevice device,
        int psm,
        ILogger? logger = null)
    {
        ArgumentNullException.ThrowIfNull(device);
        if (psm <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(psm), psm, "PSM must be positive");
        }

        Device = device;
        Psm = psm;
        Logger = logger;
    }

    /// <inheritdoc />
    public IBluetoothRemoteDevice Device { get; }

    /// <inheritdoc />
    public int Psm { get; }

    /// <inheritdoc />
    public bool IsOpen
    {
        get => GetValue(false);
        protected set => SetValue(value);
    }

    /// <inheritdoc />
    public int Mtu
    {
        get => GetValue(0);
        protected set => SetValue(value);
    }

    /// <summary>
    ///     Semaphore for serializing channel operations to prevent concurrent access issues.
    /// </summary>
    private readonly SemaphoreSlim _operationSemaphore = new(1, 1);

    /// <inheritdoc />
    public async ValueTask OpenAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        timeout ??= TimeSpan.FromSeconds(30);

        await _operationSemaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            if (IsOpen)
            {
                throw new InvalidOperationException("Channel is already open");
            }

            await NativeOpenAsync().ConfigureAwait(false);
        }
        finally
        {
            _operationSemaphore.Release();
        }
    }

    /// <inheritdoc />
    public async ValueTask CloseAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        timeout ??= TimeSpan.FromSeconds(10);

        await _operationSemaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            if (!IsOpen)
            {
                return;
            }

            await NativeCloseAsync().ConfigureAwait(false);
        }
        finally
        {
            _operationSemaphore.Release();
        }
    }

    /// <inheritdoc />
    public async ValueTask<int> ReadAsync(
        Memory<byte> buffer,
        TimeSpan? timeout = null,
        CancellationToken cancellationToken = default)
    {
        timeout ??= TimeSpan.FromSeconds(10);

        if (!IsOpen)
        {
            throw new InvalidOperationException("Channel is not open");
        }

        return await NativeReadAsync(buffer).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask WriteAsync(
        ReadOnlyMemory<byte> data,
        TimeSpan? timeout = null,
        CancellationToken cancellationToken = default)
    {
        timeout ??= TimeSpan.FromSeconds(10);

        if (!IsOpen)
        {
            throw new InvalidOperationException("Channel is not open");
        }

        // Serialize writes to prevent concurrent access
        await _operationSemaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            await NativeWriteAsync(data).ConfigureAwait(false);
        }
        finally
        {
            _operationSemaphore.Release();
        }
    }

    /// <summary>
    ///     Platform-specific implementation for opening the L2CAP channel.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    protected abstract ValueTask NativeOpenAsync();

    /// <summary>
    ///     Platform-specific implementation for closing the L2CAP channel.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    protected abstract ValueTask NativeCloseAsync();

    /// <summary>
    ///     Platform-specific implementation for reading data from the L2CAP channel.
    /// </summary>
    /// <param name="buffer">The buffer to read data into.</param>
    /// <returns>A task representing the asynchronous operation, with the number of bytes read.</returns>
    protected abstract ValueTask<int> NativeReadAsync(Memory<byte> buffer);

    /// <summary>
    ///     Platform-specific implementation for writing data to the L2CAP channel.
    /// </summary>
    /// <param name="data">The data to write.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    protected abstract ValueTask NativeWriteAsync(ReadOnlyMemory<byte> data);

    /// <inheritdoc />
    public event EventHandler<L2CapDataReceivedEventArgs>? DataReceived;

    /// <summary>
    ///     Raises the <see cref="DataReceived"/> event when data is received on the channel.
    /// </summary>
    /// <param name="data">The data that was received.</param>
    protected void OnDataReceived(ReadOnlyMemory<byte> data)
    {
        DataReceived?.Invoke(this, new L2CapDataReceivedEventArgs(this, data));
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        await DisposeAsyncCore().ConfigureAwait(false);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    ///     Performs asynchronous cleanup of resources used by this channel.
    /// </summary>
    /// <returns>A task representing the asynchronous dispose operation.</returns>
    protected virtual async ValueTask DisposeAsyncCore()
    {
        if (IsOpen)
        {
            try
            {
                await CloseAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex, "Error closing L2CAP channel during disposal");
            }
        }

        _operationSemaphore?.Dispose();
    }
}
