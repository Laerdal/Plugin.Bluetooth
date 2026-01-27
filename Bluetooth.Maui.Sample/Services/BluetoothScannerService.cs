using System.Collections.ObjectModel;

using Bluetooth.Core.Abstractions;
using Bluetooth.Core.Abstractions.Scan;

using Plugin.BaseTypeExtensions;

namespace Bluetooth.Maui.Sample.Services;

public class BluetoothScannerService
{
    public IBluetoothScanner Scanner { get; }

    /// <summary>
    /// Observable collection of discovered Bluetooth devices
    /// </summary>
    public ObservableCollection<IBluetoothDevice> Devices { get; }

    private bool _includeUnnamedDevices;

    public bool IncludeUnnamedDevices
    {
        get => _includeUnnamedDevices;
        set
        {
            if (_includeUnnamedDevices != value)
            {
                _includeUnnamedDevices = value;
                Devices.UpdateFrom(Scanner.GetDevices(Filter).ToList());
            }
        }
    }

    public BluetoothScannerService(IBluetoothScanner bluetoothScanner)
    {
        Scanner = bluetoothScanner ?? throw new ArgumentNullException(nameof(bluetoothScanner));
        Scanner.DeviceListChanged += OnScannerDeviceListChanged;
        Devices = new ObservableCollection<IBluetoothDevice>();
    }

    private void OnScannerDeviceListChanged(object? sender, DeviceListChangedEventArgs e)
    {
        Devices.UpdateFrom(Scanner.GetDevices(Filter).ToList());
    }

    private Func<IBluetoothDevice, bool> Filter => device =>
    {
        if (string.IsNullOrEmpty(device.Name) && !IncludeUnnamedDevices)
        {
            return false;
        }

        return true;
    };
}
