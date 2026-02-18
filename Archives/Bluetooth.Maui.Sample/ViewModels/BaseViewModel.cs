using System.Collections.Concurrent;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;

using Bluetooth.Maui.Sample.Services;
using Bluetooth.Maui.Sample.Views;

namespace Bluetooth.Maui.Sample.ViewModels;


public interface IViewModel
{
    void OnViewAppearing(Page baseContentPage);

    void OnViewDisappearing(Page baseContentPage);

    void OnViewNavigatedFrom(Page baseContentPage, NavigatedFromEventArgs args);

    void OnViewNavigatedTo(Page baseContentPage, NavigatedToEventArgs args);

    void OnViewNavigatingFrom(Page baseContentPage, NavigatingFromEventArgs args);

    void OnNavigatingToParameters(IDictionary<string, object>? parameters);
}

public abstract class BaseViewModel : INotifyPropertyChanged, IViewModel
{
    public INavigationService NavigationService { get; }

    public virtual void OnNavigatingToParameters(IDictionary<string, object>? parameters = null)
    {
        // Override in derived classes if needed
    }

    public virtual void OnViewAppearing(Page baseContentPage)
    {
        Debug.WriteLine($"View Appearing: {baseContentPage.GetType().Name}");
        // Override in derived classes if needed
    }

    public virtual void OnViewDisappearing(Page baseContentPage)
    {
        Debug.WriteLine($"View Disappearing: {baseContentPage.GetType().Name}");
        // Override in derived classes if needed
    }

    public virtual void OnViewNavigatedFrom(Page baseContentPage, NavigatedFromEventArgs args)
    {
        Debug.WriteLine($"Navigated From: {baseContentPage.GetType().Name}; To: {args.DestinationPage?.GetType().Name}; NavigationType: {args.NavigationType}");
        // Override in derived classes if needed
    }

    public virtual void OnViewNavigatedTo(Page baseContentPage, NavigatedToEventArgs args)
    {
        Debug.WriteLine($"Navigated To: {baseContentPage.GetType().Name}; From: {args.PreviousPage?.GetType().Name}; NavigationType: {args.NavigationType}");
        // Override in derived classes if needed
    }

    public virtual void OnViewNavigatingFrom(Page baseContentPage, NavigatingFromEventArgs args)
    {
        Debug.WriteLine($"Navigating From: {baseContentPage.GetType().Name}; To: {args.DestinationPage?.GetType().Name}; NavigationType: {args.NavigationType}");
        // Override in derived classes if needed
    }

    protected BaseViewModel(INavigationService navigationService)
    {
        NavigationService = navigationService;
    }

    #region Bindable Properties
    private readonly ConcurrentDictionary<string, object?> _values = new ConcurrentDictionary<string, object?>();

    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Gets or sets the title for the page/view.
    /// </summary>
    public string Title
    {
        get => GetValue(string.Empty);
        set => SetValue(value);
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    /// <summary>
    ///     Gets the value of the specified property, or sets and returns the default value if not present.
    /// </summary>
    /// <typeparam name="T">The type of the property value.</typeparam>
    /// <param name="defaultValue">The default value to use if the property is not set.</param>
    /// <param name="propertyName">The name of the property. If not provided, the caller's member name is used.</param>
    /// <returns>The value of the property, or the default value if not set.</returns>
    /// <exception cref="ArgumentException">If the property name is null or whitespace.</exception>
    protected T GetValue<T>(T defaultValue, [CallerMemberName] string? propertyName = null)
    {
        if (string.IsNullOrWhiteSpace(propertyName))
        {
            throw new ArgumentException("Invalid property name", propertyName);
        }

        if (_values.TryGetValue(propertyName, out var value) && value is T tValue)
        {
            return tValue;
        }

        _values.TryAdd(propertyName, defaultValue);
        return defaultValue;
    }

    /// <summary>
    ///     Sets the value of the specified property and raises the <see cref="PropertyChanged"/> event if the value changes.
    /// </summary>
    /// <typeparam name="T">The type of the property value.</typeparam>
    /// <param name="value">The value to set.</param>
    /// <param name="propertyName">The name of the property. If not provided, the caller's member name is used.</param>
    /// <returns>True if the value was changed and the property was set; otherwise, false.</returns>
    /// <exception cref="ArgumentException">If the property name is null or whitespace.</exception>
    protected bool SetValue<T>(T value, [CallerMemberName] string? propertyName = null)
    {
        if (string.IsNullOrWhiteSpace(propertyName))
        {
            throw new ArgumentException("Invalid property name", propertyName);
        }

        if (_values.TryGetValue(propertyName, out var existingValue) && Equals(existingValue, value))
        {
            return false; // No change
        }

        _values.AddOrUpdate(propertyName, value, (_, _) => value);
        OnPropertyChanged(propertyName);
        return true;
    }

    public override string ToString()
    {
        return GetType().FullName ?? string.Empty;
    }
    #endregion


}
