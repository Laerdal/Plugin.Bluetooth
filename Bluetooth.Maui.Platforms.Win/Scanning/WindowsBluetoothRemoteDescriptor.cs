using Bluetooth.Maui.Platforms.Win.Exceptions;
using Bluetooth.Maui.Platforms.Win.Logging;
using Bluetooth.Maui.Platforms.Win.Scanning.Factories;

using Windows.Storage.Streams;

namespace Bluetooth.Maui.Platforms.Win.Scanning;

/// <summary>
///     Represents a Windows-specific Bluetooth Low Energy descriptor.
///     This class wraps Windows's GattDescriptor, providing platform-specific
///     implementations for reading and writing descriptor values.
/// </summary>
/// <remarks>
///     <seealso href="https://learn.microsoft.com/en-us/uwp/api/windows.devices.bluetooth.genericattributeprofile.gattdescriptor">GattDescriptor</seealso>
///     <seealso href="https://learn.microsoft.com/en-us/uwp/api/windows.devices.bluetooth.genericattributeprofile.gattreadresult">GattReadResult</seealso>
///     <seealso href="https://learn.microsoft.com/en-us/uwp/api/windows.devices.bluetooth.genericattributeprofile.gattwriteresult">GattWriteResult</seealso>
/// </remarks>
public class WindowsBluetoothRemoteDescriptor : BaseBluetoothRemoteDescriptor
{
    /// <summary>
    ///     Initializes a new instance of the Windows <see cref="WindowsBluetoothRemoteDescriptor" /> class.
    /// </summary>
    /// <param name="characteristic">The Bluetooth characteristic that contains this descriptor.</param>
    /// <param name="spec">The descriptor factory spec containing descriptor information.</param>
    public WindowsBluetoothRemoteDescriptor(IBluetoothRemoteCharacteristic characteristic, IBluetoothRemoteDescriptorFactory.BluetoothRemoteDescriptorFactorySpec spec) : base(characteristic, spec)
    {
        ArgumentNullException.ThrowIfNull(spec);
        if (spec is not WindowsBluetoothRemoteDescriptorFactorySpec nativeSpec)
        {
            throw new ArgumentException($"Expected spec of type {typeof(WindowsBluetoothRemoteDescriptorFactorySpec)}, but got {spec.GetType()}");
        }

        NativeDescriptor = nativeSpec.NativeDescriptor;
    }

    /// <summary>
    ///     Gets the native Windows GATT descriptor.
    /// </summary>
    public GattDescriptor NativeDescriptor { get; }

    #region Read

    /// <inheritdoc />
    protected async override ValueTask NativeReadValueAsync()
    {
        Logger?.LogDescriptorRead(Id, RemoteCharacteristic.RemoteService.Device.Id);

        try
        {
            var result = await NativeDescriptor.ReadValueAsync(BluetoothCacheMode.Uncached).AsTask().ConfigureAwait(false);

            WindowsNativeGattCommunicationStatusException.ThrowIfNotSuccess(result.Status);

            if (result.Value is { Length: > 0 } buffer)
            {
                var data = new byte[buffer.Length];
                using var reader = DataReader.FromBuffer(buffer);
                reader.ReadBytes(data);
                Logger?.LogDescriptorReadCompleted(Id, RemoteCharacteristic.RemoteService.Device.Id, data.Length);
                OnReadValueSucceeded(data);
            }
            else
            {
                Logger?.LogDescriptorReadCompleted(Id, RemoteCharacteristic.RemoteService.Device.Id, 0);
                OnReadValueSucceeded(Array.Empty<byte>());
            }
        }
        catch (Exception ex)
        {
            Logger?.LogDescriptorReadError(Id, RemoteCharacteristic.RemoteService.Device.Id, ex.Message, ex);
            OnReadValueFailed(ex);
        }
    }

    /// <inheritdoc />
    protected override bool NativeCanRead()
    {
        // Windows GATT descriptors generally support read operations
        // There's no explicit property to check, so we assume read is supported
        return true;
    }

    #endregion

    #region Write

    /// <inheritdoc />
    protected async override ValueTask NativeWriteValueAsync(ReadOnlyMemory<byte> value)
    {
        Logger?.LogDescriptorWrite(Id, RemoteCharacteristic.RemoteService.Device.Id, value.Length);

        try
        {
            // Create buffer
            IBuffer buffer;

            using (var writer = new DataWriter())
            {
                writer.WriteBytes(value.ToArray());
                buffer = writer.DetachBuffer();
            }

            // Write with result check
            var result = await NativeDescriptor.WriteValueWithResultAsync(buffer).AsTask().ConfigureAwait(false);

            WindowsNativeGattCommunicationStatusException.ThrowIfNotSuccess(result.Status);

            Logger?.LogDescriptorWriteCompleted(Id, RemoteCharacteristic.RemoteService.Device.Id);
            OnWriteValueSucceeded();
        }
        catch (Exception ex)
        {
            Logger?.LogDescriptorWriteError(Id, RemoteCharacteristic.RemoteService.Device.Id, ex.Message, ex);
            OnWriteValueFailed(ex);
        }
    }

    /// <inheritdoc />
    protected override bool NativeCanWrite()
    {
        // Windows GATT descriptors generally support write operations
        // There's no explicit property to check, so we assume write is supported
        return true;
    }

    #endregion
}
