namespace Bluetooth.Abstractions.Scanning;

public partial interface IBluetoothRemoteDevice
{
    /// <summary>
    ///     Gets the innermost platform-specific device this instance ultimately wraps.
    /// </summary>
    /// <remarks>
    ///     A scanner's <c>DeviceWrapper</c> hook lets client code substitute a custom subtype (e.g. by
    ///     inheriting <c>Bluetooth.Maui.BluetoothRemoteDevice</c>) for the raw platform device added to
    ///     the registry. Native platform code that needs to resolve a native callback (e.g. a
    ///     CoreBluetooth or GATT delegate callback) back to the concrete implementation it can invoke
    ///     methods on should use this property rather than casting the device directly - casting a
    ///     wrapped device to a platform-specific type always fails, since the wrapper does not inherit
    ///     from it. The default implementation returns <see langword="this"/>, so unwrapped platform
    ///     devices need no override; wrapper types must override this to delegate to the device they wrap.
    /// </remarks>
    IBluetoothRemoteDevice UnderlyingPlatformDevice => this;
}
