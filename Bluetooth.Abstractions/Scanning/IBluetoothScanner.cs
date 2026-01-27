using Bluetooth.Abstractions.AccessService;
using Bluetooth.Abstractions.Scanning.EventArgs;

namespace Bluetooth.Abstractions.Scanning;

/// <summary>
/// Interface for managing and scanning Bluetooth devices.
/// </summary>
public interface IBluetoothScanner
{
    /// <summary>
    /// Gets the Bluetooth adapter associated with this scanner.
    /// </summary>
    IBluetoothAdapter Adapter { get; }

    /// <summary>
    /// Gets the repository for known Bluetooth services and characteristics.
    /// </summary>
    IBluetoothCharacteristicAccessServicesRepository KnownServicesAndCharacteristicsRepository { get; }

    /// <summary>
    /// Gets the factory used to create Bluetooth devices.
    /// </summary>
    IBluetoothDeviceFactory DeviceFactory { get; }

    /// <summary>
    /// Gets the permission manager for handling Bluetooth permissions.
    /// </summary>
    IBluetoothPermissionManager PermissionManager { get; }

    #region Start/Stop

    /// <summary>
    /// Occurs when the running state of the Bluetooth activity changes.
    /// </summary>
    event EventHandler? RunningStateChanged;

    /// <summary>
    /// Gets a value indicating whether the Bluetooth activity is actively running.
    /// </summary>
    bool IsRunning { get; }

    #region Start

    /// <summary>
    /// Gets a value indicating whether the Bluetooth activity is starting.
    /// </summary>
    bool IsStarting { get; }

    /// <summary>
    /// Occurs when the Bluetooth activity is starting.
    /// </summary>
    event EventHandler Starting;

    /// <summary>
    /// Occurs when the Bluetooth activity has started.
    /// </summary>
    event EventHandler Started;

    /// <summary>
    /// Asynchronously starts the Bluetooth activity with an optional timeout.
    /// </summary>
    /// <param name="options">The options for starting the Bluetooth activity.</param>
    /// <param name="timeout">The timeout for this operation</param>
    /// <param name="cancellationToken">A cancellation token to cancel this operation.</param>
    /// <returns>A task that represents the asynchronous start operation.</returns>
    /// <remarks>Ensures that the Bluetooth activity is initialized and ready for use.</remarks>
    Task StartScanningAsync(IBluetoothScannerStartScanningOptions options, TimeSpan? timeout = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously starts the Bluetooth activity if it is not already running, with an optional timeout.
    /// </summary>
    /// <param name="options">The options for starting the Bluetooth activity.</param>
    /// <param name="timeout">The timeout for this operation</param>
    /// <param name="cancellationToken">A cancellation token to cancel this operation.</param>
    /// <returns>A task that represents the asynchronous start operation.</returns>
    /// <remarks>Checks if the Bluetooth activity is already running before attempting to start it.</remarks>
    ValueTask StartScanningIfNeededAsync(IBluetoothScannerStartScanningOptions options, TimeSpan? timeout = null, CancellationToken cancellationToken = default);

    #endregion

    #region Stop

    /// <summary>
    /// Gets a value indicating whether the Scanner is stopping.
    /// </summary>
    bool IsStopping { get; }

    /// <summary>
    /// Occurs when the Scanner is stopping.
    /// </summary>
    event EventHandler Stopping;

    /// <summary>
    /// Occurs when the Scanner has stopped.
    /// </summary>
    event EventHandler Stopped;

    /// <summary>
    /// Asynchronously stops the Scanner with an optional timeout.
    /// </summary>
    /// <param name="timeout">The timeout for this operation</param>
    /// <param name="cancellationToken">A cancellation token to cancel this operation.</param>
    /// <returns>A task that represents the asynchronous stop operation.</returns>
    /// <remarks>Ensures that the Scanner and its resources are safely released.</remarks>
    Task StopScanningAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Asynchronously stops the Scanner if it is running, with an optional timeout.
    /// </summary>
    /// <param name="timeout">The timeout for this operation</param>
    /// <param name="cancellationToken">A cancellation token to cancel this operation.</param>
    /// <returns>A task that represents the asynchronous stop operation.</returns>
    /// <remarks>Checks if the Scanner is running before attempting to stop it.</remarks>
    ValueTask StopScanningIfNeededAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default);

    #endregion

    #endregion

    #region Configuration

    /// <summary>
    /// Changes the current scanner configuration while scanning is active.
    /// </summary>
    /// <param name="options">The new configuration</param>
    /// <param name="timeout">The timeout for this operation</param>
    /// <param name="cancellationToken">A cancellation token to cancel this operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    Task UpdateScannerOptionsAsync(IBluetoothScannerStartScanningOptions options,TimeSpan? timeout = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the current information being advertised
    /// </summary>
    IBluetoothScannerStartScanningOptions StartScanningOptions { get; }

    #endregion

    #region Advertisement

    /// <summary>
    /// Event triggered when a Bluetooth advertisement is received.
    /// </summary>
    event EventHandler<AdvertisementReceivedEventArgs> AdvertisementReceived;

    #endregion

    #region Clean

