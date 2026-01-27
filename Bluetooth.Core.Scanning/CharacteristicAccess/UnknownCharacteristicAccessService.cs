namespace Bluetooth.Core.Scanning.CharacteristicAccess;

/// <summary>
/// This is the default implementation of a CharacteristicAccessService for unknown characteristics.
/// </summary>
internal sealed class UnknownCharacteristicAccessService : CharacteristicAccessService
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UnknownCharacteristicAccessService"/> class.
    /// </summary>
    internal UnknownCharacteristicAccessService() : base(Guid.Empty, "Unknown Characteristic")
    {
    }
}
