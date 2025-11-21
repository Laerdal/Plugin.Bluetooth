using Plugin.BaseTypeExtensions;

namespace Plugin.Bluetooth.Maui;

public partial class BluetoothDevice
{
    public bool IsBonded => NativeDevice.BondState == Bond.Bonded;

    public bool IsBonding => BondingTcs is { Task.IsCompleted: false };

    public event EventHandler? Bonding;

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

    public virtual void OnPairingRequest(int pairingVariant, int? passKey)
    {
        // Placeholder for future implementation
    }

}
