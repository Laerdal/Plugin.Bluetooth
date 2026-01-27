namespace Bluetooth.Maui.Platforms.Apple.Scanning;

/// <inheritdoc/>
public class BluetoothService : BaseBluetoothService
{
    /// <inheritdoc/>
    public BluetoothService(IBluetoothDevice device,
        IBluetoothServiceFactory.BluetoothServiceFactoryRequest request,
        IBluetoothCharacteristicFactory characteristicFactory,
        IBluetoothCharacteristicAccessServicesRepository characteristicAccessServicesRepository) : base(device, request, characteristicFactory, characteristicAccessServicesRepository)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (request is not BluetoothServiceFactoryRequest nativeRequest)
        {
            throw new ArgumentException($"Request must be of type {nameof(BluetoothServiceFactoryRequest)}", nameof(request));
        }

    }

    /// <inheritdoc/>
    protected override ValueTask NativeCharacteristicsExplorationAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
