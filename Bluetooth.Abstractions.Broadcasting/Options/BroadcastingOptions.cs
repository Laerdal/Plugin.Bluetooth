namespace Bluetooth.Abstractions.Broadcasting.Options;

/// <summary>
///     Represents a Bluetooth broadcast advertisement configuration.
/// </summary>
public record BroadcastingOptions
{
    #region Permission Handling

    /// <summary>
    ///     Gets the permission request strategy for this broadcasting operation.
    /// </summary>
    /// <remarks>
    ///     Defaults to <see cref="PermissionRequestStrategy.RequestAutomatically"/> which automatically
    ///     requests permissions before starting broadcasting if not already granted.
    /// </remarks>
    public PermissionRequestStrategy PermissionStrategy { get; init; } = PermissionRequestStrategy.RequestAutomatically;

    #endregion

    /// <summary>
    ///     Gets the local device name to be included in the advertisement.
    /// </summary>
    public string? LocalDeviceName { get; init; }

    /// <summary>
    ///     Gets a value indicating whether to include the device name in the advertisement.
    /// </summary>
    public bool IncludeDeviceName { get; init; }

    /// <summary>
    ///     Gets a value indicating whether the advertising device is connectable.
    /// </summary>
    /// <remarks>
    ///     <b>Platform Support:</b>
    ///     <list type="bullet">
    ///         <item><b>Android</b>: Controlled via AdvertiseSettings.setConnectable() - restricts whether devices can connect</item>
    ///         <item><b>iOS/macOS</b>: Always connectable when CBPeripheralManager is advertising services</item>
    ///         <item><b>Windows</b>: Controlled via BluetoothLEAdvertisementPublisher.IsDiscoverable property</item>
    ///     </list>
    ///     <para>When false, devices can see advertisements but cannot establish GATT connections (useful for beacon-only applications).</para>
    /// </remarks>
    public bool IsConnectable { get; init; }

    /// <summary>
    ///     Gets the transmit power level to include in the advertisement (in dBm).
    /// </summary>
    public int? TxPowerLevel { get; init; }

    /// <summary>
    ///     Gets a value indicating whether to include the transmit power level in the advertisement.
    /// </summary>
    public bool IncludeTxPowerLevel { get; init; }

    /// <summary>
    ///     Gets the manufacturer identifier included in the advertisement data.
    /// </summary>
    public ushort? ManufacturerId { get; init; }

    /// <summary>
    ///     Gets the manufacturer-specific data included in the advertisement.
    /// </summary>
    public ReadOnlyMemory<byte>? ManufacturerData { get; init; }

    /// <summary>
    ///     Gets multiple manufacturer-specific data entries.
    ///     Key: Manufacturer ID, Value: Manufacturer data.
    /// </summary>
    public IReadOnlyDictionary<ushort, ReadOnlyMemory<byte>>? ManufacturerDataEntries { get; init; }

    /// <summary>
    ///     Gets the list of service UUIDs advertised by the device.
    /// </summary>
    public IReadOnlyList<Guid>? AdvertisedServiceUuids { get; init; }

    /// <summary>
    ///     Gets service-specific data to include in the advertisement.
    ///     Key: Service UUID, Value: Service data.
    /// </summary>
    public IReadOnlyDictionary<Guid, ReadOnlyMemory<byte>>? ServiceData { get; init; }

    #region Extended Advertising (Bluetooth 5.0+)

    /// <summary>
    ///     Gets a value indicating whether to use extended advertising (Bluetooth 5.0+ feature).
    ///     Extended advertising supports larger payloads and additional features.
    /// </summary>
    /// <remarks>
    ///     <b>Platform Support:</b>
    ///     <list type="bullet">
    ///         <item><b>Android</b>: Supported on Android 8.0 (API 26+) with Bluetooth 5.0 hardware via AdvertisingSetParameters</item>
    ///         <item><b>iOS/macOS</b>: Automatically used on devices with Bluetooth 5.0 - system-managed</item>
    ///         <item><b>Windows</b>: Supported on Windows 10 version 2004+ with Bluetooth 5.0 adapter via BluetoothLEAdvertisementPublisher</item>
    ///     </list>
    ///     <para>
    ///         Extended advertising benefits:
    ///         - Larger advertisement payload (up to 254 bytes vs 31 bytes legacy)
    ///         - Multiple advertising sets support
    ///         - Periodic advertising support
    ///         - Better coexistence with other Bluetooth operations
    ///     </para>
    /// </remarks>
    public bool UseExtendedAdvertising { get; init; }

