using System.Diagnostics;

using Bluetooth.Maui.Sample.ViewModels;

namespace Bluetooth.Maui.Sample.Views;

/// <summary>
/// Bluetooth scanner page
/// </summary>
public partial class ScannerPage
{
    public ScannerPage(ScannerPageViewModel vm) : base(vm)
    {
        InitializeComponent();
    }
}
