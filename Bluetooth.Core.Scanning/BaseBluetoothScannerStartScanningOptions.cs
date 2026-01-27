using Bluetooth.Abstractions.Scanning;

namespace Bluetooth.Core.Scanning;

/// <inheritdoc />
public record BaseBluetoothScannerStartScanningOptions : IBluetoothScannerStartScanningOptions
{
    /// <inheritdoc/>
    public bool IgnoreDuplicateAdvertisements { get; init; }

    /// <inheritdoc/>
    public Func<IBluetoothAdvertisement, bool> AdvertisementFilter { get; init; } = DefaultAdvertisementFilter;

    /// <summary>
    /// Default advertisement filter that accepts all advertisements.
    /// </summary>
    /// <param name="arg">The advertisement to filter.</param>
    /// <returns><c>true</c> for all advertisements.</returns>
    private static bool DefaultAdvertisementFilter(IBluetoothAdvertisement arg)
    {
        return true;
    }
}

/// <inheritdoc />
public record DefaultBluetoothScannerStartScanningOptions : BaseBluetoothScannerStartScanningOptions
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultBluetoothScannerStartScanningOptions"/> class.
    /// </summary>
    public DefaultBluetoothScannerStartScanningOptions()
        : base()
    {
    }
}
