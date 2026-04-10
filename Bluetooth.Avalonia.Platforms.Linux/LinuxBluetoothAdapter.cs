namespace Bluetooth.Avalonia.Platforms.Linux;

/// <summary>
///     Linux implementation of the Bluetooth adapter, backed by BlueZ via D-Bus.
/// </summary>
/// <remarks>
///     On first use the adapter lazily discovers the first <c>org.bluez.Adapter1</c> object
///     (typically at <c>/org/bluez/hci0</c>) through the system D-Bus ObjectManager.
///     All platform implementations share the <see cref="DBusConnection.System" /> singleton.
/// </remarks>
public class LinuxBluetoothAdapter : BaseBluetoothAdapter, IAsyncDisposable
{
    private string? _adapterPath;
    private readonly SemaphoreSlim _initSemaphore = new SemaphoreSlim(1, 1);

    /// <inheritdoc />
    public LinuxBluetoothAdapter(ILogger<IBluetoothAdapter>? logger = null) : base(logger)
    {
    }

    /// <summary>
    ///     Gets the D-Bus connection to the system bus.
    ///     All BlueZ operations share this connection.
    /// </summary>
#pragma warning disable CA1822
    internal DBusConnection Connection => DBusConnection.System;
#pragma warning restore CA1822

    /// <summary>
    ///     Returns the cached adapter path without triggering discovery.
    ///     Returns <see langword="null" /> if the adapter path has not yet been discovered.
    /// </summary>
    internal string? CachedAdapterPath => _adapterPath;

    /// <summary>
    ///     Returns the D-Bus object path of the first available Bluetooth adapter
    ///     (e.g. <c>/org/bluez/hci0</c>), discovering it lazily on the first call.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    ///     Thrown when BlueZ is not running or no adapter is available.
    /// </exception>
    internal async ValueTask<string> GetAdapterPathAsync(CancellationToken cancellationToken = default)
    {
        if (_adapterPath != null)
        {
            return _adapterPath;
        }

        await _initSemaphore.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            if (_adapterPath != null)
            {
                return _adapterPath;
            }

            var objects = await BlueZObjectManager
                .GetManagedObjectsAsync(Connection, cancellationToken)
                .ConfigureAwait(false);

            _adapterPath = objects
                               .FirstOrDefault(o => o.HasInterface(BlueZConstants.Adapter1Interface))
                               ?.Path
                           ?? throw new InvalidOperationException(
                               "No Bluetooth adapter found. Ensure BlueZ is running and the adapter is enabled.");

            return _adapterPath;
        }
        finally
        {
            _initSemaphore.Release();
        }
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        _initSemaphore.Dispose();
        GC.SuppressFinalize(this);
        await ValueTask.CompletedTask.ConfigureAwait(false);
    }
}

