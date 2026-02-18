namespace Bluetooth.Maui.Platforms.Apple.Scanning.Options;

/// <inheritdoc/>
public record ConnectionOptions : Abstractions.Scanning.Options.ConnectionOptions
{
    /// <summary>
    /// Gets or sets whether to notify on connection events.
    /// </summary>
    public bool? NotifyOnConnection { get; set; }

    /// <summary>
    /// Gets or sets whether to notify on disconnection events.
    /// </summary>
    public bool? NotifyOnDisconnection { get; set; }

    /// <summary>
    /// Gets or sets whether to notify on notification events.
    /// </summary>
    public bool? NotifyOnNotification { get; set; }

    /// <summary>
    /// Gets or sets whether to enable transport bridging.
    /// </summary>
    public bool? EnableTransportBridging { get; set; }

    /// <summary>
    /// Gets or sets whether to require ANCS (Apple Notification Center Service) support.
    /// </summary>
    public bool? RequiresAncs { get; set; }


    /// <summary>
    /// Implicitly converts CbPeripheralManagerOptions to NSDictionary.
    /// </summary>
    /// <param name="options">The options to convert.</param>
    /// <returns>An NSDictionary containing the options.</returns>
    public static implicit operator CBConnectPeripheralOptions(ConnectionOptions? options)
    {
        var dict = new CBConnectPeripheralOptions();

        if (options == null)
        {
            return dict;
        }

        if (options.NotifyOnConnection.HasValue)
        {
            dict.NotifyOnConnection = options.NotifyOnConnection.Value;
        }

        if (options.NotifyOnDisconnection.HasValue)
        {
            dict.NotifyOnDisconnection = options.NotifyOnDisconnection.Value;
        }

        if (options.NotifyOnNotification.HasValue)
        {
            dict.NotifyOnNotification = options.NotifyOnNotification.Value;
        }

        if (options.EnableTransportBridging.HasValue && (OperatingSystem.IsIOSVersionAtLeast(13) || OperatingSystem.IsTvOSVersionAtLeast(13)))
        {
            dict.EnableTransportBridging = options.EnableTransportBridging.Value;
        }

        if (options.RequiresAncs.HasValue && (OperatingSystem.IsIOSVersionAtLeast(13) || OperatingSystem.IsTvOSVersionAtLeast(13)))
        {
            dict.RequiresAncs = options.RequiresAncs.Value;
        }

        return dict;
    }

    /// <summary>
    /// Converts the CbPeripheralManagerOptions to an NSDictionary.
    /// </summary>
    /// <returns>An NSDictionary containing the options.</returns>
    // ReSharper disable once InconsistentNaming
    public CBConnectPeripheralOptions ToCBConnectPeripheralOptions()
    {
        return this;
    }
}
