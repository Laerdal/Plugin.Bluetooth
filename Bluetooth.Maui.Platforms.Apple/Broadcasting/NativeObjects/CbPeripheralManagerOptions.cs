namespace Bluetooth.Maui.Platforms.Apple.Broadcasting.NativeObjects;

/// <summary>
///     Options for initializing a CBPeripheralManager.
/// </summary>
/// <see href="https://developer.apple.com/documentation/corebluetooth/peripheral-manager-initialization-options" />
public record CbPeripheralManagerOptions
{
    /// <summary>
    ///     Gets or sets the restore identifier key for state restoration.
    /// </summary>
    /// <see href="https://developer.apple.com/documentation/corebluetooth/cbperipheralmanageroptionrestoreidentifierkey" />
    public string? RestoreIdentifierKey { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether to show a power alert when Bluetooth is off.
    /// </summary>
    /// <see href="https://developer.apple.com/documentation/corebluetooth/cbperipheralmanageroptionshowpoweralertkey" />
    public bool ShowPowerAlert { get; set; } = true;

    /// <summary>
    ///     Implicitly converts CbPeripheralManagerOptions to NSDictionary.
    /// </summary>
    /// <param name="options">The options to convert.</param>
    /// <returns>An NSDictionary containing the options.</returns>
    public static implicit operator NSDictionary(CbPeripheralManagerOptions? options)
    {
        var dict = new NSMutableDictionary();

        if (options == null)
        {
            return dict;
        }

        // Show power alert
        dict[CBPeripheralManager.OptionShowPowerAlertKey] = NSNumber.FromBoolean(options.ShowPowerAlert);

        if (!string.IsNullOrEmpty(options.RestoreIdentifierKey))
        {
            dict[CBPeripheralManager.OptionRestoreIdentifierKey] = new NSString(options.RestoreIdentifierKey);
        }

        return dict;
    }

    /// <summary>
    ///     Converts the CbPeripheralManagerOptions to an NSDictionary.
    /// </summary>
    /// <returns>An NSDictionary containing the options.</returns>
    // ReSharper disable once InconsistentNaming
    public NSDictionary ToNSDictionary()
    {
        return this;
    }
}
