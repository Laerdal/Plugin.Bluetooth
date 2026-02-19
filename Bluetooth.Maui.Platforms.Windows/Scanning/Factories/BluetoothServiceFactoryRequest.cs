using System.Diagnostics.CodeAnalysis;

namespace Bluetooth.Maui.Platforms.Windows.Scanning.Factories;

/// <summary>
/// Windows-specific implementation of the Bluetooth service factory request.
/// </summary>
public record BluetoothServiceFactoryRequest : IBluetoothServiceFactory.BluetoothServiceFactoryRequest
{
    /// <summary>
    /// Gets the native Windows GATT device service.
    /// </summary>
    public GattDeviceService NativeService { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="BluetoothServiceFactoryRequest"/> record.
    /// </summary>
    /// <param name="nativeService">The native Windows GATT device service.</param>
    public BluetoothServiceFactoryRequest([NotNull]GattDeviceService nativeService)
        : base(nativeService.Uuid)
    {
        ArgumentNullException.ThrowIfNull(nativeService);
        NativeService = nativeService;
    }
}
