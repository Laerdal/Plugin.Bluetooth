namespace Bluetooth.Maui.Sample.Scanner.Views;

/// <summary>
///     Full-screen page that continuously shows the closest discovered Bluetooth device.
/// </summary>
public partial class ScanClosestDevicePage : BaseContentPage<ScanClosestDeviceViewModel>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="ScanClosestDevicePage" /> class.
    /// </summary>
    public ScanClosestDevicePage(ScanClosestDeviceViewModel viewModel) : base(viewModel)
    {
        InitializeComponent();
    }
}
