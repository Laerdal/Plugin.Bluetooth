namespace Plugin.Bluetooth.BaseClasses;

public abstract partial class BaseBluetoothDevice
{
    /// <summary>
    /// Gets a value indicating whether service exploration is currently in progress.
    /// </summary>
    public bool IsExploringServices
    {
        get => GetValue(false);
        private set => SetValue(value);
    }

    private TaskCompletionSource? ServicesExplorationTcs
    {
        get => GetValue<TaskCompletionSource?>(null);
        set => SetValue(value);
    }

    /// <summary>
    /// Called when service exploration succeeds. Updates the Services collection and completes the exploration task.
    /// </summary>
    /// <typeparam name="TNativeServiceType">The platform-specific service type.</typeparam>
    /// <param name="services">The list of native services discovered.</param>
    /// <param name="fromInputTypeToOutputTypeConversion">Function to convert from native service type to IBluetoothService.</param>
    /// <param name="areRepresentingTheSameObject">Function to determine if a native service and IBluetoothService represent the same object.</param>
    protected void OnServicesExplorationSucceeded<TNativeServiceType>(IList<TNativeServiceType> services, Func<TNativeServiceType, IBluetoothService> fromInputTypeToOutputTypeConversion, Func<TNativeServiceType, IBluetoothService, bool> areRepresentingTheSameObject)
    {
        Services.UpdateFrom(services, areRepresentingTheSameObject, fromInputTypeToOutputTypeConversion);

        // Attempt to dispatch success to the TaskCompletionSource
        var success = ServicesExplorationTcs?.TrySetResult() ?? false;
        if (success)
        {
            return;
        }

        // Else throw an exception
        throw new UnexpectedServiceExplorationException(this);
    }

    /// <summary>
    /// Called when service exploration fails. Completes the exploration task with an exception or dispatches to the unhandled exception listener.
    /// </summary>
    /// <param name="e">The exception that occurred during service exploration.</param>
    protected void OnServicesExplorationFailed(Exception e)
    {
        // Attempt to dispatch exception to the TaskCompletionSource
        var success = ServicesExplorationTcs?.TrySetException(e) ?? false;
        if (success)
        {
            return;
        }

        // If the TaskCompletionSource was already completed, dispatch the exception to the listener
        BluetoothUnhandledExceptionListener.OnBluetoothUnhandledException(this, e);
    }

    /// <summary>
    /// Platform-specific implementation to explore services.
    /// </summary>
    /// <param name="timeout">Optional timeout for the operation.</param>
    /// <param name="cancellationToken">Optional cancellation token for the operation.</param>
    /// <returns>A ValueTask representing the asynchronous operation.</returns>
    protected abstract ValueTask NativeServicesExplorationAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default);

    /// <inheritdoc/>
    public async Task ExploreServicesAsync(bool clearBeforeExploring = false, bool exploreCharacteristicsToo = false, TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        // Check if services have already been explored
        if (Services.Any() && !clearBeforeExploring)
        {
            return;
        }

        // Prevents multiple calls to ReadValueAsync, if already exploring, we merge the calls
        if (ServicesExplorationTcs is { Task.IsCompleted: false })
        {
            await ServicesExplorationTcs.Task.ConfigureAwait(false);
            return;
        }

        ServicesExplorationTcs = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously); // Reset the TCS
        IsExploringServices = true; // Set the flag to true

        try // try-catch to dispatch exceptions rising from start
        {

            // Check if services need to be cleaned
            if (Services.Any() && clearBeforeExploring)
            {
                await ClearServicesAsync().ConfigureAwait(false);
            }

            // Ensure Device is Connected
            DeviceNotConnectedException.ThrowIfNotConnected(this);

            await NativeServicesExplorationAsync(timeout, cancellationToken).ConfigureAwait(false); // actual service exploration native call
        }
        catch (Exception e)
        {
            OnServicesExplorationFailed(e); // if exception is thrown during start, we trigger the failure
        }

        // try-finally to ensure disposal and release of resources
        try
        {
            // Wait for OnServicesExplorationSucceeded to be called
            await ServicesExplorationTcs.Task.WaitBetterAsync(timeout, cancellationToken).ConfigureAwait(false);

            if (exploreCharacteristicsToo) // EXPLORE CHARACTERISTICS IF NEEDED
            {
                foreach (var service in Services.ToList())
                {
                    await service.ExploreCharacteristicsAsync(false, timeout, cancellationToken).ConfigureAwait(false);
                }
            }
        }
        finally
        {
            IsExploringServices = false; // Reset the flag
            ServicesExplorationTcs = null;
        }
    }
}
