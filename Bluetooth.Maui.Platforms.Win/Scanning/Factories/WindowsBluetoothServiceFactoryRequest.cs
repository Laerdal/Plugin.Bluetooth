using System.Diagnostics.CodeAnalysis;

namespace Bluetooth.Maui.Platforms.Win.Scanning.Factories;

/// <summary>
///     Windows-specific implementation of the Bluetooth service factory request.
/// </summary>
public record WindowsBluetoothServiceFactoryRequest : IBluetoothServiceFactory.BluetoothServiceFactoryRequest
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="WindowsBluetoothServiceFactoryRequest" /> record.
    /// </summary>
    /// <param name="nativeService">The native Windows GATT device service.</param>
    public WindowsBluetoothServiceFactoryRequest(GattDeviceService nativeService)
        : base(nativeService?.Uuid ?? throw new ArgumentNullException(nameof(nativeService)))
    {
        ArgumentNullException.ThrowIfNull(nativeService);
        NativeService = nativeService;
    }

    /// <summary>
    ///     Gets the native Windows GATT device service.
    /// </summary>
    public GattDeviceService NativeService { get; }
}
