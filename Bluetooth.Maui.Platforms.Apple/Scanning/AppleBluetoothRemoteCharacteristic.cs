using Bluetooth.Maui.Platforms.Apple.Scanning.Factories;
using Bluetooth.Maui.Platforms.Apple.Scanning.NativeObjects;

using DescriptorNotFoundException = Bluetooth.Abstractions.Scanning.Exceptions.DescriptorNotFoundException;
using MultipleDescriptorsFoundException = Bluetooth.Abstractions.Scanning.Exceptions.MultipleDescriptorsFoundException;

namespace Bluetooth.Maui.Platforms.Apple.Scanning;

/// <inheritdoc cref="BaseBluetoothRemoteCharacteristic" />
public class AppleBluetoothRemoteCharacteristic : BaseBluetoothRemoteCharacteristic, CbPeripheralWrapper.ICbCharacteristicDelegate
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="AppleBluetoothRemoteCharacteristic" /> class with the specified service, factory request, and descriptor factory.
    /// </summary>
    /// <param name="remoteService">The Bluetooth service to which this characteristic belongs.</param>
    /// <param name="request">The factory request containing the information needed to create this characteristic.</param>
    /// <param name="descriptorFactory">The factory used to create descriptors for this characteristic.</param>
    public AppleBluetoothRemoteCharacteristic(IBluetoothRemoteService remoteService, IBluetoothCharacteristicFactory.BluetoothCharacteristicFactoryRequest request, IBluetoothDescriptorFactory descriptorFactory) :
        base(remoteService, request, descriptorFactory)
    {
        ArgumentNullException.ThrowIfNull(request);
        if (request is not AppleBluetoothCharacteristicFactoryRequest appleRequest)
        {
            throw new ArgumentException($"Expected request of type {typeof(AppleBluetoothCharacteristicFactoryRequest)}, but got {request.GetType()}");
        }

        CbCharacteristic = appleRequest.CbCharacteristic;
    }

    /// <summary>
    ///     Gets the native iOS Core Bluetooth characteristic.
    /// </summary>
    public CBCharacteristic CbCharacteristic { get; }

    /// <summary>
    ///     Gets the Bluetooth service to which this characteristic belongs, cast to the Apple-specific implementation.
    /// </summary>
    public AppleBluetoothRemoteService AppleBluetoothRemoteService => (AppleBluetoothRemoteService) RemoteService;

    #region Read

    /// <inheritdoc />
    protected override ValueTask NativeReadValueAsync()
    {
        AppleBluetoothRemoteService.AppleBluetoothRemoteDevice.CbPeripheralWrapper.CbPeripheral.ReadValue(CbCharacteristic);
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc />
    protected override bool NativeCanRead()
    {
        return CbCharacteristic.Properties.HasFlag(CBCharacteristicProperties.Read);
    }

    /// <inheritdoc />
    public void UpdatedCharacteristicValue(NSError? error, CBCharacteristic characteristic)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(characteristic);
            AppleNativeBluetoothException.ThrowIfError(error);
            OnReadValueSucceeded(characteristic.Value?.ToArray() ?? []);
        }
        catch (Exception e)
        {
            OnReadValueFailed(e);
        }
    }

    #endregion

    #region Write

    /// <inheritdoc />
    protected override ValueTask NativeWriteValueAsync(ReadOnlyMemory<byte> value)
    {
        AppleBluetoothRemoteService.AppleBluetoothRemoteDevice.CbPeripheralWrapper.CbPeripheral.WriteValue(value.ToNSData(),
            CbCharacteristic,
            CbCharacteristic.Properties.HasFlag(CBCharacteristicProperties.WriteWithoutResponse) ? CBCharacteristicWriteType.WithoutResponse : CBCharacteristicWriteType.WithResponse);
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc />
    protected override bool NativeCanWrite()
    {
        return CbCharacteristic.Properties.HasFlag(CBCharacteristicProperties.Write) || CbCharacteristic.Properties.HasFlag(CBCharacteristicProperties.WriteWithoutResponse);
    }

    /// <inheritdoc />
    public void WroteCharacteristicValue(NSError? error, CBCharacteristic characteristic)
    {
        try
        {
            AppleNativeBluetoothException.ThrowIfError(error);
            OnWriteValueSucceeded();
        }
        catch (Exception e)
        {
            OnWriteValueFailed(e);
        }
    }

    /// <summary>
    ///     Gets the iOS-specific write capability string representation for the characteristic.
    /// </summary>
    /// <returns>
    ///     Returns "WNR" for write without response, "WS" for authenticated signed writes, "W" for standard write,
    ///     or an empty string if no write operations are supported.
    /// </returns>
    protected override string ToWriteString()
    {
        if (CbCharacteristic.Properties.HasFlag(CBCharacteristicProperties.WriteWithoutResponse))
        {
            return "WNR";
        }

        if (CbCharacteristic.Properties.HasFlag(CBCharacteristicProperties.AuthenticatedSignedWrites))
        {
            return "WS";
        }

        if (CbCharacteristic.Properties.HasFlag(CBCharacteristicProperties.Write))
        {
            return "W";
        }

        return string.Empty;
    }

    #endregion

    #region Listen

    /// <inheritdoc />
    protected override bool NativeCanListen()
    {
        return CbCharacteristic.Properties.HasFlag(CBCharacteristicProperties.Notify) || CbCharacteristic.Properties.HasFlag(CBCharacteristicProperties.Indicate);
    }

    /// <inheritdoc />
    protected override ValueTask NativeReadIsListeningAsync()
    {
        OnReadIsListeningSucceeded(CbCharacteristic.IsNotifying);
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc />
    protected override ValueTask NativeWriteIsListeningAsync(bool shouldBeListening)
    {
        AppleBluetoothRemoteService.AppleBluetoothRemoteDevice.CbPeripheralWrapper.CbPeripheral.SetNotifyValue(shouldBeListening, CbCharacteristic);
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc />
    public void UpdatedNotificationState(NSError? error, CBCharacteristic characteristic)
    {
        try
        {
            AppleNativeBluetoothException.ThrowIfError(error);
            OnWriteIsListeningSucceeded();
        }
        catch (Exception e)
        {
            OnWriteIsListeningFailed(e);
        }
    }

    #endregion

    #region ReliableWrite

    /// <inheritdoc />
    protected override ValueTask NativeBeginReliableWriteAsync()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    protected override ValueTask NativeExecuteReliableWriteAsync()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    protected override ValueTask NativeAbortReliableWriteAsync()
    {
        throw new NotImplementedException();
    }

    #endregion

    #region Descriptors

    /// <inheritdoc />
    protected override ValueTask NativeDescriptorsExplorationAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        AppleBluetoothRemoteService.AppleBluetoothRemoteDevice.CbPeripheralWrapper.CbPeripheral.DiscoverDescriptors(CbCharacteristic);
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc />
    public void DiscoveredDescriptor(NSError? error, CBCharacteristic characteristic)
    {
        try
        {
            AppleNativeBluetoothException.ThrowIfError(error);
            var descriptors = CbCharacteristic.Descriptors ?? [];
            OnDescriptorsExplorationSucceeded(descriptors, FromInputTypeToOutputTypeConversion, AreRepresentingTheSameObject);
        }
        catch (Exception e)
        {
            OnDescriptorsExplorationFailed(e);
        }

        return;

        IBluetoothRemoteDescriptor FromInputTypeToOutputTypeConversion(CBDescriptor native)
        {
            var request = new AppleBluetoothDescriptorFactoryRequest(native);
            return DescriptorFactory.CreateDescriptor(this, request);
        }
    }

    /// <inheritdoc />
    public CbPeripheralWrapper.ICbDescriptorDelegate GetDescriptor(CBDescriptor? native)
    {
        ArgumentNullException.ThrowIfNull(native);
        try
        {
            var match = GetDescriptorOrDefault(descriptor => AreRepresentingTheSameObject(native, descriptor));
            return match as CbPeripheralWrapper.ICbDescriptorDelegate ?? throw new DescriptorNotFoundException(this, native.UUID.ToGuid());
        }
        catch (InvalidOperationException e)
        {
            var matches = GetDescriptors(descriptor => AreRepresentingTheSameObject(native, descriptor)).ToArray();
            throw new MultipleDescriptorsFoundException(this, matches, e);
        }
    }

    private static bool AreRepresentingTheSameObject(CBDescriptor native, IBluetoothRemoteDescriptor shared)
    {
        return shared is AppleBluetoothRemoteDescriptor s && native.UUID.Equals(s.CbDescriptor.UUID) && native.Handle.Handle.Equals(s.CbDescriptor.Handle.Handle);
    }

    #endregion
}
