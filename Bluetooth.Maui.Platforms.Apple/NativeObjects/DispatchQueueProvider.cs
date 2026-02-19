namespace Bluetooth.Maui.Platforms.Apple.NativeObjects;

/// <summary>
/// Implementation of IDispatchQueueProvider that provides DispatchQueue instances for CoreBluetooth managers.
/// </summary>
public class DispatchQueueProvider : IDispatchQueueProvider
{
    private readonly string _centralQueueLabel;
    private readonly string _peripheralQueueLabel;
    
    /// Initializes a new instance of the DispatchQueueProvider class with the specified options.
    public DispatchQueueProvider(IOptions<DispatchQueueProviderOptions> options)
    {
        _centralQueueLabel = options?.Value.CentralQueueLabel ?? "com.bluetooth.maui.central";
        _peripheralQueueLabel = options?.Value.PeripheralQueueLabel ?? "com.bluetooth.maui.peripheral";
    }
    
    /// <inheritdoc />
    public DispatchQueue GetCbPeripheralManagerDispatchQueue()
    {
        return new DispatchQueue(_peripheralQueueLabel);
    }

    /// <inheritdoc />
    public DispatchQueue GetCbCentralManagerDispatchQueue()
    {
        return new DispatchQueue(_centralQueueLabel);
    }
}