using Bluetooth.Maui.Platforms.Apple.Scanning.NativeObjects;

namespace Bluetooth.Maui.Platforms.Apple.Scanning.Factories;

/// <summary>
///     Apple-specific factory spec for creating <see cref="AppleBluetoothRemoteService" /> instances.
///     Extends the base spec with the native <see cref="CBService" /> required for CoreBluetooth.
/// </summary>
public record AppleBluetoothRemoteServiceFactorySpec(Guid ServiceId, CBService CbService)
    : IBluetoothRemoteServiceFactory.BluetoothRemoteServiceFactorySpec(ServiceId)
{
    /// <summary>
    ///     Initializes a new instance from a native Core Bluetooth service.
    /// </summary>
    /// <param name="cbService">The native iOS Core Bluetooth service.</param>
    public AppleBluetoothRemoteServiceFactorySpec(CBService cbService)
        : this((cbService ?? throw new ArgumentNullException(nameof(cbService))).UUID.ToGuid(), cbService)
    {
    }
}
