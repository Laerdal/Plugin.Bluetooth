namespace Bluetooth.Maui.Sample.Scanner.Services;

/// <summary>
/// Implementation of <see cref="INavigationService"/> using MAUI's NavigationPage.
/// </summary>
public class NavigationService : INavigationService
{
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// Gets the current navigation instance from the application's main page.
    /// </summary>
    private INavigation? Navigation => Application.Current?.MainPage?.Navigation;

    /// <summary>
    /// Initializes a new instance of the <see cref="NavigationService"/> class.
    /// </summary>
    /// <param name="serviceProvider">The service provider for resolving pages.</param>
    public NavigationService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    /// <inheritdoc/>
    public async ValueTask NavigateToAsync<TPage>(IDictionary<string, object>? parameters = null)
        where TPage : Page
    {
        var page = _serviceProvider.GetRequiredService<TPage>();

        // If the page has a ViewModel that needs parameters, we could add a method
        // like OnNavigatingTo(parameters) to BaseViewModel to pass them
        // For now, parameters can be accessed directly by the caller before navigation

        if (Navigation != null)
        {
            await Navigation.PushAsync(page, true);
        }
    }

    /// <inheritdoc/>
    public async ValueTask NavigateBackAsync()
    {
        if (Navigation != null)
        {
            await Navigation.PopAsync(true);
        }
    }

    /// <inheritdoc/>
    public async ValueTask PopToRootAsync()
    {
        if (Navigation != null)
        {
            await Navigation.PopToRootAsync(true);
        }
    }
}
