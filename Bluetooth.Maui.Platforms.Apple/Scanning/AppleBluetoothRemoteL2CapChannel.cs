using Bluetooth.Core.Scanning;
using Bluetooth.Maui.Platforms.Apple.Exceptions;
using Bluetooth.Maui.Platforms.Apple.Logging;

namespace Bluetooth.Maui.Platforms.Apple.Scanning;

/// <summary>
///     Apple (iOS/macOS) implementation of <see cref="IBluetoothL2CapChannel"/> using CBL2CAPChannel.
///     Provides stream-based I/O for L2CAP channels with NSStreamDelegate for push-based DataReceived events.
/// </summary>
/// <remarks>
///     Requires iOS 11+ / macOS 10.13+ for L2CAP channel support.
///     Uses NSStreamDelegate for event-driven data reading.
/// </remarks>
public class AppleBluetoothRemoteL2CapChannel : BaseBluetoothRemoteL2CapChannel, IAsyncDisposable
{
    private readonly CBL2CapChannel _nativeChannel;
    private NSInputStream? _inputStream;
    private NSOutputStream? _outputStream;
    private StreamDelegate? _streamDelegate;

    /// <summary>
    ///     Initializes a new instance of the <see cref="AppleBluetoothRemoteL2CapChannel"/> class.
    /// </summary>
    /// <param name="device">The Bluetooth device this channel belongs to.</param>
    /// <param name="nativeChannel">The native CBL2CAPChannel from CoreBluetooth.</param>
    /// <param name="logger">Optional logger for logging channel operations.</param>
    public AppleBluetoothRemoteL2CapChannel(
        IBluetoothRemoteDevice device,
        CBL2CapChannel nativeChannel,
        ILogger? logger = null)
        : base(device, (int)nativeChannel.Psm, logger)
    {
        ArgumentNullException.ThrowIfNull(nativeChannel);
        _nativeChannel = nativeChannel;

        // Channel is already open when created by CoreBluetooth
        // Use default L2CAP MTU - iOS L2CAP channels typically support larger MTUs (672 is minimum guaranteed)
        Mtu = 672;
        IsOpen = true;
    }

    /// <inheritdoc />
    protected override ValueTask NativeOpenAsync()
    {
        Logger?.LogL2CapChannelOpening(Psm, Device.Id);

        try
        {
            // Set up streams
            _inputStream = _nativeChannel.InputStream
                ?? throw new IOException("Input stream is null");
            _outputStream = _nativeChannel.OutputStream
                ?? throw new IOException("Output stream is null");

            // Set up stream delegate for event-based data reading
            _streamDelegate = new StreamDelegate(this);
            _inputStream.Delegate = _streamDelegate;
            _outputStream.Delegate = _streamDelegate;

            // Schedule streams on run loop
            _inputStream.Schedule(NSRunLoop.Current, NSRunLoopMode.Default);
            _outputStream.Schedule(NSRunLoop.Current, NSRunLoopMode.Default);

            // Open streams
            _inputStream.Open();
            _outputStream.Open();

            Logger?.LogL2CapChannelOpened(Psm, Mtu);
        }
        catch (Exception ex)
        {
            Logger?.LogL2CapChannelOpenFailed(Psm, ex);
            throw;
        }

        return ValueTask.CompletedTask;
    }

    /// <inheritdoc />
    protected override async ValueTask<int> NativeReadAsync(Memory<byte> buffer)
    {
        if (_inputStream == null || !IsOpen)
        {
            throw new InvalidOperationException("Channel not open");
        }

        Logger?.LogL2CapChannelReading(Psm, buffer.Length);

        // NSInputStream is synchronous, wrap in Task.Run
        return await Task.Run(() =>
        {
            // Create temp buffer for NSInputStream.Read (expects byte[])
            var tempBuffer = new byte[buffer.Length];
            var bytesRead = (int)_inputStream.Read(tempBuffer, 0, (nuint)tempBuffer.Length);

            if (bytesRead > 0)
            {
                tempBuffer.AsMemory(0, bytesRead).CopyTo(buffer);
            }

            Logger?.LogL2CapChannelRead(Psm, bytesRead);
            return bytesRead;
        }).ConfigureAwait(false);
    }

    /// <inheritdoc />
    protected override async ValueTask NativeWriteAsync(ReadOnlyMemory<byte> data)
    {
        if (_outputStream == null || !IsOpen)
        {
            throw new InvalidOperationException("Channel not open");
        }

        Logger?.LogL2CapChannelWriting(Psm, data.Length);

        // NSOutputStream is synchronous, wrap in Task.Run
        await Task.Run(() =>
        {
            // Convert ReadOnlyMemory to byte array for NSOutputStream.Write
            var buffer = data.ToArray();
            var bytesWritten = _outputStream.Write(buffer, 0, (nuint)buffer.Length);
            if (bytesWritten < 0)
            {
                throw new IOException("Failed to write to L2CAP channel");
            }

            Logger?.LogL2CapChannelWritten(Psm);
        }).ConfigureAwait(false);
    }

    /// <summary>
    ///     NSStreamDelegate for handling stream events (data available, errors, end of stream).
    /// </summary>
    private sealed class StreamDelegate : NSStreamDelegate
    {
        private readonly AppleBluetoothRemoteL2CapChannel _channel;

        public StreamDelegate(AppleBluetoothRemoteL2CapChannel channel)
        {
            _channel = channel;
        }

        public override void HandleEvent(NSStream stream, NSStreamEvent eventCode)
        {
            try
            {
                if (eventCode == NSStreamEvent.HasBytesAvailable && stream == _channel._inputStream)
                {
                    // Data available - read and raise event
                    var buffer = new byte[_channel.Mtu];
                    var bytesRead = (int)_channel._inputStream!.Read(buffer, 0, (nuint)buffer.Length);
                    if (bytesRead > 0)
                    {
                        _channel.Logger?.LogL2CapDataReceived(_channel.Psm, bytesRead);
                        _channel.OnDataReceived(buffer.AsMemory(0, bytesRead));
                    }
                }
                else if (eventCode == NSStreamEvent.ErrorOccurred)
                {
                    // Stream error occurred - log it
                    _channel.Logger?.LogL2CapStreamError(
                        _channel.Psm,
                        new IOException("Stream error occurred"));
                }
                else if (eventCode == NSStreamEvent.EndEncountered)
                {
                    // Stream ended, channel closed remotely
                    _channel.IsOpen = false;
                }
            }
            catch (Exception ex)
            {
                _channel.Logger?.LogL2CapStreamError(_channel.Psm, ex);
            }
        }
    }

    /// <inheritdoc />
    protected override ValueTask NativeCloseAsync()
    {
        Logger?.LogL2CapChannelClosing(Psm);

        try
        {
            _inputStream?.Close();
            _outputStream?.Close();
            _inputStream?.Unschedule(NSRunLoop.Current, NSRunLoopMode.Default);
            _outputStream?.Unschedule(NSRunLoop.Current, NSRunLoopMode.Default);
            _inputStream?.Dispose();
            _outputStream?.Dispose();
            _streamDelegate?.Dispose();
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

        return ValueTask.CompletedTask;
    }

    /// <inheritdoc />
    public new async ValueTask DisposeAsync()
    {
        if (IsOpen)
        {
            await NativeCloseAsync().ConfigureAwait(false);
        }

        // Call base disposal
        await base.DisposeAsync().ConfigureAwait(false);
    }
}
