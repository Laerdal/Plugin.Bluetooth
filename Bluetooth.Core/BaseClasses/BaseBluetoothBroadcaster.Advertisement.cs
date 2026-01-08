namespace Bluetooth.Core.BaseClasses;

public abstract partial class BaseBluetoothBroadcaster
{

    #region LocalDeviceName

    /// <inheritdoc/>
    public string? LocalDeviceName
    {
        get => GetValue<string?>(null);
        private set => SetValue(value);
    }

    /// <inheritdoc/>
    public void SetLocalDeviceName(string? localDeviceName)
    {
        LocalDeviceName = localDeviceName;
        NativeAdvertisementDataChanged();
    }

    #endregion

    #region IsConnectable

    /// <inheritdoc/>
    public bool IsConnectable
    {
        get => GetValue(false);
        private set => SetValue(value);
    }

    /// <inheritdoc/>
    public void SetIsConnectable(bool isConnectable)
    {
        IsConnectable = isConnectable;
        NativeAdvertisementDataChanged();
    }

    #endregion

    #region Manufacturer

    /// <inheritdoc/>
    public ushort? ManufacturerId { get; set; }

    /// <inheritdoc/>
    public ReadOnlyMemory<byte>? ManufacturerData { get; set; }

    /// <inheritdoc/>
    public void SetManufacturerData(ushort manufacturerId, ReadOnlySpan<byte> data)
    {
        ManufacturerId = manufacturerId;
        ManufacturerData = data.ToArray();
        NativeAdvertisementDataChanged();
    }

    #endregion

    #region ServiceUuids

    /// <inheritdoc/>
    public IReadOnlyList<Guid>? AdvertisedServiceUuids { get; set; }

    /// <inheritdoc/>
    public void SetAdvertisedServiceUuids(IEnumerable<Guid> serviceUuids)
    {
        AdvertisedServiceUuids = serviceUuids.ToList();
        NativeAdvertisementDataChanged();
    }

    #endregion

    /// <inheritdoc/>
    public void ClearAdvertisementData()
    {
        ManufacturerId = null;
        ManufacturerData = null;
        AdvertisedServiceUuids = null;
        NativeAdvertisementDataChanged();
    }

    /// <summary>
    /// Platform-specific implementation for updating advertised data.
    /// </summary>
    protected abstract void NativeAdvertisementDataChanged();
}
