using System.Collections.ObjectModel;

using Bluetooth.Abstractions;
using Bluetooth.Abstractions.AccessService;
using Bluetooth.Abstractions.Scanning;
using Bluetooth.Core.Scanning.CharacteristicAccess;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Bluetooth.Core.Scanning;

/// <inheritdoc cref="IBluetoothScanner" />
public abstract partial class BaseBluetoothScanner : BaseBindableObject, IBluetoothScanner
{
    /// <inheritdoc />
    public IBluetoothAdapter Adapter { get; }

    /// <inheritdoc />
    public IBluetoothCharacteristicAccessServicesRepository KnownServicesAndCharacteristicsRepository { get; }

    /// <inheritdoc />
    public IBluetoothDeviceFactory DeviceFactory { get; }

    /// <inheritdoc />
    public IBluetoothPermissionManager PermissionManager { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseBluetoothScanner"/> class.
    /// </summary>
    /// <param name="adapter">The Bluetooth adapter associated with this scanner.</param>
    /// <param name="permissionManager">The permission manager for handling Bluetooth permissions.</param>
    /// <param name="deviceFactory">The factory for creating Bluetooth devices.</param>
    /// <param name="knownServicesAndCharacteristicsRepository">The repository for known services and characteristics.</param>
    /// <param name="logger">The logger instance to use for logging.</param>
    protected BaseBluetoothScanner(IBluetoothAdapter adapter, IBluetoothPermissionManager permissionManager, IBluetoothDeviceFactory deviceFactory, IBluetoothCharacteristicAccessServicesRepository knownServicesAndCharacteristicsRepository, ILogger? logger = null)
        : base(logger)
    {
        ArgumentNullException.ThrowIfNull(adapter);
        Adapter = adapter;

        ArgumentNullException.ThrowIfNull(permissionManager);
        PermissionManager = permissionManager;

        ArgumentNullException.ThrowIfNull(deviceFactory);
        DeviceFactory = deviceFactory;

        ArgumentNullException.ThrowIfNull(knownServicesAndCharacteristicsRepository);
        KnownServicesAndCharacteristicsRepository = knownServicesAndCharacteristicsRepository;

        Devices = new ObservableCollection<IBluetoothDevice>();
        Devices.CollectionChanged += DevicesOnCollectionChanged;
    }

    /// <summary>
    /// Gets the collection of Bluetooth devices managed by this scanner.
    /// </summary>
    /// <remarks>
    /// This collection is lazily initialized and automatically hooks up collection change notifications
    /// to raise the appropriate events (<see cref="DevicesAdded"/>, <see cref="DevicesRemoved"/>, <see cref="DeviceListChanged"/>).
    /// </remarks>
    private ObservableCollection<IBluetoothDevice> Devices { get; }

}
