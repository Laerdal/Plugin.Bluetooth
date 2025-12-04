namespace Bluetooth.Maui;

/// <summary>
/// Provides factory methods for creating and managing the default Bluetooth broadcaster instance.
/// </summary>
public partial class BluetoothBroadcaster
{
    private static BluetoothBroadcaster? _defaultInstance;

    /// <summary>
    /// Gets or creates the default singleton Bluetooth broadcaster instance.
    /// </summary>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// The task result contains the initialized <see cref="IBluetoothBroadcaster"/> instance.
    /// </returns>
    /// <remarks>
    /// <para>
    /// This method creates a new broadcaster instance if one doesn't exist, or returns the existing instance.
    /// The broadcaster is initialized asynchronously, which may include platform-specific adapter initialization
    /// and permission checks.
    /// </para>
    /// <para>
    /// For MAUI applications using dependency injection, consider using the <c>UseBluetoothBroadcaster()</c>
    /// extension method in MauiProgram.cs instead.
    /// </para>
    /// <para>
    /// <strong>Note:</strong> Broadcasting (peripheral mode) is currently only fully implemented on Android.
    /// iOS and Windows implementations throw <see cref="NotImplementedException"/>.
    /// </para>
    /// <para>
    /// Example usage:
    /// <code>
    /// var broadcaster = await BluetoothBroadcaster.GetOrCreateDefaultBroadcasterAsync();
    /// await broadcaster.StartAsync();
    /// </code>
    /// </para>
    /// </remarks>
    public static async ValueTask<IBluetoothBroadcaster> GetOrCreateDefaultBroadcasterAsync()
    {
        _defaultInstance = new BluetoothBroadcaster();
        await _defaultInstance.InitializeAsync().ConfigureAwait(false);
        return _defaultInstance;
    }
}
