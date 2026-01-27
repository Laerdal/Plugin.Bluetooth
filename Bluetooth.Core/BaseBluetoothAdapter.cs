using Bluetooth.Abstractions;
using Bluetooth.Abstractions.Scanning.EventArgs;
using Bluetooth.Core.Exceptions;
using Bluetooth.Core.Infrastructure.Scheduling;

using Microsoft.Extensions.Logging;

namespace Bluetooth.Core;

/// <summary>
/// Base class for Bluetooth adapters.
/// </summary>
public abstract class BaseBluetoothAdapter : BaseBindableObject, IBluetoothAdapter
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BaseBluetoothAdapter"/> class.
    /// </summary>
    /// <param name="logger">Optional logger instance for tracking property changes.</param>
    protected BaseBluetoothAdapter(ILogger? logger = null)
        : base(logger)
    {
    }
}
