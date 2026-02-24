using Bluetooth.Maui.Platforms.Droid.Tools;

namespace Bluetooth.Maui.Platforms.Droid.Scanning.Factories;

/// <inheritdoc />
public record AndroidBluetoothRemoteServiceFactorySpec : IBluetoothRemoteServiceFactory.BluetoothRemoteServiceFactorySpec
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="AndroidBluetoothRemoteServiceFactorySpec" /> class with the specified Android GATT service.
    /// </summary>
    /// <param name="nativeService">The native Android GATT service from which to create the factory spec.</param>
    public AndroidBluetoothRemoteServiceFactorySpec(BluetoothGattService nativeService)
        : base(nativeService?.Uuid?.ToGuid() ?? throw new ArgumentNullException(nameof(nativeService)))
    {
        NativeService = nativeService;
    }

    /// <summary>
    ///     Gets the native Android GATT service.
    /// </summary>
    public BluetoothGattService NativeService { get; init; }
}
