using Bluetooth.Maui.Platforms.Apple.Logging;
using Bluetooth.Maui.Platforms.Apple.Scanning.Factories;
using Bluetooth.Maui.Platforms.Apple.Scanning.NativeObjects;

namespace Bluetooth.Maui.Platforms.Apple.Scanning;

/// <inheritdoc cref="BaseBluetoothRemoteDescriptor" />
public class AppleBluetoothRemoteDescriptor : BaseBluetoothRemoteDescriptor, CbPeripheralWrapper.ICbDescriptorDelegate
{
    private bool _canWrite = true; // iOS doesn't provide explicit write permissions for descriptors, so we optimistically assume we can write until we get an error.

    /// <summary>
    ///     Initializes a new instance of the <see cref="AppleBluetoothRemoteDescriptor" /> class with the specified Core Bluetooth descriptor, parent characteristic, ID, and logger.
    /// </summary>
    /// <param name="cbDescriptor">The native iOS Core Bluetooth descriptor represented by this remote descriptor.</param>
    /// <param name="parentCharacteristic">The Bluetooth characteristic to which this descriptor belongs.</param>
    /// <param name="id">The unique identifier for this descriptor.</param>
    /// <param name="nameProvider">An optional name provider for resolving the descriptor's name based on its ID.</param>
    /// <param name="logger">An optional logger for logging descriptor-related events and errors.</param>
    public AppleBluetoothRemoteDescriptor(CBDescriptor cbDescriptor,
        IBluetoothRemoteCharacteristic parentCharacteristic,
        Guid id,
        IBluetoothNameProvider? nameProvider = null,
        ILogger<IBluetoothRemoteDescriptor>? logger = null) : base(parentCharacteristic, id, nameProvider, logger)
    {
        CbDescriptor = cbDescriptor;
    }

    /// <summary>
    ///     Initializes a new instance using a factory spec.
    /// </summary>
    /// <param name="parentCharacteristic">The Bluetooth characteristic to which this descriptor belongs.</param>
    /// <param name="spec">The Apple-specific factory spec containing the native descriptor.</param>
    /// <param name="logger">An optional logger for logging descriptor-related events and errors.</param>
    public AppleBluetoothRemoteDescriptor(
        IBluetoothRemoteCharacteristic parentCharacteristic,
        AppleBluetoothRemoteDescriptorFactorySpec spec,
        ILogger<IBluetoothRemoteDescriptor>? logger = null) : base(parentCharacteristic, spec, logger)
    {
        ArgumentNullException.ThrowIfNull(spec);
        CbDescriptor = spec.CbDescriptor;
    }

    /// <summary>
    ///     Gets the native iOS Core Bluetooth descriptor.
    /// </summary>
    public CBDescriptor CbDescriptor { get; }

    /// <summary>
    ///     Gets the Bluetooth characteristic to which this descriptor belongs, cast to the Apple-specific implementation.
    /// </summary>
    public AppleBluetoothRemoteCharacteristic AppleBluetoothRemoteCharacteristic => (AppleBluetoothRemoteCharacteristic) Characteristic;

    /// <inheritdoc />
    public void UpdatedValue(NSError? error, CBDescriptor descriptor)
    {
        try
        {
            AppleNativeBluetoothException.ThrowIfError(error);
            ArgumentNullException.ThrowIfNull(descriptor);

            var data = descriptor.Value.ToReadOnlyMemoryBytes();
            Logger?.LogDescriptorReadCompleted(Id, Characteristic.Service.Device.Id, data.Length);
            OnReadValueSucceeded(data);
        }
        catch (Exception e)
        {
            Logger?.LogDescriptorReadError(Id, Characteristic.Service.Device.Id, e.Message, e);
            OnReadValueFailed(e);
        }
    }

    /// <inheritdoc />
    public void WroteDescriptorValue(NSError? error, CBDescriptor descriptor)
    {
        try
        {
            AppleNativeBluetoothException.ThrowIfError(error);
            Logger?.LogDescriptorWriteCompleted(Id, Characteristic.Service.Device.Id);
            OnWriteValueSucceeded();
        }
        catch (Exception e)
        {
            if (e is AppleNativeBluetoothException { Code: (int) CBATTError.WriteNotPermitted })
            {
                // iOS doesn't provide explicit write permissions for descriptors, so if we get a permission denied error, we assume it's because the descriptor doesn't allow writing and update our state accordingly.
                _canWrite = false;
            }

            Logger?.LogDescriptorWriteError(Id, Characteristic.Service.Device.Id, e.Message, e);
            OnWriteValueFailed(e);
        }
    }

    /// <inheritdoc />
    protected override ValueTask NativeWriteValueAsync(ReadOnlyMemory<byte> value)
    {
        Logger?.LogDescriptorWrite(Id, Characteristic.Service.Device.Id, value.Length);
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
        Logger?.LogDescriptorRead(Id, Characteristic.Service.Device.Id);
        AppleBluetoothRemoteCharacteristic.AppleBluetoothRemoteService.AppleBluetoothRemoteDevice.CbPeripheralWrapper.CbPeripheral.ReadValue(CbDescriptor);
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc />
    protected override bool NativeCanRead()
    {
        return true;
    }
}
