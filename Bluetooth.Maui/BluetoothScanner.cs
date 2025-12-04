namespace Bluetooth.Maui;

/// <summary>
/// Provides factory methods for creating and managing the default Bluetooth scanner instance.
/// </summary>
public partial class BluetoothScanner
{
    private static BluetoothScanner? _defaultInstance;

    /// <summary>
    /// Gets or creates the default singleton Bluetooth scanner instance.
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the initialized <see cref="IBluetoothScanner"/> instance.
    /// </returns>
    /// <remarks>
    /// <para>
    /// This method creates a new scanner instance if one doesn't exist, or returns the existing instance.
    /// The scanner is initialized asynchronously, which may include platform-specific adapter initialization
    /// and permission checks.
    /// </para>
    /// <para>
    /// For MAUI applications using dependency injection, consider using the <c>UseBluetoothScanner()</c>
    /// extension method in MauiProgram.cs instead.
    /// </para>
    /// <para>
    /// Example usage:
    /// <code>
    /// var scanner = await BluetoothScanner.GetOrCreateDefaultScannerAsync();
    /// await scanner.StartAsync();
    /// </code>
    /// </para>
    /// </remarks>
    public static async ValueTask<IBluetoothScanner> GetOrCreateDefaultScannerAsync()
    {
        _defaultInstance = new BluetoothScanner();
        await _defaultInstance.InitializeAsync().ConfigureAwait(false);
        return _defaultInstance;
    }
}
