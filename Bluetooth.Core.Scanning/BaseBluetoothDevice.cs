using Bluetooth.Abstractions.Enums;
using Bluetooth.Abstractions.Scanning;
using Bluetooth.Core.Exceptions;

namespace Bluetooth.Core.Scanning;

/// <inheritdoc cref="IBluetoothDevice" />
public abstract partial class BaseBluetoothDevice : BaseBindableObject, IBluetoothDevice
{
    /// <inheritdoc/>
    public IBluetoothScanner Scanner { get; }

    /// <inheritdoc/>
    public IBluetoothServiceFactory ServiceFactory { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseBluetoothDevice"/> class.
    /// </summary>
    /// <param name="scanner">The Bluetooth scanner associated with this device.</param>
    /// <param name="request">The factory request containing device information.</param>
    /// <param name="serviceFactory">The factory for creating Bluetooth services.</param>
    protected BaseBluetoothDevice(IBluetoothScanner scanner, IBluetoothDeviceFactory.BluetoothDeviceFactoryRequest request, IBluetoothServiceFactory serviceFactory)
    {
        ArgumentNullException.ThrowIfNull(scanner);
        Scanner = scanner;

        ArgumentNullException.ThrowIfNull(serviceFactory);
        ServiceFactory = serviceFactory;

        ArgumentNullException.ThrowIfNull(request);
        Id = request.Id;
        Manufacturer = request.Manufacturer;
        if(request is IBluetoothDeviceFactory.BluetoothDeviceFactoryRequestWithAdvertisement advRequest)
        {
            OnAdvertisementReceived(advRequest.Advertisement);
        }
    }

    /// <inheritdoc/>
    public string Id { get; }

    /// <inheritdoc/>
    public Manufacturer Manufacturer { get; }

    /// <inheritdoc/>
    public DateTimeOffset LastSeen { get; private set; }

    /// <summary>
    /// Performs the core disposal logic for the device, including disconnection, cleanup of pending operations, and resource disposal.
    /// </summary>
    /// <returns>A task that represents the asynchronous disposal operation.</returns>
    protected async virtual ValueTask DisposeAsyncCore()
    {

        try
        {
            // Ensure device is disconnected
            await DisconnectIfNeededAsync().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, ex);
        }

        // Clear RSSI history
        _rssiHistory.Clear();

        // Complete any pending tasks
        ConnectionTcs?.TrySetCanceled();
        DisconnectionTcs?.TrySetCanceled();
        ServicesExplorationTcs?.TrySetCanceled();

        // Unsubscribe from events
        Services.CollectionChanged -= ServicesOnCollectionChanged;
        AdvertisementReceived = null;

        Connected = null;
        Disconnected = null;
        Connecting = null;
        Disconnecting = null;
        UnexpectedDisconnection = null;

        ServiceListChanged = null;
        ServicesAdded = null;
        ServicesRemoved = null;

        await ClearServicesAsync().ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async ValueTask DisposeAsync()
    {
        await DisposeAsyncCore().ConfigureAwait(false);
            }
}
