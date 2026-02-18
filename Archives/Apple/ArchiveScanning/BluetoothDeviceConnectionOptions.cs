namespace Bluetooth.Maui.Platforms.Apple.Scanning;

/// <summary>
/// Represents iOS-specific Bluetooth device connection options.
/// </summary>
public record BluetoothDeviceConnectionOptions : BaseBluetoothDevice.BaseBluetoothDeviceConnectionOptions
{
    /// <summary>
    /// Gets or sets whether to notify on connection events.
    /// </summary>
    public bool? NotifyOnConnection { get; init; }

    /// <summary>
    /// Gets or sets whether to notify on disconnection events.
    /// </summary>
    public bool? NotifyOnDisconnection { get; init; }

    /// <summary>
    /// Gets or sets whether to notify on notification events.
    /// </summary>
    public bool? NotifyOnNotification { get; init; }

    /// <summary>
    /// Gets or sets whether to enable transport bridging.
    /// </summary>
    public bool? EnableTransportBridging { get; init; }

    /// <summary>
    /// Gets or sets whether to require ANCS (Apple Notification Center Service) support.
    /// </summary>
    public bool? RequireAncs { get; init; }

    #region To CBConnectPeripheralOptions

    /// <summary>
    /// Implicitly converts BluetoothDeviceConnectionOptions to CBConnectPeripheralOptions.
    /// </summary>
    public static implicit operator CBConnectPeripheralOptions?(BluetoothDeviceConnectionOptions? options)
    {
        if (options == null)
        {
            return null;
        }

        var cbOptions = new CBConnectPeripheralOptions
        {
            NotifyOnConnection = options.NotifyOnConnection,
            NotifyOnDisconnection = options.NotifyOnDisconnection,
            NotifyOnNotification = options.NotifyOnNotification
        };

        if (OperatingSystem.IsIOSVersionAtLeast(13) || OperatingSystem.IsMacCatalystVersionAtLeast(13))
        {
            cbOptions.EnableTransportBridging = options.EnableTransportBridging;
            cbOptions.RequiresAncs = options.RequireAncs;
        }

        return cbOptions;
    }

    /// <summary>
    /// Converts the BluetoothDeviceConnectionOptions to a CBConnectPeripheralOptions.
    /// </summary>

    // ReSharper disable once InconsistentNaming
    public CBConnectPeripheralOptions? ToCBConnectPeripheralOptions()
    {
        return this;
    }

    #endregion

    #region To PeripheralConnectionOptions

    /// <summary>
    /// Implicitly converts BluetoothDeviceConnectionOptions to PeripheralConnectionOptions.
    /// </summary>
    /// <param name="options">The options to convert.</param>
    /// <returns>A PeripheralConnectionOptions containing the converted options.</returns>
    public static implicit operator PeripheralConnectionOptions?(BluetoothDeviceConnectionOptions? options)
    {
        if (options == null)
        {
            return null;
        }

        return new PeripheralConnectionOptions
        {
            NotifyOnConnection = options.NotifyOnConnection,
            NotifyOnDisconnection = options.NotifyOnDisconnection,
            NotifyOnNotification = options.NotifyOnNotification
        };
    }

    /// <summary>
    /// Converts the BluetoothDeviceConnectionOptions to an PeripheralConnectionOptions.
    /// </summary>
    /// <returns>A PeripheralConnectionOptions containing the converted options.</returns>

    // ReSharper disable once InconsistentNaming
    public PeripheralConnectionOptions? ToPeripheralConnectionOptions()
    {
        return this;
    }

    #endregion

}
