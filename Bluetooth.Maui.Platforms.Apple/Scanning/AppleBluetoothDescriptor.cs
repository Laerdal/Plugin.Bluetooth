using Bluetooth.Abstractions.Scanning.Factories;
using Bluetooth.Maui.Platforms.Apple.Exceptions;
using Bluetooth.Maui.Platforms.Apple.Scanning.Factories;
using Bluetooth.Maui.Platforms.Apple.Scanning.NativeObjects;
using Bluetooth.Maui.Platforms.Apple.Tools;

namespace Bluetooth.Maui.Platforms.Apple.Scanning;

/// <inheritdoc cref="BaseBluetoothRemoteDescriptor" />
public class AppleBluetoothRemoteDescriptor : Core.Scanning.BaseBluetoothRemoteDescriptor, CbPeripheralWrapper.ICbDescriptorDelegate
{
    /// <summary>
    /// Gets the native iOS Core Bluetooth descriptor.
    /// </summary>
    public CBDescriptor CbDescriptor { get; }

    /// <summary>
    /// Gets the Bluetooth characteristic to which this descriptor belongs, cast to the Apple-specific implementation.
    /// </summary>
    public AppleBluetoothRemoteCharacteristic AppleBluetoothRemoteCharacteristic => (AppleBluetoothRemoteCharacteristic) RemoteCharacteristic;

    /// <summary>
    /// Initializes a new instance of the <see cref="AppleBluetoothRemoteDescriptor"/> class with the specified characteristic and factory request.
    /// </summary>
    /// <param name="remoteCharacteristic">The Bluetooth characteristic to which this descriptor belongs.</param>
    /// <param name="request">The factory request containing the native Core Bluetooth descriptor.</param>
    public AppleBluetoothRemoteDescriptor(Abstractions.Scanning.IBluetoothRemoteCharacteristic remoteCharacteristic, IBluetoothDescriptorFactory.BluetoothDescriptorFactoryRequest request) : base(remoteCharacteristic, request)
    {
        ArgumentNullException.ThrowIfNull(request);
        if (request is not AppleBluetoothDescriptorFactoryRequest appleRequest)
        {
            throw new ArgumentException($"Expected request of type {typeof(AppleBluetoothDescriptorFactoryRequest)}, but got {request.GetType()}");
        }
        CbDescriptor = appleRequest.CbDescriptor;
    }

    /// <inheritdoc />
    protected override ValueTask NativeWriteValueAsync(ReadOnlyMemory<byte> value)
    {
        AppleBluetoothRemoteCharacteristic.AppleBluetoothRemoteService.AppleBluetoothRemoteDevice.CbPeripheralWrapper.CbPeripheral.WriteValue(value.ToNSData(), CbDescriptor);
        return ValueTask.CompletedTask;
    }

    private bool _canWrite = true; // iOS doesn't provide explicit write permissions for descriptors, so we optimistically assume we can write until we get an error.

    /// <inheritdoc />
    protected override bool NativeCanWrite()
    {
        return _canWrite;
    }

    /// <inheritdoc />
    protected override ValueTask NativeReadValueAsync()
    {
        AppleBluetoothRemoteCharacteristic.AppleBluetoothRemoteService.AppleBluetoothRemoteDevice.CbPeripheralWrapper.CbPeripheral.ReadValue(CbDescriptor);
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc />
    protected override bool NativeCanRead()
    {
        return true;
    }

    /// <inheritdoc />
    public void UpdatedValue(NSError? error, CBDescriptor descriptor)
    {
        try
        {
            AppleNativeBluetoothException.ThrowIfError(error);
            ArgumentNullException.ThrowIfNull(descriptor);

            OnReadValueSucceeded(descriptor.Value.ToReadOnlyMemoryBytes());
        }
        catch (Exception e)
        {
            OnReadValueFailed(e);
        }
    }

    /// <inheritdoc />
    public void WroteDescriptorValue(NSError? error, CBDescriptor descriptor)
    {
        try
        {
            AppleNativeBluetoothException.ThrowIfError(error);
            OnWriteValueSucceeded();
        }
        catch (Exception e)
        {
            if (e is AppleNativeBluetoothException { Code: (int)CBATTError.WriteNotPermitted })
            {
                // iOS doesn't provide explicit write permissions for descriptors, so if we get a permission denied error, we assume it's because the descriptor doesn't allow writing and update our state accordingly.
                _canWrite = false;
            }
            OnWriteValueFailed(e);
        }
    }


}
