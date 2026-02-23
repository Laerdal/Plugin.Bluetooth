using Bluetooth.Maui.Platforms.Win.Logging;
using Bluetooth.Maui.Platforms.Win.NativeObjects;

namespace Bluetooth.Maui.Platforms.Win;

/// <summary>
///     Windows implementation of the Bluetooth adapter using Windows.Devices.Bluetooth namespace.
/// </summary>
/// <remarks>
///     This adapter lazily initializes native object wrappers on demand using async initialization patterns.
///     Wrappers are not injected via DI as they require asynchronous initialization.
/// </remarks>
public partial class WindowsBluetoothAdapter : BaseBluetoothAdapter, IDisposable
{
    private readonly ITicker _ticker;

    // Lazy-initialized wrappers
    private BluetoothAdapterWrapper? _bluetoothAdapterWrapper;
    private RadioWrapper? _radioWrapper;

    // TaskCompletionSources for concurrent initialization handling
    private TaskCompletionSource<BluetoothAdapterWrapper>? _adapterInitializationTcs;
    private TaskCompletionSource<RadioWrapper>? _radioInitializationTcs;

    /// <summary>
    ///     Initializes a new instance of the <see cref="WindowsBluetoothAdapter" /> class.
    /// </summary>
    /// <param name="ticker">The ticker for scheduling periodic property updates in wrappers.</param>
    /// <param name="logger">An optional logger for logging adapter operations.</param>
    public WindowsBluetoothAdapter(ITicker ticker, ILogger<IBluetoothAdapter>? logger = null) : base(logger)
    {
        ArgumentNullException.ThrowIfNull(ticker);
        _ticker = ticker;
    }

    /// <summary>
    ///     Gets the Bluetooth adapter wrapper asynchronously, initializing it on first access.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the initialization operation.</param>
    /// <returns>A task that represents the asynchronous operation and contains the initialized adapter wrapper.</returns>
    /// <remarks>
    ///     This method uses TaskCompletionSource to ensure only one concurrent initialization occurs.
    ///     Subsequent calls will wait for the ongoing initialization or return the cached instance.
    /// </remarks>
    public async Task<BluetoothAdapterWrapper> GetBluetoothAdapterWrapperAsync(CancellationToken cancellationToken = default)
    {
        // Return cached instance if already initialized
        if (_bluetoothAdapterWrapper != null)
        {
            return _bluetoothAdapterWrapper;
        }

        // If initialization is in progress, wait for it
        if (_adapterInitializationTcs != null)
        {
            return await _adapterInitializationTcs.Task.ConfigureAwait(false);
        }

        // Start new initialization
        _adapterInitializationTcs = new TaskCompletionSource<BluetoothAdapterWrapper>();

        try
        {
            Logger?.LogAdapterWrapperInitializing();

            _bluetoothAdapterWrapper = new BluetoothAdapterWrapper(_ticker, Logger as ILogger<IBluetoothAdapterWrapper>);

            // Trigger native adapter initialization to validate it works
            await _bluetoothAdapterWrapper.GetBluetoothAdapterAsync(cancellationToken).ConfigureAwait(false);

            Logger?.LogAdapterWrapperInitialized();

            _adapterInitializationTcs.TrySetResult(_bluetoothAdapterWrapper);
            return _bluetoothAdapterWrapper;
        }
        catch (Exception ex)
        {
            Logger?.LogAdapterWrapperInitializationFailed(ex);
            _adapterInitializationTcs.TrySetException(ex);
            throw;
        }
        finally
        {
            _adapterInitializationTcs = null;
        }
    }

    /// <summary>
    ///     Gets the Bluetooth radio wrapper asynchronously, initializing it on first access.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the initialization operation.</param>
    /// <returns>A task that represents the asynchronous operation and contains the initialized radio wrapper.</returns>
    /// <remarks>
    ///     This method depends on the adapter wrapper being initialized first.
    ///     Uses TaskCompletionSource to ensure only one concurrent initialization occurs.
    /// </remarks>
    public async Task<RadioWrapper> GetRadioWrapperAsync(CancellationToken cancellationToken = default)
    {
        // Return cached instance if already initialized
        if (_radioWrapper != null)
        {
            return _radioWrapper;
        }

        // If initialization is in progress, wait for it
        if (_radioInitializationTcs != null)
        {
            return await _radioInitializationTcs.Task.ConfigureAwait(false);
        }

        // Start new initialization
        _radioInitializationTcs = new TaskCompletionSource<RadioWrapper>();

        try
        {
            Logger?.LogRadioWrapperInitializing();

            // Ensure adapter wrapper is initialized first
            var adapterWrapper = await GetBluetoothAdapterWrapperAsync(cancellationToken).ConfigureAwait(false);

            _radioWrapper = new RadioWrapper(adapterWrapper, _ticker, Logger as ILogger<IRadioWrapper>);

            // Trigger native radio initialization to validate it works
            await _radioWrapper.GetRadioAsync(cancellationToken).ConfigureAwait(false);

            Logger?.LogRadioWrapperInitialized();

            _radioInitializationTcs.TrySetResult(_radioWrapper);
            return _radioWrapper;
        }
        catch (Exception ex)
        {
            Logger?.LogRadioWrapperInitializationFailed(ex);
            _radioInitializationTcs.TrySetException(ex);
            throw;
        }
        finally
        {
            _radioInitializationTcs = null;
        }
    }

    /// <summary>
    ///     Disposes the adapter and its associated native wrappers.
    /// </summary>
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    ///     Disposes managed and unmanaged resources.
    /// </summary>
    /// <param name="disposing">True if called from Dispose(), false if called from finalizer.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            // Dispose managed resources
            _bluetoothAdapterWrapper?.Dispose();
            _radioWrapper?.Dispose();
        }
    }
}
