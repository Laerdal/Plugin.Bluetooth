namespace Plugin.Bluetooth.BaseClasses;

/// <inheritdoc cref="IBluetoothService" />
public abstract partial class BaseBluetoothService
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BaseBluetoothService"/> class.
    /// </summary>
    /// <param name="device">The Bluetooth device associated with this service.</param>
    /// <param name="serviceUuid">The unique identifier of the service.</param>
    protected BaseBluetoothService(IBluetoothDevice device, Guid serviceUuid)
    {
        Device = device;
        Id = serviceUuid;
        Name = Device.Scanner.KnownServicesAndCharacteristicsRepository.GetServiceName(serviceUuid);
    }

    /// <inheritdoc/>
    public IBluetoothDevice Device { get; }

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

    /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources asynchronously.</summary>
    /// <returns>A task that represents the asynchronous dispose operation.</returns>
    public async ValueTask DisposeAsync()
    {
        await DisposeAsyncCore().ConfigureAwait(false);
        GC.SuppressFinalize(this);
    }
}
