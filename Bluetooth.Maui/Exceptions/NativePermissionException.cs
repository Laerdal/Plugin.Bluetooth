namespace Bluetooth.Maui.Exceptions;

/// <summary>
/// Exception thrown when a native platform permission is denied or unavailable.
/// This exception provides detailed information about the specific permission that failed
/// and its current status on the native platform.
/// </summary>
public class NativePermissionException : BluetoothException
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NativePermissionException"/> class.
    /// </summary>
    public NativePermissionException() : base("Permission denied")
    {
        PermissionKey = string.Empty;
        PermissionStatus = PermissionStatus.Unknown;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NativePermissionException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public NativePermissionException(string message) : base(message)
    {
        PermissionKey = string.Empty;
        PermissionStatus = PermissionStatus.Unknown;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NativePermissionException"/> class with a specified error message and inner exception.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public NativePermissionException(string message, Exception innerException) : base(message, innerException)
    {
        PermissionKey = string.Empty;
        PermissionStatus = PermissionStatus.Unknown;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="NativePermissionException"/> class with permission details.
    /// </summary>
    /// <param name="permissionKey">The identifier of the permission that was denied.</param>
    /// <param name="permissionStatus">The current status of the permission.</param>
    /// <param name="innerException">The exception that is the cause of the current exception, if any.</param>
    public NativePermissionException(string permissionKey, PermissionStatus permissionStatus, Exception? innerException = null)
        : base($"Permission {permissionKey} : {permissionStatus}", innerException)
    {
        PermissionKey = permissionKey;
        PermissionStatus = permissionStatus;
    }

    /// <summary>
    /// Gets the identifier of the permission that caused the exception.
    /// </summary>
    /// <value>
    /// A string that identifies the specific permission (e.g., "bluetooth", "location")
    /// that was denied or is unavailable.
    /// </value>
    public string PermissionKey { get; }

    /// <summary>
    /// Gets the current status of the permission that caused the exception.
    /// </summary>
    /// <value>
    /// A <see cref="PermissionStatus"/> value indicating whether the permission is granted,
    /// denied, disabled, or in an unknown state.
    /// </value>
    public PermissionStatus PermissionStatus { get; }
}
