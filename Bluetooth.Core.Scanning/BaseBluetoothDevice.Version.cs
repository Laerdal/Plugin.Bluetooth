using Bluetooth.Core.Scanning.BluetoothSigSpecific.ServiceDefinitions;

namespace Bluetooth.Core.Scanning;

public abstract partial class BaseBluetoothDevice
{
    /// <inheritdoc/>
    public Version? FirmwareVersion
    {
        get => GetValue<Version?>(null);
        private set => SetValue(value);
    }

    /// <inheritdoc/>
    public async Task<Version> ReadFirmwareVersionAsync()
    {
        FirmwareVersion = await DeviceInformationServiceDefinition.FirmwareRevision.ReadAsync(this, true).ConfigureAwait(false);
        return FirmwareVersion;
    }

    /// <inheritdoc/>
    public Version? SoftwareVersion
    {
        get => GetValue<Version?>(null);
        private set => SetValue(value);
    }

    /// <inheritdoc/>
    public async Task<Version> ReadSoftwareVersionAsync()
    {
        SoftwareVersion = await DeviceInformationServiceDefinition.SoftwareRevision.ReadAsync(this, true).ConfigureAwait(false);
        return SoftwareVersion;
    }

    /// <inheritdoc/>
    public string? HardwareVersion
    {
        get => GetValue<string?>(null);
        private set => SetValue(value);
    }

    /// <inheritdoc/>
    public async Task<string> ReadHardwareVersionAsync()
    {
        HardwareVersion = await DeviceInformationServiceDefinition.HardwareRevision.ReadAsync(this, true).ConfigureAwait(false);
        return HardwareVersion;
    }

    /// <inheritdoc/>
    public async Task ReadVersionsAsync()
    {
        await ReadFirmwareVersionAsync().ConfigureAwait(false);
        await ReadSoftwareVersionAsync().ConfigureAwait(false);
        await ReadHardwareVersionAsync().ConfigureAwait(false);
    }
}
