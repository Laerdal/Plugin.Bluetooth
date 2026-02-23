using Bluetooth.Maui.Platforms.Apple.Logging;
using Bluetooth.Maui.Platforms.Apple.Scanning.Factories;
using Bluetooth.Maui.Platforms.Apple.Scanning.NativeObjects;

namespace Bluetooth.Maui.Platforms.Apple.Scanning;

/// <inheritdoc cref="BaseBluetoothRemoteDescriptor" />
public class AppleBluetoothRemoteDescriptor : BaseBluetoothRemoteDescriptor, CbPeripheralWrapper.ICbDescriptorDelegate
{
    private bool _canWrite = true; // iOS doesn't provide explicit write permissions for descriptors, so we optimistically assume we can write until we get an error.

    /// <summary>
    ///     Initializes a new instance of the <see cref="AppleBluetoothRemoteDescriptor" /> class with the specified characteristic and factory request.
    /// </summary>
    /// <param name="remoteCharacteristic">The Bluetooth characteristic to which this descriptor belongs.</param>
    /// <param name="request">The factory request containing the native Core Bluetooth descriptor.</param>
    public AppleBluetoothRemoteDescriptor(IBluetoothRemoteCharacteristic remoteCharacteristic, IBluetoothDescriptorFactory.BluetoothDescriptorFactoryRequest request) : base(remoteCharacteristic, request)
    {
        ArgumentNullException.ThrowIfNull(request);
        if (request is not AppleBluetoothDescriptorFactoryRequest appleRequest)
        {
            throw new ArgumentException($"Expected request of type {typeof(AppleBluetoothDescriptorFactoryRequest)}, but got {request.GetType()}");
        }

        CbDescriptor = appleRequest.CbDescriptor;
    }

    /// <summary>
    ///     Gets the native iOS Core Bluetooth descriptor.
    /// </summary>
    public CBDescriptor CbDescriptor { get; }

    /// <summary>
    ///     Gets the Bluetooth characteristic to which this descriptor belongs, cast to the Apple-specific implementation.
    /// </summary>
    public AppleBluetoothRemoteCharacteristic AppleBluetoothRemoteCharacteristic => (AppleBluetoothRemoteCharacteristic) RemoteCharacteristic;

    /// <inheritdoc />
    public void UpdatedValue(NSError? error, CBDescriptor descriptor)
    {
        try
        {
            AppleNativeBluetoothException.ThrowIfError(error);
            ArgumentNullException.ThrowIfNull(descriptor);

            var data = descriptor.Value.ToReadOnlyMemoryBytes();
            Logger?.LogDescriptorReadCompleted(Id, RemoteCharacteristic.RemoteService.Device.Id, data.Length);
            OnReadValueSucceeded(data);
        }
        catch (Exception e)
        {
            Logger?.LogDescriptorReadError(Id, RemoteCharacteristic.RemoteService.Device.Id, e.Message, e);
            OnReadValueFailed(e);
        }
    }

    /// <inheritdoc />
    public void WroteDescriptorValue(NSError? error, CBDescriptor descriptor)
    {
        try
        {
            AppleNativeBluetoothException.ThrowIfError(error);
            Logger?.LogDescriptorWriteCompleted(Id, RemoteCharacteristic.RemoteService.Device.Id);
            OnWriteValueSucceeded();
        }
        catch (Exception e)
        {
            if (e is AppleNativeBluetoothException { Code: (int) CBATTError.WriteNotPermitted })
            {
                // iOS doesn't provide explicit write permissions for descriptors, so if we get a permission denied error, we assume it's because the descriptor doesn't allow writing and update our state accordingly.
                _canWrite = false;
            }

            Logger?.LogDescriptorWriteError(Id, RemoteCharacteristic.RemoteService.Device.Id, e.Message, e);
            OnWriteValueFailed(e);
        }
    }

    /// <inheritdoc />
    protected override ValueTask NativeWriteValueAsync(ReadOnlyMemory<byte> value)
    {
        Logger?.LogDescriptorWrite(Id, RemoteCharacteristic.RemoteService.Device.Id, value.Length);
        AppleBluetoothRemoteCharacteristic.AppleBluetoothRemoteService.AppleBluetoothRemoteDevice.CbPeripheralWrapper.CbPeripheral.WriteValue(value.ToNSData(), CbDescriptor);
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc />
    protected override bool NativeCanWrite()
    {
        return _canWrite;
    }

    /// <inheritdoc />
    protected override ValueTask NativeReadValueAsync()
    {
        Logger?.LogDescriptorRead(Id, RemoteCharacteristic.RemoteService.Device.Id);
        AppleBluetoothRemoteCharacteristic.AppleBluetoothRemoteService.AppleBluetoothRemoteDevice.CbPeripheralWrapper.CbPeripheral.ReadValue(CbDescriptor);
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc />
    protected override bool NativeCanRead()
    {
        return true;
    }
}
