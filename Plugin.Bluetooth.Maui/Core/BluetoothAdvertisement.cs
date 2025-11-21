namespace Plugin.Bluetooth.Maui;

/// <inheritdoc/>
public class BluetoothAdvertisement : BaseBluetoothAdvertisement
{
    /// <inheritdoc/>
    protected override string InitDeviceName()
    {
        throw new PlatformNotSupportedException("This functionality is only supported on Native platforms. You called the shared version.");
    }

    /// <inheritdoc/>
    protected override IEnumerable<Guid> InitServicesGuids()
    {
        throw new PlatformNotSupportedException("This functionality is only supported on Native platforms. You called the shared version.");
    }

    /// <inheritdoc/>
    protected override bool InitIsConnectable()
    {
        throw new PlatformNotSupportedException("This functionality is only supported on Native platforms. You called the shared version.");
    }

    /// <inheritdoc/>
    protected override int InitRawSignalStrengthInDBm()
    {
        throw new PlatformNotSupportedException("This functionality is only supported on Native platforms. You called the shared version.");
    }

    /// <inheritdoc/>
    protected override int InitTransmitPowerLevelInDBm()
    {
        throw new PlatformNotSupportedException("This functionality is only supported on Native platforms. You called the shared version.");
    }

    /// <inheritdoc/>
    protected override byte[] InitManufacturerData()
    {
        throw new PlatformNotSupportedException("This functionality is only supported on Native platforms. You called the shared version.");
    }

    /// <inheritdoc/>
    protected override string InitBluetoothAddress()
    {
        throw new PlatformNotSupportedException("This functionality is only supported on Native platforms. You called the shared version.");
    }
}
