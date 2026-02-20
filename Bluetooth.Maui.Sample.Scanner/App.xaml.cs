namespace Bluetooth.Maui.Sample.Scanner;

/// <summary>
///     Main application class for the Bluetooth Scanner sample.
/// </summary>
public partial class App : Application
{
    private readonly BluetoothUnhandledExceptionListener _exceptionListener;
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    ///     Initializes a new instance of the <see cref="App" /> class.
    /// </summary>
    /// <param name="serviceProvider">The service provider for resolving dependencies.</param>
    public App(IServiceProvider serviceProvider)
    {
        InitializeComponent();
        _serviceProvider = serviceProvider;

        // Setup global exception handling
        _exceptionListener = SetupExceptionHandlers();
    }

    /// <summary>
    ///     Creates and configures the main application window.
    /// </summary>
    /// <param name="activationState">The activation state for the window.</param>
    /// <returns>A configured window with the main navigation page.</returns>
    protected override Window CreateWindow(IActivationState? activationState)
    {
        var scannerPage = _serviceProvider.GetRequiredService<ScannerPage>();
        var navigationPage = new NavigationPage(scannerPage);
        return new Window(navigationPage);
    }

    /// <summary>
    ///     Sets up global exception handlers for the application.
    /// </summary>
    /// <returns>The Bluetooth exception listener instance.</returns>
    private BluetoothUnhandledExceptionListener SetupExceptionHandlers()
    {
        // Setup Bluetooth exception listener
        return new BluetoothUnhandledExceptionListener((sender, args) => {
            OnBluetoothException(sender, args.Exception);
        });
    }

    /// <summary>
    ///     Handles Bluetooth exceptions globally.
    /// </summary>
    private void OnBluetoothException(object? sender, Exception exception)
    {
        MainThread.BeginInvokeOnMainThread(async () => {
            var mainPage = Windows.FirstOrDefault()?.Page;
            if (mainPage != null)
            {
                await mainPage.DisplayAlertAsync("Bluetooth Error", exception.Message, "OK");
            }
        });
    }
}