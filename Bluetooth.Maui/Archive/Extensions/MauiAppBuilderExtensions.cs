namespace Bluetooth.Maui.Extensions;

/// <summary>
/// Extension methods for configuring Bluetooth services in a MAUI application.
/// </summary>
public static class MauiAppBuilderExtensions
{
    /// <summary>
    /// Registers Bluetooth scanner and broadcaster services with the MAUI dependency injection container.
    /// Services are registered as singletons.
    /// </summary>
    /// <param name="builder">The <see cref="MauiAppBuilder"/> to configure.</param>
    /// <returns>The <see cref="MauiAppBuilder"/> for method chaining.</returns>
    /// <remarks>
    /// <para>
    /// Example usage in MauiProgram.cs:
    /// <code>
    /// public static MauiApp CreateMauiApp()
    /// {
    ///     var builder = MauiApp.CreateBuilder();
    ///     builder.UseBluetooth();
    ///     return builder.Build();
    /// }
    /// </code>
    /// </para>
    /// <para>
    /// Then inject in your pages/view models:
    /// <code>
    /// public MainPage(IBluetoothScanner scanner, IBluetoothBroadcaster broadcaster)
    /// {
    ///     _scanner = scanner;
    ///     _broadcaster = broadcaster;
    /// }
    /// </code>
    /// </para>
    /// </remarks>
    public static MauiAppBuilder UseBluetooth(
        this MauiAppBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);
        builder.Services.AddBluetoothServices();
        return builder;
    }
}
