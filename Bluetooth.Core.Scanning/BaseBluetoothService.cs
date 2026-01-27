using Bluetooth.Abstractions.AccessService;
using Bluetooth.Abstractions.Scanning;

namespace Bluetooth.Core.Scanning;

/// <inheritdoc cref="IBluetoothService" />
public abstract partial class BaseBluetoothService : BaseBindableObject, IBluetoothService
{
    /// <inheritdoc/>
    public IBluetoothDevice Device { get; }

    /// <inheritdoc/>
    public IBluetoothCharacteristicFactory CharacteristicFactory { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseBluetoothService"/> class.
    /// </summary>
    /// <param name="device">The Bluetooth device associated with this service.</param>
    /// <param name="request">The factory request containing service information.</param>
    /// <param name="characteristicFactory">The factory for creating Bluetooth characteristics.</param>
    /// <param name="characteristicAccessServicesRepository">The repository for Bluetooth characteristic access services.</param>
    protected BaseBluetoothService(IBluetoothDevice device, IBluetoothServiceFactory.BluetoothServiceFactoryRequest request, IBluetoothCharacteristicFactory characteristicFactory, IBluetoothCharacteristicAccessServicesRepository characteristicAccessServicesRepository) : base()
    {
        ArgumentNullException.ThrowIfNull(device);
        Device = device;

        ArgumentNullException.ThrowIfNull(characteristicFactory);
        CharacteristicFactory = characteristicFactory;

        ArgumentNullException.ThrowIfNull(request);
        Id = request.ServiceId;

        ArgumentNullException.ThrowIfNull(characteristicAccessServicesRepository);
        Name = characteristicAccessServicesRepository.GetServiceName(request.ServiceId);
    }

    /// <inheritdoc/>
    public Guid Id { get; }

    /// <inheritdoc/>
    public string Name { get;}

    /// <summary>
    /// Performs the core disposal logic for the service, including canceling pending operations and cleaning up resources.
    /// </summary>
    /// <returns>A task that represents the asynchronous disposal operation.</returns>
    protected async virtual ValueTask DisposeAsyncCore()
    {
        // Cancel any pending explorations
        CharacteristicsExplorationTcs?.TrySetCanceled();

        // Unsubscribe from event
        Characteristics.CollectionChanged -= CharacteristicsOnCollectionChanged;

        // Unsubscribe from other events
        CharacteristicListChanged = null;
        CharacteristicsAdded = null;
        CharacteristicsRemoved = null;

        await ClearCharacteristicsAsync().ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async ValueTask DisposeAsync()
    {
        await DisposeAsyncCore().ConfigureAwait(false);
            }
}
