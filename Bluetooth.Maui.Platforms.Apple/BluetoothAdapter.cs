using Bluetooth.Core.Infrastructure.Scheduling;
using Bluetooth.Maui.Platforms.Apple.Broadcasting;
using Bluetooth.Maui.Platforms.Apple.Broadcasting.NativeObjects;
using Bluetooth.Maui.Platforms.Apple.Scanning;
using Bluetooth.Maui.Platforms.Apple.Scanning.NativeObjects;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Bluetooth.Maui.Platforms.Apple;

/// <summary>
/// iOS implementation of the Bluetooth adapter using Core Bluetooth framework.
/// </summary>
public class BluetoothAdapter : BaseBluetoothAdapter
{
    /// <inheritdoc/>
    public BluetoothAdapter(ILogger<IBluetoothAdapter>? logger = null) : base(logger)
    {
    }
}