    /// <summary>
    ///     Gets the primary PHY mode for advertising.
    ///     Only applicable when UseExtendedAdvertising is true.
    /// </summary>
    /// <remarks>
    ///     <b>Platform Support:</b>
    ///     <list type="bullet">
    ///         <item><b>Android</b>: Supported with extended advertising (API 26+) via AdvertisingSetParameters.setPrimaryPhy()</item>
    ///         <item><b>iOS/macOS</b>: Not configurable - system automatically selects PHY mode</item>
    ///         <item><b>Windows</b>: Not configurable - system automatically selects PHY mode</item>
    ///     </list>
    ///     <para>Primary PHY is always 1M or Coded PHY (for long-range). Value corresponds to Android BluetoothDevice PHY constants.</para>
    /// </remarks>
    public int? PrimaryPhy { get; init; }

    /// <summary>
    ///     Gets the secondary PHY mode for advertising.
    ///     Only applicable when UseExtendedAdvertising is true.
    /// </summary>
    /// <remarks>
    ///     <b>Platform Support:</b>
    ///     <list type="bullet">
    ///         <item><b>Android</b>: Supported with extended advertising (API 26+) via AdvertisingSetParameters.setSecondaryPhy()</item>
    ///         <item><b>iOS/macOS</b>: Not configurable - system automatically selects PHY mode</item>
    ///         <item><b>Windows</b>: Not configurable - system automatically selects PHY mode</item>
    ///     </list>
    ///     <para>Secondary PHY can be 1M, 2M, or Coded PHY. Used for carrying advertisement data. Value corresponds to Android BluetoothDevice PHY constants.</para>
    /// </remarks>
    public int? SecondaryPhy { get; init; }

    /// <summary>
    ///     Gets a value indicating whether this is an anonymous advertisement.
    ///     Anonymous advertisements don't include the device address.
    ///     Only applicable when UseExtendedAdvertising is true.
    /// </summary>
    /// <remarks>
    ///     <b>Platform Support:</b>
    ///     <list type="bullet">
    ///         <item><b>Android</b>: Supported with extended advertising (API 26+) via AdvertisingSetParameters.setAnonymous()</item>
    ///         <item><b>iOS/macOS</b>: Not supported - device address is always included or randomized by system</item>
    ///         <item><b>Windows</b>: Not supported - device address handling is system-managed</item>
    ///     </list>
    ///     <para>Anonymous advertising improves privacy by omitting the device address from advertisements, useful for privacy-sensitive beacon applications.</para>
    /// </remarks>
    public bool IsAnonymous { get; init; }

    /// <summary>
    ///     Gets the advertising interval in milliseconds.
    /// </summary>
    /// <remarks>
    ///     <b>Platform Support:</b>
    ///     <list type="bullet">
    ///         <item><b>Android</b>: Supported via AdvertisingSetParameters.setInterval() (range: 160-16384 units, where 1 unit = 0.625ms)</item>
    ///         <item><b>iOS/macOS</b>: Not directly configurable - system manages interval automatically (typically ~100ms)</item>
    ///         <item><b>Windows</b>: Not directly configurable - system manages interval based on priority settings</item>
    ///     </list>
    ///     <para>
    ///         Advertising interval affects power consumption and discoverability:
    ///         - Shorter interval (~100ms): Faster discovery, higher power consumption
    ///         - Longer interval (~1000ms): Slower discovery, better battery life
    ///         - Typical range: 100ms - 10000ms
    ///     </para>
    ///     <para>On Android, value is converted to Advertisement interval units (value_ms / 0.625). System may adjust based on other active advertisements.</para>
    /// </remarks>
    public int? AdvertisingInterval { get; init; }

    #endregion
}