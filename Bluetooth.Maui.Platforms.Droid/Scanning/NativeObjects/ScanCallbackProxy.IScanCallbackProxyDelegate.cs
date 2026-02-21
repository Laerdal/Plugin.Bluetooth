namespace Bluetooth.Maui.Platforms.Droid.Scanning.NativeObjects;

// Mapping native APIs leads to unclean interfaces, ignoring warnings here
#pragma warning disable CA1711 // Identifiers should not have incorrect suffix
#pragma warning disable CA1716 // Identifiers should not match keywords

public partial class ScanCallbackProxy
{
    /// <summary>
    ///     Interface for handling Bluetooth LE scan operations and callbacks.
    ///     Extends the base scanner interface with Android-specific callback methods.
    /// </summary>
    public interface IScanCallbackProxyDelegate
    {
        /// <summary>
        ///     Called when a scan operation has failed.
        /// </summary>
        /// <param name="errorCode">The error code indicating why the scan failed.</param>
        void OnScanFailed(ScanFailure errorCode);

        /// <summary>
        ///     Called when scan results are received as a batch.
        /// </summary>
        /// <param name="callbackType">The type of callback that triggered the results.</param>
        /// <param name="results">The collection of scan results.</param>
        void OnScanResult(ScanCallbackType callbackType, IEnumerable<ScanResult> results);

        /// <summary>
        ///     Called when a single scan result is received.
        /// </summary>
        /// <param name="callbackType">The type of callback that triggered the result.</param>
        /// <param name="result">The scan result.</param>
        void OnScanResult(ScanCallbackType callbackType, ScanResult result);
    }
}

#pragma warning restore CA1711 // Identifiers should not have incorrect suffix
#pragma warning restore CA1716 // Identifiers should not match keywords