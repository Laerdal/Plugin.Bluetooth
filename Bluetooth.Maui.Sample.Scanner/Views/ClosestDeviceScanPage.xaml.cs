namespace Bluetooth.Maui.Sample.Scanner.Views;

/// <summary>
///     Full-screen page that continuously shows the closest discovered Bluetooth device.
/// </summary>
public partial class ClosestDeviceScanPage : BaseContentPage<ClosestDeviceScanViewModel>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="ClosestDeviceScanPage" /> class.
    /// </summary>
    public ClosestDeviceScanPage(ClosestDeviceScanViewModel viewModel) : base(viewModel)
    {
        InitializeComponent();
    }
}
