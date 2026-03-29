namespace Bluetooth.Core.Scanning;

using Bluetooth.Core.Scanning.Profiles;
using Bluetooth.Core.Scanning.Profiles.BluetoothSig;

public abstract partial class BaseBluetoothRemoteDevice
{
    /// <inheritdoc />
    public Version? FirmwareVersion
    {
        get => GetValue<Version?>(null);
        private set => SetValue(value);
    }

    /// <inheritdoc />
    public async Task<Version> ReadFirmwareVersionAsync()
    {
        var defaultVersion = FirmwareVersion ?? new Version(0, 0);
        FirmwareVersion = await DeviceInformationServiceDefinition.FirmwareRevision
            .ReadValueOrDefaultAsync(this, defaultVersion, skipIfPreviouslyRead: true)
            .ConfigureAwait(false);
        return FirmwareVersion;
    }

    /// <inheritdoc />
    public Version? SoftwareVersion
    {
        get => GetValue<Version?>(null);
        private set => SetValue(value);
    }

    /// <inheritdoc />
    public async Task<Version> ReadSoftwareVersionAsync()
    {
        var defaultVersion = SoftwareVersion ?? new Version(0, 0);
        SoftwareVersion = await DeviceInformationServiceDefinition.SoftwareRevision
            .ReadValueOrDefaultAsync(this, defaultVersion, skipIfPreviouslyRead: true)
            .ConfigureAwait(false);
        return SoftwareVersion;
    }

    /// <inheritdoc />
    public string? HardwareVersion
    {
        get => GetValue<string?>(null);
        private set => SetValue(value);
    }

    /// <inheritdoc />
    public async Task<string> ReadHardwareVersionAsync()
    {
        var defaultVersion = HardwareVersion ?? string.Empty;
        HardwareVersion = await DeviceInformationServiceDefinition.HardwareRevision
            .ReadValueOrDefaultAsync(this, defaultVersion, skipIfPreviouslyRead: true)
            .ConfigureAwait(false);
        return HardwareVersion;
    }

    /// <inheritdoc />
    public async Task ReadVersionsAsync()
    {
        await ReadFirmwareVersionAsync().ConfigureAwait(false);
        await ReadSoftwareVersionAsync().ConfigureAwait(false);
        await ReadHardwareVersionAsync().ConfigureAwait(false);
    }
}
