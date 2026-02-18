using Bluetooth.Maui.Sample.ViewModels;

using System.Collections.Concurrent;
using System.Diagnostics;

using Bluetooth.Maui.Sample.Views;

using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Extensions;

namespace Bluetooth.Maui.Sample.Services;

public interface INavigationService
{
    Task NavigateToAsync<TView>(IDictionary<string, object>? parameters = null, bool animated = true, bool cleanStack = false) where TView : Page;

    Task NavigateBackAsync(bool animated = true, bool toRoot = false);

    Task PushModalAsync<TView>(IDictionary<string, object>? parameters = null, bool animated = true) where TView : Page;

    Task PopModalAsync();

    Task ShowPopupAsync<TView>(IDictionary<string, object>? parameters = null, IPopupOptions? popupOptions = null) where TView : View;

    Task ClosePopupAsync();

    Window CreateWindow(IActivationState? activationState);

    void NavigateTo<TView>(IDictionary<string, object>? parameters = null, bool animated = true, bool cleanStack = false) where TView : Page;

    void NavigateBack(bool animated = true, bool toRoot = false);

    void PushModal<TView>(IDictionary<string, object>? parameters = null, bool animated = true) where TView : Page;

    void PopModal();

    void ShowPopup<TView>(IDictionary<string, object>? parameters = null, IPopupOptions? popupOptions = null) where TView : View;

    void ClosePopup();
}

public class NavigationService : INavigationService
{
    private readonly IServiceProvider _serviceProvider;

    public NavigationService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task NavigateToAsync<TView>(IDictionary<string, object>? parameters = null, bool animated = true, bool cleanStack = false) where TView : Page
    {
        if (Application.Current == null)
        {
            throw new InvalidOperationException("Application.Current is null");
        }

        // GET PAGE
        if (_serviceProvider.GetRequiredService<TView>() is not Page page)
        {
            throw new InvalidOperationException($"No page registered for type {typeof(TView).Name}.");
        }

        // PASS DOWN VIEWMODEL PARAMETERS
        if (page.BindingContext is IViewModel viewModel)
        {
            viewModel.OnNavigatingToParameters(parameters);
        }

        // NAVIGATE
        if (Application.Current.Windows[0].Page is not NavigationPage navigationPage)
        {
            Application.Current.Windows[0].Page = new NavigationPage(page);
        }
        else if (cleanStack)
        {
            navigationPage.Navigation.InsertPageBefore(page, navigationPage.Navigation.NavigationStack.First());
            await navigationPage.Navigation.PopToRootAsync(false);
        }
        else
        {
            await navigationPage.Navigation.PushAsync(page, animated);
        }
    }

    public Task NavigateBackAsync(bool animated = true, bool toRoot = false)
    {
        // NULL CHECKS
        if (Application.Current == null)
        {
            throw new InvalidOperationException("Application.Current is null");
        }

        if (Application.Current.Windows[0].Page is not NavigationPage navigationPage)
        {
            throw new InvalidOperationException("Current page is not a NavigationPage");
        }

        // NAVIGATE BACK
        return toRoot ? navigationPage.Navigation.PopToRootAsync(animated) : navigationPage.PopAsync(animated);
    }

    public Task PushModalAsync<TView>(IDictionary<string, object>? parameters = null, bool animated = true) where TView : Page
    {
        // NULL CHECKS
        if (Application.Current == null)
        {
            throw new InvalidOperationException("Application.Current is null");
        }

        if (Application.Current.Windows[0].Page is not NavigableElement hostPage)
        {
            throw new InvalidOperationException("No windows available in Application.Current");
        }

        // GET PAGE
        if (_serviceProvider.GetRequiredService<TView>() is not Page page)
        {
            throw new InvalidOperationException($"No page registered for type {typeof(TView).Name}.");
        }

        // PASS DOWN VIEWMODEL PARAMETERS
        if (page.BindingContext is IViewModel viewModel)
        {
            viewModel.OnNavigatingToParameters(parameters);
        }

        // NAVIGATE
        return hostPage.Navigation.PushModalAsync(page, animated);
    }

    public Task PopModalAsync()
    {
        if (Application.Current == null)
        {
            throw new InvalidOperationException("Application.Current is null");
        }

        if (Application.Current.Windows[0].Page is not NavigableElement hostPage)
        {
            throw new InvalidOperationException("No windows available in Application.Current");
        }

        return hostPage.Navigation.PopModalAsync();
    }

    public Task ShowPopupAsync<TView>(IDictionary<string, object>? parameters = null, IPopupOptions? popupOptions = null) where TView : View
    {
        if (Application.Current == null)
        {
            throw new InvalidOperationException("Application.Current is null");
        }

        if (Application.Current.Windows.Count == 0)
        {
            throw new InvalidOperationException("No windows available in Application.Current");
        }

        if (Application.Current.Windows[0].Page is not { } hostPage)
        {
            throw new InvalidOperationException("No windows available in Application.Current");
        }

        // GET PAGE
        if (_serviceProvider.GetRequiredService<TView>() is not View view)
        {
            throw new InvalidOperationException($"No view registered for type {typeof(TView).Name}.");
        }

        // PASS DOWN VIEWMODEL PARAMETERS
        if (view.BindingContext is IViewModel viewModel)
        {
            viewModel.OnNavigatingToParameters(parameters);
        }

        // SHOW POPUP
        return hostPage.ShowPopupAsync(view, popupOptions);
        // TODO : HANDLE CancellationToken
    }

    public Task ClosePopupAsync()
    {
        if (Application.Current == null)
        {
            throw new InvalidOperationException("Application.Current is null");
        }

        if (Application.Current.Windows.Count == 0)
        {
            throw new InvalidOperationException("No windows available in Application.Current");
        }

        if (Application.Current.Windows[0].Page is not { } hostPage)
        {
            throw new InvalidOperationException("No windows available in Application.Current");
        }

        return hostPage.ClosePopupAsync();
    }



    public Window CreateWindow(IActivationState? activationState)
    {
        var page = _serviceProvider.GetRequiredService<SplashPage>();
        return new Window(new NavigationPage(page));
    }

    public async void NavigateTo<TView>(IDictionary<string, object>? parameters = null, bool animated = true, bool cleanStack = false) where TView : Page
    {
        try
        {
            await NavigateToAsync<TView>(parameters, animated, cleanStack).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            App.DisplayAlert(ex);
        }
    }

    public async void NavigateBack(bool animated = true, bool toRoot = false)
    {
        try
        {
            await NavigateBackAsync(animated, toRoot).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            App.DisplayAlert(ex);
        }
    }

    public async void PushModal<TView>(IDictionary<string, object>? parameters = null, bool animated = true) where TView : Page
    {
        try
        {
            await PushModalAsync<TView>(parameters, animated).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            App.DisplayAlert(ex);
        }
    }

    public async void PopModal()
    {
        try
        {
            await PopModalAsync().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            App.DisplayAlert(ex);
        }
    }

    public async void ShowPopup<TView>(IDictionary<string, object>? parameters = null, IPopupOptions? popupOptions = null) where TView : View
    {
        try
        {
            await ShowPopupAsync<TView>(parameters, popupOptions).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            App.DisplayAlert(ex);
        }
    }

    public async void ClosePopup()
    {
        try
        {
            await ClosePopupAsync().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            App.DisplayAlert(ex);
        }
    }
}
