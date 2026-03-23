namespace Bluetooth.Maui.Sample.Scanner.Views;

/// <summary>
///     Page for interactive read/write/listen experimentation on a selected characteristic.
/// </summary>
public partial class WriteListenLabPage : BaseContentPage<WriteListenLabViewModel>, IQueryAttributable
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="WriteListenLabPage" /> class.
    /// </summary>
    public WriteListenLabPage(WriteListenLabViewModel viewModel) : base(viewModel)
    {
        InitializeComponent();
    }

    /// <inheritdoc />
    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue("Characteristic", out var characteristicObj) &&
            characteristicObj is IBluetoothRemoteCharacteristic characteristic)
        {
            ViewModel.SetCharacteristic(characteristic);
        }
    }
}
