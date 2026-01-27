namespace Bluetooth.Maui.Platforms.Windows.Scanning;

/// <inheritdoc/>
public class BluetoothAdvertisement : BaseBluetoothAdvertisement
{
    /// <inheritdoc/>
    protected override string InitDeviceName()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    protected override IEnumerable<Guid> InitServicesGuids()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    protected override bool InitIsConnectable()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    protected override int InitRawSignalStrengthInDBm()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    protected override int InitTransmitPowerLevelInDBm()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    protected override byte[] InitManufacturerData()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc/>
    protected override string InitBluetoothAddress()
    {
        throw new NotImplementedException();
    }
}
