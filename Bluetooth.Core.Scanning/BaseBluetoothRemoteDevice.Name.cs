namespace Bluetooth.Core.Scanning;

public abstract partial class BaseBluetoothRemoteDevice
{
    /// <summary>
    ///     Waits for the device's advertised name to change.
    /// </summary>
    /// <param name="timeout">Optional timeout for the operation.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A task that completes when the advertised name changes.</returns>
    public ValueTask WaitForNameToChangeAsync(TimeSpan? timeout = null, CancellationToken cancellationToken = default)
    {
        return new ValueTask(WaitForPropertyToChange<string>(nameof(AdvertisedName), timeout, cancellationToken).AsTask());
    }

    /// <summary>
    ///     Gets the debug-friendly name of the device, including both advertised and cached names for logging purposes.
    /// </summary>
    public string DebugName
    {
        get => GetValue<string>("");
        private set => SetValue(value);
    }

    /// <summary>
    ///     Gets the name advertised by the device in its advertisement data.
    /// </summary>
    public string AdvertisedName
    {
        get => GetValue<string>("");
        private set
        {
            if (!SetValue(value))
            {
                return;
            }

            UpdateName();
        }
    }

    /// <inheritdoc />
    public string Name
    {
        get => GetValue("");
        private set => SetValue(value);
    }

    /// <summary>
    ///     Gets the cached name of the device, typically stored from previous connections or GATT reads.
    /// </summary>
    public string CachedName
    {
        get => GetValue<string>("");
        protected set
        {
            if (!SetValue(value))
            {
                return;
            }

            UpdateName();
        }
    }

    private void UpdateName()
    {
        var nameSb = new StringBuilder();
        var debugNameSb = new StringBuilder();

        if (!string.IsNullOrWhiteSpace(AdvertisedName))
        {
            nameSb.Append(AdvertisedName);
            debugNameSb.Append(AdvertisedName);
            if (!string.IsNullOrWhiteSpace(CachedName) && AdvertisedName != CachedName)
            {
                debugNameSb.Append(CultureInfo.InvariantCulture, $" (cached: {CachedName})");
            }
        }
        else if (!string.IsNullOrWhiteSpace(CachedName))
        {
            nameSb.Append(CachedName);
            debugNameSb.Append(CachedName);
        }

        Name = nameSb.ToString(); // Name is the name that is shown in the UI (AdvertisedName OR CachedName)
        DebugName = debugNameSb.ToString(); // DebugName is the name that is shown in the logs (AdvertisedName + CachedName)
    }
}
