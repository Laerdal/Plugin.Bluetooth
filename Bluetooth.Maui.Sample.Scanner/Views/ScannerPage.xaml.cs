namespace Bluetooth.Maui.Sample.Scanner.Views;

/// <summary>
///     Scanner page for discovering BLE devices.
/// </summary>
public partial class ScannerPage : BaseContentPage<ScannerViewModel>
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="ScannerPage" /> class.
    /// </summary>
    /// <param name="viewModel">The scanner view model.</param>
    public ScannerPage(ScannerViewModel viewModel) : base(viewModel)
    {
        InitializeComponent();
    }
}
