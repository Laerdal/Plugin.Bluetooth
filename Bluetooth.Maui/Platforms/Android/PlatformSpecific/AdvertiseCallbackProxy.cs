using Exception = System.Exception;

namespace Bluetooth.Maui.PlatformSpecific;

/// <summary>
/// Android Bluetooth LE advertise callback proxy that handles advertising events.
/// Implements <see cref="AdvertiseCallback"/> to redirect events to the broadcaster instance.
/// </summary>
/// <remarks>
/// This class wraps the Android AdvertiseCallback and provides exception handling
/// for all callback methods. See Android documentation:
/// https://developer.android.com/reference/android/bluetooth/le/AdvertiseCallback
/// </remarks>
public partial class AdvertiseCallbackProxy : AdvertiseCallback
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AdvertiseCallbackProxy"/> class.
    /// </summary>
    /// <param name="broadcaster">The broadcaster instance that will receive the callback events.</param>
    public AdvertiseCallbackProxy(IBroadcaster broadcaster)
    {
        Broadcaster = broadcaster;
    }

    /// <summary>
    /// Gets the broadcaster instance that receives callback events.
    /// </summary>
    public IBroadcaster Broadcaster { get; }

    /// <inheritdoc cref="AdvertiseCallback.OnStartSuccess(AdvertiseSettings)"/>
    public override void OnStartSuccess(AdvertiseSettings? settingsInEffect)
    {
        try
        {
            Broadcaster.OnStartSuccess(settingsInEffect);
        }
        catch (Exception e)
        {
            BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, e);
        }
    }

    /// <inheritdoc cref="AdvertiseCallback.OnStartFailure(AdvertiseFailure)"/>
    public override void OnStartFailure(AdvertiseFailure errorCode)
    {
        try
        {
            Broadcaster.OnStartFailure(errorCode);
        }
        catch (Exception e)
        {
            BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, e);
        }
    }
}

