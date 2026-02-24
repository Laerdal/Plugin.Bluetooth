using Bluetooth.Maui.Platforms.Apple.Logging;
using Bluetooth.Maui.Platforms.Apple.Scanning.Factories;
using Bluetooth.Maui.Platforms.Apple.Scanning.NativeObjects;

using DescriptorNotFoundException = Bluetooth.Abstractions.Scanning.Exceptions.DescriptorNotFoundException;
using MultipleDescriptorsFoundException = Bluetooth.Abstractions.Scanning.Exceptions.MultipleDescriptorsFoundException;

namespace Bluetooth.Maui.Platforms.Apple.Scanning;

/// <inheritdoc cref="BaseBluetoothRemoteCharacteristic" />
public class AppleBluetoothRemoteCharacteristic : BaseBluetoothRemoteCharacteristic, CbPeripheralWrapper.ICbCharacteristicDelegate
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="AppleBluetoothRemoteCharacteristic" /> class with the specified service, factory spec, and descriptor factory.
    /// </summary>
    /// <param name="remoteService">The Bluetooth service to which this characteristic belongs.</param>
    /// <param name="spec">The factory spec containing the information needed to create this characteristic.</param>
    /// <param name="descriptorFactory">The factory used to create descriptors for this characteristic.</param>
    public AppleBluetoothRemoteCharacteristic(IBluetoothRemoteService remoteService, IBluetoothRemoteCharacteristicFactory.BluetoothRemoteCharacteristicFactorySpec spec, IBluetoothRemoteDescriptorFactory descriptorFactory) :
        base(remoteService, spec, descriptorFactory)
    {
        ArgumentNullException.ThrowIfNull(spec);
        if (spec is not AppleBluetoothRemoteCharacteristicFactorySpec nativeSpec)
        {
            throw new ArgumentException($"Expected spec of type {typeof(AppleBluetoothRemoteCharacteristicFactorySpec)}, but got {spec.GetType()}");
        }

        CbCharacteristic = nativeSpec.CbCharacteristic;
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
    /// <seealso href="https://developer.apple.com/documentation/corebluetooth/cbperipheral/1518759-readvalue">iOS CBPeripheral.readValue</seealso>
    protected override ValueTask NativeReadValueAsync()
    {
        Logger?.LogCharacteristicRead(Id, RemoteService.Device.Id);
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
            var data = characteristic.Value?.ToArray() ?? [];

            // Check if this is a notification/indication or a read response
            if (characteristic.IsNotifying)
            {
                Logger?.LogNotificationReceived(Id, RemoteService.Device.Id, data.Length);
            }
            else
            {
                Logger?.LogCharacteristicReadCompleted(Id, RemoteService.Device.Id, data.Length);
            }

            OnReadValueSucceeded(data);
        }
        catch (Exception e)
        {
            Logger?.LogCharacteristicReadError(Id, RemoteService.Device.Id, e.Message, e);
            OnReadValueFailed(e);
        }
    }

    #endregion

    #region Write

    /// <inheritdoc />
    /// <seealso href="https://developer.apple.com/documentation/corebluetooth/cbperipheral/1518747-writevalue">iOS CBPeripheral.writeValue</seealso>
    protected override ValueTask NativeWriteValueAsync(ReadOnlyMemory<byte> value)
    {
        Logger?.LogCharacteristicWrite(Id, RemoteService.Device.Id, value.Length);
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
            Logger?.LogCharacteristicWriteCompleted(Id, RemoteService.Device.Id);
            OnWriteValueSucceeded();
        }
        catch (Exception e)
        {
            Logger?.LogCharacteristicWriteError(Id, RemoteService.Device.Id, e.Message, e);
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
    /// <seealso href="https://developer.apple.com/documentation/corebluetooth/cbperipheral/1518949-setnotifyvalue">iOS CBPeripheral.setNotifyValue</seealso>
    protected override ValueTask NativeWriteIsListeningAsync(bool shouldBeListening)
    {
        var action = shouldBeListening ? "Enabling" : "Disabling";
        Logger?.LogNotificationStateChange(action, Id, RemoteService.Device.Id);
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
        // CoreBluetooth doesn't support reliable write transactions
        throw new NotSupportedException(
            "Reliable write transactions are not supported on iOS/macOS platforms. " +
            "CoreBluetooth does not provide native transaction support for characteristic writes.");
    }

    /// <inheritdoc />
    protected override ValueTask NativeExecuteReliableWriteAsync()
    {
        // CoreBluetooth doesn't support reliable write transactions
        throw new NotSupportedException(
            "Reliable write transactions are not supported on iOS/macOS platforms. " +
            "CoreBluetooth does not provide native transaction support for characteristic writes.");
    }

    /// <inheritdoc />
    protected override ValueTask NativeAbortReliableWriteAsync()
    {
        // CoreBluetooth doesn't support reliable write transactions
        throw new NotSupportedException(
            "Reliable write transactions are not supported on iOS/macOS platforms. " +
            "CoreBluetooth does not provide native transaction support for characteristic writes.");
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
            OnDescriptorsExplorationSucceeded(descriptors, AreRepresentingTheSameObject, FromInputTypeToOutputTypeConversion);
        }
        catch (Exception e)
        {
            OnDescriptorsExplorationFailed(e);
        }

        return;

        IBluetoothRemoteDescriptor FromInputTypeToOutputTypeConversion(CBDescriptor native)
        {
            var spec = new AppleBluetoothRemoteDescriptorFactorySpec(native);
            return DescriptorFactory.Create(this, spec);
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
