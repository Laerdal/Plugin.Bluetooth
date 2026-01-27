namespace Bluetooth.Maui.PlatformSpecific;

public sealed record BluetoothAppleOptions
{
    public CBCentralInitOptions CBCentralInitOptions { get; init; } = new CBCentralInitOptions()
    {
        ShowPowerAlert = true
    };

    public Func<DispatchQueue> GetCentralQueue { get; init; } = () => DispatchQueue.DefaultGlobalQueue;

    public CbPeripheralManagerOptions CBPeripheralManagerOptions { get; init; } = new CbPeripheralManagerOptions()
    {
        ShowPowerAlert = true
    };

    public Func<DispatchQueue> GetPeripheralQueue { get; init; } = () => DispatchQueue.DefaultGlobalQueue;
}
