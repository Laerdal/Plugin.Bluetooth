namespace Bluetooth.Core.Scanning;

/// <summary>
///     Abstract base class for platform-specific L2CAP channel implementations.
///     Provides common functionality for opening, closing, reading, and writing to L2CAP channels.
/// </summary>
public abstract partial class BaseBluetoothRemoteL2CapChannel : BaseBindableObject, IBluetoothL2CapChannel, IAsyncDisposable
{
    /// <summary>
    ///     The logger instance used for LoggerMessage source generation.
    /// </summary>
    private readonly ILogger _logger;

    /// <summary>
    ///     Gets the options for configuring L2CAP channel timeouts.
    /// </summary>
    protected L2CapChannelOptions Options { get; }

    /// <summary>
    ///     Gets the logger for this channel, if any.
    /// </summary>
    protected new ILogger? Logger { get; }

    /// <summary>
    ///     Initializes a new instance of the <see cref="BaseBluetoothRemoteL2CapChannel"/> class.
    /// </summary>
    /// <param name="device">The Bluetooth device this channel belongs to.</param>
    /// <param name="psm">The Protocol/Service Multiplexer (PSM) for this channel.</param>
    /// <param name="options">Optional configuration options for L2CAP channel timeouts. If null, default options will be used.</param>
    /// <param name="logger">Optional logger for logging channel operations.</param>
    /// <exception cref="ArgumentNullException">Thrown when device is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when PSM is not positive.</exception>
    protected BaseBluetoothRemoteL2CapChannel(
        IBluetoothRemoteDevice device,
        int psm,
        IOptions<L2CapChannelOptions>? options = null,
        ILogger? logger = null)
    {
        ArgumentNullException.ThrowIfNull(device);
        if (psm <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(psm), psm, "PSM must be positive");
        }

        _logger = logger ?? NullLogger.Instance;
        Options = options?.Value ?? new L2CapChannelOptions();
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
        timeout ??= Options.OpenTimeout;

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
        timeout ??= Options.CloseTimeout;

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
        timeout ??= Options.ReadTimeout;

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
        timeout ??= Options.WriteTimeout;

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
    protected async virtual ValueTask DisposeAsyncCore()
    {
        if (IsOpen)
        {
            try
            {
                await CloseAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                LogL2CapChannelDisposalError(Psm, Device.Id, ex);
            }
        }

        _operationSemaphore?.Dispose();
    }
}
