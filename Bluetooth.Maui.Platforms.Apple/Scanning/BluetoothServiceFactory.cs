namespace Bluetooth.Maui.Platforms.Apple.Scanning;

/// <inheritdoc/>
public class BluetoothServiceFactory : IBluetoothServiceFactory
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BluetoothServiceFactory"/> class.
    /// </summary>
    public BluetoothServiceFactory(IBluetoothCharacteristicAccessServicesRepository characteristicAccessServicesRepository, IBluetoothCharacteristicFactory characteristicFactory)
    {
        CharacteristicAccessServicesRepository = characteristicAccessServicesRepository;
        CharacteristicFactory = characteristicFactory;
    }

    private IBluetoothCharacteristicAccessServicesRepository CharacteristicAccessServicesRepository { get; }

    private IBluetoothCharacteristicFactory CharacteristicFactory { get; }

    /// <inheritdoc/>
    public ValueTask<IBluetoothService> CreateServiceAsync(IBluetoothDevice device, IBluetoothServiceFactory.BluetoothServiceFactoryRequest request, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
#pragma warning disable CA2000 // Dispose objects before losing scope
        return ValueTask.FromResult<IBluetoothService>(new BluetoothService(device, request, CharacteristicFactory, CharacteristicAccessServicesRepository));
#pragma warning restore CA2000 // Dispose objects before losing scope
    }
}
