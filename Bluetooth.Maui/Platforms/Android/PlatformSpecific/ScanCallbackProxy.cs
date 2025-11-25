using Exception = System.Exception;

namespace Bluetooth.Maui.PlatformSpecific;

/// <summary>
/// Android Bluetooth LE scan callback proxy that handles scan events.
/// Implements <see cref="ScanCallback"/> to redirect events to the scanner instance.
/// </summary>
/// <remarks>
/// This class wraps the Android ScanCallback and provides exception handling
/// for all callback methods. See Android documentation:
/// https://developer.android.com/reference/android/bluetooth/le/ScanCallback
/// </remarks>
public partial class ScanCallbackProxy : ScanCallback
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ScanCallbackProxy"/> class.
    /// </summary>
    /// <param name="scanCallbackProxyDelegate">The scanner instance that will receive the callback events.</param>
    public ScanCallbackProxy(IScanner scanCallbackProxyDelegate)
    {
        ScanCallbackProxyDelegate = scanCallbackProxyDelegate;
    }

    /// <summary>
    /// Gets the scanner instance that receives callback events.
    /// </summary>
    public IScanner ScanCallbackProxyDelegate { get; }

    /// <inheritdoc cref="ScanCallback.OnScanFailed(ScanFailure)"/>
    public override void OnScanFailed(ScanFailure errorCode)
    {
        try
        {
            ScanCallbackProxyDelegate.OnScanFailed(errorCode);
        }
        catch (Exception e)
        {
            BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, e);
        }
    }

    /// <inheritdoc cref="ScanCallback.OnBatchScanResults(IList{ScanResult})"/>
    public override void OnBatchScanResults(IList<ScanResult>? results)
    {
        try
        {
            if (results == null || results.Count == 0)
            {
                return; // No results to process
            }

            ScanCallbackProxyDelegate.OnScanResult(ScanCallbackType.AllMatches, results);
        }
        catch (Exception e)
        {
            BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, e);
        }
    }

    /// <inheritdoc cref="ScanCallback.OnScanResult(ScanCallbackType, ScanResult)"/>
    public override void OnScanResult(ScanCallbackType callbackType, ScanResult? result)
    {
        try
        {
            if (result == null)
            {
                return; // No result to process
            }

            ScanCallbackProxyDelegate.OnScanResult(callbackType, result);
        }
        catch (Exception e)
        {
            BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, e);
        }
    }
}
