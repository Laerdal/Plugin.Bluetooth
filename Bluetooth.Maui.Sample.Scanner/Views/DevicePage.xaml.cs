using System.Collections.Generic;

using Microsoft.Maui.Controls;

namespace Bluetooth.Maui.Sample.Scanner.Views;

/// <summary>
/// Device detail page displaying connection controls and GATT services.
/// </summary>
public partial class DevicePage : BaseContentPage<DeviceViewModel>, IQueryAttributable
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DevicePage"/> class.
    /// </summary>
    /// <param name="viewModel">The device view model.</param>
    public DevicePage(DeviceViewModel viewModel) : base(viewModel)
    {
        InitializeComponent();
    }

    /// <summary>
    /// Applies query attributes (navigation parameters) to the page.
    /// </summary>
    /// <param name="query">The query parameters.</param>
    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        // Get the device from navigation parameters
        if (query.TryGetValue("Device", out var deviceObj) &&
            deviceObj is IBluetoothRemoteDevice device)
        {
            ViewModel.SetDevice(device);
        }
    }
}
