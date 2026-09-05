namespace Bluetooth.Maui.Sample.Scanner.Views;

/// <summary>
///     Scanner page for discovering BLE devices.
/// </summary>
public partial class ScanListPage
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="ScanListPage" /> class.
    /// </summary>
    /// <param name="viewModel">The scanner view model.</param>
    public ScanListPage(ScanListViewModel viewModel) : base(viewModel)
    {
        InitializeComponent();
    }
}
