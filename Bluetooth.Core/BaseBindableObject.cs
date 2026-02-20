using System.Runtime.CompilerServices;

namespace Bluetooth.Core;

/// <summary>
///     Provides a base class for bindable objects with property change notification, value storage.
/// </summary>
public abstract partial class BaseBindableObject : INotifyPropertyChanged
{
    private readonly ConcurrentDictionary<string, object?> _values = new();

    /// <summary>
    ///     Initializes a new instance of the <see cref="BaseBindableObject" /> class.
    /// </summary>
    /// <param name="logger">Optional logger instance for tracking property changes.</param>
    protected BaseBindableObject(ILogger? logger = null)
    {
        Logger = logger;
    }

    /// <summary>
    ///     The logger instance for this object.
    /// </summary>
    protected ILogger? Logger { get; }

    /// <summary>
    ///     Occurs when a property value changes.
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    ///     Returns a string that represents the current object.
    /// </summary>
    public override string ToString()
    {
        return GetType().Name;
    }

    /// <summary>
    ///     Determines whether a value has been set for the specified property.
    /// </summary>
    /// <param name="propertyName">The name of the property. If not provided, the caller's member name is used.</param>
    /// <returns>True if a value has been set for the property; otherwise, false.</returns>
    /// <exception cref="ArgumentException">If the property name is null or whitespace.</exception>
    protected bool HasValue([CallerMemberName] string? propertyName = null)
    {
        if (string.IsNullOrWhiteSpace(propertyName))
        {
            throw new ArgumentException("Invalid property name", propertyName);
        }

        return _values.ContainsKey(propertyName);
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
    ///     Gets the value of the specified property without storing a default value if not present.
    /// </summary>
    /// <typeparam name="T">The type of the property value.</typeparam>
    /// <param name="defaultValue">The default value to return if the property is not set.</param>
    /// <param name="propertyName">The name of the property. If not provided, the caller's member name is used.</param>
    /// <returns>The value of the property, or the default value if not set.</returns>
    /// <exception cref="ArgumentException">If the property name is null or whitespace.</exception>
    protected T GetValueOrDefault<T>(T defaultValue, [CallerMemberName] string? propertyName = null)
    {
        if (string.IsNullOrWhiteSpace(propertyName))
        {
            throw new ArgumentException("Invalid property name", propertyName);
        }

        return _values.TryGetValue(propertyName, out var value) && value is T tValue ? tValue : defaultValue;
    }

    /// <summary>
    ///     Sets the value of the specified property and raises the <see cref="PropertyChanged" /> event if the value changes.
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
            if (Logger?.IsEnabled(LogLevel.Trace) == true)
            {
                LogPropertyNotChanged(Logger, propertyName, this, value);
            }

            return false; // No change
        }

        _values.AddOrUpdate(propertyName, value, (_, _) => value);
        if (Logger?.IsEnabled(LogLevel.Debug) == true)
        {
            LogPropertyChanged(Logger,
                propertyName,
                this,
                existingValue,
                value);
        }

