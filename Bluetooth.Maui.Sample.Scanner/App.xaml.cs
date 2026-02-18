using Plugin.ExceptionListeners;

namespace Bluetooth.Maui.Sample.Scanner;

/// <summary>
/// Main application class for the Bluetooth Scanner sample.
/// </summary>
public partial class App : Application
{
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="App"/> class.
    /// </summary>
    /// <param name="serviceProvider">The service provider for resolving dependencies.</param>
    public App(IServiceProvider serviceProvider)
    {
        InitializeComponent();
        _serviceProvider = serviceProvider;

        // Setup global exception handling
        SetupExceptionHandlers();

        // Set main page with navigation
        var scannerPage = _serviceProvider.GetRequiredService<ScannerPage>();
        MainPage = new NavigationPage(scannerPage);
    }

    /// <summary>
    /// Sets up global exception handlers for the application.
    /// </summary>
    private void SetupExceptionHandlers()
    {
        // TODO: Setup global exception listeners after Plugin.ExceptionListeners is properly configured
        // BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException += OnBluetoothException;
    }

    /// <summary>
    /// Handles Bluetooth exceptions globally.
    /// </summary>
    private void OnBluetoothException(object? sender, Exception exception)
    {
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            if (MainPage != null)
            {
                await MainPage.DisplayAlert("Bluetooth Error", exception.Message, "OK");
            }
        });
    }
}
