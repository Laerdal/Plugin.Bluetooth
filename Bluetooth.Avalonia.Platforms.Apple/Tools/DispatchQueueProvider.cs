namespace Bluetooth.Avalonia.Platforms.Apple.Tools;

/// <summary>
///     Default implementation of <see cref="IDispatchQueueProvider"/> that creates named serial queues.
/// </summary>
public class DispatchQueueProvider : IDispatchQueueProvider
{
    /// <inheritdoc />
    public DispatchQueue CentralQueue { get; }

    /// <inheritdoc />
    public DispatchQueue PeripheralQueue { get; }

    /// <summary>
    ///     Initializes a new instance of the <see cref="DispatchQueueProvider"/> class.
    /// </summary>
    /// <param name="options">The options containing queue label names.</param>
    public DispatchQueueProvider(IOptions<DispatchQueueProviderOptions> options)
    {
        ArgumentNullException.ThrowIfNull(options);

        CentralQueue = new DispatchQueue(options.Value.CentralQueueLabel);
        PeripheralQueue = new DispatchQueue(options.Value.PeripheralQueueLabel);
    }
}
