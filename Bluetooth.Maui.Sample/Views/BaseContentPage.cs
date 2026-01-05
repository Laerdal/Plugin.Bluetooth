using System.Diagnostics;

using Bluetooth.Maui.Sample.ViewModels;

namespace Bluetooth.Maui.Sample.Views;

public abstract class BaseContentPage : ContentPage
{
    protected BaseContentPage(IViewModel viewModel)
    {
        BindingContext = viewModel;
    }

    protected override void OnBindingContextChanged()
    {
        base.OnBindingContextChanged();
        Debug.WriteLine($"BindingContext changed to {BindingContext?.GetType().Name}");
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if(BindingContext is IViewModel viewModel)
        {
            viewModel.OnViewAppearing(this);
        }
    }

    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        if (BindingContext is IViewModel viewModel)
        {
            viewModel.OnViewDisappearing(this);
        }
    }

    protected override void OnNavigatedFrom(NavigatedFromEventArgs args)
    {
        base.OnNavigatedFrom(args);
        if (BindingContext is IViewModel viewModel)
        {
            viewModel.OnViewNavigatedFrom(this, args);
        }
    }

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        if (BindingContext is IViewModel viewModel)
        {
            viewModel.OnViewNavigatedTo(this, args);
        }
    }

    protected override void OnNavigatingFrom(NavigatingFromEventArgs args)
    {
        if (BindingContext is IViewModel viewModel)
        {
            viewModel.OnViewNavigatingFrom(this, args);
        }
        base.OnNavigatingFrom(args);
    }
}
