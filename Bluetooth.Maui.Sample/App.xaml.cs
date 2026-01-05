using System.Diagnostics;
using System.Net.Sockets;

using Bluetooth.Core.Exceptions;
using Bluetooth.Maui.Sample.Services;
using Bluetooth.Maui.Sample.Views;

using Microsoft.Extensions.DependencyInjection;

using Plugin.ExceptionListeners;
using Plugin.ExceptionListeners.Listeners;
using Plugin.ExceptionListeners.Maui;

namespace Bluetooth.Maui.Sample;

public partial class App : Application
{
    public INavigationService NavigationService { get; }

    private readonly NativeUnhandledExceptionListener _nativeUnhandledExceptionListener;

    private readonly CurrentDomainUnhandledExceptionListener _currentDomainUnhandledExceptionListener;

    private readonly CurrentDomainFirstChanceExceptionListener _currentDomainFirstChanceExceptionListener;

    private readonly TaskSchedulerUnobservedTaskExceptionListener _taskSchedulerUnobservedTaskExceptionListener; // ReSharper restore NotAccessedField.Local

    private readonly BluetoothUnhandledExceptionListener _bluetoothGattUnhandledExceptionListener;

    public App(INavigationService navigationService)
    {
        _nativeUnhandledExceptionListener = new NativeUnhandledExceptionListener(OnNativeUnhandled);
        _currentDomainUnhandledExceptionListener = new CurrentDomainUnhandledExceptionListener(OnCurrentDomainUnhandled);
        _currentDomainFirstChanceExceptionListener = new CurrentDomainFirstChanceExceptionListener(OnCurrentDomainFirstChance); //very noisy
        _taskSchedulerUnobservedTaskExceptionListener = new TaskSchedulerUnobservedTaskExceptionListener(OnTaskSchedulerUnobservedTask);
        _bluetoothGattUnhandledExceptionListener = new BluetoothUnhandledExceptionListener(OnBluetoothUnhandled);

        NavigationService = navigationService;

        InitializeComponent();
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        return NavigationService.CreateWindow(activationState);
    }

    public static void DisplayAlert(Exception ex)
    {
        DisplayAlert($"{ex.GetType().Name}", $"{ex.Message}\nFull Details:{ex}\n{ex.StackTrace}");
    }

    public static void DisplayAlert(string title, string message)
    {
        Debug.WriteLine($"{title}: {message}");
        MainThread.InvokeOnMainThreadAsync(() => Current?.Windows[0].Page?.DisplayAlertAsync(title, cancel: "Ok", message: message));
    }

    private void OnBluetoothUnhandled(object? sender, ExceptionEventArgs e)
    {
        DisplayAlert("BluetoothUnhandledException", $"{e.Exception.Message}\nFull Details:{e.Exception}\n{e.Exception.StackTrace}");
    }

    private static void OnTaskSchedulerUnobservedTask(object? sender, ExceptionEventArgs e)
    {
        if (Debugger.IsAttached && e.Exception is SocketException) // known issue in debugger   can be ignored
        {
            return;
        }

        DisplayAlert("TaskSchedulerUnobservedTaskException", $"{e.Exception.Message}\nFull Details:{e.Exception}\n{e.Exception.StackTrace}");
    }

    private static void OnNativeUnhandled(object? sender, ExceptionEventArgs e)
    {
        DisplayAlert("NativeUnhandledException", $"{e.Exception.Message}\nFull Details:{e.Exception}\n{e.Exception.StackTrace}");
    }

    private static void OnCurrentDomainUnhandled(object? sender, ExceptionEventArgs e)
    {
        DisplayAlert("CurrentDomainUnhandledException", $"{e.Exception.Message}\nFull Details:{e.Exception}\n{e.Exception.StackTrace}");
    }

    private static void OnCurrentDomainFirstChance(object? sender, ExceptionEventArgs e)
    {
        /*
        if (Debugger.IsAttached && ex is FileNotFoundException) // known issue in debugger   can be ignored
            return;
        if (Debugger.IsAttached && ex is MissingMethodException) // known issue in debugger   can be ignored
            return;
        if (Debugger.IsAttached && ex is SocketException) // known issue in debugger   can be ignored
            return;*/

        //DisplayAlert("CurrentDomainFirstChanceException", $"{ex.Message}\nFull Details:{ex}\n{ex.StackTrace}");
        //Logger.Info($"CurrentDomainFirstChanceException: {ex.Message}\nFull Details:{ex}\n{ex.StackTrace}");
    }
}
