using Bluetooth.Abstractions.Scanning;
using Bluetooth.Core.Exceptions;
using Bluetooth.Core.Scanning.Exceptions;

using Plugin.BaseTypeExtensions;

namespace Bluetooth.Core.Scanning;

public abstract partial class BaseBluetoothService
{
    /// <summary>
    /// Gets a value indicating whether characteristic exploration is currently in progress.
    /// </summary>
    public bool IsExploringCharacteristics
    {
        get => GetValue(false);
        private set => SetValue(value);
    }

    /// <summary>
    /// Gets or sets the task completion source for characteristic exploration operations.
    /// </summary>
    /// <remarks>
    /// This is used to coordinate asynchronous characteristic exploration and ensure only one exploration occurs at a time.
    /// </remarks>
    private TaskCompletionSource? CharacteristicsExplorationTcs
    {
        get => GetValue<TaskCompletionSource?>(null);
        set => SetValue(value);
    }

    /// <summary>
    /// Called when characteristic exploration succeeds. Updates the Characteristics collection and completes the exploration task.
    /// </summary>
    /// <typeparam name="TNativeCharacteristicType">The platform-specific characteristic type.</typeparam>
    /// <param name="characteristics">The list of native characteristics discovered.</param>
    /// <param name="fromInputTypeToOutputTypeConversion">Function to convert from native characteristic type to IBluetoothCharacteristic.</param>
    /// <param name="areRepresentingTheSameObject">Function to determine if a native characteristic and IBluetoothCharacteristic represent the same object.</param>
    /// <exception cref="UnexpectedCharacteristicExplorationException">Thrown when the task completion source is not in the expected state.</exception>
    protected void OnCharacteristicsExplorationSucceeded<TNativeCharacteristicType>(IList<TNativeCharacteristicType> characteristics, Func<TNativeCharacteristicType, IBluetoothCharacteristic> fromInputTypeToOutputTypeConversion, Func<TNativeCharacteristicType, IBluetoothCharacteristic, bool> areRepresentingTheSameObject)
    {
        Characteristics.UpdateFrom(characteristics, areRepresentingTheSameObject, fromInputTypeToOutputTypeConversion);

        // Attempt to dispatch success to the TaskCompletionSource
        var success = CharacteristicsExplorationTcs?.TrySetResult() ?? false;
        if (success)
        {
            return;
        }

        // Else throw an exception
        throw new UnexpectedCharacteristicExplorationException(this);
    }

    /// <summary>
    /// Called when characteristic exploration fails. Completes the exploration task with an exception or dispatches to the unhandled exception listener.
    /// </summary>
    /// <param name="e">The exception that occurred during characteristic exploration.</param>
    /// <remarks>
    /// If the task completion source accepts the exception, it is propagated to waiting tasks.
    /// Otherwise, the exception is dispatched to the <see cref="BluetoothUnhandledExceptionListener"/>.
    /// </remarks>
    protected void OnCharacteristicsExplorationFailed(Exception e)
    {
        // Attempt to dispatch exception to the TaskCompletionSource
        var success = CharacteristicsExplorationTcs?.TrySetException(e) ?? false;
        if (success)
        {
            return;
        }

        // If the TaskCompletionSource was already completed, dispatch the exception to the listener
        BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, e);
    }

    /// <summary>
    /// Platform-specific implementation to explore characteristics.
    /// </summary>
    /// <param name="timeout">Optional timeout for the operation.</param>
    /// <param name="cancellationToken">Optional cancellation token for the operation.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    protected abstract ValueTask NativeCharacteristicsExplorationAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default);

    /// <inheritdoc/>
    /// <remarks>
    /// This method ensures the device is connected, prevents concurrent explorations,
    /// and optionally clears existing characteristics before exploring.
    /// </remarks>
    /// <exception cref="DeviceNotConnectedException">Thrown when the device is not connected.</exception>
    public async Task ExploreCharacteristicsAsync(bool clearBeforeExploring = false, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        // Check if characteristics have already been explored
        if (Characteristics.Any() && !clearBeforeExploring)
        {
            return;
        }

        // Prevents multiple calls to ReadValueAsync, if already exploring, we merge the calls
        if (CharacteristicsExplorationTcs is { Task.IsCompleted: false })
        {
            await CharacteristicsExplorationTcs.Task.ConfigureAwait(false);
            return;
        }

        CharacteristicsExplorationTcs = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously); // Reset the TCS
        IsExploringCharacteristics = true; // Set the flag to true

        try // try-catch to dispatch exceptions rising from start
        {

            // Check if characteristics need to be cleaned
            if (Characteristics.Any() && clearBeforeExploring)
            {
                await ClearCharacteristicsAsync().ConfigureAwait(false);
            }

            // Ensure Device is Connected
            DeviceNotConnectedException.ThrowIfNotConnected(this.Device);

            await NativeCharacteristicsExplorationAsync(timeout, cancellationToken).ConfigureAwait(false); // actual characteristic exploration native call
        }
        catch (Exception e)
        {
            OnCharacteristicsExplorationFailed(e); // if exception is thrown during start, we trigger the failure
        }

        // try-finally to ensure disposal and release of resources
        try
        {
            // Wait for OnCharacteristicsExplorationSucceeded to be called
            await CharacteristicsExplorationTcs.Task.WaitBetterAsync(timeout, cancellationToken).ConfigureAwait(false);
        }
        finally
        {
            IsExploringCharacteristics = false; // Reset the flag
            CharacteristicsExplorationTcs = null;
        }
    }
}
