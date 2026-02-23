using Bluetooth.Maui.Platforms.Droid.Tools;

namespace Bluetooth.Maui.Platforms.Droid.Scanning.Factories;

/// <inheritdoc />
public record AndroidBluetoothServiceFactoryRequest : IBluetoothServiceFactory.BluetoothServiceFactoryRequest
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="AndroidBluetoothServiceFactoryRequest" /> class with the specified Android GATT service.
    /// </summary>
    /// <param name="nativeService">The native Android GATT service from which to create the factory request.</param>
    public AndroidBluetoothServiceFactoryRequest(BluetoothGattService nativeService)
        : base(nativeService?.Uuid?.ToGuid() ?? throw new ArgumentNullException(nameof(nativeService)))
    {
        NativeService = nativeService;
    }

    /// <summary>
    ///     Gets the native Android GATT service.
    /// </summary>
    public BluetoothGattService NativeService { get; init; }
}
