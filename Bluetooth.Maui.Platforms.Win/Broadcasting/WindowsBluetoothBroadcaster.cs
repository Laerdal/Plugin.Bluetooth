using System.Runtime.InteropServices;

using Bluetooth.Maui.Platforms.Win.Broadcasting.NativeObjects;
using Bluetooth.Maui.Platforms.Win.Exceptions;

namespace Bluetooth.Maui.Platforms.Win.Broadcasting;

/// <inheritdoc />
public class WindowsBluetoothBroadcaster : BaseBluetoothBroadcaster,
                                         BluetoothLeAdvertisementPublisherWrapper.IBluetoothLeAdvertisementPublisherProxyDelegate
{
    private readonly ITicker _ticker;

    private BluetoothLeAdvertisementPublisherWrapper? _publisherWrapper;

    /// <summary>
    ///     Initializes a new instance of the <see cref="WindowsBluetoothBroadcaster" /> class.
    /// </summary>
    /// <param name="adapter">The Bluetooth adapter associated with this broadcaster.</param>
    /// <param name="ticker">The ticker for scheduling periodic refresh tasks.</param>
    /// <param name="loggerFactory">An optional logger factory for creating loggers used by this broadcaster and its components.</param>
    public WindowsBluetoothBroadcaster(IBluetoothAdapter adapter, ITicker ticker, ILoggerFactory? loggerFactory = null) : base(adapter, ticker, loggerFactory)
    {
        _ticker = ticker;
    }

    private BluetoothLeAdvertisementPublisherWrapper PublisherWrapper =>
        _publisherWrapper ??= new BluetoothLeAdvertisementPublisherWrapper(this, _ticker);

    /// <inheritdoc />
    public void OnAdvertisementPublisherStatusChanged(BluetoothLEAdvertisementPublisherStatus status,
        BluetoothError errorCode,
        short? selectedTransmitPowerLevelInDBm = null)
    {
        if (errorCode != BluetoothError.Success)
        {
            var exception = new WindowsNativeBluetoothErrorException(errorCode);
            if (IsStarting)
            {
                OnStartFailed(exception);
            }
            else if (IsStopping)
            {
                OnStopFailed(exception);
            }
            else
            {
                BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, exception);
            }

            return;
        }

        IsRunning = status is BluetoothLEAdvertisementPublisherStatus.Started or BluetoothLEAdvertisementPublisherStatus.Waiting;
    }

    /// <inheritdoc />
    protected override void NativeRefreshIsRunning()
    {
        IsRunning = _publisherWrapper?.Status is BluetoothLEAdvertisementPublisherStatus.Started or BluetoothLEAdvertisementPublisherStatus.Waiting;
    }

    /// <inheritdoc />
    protected override ValueTask NativeStartAsync(BroadcastingOptions options, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(options);
        cancellationToken.ThrowIfCancellationRequested();

        try
        {
            var publisher = PublisherWrapper.BluetoothLeAdvertisementPublisher;

            var serviceUuids = publisher.Advertisement.ServiceUuids;
            serviceUuids.Clear();
            foreach (var advertisedServiceUuid in options.AdvertisedServiceUuids ?? [])
            {
                serviceUuids.Add(advertisedServiceUuid);
            }

            if (options.IncludeDeviceName && !string.IsNullOrWhiteSpace(options.LocalDeviceName))
            {
                publisher.Advertisement.LocalName = options.LocalDeviceName;
            }

            var serviceAdvertisingParameters = new GattServiceProviderAdvertisingParameters
            {
                IsConnectable = true,
                IsDiscoverable = true
            };

            foreach (var localService in GetServices().OfType<WindowsBluetoothLocalService>())
            {
                localService.ServiceProvider.StartAdvertising(serviceAdvertisingParameters);
            }

            publisher.Start();
            return ValueTask.CompletedTask;
        }
        catch (COMException e)
        {
            const int eAccessDenied = unchecked((int) 0x80070005);
            if (e.HResult == eAccessDenied)
            {
                return ValueTask.FromException(new BluetoothPermissionException("Access denied when starting Windows Bluetooth advertising. Ensure 'bluetooth' capability is declared in Package.appxmanifest and Bluetooth radio is enabled.",
                    e));
            }

            return ValueTask.FromException(new WindowsNativeBluetoothException("Failed to start Windows Bluetooth LE advertisement publisher.", e));
        }
    }

    /// <inheritdoc />
    protected override ValueTask NativeStopAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        foreach (var localService in GetServices().OfType<WindowsBluetoothLocalService>())
        {
            localService.ServiceProvider.StopAdvertising();
        }

        _publisherWrapper?.BluetoothLeAdvertisementPublisher.Stop();
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc />
    protected override ValueTask<IBluetoothLocalService> NativeCreateServiceAsync(Guid id,
        string? name = null,
        bool isPrimary = true,
        TimeSpan? timeout = null,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return CreateServiceAsyncCore(id, name, isPrimary, cancellationToken);
    }

    private async ValueTask<IBluetoothLocalService> CreateServiceAsyncCore(Guid id,
        string? name,
        bool isPrimary,
        CancellationToken cancellationToken)
    {
        var result = await GattServiceProvider.CreateAsync(id).AsTask(cancellationToken).ConfigureAwait(false);
        WindowsNativeBluetoothErrorException.ThrowIfNotSuccess(result.Error);

        var service = new WindowsBluetoothLocalService(result.ServiceProvider,
                                                       result.ServiceProvider.Service,
                                                       this,
                                                       id,
                                                       name,
                                                       isPrimary);

        if (IsRunning)
        {
            service.ServiceProvider.StartAdvertising(new GattServiceProviderAdvertisingParameters
            {
                IsConnectable = true,
                IsDiscoverable = true
            });
        }

        return service;
    }

    internal WindowsBluetoothConnectedDevice GetOrCreateClientDevice(string? deviceId, GattSubscribedClient? nativeClient)
    {
        var resolvedId = !string.IsNullOrWhiteSpace(deviceId) ? deviceId : Guid.NewGuid().ToString("N");

        var existing = GetClientDeviceOrDefault(resolvedId) as WindowsBluetoothConnectedDevice;
        if (existing != null)
        {
            existing.SetNativeClient(nativeClient);
            return existing;
        }

#pragma warning disable CA2000 // Device lifetime is owned by broadcaster client-device registry
        var device = new WindowsBluetoothConnectedDevice(this, new IBluetoothConnectedDeviceFactory.BluetoothConnectedDeviceSpec(resolvedId));
#pragma warning restore CA2000
        device.SetNativeClient(nativeClient);
        AddClientDevice(device);
        return device;
    }

    #region Permission Methods

    /// <inheritdoc />
    /// <remarks>
    ///     On Windows, Bluetooth permissions are capability-based and granted at install time
    ///     if the 'bluetooth' capability is declared in Package.appxmanifest.
    ///     This method always returns true.
    /// </remarks>
    protected override ValueTask<bool> NativeHasBroadcasterPermissionsAsync()
    {
        // On Windows, Bluetooth permissions are capability-based and granted at install time
        return ValueTask.FromResult(true);
    }

    /// <inheritdoc />
    /// <remarks>
    ///     On Windows, no runtime permission spec is needed. Bluetooth permissions are
    ///     declared in Package.appxmanifest and granted at install time.
    /// </remarks>
    protected override ValueTask NativeRequestBroadcasterPermissionsAsync(CancellationToken cancellationToken)
    {
        // No runtime spec needed on Windows - permissions are declared at install time
        return ValueTask.CompletedTask;
    }

    #endregion

    /// <inheritdoc />
    protected override async ValueTask DisposeAsyncCore()
    {
        await base.DisposeAsyncCore().ConfigureAwait(false);
        _publisherWrapper?.Dispose();
        _publisherWrapper = null;
    }

}
