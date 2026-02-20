namespace Bluetooth.Maui.Sample.Scanner.Services;

/// <summary>
///     Implementation of <see cref="INavigationService" /> using MAUI's NavigationPage.
/// </summary>
public class NavigationService : INavigationService
{
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    ///     Initializes a new instance of the <see cref="NavigationService" /> class.
    /// </summary>
    /// <param name="serviceProvider">The service provider for resolving pages.</param>
    public NavigationService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    ///     Gets the current navigation instance from the application's main window.
    /// </summary>
    private INavigation? Navigation => Application.Current?.Windows.FirstOrDefault()?.Page?.Navigation;

    /// <inheritdoc />
    public async ValueTask NavigateToAsync<TPage>(IDictionary<string, object>? parameters = null)
        where TPage : Page
    {
        var page = _serviceProvider.GetRequiredService<TPage>();

        // Pass parameters to the page if provided
        if (parameters != null && page is IQueryAttributable queryAttributable)
        {
            queryAttributable.ApplyQueryAttributes(parameters);
        }

        if (Navigation != null)
        {
            await Navigation.PushAsync(page, true);
        }
    }

    /// <inheritdoc />
    public async ValueTask NavigateBackAsync()
    {
        if (Navigation != null)
        {
            await Navigation.PopAsync(true);
        }
    }

    /// <inheritdoc />
    public async ValueTask PopToRootAsync()
    {
        if (Navigation != null)
        {
            await Navigation.PopToRootAsync(true);
        }
    }
}