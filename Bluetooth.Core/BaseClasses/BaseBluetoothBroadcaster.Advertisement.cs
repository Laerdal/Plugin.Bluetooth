namespace Bluetooth.Core.BaseClasses;

public abstract partial class BaseBluetoothBroadcaster
{
    /// <inheritdoc/>
    public bool IsAdvertising
    {
        get => GetValue(false);
        protected set => SetValue(value);
    }

    /// <inheritdoc/>
    public string? LocalDeviceName
    {
        get => GetValue<string?>(null);
        set => SetValue(value);
    }

    /// <inheritdoc/>
    public bool IsConnectable
    {
        get => GetValue(true);
        set => SetValue(value);
    }

    private ushort? _manufacturerId;
    private byte[]? _manufacturerData;
    private List<Guid>? _advertisedServiceUuids;

    /// <inheritdoc/>
    public void SetManufacturerData(ushort manufacturerId, byte[] data)
    {
        ArgumentNullException.ThrowIfNull(data);

        _manufacturerId = manufacturerId;
        _manufacturerData = data;

        NativeSetManufacturerData(manufacturerId, data);
    }

    /// <summary>
    /// Platform-specific implementation for setting manufacturer data in the advertisement.
    /// </summary>
    /// <param name="manufacturerId">The manufacturer identifier.</param>
    /// <param name="data">The manufacturer-specific data.</param>
    protected abstract void NativeSetManufacturerData(ushort manufacturerId, byte[] data);

    /// <inheritdoc/>
    public void SetAdvertisedServiceUuids(IEnumerable<Guid> serviceUuids)
    {
        ArgumentNullException.ThrowIfNull(serviceUuids);

        _advertisedServiceUuids = serviceUuids.ToList();

        NativeSetAdvertisedServiceUuids(_advertisedServiceUuids);
    }

    /// <summary>
    /// Platform-specific implementation for setting advertised service UUIDs.
    /// </summary>
    /// <param name="serviceUuids">The collection of service UUIDs to advertise.</param>
    protected abstract void NativeSetAdvertisedServiceUuids(IEnumerable<Guid> serviceUuids);

    /// <inheritdoc/>
    public void ClearAdvertisementData()
    {
        _manufacturerId = null;
        _manufacturerData = null;
        _advertisedServiceUuids = null;

        NativeClearAdvertisementData();
    }

    /// <summary>
    /// Platform-specific implementation for clearing advertisement data.
    /// </summary>
    protected abstract void NativeClearAdvertisementData();

    /// <summary>
    /// Called when the advertising state changes.
    /// </summary>
    /// <param name="isAdvertising">The new advertising state.</param>
    protected virtual void OnAdvertisingStateChanged(bool isAdvertising)
    {
        IsAdvertising = isAdvertising;
    }
}
