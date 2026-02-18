using Windows.Storage.Streams;

using Bluetooth.Abstractions.Scanning;
using Bluetooth.Abstractions.Scanning.Factories;
using Bluetooth.Core.Scanning;
using Bluetooth.Maui.Platforms.Windows.Exceptions;
using Bluetooth.Maui.Platforms.Windows.Scanning.Factories;
using Bluetooth.Maui.Platforms.Windows.Scanning.NativeObjects;

namespace Bluetooth.Maui.Platforms.Windows.Scanning;

/// <summary>
/// Represents a Windows-specific Bluetooth Low Energy characteristic.
/// This class wraps Windows's GattCharacteristic, providing platform-specific
/// implementations for reading, writing, and listening to characteristic values.
/// </summary>
public class BluetoothCharacteristic : BaseBluetoothRemoteCharacteristic,
    GattCharacteristicProxy.IBluetoothCharacteristicProxyDelegate
{
    /// <summary>
    /// Gets the Windows GATT characteristic proxy used for characteristic operations.
    /// </summary>
    public GattCharacteristicProxy NativeCharacteristicProxy { get; }

    /// <summary>
    /// Initializes a new instance of the Windows <see cref="BluetoothCharacteristic"/> class.
    /// </summary>
    /// <param name="service">The Bluetooth service that contains this characteristic.</param>
    /// <param name="request">The characteristic factory request containing characteristic information.</param>
    /// <param name="descriptorFactory">The factory for creating Bluetooth descriptors.</param>
    public BluetoothCharacteristic(
        IBluetoothRemoteService service,
        IBluetoothCharacteristicFactory.BluetoothCharacteristicFactoryRequest request,
        IBluetoothDescriptorFactory descriptorFactory)
        : base(service, request, descriptorFactory)
    {
        ArgumentNullException.ThrowIfNull(request);
        if (request is not BluetoothCharacteristicFactoryRequest windowsRequest)
        {
            throw new ArgumentException(
                $"Expected request of type {typeof(BluetoothCharacteristicFactoryRequest)}, but got {request.GetType()}");
        }

        NativeCharacteristicProxy = new GattCharacteristicProxy(windowsRequest.NativeCharacteristic, this);
    }

    #region Read

    /// <inheritdoc/>
    protected async override ValueTask NativeReadValueAsync()
    {
        try
        {
            var result = await NativeCharacteristicProxy.GattCharacteristic
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
        return NativeCharacteristicProxy.GattCharacteristic.CharacteristicProperties
            .HasFlag(GattCharacteristicProperties.Read);
    }

    #endregion

    #region Write

    /// <inheritdoc/>
    protected async override ValueTask NativeWriteValueAsync(ReadOnlyMemory<byte> value)
    {
        try
        {
            // Choose write option based on characteristic properties
            var properties = NativeCharacteristicProxy.GattCharacteristic.CharacteristicProperties;
            var supportsWriteWithResponse = properties.HasFlag(GattCharacteristicProperties.Write);
            var supportsWriteWithoutResponse = properties.HasFlag(GattCharacteristicProperties.WriteWithoutResponse);

            if (!supportsWriteWithResponse && !supportsWriteWithoutResponse)
            {
                throw new Abstractions.Scanning.Exceptions.CharacteristicCantWriteException(
                    this,
                    "Characteristic doesn't support Write");
            }

            // Prefer write with response if available
            var writeOption = supportsWriteWithResponse
                ? GattWriteOption.WriteWithResponse
                : GattWriteOption.WriteWithoutResponse;

            // Create buffer
            IBuffer buffer;
            using (var writer = new global::Windows.Storage.Streams.DataWriter())
            {
                writer.WriteBytes(value.ToArray());
                buffer = writer.DetachBuffer();
            }

            // Write with result check
            var result = await NativeCharacteristicProxy.GattCharacteristic
                .WriteValueWithResultAsync(buffer, writeOption)
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
        var properties = NativeCharacteristicProxy.GattCharacteristic.CharacteristicProperties;
        return properties.HasFlag(GattCharacteristicProperties.Write) ||
               properties.HasFlag(GattCharacteristicProperties.WriteWithoutResponse);
    }

    #endregion

    #region Listen (Notify/Indicate)

    /// <inheritdoc/>
    protected async override ValueTask NativeReadIsListeningAsync()
    {
        try
        {
            var result = await NativeCharacteristicProxy.GattCharacteristic
                .ReadClientCharacteristicConfigurationDescriptorAsync()
                .AsTask()
                .ConfigureAwait(false);

            WindowsNativeGattCommunicationStatusException.ThrowIfNotSuccess(result.Status);

            var isListening = result.ClientCharacteristicConfigurationDescriptor !=
                             GattClientCharacteristicConfigurationDescriptorValue.None;
            OnReadIsListeningSucceeded(isListening);
        }
        catch (Exception ex)
        {
            OnReadIsListeningFailed(ex);
        }
    }

    /// <inheritdoc/>
    protected async override ValueTask NativeWriteIsListeningAsync(bool shouldBeListening)
    {
        try
        {
            GattClientCharacteristicConfigurationDescriptorValue cccdValue;

            if (shouldBeListening)
            {
                // Check properties to choose between Notify and Indicate
                var properties = NativeCharacteristicProxy.GattCharacteristic.CharacteristicProperties;
                var supportsNotify = properties.HasFlag(GattCharacteristicProperties.Notify);
                var supportsIndicate = properties.HasFlag(GattCharacteristicProperties.Indicate);

                if (!supportsNotify && !supportsIndicate)
                {
                    throw new Abstractions.Scanning.Exceptions.CharacteristicCantListenException(
                        this,
                        "Characteristic doesn't support Notify or Indicate");
                }

                // Prefer Notify over Indicate
                cccdValue = supportsNotify
                    ? GattClientCharacteristicConfigurationDescriptorValue.Notify
                    : GattClientCharacteristicConfigurationDescriptorValue.Indicate;
            }
            else
            {
                cccdValue = GattClientCharacteristicConfigurationDescriptorValue.None;
            }

            // Write CCCD to enable/disable notifications
            var result = await NativeCharacteristicProxy.GattCharacteristic
                .WriteClientCharacteristicConfigurationDescriptorWithResultAsync(cccdValue)
                .AsTask()
                .ConfigureAwait(false);

            WindowsNativeGattCommunicationStatusException.ThrowIfNotSuccess(result.Status);

            OnWriteIsListeningSucceeded();
        }
        catch (Exception ex)
        {
            OnWriteIsListeningFailed(ex);
        }
    }

    /// <inheritdoc/>
    protected override bool NativeCanListen()
    {
        var properties = NativeCharacteristicProxy.GattCharacteristic.CharacteristicProperties;
        return properties.HasFlag(GattCharacteristicProperties.Notify) ||
               properties.HasFlag(GattCharacteristicProperties.Indicate);
    }

    #endregion

    #region Reliable Write

    /// <inheritdoc/>
    protected override ValueTask NativeBeginReliableWriteAsync()
    {
        // Windows doesn't support reliable write transactions
        throw new NotSupportedException("Reliable write transactions are not supported on Windows platform");
    }

    /// <inheritdoc/>
    protected override ValueTask NativeExecuteReliableWriteAsync()
    {
        // Windows doesn't support reliable write transactions
        throw new NotSupportedException("Reliable write transactions are not supported on Windows platform");
    }

    /// <inheritdoc/>
    protected override ValueTask NativeAbortReliableWriteAsync()
    {
        // Windows doesn't support reliable write transactions
        throw new NotSupportedException("Reliable write transactions are not supported on Windows platform");
    }

    #endregion

    #region Descriptor Discovery

    /// <inheritdoc/>
    protected async override ValueTask NativeDescriptorsExplorationAsync(
        TimeSpan? timeout = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await NativeCharacteristicProxy.GattCharacteristic
                .GetDescriptorsAsync(BluetoothCacheMode.Uncached)
                .AsTask(cancellationToken)
                .ConfigureAwait(false);

            WindowsNativeGattCommunicationStatusException.ThrowIfNotSuccess(result.Status);

            OnDescriptorsExplorationSucceeded(
                result.Descriptors.ToList(),
                ConvertNativeDescriptorToDescriptor,
                AreRepresentingTheSameObject);
        }
        catch (Exception ex)
        {
            OnDescriptorsExplorationFailed(ex);
        }
    }

    private IBluetoothRemoteDescriptor ConvertNativeDescriptorToDescriptor(GattDescriptor nativeDescriptor)
    {
        var descriptorRequest = new BluetoothDescriptorFactoryRequest(nativeDescriptor);
        return DescriptorFactory.CreateDescriptor(this, descriptorRequest);
    }

    private static bool AreRepresentingTheSameObject(GattDescriptor native, IBluetoothRemoteDescriptor shared)
    {
        return native.Uuid.Equals(shared.Id);
    }

    #endregion

    #region Delegate Callbacks

    /// <summary>
    /// Called when the characteristic value changes on the Windows platform.
    /// </summary>
    /// <param name="value">The new characteristic value.</param>
    /// <param name="argsTimestamp">The timestamp when the value changed.</param>
    public void OnValueChanged(byte[] value, DateTimeOffset argsTimestamp)
    {
        OnReadValueSucceeded(value);
    }

    #endregion
}
