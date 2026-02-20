namespace Bluetooth.Maui.Sample.Scanner.Infrastructure;

/// <summary>
/// Base class for all ContentPages in the application.
/// Automatically binds the ViewModel and invokes lifecycle hooks.
/// </summary>
/// <typeparam name="TViewModel">The ViewModel type for this page.</typeparam>
public abstract class BaseContentPage<TViewModel> : ContentPage
    where TViewModel : BaseViewModel
{
    /// <summary>
    /// Gets the ViewModel associated with this page.
    /// </summary>
    protected TViewModel ViewModel { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseContentPage{TViewModel}"/> class.
    /// </summary>
    /// <param name="viewModel">The ViewModel to bind to this page.</param>
    protected BaseContentPage(TViewModel viewModel)
    {
        ViewModel = viewModel;
        BindingContext = viewModel;
    }

    /// <inheritdoc/>
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await ViewModel.OnAppearingAsync();
    }

    /// <inheritdoc/>
    protected override async void OnDisappearing()
    {
        base.OnDisappearing();
        await ViewModel.OnDisappearingAsync();
    }
}
