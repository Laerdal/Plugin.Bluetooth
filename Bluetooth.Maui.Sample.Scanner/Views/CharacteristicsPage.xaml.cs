namespace Bluetooth.Maui.Sample.Scanner.Views;

/// <summary>
///     Characteristics page displaying service details and GATT characteristics.
/// </summary>
public partial class CharacteristicsPage : BaseContentPage<CharacteristicsViewModel>, IQueryAttributable
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="CharacteristicsPage" /> class.
    /// </summary>
    /// <param name="viewModel">The characteristics view model.</param>
    public CharacteristicsPage(CharacteristicsViewModel viewModel) : base(viewModel)
    {
        InitializeComponent();
    }

    /// <summary>
    ///     Applies query attributes (navigation parameters) to the page.
    /// </summary>
    /// <param name="query">The query parameters.</param>
    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        // Get the service from navigation parameters
        if (query.TryGetValue("Service", out var serviceObj) &&
            serviceObj is IBluetoothRemoteService service)
        {
            ViewModel.SetService(service);
        }
    }
}
