using System.Runtime.CompilerServices;

namespace Bluetooth.Maui.Sample.Scanner.Infrastructure;

/// <summary>
/// Base class for all ViewModels in the application.
/// Provides property change notification and lifecycle hooks using a concurrent dictionary backing store.
/// </summary>
public abstract class BaseViewModel : INotifyPropertyChanged
{
    private readonly ConcurrentDictionary<string, object?> _values = new ConcurrentDictionary<string, object?>();

    /// <inheritdoc/>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Gets the value of a property from the backing store.
    /// </summary>
    /// <typeparam name="T">Type of the property value.</typeparam>
    /// <param name="propertyName">Name of the property (auto-populated by compiler).</param>
    /// <returns>The property value, or default if not found.</returns>
    protected T? GetValue<T>([CallerMemberName] string? propertyName = null)
    {
        if (propertyName == null)
        {
            return default;
        }
        return _values.TryGetValue(propertyName, out var value) ? (T?)value : default;
    }

    /// <summary>
    /// Sets the value of a property in the backing store and raises PropertyChanged.
    /// </summary>
    /// <typeparam name="T">Type of the property value.</typeparam>
    /// <param name="value">The new value to set.</param>
    /// <param name="propertyName">Name of the property (auto-populated by compiler).</param>
    protected void SetValue<T>(T value, [CallerMemberName] string? propertyName = null)
    {
        if (propertyName == null)
        {
            return;
        }
        _values[propertyName] = value;
        OnPropertyChanged(propertyName);
    }

    /// <summary>
    /// Raises the PropertyChanged event for the specified property.
    /// </summary>
    /// <param name="propertyName">Name of the property that changed.</param>
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    /// <summary>
    /// Called when the associated page is appearing.
    /// Override to perform initialization logic.
    /// </summary>
    public virtual ValueTask OnAppearingAsync() => ValueTask.CompletedTask;

    /// <summary>
    /// Called when the associated page is disappearing.
    /// Override to perform cleanup logic.
    /// </summary>
    public virtual ValueTask OnDisappearingAsync() => ValueTask.CompletedTask;
}
