using Bluetooth.Abstractions.Scanning;
using Bluetooth.Abstractions.Scanning.Factories;
using Bluetooth.Core.Scanning;
using Bluetooth.Maui.Platforms.Windows.Exceptions;
using Bluetooth.Maui.Platforms.Windows.Scanning.Factories;

namespace Bluetooth.Maui.Platforms.Windows.Scanning;

/// <summary>
/// Represents a Windows-specific Bluetooth Low Energy descriptor.
/// This class wraps Windows's GattDescriptor, providing platform-specific
/// implementations for reading and writing descriptor values.
/// </summary>
public class BluetoothDescriptor : BaseBluetoothRemoteDescriptor
{
    /// <summary>
    /// Gets the native Windows GATT descriptor.
    /// </summary>
    public GattDescriptor NativeDescriptor { get; }

    /// <summary>
    /// Initializes a new instance of the Windows <see cref="BluetoothDescriptor"/> class.
    /// </summary>
    /// <param name="characteristic">The Bluetooth characteristic that contains this descriptor.</param>
    /// <param name="request">The descriptor factory request containing descriptor information.</param>
    public BluetoothDescriptor(
        IBluetoothRemoteCharacteristic characteristic,
        IBluetoothDescriptorFactory.BluetoothDescriptorFactoryRequest request)
        : base(characteristic, request)
    {
        ArgumentNullException.ThrowIfNull(request);
        if (request is not BluetoothDescriptorFactoryRequest windowsRequest)
        {
            throw new ArgumentException(
                $"Expected request of type {typeof(BluetoothDescriptorFactoryRequest)}, but got {request.GetType()}");
        }

        NativeDescriptor = windowsRequest.NativeDescriptor;
    }

    #region Read

    /// <inheritdoc/>
    protected override async ValueTask NativeReadValueAsync()
    {
        try
        {
            var result = await NativeDescriptor
                .ReadValueAsync(BluetoothCacheMode.Uncached)
                .AsTask()
                .ConfigureAwait(false);

            WindowsNativeGattCommunicationStatusException.ThrowIfNotSuccess(result.Status);

            if (result.Value is { Length: > 0 } buffer)
            {
                var data = new byte[buffer.Length];
                using var reader = global::Windows.Storage.Streams.DataReader.FromBuffer(buffer);
                reader.ReadBytes(data);
                OnReadValueSucceeded(data);
            }
            else
            {
                OnReadValueSucceeded(Array.Empty<byte>());
            }
        }
        catch (Exception ex)
        {
            OnReadValueFailed(ex);
        }
    }

    /// <inheritdoc/>
    protected override bool NativeCanRead()
    {
        // Windows GATT descriptors generally support read operations
        // There's no explicit property to check, so we assume read is supported
        return true;
    }

    #endregion

    #region Write

    /// <inheritdoc/>
    protected override async ValueTask NativeWriteValueAsync(ReadOnlyMemory<byte> value)
    {
        try
        {
            // Create buffer
            var writer = new global::Windows.Storage.Streams.DataWriter();
            writer.WriteBytes(value.ToArray());
            var buffer = writer.DetachBuffer();

            // Write with result check
            var result = await NativeDescriptor
                .WriteValueWithResultAsync(buffer)
                .AsTask()
                .ConfigureAwait(false);

            WindowsNativeGattCommunicationStatusException.ThrowIfNotSuccess(result.Status);

            OnWriteValueSucceeded();
        }
        catch (Exception ex)
        {
            OnWriteValueFailed(ex);
        }
    }

    /// <inheritdoc/>
    protected override bool NativeCanWrite()
    {
        // Windows GATT descriptors generally support write operations
        // There's no explicit property to check, so we assume write is supported
        return true;
    }

    #endregion
}
