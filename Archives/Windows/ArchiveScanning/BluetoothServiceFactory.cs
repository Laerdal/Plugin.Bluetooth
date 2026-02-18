using Bluetooth.Abstractions.Scanning.AccessService;
using Bluetooth.Abstractions.Scanning.Factories;

namespace Bluetooth.Maui.Platforms.Windows.Scanning;

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
    public IBluetoothService CreateService(IBluetoothDevice device, IBluetoothServiceFactory.BluetoothServiceFactoryRequest request)
    {
        return new BluetoothService(device, request, CharacteristicFactory, CharacteristicAccessServicesRepository);
    }
}
