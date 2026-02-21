namespace Bluetooth.Core;

/// <summary>
///     Base class for Bluetooth adapters.
/// </summary>
public abstract class BaseBluetoothAdapter : BaseBindableObject, IBluetoothAdapter
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="BaseBluetoothAdapter" /> class.
    /// </summary>
    /// <param name="logger">The logger instance for logging adapter operations.</param>
    protected BaseBluetoothAdapter(ILogger<IBluetoothAdapter>? logger = null) : base(logger)
    {
    }
}