    /// <summary>
    /// Cleans resources associated with a list of Bluetooth devices.
    /// </summary>
    /// <param name="devices">The devices to clean.</param>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    ValueTask CleanAsync(IEnumerable<IBluetoothDevice> devices);

    /// <summary>
    /// Cleans resources associated with a specific Bluetooth device.
    /// </summary>
    /// <param name="device">The device to clean.</param>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    ValueTask CleanAsync(IBluetoothDevice? device);

    /// <summary>
    /// Cleans resources associated with a Bluetooth device by its ID.
    /// </summary>
    /// <param name="deviceId">The ID of the device to clean.</param>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    ValueTask CleanAsync(string deviceId);

    /// <summary>
    /// Cleans all resources associated with Bluetooth devices. This includes all devices, services, and characteristics.
    /// </summary>
    /// <returns>A <see cref="Task" /> representing the asynchronous operation.</returns>
    ValueTask CleanAsync();

    #endregion

    #region Devices - Exploration

    /// <summary>
    /// Event triggered when the list of available devices changes.
    /// </summary>
    event EventHandler<DeviceListChangedEventArgs>? DeviceListChanged;

    /// <summary>
    /// Event triggered when devices are added.
    /// </summary>
    event EventHandler<DevicesAddedEventArgs>? DevicesAdded;

    /// <summary>
    /// Event triggered when devices are removed.
    /// </summary>
    event EventHandler<DevicesRemovedEventArgs>? DevicesRemoved;

    #endregion

    #region Devices - Get

    /// <summary>
    /// Returns the closest Bluetooth device currently available.
    /// </summary>
    /// <param name="filter">An optional function to filter devices. Defaults to null for all devices.</param>
    /// <returns>The closest <see cref="IBluetoothDevice" />, or null if none are found.</returns>
    IBluetoothDevice? GetClosestDeviceOrDefault(Func<IBluetoothDevice, bool>? filter = null);

    /// <summary>
    /// Returns the first Bluetooth device that matches the specified filter.
    /// </summary>
    /// <param name="filter">A function to filter devices. Should return true for matching devices.</param>
    /// <returns>The matching <see cref="IBluetoothDevice" />, or null if none are found.</returns>
    IBluetoothDevice? GetDeviceOrDefault(Func<IBluetoothDevice, bool> filter);

    /// <summary>
    /// Returns a Bluetooth device with the specified ID.
    /// </summary>
    /// <param name="id">The ID of the device to retrieve.</param>
    /// <returns>The matching <see cref="IBluetoothDevice" />, or null if none are found.</returns>
    IBluetoothDevice? GetDeviceOrDefault(string id);

    /// <summary>
    /// Returns all Bluetooth devices that match the specified filter.
    /// </summary>
    /// <param name="filter">An optional function to filter devices. Defaults to null for all devices.</param>
    /// <returns>A collection of <see cref="IBluetoothDevice" /> that match the filter.</returns>
    IEnumerable<IBluetoothDevice> GetDevices(Func<IBluetoothDevice, bool>? filter = null);

    /// <summary>
    /// Gets a Bluetooth device that matches the specified filter or waits for it to appear if not already available.
    /// </summary>
    /// <param name="filter">An optional function to filter devices. Defaults to null for all devices.</param>
    /// <param name="timeout">The timeout for this operation</param>
    /// <param name="cancellationToken">A cancellation token to cancel this operation.</param>
    /// <returns>The <see cref="IBluetoothDevice" /> when it appears.</returns>
    ValueTask<IBluetoothDevice> GetDeviceOrWaitForDeviceToAppearAsync(Func<IBluetoothDevice, bool>? filter = null, TimeSpan? timeout = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a Bluetooth device with the specified ID or waits for it to appear if not already available.
    /// </summary>
    /// <param name="id">The ID of the device to retrieve.</param>
    /// <param name="timeout">The timeout for this operation</param>
    /// <param name="cancellationToken">A cancellation token to cancel this operation.</param>
    /// <returns>The <see cref="IBluetoothDevice" /> when it appears.</returns>
    ValueTask<IBluetoothDevice> GetDeviceOrWaitForDeviceToAppearAsync(string id, TimeSpan? timeout = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Waits for a Bluetooth device with the specified ID to appear or returns it if already available.
    /// </summary>
    /// <param name="id">The ID of the device to wait for.</param>
    /// <param name="timeout">The timeout for this operation</param>
    /// <param name="cancellationToken">A cancellation token to cancel this operation.</param>
    /// <returns>The <see cref="IBluetoothDevice" /> when it appears.</returns>
    ValueTask<IBluetoothDevice> WaitForDeviceToAppearAsync(string id, TimeSpan? timeout = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Waits for the first Bluetooth device that matches the specified filter to appear.
    /// </summary>
    /// <param name="filter">A function to filter devices. Should return true for matching devices.</param>
    /// <param name="timeout">The timeout for this operation</param>
    /// <param name="cancellationToken">A cancellation token to cancel this operation.</param>
    /// <returns>The <see cref="IBluetoothDevice" /> that matches the filter when it appears.</returns>
    ValueTask<IBluetoothDevice> WaitForDeviceToAppearAsync(Func<IBluetoothDevice, bool>? filter = null, TimeSpan? timeout = null, CancellationToken cancellationToken = default);

    #endregion
}
