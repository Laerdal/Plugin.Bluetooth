namespace Bluetooth.Maui.PlatformSpecific;

public interface IAdvertiseCallbackProxy
{
    /// <summary>
    /// Gets the broadcaster instance that receives callback events.
    /// </summary>
    AdvertiseCallbackProxy.IBroadcaster Broadcaster { get; }

    /// <inheritdoc cref="AdvertiseCallback.OnStartSuccess(AdvertiseSettings)"/>
    void OnStartSuccess(AdvertiseSettings? settingsInEffect);

    /// <inheritdoc cref="AdvertiseCallback.OnStartFailure(AdvertiseFailure)"/>
    void OnStartFailure(AdvertiseFailure errorCode);
}
