namespace Bluetooth.Abstractions.Broadcasting.Options;

public record PermissionOptions
{
    /// <summary>
    ///     Gets the permission request strategy for this scanning operation.
    /// </summary>
    /// <remarks>
    ///     Defaults to <see cref="PermissionRequestStrategy.RequestAutomatically"/> which automatically
    ///     requests permissions before starting the scan if not already granted.
    /// </remarks>
    public PermissionRequestStrategy PermissionStrategy { get; init; } = PermissionRequestStrategy.RequestAutomatically;

}
