using Bluetooth.Abstractions.Scanning.Factories;

namespace Bluetooth.Maui.Platforms.Windows.Scanning;

/// <inheritdoc/>
public class BluetoothCharacteristic : BaseBluetoothCharacteristic
{
    /// <inheritdoc/>
    public BluetoothCharacteristic(IBluetoothService service, IBluetoothCharacteristicFactory.BluetoothCharacteristicFactoryRequest request) : base(service, request)
    {
    }

    /// <inheritdoc/>
    protected override bool NativeCanListen()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    protected override ValueTask NativeReadIsListeningAsync()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    protected override ValueTask NativeWriteIsListeningAsync(bool shouldBeListening)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    protected override ValueTask NativeWriteValueAsync(ReadOnlyMemory<byte> value)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    protected override bool NativeCanWrite()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    protected override ValueTask NativeReadValueAsync()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    protected override bool NativeCanRead()
    {
        throw new NotImplementedException();
    }
}
