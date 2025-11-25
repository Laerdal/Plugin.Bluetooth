namespace Bluetooth.Core.Exceptions;

/// <summary>
///     Represents an exception that occurs in Bluetooth activity operations.
/// </summary>
/// <remarks>
///     This exception provides information about the Bluetooth activity associated with the error,
///     allowing for easier debugging and tracking of activity-related issues.
/// </remarks>
/// <seealso cref="IBluetoothActivity" />
public abstract class ActivityException : BluetoothException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="ActivityException"/> class.
    /// </summary>
    protected ActivityException()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="ActivityException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    protected ActivityException(string message) : base(message)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="ActivityException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    protected ActivityException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="ActivityException"/> class.
    /// </summary>
    /// <param name="activity">The Bluetooth activity associated with the exception.</param>
    /// <param name="message">A message that describes the error.</param>
    /// <param name="innerException">The inner exception that caused the current exception, if any.</param>
    protected ActivityException(
        IBluetoothActivity activity,
        string message = "Unknown BLE activity exception",
        Exception? innerException = null)
        : base(message, innerException)
    {
        Activity = activity;
    }

    /// <summary>
    ///     Gets the Bluetooth activity associated with the exception.
    /// </summary>
    public IBluetoothActivity? Activity { get; }
}

/// <summary>
///     Represents an exception that occurs when a Bluetooth activity fails to start.
/// </summary>
/// <seealso cref="ActivityException" />
public class ActivityFailedToStartException : ActivityException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="ActivityFailedToStartException"/> class.
    /// </summary>
    public ActivityFailedToStartException()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="ActivityFailedToStartException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public ActivityFailedToStartException(string message) : base(message)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="ActivityFailedToStartException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public ActivityFailedToStartException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="ActivityFailedToStartException"/> class.
    /// </summary>
    /// <param name="activity">The Bluetooth activity associated with the exception.</param>
    /// <param name="message">A message that describes the error.</param>
    /// <param name="innerException">The inner exception that caused the current exception, if any.</param>
    public ActivityFailedToStartException(
        IBluetoothActivity activity,
        string message = "Failed to start activity",
        Exception? innerException = null)
        : base(activity, message, innerException)
    {
    }
}

/// <summary>
///     Represents an exception that occurs when a Bluetooth activity is already started.
/// </summary>
/// <seealso cref="ActivityFailedToStartException" />
public class ActivityIsAlreadyStartedException : ActivityFailedToStartException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="ActivityIsAlreadyStartedException"/> class.
    /// </summary>
    public ActivityIsAlreadyStartedException()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="ActivityIsAlreadyStartedException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public ActivityIsAlreadyStartedException(string message) : base(message)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="ActivityIsAlreadyStartedException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public ActivityIsAlreadyStartedException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="ActivityIsAlreadyStartedException"/> class.
    /// </summary>
    /// <param name="activity">The Bluetooth activity associated with the exception.</param>
    /// <param name="message">A message that describes the error.</param>
    /// <param name="innerException">The inner exception that caused the current exception, if any.</param>
    public ActivityIsAlreadyStartedException(
        IBluetoothActivity activity,
        string message = "Activity is already started",
        Exception? innerException = null)
        : base(activity, message, innerException)
    {
    }

    /// <summary>
    ///     Throws an <see cref="ActivityIsAlreadyStartedException"/> if the Bluetooth activity is already started.
    /// </summary>
    /// <param name="activity">The Bluetooth activity to check.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="activity"/> is null.</exception>
    /// <exception cref="ActivityIsAlreadyStartedException">Thrown when the activity is already running.</exception>
    public static void ThrowIfIsStarted(IBluetoothActivity activity)
    {
        ArgumentNullException.ThrowIfNull(activity);
        if (activity.IsRunning)
        {
            throw new ActivityIsAlreadyStartedException(activity);
        }
    }
}

/// <summary>
///     Represents an exception that occurs when a Bluetooth activity starts unexpectedly.
/// </summary>
/// <seealso cref="ActivityException" />
public class ActivityUnexpectedStartException : ActivityException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="ActivityUnexpectedStartException"/> class.
    /// </summary>
    public ActivityUnexpectedStartException()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="ActivityUnexpectedStartException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public ActivityUnexpectedStartException(string message) : base(message)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="ActivityUnexpectedStartException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public ActivityUnexpectedStartException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="ActivityUnexpectedStartException"/> class.
    /// </summary>
    /// <param name="activity">The Bluetooth activity associated with the exception.</param>
    /// <param name="message">A message that describes the error.</param>
    /// <param name="innerException">The inner exception that caused the current exception, if any.</param>
    public ActivityUnexpectedStartException(
        IBluetoothActivity activity,
        string message = "Activity started unexpectedly",
        Exception? innerException = null)
        : base(activity, message, innerException)
    {
    }
}