        OnPropertyChanged(propertyName);
        return true;
    }

    /// <summary>
    ///     Clears the value of the specified property and raises the <see cref="PropertyChanged" /> event.
    /// </summary>
    /// <param name="propertyName">The name of the property to clear. If not provided, the caller's member name is used.</param>
    /// <returns>True if the property was cleared; otherwise, false.</returns>
    /// <exception cref="ArgumentException">If the property name is null or whitespace.</exception>
    protected bool ClearValue([CallerMemberName] string? propertyName = null)
    {
        if (string.IsNullOrWhiteSpace(propertyName))
        {
            throw new ArgumentException("Invalid property name", propertyName);
        }

        if (_values.TryRemove(propertyName, out var removedValue))
        {
            if (Logger?.IsEnabled(LogLevel.Debug) == true)
            {
                LogPropertyCleared(Logger, propertyName, this, removedValue);
            }

            OnPropertyChanged(propertyName);
            return true;
        }

        if (Logger?.IsEnabled(LogLevel.Trace) == true)
        {
            LogPropertyClearAttempt(Logger, propertyName, this);
        }

        return false;
    }

    /// <summary>
    ///     Raises the <see cref="PropertyChanged" /> event for the specified property.
    /// </summary>
    /// <param name="propertyName">The name of the property that changed. If not provided, the caller's member name is used.</param>
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    /// <summary>
    ///     Waits asynchronously until the specified property equals the expected value, or until the timeout expires.
    /// </summary>
    /// <typeparam name="T">The type of the property value.</typeparam>
    /// <param name="propertyName">The name of the property to monitor.</param>
    /// <param name="expectedValue">The value to wait for.</param>
    /// <param name="timeout">The timeout for this operation</param>
    /// <param name="cancellationToken">A cancellation token to cancel this operation.</param>
    /// <returns>A task that completes when the property equals the expected value or the timeout expires.</returns>
    protected ValueTask<T> WaitForPropertyToBeOfValue<T>(string propertyName, T expectedValue, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        return WaitForPropertyToBe<T>(propertyName, arg => Equals(arg, expectedValue), timeout, cancellationToken);
    }

    /// <summary>
    ///     Waits asynchronously until the specified property does not equal the unwanted value, or until the timeout expires.
    /// </summary>
    /// <typeparam name="T">The type of the property value.</typeparam>
    /// <param name="propertyName">The name of the property to monitor.</param>
    /// <param name="unwantedValue">The value to avoid.</param>
    /// <param name="timeout">The timeout for this operation</param>
    /// <param name="cancellationToken">A cancellation token to cancel this operation.</param>
    /// <returns>A task that completes when the property does not equal the unwanted value or the timeout expires.</returns>
    protected ValueTask<T> WaitForPropertyToBeDifferentThanValue<T>(string propertyName, T unwantedValue, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        return WaitForPropertyToBe<T>(propertyName, arg => !Equals(arg, unwantedValue), timeout, cancellationToken);
    }

    /// <summary>
    ///     Waits asynchronously until the specified property satisfies the given condition, or until the timeout expires.
    /// </summary>
    /// <typeparam name="T">The type of the property value.</typeparam>
    /// <param name="propertyName">The name of the property to monitor.</param>
    /// <param name="condition">A function that determines whether the property value satisfies the condition.</param>
    /// <param name="timeout">The timeout for this operation</param>
    /// <param name="cancellationToken">A cancellation token to cancel this operation.</param>
    /// <returns>A task that completes when the property satisfies the condition or the timeout expires.</returns>
    /// <exception cref="ArgumentException">If the property name is null or whitespace.</exception>
    protected async ValueTask<T> WaitForPropertyToBe<T>(string propertyName, Func<T?, bool> condition, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(condition);
        var propertyInfo = GetPropertyInfo(this, propertyName);
        var propertyValue = GetPropertyValue<T>(this, propertyInfo);

        if (condition.Invoke(propertyValue))
        {
            return propertyValue!;
        }

        var tcs = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

        try
        {
            PropertyChanged += BindableObjectPropertyChanged;
            await tcs.Task.WaitBetterAsync(timeout, cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            PropertyChanged -= BindableObjectPropertyChanged; //this is needed in the case of a timeout
        }

        return GetPropertyValue<T>(this, propertyInfo)!;

        void BindableObjectPropertyChanged(object? obj, PropertyChangedEventArgs ea)
        {
            try
            {
                if (ea.PropertyName != propertyName)
                {
                    return;
                }

                var value = GetPropertyValue<T>(obj!, propertyInfo);
                if (!condition.Invoke(value))
                {
                    return;
                }

                PropertyChanged -= BindableObjectPropertyChanged;
                tcs.TrySetResult();
            }
            catch (Exception ex)
            {
                PropertyChanged -= BindableObjectPropertyChanged;
                tcs.TrySetException(ex);
            }
        }

        static PropertyInfo GetPropertyInfo(object obj, string propertyName)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(propertyName);
            ArgumentNullException.ThrowIfNull(obj);

            var propertyInfo = obj.GetType().GetProperty(propertyName);
            return propertyInfo == null ? throw new ArgumentException($"Property '{propertyName}' not found on type '{obj.GetType().FullName}'") : propertyInfo;
        }

        static TP? GetPropertyValue<TP>(object obj, PropertyInfo propertyInfo)
        {
            return (TP?) propertyInfo.GetValue(obj, null);
        }
    }

    /// <summary>
    ///     Waits asynchronously until the property value changes, or until the timeout expires.
    /// </summary>
    /// <typeparam name="T">The type of the property value.</typeparam>
    /// <param name="propertyName">The name of the property to monitor.</param>
    /// <param name="timeout">The timeout for this operation</param>
    /// <param name="cancellationToken">A cancellation token to cancel this operation.</param>
    /// <returns>A task that completes when the property satisfies the condition or the timeout expires.</returns>
    /// <exception cref="ArgumentException">If the property name is null or whitespace.</exception>
    protected async ValueTask<T?> WaitForPropertyToChange<T>(string propertyName, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        var propertyInfo = GetPropertyInfo(this, propertyName);
        var tcs = new TaskCompletionSource<T?>(TaskCreationOptions.RunContinuationsAsynchronously);

        try
        {
            PropertyChanged += BindableObjectPropertyChanged;
            return await tcs.Task.WaitBetterAsync(timeout, cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            PropertyChanged -= BindableObjectPropertyChanged; //this is needed in the case of a timeout
        }

        void BindableObjectPropertyChanged(object? obj, PropertyChangedEventArgs ea)
        {
            try
            {
                if (ea.PropertyName != propertyName)
                {
                    return;
                }

                var value = GetPropertyValue<T>(obj!, propertyInfo);

                PropertyChanged -= BindableObjectPropertyChanged;
                tcs.TrySetResult(value);
            }
            catch (Exception ex)
            {
                PropertyChanged -= BindableObjectPropertyChanged;
                tcs.TrySetException(ex);
            }
        }

        static PropertyInfo GetPropertyInfo(object obj, string propertyName)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(propertyName);
            ArgumentNullException.ThrowIfNull(obj);

            var propertyInfo = obj.GetType().GetProperty(propertyName);
            return propertyInfo == null ? throw new ArgumentException($"Property '{propertyName}' not found on type '{obj.GetType().FullName}'") : propertyInfo;
        }

        static Tp? GetPropertyValue<Tp>(object obj, PropertyInfo propertyInfo)
        {
            return (Tp?) propertyInfo.GetValue(obj, null);
        }
    }

    /// <summary>
    ///     Waits asynchronously until the property value changes to a new value, or until the timeout expires.
    /// </summary>
    /// <typeparam name="T">The type of the property value.</typeparam>
    /// <param name="propertyName">The name of the property to monitor.</param>
    /// <param name="timeout">The timeout for this operation</param>
    /// <param name="cancellationToken">A cancellation token to cancel this operation.</param>
    /// <returns>A task that completes when the property satisfies the condition or the timeout expires.</returns>
    /// <exception cref="ArgumentException">If the property name is null or whitespace.</exception>
    protected async ValueTask<T> WaitForPropertyToChangeNotNull<T>(string propertyName, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        var propertyInfo = GetPropertyInfo(this, propertyName);
        var tcs = new TaskCompletionSource<T>(TaskCreationOptions.RunContinuationsAsynchronously);

        try
        {
            PropertyChanged += BindableObjectPropertyChanged;
            return await tcs.Task.WaitBetterAsync(timeout, cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            PropertyChanged -= BindableObjectPropertyChanged; //this is needed in the case of a timeout
        }

        void BindableObjectPropertyChanged(object? obj, PropertyChangedEventArgs ea)
        {
            try
            {
                if (ea.PropertyName != propertyName)
                {
                    return;
                }

                var value = GetPropertyValue<T>(obj!, propertyInfo);
                if (value == null)
                {
                    return;
                }

                PropertyChanged -= BindableObjectPropertyChanged;
                tcs.TrySetResult(value);
            }
            catch (Exception ex)
            {
                PropertyChanged -= BindableObjectPropertyChanged;
                tcs.TrySetException(ex);
            }
        }

        static PropertyInfo GetPropertyInfo(object obj, string propertyName)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(propertyName);
            ArgumentNullException.ThrowIfNull(obj);

            var propertyInfo = obj.GetType().GetProperty(propertyName);
            return propertyInfo == null ? throw new ArgumentException($"Property '{propertyName}' not found on type '{obj.GetType().FullName}'") : propertyInfo;
        }

        static Tp? GetPropertyValue<Tp>(object obj, PropertyInfo propertyInfo)
        {
            return (Tp?) propertyInfo.GetValue(obj, null);
        }
    }

    #region Logging

    [LoggerMessage(Level = LogLevel.Trace, Message = "{Sender}.{PropertyName} : {Value} (no change)")]
    private static partial void LogPropertyNotChanged(ILogger logger, string propertyName, object sender, object? value);

    [LoggerMessage(Level = LogLevel.Debug, Message = "{Sender}.{PropertyName} : {OldValue} -> {NewValue}")]
    private static partial void LogPropertyChanged(ILogger logger,
        string propertyName,
        object sender,
        object? oldValue,
        object? newValue);

    [LoggerMessage(Level = LogLevel.Debug, Message = "{Sender}.{PropertyName} : {OldValue} -> (cleared)")]
    private static partial void LogPropertyCleared(ILogger logger, string propertyName, object sender, object? oldValue);

    [LoggerMessage(Level = LogLevel.Trace, Message = "{Sender}.{PropertyName} : (already cleared)")]
    private static partial void LogPropertyClearAttempt(ILogger logger, string propertyName, object sender);

    #endregion
}