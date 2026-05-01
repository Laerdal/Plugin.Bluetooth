using Bluetooth.Linux.Scanning.Factories;

namespace Bluetooth.Linux.Scanning;

/// <summary>
///     Linux implementation of a remote GATT descriptor backed by a BlueZ D-Bus <see cref="IGattDescriptor1"/> proxy.
/// </summary>
public class LinuxBluetoothRemoteDescriptor : BaseBluetoothRemoteDescriptor
{
    private readonly IGattDescriptor1 _nativeDescriptor;
    private readonly string[] _flags;

    /// <inheritdoc />
    public LinuxBluetoothRemoteDescriptor(
        IBluetoothRemoteCharacteristic parentCharacteristic,
        IBluetoothRemoteDescriptorFactory.BluetoothRemoteDescriptorFactorySpec spec,
        ILogger<IBluetoothRemoteDescriptor>? logger = null)
        : base(parentCharacteristic, spec, logger)
    {
        if (spec is LinuxBluetoothRemoteDescriptorFactorySpec linuxSpec)
        {
            _nativeDescriptor = linuxSpec.NativeDescriptor;
            _flags = linuxSpec.Flags;
        }
        else
        {
            throw new ArgumentException($"Expected {nameof(LinuxBluetoothRemoteDescriptorFactorySpec)}", nameof(spec));
        }
    }

    #region Capabilities

    /// <inheritdoc />
    protected override bool NativeCanRead() =>
        _flags.Contains("read", StringComparer.OrdinalIgnoreCase)
        || _flags.Contains("encrypt-read", StringComparer.OrdinalIgnoreCase)
        || _flags.Contains("encrypt-authenticated-read", StringComparer.OrdinalIgnoreCase)
        || _flags.Contains("secure-read", StringComparer.OrdinalIgnoreCase);

    /// <inheritdoc />
    protected override bool NativeCanWrite() =>
        _flags.Contains("write", StringComparer.OrdinalIgnoreCase)
        || _flags.Contains("encrypt-write", StringComparer.OrdinalIgnoreCase)
        || _flags.Contains("encrypt-authenticated-write", StringComparer.OrdinalIgnoreCase)
        || _flags.Contains("secure-write", StringComparer.OrdinalIgnoreCase);

    #endregion

    #region Read

    /// <inheritdoc />
    protected override async ValueTask NativeReadValueAsync()
    {
        try
        {
            var options = new Dictionary<string, object>();
            var bytes = await _nativeDescriptor.ReadValueAsync(options).ConfigureAwait(false);
            OnReadValueSucceeded(bytes);
        }
        catch (Exception ex)
        {
            OnReadValueFailed(ex);
        }
    }

    #endregion

    #region Write

    /// <inheritdoc />
    protected override async ValueTask NativeWriteValueAsync(ReadOnlyMemory<byte> value)
    {
        try
        {
            var options = new Dictionary<string, object>();
            await _nativeDescriptor.WriteValueAsync(value.ToArray(), options).ConfigureAwait(false);
            OnWriteValueSucceeded();
        }
        catch (Exception ex)
        {
            OnWriteValueFailed(ex);
        }
    }

    #endregion
}
