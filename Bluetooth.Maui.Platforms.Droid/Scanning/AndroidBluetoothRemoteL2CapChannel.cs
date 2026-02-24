using Android.Bluetooth;
using Bluetooth.Core.Scanning;
using Bluetooth.Maui.Platforms.Droid.Exceptions;
using Bluetooth.Maui.Platforms.Droid.Logging;

namespace Bluetooth.Maui.Platforms.Droid.Scanning;

/// <summary>
///     Android implementation of <see cref="IBluetoothL2CapChannel"/> using BluetoothSocket.
///     Provides stream-based I/O for L2CAP channels with automatic background reading for DataReceived events.
/// </summary>
/// <remarks>
///     Requires Android API 29+ (Android 10+) for L2CAP channel support.
///     Uses a background read loop to enable push-based DataReceived events.
/// </remarks>
public class AndroidBluetoothRemoteL2CapChannel : BaseBluetoothRemoteL2CapChannel, IAsyncDisposable
{
    private readonly BluetoothDevice _nativeDevice;
    private BluetoothSocket? _socket;
    private Stream? _inputStream;
    private Stream? _outputStream;
    private CancellationTokenSource? _readLoopCts;

    /// <summary>
    ///     Initializes a new instance of the <see cref="AndroidBluetoothRemoteL2CapChannel"/> class.
    /// </summary>
    /// <param name="device">The Bluetooth device this channel belongs to.</param>
    /// <param name="nativeDevice">The native Android BluetoothDevice.</param>
    /// <param name="psm">The Protocol/Service Multiplexer (PSM) for this channel.</param>
    /// <param name="options">Optional configuration options for L2CAP channel timeouts.</param>
    /// <param name="logger">Optional logger for logging channel operations.</param>
    public AndroidBluetoothRemoteL2CapChannel(
        IBluetoothRemoteDevice device,
        BluetoothDevice nativeDevice,
        int psm,
        IOptions<L2CapChannelOptions>? options = null,
        ILogger? logger = null)
        : base(device, psm, options, logger)
    {
        ArgumentNullException.ThrowIfNull(nativeDevice);
        _nativeDevice = nativeDevice;
    }

    /// <inheritdoc />
    protected async override ValueTask NativeOpenAsync()
    {
        // API 29+ required for L2CAP channels
        if (!OperatingSystem.IsAndroidVersionAtLeast(29))
        {
            throw new PlatformNotSupportedException(
                "L2CAP channels require Android 10+ (API 29). Current version does not support this feature.");
        }

        Logger?.LogL2CapChannelOpening(Psm, Device.Id);

        try
        {
            // Create L2CAP socket
            _socket = _nativeDevice.CreateL2capChannel(Psm)
                ?? throw new AndroidNativeBluetoothException("Failed to create L2CAP channel");

            // Connect to the socket (blocking operation, wrap in Task.Run)
            await Task.Run(() => _socket.Connect(), CancellationToken.None).ConfigureAwait(false);

            // Get streams - Android streams are already .NET Stream objects
            _inputStream = _socket.InputStream
                ?? throw new AndroidNativeBluetoothException("Input stream is null");
            _outputStream = _socket.OutputStream
                ?? throw new AndroidNativeBluetoothException("Output stream is null");

            // Get MTU from socket
            // Note: MaxTransmitPacketSize available API 33+, fallback to configured default MTU
            Mtu = OperatingSystem.IsAndroidVersionAtLeast(33)
                ? _socket.MaxTransmitPacketSize
                : Options.DefaultMtu;

            IsOpen = true;
            Logger?.LogL2CapChannelOpened(Psm, Mtu);

            // Start background read loop for DataReceived events (if enabled)
            if (Options.EnableBackgroundReading)
            {
                _readLoopCts = new CancellationTokenSource();
                _ = Task.Run(() => ReadLoopAsync(_readLoopCts.Token), _readLoopCts.Token);
            }
        }
        catch (Exception ex)
        {
            Logger?.LogL2CapChannelOpenFailed(Psm, ex);
            throw;
        }
    }

    /// <inheritdoc />
    protected async override ValueTask<int> NativeReadAsync(Memory<byte> buffer)
    {
        if (_inputStream == null || !IsOpen)
        {
            throw new InvalidOperationException("Channel not open");
        }

        Logger?.LogL2CapChannelReading(Psm, buffer.Length);

        var bytesRead = await _inputStream.ReadAsync(buffer, CancellationToken.None).ConfigureAwait(false);

        Logger?.LogL2CapChannelRead(Psm, bytesRead);
        return bytesRead;
    }

    /// <inheritdoc />
    protected async override ValueTask NativeWriteAsync(ReadOnlyMemory<byte> data)
    {
        if (_outputStream == null || !IsOpen)
        {
            throw new InvalidOperationException("Channel not open");
        }

        Logger?.LogL2CapChannelWriting(Psm, data.Length);

        await _outputStream.WriteAsync(data, CancellationToken.None).ConfigureAwait(false);

        // Conditionally flush based on options
        if (Options.AutoFlushWrites)
        {
            await _outputStream.FlushAsync(CancellationToken.None).ConfigureAwait(false);
        }

        Logger?.LogL2CapChannelWritten(Psm);
    }

    /// <summary>
    ///     Background read loop that continuously reads from the input stream and raises DataReceived events.
    /// </summary>
    /// <param name="ct">Cancellation token to stop the read loop.</param>
    private async Task ReadLoopAsync(CancellationToken ct)
    {
        var bufferSize = Options.ReadBufferSize ?? Mtu;
        var buffer = new byte[bufferSize];
        try
        {
            while (!ct.IsCancellationRequested && IsOpen)
            {
                var bytesRead = await _inputStream!.ReadAsync(buffer, ct).ConfigureAwait(false);
                if (bytesRead > 0)
                {
                    Logger?.LogL2CapDataReceived(Psm, bytesRead);
                    OnDataReceived(buffer.AsMemory(0, bytesRead));
                }
                else if (bytesRead == 0)
                {
                    // End of stream, channel closed remotely
                    IsOpen = false;
                    break;
                }
            }
        }
        catch (System.OperationCanceledException)
        {
            // Expected during close
        }
        catch (Exception ex)
        {
            Logger?.LogL2CapReadLoopError(Psm, ex);
        }
    }

    /// <inheritdoc />
    protected async override ValueTask NativeCloseAsync()
    {
        Logger?.LogL2CapChannelClosing(Psm);

        if (_readLoopCts != null && _readLoopCts.IsCancellationRequested == false)
        {
            await _readLoopCts.CancelAsync().ConfigureAwait(false);
        }

        try
        {
            if (_inputStream != null)
            {
                await _inputStream.DisposeAsync().ConfigureAwait(false);
            }
            if(_outputStream != null)
            {
                await _outputStream.DisposeAsync().ConfigureAwait(false);
            }
            _socket?.Close();
            _socket?.Dispose();
        }
        catch (Exception ex)
        {
            Logger?.LogL2CapChannelCloseError(Psm, ex);
        }
        finally
        {
            IsOpen = false;
            Logger?.LogL2CapChannelClosed(Psm);
        }
    }

    /// <inheritdoc />
    public new async ValueTask DisposeAsync()
    {
        if (IsOpen)
        {
            await NativeCloseAsync().ConfigureAwait(false);
        }

        _readLoopCts?.Dispose();

        // Call base disposal
        await base.DisposeAsync().ConfigureAwait(false);
    }
}
