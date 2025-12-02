using Plugin.BaseTypeExtensions;

namespace Bluetooth.Maui;

public partial class BluetoothDevice
{
    /// <summary>
    /// Gets a value indicating whether the device is currently bonded (paired) on Android.
    /// </summary>
    public bool IsBonded => NativeDevice.BondState == Bond.Bonded;

    /// <summary>
    /// Gets a value indicating whether a bonding (pairing) operation is currently in progress.
    /// </summary>
    public bool IsBonding => BondingTcs is { Task.IsCompleted: false };

    /// <summary>
    /// Occurs when a bonding operation starts.
    /// </summary>
    public event EventHandler? Bonding;

    /// <summary>
    /// Gets or sets the task completion source for the current bonding operation.
    /// Used to signal completion of asynchronous bonding operations.
    /// </summary>
    private TaskCompletionSource? BondingTcs
    {
        get;
        set
        {
            field = value;
            OnPropertyChanged(nameof(IsBonding));
            if (value is { Task.IsCompleted: false })
            {
                Bonding?.Invoke(this, System.EventArgs.Empty);
            }
        }
    }

    /// <summary>
    /// Initiates bonding (pairing) with the device asynchronously.
    /// </summary>
    /// <param name="timeout">The timeout for the bonding operation.</param>
    /// <param name="cancellationToken">Token to cancel the operation.</param>
    /// <returns>A task that completes when the device is bonded.</returns>
    /// <exception cref="InvalidOperationException">Thrown when a bonding operation is already in progress.</exception>
    /// <exception cref="BondingFailedException">Thrown when the CreateBond call fails or when bonding fails.</exception>
    /// <exception cref="System.OperationCanceledException">Thrown when the operation is cancelled via the cancellation token.</exception>
    /// <exception cref="TimeoutException">Thrown when the operation times out.</exception>
    public async Task BondAsync(TimeSpan timeout, CancellationToken cancellationToken = default)
    {
        if (IsBonded)
        {
            return;
        }

        if (IsBonding)
        {
            throw new InvalidOperationException("Bonding is already in progress");
        }

        BondingTcs = new TaskCompletionSource();

        try
        {
            if (!NativeDevice.CreateBond())
            {
                throw new BondingFailedException(this);
            }

            await BondingTcs.Task.WaitBetterAsync(timeout, cancellationToken).ConfigureAwait(false);
            await Task.Delay(timeout, cancellationToken).ConfigureAwait(false); // Wait for the bond state to be updated
        }
        finally
        {
            BondingTcs = null;
        }
    }

    /// <summary>
    /// Called when the bond state changes on the Android platform.
    /// </summary>
    /// <param name="previousBondState">The previous bond state.</param>
    /// <param name="bondState">The new bond state.</param>
    public void OnBondStateChanged(Bond previousBondState, Bond bondState)
    {
        OnPropertyChanged(nameof(IsBonded));
        switch (bondState)
        {
            case Bond.Bonded:
                BondingTcs?.TrySetResult();
                break;
            case Bond.Bonding:
                break;
            case Bond.None:
                BondingTcs?.TrySetException(new BondingFailedException(this));
                break;
        }
    }

    /// <summary>
    /// Called when a pairing request is received from the device.
    /// </summary>
    /// <param name="pairingVariant">The type of pairing being requested.</param>
    /// <param name="passKey">The passkey for pairing, if applicable.</param>
    /// <remarks>
    /// Placeholder for future implementation.
    /// </remarks>
    public virtual void OnPairingRequest(int pairingVariant, int? passKey)
    {
        // Placeholder for future implementation
    }

}
