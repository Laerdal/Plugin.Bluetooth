using Bluetooth.Maui.Platforms.Win.Exceptions;
using Bluetooth.Maui.Platforms.Win.Logging;
using Bluetooth.Maui.Platforms.Win.Scanning.Factories;
using Bluetooth.Maui.Platforms.Win.Scanning.NativeObjects;

using Windows.Storage.Streams;

namespace Bluetooth.Maui.Platforms.Win.Scanning;

/// <summary>
///     Represents a Windows-specific Bluetooth Low Energy characteristic.
///     This class wraps Windows's GattCharacteristic, providing platform-specific
///     implementations for reading, writing, and listening to characteristic values.
/// </summary>
/// <remarks>
///     <seealso href="https://learn.microsoft.com/en-us/uwp/api/windows.devices.bluetooth.genericattributeprofile.gattcharacteristic">GattCharacteristic</seealso>
///     <seealso href="https://learn.microsoft.com/en-us/uwp/api/windows.devices.bluetooth.genericattributeprofile.gattreadresult">GattReadResult</seealso>
///     <seealso href="https://learn.microsoft.com/en-us/uwp/api/windows.devices.bluetooth.genericattributeprofile.gattwriteresult">GattWriteResult</seealso>
/// </remarks>
public class WindowsBluetoothRemoteCharacteristic : BaseBluetoothRemoteCharacteristic,
    GattCharacteristicProxy.IBluetoothCharacteristicProxyDelegate
{
    /// <summary>
    ///     Initializes a new instance of the Windows <see cref="WindowsBluetoothRemoteCharacteristic" /> class.
    /// </summary>
    /// <param name="service">The Bluetooth service that contains this characteristic.</param>
    /// <param name="request">The characteristic factory request containing characteristic information.</param>
    /// <param name="descriptorFactory">The factory for creating Bluetooth descriptors.</param>
    public WindowsBluetoothRemoteCharacteristic(
        IBluetoothRemoteService service,
        IBluetoothCharacteristicFactory.BluetoothCharacteristicFactoryRequest request,
        IBluetoothDescriptorFactory descriptorFactory)
        : base(service, request, descriptorFactory)
    {
        ArgumentNullException.ThrowIfNull(request);
        if (request is not WindowsBluetoothCharacteristicFactoryRequest windowsRequest)
        {
            throw new ArgumentException(
                $"Expected request of type {typeof(WindowsBluetoothCharacteristicFactoryRequest)}, but got {request.GetType()}");
        }

        NativeCharacteristicProxy = new NativeObjects.GattCharacteristicProxy(windowsRequest.NativeCharacteristic, this);
    }

    /// <summary>
    ///     Gets the Windows GATT characteristic proxy used for characteristic operations.
    /// </summary>
    public NativeObjects.GattCharacteristicProxy NativeCharacteristicProxy { get; }

    #region Delegate Callbacks

    /// <summary>
    ///     Called when the characteristic value changes on the Windows platform.
    /// </summary>
    /// <param name="value">The new characteristic value.</param>
    /// <param name="argsTimestamp">The timestamp when the value changed.</param>
    public void OnValueChanged(byte[] value, DateTimeOffset argsTimestamp)
    {
        Logger?.LogNotificationReceived(Id, RemoteService.Device.Id, value.Length);
        OnReadValueSucceeded(value);
    }

    #endregion

    #region Read

    /// <inheritdoc />
    protected async override ValueTask NativeReadValueAsync()
    {
        Logger?.LogCharacteristicRead(Id, RemoteService.Device.Id);

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
                using var reader = DataReader.FromBuffer(buffer);
                reader.ReadBytes(data);
                Logger?.LogCharacteristicReadCompleted(Id, RemoteService.Device.Id, data.Length);
                OnReadValueSucceeded(data);
            }
            else
            {
                Logger?.LogCharacteristicReadCompleted(Id, RemoteService.Device.Id, 0);
                OnReadValueSucceeded(Array.Empty<byte>());
            }
        }
        catch (Exception ex)
        {
            Logger?.LogCharacteristicReadError(Id, RemoteService.Device.Id, ex.Message, ex);
            OnReadValueFailed(ex);
        }
    }

    /// <inheritdoc />
    protected override bool NativeCanRead()
    {
        return NativeCharacteristicProxy.GattCharacteristic.CharacteristicProperties
            .HasFlag(GattCharacteristicProperties.Read);
    }

    #endregion

    #region Write

    /// <inheritdoc />
    protected async override ValueTask NativeWriteValueAsync(ReadOnlyMemory<byte> value)
    {
        Logger?.LogCharacteristicWrite(Id, RemoteService.Device.Id, value.Length);

        try
        {
            // Choose write option based on characteristic properties
            var properties = NativeCharacteristicProxy.GattCharacteristic.CharacteristicProperties;
            var supportsWriteWithResponse = properties.HasFlag(GattCharacteristicProperties.Write);
            var supportsWriteWithoutResponse = properties.HasFlag(GattCharacteristicProperties.WriteWithoutResponse);

            if (!supportsWriteWithResponse && !supportsWriteWithoutResponse)
            {
                throw new CharacteristicCantWriteException(
                    this,
                    "Characteristic doesn't support Write");
            }

            // Prefer write with response if available
            var writeOption = supportsWriteWithResponse
                ? GattWriteOption.WriteWithResponse
                : GattWriteOption.WriteWithoutResponse;

            // Create buffer
            IBuffer buffer;
            using (var writer = new DataWriter())
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

            Logger?.LogCharacteristicWriteCompleted(Id, RemoteService.Device.Id);
            OnWriteValueSucceeded();
        }
        catch (Exception ex)
        {
            Logger?.LogCharacteristicWriteError(Id, RemoteService.Device.Id, ex.Message, ex);
            OnWriteValueFailed(ex);
        }
    }

    /// <inheritdoc />
    protected override bool NativeCanWrite()
    {
        var properties = NativeCharacteristicProxy.GattCharacteristic.CharacteristicProperties;
        return properties.HasFlag(GattCharacteristicProperties.Write) ||
               properties.HasFlag(GattCharacteristicProperties.WriteWithoutResponse);
    }

    #endregion

    #region Listen (Notify/Indicate)

    /// <inheritdoc />
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

    /// <inheritdoc />
    protected async override ValueTask NativeWriteIsListeningAsync(bool shouldBeListening)
    {
        var action = shouldBeListening ? "Enabling" : "Disabling";
        Logger?.LogNotificationStateChange(action, Id, RemoteService.Device.Id);

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
                    throw new CharacteristicCantListenException(
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
            Logger?.LogNotificationStateChangeError(Id, RemoteService.Device.Id, ex.Message, ex);
            OnWriteIsListeningFailed(ex);
        }
    }

    /// <inheritdoc />
    protected override bool NativeCanListen()
    {
        var properties = NativeCharacteristicProxy.GattCharacteristic.CharacteristicProperties;
        return properties.HasFlag(GattCharacteristicProperties.Notify) ||
               properties.HasFlag(GattCharacteristicProperties.Indicate);
    }

    #endregion

    #region Reliable Write

    /// <inheritdoc />
    protected override ValueTask NativeBeginReliableWriteAsync()
    {
        // Windows doesn't support reliable write transactions
        throw new NotSupportedException("Reliable write transactions are not supported on Windows platform");
    }

    /// <inheritdoc />
    protected override ValueTask NativeExecuteReliableWriteAsync()
    {
        // Windows doesn't support reliable write transactions
        throw new NotSupportedException("Reliable write transactions are not supported on Windows platform");
    }

    /// <inheritdoc />
    protected override ValueTask NativeAbortReliableWriteAsync()
    {
        // Windows doesn't support reliable write transactions
        throw new NotSupportedException("Reliable write transactions are not supported on Windows platform");
    }

    #endregion

    #region Descriptor Discovery

    /// <inheritdoc />
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
        var descriptorRequest = new WindowsBluetoothDescriptorFactoryRequest(nativeDescriptor);
        return DescriptorFactory.CreateDescriptor(this, descriptorRequest);
    }

    private static bool AreRepresentingTheSameObject(GattDescriptor native, IBluetoothRemoteDescriptor shared)
    {
        return native.Uuid.Equals(shared.Id);
    }

    #endregion
}
