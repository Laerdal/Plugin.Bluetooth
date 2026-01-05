using System.Diagnostics;

using Bluetooth.Maui.Sample.ViewModels;

namespace Bluetooth.Maui.Sample.Views.Popup;

public abstract class BaseView : ContentView
{
    protected BaseView(IViewModel viewModel)
    {
        BindingContext = viewModel;
    }

    protected override void OnBindingContextChanged()
    {
        base.OnBindingContextChanged();
        Debug.WriteLine($"BindingContext changed to {BindingContext?.GetType().Name}");
    }
}
