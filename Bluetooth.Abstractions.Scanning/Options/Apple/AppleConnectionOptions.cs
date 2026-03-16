namespace Bluetooth.Abstractions.Scanning.Options.Apple;

public record AppleScanningOptions : ScanningOptions
{
    public bool? AllowDuplicates { get; init; }
    
    public Guid? ServiceUuid { get; init; }
    
    public IList<Guid>? PeripheralUuids { get; init; }
}

/// <summary>
///     Apple (iOS/macOS) platform-specific connection options.
/// </summary>
public record AppleConnectionOptions : ConnectionOptions
{
    /// <summary>
    ///     Gets a value indicating whether to notify on connection events.
    /// </summary>
    /// <remarks>
    ///     Corresponds to CBConnectPeripheralOptionNotifyOnConnectionKey.
    ///     When true, the system will display an alert when the peripheral connects while
    ///     the app is suspended.
    /// </remarks>
    public bool? NotifyOnConnection { get; init; }

    /// <summary>
    ///     Gets a value indicating whether to notify on disconnection events.
    /// </summary>
    /// <remarks>
    ///     Corresponds to CBConnectPeripheralOptionNotifyOnDisconnectionKey.
    ///     When true, the system will display an alert when the peripheral disconnects while
    ///     the app is suspended.
    /// </remarks>
    public bool? NotifyOnDisconnection { get; init; }

    /// <summary>
    ///     Gets a value indicating whether to notify on notification events.
    /// </summary>
    /// <remarks>
    ///     Corresponds to CBConnectPeripheralOptionNotifyOnNotificationKey.
    ///     When true, the system will display an alert for incoming notifications while
    ///     the app is suspended.
    /// </remarks>
    public bool? NotifyOnNotification { get; init; }

    /// <summary>
    ///     Gets a value indicating whether to enable transport bridging (iOS 13+).
    /// </summary>
    /// <remarks>
    ///     Corresponds to CBConnectPeripheralOptionEnableTransportBridgingKey.
    ///     When true, allows the peripheral to be used over both Classic and LE transports.
    ///     Available on iOS 13.0+ and tvOS 13.0+.
    /// </remarks>
    public bool? EnableTransportBridging { get; init; }

    /// <summary>
    ///     Gets a value indicating whether to require ANCS (Apple Notification Center Service) support (iOS 13+).
    /// </summary>
    /// <remarks>
    ///     Corresponds to CBConnectPeripheralOptionRequiresANCS.
    ///     When true, the connection will only succeed if the peripheral supports ANCS.
    ///     Available on iOS 13.0+ and tvOS 13.0+.
    /// </remarks>
    public bool? RequiresAncs { get; init; }
}