/// <summary>
///     Represents an exception that occurs when a Bluetooth activity fails to stop.
/// </summary>
/// <seealso cref="ActivityException" />
public class ActivityFailedToStopException : ActivityException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="ActivityFailedToStopException"/> class.
    /// </summary>
    public ActivityFailedToStopException()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="ActivityFailedToStopException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public ActivityFailedToStopException(string message) : base(message)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="ActivityFailedToStopException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public ActivityFailedToStopException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="ActivityFailedToStopException"/> class.
    /// </summary>
    /// <param name="activity">The Bluetooth activity associated with the exception.</param>
    /// <param name="message">A message that describes the error.</param>
    /// <param name="innerException">The inner exception that caused the current exception, if any.</param>
    public ActivityFailedToStopException(
        IBluetoothActivity activity,
        string message = "Failed to stop activity",
        Exception? innerException = null)
        : base(activity, message, innerException)
    {
    }
}

/// <summary>
///     Represents an exception that occurs when a Bluetooth activity is already stopped.
/// </summary>
/// <seealso cref="ActivityFailedToStopException" />
public class ActivityIsAlreadyStoppedException : ActivityFailedToStopException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="ActivityIsAlreadyStoppedException"/> class.
    /// </summary>
    public ActivityIsAlreadyStoppedException()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="ActivityIsAlreadyStoppedException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public ActivityIsAlreadyStoppedException(string message) : base(message)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="ActivityIsAlreadyStoppedException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public ActivityIsAlreadyStoppedException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="ActivityIsAlreadyStoppedException"/> class.
    /// </summary>
    /// <param name="activity">The Bluetooth activity associated with the exception.</param>
    /// <param name="message">A message that describes the error.</param>
    /// <param name="innerException">The inner exception that caused the current exception, if any.</param>
    public ActivityIsAlreadyStoppedException(
        IBluetoothActivity activity,
        string message = "Activity is already stopped",
        Exception? innerException = null)
        : base(activity, message, innerException)
    {
    }

    /// <summary>
    ///     Throws an <see cref="ActivityIsAlreadyStoppedException"/> if the Bluetooth activity is already stopped.
    /// </summary>
    /// <param name="activity">The Bluetooth activity to check.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="activity"/> is null.</exception>
    /// <exception cref="ActivityIsAlreadyStoppedException">Thrown when the activity is already stopped.</exception>
    public static void ThrowIfIsStopped(IBluetoothActivity activity)
    {
        ArgumentNullException.ThrowIfNull(activity);
        if (!activity.IsRunning)
        {
            throw new ActivityIsAlreadyStoppedException(activity);
        }
    }
}

/// <summary>
///     Represents an exception that occurs when a Bluetooth activity stops unexpectedly.
/// </summary>
/// <seealso cref="ActivityException" />
public class ActivityUnexpectedStopException : ActivityException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="ActivityUnexpectedStopException"/> class.
    /// </summary>
    public ActivityUnexpectedStopException()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="ActivityUnexpectedStopException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public ActivityUnexpectedStopException(string message) : base(message)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="ActivityUnexpectedStopException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public ActivityUnexpectedStopException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="ActivityUnexpectedStopException"/> class.
    /// </summary>
    /// <param name="activity">The Bluetooth activity associated with the exception.</param>
    /// <param name="message">A message that describes the error.</param>
    /// <param name="innerException">The inner exception that caused the current exception, if any.</param>
    public ActivityUnexpectedStopException(
        IBluetoothActivity activity,
        string message = "Activity stopped unexpectedly",
        Exception? innerException = null)
        : base(activity, message, innerException)
    {
    }
}

/// <summary>
///     Represents an exception that occurs when Bluetooth is turned off.
/// </summary>
/// <seealso cref="ActivityException" />
public class BluetoothIsOffException : ActivityException
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="BluetoothIsOffException"/> class.
    /// </summary>
    public BluetoothIsOffException()
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="BluetoothIsOffException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public BluetoothIsOffException(string message) : base(message)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="BluetoothIsOffException"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public BluetoothIsOffException(string message, Exception innerException) : base(message, innerException)
    {
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="BluetoothIsOffException"/> class.
    /// </summary>
    /// <param name="activity">The Bluetooth activity associated with the exception.</param>
    /// <param name="message">A message that describes the error.</param>
    /// <param name="innerException">The inner exception that caused the current exception, if any.</param>
    public BluetoothIsOffException(
        IBluetoothActivity activity,
        string message = "Bluetooth is off",
        Exception? innerException = null)
        : base(activity, message, innerException)
    {
    }

    /// <summary>
    ///     Throws a <see cref="BluetoothIsOffException"/> if Bluetooth is turned off.
    /// </summary>
    /// <param name="activity">The Bluetooth activity to check.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="activity"/> is null.</exception>
    /// <exception cref="BluetoothIsOffException">Thrown when Bluetooth is turned off.</exception>
    public static void ThrowIfBluetoothIsOff(IBluetoothActivity activity)
    {
        ArgumentNullException.ThrowIfNull(activity);
        if (!activity.IsBluetoothOn)
        {
            throw new BluetoothIsOffException(activity);
        }
    }
}
