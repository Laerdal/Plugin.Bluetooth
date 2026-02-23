namespace Bluetooth.Maui.Sample.Scanner.Services;

/// <summary>
///     Service for navigating between pages in the application.
/// </summary>
public interface INavigationService
{
    /// <summary>
    ///     Navigates to a page asynchronously.
    /// </summary>
    /// <typeparam name="TPage">The type of page to navigate to.</typeparam>
    /// <param name="parameters">Optional parameters to pass to the page's ViewModel.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    ValueTask NavigateToAsync<TPage>(IDictionary<string, object>? parameters = null) where TPage : Page;

    /// <summary>
    ///     Navigates back to the previous page.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    ValueTask NavigateBackAsync();

    /// <summary>
    ///     Pops the navigation stack to the root page.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    ValueTask PopToRootAsync();
}
